import { CompanyJso, ProductApi, ProductVm } from ".";
import { FlatMate } from "app";
import { PartialListParameter } from "Api/PartialList";

export interface ProductFavoriteManageVm {
    companies: CompanyJso[];
    currentCompany: number | undefined;
}

export class ProductFavoriteEditor {
    private selectedCompany: KnockoutObservable<CompanyJso | undefined>;

    private productPage: KnockoutObservableArray<ProductVm>;
    private totalCount: KnockoutObservable<number>;

    // @ts-ignore: used by view
    private hasMoreProducts: KnockoutComputed<boolean>;
    // @ts-ignore: used by view
    private pageString: KnockoutComputed<string>;

    private productsPerPage: KnockoutObservable<number>;
    private currentPage: KnockoutObservable<number>;
    private searchTerm: KnockoutObservable<string>;
    private onlyFavorites: KnockoutObservable<boolean>;

    private readonly apiClient = new ProductApi();
    private readonly model: ProductFavoriteManageVm;

    constructor(model: ProductFavoriteManageVm) {
        this.model = model;

        // setup observables
        this.selectedCompany = ko.observable(undefined);
        this.selectedCompany.subscribe(() => this.showPage(0));

        this.searchTerm = ko.observable("");
        this.searchTerm.subscribe(() => this.showPage(0));

        this.onlyFavorites = ko.observable(false);
        this.onlyFavorites.subscribe(() => this.showPage(0));

        this.productPage = ko.observableArray([]);
        this.currentPage = ko.observable(1);
        this.productsPerPage = ko.observable(15);
        this.totalCount = ko.observable(0);

        this.hasMoreProducts = ko.computed<boolean>(() => {
            return this.productsPerPage() * (this.currentPage() + 1) < this.totalCount();
        });

        this.pageString = ko.computed<string>(() => {
            let totalPages = Math.ceil(this.totalCount() / this.productsPerPage());
            if (totalPages < 1) {
                totalPages = 1;
            }
            return `${this.currentPage() + 1} / ${totalPages}`;
        });

        // init editor
        if (model.currentCompany) {
            let market = this.model.companies.find(m => m.id === model.currentCompany);
            if (market) {
                this.selectedCompany(market);
            }
        }
    }

    public nextPage() {
        this.showPage(this.currentPage() + 1);
    }

    public prevPage() {
        this.showPage(this.currentPage() - 1);
    }

    public async showPage(page: number): Promise<void> {
        this.currentPage(page);

        return this.loadProductPage()
    }

    private async loadProductPage(): Promise<void> {
        const market = this.selectedCompany();
        if (!market) {
            return;
        }

        const queryParam = new PartialListParameter(this.productsPerPage(), this.productsPerPage() * this.currentPage());

        this.productPage.removeAll();
        if (this.onlyFavorites()) {
            const products = await this.apiClient.searchFavorites(market.id, this.searchTerm(), queryParam);
            this.totalCount(products.total);

            for (const productJso of products.items) {
                let vm = new ProductVm(productJso, market);
                vm.isFavorite(true);
                this.productPage.push(vm);
            }
        }
        else {
            const productsTask = this.apiClient.searchProducts(market.id, this.searchTerm(), queryParam);
            const favProductIdsTask = this.apiClient.getProductFavoriteIds(market.id);

            const products = await productsTask;
            const favProductIds = await favProductIdsTask;

            this.totalCount(products.total);

            for (const productJso of products.items) {
                const product = new ProductVm(productJso, market);
                product.isFavorite(favProductIds.some(fp => fp === product.id));
                this.productPage.push(product);
            }
        }

        setTimeout(() => FlatMate.blazy.revalidate(), 1);
    }
}
