import { OfferedProduct } from "./CompanyOffersList";

export class OfferTile {
    private readonly model: OfferedProduct;

    constructor(params: any) {
        this.model = params.offer;
    }

    public get isStartingLater() {
        // return this.model.isStartingLater;
        return false;
    }

    public get name() {
        return this.model.product.name;
    }

    public get model_() {
        return this.model;
    }

    public get price() {
        return this.model.offerMarkets[0].price;
    }

    public get from() {
        return this.model.offerMarkets[0].from;
    }


    public get imageUrl() {
        if (!this.model.product.companyId) {
            return this.model.product.imageUrl;
        }

        switch (this.model.product.companyId) {
            case 1:
                return `${this.model.product.imageUrl}?resize=150px:150px`;
            case 2:
                return this.model.product.imageUrl.replace("/1080/", "/312/");
            default:
                return this.model.product.imageUrl;
        }
    }

    public get productUrl() {
        return `/Offers/Product/View/${this.model.product.id}`;
    }
}