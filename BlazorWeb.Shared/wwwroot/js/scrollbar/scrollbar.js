import { BaseComponent } from "../base/base-component.js";
import { getComponentById } from "../base/component-store.js";
import { EventHandler } from "../base/event-handler.js";
import { Bar } from "./bar.js";
import { Thumb } from "./thumb.js";
import { GAP } from "./util.js";

export class ScrollBar extends BaseComponent {
    constructor() {
        super();
        this.root = null;
        this.wrap = null;
        this.resize = null;
        this.minSize = 0;
        this.ratioX = 1;
        this.ratioY = 1;
        this.sizeHeight = '';
        this.sizeWidth = '';
        this.bar = new Bar(this);
    }

    update() {
        if (!this.wrap) return;
        var offsetHeight = this.wrap.offsetHeight - GAP;
        var offsetWidth = this.wrap.offsetWidth - GAP;
        var scrollHeight = this.wrap.scrollHeight;
        var scrollWidth = this.wrap.scrollWidth;
        var originalHeight = offsetHeight * offsetHeight / scrollHeight;
        var originalWidth = offsetWidth * offsetWidth / scrollWidth;
        var height = Math.max(originalHeight, this.minSize);
        var width = Math.max(originalWidth, this.minSize);

        this.ratioY = originalHeight /
            (offsetHeight - originalHeight) /
            (height / (offsetHeight - height));
        this.ratioX = originalWidth /
            (offsetWidth - originalWidth) /
            (width / (offsetWidth - width));

        this.sizeHeight = height + GAP < offsetHeight ? height + 'px' : '';
        this.sizeWidth = width + GAP < offsetWidth ? width + 'px' : '';
        this.bar.update();
    }

    handleMouseMove(e) {
        this.bar.setVisible(true);
    }

    handleMouseLeave(e) {
        this.bar.setVisible(false);
    }

    handleScroll(e) {
        this.bar.handleScroll(e);
    }

    initEvents() {
        EventHandler.listen(this.root, 'mousemove', this.handleMouseMove.bind(this));
        EventHandler.listen(this.root, 'mouseleave', this.handleMouseLeave.bind(this));
        EventHandler.listen(this.wrap, 'scroll', this.handleScroll.bind(this));
        EventHandler.listen(this.wrap, 'resize', (entries) => {
            this.update()
        })
        EventHandler.listen(this.resize, 'resize', () => {
            this.update()
        })
    }

    dispose() {
        //console.log('dispose scrollbar')
        this.bar.dispose()
        EventHandler.remove(this.root, 'mousemove');
        EventHandler.remove(this.root, 'mouseleave');
        EventHandler.remove(this.wrap, 'scroll');
        EventHandler.remove(this.wrap, 'resize');
        EventHandler.remove(this.resize, 'resize');
    }

    static initScrollbar(id, scrollbar, wrap, resize, minSize, always) {
        const component = getComponentById(id, () => {
            return new ScrollBar();
        });
        component.root = scrollbar;
        component.wrap = wrap;
        component.resize = resize;
        component.minSize = minSize;
        component.initEvents();
        component.update();
        if (always) {
            component.bar.setVisible(true);
            component.bar.always = always;
        }
    }

    static initBarInstance(id, position, tracker, thumb) {
        const component = getComponentById(id, () => {
            return new ScrollBar();
        });
        var o = new Thumb(component, position, tracker, thumb);
        o.setVisible(false);
        o.initEvents();
        component.bar[position] = o;
    }
}

//export function initScrollbar(id, scrollbar, wrap, resize, minSize, always) {
//    const component = getComponentById(id, () => {
//        return new ScrollBar();
//    });
//    component.root = scrollbar;
//    component.wrap = wrap;
//    component.resize = resize;
//    component.minSize = minSize;
//    component.initEvents();
//    component.update();
//    if (always) {
//        component.bar.setVisible(true);
//        component.bar.always = always;
//    }
//}

//export function initBarInstance(id, position, tracker, thumb) {
//    const component = getComponentById(id, () => {
//        return new ScrollBar();
//    });
//    var o = new Thumb(component, position, tracker, thumb);
//    o.setVisible(false);
//    o.initEvents();
//    component.bar[position] = o;
//}