import * as ko from "knockout";

import { ItemListEditor } from "./areas/lists/";
import { IWindow } from "./window";

// init ItemListEditor
document.addEventListener("DOMContentLoaded", () => new ItemListEditor(), false);

// add knockout to window
declare var window: IWindow;
window.ko = ko;

// add EnterKey event
ko.bindingHandlers.enterkey = {
    init: (element: HTMLElement, valueAccessor, allBindings, viewModel) => {
        const callback = valueAccessor();

        element.addEventListener("keypress", (event) => {
            const keyCode = event.which ? event.which : event.keyCode;
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};
