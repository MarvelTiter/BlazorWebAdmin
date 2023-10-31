import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/BlazorWeb.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/BlazorWeb.Shared/js/jscomponentbase/event-handler.js";

export class ActionWatcher extends BaseComponent {
    constructor(instance, type, timeout, element) {
        super()
        this.instance = instance
        this.timeout = timeout
        this.type = type
        this.element = element || window
        this.timer = undefined
    }

    start() {
        if (this.type == 1) {
            EventHandler.listen(this.element, "mousemove", this.debounce.bind(this))
            EventHandler.listen(this.element, "keydown", this.debounce.bind(this))
        } else if (this.type == 2) {
            EventHandler.listen(this.element, "mousemove", this.throttle.bind(this))
            EventHandler.listen(this.element, "keydown", this.throttle.bind(this))
        }
    }

    debounce() {
        clearTimeout(this.timer)
        this.timer = setTimeout(() => {
            this.invoke()
        }, this.timeout)
    }

    throttle() {
        if (!this.timer) {
            this.invoke()
            this.timer = setTimeout(() => {
                this.timer = undefined
            }, this.timeout)
        }
    }

    invoke() {
        this.instance.invokeMethodAsync('Call')
    }

    dispose() {
        EventHandler.remove(this.element, "mousemove")
        EventHandler.remove(this.element, "keydown")
        window.clearTimeout(this.timer)
        this.timer = undefined
    }

    static init(id, dotNetRef, type, timeout, element) {
        var watcher = getComponentById(id, () => {
            return new ActionWatcher(dotNetRef, type, timeout, element);
        });
        watcher.start();
    }
}