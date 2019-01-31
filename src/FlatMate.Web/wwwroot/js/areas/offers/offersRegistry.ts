import * as ko from "knockout";
import { StartupHelper } from "../../koStartup";
import { ProductFavoriteEditor, ProductFavoriteManageVm } from "./productFavoriteEditor";
import { MarketOffersView as CompanyOffersList, MarketOffersVm as CompanyOffersListVm } from "./marketOffersView";
import { OfferedProductTile as OfferTile } from "./offeredProductTile";

export class OffersRegistry {
    public static registerComponents() {
        ko.components.register("product-favorite-editor", {
            template: {element: "product-favorite-editor-template"},
            viewModel: function () {
                return new ProductFavoriteEditor(StartupHelper.readViewData<ProductFavoriteManageVm>("product-favorite-manage"));
            }
        });
        ko.components.register("company-offers-list", {
            template: {element: "company-offers-list-template"},
            viewModel: function () {
                return new CompanyOffersList(StartupHelper.readViewData<CompanyOffersListVm>("company-offers-list"));
            }
        });
        ko.components.register("offered-product-tile", {
            template: {element: "offered-product-tile-template"},
            viewModel: function (params) {
                return new OfferTile(params);
            }
        });
    }
}
