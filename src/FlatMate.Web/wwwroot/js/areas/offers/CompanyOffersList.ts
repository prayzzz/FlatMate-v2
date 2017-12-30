import { CompanyJso, MarketJso, OfferJso, ProductCategoryJso } from ".";

export interface CompanyOffersListVm {
    company: CompanyJso;
    offers: OfferJso[];
    offersFrom: Date;
    offersTo: Date;
    productCategories: ProductCategoryJso[];
    favorites: OfferJso[];
    markets: MarketJso[];
}

export class CategoryToOffers {
    constructor(public category: ProductCategoryJso,
                public offers: OfferVm[]) {
    }
}

export class OfferVm {
    public isFavorite: boolean;
    public isStartingLater: boolean;
    private readonly model: OfferJso;

    constructor(model: OfferJso) {
        this.model = model;

        this.isFavorite = false;
        this.isStartingLater = false;
    }

    public get id() {
        return this.model.id;
    }

    public get product() {
        return this.model.product;
    }

    public get name() {
        return this.model.product.name;
    }

    public get model_() {
        return this.model;
    }

    public get price() {
        return this.model.price;
    }

    public get from() {
        return new Date(this.model.from);
    }

    public get imageUrl() {
        if (!this.model.product.companyId) {
            return this.model.imageUrl;
        }

        switch (this.model.product.companyId) {
            case 1:
                return `${this.model.imageUrl}?resize=150px:150px`;
            case 2:
                return this.model.imageUrl.replace("/1080/", "/312/");
            default:
                return this.model.imageUrl;
        }
    }

    public get productUrl() {
        return `/Offers/Product/View/${this.model.product.id}`;
    }
}

export class CompanyOffersList {
    private readonly model: CompanyOffersListVm;

    // @ts-ignore: used by view
    private readonly categoryToOffers: CategoryToOffers[];
    // @ts-ignore: used by view
    private readonly favorites: OfferVm[];
    // @ts-ignore: used by view
    private readonly offersFrom: Date;
    // @ts-ignore: used by view
    private readonly offersTo: Date;

    constructor(model: CompanyOffersListVm) {
        this.model = model;

        this.offersFrom = new Date(this.model.offersFrom);
        this.offersTo = new Date(this.model.offersTo);

        let offerVms = this.model.offers.map(o => new OfferVm(o));
        offerVms.forEach(x => {
            x.isFavorite = (this.model.favorites.find(f => f.id === x.id) != undefined);
            x.isStartingLater = (this.offersFrom.getTime() !== x.from.getTime());
        });
        this.categoryToOffers = this.groupOffersInCategory(offerVms);
        this.favorites = this.model.favorites.map(o => new OfferVm(o));
    }

    private get productFavoriteLink(): string {
        return `/Offers/ProductFavorite/Manage?companyId=${this.model.company.id}`;
    }

    private groupOffersInCategory(offerVms: OfferVm[]): CategoryToOffers[] {
        const groupedOffers: CategoryToOffers[] = this.model.productCategories.sort((a, b) => b.sortWeight - a.sortWeight)
            .map(cat => new CategoryToOffers(cat, []));

        for (let offer of offerVms) {
            const offerCategory = groupedOffers.find(g => g.category.id == offer.model_.product.productCategory.id);
            if (offerCategory) {
                offerCategory.offers.push(offer);
            }
        }

        return groupedOffers;
    }
}