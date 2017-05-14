import { IDraggable } from "./IDraggable";

export default class DragZone<T extends IDraggable> {
    public readonly name: string;
    public readonly dragStart: (data: T) => void;
    public readonly dragEnd: (data: T) => void;

    constructor(name: string, dragStart: (data: T) => void, dragEnd: (data: T) => void) {
        this.name = name;
        this.dragStart = dragStart;
        this.dragEnd = dragEnd;
    }
}