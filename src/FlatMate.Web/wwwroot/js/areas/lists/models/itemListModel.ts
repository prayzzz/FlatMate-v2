import { IItemListJso } from "jso";
import * as ko from "knockout";
import { IUserInfoJso } from "../../account/jso";
import ItemGroupModel from "./itemGroupModel";
import ItemModel from "./itemModel";

export default class ItemListModel {
    public description: string;
    public groups: KnockoutObservableArray<ItemGroupModel>;
    public id: number;
    public isPublic: boolean;
    public items: KnockoutObservableArray<ItemModel>;
    public lastEditor: IUserInfoJso;
    public name: string;
    public owner: IUserInfoJso;

    constructor(data: IItemListJso) {
        this.description = data.description;
        this.groups = ko.observableArray<ItemGroupModel>();
        this.id = data.id;
        this.isPublic = data.isPublic;
        this.items = ko.observableArray<ItemGroupModel>();
        this.lastEditor = data.lastEditor;
        this.name = data.name;
        this.owner = data.owner;

        data.itemGroups
            .forEach(g => {
                const items = data.items.filter(i => i.itemGroupId === g.id);
                this.groups.push(new ItemGroupModel(g, items));
            });

        data.items
            .filter(i => i.itemGroupId == null)
            .forEach(i => this.items.push(new ItemModel(i)));
    }
}
