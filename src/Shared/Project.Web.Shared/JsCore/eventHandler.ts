import {addListener, getElementEvents} from './eventHandlerSet'

export const EventHandler = {
    listen: function (el: EventTarget, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, false)
    },
    once: function (el: EventTarget, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, true)
    },
    remove: function (el: EventTarget, eventType: string, action?: Function) {
        if (!el) return
        const ets = getElementEvents(el);
        ets.removeHandler(eventType, action);
    }
}
