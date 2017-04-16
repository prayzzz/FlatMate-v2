import { IItemGroupJso, IItemJso } from "jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";

export default class ItemGroupViewModel {
    public newItemName: KnockoutObservable<String>;
    public model: IItemGroupJso;
    public items: KnockoutObservableArray<IItemJso>;
    private apiClient: ApiClient;

    constructor(model: IItemGroupJso, items: IItemJso[]) {
        this.model = model;

        this.newItemName = ko.observable<String>();
        this.items = ko.observableArray(items);
        this.apiClient = new ApiClient();
    }

    public get name(): string {
        return this.model.name;
    }

    public get id(): number {
        return this.model.id;
    }

    public addItem = () => {
        const self = this;

        const groupName = self.newItemName().trim();

        if (!groupName || groupName === "") {
            return;
        }

        const itemToAdd = { name: groupName, sortIndex: 0 };
        const done = (i: IItemJso) => {
            self.items.push(i);
            self.newItemName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.itemListId}/group/${self.model.id}/item`, itemToAdd, done);
    }

    public removeItem = (item: IItemJso) => {
        const self = this;

        const done = () => { self.items.remove(item); };

        self.apiClient.delete(`lists/itemlist/${self.model.itemListId}/item/${item.id}`, done);
    }
}
