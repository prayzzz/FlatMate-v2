import * as dateFormat from "dateformat";
import { CompanyJso, MarketJso, OfferedProduct, ProductCategoryWithOffers } from "./jso";

export interface MarketOffersVm {
    company: CompanyJso;
    offerCount: number;
    offersFrom: string;
    offersTo: string;
    categories: ProductCategoryWithOffers[];
    favoriteProducts: OfferedProduct[];
    markets: MarketJso[]
}

export class OfferedProductModel {
    public readonly product: OfferedProduct;
    public readonly company: CompanyJso;
    public readonly markets: MarketJso[];
    public readonly offersFrom: string;

    constructor(product: OfferedProduct, company: CompanyJso, markets: MarketJso[], offersFrom: string) {
        this.product = product;
        this.company = company;
        this.markets = markets;
        this.offersFrom = offersFrom;
    }
}

export class MarketOffersView {
    public readonly categories: ProductCategoryWithOffers[];
    public readonly company: CompanyJso;
    public readonly favorites: OfferedProduct[];
    public readonly markets: MarketJso[];
    public readonly offerCount: number;
    public readonly offersFrom: string;
    public readonly offersFromFormatted: string;
    public readonly offersToFormatted: string;

    constructor(model: MarketOffersVm) {
        this.categories = model.categories;
        this.company = model.company;
        this.favorites = model.favoriteProducts;
        this.markets = model.markets;
        this.offerCount = model.offerCount;
        this.offersFrom = model.offersFrom;
        this.offersFromFormatted = dateFormat(model.offersFrom, "dd.mm.yyyy");
        this.offersToFormatted = dateFormat(model.offersTo, "dd.mm.yyyy");
    }

    public getOfferedProductModel(offeredProduct: OfferedProduct): OfferedProductModel {
        return new OfferedProductModel(offeredProduct, this.company, this.markets, this.offersFrom)
    }
}