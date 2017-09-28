import { ProductJso, ProductApi } from ".";

export class ProductVm {
    private productApi = new ProductApi();

    private model: KnockoutObservable<ProductJso>;
    public isFavorite: KnockoutObservable<boolean>;
    public isLoading: KnockoutObservable<boolean>;
    public showLoadingButton: KnockoutComputed<boolean>;
    public showFavoriteBtn: KnockoutComputed<boolean>;
    public showUnfavoriteBtn: KnockoutComputed<boolean>;

    constructor(model: ProductJso) {
        const self = this;

        this.model = ko.observable(model);
        this.isFavorite = ko.observable(false);
        this.isLoading = ko.observable(false);

        this.showLoadingButton = ko.computed(() => self.isLoading())
        this.showFavoriteBtn = ko.computed(() => !self.isLoading() && !self.isFavorite())
        this.showUnfavoriteBtn = ko.computed(() => !self.isLoading() && self.isFavorite())
    }

    public get id(): number {
        return this.model().id;
    }

    public get name(): string {
        return this.model().name;
    }

    public get description(): string | undefined {
        return this.model().description;
    }

    public get imageUrl(): string | undefined {
        return this.model().imageUrl + "?resize=150px:150px";
    }

    public match(term: string): boolean {
        if (!term || term == "") {
            return true;
        }

        term = term.toLowerCase();

        if (this.model().name && this.model().name.toLowerCase().includes(term)) {
            return true;
        }

        if (this.model().brand && this.model().brand.toLowerCase().includes(term)) {
            return true;
        }

        if (this.model().description && this.model().description.toLowerCase().includes(term)) {
            return true;
        }

        return false;
    }

    public favorite(): void {
        const self = this;

        this.isLoading(true);
        this.productApi.favorite(this.id).then(() => {
            self.isFavorite(true);
            self.isLoading(false);
        })
    }

    public unfavorite(): void {
        const self = this;

        this.isLoading(true);
        this.productApi.unfavorite(this.id).then(() => {
            self.isFavorite(false);
            self.isLoading(false);
        })
    }
}