import * as ko from "knockout";
import { StartupHelper } from "../../koStartup";
import { ProductFavoriteEditor, ProductFavoriteManageVm } from "./ProductFavoriteEditor";

export class OffersRegistry {
    public static registerComponents() {
        ko.components.register("product-favorite-editor", {
            template: { element: "product-favorite-editor-template" },
            viewModel: function () {
                return new ProductFavoriteEditor(StartupHelper.readViewData<ProductFavoriteManageVm>("product-favorite-manage"));
            }
        });
    }
}
