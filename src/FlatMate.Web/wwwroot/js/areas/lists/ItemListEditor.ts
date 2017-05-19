import * as ko from "knockout";
import { ItemListJso, ItemListViewModel } from ".";

export class ItemListEditor {
    public model: ItemListJso;

    constructor() {
        this.model = this.readModel();

        ko.components.register("item-list-editor", {
            template: this.readTemplate(),
            viewModel: ItemListViewModel,
        });

        ko.applyBindings(this);
    }

    private readModel(): ItemListJso {
        const element = document.getElementById("view-data");
        if (!element || !element.innerText.trim()) {
            throw "no data available";
        }

        return <ItemListJso>JSON.parse(element.innerText);
    }

    private readTemplate(): string {
        const element = document.getElementById("item-list-editor-template");
        if (!element) {
            throw "no template available";
        }

        return element.innerHTML;
    }
}
