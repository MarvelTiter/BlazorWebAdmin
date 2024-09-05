import {HandlerBase, makeUid} from './handlerBase'
import {CustomEventHandler} from './customEventHandler'
import {ResizeHandler} from './resizeHandler'

declare global {
    interface Window {
        allEventsMap: Map<string, ElementHandlerSet>
    }
}
const registry = new Map<string, ElementHandlerSet>()
const RESIZE_EVENT = 'resize'
window.allEventsMap = registry

export function getElementEvents(el: EventTarget): ElementHandlerSet {
    const uid = makeUid(el)
    el.eventUid = uid
    registry[uid] = registry[uid] || new ElementHandlerSet(el)
    return registry[uid]
}

export function removeRegistry(el: EventTarget): void {
    const uid = makeUid(el)
    delete registry[uid]
}

export function addListener(el: EventTarget, eventType: string, action: Function, once: boolean) {
    const ets = getElementEvents(el)

    if (eventType == RESIZE_EVENT) {
        ets.addResizeHandler(action, once)
    } else {
        ets.addHandler(action, eventType, once)
    }
}

export class ElementHandlerSet {
    element: EventTarget
    events: Map<string, Map<string, HandlerBase>>

    constructor(el: EventTarget) {
        this.element = el
        this.events = new Map<string, Map<string, HandlerBase>>()
    }

    addHandler(fn: Function, eventType: string, once: boolean): void {
        const uid = makeUid(this.element, eventType)
        let dropHandler: Function | undefined = undefined
        if (once) {
            dropHandler = () => this.removeHandler(eventType, fn)
        }
        var handler = new CustomEventHandler(this.element, eventType, fn, uid, once, dropHandler)
        const handlers: Map<string, HandlerBase> = this.events.get(eventType) || new Map<string, HandlerBase>()
        // handlers[uid] = handler
        handlers.set(uid, handler)
        this.events.set(eventType, handlers)
        handler.action = handler.action.bind(handler)
        //this.element.addEventListener(eventType, handler.action)
        handler.on()
    }

    addResizeHandler(fn: Function, once: boolean) {
        const uid = makeUid(this.element, RESIZE_EVENT)
        let dropHandler: Function | undefined = undefined
        if (once) {
            dropHandler = () => this.removeHandler(RESIZE_EVENT, fn)
        }
        var handler = new ResizeHandler(this.element, fn, uid, once, dropHandler)
        const handlers: Map<string, HandlerBase> = this.events.get(RESIZE_EVENT) || new Map<string, HandlerBase>()
        handlers.set(uid, handler)
        this.events.set(RESIZE_EVENT, handlers)
        handler.on()
    }

    removeHandler(eventType: string, action?: Function) {
        const handlers = this.events.get(eventType)
        if (handlers == undefined) {
            return
        }
        if (action) {
            const enumerator = handlers.values()
            let r: IteratorResult<HandlerBase>
            while (r = enumerator.next(), !r.done) {
                const handler = r.value
                if (handler.delegate == action) {
                    handler.off()
                    handlers.delete(handler.id)
                    break
                }
            }
            // for (const handler of handlers.values()) {
            //     if (handler.delegate == action) {
            //         handler.off()
            //         handlers.delete(handler.id)
            //         break
            //     }
            // }
        } else {
            // 移除所有 eventType 的处理
            for (var h in handlers) {
                handlers.get(h)?.off()
                handlers.delete(h)
            }
        }
        // 事件类型为空，移除事件类型
        if (handlers.size == 0) {
            this.events.delete(eventType)
        }
        // 事件为空，移除对象
        if (this.events.size == 0) {
            removeRegistry(this.element)
        }
    }
}
