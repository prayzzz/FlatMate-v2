import * as ko from "knockout";
import { ProductFavoriteEditor, ProductFavoriteManageVm } from ".";
import { StartupHelper } from "../../koStartup";

export class OffersRegistry {
    public static registerComponents() {
        ko.components.register("product-favorite-editor", {
            template: { element: "product-favorite-editor-template" },
            viewModel: function() {
                return new ProductFavoriteEditor(StartupHelper.readViewData<ProductFavoriteManageVm>("product-favorite-manage"));
            }
        });
    }
}
