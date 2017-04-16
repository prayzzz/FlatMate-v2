import { IItemGroupJso, IItemJso } from "jso";
import * as ko from "knockout";
import { IUserInfoJso } from "../../account/jso";
import ItemModel from "./itemModel";

export default class ItemGroupModel {
    public id: number;
    public lastEditor: IUserInfoJso;
    public items: KnockoutObservableArray<ItemModel>;
    public itemListId: number;
    public name: KnockoutObservable<string>;
    public owner: IUserInfoJso;
    public sortIndex: KnockoutObservable<number>;

    constructor(data: IItemGroupJso, items: IItemJso[] = new Array()) {
        this.id = data.id;
        this.lastEditor = data.lastEditor;
        this.items = ko.observableArray<ItemModel>();
        this.itemListId = data.itemListId;
        this.name = ko.observable(data.name);
        this.owner = data.owner;
        this.sortIndex = ko.observable(data.sortIndex);

        items.forEach(i => this.items.push(new ItemModel(i)));
    }

    public get itemsSorted(): ItemModel[] {
        return this.items().sort((a, b) => a.sortIndex() - b.sortIndex());
    }
}
