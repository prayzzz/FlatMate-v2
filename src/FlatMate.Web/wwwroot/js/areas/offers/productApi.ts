import { ProductJso } from ".";
import ApiClient from "../../api/apiClient";

/**
 * Singleton
 */
export class ProductApi {
    private static instance: ProductApi;
    private readonly apiClient = new ApiClient();

    /**
     * Returns the singleton instance
     */
    constructor() {
        if (!ProductApi.instance) {
            ProductApi.instance = this;
        }

        return ProductApi.instance;
    }

    public getProducts(marketId: number): Promise<ProductJso[]> {
        return this.apiClient.get<ProductJso[]>(`offers/product?marketId=${marketId}`);
    }

    public getProductFavorites(marketId: number): Promise<ProductJso[]> {
        return this.apiClient.get<ProductJso[]>(`offers/product/favorite?marketId=${marketId}`);
    }
}