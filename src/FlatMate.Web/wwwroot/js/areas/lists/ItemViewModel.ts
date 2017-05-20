﻿import * as ko from "knockout";
import { ItemJso, ItemListApi } from ".";
import { IDraggable } from "../../ko/dragdrop";

export class ItemViewModel implements IDraggable {
    private readonly apiClient: ItemListApi;
    private readonly model = ko.observable<ItemJso>();

    // from model
    public readonly name = ko.observable<string>();
    public readonly sortIndex = ko.observable<number>();

    // ui
    public readonly isInEditMode = ko.observable<boolean>(false);
    public readonly isDragging = ko.observable<boolean>(false);

    constructor(model: ItemJso) {
        this.model.subscribe(val => {
            this.name(val.name);
            this.sortIndex(val.sortIndex);
        })

        this.model(model);

        this.apiClient = new ItemListApi();
        this.isDragging = ko.observable(false);
        this.isInEditMode = ko.observable(false);
    }

    public get id(): number | undefined {
        return this.model().id;
    }

    public leaveEditMode = (): boolean => {
        // prevent multiple updates
        if (!this.isInEditMode()) {
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
     * Saves this group
     */
    public async save(): Promise<void> {
        const model = this.model();
        const oldModel = Object.clone(model)

        model.name = this.name();
        model.sortIndex = this.sortIndex();

        try {
            if (model.id) {
                this.model(await this.apiClient.updateItem(model.itemListId, model.id, model))
            }
            else {
                this.model(await this.apiClient.createItem(model.itemListId, model.itemGroupId, model));
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
            return this.apiClient.deleteItem(model.itemListId, model.id);
        }

        return new Promise<void>((resolve, reject) => resolve());
    }
}