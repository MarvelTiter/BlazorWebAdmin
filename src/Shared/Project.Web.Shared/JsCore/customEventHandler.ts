﻿import {HandlerBase} from './handlerBase'

export class CustomEventHandler extends HandlerBase {
    constructor(el: EventTarget
        , eventType: string
        , fn: Function
        , id: string
        , once: boolean
        , drop: Function | undefined = undefined) {
        super(el, eventType, fn, id, once, drop);
    }

    on(): void {
        this.element.addEventListener(this.type, this.action);
    }

    off(): void {
        this.element.removeEventListener(this.type, this.action);
    }
}
