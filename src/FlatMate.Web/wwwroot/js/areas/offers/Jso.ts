// tslint:disable:max-classes-per-file

export class CompanyJso {
    public id: number;
    public imageGuid: string;
    public imageLink: string;
    public name: string;
}

export class MarketJso {
    public city: string;
    public company: CompanyJso;
    public id: number;
    public name: string;
    public postalCode: string;
    public street: string;
}

export class ProductCategoryJso {
    public id: number;
    public name: string;
    public sortWeight: number;
}

export class ProductJso {
    public brand: string;
    public companyId: number;
    public description: string;
    public externalId: string;
    public id: number;
    public imageUrl: string;
    public name: string;
    public price: number;
    public productCategory: ProductCategoryJso;
    public sizeInfo: string;
}

export interface OfferInMarket {
    offerId: number;
    price: number;
    marketId: number;
    from: string;
    to: string;
}

export interface OfferedProduct {
    name: string;
    imageUrl: string;
    offers: OfferInMarket[];
    productId: number;
    productCategoryId: number;
}

export interface ProductCategoryWithOffers {
    products: OfferedProduct[];
    name: string;
}