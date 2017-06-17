import * as ko from "knockout";
import { KoStartup } from "./koStartup";
import "./shared/objectExtensions";
import { IWindow } from "./window";

// add knockout to window
declare var window: IWindow;
window.ko = ko;

KoStartup.start();
