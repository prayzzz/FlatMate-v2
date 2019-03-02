import { CompanyJso, ProductApi, ProductVm } from ".";
import { FlatMate } from "app";
import { PartialListParameter } from "api/partialList";
import { computed, Computed, observable, Observable, observableArray, ObservableArray } from "knockout";

export interface ProductFavoriteManageVm {
    companies: CompanyJso[];
    currentCompany: number | undefined;
}

export class ProductFavoriteEditor {
    private selectedCompany: Observable<CompanyJso | undefined>;

    private productPage: ObservableArray<ProductVm>;
    private totalCount: Observable<number>;

    // @ts-ignore: used by view
    private hasMoreProducts: Computed<boolean>;
    // @ts-ignore: used by view
    private pageString: Computed<string>;

    private productsPerPage: Observable<number>;
    private currentPage: Observable<number>;
    private searchTerm: Observable<string>;
    private onlyFavorites: Observable<boolean>;

    private readonly apiClient = new ProductApi();
    private readonly model: ProductFavoriteManageVm;

    constructor(model: ProductFavoriteManageVm) {
        this.model = model;

        // setup observables
        this.selectedCompany = observable(undefined);
        this.selectedCompany.subscribe(() => this.showPage(0));

        this.searchTerm = observable("");
        this.searchTerm.subscribe(() => this.showPage(0));

        this.onlyFavorites = observable(false);
        this.onlyFavorites.subscribe(() => this.showPage(0));

        this.productPage = observableArray([]);
        this.currentPage = observable(1);
        this.productsPerPage = observable(12);
        this.totalCount = observable(0);

        this.hasMoreProducts = computed<boolean>(() => {
            return this.productsPerPage() * (this.currentPage() + 1) < this.totalCount();
        });

        this.pageString = computed<string>(() => {
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

        setTimeout(() => {
            FlatMate.blazy.revalidate();
            window.scroll(0, 0);
        }, 1);
    }
}
