import * as ko from "knockout";
import { IItemJso } from "./jso";
import { IDraggable } from "../../shared/ko/IDraggable";

export default class ItemViewModel implements IDraggable {
    private readonly model: IItemJso;

    public isDragging: KnockoutObservable<boolean>;

    constructor(model: IItemJso) {
        this.model = model;

        this.isDragging = ko.observable(false);
    }

    public get name(): string {
        return this.model.name;
    }

    public get sortIndex(): number {
        return this.model.sortIndex;
    }

    public set sortIndex(n: number) {
        this.model.sortIndex = n;
    }

    public get id(): number {
        return this.model.id;
    }
}