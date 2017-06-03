import * as ko from "knockout";
import { KoStartup } from "./KoStartup";
import "./shared/ObjectExtensions";
import { IWindow } from "./Window";

// add knockout to window
declare var window: IWindow;
window.ko = ko;

KoStartup.start();
