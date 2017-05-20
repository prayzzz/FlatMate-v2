import * as ko from "knockout";
import { StartupHelper } from "../../KoStartup"
import { ItemListJso, ItemListEditor } from ".";

export class ListsRegistry {
    public static registerComponents() {
        // ItemListEditor
        ko.components.register("item-list-editor", {
            viewModel: function () { return new ItemListEditor(StartupHelper.readModel<ItemListJso>()) },
            template: { element: "item-list-editor-template" }
        });
    }
}
