import { IUserInfoJso } from "../account/jso";

export interface IItemJso {
    id: number;
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
    itemGroupId: number | null;
    itemListId: number;
    sortIndex: number;
}

export interface IItemGroupJso {
    id: number;
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
    itemListId: number;
    sortIndex: number;
}

export interface IItemListJso {
    description: string;
    id: number;
    isPublic: boolean;
    itemCount: number;
    items: IItemJso[];
    itemGroups: IItemGroupJso[];
    lastEditor: IUserInfoJso;
    name: string;
    owner: IUserInfoJso;
}