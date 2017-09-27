import { ProductJso } from ".";

export class ProductVm {
    private model: KnockoutObservable<ProductJso>;

    constructor(model: ProductJso) {
        this.model = ko.observable(model);
    }

    public get id(): number | undefined {
        return this.model().id;
    }

    public get name(): string | undefined {
        return this.model().name;
    }

    public get imageUrl(): string | undefined {
        return this.model().imageUrl + "?resize=150px:150px";
    }
}