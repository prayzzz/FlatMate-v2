export class PartialListParameter {
    constructor(public limit: number,
                public offset: number) {
    };
}

export interface PartialList<T> {
    items: T[];
    limit: number;
    offset: number;
    total: number;
}