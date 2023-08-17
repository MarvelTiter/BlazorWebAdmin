import { EventHandler } from "./base/event-handler.js"
function c(e) {
    console.log('click first')
}
export function init(dom, h32, h33, button) {
    EventHandler.listen(dom, 'click', c)

    EventHandler.listen(dom, 'click', e => {
        console.log('click second')
    })

    EventHandler.once(dom, 'click', e => {
        console.log('click third')
    })

    EventHandler.listen(button, 'click', e => {
        EventHandler.remove(dom, 'click')
        EventHandler.remove(dom, 'resize')
    })

    EventHandler.listen(dom, 'resize', () => {
        console.log('had resize')
    })
}