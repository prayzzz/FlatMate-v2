import { AlertLevel } from "./";

export class AlertViewModel {
    public level = ko.observable<AlertLevel>();
    public levelString = ko.computed(() => AlertLevel[this.level()]);
    public message = ko.observable<string>();

    constructor(level: AlertLevel, message: string) {
        this.level(level);
        this.message(message);
    }
}