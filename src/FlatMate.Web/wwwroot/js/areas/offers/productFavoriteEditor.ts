import { MarketJso, ProductApi, ProductFavoriteManageVm, ProductVm } from ".";
import { FlatMate } from "../../app";

export class ProductFavoriteEditor {
    private selectedMarket: KnockoutObservable<MarketJso | undefined>;

    private allProducts: KnockoutObservableArray<ProductVm>;
    private filteredProducts: KnockoutObservableArray<ProductVm>;
    private productPage: KnockoutObservableArray<ProductVm>;

    private itemsPerPage: KnockoutObservable<number>;
    private currentPage: KnockoutObservable<number>;
    private searchTerm: KnockoutObservable<string>;
    private onlyFavorites: KnockoutObservable<boolean>;

    private readonly apiClient = new ProductApi();
    private readonly model: ProductFavoriteManageVm;

    constructor(model: ProductFavoriteManageVm) {
        this.model = model;

        this.selectedMarket = ko.observable(undefined);
        this.selectedMarket.subscribe(m => this.selectMarket(m));

        if (model.currentMarket) {
            this.selectedMarket(this.model.markets.find(m => m.id === model.currentMarket));
        }

        this.onlyFavorites = ko.observable(false);
        this.onlyFavorites.subscribe(value => this.applyFilter(this.searchTerm(), value));

        this.searchTerm = ko.observable("");
        this.searchTerm.subscribe(value => this.applyFilter(value, this.onlyFavorites()));

        this.itemsPerPage = ko.observable(15);
        this.currentPage = ko.observable(1);

        this.allProducts = ko.observableArray();
        this.filteredProducts = ko.observableArray();
        this.productPage = ko.observableArray();
    }

    public nextPage() {
        this.showPage(this.currentPage() + 1);
    }

    public prevPage() {
        this.showPage(this.currentPage() - 1);
    }

    public showPage(page: number) {
        this.currentPage(page);

        const from = (this.currentPage() - 1) * this.itemsPerPage();
        const to = this.currentPage() * this.itemsPerPage();

        this.productPage.removeAll();
        this.filteredProducts.slice(from, to).forEach(p => this.productPage.push(p));

        setTimeout(() => FlatMate.blazy.revalidate(), 1);
    }

    private applyFilter(term: string, onlyFavorites: boolean): void {
        this.filteredProducts.removeAll();

        for (const product of this.allProducts()) {
            if (onlyFavorites && !product.isFavorite()) {
                continue;
            }

            if (product.match(term)) {
                this.filteredProducts.push(product);
            }
        }

        this.showPage(1);
    }

    private async selectMarket(market: MarketJso | undefined): Promise<void> {
        if (!market) {
            return;
        }

        // start api calls
        const productsTask = this.apiClient.getProducts(market.id);
        const favProductIdsTask = this.apiClient.getProductFavoriteIds(market.id);

        // wait for api calls
        const products = await productsTask;
        const favProductIds = await favProductIdsTask;

        // apply loaded products
        this.allProducts.removeAll();
        for (const productJso of products.sort((a, b) => a.name.localeCompare(b.name))) {
            const product = new ProductVm(productJso, market);
            product.isFavorite(favProductIds.some(fp => fp === product.id));
            this.allProducts.push(product);
        }

        this.applyFilter(this.searchTerm(), this.onlyFavorites());
        this.showPage(1);
    }
}
