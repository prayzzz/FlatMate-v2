// tslint:disable:max-classes-per-file

import * as ko from "knockout";
import { ListsRegistry } from "./areas/lists/";
import { OffersRegistry } from "./areas/offers/";
import { SharedRegistry } from "./shared/";
import EnterKey from "./ko/bindingHandlers/enterKey";

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
        OffersRegistry.registerComponents();
        SharedRegistry.registerComponents();
    }
}

export class StartupHelper {
    public static readViewData<T>(idSuffix: string): T {
        const elementId = "data-" + idSuffix;
        const element = document.getElementById(elementId);
        if (!element || !element.innerText.trim()) {
            throw new Error(`no data available for ${elementId}`);
        }

        return JSON.parse(element.innerText) as T;
    }
}
