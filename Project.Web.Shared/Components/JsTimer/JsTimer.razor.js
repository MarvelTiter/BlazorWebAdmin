import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/BlazorWeb.Shared/js/jscomponentbase/component-store.js";

export class JsTimer extends BaseComponent {
    constructor(dotNetRef, interval) {
        super();
        this.instance = dotNetRef;
        this.interval = interval;
        this.timer = null
    }

    start() {
        this.timer = window.setInterval(() => {
            this.instance.invokeMethodAsync("Call");
        }, this.interval)
    }

    dispose() {
        if (this.timer) {
            window.clearInterval(this.timer);
            this.timer = null;
        }
    }

    //static init(id, dotNetRef, interval) {
    //    var timter = getComponentById(id, () => {
    //        return new JsTimer(dotNetRef, interval);
    //    });
    //    timter.start();
    //}
}

export function init(id, dotNetRef, interval) {
    var timter = getComponentById(id, () => {
        return new JsTimer(dotNetRef, interval);
    });
    timter.start();
}