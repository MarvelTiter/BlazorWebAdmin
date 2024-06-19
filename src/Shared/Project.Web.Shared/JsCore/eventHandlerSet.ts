import { HandlerBase, makeUid } from './handlerBase'
import { CustomEventHandler } from './customEventHandler'
import { ResizeHandler } from './resizeHandler'

declare global {
    interface Window {
        r: Map<string, ElementHandlerSet>
    }
}
const registry = new Map<string, ElementHandlerSet>()
window.r = registry;
export function getElementEvents(el: Element): ElementHandlerSet {
    const uid = makeUid(el);
    el.eventUid = uid;
    registry[uid] = registry[uid] || new ElementHandlerSet(el);
    return registry[uid];
}
export function removeRegistry(el: Element): void {
    const uid = makeUid(el);
    delete registry[uid];
}

export function addListener(el: Element, eventType: string, action: Function, once: boolean) {
    const ets = getElementEvents(el);

    if (eventType == 'resize') {
        ets.addResizeHandler(action, once);
    } else {
        ets.addHandler(action, eventType, once);
    }
}

export class ElementHandlerSet {
    element: Element
    events: Map<string, Map<string, HandlerBase>>
    constructor(el: Element) {
        this.element = el
        this.events = new Map<string, Map<string, HandlerBase>>()
    }

    addHandler(fn: Function, eventType: string, once: boolean): void {
        const uid = makeUid(this.element, eventType);
        let dropHandler: Function = undefined
        if (once) {
            dropHandler = () => this.removeHandler(eventType, fn)
        }
        var handler = new CustomEventHandler(this.element, eventType, fn, uid, once, dropHandler);
        const handlers: Map<string, HandlerBase> = this.events[eventType] || new Map<string, HandlerBase>();
        handlers[uid] = handler;
        this.events[eventType] = handlers;
        handler.action = handler.action.bind(handler);
        //this.element.addEventListener(eventType, handler.action);
        handler.on();
    }

    addResizeHandler(fn: Function, once: boolean) {
        const uid = makeUid(this.element, 'resize');
        let dropHandler: Function = undefined
        if (once) {
            dropHandler = () => this.removeHandler('resize', fn)
        }
        var handler = new ResizeHandler(this.element, fn, uid, once, dropHandler);
        const handlers = this.events['resize'] || {};
        handlers[uid] = handler;
        this.events['resize'] = handlers;
        handler.on();
    }

    removeHandler(eventType: string, action: Function) {
        const handlers: Map<string, HandlerBase> = this.events[eventType];
        if (action) {
            for (const handler of handlers.values()) {
                if (handler.delegate == action) {
                    handler.off()
                    handlers.delete(handler.id)
                    break
                }

            }
        } else {
            // 移除所有 eventType 的处理
            for (var h in handlers) {
                handlers[h].off()
                handlers.delete(h)
            }
        }
        // 事件类型为空，移除事件类型
        if (!Object.keys(this.events[eventType]).length) {
            this.events.delete(eventType)
        }
        // 事件为空，移除对象
        if (!Object.keys(this.events).length) {
            removeRegistry(this.element)
        }
    }
}