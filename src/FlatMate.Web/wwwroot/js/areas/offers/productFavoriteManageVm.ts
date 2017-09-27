import { MarketJso } from ".";

export interface ProductFavoriteManageVm {
    markets: Array<MarketJso>;
    currentMarket: number | undefined;
}