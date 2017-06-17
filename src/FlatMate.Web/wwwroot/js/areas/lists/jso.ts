// tslint:disable:max-classes-per-file

import { UserInfoJso } from "../account/jso";

export class ItemJso {
    public id: number | undefined;
    public lastEditor: UserInfoJso;
    public name: string;
    public owner: UserInfoJso;
    public itemGroupId: number | undefined;
    public itemListId: number;
    public sortIndex: number;

    constructor(itemListId: number, itemGroupId?: number) {
        this.itemListId = itemListId;
        this.itemGroupId = itemGroupId;
    }
}

export class ItemGroupJso {
    public id: number | undefined;
    public lastEditor: UserInfoJso;
    public name: string;
    public owner: UserInfoJso;
    public itemListId: number;
    public sortIndex: number;

    constructor(itemListId: number) {
        this.itemListId = itemListId;
    }
}

export class ItemListJso {
    public description: string;
    public id: number;
    public isPublic: boolean;
    public itemCount: number;
    public items: ItemJso[];
    public itemGroups: ItemGroupJso[];
    public lastEditor: UserInfoJso;
    public name: string;
    public owner: UserInfoJso;
}
