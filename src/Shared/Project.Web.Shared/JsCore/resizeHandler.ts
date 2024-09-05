import {HandlerBase} from './handlerBase'

export class ResizeHandler extends HandlerBase {
    resizeObserver: ResizeObserver

    constructor(el: Element | EventTarget
        , fn: Function
        , id: string
        , once: boolean
        , drop: Function | undefined = undefined) {
        super(el, 'resize', fn, id, once, drop);
        this.resizeObserver = new ResizeObserver(entries => {
            this.action(entries);
        });
    }

    on(): void {
        if (this.element instanceof Element)
            this.resizeObserver.observe(this.element);
    }

    off(): void {
        this.resizeObserver.disconnect();
    }
}
