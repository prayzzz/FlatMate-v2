import { ItemGroupJso, ItemJso } from ".";
import ApiClient from "api/apiClient";

/**
 * Singleton
 */
export class ItemListApi {
    private static instance: ItemListApi;
    private readonly apiClient = new ApiClient();

    /**
     * Returns the singleton instance
     */
    constructor() {
        if (!ItemListApi.instance) {
            ItemListApi.instance = this;
        }

        return ItemListApi.instance;
    }

    public createItem(listId: number, groupId: number | undefined, item: any): Promise<ItemJso> {
        if (groupId) {
            return this.apiClient.post<ItemJso>(`lists/itemlist/${listId}/group/${groupId}/item`, item);
        }

        return this.apiClient.post<ItemJso>(`lists/itemlist/${listId}/item`, item);
    }

    public createGroup(listId: number, group: any): Promise<ItemGroupJso> {
        return this.apiClient.post<ItemGroupJso>(`lists/itemlist/${listId}/group/`, group);
    }

    public updateItem(listId: number, itemId: number, item: ItemJso): Promise<ItemJso> {
        return this.apiClient.put<ItemJso>(`lists/itemlist/${listId}/item/${itemId}`, item);
    }

    public updateGroup(listId: number, groupId: number, group: ItemGroupJso): Promise<ItemGroupJso> {
        return this.apiClient.put<ItemGroupJso>(`lists/itemlist/${listId}/group/${groupId}`, group);
    }

    public deleteItem(listId: number, itemId: number): Promise<void> {
        return this.apiClient.delete(`lists/itemlist/${listId}/item/${itemId}`);
    }

    public deleteGroup(listId: number, groupId: number): Promise<void> {
        return this.apiClient.delete(`lists/itemlist/${listId}/group/${groupId}`);
    }
}
