import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { GAP } from "/_content/BlazorWeb.Shared/js/jscomponentbase/utils.js";

export class Bar extends BaseComponent {
    constructor(scrollbar) {
        super();
        this.always = false;
        this.vertical = null;
        this.horizontal = null;
        this.parent = scrollbar;
    }

    update() {
        if (this.vertical && this.vertical.thumb) {
            this.vertical.thumb.style['height'] = this.parent.sizeHeight;
        }
        if (this.horizontal && this.horizontal.thumb) {
            this.horizontal.thumb.style['width'] = this.parent.sizeWidth;
        }
    }

    handleScroll(e) {
        if (this.parent.wrap) {
            const offsetHeight = this.parent.wrap.offsetHeight - GAP
            const offsetWidth = this.parent.wrap.offsetWidth - GAP
            if (this.vertical)
                this.vertical.setMove(((this.parent.wrap.scrollTop * 100) / offsetHeight) * this.parent.ratioY);
            if (this.horizontal)
                this.horizontal.setMove(((this.parent.wrap.scrollLeft * 100) / offsetWidth) * this.parent.ratioX);
        }
    }

    setVisible(visible) {
        if (this.always) return;
        if (this.vertical) this.vertical.setVisible(visible);
        if (this.horizontal) this.horizontal.setVisible(visible);
    }

    dispose() {
        if (this.vertical) this.vertical.dispose();
        if (this.horizontal) this.horizontal.dispose();
    }
}