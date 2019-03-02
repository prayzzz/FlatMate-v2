import { ItemListEditor, ItemListJso } from ".";
import { StartupHelper } from "../../koStartup";
import { components } from "knockout";

export class ListsRegistry {
    public static registerComponents() {
        // ItemListEditor
        components.register("item-list-editor", {
            template: { element: "item-list-editor-template" },
            viewModel: function() {
                return new ItemListEditor(StartupHelper.readViewData<ItemListJso>("itemlist"));
            }
        });
    }
}
