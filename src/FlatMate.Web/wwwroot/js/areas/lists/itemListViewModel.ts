import { IItemGroupJso, IItemJso } from "jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";
import ItemGroupModel from "./models/itemGroupModel";
import ItemListModel from "./models/itemListModel";
import ItemModel from "./models/itemModel";

export default class ItemListViewModel {
    public newGroupName: KnockoutObservable<String>;
    public newItemName: KnockoutObservable<String>;
    public model: ItemListModel;
    private apiClient: ApiClient;

    constructor(params: any) {
        this.model = params.model;

        this.newGroupName = ko.observable<String>();
        this.newItemName = ko.observable<String>();
        this.apiClient = new ApiClient();
    }

    public addItem = (group: ItemGroupModel) => {
        const self = this;

        const itemName = self.newItemName().trim();

        if (!itemName || itemName === "") {
            return;
        }

        const itemToAdd = { name: itemName, sortIndex: 0 };
        const done = (i: IItemJso) => {
            group.items.push(new ItemModel(i));
            self.newItemName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.id}/group/${group.id}/item`, itemToAdd, done);
    }

    public addGroup = () => {
        const self = this;

        const groupName = self.newGroupName().trim();

        if (!groupName || groupName === "") {
            return;
        }

        const groupToAdd = { name: groupName, sortIndex: 0 };
        const done = (g: IItemGroupJso) => {
            self.model.groups.push(new ItemGroupModel(g));
            self.newGroupName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.id}/group`, groupToAdd, done);
    }

    public removeGroup = (group: ItemGroupModel) => {
        const self = this;

        const done = () => { self.model.groups.remove(group); };

        self.apiClient.delete(`lists/itemlist/${self.model.id}/group/${group.id}`, done);
    }
}
