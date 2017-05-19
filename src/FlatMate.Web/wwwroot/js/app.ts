import * as ko from "knockout";
import './shared/ObjectExtensions';
import { IWindow } from "./Window";
import { KoStartup } from "./KoStartup";

// add knockout to window
declare var window: IWindow;
window.ko = ko;

KoStartup.start();
