import * as ko from "knockout";
import { IItemJso, ItemListApi } from ".";
import { IDraggable } from "../../shared/ko/";

export class ItemViewModel implements IDraggable {
    private readonly apiClient: ItemListApi;
    private model: IItemJso;

    public readonly name: KnockoutObservable<string>;
    public readonly isInEditMode: KnockoutObservable<boolean>;
    public readonly isDragging: KnockoutObservable<boolean>;

    constructor(model: IItemJso) {
        this.model = model;

        this.apiClient = new ItemListApi();
        this.name = ko.observable(this.model.name)
        this.isDragging = ko.observable(false);
        this.isInEditMode = ko.observable(false);
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

    public leaveEditMode = (): boolean => {
        this.isInEditMode(false);

        if (this.name() !== this.model.name) {
            this.model.name = this.name();

            this.save();
        }

        return false;
    }

    public enterEditMode = (): boolean => {
        this.isInEditMode(true);

        return false;
    }

    public save(): Promise<IItemJso> {
        const self = this;

        return this.apiClient.updateItem(self.model.itemListId, self.model.id, self.model).then((d: IItemJso) => self.model = d)
    }
}