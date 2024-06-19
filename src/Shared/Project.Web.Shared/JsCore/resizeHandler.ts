import { HandlerBase } from './handlerBase'

export class ResizeHandler extends HandlerBase {
    resizeObserver: ResizeObserver
    constructor(el: Element
        , fn: Function
        , id: string
        , once: boolean
        , drop: Function = undefined) {
        super(el, 'resize', fn, id, once, drop);
        this.resizeObserver = new ResizeObserver(entries => {
            this.action(entries);
        });
    }

    on(): void {
        this.resizeObserver.observe(this.element);
    }
    off(): void {
        this.resizeObserver.disconnect();
    }
}
