import { getElementEvents, addListener } from './eventHandlerSet'

export const EventHandler = {
    listen: function (el: Element, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, false)
    },
    once: function (el: Element, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, true)
    },
    remove: function (el: Element, eventType: string, action?: Function) {
        if (!el) return
        const ets = getElementEvents(el);
        ets.removeHandler(eventType, action);
    }
}