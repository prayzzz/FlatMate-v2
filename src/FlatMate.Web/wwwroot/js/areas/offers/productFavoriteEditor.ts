import { ProductFavoriteManageVm, MarketJso, ProductApi, ProductVm } from ".";
import { FlatMate } from "../../app";

export class ProductFavoriteEditor {
    private selectedMarket: KnockoutObservable<MarketJso | undefined>;
    private availableProducts: KnockoutObservableArray<ProductVm>;
    private productFavorites: KnockoutObservableArray<ProductVm>;

    private itemsPerPage: KnockoutObservable<number>;
    private currentPage: KnockoutObservable<number>;;
    private availableProductsView: KnockoutObservableArray<ProductVm>;

    private readonly apiClient = new ProductApi();
    private readonly model: ProductFavoriteManageVm;

    constructor(model: ProductFavoriteManageVm) {
        this.model = model;

        this.selectedMarket = ko.observable(undefined);
        this.selectedMarket.subscribe(m => this.selectedMarketChanges(m));

        if (model.currentMarket) {
            this.selectedMarket(this.model.markets.find(m => m.id == model.currentMarket));
        }

        this.itemsPerPage = ko.observable(12);
        this.currentPage = ko.observable(1);
        this.availableProducts = ko.observableArray();
        this.availableProductsView = ko.observableArray();
        this.productFavorites = ko.observableArray();
    }

    public nextPage()
    {
        this.showPage(this.currentPage() + 1);
    }

    public prevPage()
    {
        this.showPage(this.currentPage() - 1);
    }

    public showPage(page: number) {
        this.currentPage(page);
        this.availableProductsView.removeAll();
        this.availableProducts.slice((this.currentPage() - 1) * this.itemsPerPage(), this.currentPage() * this.itemsPerPage())
            .forEach(p => this.availableProductsView.push(p))

        setTimeout(() => FlatMate.blazy.revalidate(), 1);
    }

    private selectedMarketChanges(market: MarketJso | undefined): void {
        const self = this;

        if (!market) {
            return;
        }

        this.apiClient.getProducts(market.id).then(products => {
            self.availableProducts.removeAll();

            products.sort((a, b) => { return a.name.localeCompare(b.name) })
                .forEach(p => self.availableProducts.push(new ProductVm(p)))

            self.showPage(1);
        })

        this.apiClient.getProductFavorites(market.id).then(products => {
            self.productFavorites.removeAll();

            products.sort((a, b) => { return a.name.localeCompare(b.name) })
                .forEach(p => {
                    self.productFavorites.push(new ProductVm(p));
                })

        })
    }
}
