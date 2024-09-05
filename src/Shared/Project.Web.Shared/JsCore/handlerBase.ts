declare global {
    interface EventTarget {
        eventUid: any
    }
}
let eventUid = 1

export function makeUid(el: EventTarget, uid: any = undefined): any {
    return ((uid && `${uid}::${eventUid++}`) || el.eventUid || eventUid++);
}


export class HandlerBase {
    element: EventTarget | Element
    id: string
    delegate: Function
    type: string
    once: boolean
    drop: Function | undefined

    constructor(el: EventTarget | Element
        , eventType: string
        , fn: Function
        , id: string
        , once: boolean
        , drop: Function | undefined = undefined) {
        this.element = el
        this.id = id
        this.delegate = fn
        this.type = eventType
        this.once = once
        this.drop = drop
    }

    action(event: any): void {
        if (this.once) {
            this.off()
            if (this.drop) this.drop()
        }
        this.delegate.apply(this.element, [event]);
    }

    bind(): void {
    }

    off(): void {
    }

}
