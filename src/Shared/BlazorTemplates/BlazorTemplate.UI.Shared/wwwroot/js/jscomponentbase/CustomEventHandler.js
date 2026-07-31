import { HandlerBase } from "./HandlerBase.js";
import { EventHandler } from "./event-handler.js";

export class CustomEventHandler extends HandlerBase {
    constructor(el, eventType, fn, id, once) {
        super(el, eventType, fn, id, once);
    }
    action(event) {
        if (this.once) {
            EventHandler.remove(this.element, this.type, this.delegate);
        }
        this.delegate.apply(this.element, [event]);
    }
    on() {
        this.element.addEventListener(this.type, this.action);
    }

    off() {
        this.element.removeEventListener(this.type, this.action);
    }
}
