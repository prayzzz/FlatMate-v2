interface IAjaxRequest<T> {
    success(cb: (data: T | null) => void): IAjaxRequest<T>;
    error(cb: (data: T | null) => void): IAjaxRequest<T>;
    always(cb: (data: T | null) => void): IAjaxRequest<T>;
    send(): void;
}

export default class Ajax {
    public static get<T>(url: string): IAjaxRequest<T> {
        return new AjaxRequest<T>("GET", url);
    }
    public static put<T>(url: string, data: any): IAjaxRequest<T> {
        return new AjaxRequest<T>("PUT", url);
    }
    public static post<T>(url: string, data: any): IAjaxRequest<T> {
        return new AjaxRequest<T>("POST", url);
    }
    public static delete<T>(url: string): IAjaxRequest<T> {
        return new AjaxRequest<T>("DELETE", url);
    }
}

class AjaxRequest<T> implements IAjaxRequest<T> {
    private readonly method: string;
    private readonly url: string;

    private successCb: (data: T | null) => void;
    private errorCb: (data: T | null) => void;
    private alwaysCb: (data: T | null) => void;

    constructor(method: string, url: string) {
        this.method = method;
        this.url = url;

        this.successCb = (): void => { };
        this.errorCb = (): void => { };
        this.alwaysCb = (): void => { };
    }

    public success(cb: (data: T | null) => void): IAjaxRequest<T> {
        this.successCb = cb;
        return this;
    }

    public error(cb: (data: T | null) => void): IAjaxRequest<T> {
        this.errorCb = cb;
        return this;
    }

    public always(cb: (data: T | null) => void): IAjaxRequest<T> {
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

            this.alwaysCb(this.parse(request));
        }

        request.onerror = () => {
            this.errorCb(this.parse(request));
        }

        request.onloadend = () => {
            if (request.status >= 200 && request.status < 300) {
                this.successCb(this.parse(request));
            }
            else {
                this.errorCb(this.parse(request));
            }
        }

        request.send();
    }

    private parse(request: XMLHttpRequest): T | null {
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
    };
}