import * as ko from "knockout";
import { IItemListJso, IItemGroupJso, ItemGroupViewModel, ItemListApi } from ".";

interface IViewModelParams {
    model: IItemListJso;
}

export class ItemListViewModel {
    private readonly apiClient: ItemListApi;
    private readonly model: IItemListJso;
    
    public newGroupName: KnockoutObservable<String>;
    public groups: KnockoutObservableArray<ItemGroupViewModel>;

    constructor(params: IViewModelParams) {
        this.model = params.model;

        this.newGroupName = ko.observable<String>();
        this.apiClient = new ItemListApi();
        this.groups = ko.observableArray<ItemGroupViewModel>(this.model.itemGroups.map(g => {
            const items = this.model.items.filter(i => i.itemGroupId === g.id);
            return new ItemGroupViewModel(g, items);
        }));
    }

    public addGroup = () => {
        const self = this;

        const groupName = self.newGroupName().trim();
        if (!groupName) {
            return;
        }

        let maxSortIndex = -1;
        self.groups().forEach(g => maxSortIndex = g.sortIndex > maxSortIndex ? g.sortIndex : maxSortIndex);

        const groupToAdd = { name: groupName, sortIndex: maxSortIndex + 1 };

        self.apiClient
            .createGroup(self.model.id, groupToAdd)
            .then((g: IItemGroupJso) => {
                const group = new ItemGroupViewModel(g, []);
                group.isNewItemFocused(true);

                self.groups.push(group);
                self.newGroupName("");
            });
    }

    public removeGroup = (group: ItemGroupViewModel) => {
        const self = this;

        self.apiClient
            .deleteGroup(self.model.id, group.id)
            .then(() => { self.groups.remove(group); });
    }
}
