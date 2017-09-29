import * as ko from "knockout";
import * as Blazy from "blazy";
import * as promiseFill from "es6-promise";
import * as findFill from "array.prototype.find";
import { KoStartup } from "./koStartup";
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
}