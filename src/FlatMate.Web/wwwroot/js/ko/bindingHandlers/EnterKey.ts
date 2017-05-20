export default {
    init: (element: HTMLElement, valueAccessor: () => any, allBindings: KnockoutAllBindingsAccessor | undefined, viewModel: any) => {
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
}