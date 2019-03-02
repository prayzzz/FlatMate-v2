import { CompanyJso, ProductApi, ProductJso } from ".";
import { computed, Computed, observable, Observable } from "knockout";

export class ProductVm {
    public isFavorite: Observable<boolean>;
    public isLoading: Observable<boolean>;
    public showLoadingButton: Computed<boolean>;
    public showFavoriteBtn: Computed<boolean>;
    public showUnfavoriteBtn: Computed<boolean>;
    public detailUrl: string;

    private productApi = new ProductApi();
    private model: Observable<ProductJso>;
    private company: CompanyJso;

    constructor(model: ProductJso, company: CompanyJso) {
        const self = this;

        this.model = observable(model);
        this.company = company;

        this.detailUrl = "/Offers/Product/View/" + this.model().id;
        this.isFavorite = observable(false);
        this.isLoading = observable(false);

        this.showLoadingButton = computed(() => self.isLoading());
        this.showFavoriteBtn = computed(() => !self.isLoading() && !self.isFavorite());
        this.showUnfavoriteBtn = computed(() => !self.isLoading() && self.isFavorite());
    }

    public get id(): number {
        return this.model().id;
    }

    public get name(): string {
        return this.model().name.trim();
    }

    public get description(): string | undefined {
        return this.model().description;
    }

    public get imageUrl(): string | undefined {
        if (!this.company) {
            return this.model().imageUrl;
        }

        switch (this.company.id) {
            case 1:
                return `${this.model().imageUrl}?resize=150px:150px`;
            case 2:
                return this.model().imageUrl.replace("/1080/", "/312/");
            default:
                return this.model().imageUrl;
        }
    }

    public match(term: string): boolean {
        if (!term || term === "") {
            return true;
        }

        term = term.toLowerCase();

        if (
            this.model().name &&
            this.model()
                .name.toLowerCase()
                .includes(term)
        ) {
            return true;
        }

        if (
            this.model().brand &&
            this.model()
                .brand.toLowerCase()
                .includes(term)
        ) {
            return true;
        }

        if (
            this.model().description &&
            this.model()
                .description.toLowerCase()
                .includes(term)
        ) {
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
        });
    }

    public unfavorite(): void {
        const self = this;

        this.isLoading(true);
        this.productApi.unfavorite(this.id).then(() => {
            self.isFavorite(false);
            self.isLoading(false);
        });
    }
}
