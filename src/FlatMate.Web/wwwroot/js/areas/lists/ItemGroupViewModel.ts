import * as ko from "knockout";

import { ItemViewModel, ItemGroupJso, ItemJso, ItemListApi } from ".";
import { DragZone, DragEvents, DragZoneData } from "../../shared/ko/";

export class ItemGroupViewModel {
    private readonly apiClient: ItemListApi;
    private readonly model = ko.observable<ItemGroupJso>();

    // from model
    public readonly name = ko.observable<string>("");
    public readonly sortIndex = ko.observable<number>(0);

    // ui
    public readonly isInEditMode = ko.observable<boolean>(false);
    public readonly isNewItemFocused = ko.observable<Boolean>(false);
    public readonly items = ko.observableArray<ItemViewModel>();
    public readonly newItemName = ko.observable<String>("");
    public readonly dragZone: DragZone<ItemViewModel>;

    constructor(model: ItemGroupJso, items?: ItemJso[]) {
        this.model.subscribe(val => {
            this.name(val.name);
            this.sortIndex(val.sortIndex);
        })

        this.model(model);

        if (items) {
            this.items(items.map(i => new ItemViewModel(i)));
            this.items.sort((a, b) => a.sortIndex() - b.sortIndex());
        }

        this.apiClient = new ItemListApi();
        this.dragZone = new DragZone<ItemViewModel>(this.dragZoneName, this.dragStart, this.dragEnd);
    }

    private get dragZoneName(): string {
        return `group-${this.id}`;
    }

    public get id(): number | undefined {
        return this.model().id;
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

    public leaveEditMode = (): boolean => {
        if (!this.isInEditMode()) {
            // prevent multiple updates
            return false;
        }

        this.isInEditMode(false);

        if (this.name() !== this.model().name) {
            this.save();
        }

        return false; // dont continue event propagation
    }

    public enterEditMode = (): boolean => {
        this.isInEditMode(true);

        return false; // dont continue event propagation
    }

    /**
     * Creates an new item from the itemName textfield and saves it.
     */
    public addItem = () => {
        const itemName = this.newItemName().trim();
        if (!itemName) {
            return;
        }

        let maxSortIndex = -1;
        this.items().forEach(i => maxSortIndex = i.sortIndex() > maxSortIndex ? i.sortIndex() : maxSortIndex);

        const itemVm = new ItemViewModel(new ItemJso(this.model().itemListId, this.model().id));
        itemVm.name(itemName);
        itemVm.sortIndex(maxSortIndex + 1);
        itemVm.save().then(() => {
            this.items.push(itemVm);
            this.newItemName("");
        });
    }

    /**
     * Removes the given item from the group
     */
    public removeItem = (item: ItemViewModel) => {
        const self = this;
        item.delete().then(() => self.items.remove(item));
    }

    /**
     * Sets the sortindex for all items and saves them
     */
    public resetSortIndex() {
        for (let i = 0; i < this.items().length; i++) {
            const item = this.items()[i];

            if (item.sortIndex() !== i) {
                item.sortIndex(i);
                item.save();
            }
        }
    }

    /**
     * Saves this group
     */
    public async save(): Promise<void> {
        const model = this.model();
        const oldModel = Object.clone(model)

        model.name = this.name();
        model.sortIndex = this.sortIndex();

        try {
            if (model.id) {
                this.model(await this.apiClient.updateGroup(model.itemListId, model.id, model))
            }
            else {
                this.model(await this.apiClient.createGroup(model.itemListId, model));
            }
        }
        catch (err) {
            this.model(oldModel)
        }
    }

    /**
     * Deletes the this group
     */
    public delete(): Promise<void> {
        const model = this.model();
        if (model.id) {
            return this.apiClient.deleteGroup(model.itemListId, model.id);
        }

        return new Promise<void>((resolve, reject) => resolve());
    }
}