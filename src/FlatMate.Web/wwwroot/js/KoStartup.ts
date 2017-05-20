import * as ko from "knockout";
import EnterKey from "./ko/bindingHandlers/EnterKey"
import { ListsRegistry } from "./areas/lists/";

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
            throw "no data available";
        }

        return <T>JSON.parse(element.innerText);
    }
}