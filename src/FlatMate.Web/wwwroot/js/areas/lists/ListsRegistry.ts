import * as ko from "knockout";
import { StartupHelper } from "../../KoStartup"
import { ItemListJso, ItemListViewModel } from ".";

export class ListsRegistry {
    public static registerComponents() {
        // ItemListEditor
        ko.components.register("item-list-editor", {
            viewModel: function () { return new ItemListViewModel({ model: StartupHelper.readModel<ItemListJso>() }) },
            template: { element: "item-list-editor-template" }
        });
    }
}
