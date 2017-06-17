import * as ko from "knockout";
import { ItemListEditor, ItemListJso } from ".";
import { StartupHelper } from "../../koStartup";

export class ListsRegistry {
    public static registerComponents() {
        // ItemListEditor
        ko.components.register("item-list-editor", {
            template: { element: "item-list-editor-template" },
            viewModel: function() {
                return new ItemListEditor(StartupHelper.readModel<ItemListJso>());
            }
        });
    }
}
