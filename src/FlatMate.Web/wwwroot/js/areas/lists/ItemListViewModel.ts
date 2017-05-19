import * as ko from "knockout";
import { ItemListJso, ItemGroupViewModel, ItemListApi, ItemGroupJso } from ".";

interface IViewModelParams {
    model: ItemListJso;
}

export class ItemListViewModel {
    private readonly apiClient: ItemListApi;
    private readonly model: ItemListJso;

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
        self.groups().forEach(group => maxSortIndex = group.sortIndex() > maxSortIndex ? group.sortIndex() : maxSortIndex);

        const groupVm = new ItemGroupViewModel(new ItemGroupJso(self.model.id));
        groupVm.name(groupName);
        groupVm.sortIndex(maxSortIndex + 1);

        groupVm.save().then(() => {
            self.groups.push(groupVm);
            self.newGroupName("");
        }, err => { /* Handle error */ })
    }

    public removeGroup = (group: ItemGroupViewModel) => {
        const self = this;
        group.delete().then(() => self.groups.remove(group), err => { /* Handle error */ });
    }
}
