import { IItemJso } from "./jso";
import * as ko from "knockout";

interface IDraggable {
    dragging: KnockoutObservable<boolean>;
}

export default class ItemViewModel implements IDraggable {
    private readonly model: IItemJso;

    public dragging: KnockoutObservable<boolean>;

    constructor(model: IItemJso) {
        this.model = model;

        this.dragging = ko.observable(false);
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
}