﻿import { ProductJso } from ".";
import ApiClient from "api/apiClient";
import { PartialList, PartialListParameter } from "api/partialList";

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
        return this.apiClient.get<PartialList<ProductJso>>(`offers/product?companyId=${marketId}&searchTerm=${searchTerm}&limit=${listQuery.limit}&offset=${listQuery.offset}`);
    }

    public getProductFavorites(marketId: number): Promise<ProductJso[]> {
        return this.apiClient.get<ProductJso[]>(`offers/product/favorite?companyId=${marketId}`);
    }

    public getProductFavoriteIds(marketId: number): Promise<number[]> {
        return this.apiClient.get<number[]>(`offers/product/favorite/id?companyId=${marketId}`);
    }

    public favorite(productId: number): Promise<void> {
        return this.apiClient.post<void>(`offers/product/favorite`, { productId });
    }

    public unfavorite(productId: number): Promise<void> {
        return this.apiClient.delete(`offers/product/favorite`, { productId });
    }

    searchFavorites(marketId: number, searchTerm: string, listQuery: PartialListParameter) {
        return this.apiClient.get<PartialList<ProductJso>>(`offers/product/favorite?companyId=${marketId}&searchTerm=${searchTerm}&limit=${listQuery.limit}&offset=${listQuery.offset}`);
    }
}