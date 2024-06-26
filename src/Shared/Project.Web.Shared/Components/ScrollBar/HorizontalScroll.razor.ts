import { BaseComponent } from "../../JsCore/baseComponent";
import { getComponentById } from "../../JsCore/componentStore";
import { EventHandler } from "../../JsCore/eventHandler";
import { GAP } from "../../JsCore/utils";

export class HorizontalScroll extends BaseComponent {
    wrap: HTMLElement
    wheel: string = ''
    constructor(element: HTMLElement) {
        super()
        this.wrap = element
        this.initEvents()
    }

    handleWheelEvent() {
        let wheel = ''

        if ('onmousewheel' in this.wrap) {
            wheel = 'mousewheel'
        } else if ('onwheel' in this.wrap) {
            wheel = 'wheel'
        } else if ('attachEvent' in window) {
            wheel = 'onmousewheel'
        } else {
            wheel = 'DOMMouseScroll'
        }
        this.wheel = wheel
        EventHandler.listen(this.wrap, wheel, this.scroll)
    }

    scroll = (event: any) => {
        if (this.wrap.clientWidth >= this.wrap.scrollWidth) {
            debugger
            return
        }
        this.wrap.scrollLeft += event.deltaY ? event.deltaY : event.detail && event.detail !== 0 ? event.detail : -event.wheelDelta
    }

    initEvents() {
        this.handleWheelEvent()
    }

    dispose() {
        EventHandler.remove(this.wrap, this.wheel, this.scroll)
    }

    static init(id, element) {
        getComponentById(id, () => {
            return new HorizontalScroll(element)
        })
    }
}
