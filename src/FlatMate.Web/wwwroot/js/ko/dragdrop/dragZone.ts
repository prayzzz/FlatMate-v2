import { IDraggable } from ".";

export class DragZone<T extends IDraggable> {
    public readonly name: string;
    public readonly dragStart: (data: T, event: Event) => void;
    public readonly dragEnd: (data: T) => void;

    constructor(name: string, dragStart: (data: T, event: Event) => void, dragEnd: (data: T) => void) {
        this.name = name;
        this.dragStart = dragStart;
        this.dragEnd = dragEnd;
    }
}
