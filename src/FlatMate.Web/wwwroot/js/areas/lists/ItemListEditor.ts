import * as ko from "knockout";
import { IItemListJso, ItemListViewModel } from ".";

export class ItemListEditor {
    public model: IItemListJso;

    constructor() {
        this.model = this.readModel();

        ko.components.register("item-list-editor", {
            template: this.readTemplate(),
            viewModel: ItemListViewModel,
        });

        ko.applyBindings(this);
    }

    private readModel(): IItemListJso {
        const element = document.getElementById("view-data");
        if (!element) {
            throw "no data available";
        }

        return <IItemListJso>JSON.parse(element.innerText);
    }

    private readTemplate(): string {
        const element = document.getElementById("item-list-editor-template");
        if (!element) {
            throw "no template available";
        }

        return element.innerHTML;
    }
}
