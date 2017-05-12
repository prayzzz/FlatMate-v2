import { IItemGroupJso, IItemJso } from "./jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";
import ItemViewModel from "./itemViewModel";

export default class ItemGroupViewModel {
    private readonly apiClient: ApiClient;
    private readonly model: IItemGroupJso;

    public isNewItemFocused: KnockoutObservable<Boolean>;
    public items: KnockoutObservableArray<ItemViewModel>;
    public newItemName: KnockoutObservable<String>;

    constructor(model: IItemGroupJso, items: IItemJso[]) {
        this.model = model;
        this.items = ko.observableArray<ItemViewModel>(items.map(i => new ItemViewModel(i)));

        this.apiClient = new ApiClient();
        this.newItemName = ko.observable("");
        this.isNewItemFocused = ko.observable(false);
    }

    public get name(): string {
        return this.model.name;
    }

    public get sortIndex(): number {
        return this.model.sortIndex;
    }

    public get id(): number {
        return this.model.id;
    }

    public dragStart = (item: ItemViewModel) => {
        item.dragging(true);
    };

    public dragEnd = (item: ItemViewModel) => {
        item.dragging(false);
    };

    public reorder = (event: any, dragData: any, zoneData: any) => {
        if (dragData !== zoneData.item) {
            var zoneDataIndex = zoneData.items.indexOf(zoneData.item);
            zoneData.items.remove(dragData);
            zoneData.items.splice(zoneDataIndex, 0, dragData);
        }
    };

    public addItem = () => {
        const self = this;

        const groupName = self.newItemName().trim();

        if (!groupName) {
            return;
        }

        let maxSortIndex = -1;
        self.items().forEach(i => maxSortIndex = i.sortIndex > maxSortIndex ? i.sortIndex : maxSortIndex);

        const itemToAdd = { name: groupName, sortIndex: maxSortIndex + 1 };
        const done = (i: IItemJso) => {
            self.items.push(new ItemViewModel(i));
            self.newItemName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.itemListId}/group/${self.model.id}/item`, itemToAdd, done);
    }

    public removeItem = (item: ItemViewModel) => {
        const self = this;

        const done = () => { self.items.remove(item); };

        self.apiClient.delete(`lists/itemlist/${self.model.itemListId}/item/${item.id}`, done);
    }
}
