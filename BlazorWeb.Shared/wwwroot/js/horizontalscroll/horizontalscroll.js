import { BaseComponent } from "../base/base-component.js"
import { getComponentById } from "../base/component-store.js"
import { EventHandler } from "../base/event-handler.js"
import { Thumb } from "../scrollbar/thumb.js"
import { GAP } from "../scrollbar/util.js"

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
        //EventHandler.listen(this.wrap, 'resize', this.update.bind(this))
        //EventHandler.listen(this.resize, 'resize', this.update.bind(this))
    }

    dispose() {
        EventHandler.remove(this.wrap, this.wheel, this.scroll)
        //EventHandler.remove(this.wrap, 'resize')
        //EventHandler.remove(this.resize, 'resize')
    }

    static init(id, element) {
        const bar = getComponentById(id, () => {
            return new HorizontalScroll(element)
        })
        //bar.horizontal = new Thumb(bar, 'horizontal', tracker, thumb)
        bar.initEvents()
    }
}