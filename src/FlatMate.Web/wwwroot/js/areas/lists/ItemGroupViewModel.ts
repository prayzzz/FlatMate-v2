import { IItemGroupJso, IItemJso } from "./jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";
import ItemViewModel from "./itemViewModel";
import DragZone from "../../shared/ko/dragzone";
import DragEvents from "../../shared/ko/dragEvents";
import DragZoneData from "../../shared/ko/dragZoneData";

export default class ItemGroupViewModel {
    private readonly apiClient: ApiClient;
    private readonly model: IItemGroupJso;

    public readonly isNewItemFocused: KnockoutObservable<Boolean>;
    public readonly items: KnockoutObservableArray<ItemViewModel>;
    public readonly newItemName: KnockoutObservable<String>;
    public readonly dragZone: DragZone<ItemViewModel>;

    constructor(model: IItemGroupJso, items: IItemJso[]) {
        this.model = model;
        this.items = ko.observableArray<ItemViewModel>(items.map(i => new ItemViewModel(i))).sort((a, b) => a.sortIndex - b.sortIndex);

        this.apiClient = new ApiClient();
        this.newItemName = ko.observable("");
        this.isNewItemFocused = ko.observable(false);
        this.dragZone = new DragZone<ItemViewModel>(this.dragZoneName, this.dragStart, this.dragEnd);
    }

    private get dragZoneName(): string {
        return `group-${this.id}`;
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

    public dragEvents = (item: ItemViewModel): DragEvents<ItemViewModel> => {
        return new DragEvents<ItemViewModel>(this.dragZoneName, this.reorder, new DragZoneData<ItemViewModel>(item, this.items));
    }

    public dragStart = (item: ItemViewModel) => {
        item.isDragging(true);
    };

    public dragEnd = (item: ItemViewModel) => {
        item.isDragging(false);

        this.resetSortIndex();
    };

    public reorder = (event: MouseEvent, dragData: ItemViewModel, zoneData: DragZoneData<ItemViewModel>) => {
        if (dragData !== zoneData.item) {
            const zoneDataIndex = zoneData.items.indexOf(zoneData.item);
            zoneData.items.remove(dragData);
            zoneData.items.splice(zoneDataIndex, 0, dragData);
        }
    };

    public resetSortIndex() {
        const self = this;

        for (let i = 0; i < this.items().length; i++) {
            const item = this.items()[i];

            if (item.sortIndex === i) {
                continue;
            }
            
            const itemToUpdate = { name: item.name, sortIndex: i };
            self.apiClient.put(`lists/itemlist/${self.model.itemListId}/group/${self.model.id}/item/${item.id}`, itemToUpdate);
        }
    }

    public addItem = () => {
        const self = this;

        const itemName = self.newItemName().trim();

        if (!itemName) {
            return;
        }

        let maxSortIndex = -1;
        self.items().forEach(i => maxSortIndex = i.sortIndex > maxSortIndex ? i.sortIndex : maxSortIndex);

        const itemToAdd = { name: itemName, sortIndex: maxSortIndex + 1 };
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
