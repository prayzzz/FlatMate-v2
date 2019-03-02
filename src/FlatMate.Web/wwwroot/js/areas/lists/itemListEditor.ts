import { ItemGroupJso, ItemGroupViewModel, ItemListJso } from ".";
import { observable, observableArray } from "knockout";

export class ItemListEditor {
    public addGroup = () => {
        const self = this;

        const groupName = self.newGroupName().trim();
        if (!groupName) {
            return;
        }

        let maxSortIndex = -1;
        self.groups().forEach(group => (maxSortIndex = group.sortIndex() > maxSortIndex ? group.sortIndex() : maxSortIndex));

        const groupVm = new ItemGroupViewModel(new ItemGroupJso(self.model.id));
        groupVm.name(groupName);
        groupVm.sortIndex(maxSortIndex + 1);

        self.isAddLoading(true);
        groupVm.save().then(
            () => {
                self.groups.push(groupVm);
                self.newGroupName("");
                self.isAddLoading(false);
            },
            () => {
                self.isAddLoading(false);
            }
        );
    };

    public readonly newGroupName = observable("");
    public readonly groups = observableArray<ItemGroupViewModel>();
    public readonly isAddLoading = observable(false);

    constructor(model: ItemListJso) {
        this.model = model;

        this.groups(
            this.model.itemGroups.map(g => {
                const items = this.model.items.filter(i => i.itemGroupId === g.id);
                return new ItemGroupViewModel(g, items);
            })
        );
    }
    public removeGroup = (group: ItemGroupViewModel) => {
        const self = this;

        group.delete().then(
            () => {
                self.groups.remove(group);
            },
            () => {
                /* Handle error */
            }
        );
    };
    private readonly model: ItemListJso;
}
