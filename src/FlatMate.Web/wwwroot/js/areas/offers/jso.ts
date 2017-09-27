export class CompanyJso {
    id: number;
    imageGuid: string;
    imageLink: string;
    name: string;
}

export class MarketJso {
    city: string;
    company: CompanyJso;
    id: number;
    name: string;
    postalCode: string;
    street: string;
}

export class ProductCategoryJso {
    id: number;
    name: string;
    sortWeight: number;
}

export class ProductJso {
    brand: string;
    description: string;
    externalId: string;
    id: number;
    imageUrl: string;
    name: string;
    price: number;
    productCategory: ProductCategoryJso;
    sizeInfo: string;
}