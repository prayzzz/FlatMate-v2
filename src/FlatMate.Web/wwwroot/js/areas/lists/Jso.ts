import { UserInfoJso } from "../account/jso";

export class ItemJso {
    id: number | undefined;
    lastEditor: UserInfoJso;
    name: string;
    owner: UserInfoJso;
    itemGroupId: number | undefined;
    itemListId: number;
    sortIndex: number;

    constructor(itemListId: number, itemGroupId?: number) {
        this.itemListId = itemListId;
        this.itemGroupId = itemGroupId;
    }
}

export class ItemGroupJso {
    id: number | undefined;
    lastEditor: UserInfoJso;
    name: string;
    owner: UserInfoJso;
    itemListId: number;
    sortIndex: number;

    constructor(itemListId: number) {
        this.itemListId = itemListId;
    }
}

export class ItemListJso {
    description: string;
    id: number;
    isPublic: boolean;
    itemCount: number;
    items: ItemJso[];
    itemGroups: ItemGroupJso[];
    lastEditor: UserInfoJso;
    name: string;
    owner: UserInfoJso;
}