import * as findFill from "array.prototype.find";
import * as Blazy from "blazy";
import * as promiseFill from "es6-promise";
import * as ko from "knockout";
import { UserInfoJso } from "./areas/account/jso";
import { KoStartup, StartupHelper } from "./koStartup";
import "./shared/arrayExtensions";
import "./shared/objectExtensions";
import { IWindow } from "./window";

promiseFill.polyfill();
findFill.shim();

// add knockout to window
declare var window: IWindow;
window.ko = ko;

KoStartup.start();

export class FlatMate {
    public static blazy = new Blazy({});

    public static currentUser = StartupHelper.readViewData<UserInfoJso>("current-user")
}