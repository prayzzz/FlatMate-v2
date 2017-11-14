import { ProductJso } from ".";
import ApiClient from "Api/ApiClient";
import { PartialList, PartialListParameter } from "Api/PartialList";

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

    public searchProducts(marketId: number, searchTerm: string, listQuery: PartialListParameter): Promise<PartialList<ProductJso>> {
        return this.apiClient.get<PartialList<ProductJso>>(`offers/product?marketId=${marketId}&searchTerm=${searchTerm}&limit=${listQuery.limit}&offset=${listQuery.offset}`);
    }

    public getProductFavorites(marketId: number): Promise<ProductJso[]> {
        return this.apiClient.get<ProductJso[]>(`offers/product/favorite?marketId=${marketId}`);
    }

    public getProductFavoriteIds(marketId: number): Promise<number[]> {
        return this.apiClient.get<number[]>(`offers/product/favorite/id?marketId=${marketId}`);
    }

    public favorite(productId: number): Promise<void> {
        return this.apiClient.post<void>(`offers/product/favorite`, { productId });
    }

    public unfavorite(productId: number): Promise<void> {
        return this.apiClient.delete(`offers/product/favorite`, { productId });
    }

    searchFavorites(marketId: number, searchTerm: string, listQuery: PartialListParameter) {
        return this.apiClient.get<PartialList<ProductJso>>(`offers/product/favorite?marketId=${marketId}&searchTerm=${searchTerm}&limit=${listQuery.limit}&offset=${listQuery.offset}`);
    }
}