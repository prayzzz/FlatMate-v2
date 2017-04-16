import { IItemJso } from "jso";
import * as ko from "knockout";
import { IUserInfoJso } from "../../account/jso";

export default class ItemModel {
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
