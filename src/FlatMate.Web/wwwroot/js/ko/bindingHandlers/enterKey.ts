import { AllBindings } from "knockout";

export default {
    init: (element: HTMLElement, valueAccessor: () => any, allBindings: AllBindings | undefined, viewModel: any) => {
        const callback = valueAccessor();

        element.addEventListener("keypress", event => {
            const keyCode = event.which ? event.which : event.keyCode;
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};
