import { CompanyJso, MarketJso, OfferJso, ProductCategoryJso } from ".";
import * as dateFormat from "dateformat";
import { ProductJso } from "./Jso";
import { IStringDictionary } from "../../shared/types";

export interface CompanyOffersListData {
    company: CompanyJso;
    offers: OfferJso[];
    offersFrom: string;
    offersTo: string;
    productCategories: ProductCategoryJso[];
    favorites: OfferJso[];
    markets: MarketJso[];
}

export class OfferedProduct {
    public product: ProductJso;
    public offerMarkets: OfferMarketPrice[];

    /**
     * @param {[]} offers Offers regarding one product
     */
    constructor(offers: OfferJso[]) {
        if (offers.length == 0) {
            throw "Cannot construct without offers"
        }

        if (offers.filter(o => o.product.id != offers[0].product.id).length > 0) {
            throw "Offers for different products"
        }

        this.product = offers[0].product;
        this.offerMarkets = [];

        for (let offer of offers) {
            this.offerMarkets.push(new OfferMarketPrice(offer))
        }
    }

    public get name() {
        return this.product.name;
    }
}

export class OfferMarketPrice {
    public id: number;
    public price: number;
    public marketId: number;
    public from: Date;
    public to: Date;

    constructor(offer: OfferJso) {
        this.id = offer.id;
        this.price = offer.price;
        this.marketId = offer.marketId;
        this.from = new Date(offer.from);
        this.to = new Date(offer.to);
    }
}

export class CategoryToOffers {
    constructor(public category: ProductCategoryJso,
                public offers: OfferedProduct[]) {
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

    public get marketId() {
        return this.model.marketId;
    }

    public get name() {
        return this.model.product.name;
    }

    public get price() {
        return this.model.price;
    }

    public get from() {
        return new Date(this.model.from);
    }

    public get to() {
        return new Date(this.model.to);
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
    private readonly model: CompanyOffersListData;

    // @ts-ignore: used by view
    private readonly categoryToOffers: CategoryToOffers[];
    // @ts-ignore: used by view
    private readonly favorites: OfferedProduct[];

    constructor(model: CompanyOffersListData) {
        this.model = model;

        // let offerVms = this.model.offers.map(o => new OfferVm(o));
        // offerVms.forEach(x => {
        //     x.isFavorite = (this.model.favorites.find(f => f.id === x.id) != undefined);
        //     // x.isStartingLater = (this.offersFrom !== x.from);
        // });
        this.categoryToOffers = this.groupOffersInCategory(this.model.offers);

        this.favorites = [];
        let productToFavOffers = this.groupBy(this.model.favorites, o => o.product.id);
        for (let productId in productToFavOffers) {
            this.favorites.push(new OfferedProduct(productToFavOffers[productId]))
        }
    }

    public get offersFrom(): string {
        return dateFormat(this.model.offersFrom, "dd.mm.yyyy")
    }

    public get offersTo(): string {
        return dateFormat(this.model.offersTo, "dd.mm.yyyy")
    }

    private get productFavoriteLink(): string {
        return `/Offers/ProductFavorite/Manage?companyId=${this.model.company.id}`;
    }

    private groupOffersInCategory(offerVms: OfferJso[]): CategoryToOffers[] {
        let groupedByProduct = this.groupBy(offerVms, o => o.product.id);

        const groupedOffers: CategoryToOffers[] = this.model.productCategories.sort((a, b) => b.sortWeight - a.sortWeight)
            .map(cat => new CategoryToOffers(cat, []));

        for (let productId in groupedByProduct) {
            if (!groupedByProduct.hasOwnProperty(productId)) {
                continue;
            }

            const offerCategory = groupedOffers.find(g => g.category.id == groupedByProduct[productId][0].product.productCategory.id);
            if (offerCategory) {
                offerCategory.offers.push(new OfferedProduct(groupedByProduct[productId]));
            }
        }
        return groupedOffers;
    }

    private groupBy<T>(list: T[], prop: (el: T) => any): IStringDictionary<T[]> {
        return list.reduce((prevReduceValue, x) => {
            let groupedProperty = prop(x);
            if (prevReduceValue[groupedProperty] === undefined) {
                prevReduceValue[groupedProperty] = [];
            }
            prevReduceValue[groupedProperty].push(x);
            return prevReduceValue;
        }, <IStringDictionary<T[]>>{});
    };
}