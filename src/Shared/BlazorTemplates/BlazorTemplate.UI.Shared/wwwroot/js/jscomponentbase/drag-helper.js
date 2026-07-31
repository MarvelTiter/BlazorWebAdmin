import { EventHandler } from "./event-handler.js"
const storeValue = {};
function storeOnSelectstart() {
    storeValue['onselectstart'] = document.onselectstart;
    document.onselectstart = () => false
}

function restoreOnSelectstart() {
    if (document.onselectstart !== storeValue['onselectstart'])
        document.onselectstart = storeValue['onselectstart']
}
export function startDrag(e, moveHandler, upHandler) {
    e.stopImmediatePropagation()
    EventHandler.listen(document, 'mousemove', moveHandler)
    EventHandler.once(document, 'mouseup', e => {
        if (upHandler)
            upHandler()
        EventHandler.remove(document, 'mousemove', moveHandler)
        restoreOnSelectstart()
    })
    storeOnSelectstart()
}