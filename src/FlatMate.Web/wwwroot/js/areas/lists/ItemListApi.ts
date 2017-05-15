import ApiClient from "../../api/apiClient";
import { IItemJso, IItemGroupJso } from "."

export class ItemListApi {
    private static instance: ItemListApi;
    private readonly apiClient: ApiClient;

    /**
    * Returns the singleton instance
    */
    constructor() {
        if (!ItemListApi.instance) {
            this.apiClient = new ApiClient();
            ItemListApi.instance = this;
        }

        return ItemListApi.instance;
    }

    public async createItem(listId: number, groupId: number, item: any): Promise<IItemJso> {
        return this.apiClient.post<any, IItemJso>(`lists/itemlist/${listId}/group/${groupId}/item`, item);
    }

    public async createGroup(listId: number, group: any): Promise<IItemGroupJso> {
        return this.apiClient.post<any, IItemGroupJso>(`lists/itemlist/${listId}/group/`, group);
    }

    public async updateItem(listId: number, itemId: number, item: IItemJso): Promise<IItemJso> {
        return this.apiClient.put<IItemJso, IItemJso>(`lists/itemlist/${listId}/item/${itemId}`, item);
    }    

    public async deleteItem(listId: number, itemId: number): Promise<void> {
        return this.apiClient.delete(`lists/itemlist/${listId}/item/${itemId}`);
    }    

    public async deleteGroup(listId: number, groupId: number): Promise<void> {
        return this.apiClient.delete(`lists/itemlist/${listId}/group/${groupId}`);
    }    
}