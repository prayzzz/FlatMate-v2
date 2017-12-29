import {OfferVm} from "./CompanyOffersList";

export class OfferTile {
    private readonly model: OfferVm;

    constructor(params: any) {
        this.model = params.offer;
    }

    public get id() {
        return this.model.id;
    }

    public get isFavorite() {
        return this.model.isFavorite;
    }

    public get isStartingLater() {
        return this.model.isStartingLater;
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