import * as ko from "knockout";

import { ItemListEditor } from "./areas/lists/ItemListEditor";

console.log(ko);

document.addEventListener("DOMContentLoaded", () => new ItemListEditor(), false);

ko.bindingHandlers.enterkey = {
    init: (element: HTMLElement, valueAccessor, allBindings, viewModel) => {
        var callback = valueAccessor();

        element.addEventListener("keypress", (event) => {
            var keyCode = event.which ? event.which : event.keyCode;
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};