import * as ko from "knockout";
import { ApiClient } from "../../api/apiClient";

interface IItemJso {
    id: number;
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
    itemGroupId: number | null;
    itemListId: number;
    sortIndex: number;
}

interface IItemGroupJso {
    id: number;
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
    itemListId: number;
    sortIndex: number;
}

interface IUserInfoJso {
    id: number;
    name: string;
}

interface IItemListJso {
    description: string;
    id: number;
    isPublic: boolean;
    itemCount: number;
    items: Array<IItemJso>;
    itemGroups: Array<IItemGroupJso>;
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
}

class ItemListModel {
    public description: string;
    public groups: KnockoutObservableArray<ItemGroupModel>;
    public id: number;
    public isPublic: boolean;
    public items: KnockoutObservableArray<ItemModel>;
    public lastEditor: IUserInfoJso;
    public name: string;
    public owner: IUserInfoJso;

    constructor(data: IItemListJso) {
        this.description = data.description;
        this.groups = ko.observableArray<ItemGroupModel>();
        this.id = data.id;
        this.isPublic = data.isPublic;
        this.items = ko.observableArray<ItemGroupModel>();
        this.lastEditor = data.lastEditor;
        this.name = data.name;
        this.owner = data.owner;

        data.itemGroups
            .forEach(g => {
                const items = data.items.filter(i => i.itemGroupId === g.id);
                this.groups.push(new ItemGroupModel(g, items));
            });

        data.items
            .filter(i => i.itemGroupId == null)
            .forEach(i => this.items.push(new ItemModel(i)));
    }
}

class ItemGroupModel {
    public id: number;
    public lastEditor: IUserInfoJso;
    public items: Array<ItemModel>;
    public name: KnockoutObservable<string>;
    public owner: IUserInfoJso;
    public sortIndex: KnockoutObservable<number>;

    constructor(data: IItemGroupJso, items: IItemJso[] = new Array()) {
        this.id = data.id;
        this.lastEditor = data.lastEditor;
        this.items = new Array();
        this.name = ko.observable(data.name);
        this.owner = data.owner;
        this.sortIndex = ko.observable(data.sortIndex);

        items.forEach(i => this.items.push(new ItemModel(i)));
    }

    public get itemsSorted(): ItemModel[] {
        return this.items.sort((a, b) => a.sortIndex() - b.sortIndex());
    }
}

class ItemModel {
    public id: number;
    public lastEditor: IUserInfoJso;
    public name: KnockoutObservable<string>;
    public owner: IUserInfoJso;
    public sortIndex: KnockoutObservable<number>;

    constructor(data: IItemJso) {
        this.id = data.id;
        this.lastEditor = data.lastEditor;
        this.name = ko.observable(data.name);
        this.owner = data.owner;
        this.sortIndex = ko.observable(data.sortIndex);
    }
}

export class ItemListEditorViewModel {
    private apiClient: ApiClient;
    public model: ItemListModel;
    public newGroupName: KnockoutObservable<String>;

    constructor(params: any) {
        this.model = params.model;

        this.newGroupName = ko.observable<String>();

        this.apiClient = new ApiClient();
    }

    public addGroup(): void {
        const groupName = this.newGroupName();

        if (!groupName) {
            return;
        }

        var groupToAdd = { name: groupName, sortIndex: 0 };
        var done = (g: IItemGroupJso) => {
            this.model.groups.push(new ItemGroupModel(g));
            this.newGroupName("");
        }

        this.apiClient.post(`lists/itemlist/${this.model.id}/group`, groupToAdd, done);
    }
}

export class ItemListEditor {
    public model: ItemListModel;

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
