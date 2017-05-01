import { IItemListJso, IItemGroupJso } from "jso";
import * as ko from "knockout";
import ApiClient from "../../api/apiClient";
import ItemGroupViewModel from "./itemGroupViewModel";

interface IViewModelParams {
    model: IItemListJso;
}

export default class ItemListViewModel {
    public newGroupName: KnockoutObservable<String>;
    public model: IItemListJso;
    public groups: KnockoutObservableArray<ItemGroupViewModel>;
    private apiClient: ApiClient;

    constructor(params: IViewModelParams) {
        this.model = params.model;

        this.newGroupName = ko.observable<String>();
        this.apiClient = new ApiClient();
        this.groups = ko.observableArray<ItemGroupViewModel>();

        this.groups(this.model.itemGroups.map(g => {
            const items = this.model.items.filter(i => i.itemGroupId === g.id);
            return new ItemGroupViewModel(g, items);
        }));
    }

    public addGroup = () => {
        const self = this;

        const groupName = self.newGroupName().trim();

        if (!groupName || groupName === "") {
            return;
        }

        let maxSortIndex = -1;
        self.groups().forEach(g => maxSortIndex = g.sortIndex > maxSortIndex ? g.sortIndex : maxSortIndex);

        const groupToAdd = { name: groupName, sortIndex: maxSortIndex + 1 };
        const done = (g: IItemGroupJso) => {
            const group = new ItemGroupViewModel(g, []);
            group.isNewItemFocused(true);

            self.groups.push(group);
            self.newGroupName("");
        };

        self.apiClient.post(`lists/itemlist/${self.model.id}/group`, groupToAdd, done);
    }

    public removeGroup = (group: ItemGroupViewModel) => {
        const self = this;

        const done = () => { self.groups.remove(group); };

        self.apiClient.delete(`lists/itemlist/${self.model.id}/group/${group.id}`, done);
    }
}
