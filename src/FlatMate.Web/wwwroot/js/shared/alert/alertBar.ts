import { IDictionary } from "../types";
import { AlertLevel, AlertViewModel, ResultJso } from "./";
import { observableArray } from "knockout";

export class AlertBar {
    public readonly alerts = observableArray<AlertViewModel>();

    private readonly timeouts: IDictionary<number> = {};

    constructor() {
        this.timeouts[AlertLevel.Error] = 60000;
        this.timeouts[AlertLevel.Info] = 30000;
        this.timeouts[AlertLevel.Success] = 3000;
        this.timeouts[AlertLevel.Warning] = 30000;
    }

    public addAlertFromResult(result: ResultJso) {
        const level = result.isSuccess ? AlertLevel.Success : AlertLevel.Error;
        const alert = new AlertViewModel(level, result.message);

        this.pushAlert(alert);
    }

    private pushAlert(alert: AlertViewModel) {
        this.alerts.push(alert);
        setTimeout(() => this.alerts.remove(alert), this.timeouts[alert.level()]);
    }
}
