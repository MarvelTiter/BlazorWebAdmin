import { EventHandler } from './eventHandler'
const storeValue = {};
function storeOnSelectstart() {
    storeValue['onselectstart'] = document.onselectstart;
    document.onselectstart = () => false
}

function restoreOnSelectstart() {
    if (document.onselectstart !== storeValue['onselectstart'])
        document.onselectstart = storeValue['onselectstart']
}
export function startDrag(e: Event, moveHandler: Function, upHandler: Function) {
    e.stopImmediatePropagation()
    EventHandler.listen(document.documentElement, 'mousemove', moveHandler)
    EventHandler.once(document.documentElement, 'mouseup', e => {
        if (upHandler)
            upHandler()
        EventHandler.remove(document.documentElement, 'mousemove', moveHandler)
        restoreOnSelectstart()
    })
    storeOnSelectstart()
}