import { HandlerBase } from "./HandlerBase.js";

export class ResizeHandler extends HandlerBase {
    constructor(el, fn, id, once) {
        super(el, 'resize', fn, id, once);
        this.resizeObserver = new ResizeObserver(entries => {
            this.action(entries);
        });
    }
    action(event) {
        if (this.once) {
            this.off();
        }
        this.delegate.apply(this.element, [event]);
    }
    on() {
        this.resizeObserver.observe(this.element);
    }
    off() {
        this.resizeObserver.disconnect();
    }
}
