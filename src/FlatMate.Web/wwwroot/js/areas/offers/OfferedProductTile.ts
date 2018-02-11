import {OfferedProductModel} from "./MarketOffersView";
import {MarketJso, OfferInMarket} from "./Jso";
import * as dateFormat from "dateformat";

export class OfferedProductTile {
    public readonly name: string;
    public readonly offers: OfferInMarket[];
    public readonly price: number;
    public readonly offerFrom: string;
    public readonly isStartingLater: boolean;
    public readonly isEverywhereAvailable: boolean;
    public readonly productUrl: string;
    public readonly imageUrl: string;
    private readonly model: OfferedProductModel;

    constructor(params: any) {
        this.model = params.product;

        this.name = this.model.product.name;
        this.offers = this.model.product.offers;
        this.price = this.model.product.offers[0].price;
        this.offerFrom = dateFormat(this.model.product.offers[0].from, "ddd dd.mm.yyyy");
        this.isStartingLater = this.model.offersFrom != this.model.product.offers[0].from;
        this.isEverywhereAvailable = Object.keys(this.model.markets).length == this.model.product.offers.length;
        this.productUrl = `/Offers/Product/View/${this.model.product.productId}`;
        this.imageUrl = OfferedProductTile.calculateImageUrl(this.model);
    }

    private static calculateImageUrl(model: OfferedProductModel) {
        if (!model.company.id) {
            return model.product.imageUrl;
        }

        switch (model.company.id) {
            case 1:
                return `${model.product.imageUrl}?resize=150px:150px`;
            case 2:
                return model.product.imageUrl.replace("/1080/", "/312/");
            default:
                return model.product.imageUrl;
        }
    }

    public getMarketName(marketId: number): MarketJso | undefined {
        return this.model.markets.find(m => m.id == marketId);
    }
}