import * as ko from "knockout";
import * as promiseFill from "es6-promise";
import { KoStartup } from "./koStartup";
import "./shared/objectExtensions";
import { IWindow } from "./window";

promiseFill.polyfill();

// add knockout to window
declare var window: IWindow;
window.ko = ko;

KoStartup.start();
