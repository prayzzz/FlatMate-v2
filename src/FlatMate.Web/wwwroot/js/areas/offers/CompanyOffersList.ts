import { CompanyJso, MarketJso, OfferJso, ProductCategoryJso } from ".";
import * as dateFormat from "dateformat";
import { ProductJso } from "./Jso";

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
    public markets: MarketJso[];

    /**
     * @param {[]} offers Offers regarding one product
     */
    constructor(offers: OfferJso[], markets: MarketJso[]) {
        if (offers.length == 0) {
            throw "Cannot construct without offers"
        }

        if (offers.filter(o => o.product.id != offers[0].product.id).length > 0) {
            throw "Offers for different products"
        }

        this.markets = markets;
        this.product = offers[0].product;
        this.offerMarkets = [];

        for (let offer of offers) {
            const market = markets.filter(m => m.id == offer.marketId)[0];
            this.offerMarkets.push(new OfferMarketPrice(offer, market))
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
    public market: MarketJso;
    public from: Date;
    public to: Date;

    constructor(offer: OfferJso, market: MarketJso) {
        this.id = offer.id;
        this.price = offer.price;
        this.marketId = offer.marketId;
        this.market = market;
        this.from = new Date(offer.from);
        this.to = new Date(offer.to);
    }
}

export class CategoryToOffers {
    constructor(public category: ProductCategoryJso,
                public offers: OfferedProduct[]) {
    }
}

export class CompanyOffersList {
    public readonly categoryToOffers: CategoryToOffers[];
    public readonly company: CompanyJso;
    public readonly favorites: OfferedProduct[];
    public readonly markets: MarketJso[];
    public readonly offerCount: number;
    public readonly offersFrom: string;
    public readonly offersTo: string;
    public readonly productCategories: ProductCategoryJso[];
    public readonly productFavoriteLink: string;

    constructor(model: CompanyOffersListData) {
        this.company = model.company;
        this.markets = model.markets;
        this.offersFrom = dateFormat(model.offersFrom, "dd.mm.yyyy");
        this.offersTo = dateFormat(model.offersTo, "dd.mm.yyyy");
        this.productCategories = model.productCategories;
        this.productFavoriteLink = `/Offers/ProductFavorite/Manage?companyId=${this.company.id}`;

        // let offerVms = this.model.offers.map(o => new OfferVm(o));
        // offerVms.forEach(x => {
        //     x.isFavorite = (this.model.favorites.find(f => f.id === x.id) != undefined);
        //     // x.isStartingLater = (this.offersFrom !== x.from);
        // });
        this.categoryToOffers = this.groupOffersInCategory(model.offers);
        this.offerCount = this.categoryToOffers.reduce((prev: number, categoryOffers: CategoryToOffers) => prev + categoryOffers.offers.length, 0);

        this.favorites = [];
        let productToFavOffers = model.favorites.groupBy((offer: OfferJso) => offer.product.id);
        for (let productId in productToFavOffers) {
            this.favorites.push(new OfferedProduct(productToFavOffers[productId], this.markets))
        }
    }

    private groupOffersInCategory(offers: OfferJso[]): CategoryToOffers[] {
        let groupedByProduct = offers.groupBy((offer: OfferJso) => offer.product.id);

        const groupedOffers: CategoryToOffers[] = this.productCategories.sort((a, b) => b.sortWeight - a.sortWeight)
            .map(cat => new CategoryToOffers(cat, []));

        for (let productId in groupedByProduct) {
            if (!groupedByProduct.hasOwnProperty(productId)) {
                continue;
            }

            const offerCategory = groupedOffers.find(g => g.category.id == groupedByProduct[productId][0].product.productCategory.id);
            if (offerCategory) {
                offerCategory.offers.push(new OfferedProduct(groupedByProduct[productId], this.markets));
            }
        }
        return groupedOffers;
    }
}