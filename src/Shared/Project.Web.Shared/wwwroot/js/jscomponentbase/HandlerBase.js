export class HandlerBase {
    constructor(el, eventType, fn, id, once) {
        this.element = el;
        this.id = id;
        this.delegate = fn;
        this.type = eventType;
        this.once = once;
    }
    action(event) {
    }
    bind() {
    }
}
