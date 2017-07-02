interface IAjaxRequest<TData, TError> {
    success(cb: (data: TData | null) => void): IAjaxRequest<TData, TError>;
    error(cb: (error: TError | null) => void): IAjaxRequest<TData, TError>;
    always(cb: (data: any | null) => void): IAjaxRequest<TData, TError>;
    send(): void;
}

export default class Ajax {
    public static get<TData, TError>(url: string): IAjaxRequest<TData, TError> {
        return new AjaxRequest<TData, TError>("GET", url);
    }
    public static put<TData, TError>(url: string, data: any): IAjaxRequest<TData, TError> {
        return new AjaxRequest<TData, TError>("PUT", url, data);
    }
    public static post<TData, TError>(url: string, data: any): IAjaxRequest<TData, TError> {
        return new AjaxRequest<TData, TError>("POST", url, data);
    }
    public static delete<TData, TError>(url: string): IAjaxRequest<TData, TError> {
        return new AjaxRequest<TData, TError>("DELETE", url);
    }
}

// tslint:disable-next-line:max-classes-per-file
class AjaxRequest<TData, TError> implements IAjaxRequest<TData, TError> {
    private readonly method: string;
    private readonly url: string;
    private readonly data: any;

    private successCb: (data: TData | null) => void;
    private errorCb: (error: TError | null) => void;
    private alwaysCb: (data: any | null) => void;

    constructor(method: string, url: string, data?: any) {
        this.method = method;
        this.url = url;
        this.data = data;

        this.successCb = (): void => {
            /** Dummy */
        };
        this.errorCb = (): void => {
            /** Dummy */
        };
        this.alwaysCb = (): void => {
            /** Dummy */
        };
    }

    public success(cb: (data: TData | null) => void): IAjaxRequest<TData, TError> {
        this.successCb = cb;
        return this;
    }

    public error(cb: (error: TError | null) => void): IAjaxRequest<TData, TError> {
        this.errorCb = cb;
        return this;
    }

    public always(cb: (data: any | null) => void): IAjaxRequest<TData, TError> {
        this.alwaysCb = cb;
        return this;
    }

    public send() {
        const request = new XMLHttpRequest();
        request.open(this.method, this.url, true);
        request.setRequestHeader("Content-Type", "application/json");
        request.onreadystatechange = () => {
            if (4 !== request.readyState) {
                return;
            }

            this.alwaysCb(this.parse<any>(request));
        };

        request.onerror = () => {
            this.errorCb(this.parse<TError>(request));
        };

        request.onloadend = () => {
            if (request.status >= 200 && request.status < 300) {
                this.successCb(this.parse<TData>(request));
            } else {
                this.errorCb(this.parse<TError>(request));
            }
        };

        if (this.data) {
            request.send(this.data);
        } else {
            request.send();
        }
    }

    private parse<T>(request: XMLHttpRequest): T | null {
        if (!request.responseText && request.responseText === "") {
            return null;
        }

        let result: T;
        try {
            result = JSON.parse(request.responseText);
        } catch (e) {
            throw e;
        }

        return result;
    }
}
