// tslint:disable:max-classes-per-file

import * as ko from "knockout";
import { ListsRegistry } from "./areas/lists/";
import EnterKey from "./ko/bindingHandlers/EnterKey";

export class KoStartup {
    public static start() {
        KoStartup.registerComponents();
        KoStartup.registerBindingHandlers();

        ko.applyBindings();
    }

    private static registerBindingHandlers() {
        ko.bindingHandlers.enterkey = EnterKey;
    }

    private static registerComponents() {
        ListsRegistry.registerComponents();
    }
}

export class StartupHelper {
    public static readModel<T>(): T {
        const element = document.getElementById("view-data");
        if (!element || !element.innerText.trim()) {
            throw new Error("no data available");
        }

        return JSON.parse(element.innerText) as T;
    }
}
