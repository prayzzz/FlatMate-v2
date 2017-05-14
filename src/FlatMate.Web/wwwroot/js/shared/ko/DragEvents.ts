import DragZoneData from "./DragZoneData";
import { IDraggable } from "../../shared/ko/IDraggable";

export default class DragEvents<T extends IDraggable> {
    public readonly accepts: string;
    public readonly dragOver: (event: MouseEvent, dragData: T, zoneData: DragZoneData<T>) => void;
    public readonly data: DragZoneData<T>;

    constructor(accepts: string, dragOver: (event: MouseEvent, dragData: T, zoneData: DragZoneData<T>) => void, data: DragZoneData<T>) {
        this.accepts = accepts;
        this.dragOver = dragOver;
        this.data = data;
    }
}