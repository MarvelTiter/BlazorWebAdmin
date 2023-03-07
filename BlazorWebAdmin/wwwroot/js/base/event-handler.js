import { CustomEventHandler } from "./CustomEventHandler.js";
import { ResizeHandler } from "./ResizeHandler.js";

const registry = {};
let eventUid = 1;
function makeUid(el, uid) {
    return ((uid && `${uid}::${eventUid++}`) || el.eventUid || eventUid++);
}
window.r = registry;
function getElementEvents(el) {
    const uid = makeUid(el);
    el.eventUid = uid;
    registry[uid] = registry[uid] || new ElementHandlerSet(el);
    return registry[uid];
}
function removeRegistry(el) {
    const uid = makeUid(el);
    delete registry[uid];
}
class ElementHandlerSet {
    constructor(el) {
        this.element = el;
        this.events = {};
    }

    addHandler(fn, eventType, once) {
        const uid = makeUid(this.element, eventType);
        var handler = new CustomEventHandler(this.element, eventType, fn, uid, once);
        const handlers = this.events[eventType] || {};
        handlers[uid] = handler;
        this.events[eventType] = handlers;
        handler.action = handler.action.bind(handler);
        //this.element.addEventListener(eventType, handler.action);
        handler.on();
    }

    addResizeHandler(fn, once) {
        const uid = makeUid(this.element, 'resize');
        var handler = new ResizeHandler(this.element, fn, uid, once);
        const handlers = this.events['resize'] || {};
        handlers[uid] = handler;
        this.events['resize'] = handlers;
        handler.on();
    }

    removeHandler(eventType, action) {
        const handlers = this.events[eventType];
        if (action) {
            const handler = Object.values(handlers).find(h => h.delegate == action)
            if (handler) {
                handler.off();
                delete this.events[eventType][handler.id]
            }
        } else {
            // 移除所有 eventType 的处理
            for (var h in handlers) {
                handlers[h].off();
                delete this.events[eventType][h]
            }
        }
        // 事件类型为空，移除事件类型
        if (!Object.keys(this.events[eventType]).length) {
            delete this.events[eventType]
        }
        // 事件为空，移除对象
        if (!Object.keys(this.events).length) {
            removeRegistry(this.element)
        }
    }
}

function addListener(el, eventType, action, once) {
    const ets = getElementEvents(el);
    if (eventType == 'resize') {
        ets.addResizeHandler(action, once);
    } else {
        ets.addHandler(action, eventType, once);
    }
}

export const EventHandler = {
    listen: function (el, eventType, action) {
        if (!el) return
        addListener(el, eventType, action, false)
    },
    once: function (el, eventType, action) {
        if (!el) return
        addListener(el, eventType, action, true)
    },
    remove: function (el, eventType, action) {
        if (!el) return
        const ets = getElementEvents(el);
        ets.removeHandler(eventType, action);
    }
}