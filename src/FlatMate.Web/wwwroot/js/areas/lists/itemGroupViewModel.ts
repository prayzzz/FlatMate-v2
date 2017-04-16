import { IItemGroupJso } from "jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";
import ItemGroupModel from "./models/itemGroupModel";
import ItemModel from "./models/itemModel";

export default class ItemGroupViewModel {
    public newItemName: KnockoutObservable<String>;
    public model: ItemGroupModel;
    private apiClient: ApiClient;

    constructor(params: any) {
        this.model = params.model;

        this.newItemName = ko.observable<String>();

        this.apiClient = new ApiClient();
    }

    public addItem = () => {
        const self = this;

        const groupName = self.newItemName().trim();

        if (!groupName || groupName === "") {
            return;
        }

        const itemToAdd = { name: groupName, sortIndex: 0 };
        const done = (g: IItemGroupJso) => {
            self.model.items.push(new ItemGroupModel(g));
            self.newItemName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.itemListId}/group/${self.model.id}/item`, itemToAdd, done);
    }

    public removeItem = (item: ItemModel) => {
        const self = this;

        const done = () => { self.model.items.remove(item); };

        self.apiClient.delete(`lists/itemlist/${self.model.itemListId}/group/${self.model.id}/item/${item.id}`, done);
    }
}
