// tslint:disable:interface-name

import { IDictionary } from "./types";

declare global {
    interface Array<T> {
        groupBy(selector: (element: T) => string | number): IDictionary<T[]>;
    }
}

Array.prototype.groupBy = function <T>(selector: (element: T) => string | number): IDictionary<T[]> {
    return this.reduce((groups: IDictionary<T[]>, value: T) => {
        let key = selector(value);
        if (groups[key] === undefined) {
            groups[key] = [];
        }
        groups[key].push(value);
        return groups;
    }, {});
};