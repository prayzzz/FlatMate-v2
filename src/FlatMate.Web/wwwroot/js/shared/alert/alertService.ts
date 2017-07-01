import { AlertBar, ResultJso } from "./";

export class AlertService {
    private static instance: AlertService;
    private alertBar: AlertBar;

    /**
     * Returns the singleton instance
     */
    constructor() {
        if (!AlertService.instance) {
            AlertService.instance = this;
        }

        return AlertService.instance;
    }

    public setAlertBar(alertBar: AlertBar): void {
        this.alertBar = alertBar;
    }

    public addAlertFromResult(result: ResultJso) {
        this.alertBar.addAlertFromResult(result);
    }
}
