import { ProductFavoriteManageVm, MarketJso, ProductApi, ProductVm } from ".";
import { FlatMate } from "../../app";

export class ProductFavoriteEditor {
    private selectedMarket: KnockoutObservable<MarketJso | undefined>;

    private allProducts: KnockoutObservableArray<ProductVm>;
    private filteredProducts: KnockoutObservableArray<ProductVm>;
    private productPage: KnockoutObservableArray<ProductVm>;


    private itemsPerPage: KnockoutObservable<number>;
    private currentPage: KnockoutObservable<number>;
    private searchTerm: KnockoutObservable<string>;

    private readonly apiClient = new ProductApi();
    private readonly model: ProductFavoriteManageVm;

    constructor(model: ProductFavoriteManageVm) {
        this.model = model;

        this.selectedMarket = ko.observable(undefined);
        this.selectedMarket.subscribe(m => this.selectMarket(m));

        if (model.currentMarket) {
            this.selectedMarket(this.model.markets.find(m => m.id == model.currentMarket));
        }

        this.searchTerm = ko.observable("");
        this.searchTerm.subscribe(t => this.applyTermFilter(t));

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

        let from = (this.currentPage() - 1) * this.itemsPerPage();
        let to = this.currentPage() * this.itemsPerPage();

        this.productPage.removeAll();
        this.filteredProducts.slice(from, to).forEach(p => this.productPage.push(p))

        setTimeout(() => FlatMate.blazy.revalidate(), 1);
    }

    private applyTermFilter(term: string): void {
        const self = this;

        this.filteredProducts.removeAll();
        this.allProducts().filter(p => p.match(term)).forEach(p => self.filteredProducts.push(p));
        this.showPage(1);
    }

    private async selectMarket(market: MarketJso | undefined): Promise<void> {
        if (!market) {
            return;
        }

        // start api calls
        let productsOfMarketTask = this.apiClient.getProducts(market.id);
        let favProductsOfMarketTask = this.apiClient.getProductFavorites(market.id);

        // wait for api calls
        let productsOfMarket = (await productsOfMarketTask).sort((a, b) => a.name.localeCompare(b.name));
        let favProductsOfMarket = await favProductsOfMarketTask;

        // set loaded products
        this.allProducts.removeAll();
        for (var i = 0; i < productsOfMarket.length; i++) {
            var product = new ProductVm(productsOfMarket[i]);
            product.isFavorite(favProductsOfMarket.some(fp => fp.id == product.id));
            this.allProducts.push(product);
        }

        this.applyTermFilter("")
        this.showPage(1);
    }
}
