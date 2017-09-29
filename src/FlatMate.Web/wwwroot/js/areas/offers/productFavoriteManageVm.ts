import { MarketJso } from ".";

export interface ProductFavoriteManageVm {
    markets: MarketJso[];
    currentMarket: number | undefined;
}
