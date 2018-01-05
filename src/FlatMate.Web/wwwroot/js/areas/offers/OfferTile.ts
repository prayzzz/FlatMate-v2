import { OfferedProduct } from "./CompanyOffersList";

export class OfferTile {
    private readonly model: OfferedProduct;

    constructor(params: any) {
        this.model = params.offer;
    }

    public get isFavorite() {
        // return this.model.isFavorite;
        return false;
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
        return this.model.product.imageUrl;
    }

    public get productUrl() {
        return `/Offers/Product/View/${this.model.product.id}`;
    }
}