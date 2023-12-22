import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/BlazorWeb.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/BlazorWeb.Shared/js/jscomponentbase/event-handler.js";
import { GAP } from "/_content/BlazorWeb.Shared/js/jscomponentbase/utils.js"

export class HorizontalScroll extends BaseComponent {

    constructor(element) {
        super();
        this.wrap = element
        this.wheel = ''
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

    update() {
        if (!this.wrap) return;
        var offsetWidth = this.wrap.offsetWidth - GAP;
        var scrollWidth = this.wrap.scrollWidth;
        var originalWidth = offsetWidth * offsetWidth / scrollWidth;
        var width = Math.max(originalWidth, this.minSize);
        this.ratioX = originalWidth /
            (offsetWidth - originalWidth) /
            (width / (offsetWidth - width));

        this.sizeWidth = width + GAP < offsetWidth ? width + 'px' : '';
        if (this.horizontal && this.horizontal.thumb) {
            this.horizontal.thumb.style['width'] = this.sizeWidth ? this.sizeWidth : '0px';
        }
    }

    scroll = (event) => {
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

    //static init(id, element) {
    //    const bar = getComponentById(id, () => {
    //        return new HorizontalScroll(element)
    //    })
    //    bar.initEvents()
    //}
}

export function init(id, element) {
    const bar = getComponentById(id, () => {
        return new HorizontalScroll(element)
    })
    bar.initEvents()
}