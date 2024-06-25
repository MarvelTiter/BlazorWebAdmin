import { getElementEvents, addListener } from './eventHandlerSet'

export const EventHandler = {
    listen: function (el: HTMLElement, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, false)
    },
    once: function (el: HTMLElement, eventType: string, action: Function) {
        if (!el) return
        addListener(el, eventType, action, true)
    },
    remove: function (el: HTMLElement, eventType: string, action?: Function) {
        if (!el) return
        const ets = getElementEvents(el);
        ets.removeHandler(eventType, action);
    }
}
