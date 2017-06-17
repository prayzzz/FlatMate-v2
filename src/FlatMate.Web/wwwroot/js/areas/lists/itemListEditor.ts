import * as ko from "knockout";
import { ItemGroupJso, ItemGroupViewModel, ItemListJso } from ".";

export class ItemListEditor {
    public readonly newGroupName = ko.observable("");
    public readonly groups = ko.observableArray<ItemGroupViewModel>();

    private readonly model: ItemListJso;

    constructor(model: ItemListJso) {
        this.model = model;

        this.groups(
            this.model.itemGroups.map(g => {
                const items = this.model.items.filter(i => i.itemGroupId === g.id);
                return new ItemGroupViewModel(g, items);
            })
        );
    }

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

        groupVm.save().then(
            () => {
                self.groups.push(groupVm);
                self.newGroupName("");
            },
            err => {
                /* Handle error */
            }
        );
    };

    public removeGroup = (group: ItemGroupViewModel) => {
        const self = this;
        group.delete().then(
            () => self.groups.remove(group),
            err => {
                /* Handle error */
            }
        );
    };
}