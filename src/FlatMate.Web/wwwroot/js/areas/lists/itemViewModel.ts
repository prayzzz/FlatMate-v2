import { ItemJso, ItemListApi } from ".";
import { FlatMate } from "../../app";
import { IDragItem } from "./listActionManager";
import { observable } from "knockout";

export class ItemViewModel implements IDragItem {
    // from model
    public readonly name = observable("");
    public readonly sortIndex = observable(0);

    // ui
    public readonly isInEditMode = observable(false);
    public readonly isRemoveLoading = observable(false);

    private readonly apiClient = new ItemListApi();
    private readonly model = observable<ItemJso>();

    constructor(model: ItemJso) {
        this.model.subscribe(val => {
            this.name(val.name);
            this.sortIndex(val.sortIndex);
        });

        this.model(model);
    }

    public get id(): number | undefined {
        return this.model().id;
    }

    public get lastEditor(): string {
        return this.model().lastEditor.userName;
    }

    public get isLastEditorVisible(): boolean {
        return this.model().lastEditor.id != FlatMate.currentUser.id;
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
    };

    public enterEditMode = (): boolean => {
        this.isInEditMode(true);

        return false; // dont continue event propagation
    };

    /**
     * Saves this group
     */
    public async save(): Promise<void> {
        const model = this.model();
        const oldModel = Object.clone(model);

        model.name = this.name();
        model.sortIndex = this.sortIndex();

        try {
            if (model.id) {
                this.model(await this.apiClient.updateItem(model.itemListId, model.id, model));
            } else {
                this.model(await this.apiClient.createItem(model.itemListId, model.itemGroupId, model));
            }
        } catch (err) {
            this.model(oldModel);
        }
    }

    /**
     * Deletes this item
     */
    public delete(): Promise<void> {
        const self = this;
        const model = this.model();

        if (model.id) {
            self.isRemoveLoading(true);

            return this.apiClient.deleteItem(model.itemListId, model.id).then(
                () => {
                    self.isRemoveLoading(false);
                },
                err => {
                    self.isRemoveLoading(false);
                    throw err;
                }
            );
        }

        return new Promise<void>((resolve, reject) => resolve());
    }

    dragEnd(): void {
    }

    dragEnter(): void {
    }

    dragLeave(): void {
    }

    dragStart(): void {
    }
}
