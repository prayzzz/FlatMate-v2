import Ajax from "../shared/ajax";

export interface IApiClient {
    get<TData>(path: string, doneCallback?: (d: TData) => void, failCallback?: (e: IApiError) => void): void;
    put<TData, TResult>(path: string, data: TData, doneCallback?: (d: TResult) => void, failCallback?: (e: IApiError) => void): void;
    post<TData, TResult>(path: string, data: TData, doneCallback?: (d: TResult) => void, failCallback?: (e: IApiError) => void): void;
}

export interface IApiError {
    status: number;
    statusText: string;
    responseText: string;
}

/**
 * Singleton
 * new ApiV1Client() returns the singleton instance
 */
export default class ApiClient implements IApiClient {
    private static instance: ApiClient;
    private host = `//${window.location.host}/api/v1/`;

    /**
     * Returns the singleton instance
     */
    constructor() {
        if (!ApiClient.instance) {
            ApiClient.instance = this;
        }

        return ApiClient.instance;
    }

    public async get<TData>(path: string): Promise<TData> {
        const url = this.host + path;

        return new Promise<TData>((resolve, reject) =>
            Ajax.get<TData, IApiError>(url)
                .success((d: TData) => resolve(d))
                .error((e: IApiError) => reject(e))
                .send()
        );
    }

    public delete(path: string): Promise<void> {
        const url = this.host + path;

        return new Promise<void>((resolve, reject) =>
            Ajax.delete<void, IApiError>(url)
                .success(() => resolve())
                .error((e: IApiError) => reject(e))
                .send()
        );
    }

    public put<TData>(path: string, data: any): Promise<TData> {
        const url = this.host + path;

        return new Promise<TData>((resolve, reject) =>
            Ajax.put<TData, IApiError>(url, JSON.stringify(data))
                .success((d: TData) => resolve(d))
                .error((e: IApiError) => reject(e))
                .send()
        );
    }

    public async post<TData>(path: string, data: any): Promise<TData> {
        const url = this.host + path;

        return new Promise<TData>((resolve, reject) =>
            Ajax.post<TData, IApiError>(url, JSON.stringify(data))
                .success((d: TData) => resolve(d))
                .error((e: IApiError) => reject(e))
                .send()
        );
    }
}
