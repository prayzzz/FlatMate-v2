"use strict";

namespace Flatmate.Areas.Lists {

    interface IItemJso {
        id: number;
        lastEditor: IUserJso;
        name: string;
        owner: IUserJso;
        parentItemId: number | null;
        sortIndex: number;
    }

    interface IUserJso {
        id: number;
        name: string;
    }

    interface IItemListJso {
        description: string;
        id: number;
        isPublic: boolean;
        itemCount: number;
        items: Array<IItemJso>;
        lastEditor: IUserJso;
        name: string;
        owner: IUserJso;
    }

    class ItemListModel {
        description: string;
        groups: KnockoutObservableArray<ItemGroupModel>;
        id: number;
        isPublic: boolean;
        lastEditor: IUserJso;
        name: string;
        owner: IUserJso;

        constructor(data: IItemListJso) {
            this.description = data.description;
            this.groups = ko.observableArray<ItemGroupModel>();
            this.id = data.id;
            this.isPublic = data.isPublic;
            this.lastEditor = data.lastEditor;
            this.name = data.name;
            this.owner = data.owner;

            data.items
                .filter(g => g.parentItemId == null)
                .forEach(g => {
                    const items = data.items.filter(i => i.parentItemId === g.id);
                    this.groups.push(new ItemGroupModel(g, items));
                });

        }
    }

    class ItemGroupModel {
        id: number;
        lastEditor: IUserJso;
        items: Array<ItemModel>;
        name: KnockoutObservable<string>;
        owner: IUserJso;
        sortIndex: KnockoutObservable<number>;

        constructor(data: IItemJso, items: IItemJso[]) {
            this.id = data.id;
            this.lastEditor = data.lastEditor;
            this.items = new Array();
            this.name = ko.observable(data.name);
            this.owner = data.owner;
            this.sortIndex = ko.observable(data.sortIndex);

            items.forEach(i => this.items.push(new ItemModel(i)));
        }
    }

    class ItemModel {
        id: number;
        lastEditor: IUserJso;
        name: KnockoutObservable<string>;
        owner: IUserJso;
        sortIndex: KnockoutObservable<number>;

        constructor(data: IItemJso) {
            this.id = data.id;
            this.lastEditor = data.lastEditor;
            this.name = ko.observable(data.name);
            this.owner = data.owner;
            this.sortIndex = ko.observable(data.sortIndex);
        }
    }

    export class ItemListEditorViewModel {
        model: ItemListModel;

        constructor(params: any) {
            this.model = params.model;
        }
    }

    export class ItemListEditor {
        model: ItemListModel;

        constructor() {
            this.model = this.readModel();
            const template = this.readTemplate();

            ko.components.register("item-list-editor", {
                viewModel: ItemListEditorViewModel,
                template: template
            });

            ko.applyBindings(this);
        }

        private readModel(): ItemListModel {
            const element = document.getElementById("view-data");
            if (!element) {
                throw "no data available";
            }

            return new ItemListModel(JSON.parse(element.innerText));
        }

        private readTemplate(): string {
            const element = document.getElementById("item-list-editor-template");
            if (!element) {
                throw "no template available";
            }

            return element.innerHTML;
        }
    }
}