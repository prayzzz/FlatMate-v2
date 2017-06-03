import { IDraggable } from ".";

export class DragZoneData<T extends IDraggable> {
    public readonly item: T;
    public readonly items: KnockoutObservableArray<T>;

    constructor(item: T, items: KnockoutObservableArray<T>) {
        this.item = item;
        this.items = items;
    }
}
