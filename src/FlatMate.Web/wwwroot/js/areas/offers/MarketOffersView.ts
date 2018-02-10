import * as dateFormat from "dateformat";
import {IDictionary} from "../../shared/types";
import {CompanyJso, MarketJso, OfferedProduct, ProductCategoryWithOffers} from "./Jso";

export interface MarketOffersVm {
    company: CompanyJso;
    offerCount: number;
    offersFrom: string;
    offersTo: string;
    categories: ProductCategoryWithOffers[];
    favoriteProducts: OfferedProduct[];
    markets: IDictionary<MarketJso>
}

export class OfferedProductModel {
    public readonly product: OfferedProduct;
    public readonly company: CompanyJso;
    public readonly markets: IDictionary<MarketJso>;
    public readonly offersFrom: string;

    constructor(product: OfferedProduct, company: CompanyJso, markets: IDictionary<MarketJso>, offersFrom: string) {
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
    public readonly markets: IDictionary<MarketJso>;
    public readonly offerCount: number;
    public readonly offersFrom: string;
    public readonly offersFromFormatted: string;
    public readonly offersToFormatted: string;
    public readonly productFavoriteLink: string;

    constructor(model: MarketOffersVm) {
        this.categories = model.categories;
        this.company = model.company;
        this.favorites = model.favoriteProducts;
        this.markets = model.markets;
        this.offerCount = model.offerCount;
        this.offersFrom = model.offersFrom;
        this.offersFromFormatted = dateFormat(model.offersFrom, "dd.mm.yyyy");
        this.offersToFormatted = dateFormat(model.offersTo, "dd.mm.yyyy");
        this.productFavoriteLink = `/Offers/ProductFavorite/Manage?companyId=${this.company.id}`;
    }

    public getOfferedProductModel(offeredProduct: OfferedProduct): OfferedProductModel {
        return new OfferedProductModel(offeredProduct, this.company, this.markets, this.offersFrom)
    }
}