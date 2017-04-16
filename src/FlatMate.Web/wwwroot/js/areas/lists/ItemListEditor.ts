import * as ko from "knockout";
import ItemListViewModel from "./itemListViewModel";
import ItemListModel from "./models/itemListModel";

export default class ItemListEditor {
    public model: ItemListModel;

    constructor() {
        this.model = this.readModel();
        const template = this.readTemplate();

        ko.components.register("item-list-editor", {
            template: template,
            viewModel: ItemListViewModel,
        });

        ko.applyBindings(this);
    }

    private readModel(): ItemListModel {
        const element = document.getElementById("view-data");
        if (!element) {
            throw "no data available";
        }

        return new ItemListModel(JSON.parse(element.innerText));
    }

    private readTemplate(): string {
        const element = document.getElementById("item-list-editor-template");
        if (!element) {
            throw "no template available";
        }

        return element.innerHTML;
    }
}
