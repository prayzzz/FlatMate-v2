import * as ko from "knockout";
import { StartupHelper } from "../koStartup";
import { AlertBar, ResultJso } from "./alert/";

export class SharedRegistry {
    public static registerComponents() {
        // ItemListEditor
        ko.components.register("alert-bar", {
            template: { element: "alert-bar-template" },
            viewModel: function () {
                const alertBar = new AlertBar();

                const result = StartupHelper.readViewData<ResultJso>("alert");
                if (result) {
                    alertBar.addAlertFromResult(result)
                }

                return alertBar;
            }
        });
    }
}
