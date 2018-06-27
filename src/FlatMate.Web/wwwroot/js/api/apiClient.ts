import Ajax from "shared/ajax";
import { AlertService, ResultJso } from "shared/alert";

export interface IApiClient {
    get<TData>(path: string, doneCallback?: (d: TData) => void, failCallback?: (e: ResultJso) => void): void;

    put<TData, TResult>(path: string, data: TData, doneCallback?: (d: TResult) => void, failCallback?: (e: ResultJso) => void): void;

    post<TData, TResult>(path: string, data: TData, doneCallback?: (d: TResult) => void, failCallback?: (e: ResultJso) => void): void;
}

/**
 * Singleton
 * new ApiV1Client() returns the singleton instance
 */
export default class ApiClient implements IApiClient {
    private static instance: ApiClient;
    private alertService: AlertService = new AlertService();
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

        return new Promise<TData>((resolve, reject) => {
            const request = Ajax.get<TData, ResultJso>(url);

            request.success(responseData => {
                if (responseData) {
                    resolve(responseData);
                } else {
                    resolve();
                }
            });

            request.error(error => {
                if (error) {
                    this.alertService.addAlertFromResult(error);
                    reject(error);
                } else {
                    reject();
                }
            });

            request.send();
        });
    }

    public delete(path: string, data?: any): Promise<void> {
        const url = this.host + path;

        return new Promise<void>((resolve, reject) => {
            let request;
            if (data) {
                request = Ajax.delete<void, ResultJso>(url, JSON.stringify(data));
            }
            else {
                request = Ajax.delete<void, ResultJso>(url);
            }

            request.success(() => resolve());

            request.error(error => {
                if (error) {
                    this.alertService.addAlertFromResult(error);
                    reject(error);
                } else {
                    reject();
                }
            });

            request.send();
        });
    }

    public put<TData>(path: string, data: any): Promise<TData> {
        const url = this.host + path;

        return new Promise<TData>((resolve, reject) => {
            const request = Ajax.put<TData, ResultJso>(url, JSON.stringify(data));

            request.success(responseData => {
                if (responseData) {
                    resolve(responseData);
                } else {
                    resolve();
                }
            });

            request.error(error => {
                if (error) {
                    this.alertService.addAlertFromResult(error);
                    reject(error);
                } else {
                    reject();
                }
            });

            request.send();
        });
    }

    public async post<TData>(path: string, data: any): Promise<TData> {
        const url = this.host + path;

        return new Promise<TData>((resolve, reject) => {
            const request = Ajax.post<TData, ResultJso>(url, JSON.stringify(data));

            request.success(responseData => {
                if (responseData) {
                    resolve(responseData);
                } else {
                    resolve();
                }
            });

            request.error(error => {
                if (error) {
                    this.alertService.addAlertFromResult(error);
                    reject(error);
                } else {
                    reject();
                }
            });

            request.send();
        });
    }
}
