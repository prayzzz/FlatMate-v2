import * as ko from "knockout";
import { StartupHelper } from "../../koStartup";
import { ProductFavoriteEditor, ProductFavoriteManageVm } from "./ProductFavoriteEditor";
import { CompanyOffersList, CompanyOffersListData as CompanyOffersListVm } from "./CompanyOffersList";
import { OfferTile } from "./OfferTile";

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
        ko.components.register("offer-tile", {
            template: {element: "offer-tile-template"},
            viewModel: function (params) {
                return new OfferTile(params);
            }
        });
    }
}
