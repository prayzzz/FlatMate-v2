import { AlertLevel } from "./";
import { computed, observable } from "knockout";

export class AlertViewModel {
    public level = observable<AlertLevel>();
    public levelString = computed(() => AlertLevel[this.level()]);
    public message = observable<string>();

    constructor(level: AlertLevel, message: string) {
        this.level(level);
        this.message(message);
    }
}