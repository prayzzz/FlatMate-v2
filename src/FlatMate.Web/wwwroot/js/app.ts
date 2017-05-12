import * as ko from "knockout";
import * as kodd from "knockout-dragdrop";

import ItemListEditor from "./areas/lists/itemListEditor";
import { IWindow } from "./window";

// add knockout to window
declare var window: IWindow;
window.ko = ko;


console.log(kodd)

// init ItemListEditor
document.addEventListener("DOMContentLoaded", () => new ItemListEditor(), false);

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
