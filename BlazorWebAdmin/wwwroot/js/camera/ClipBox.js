import { BaseComponent } from "../base/base-component.js";
import { startDrag } from "../base/drag-helper.js";
import { EventHandler } from "../base/event-handler.js";

export class ClipBox extends BaseComponent {
    constructor(el, width, height) {
        super()
        this.el = el;
        this.w = width * 0.8;
        this.h = height * 0.8;
        this.x = width * 0.1;
        this.y = height * 0.1;
        this.videoWindowWidth = width;
        this.videoWindowHeight = height;
        this.el.style.width = this.w + 'px';
        this.el.style.height = this.h + 'px';
        this.originX = 0;
        this.originY = 0;
        this.scaleWidth = 10;
        this.applyRect();
    }

    applyRect() {
        if (this.x < 0)
            this.x = 0;
        if (this.y < 0)
            this.y = 0;
        if (this.x > this.videoWindowWidth - this.w)
            this.x = this.videoWindowWidth - this.w;
        if (this.y > this.videoWindowHeight - this.h)
            this.y = this.videoWindowHeight - this.h;
        //if (this.el.)
        if (this.el.offsetWidth > 0)
            this.w = this.el.offsetWidth;
        if (this.el.offsetHeight > 0)
            this.h = this.el.offsetHeight;
        this.el.style.top = this.y + 'px';
        this.el.style.left = this.x + 'px';
        // 用于缩放的最大限制
        this.el.style["max-width"] = (this.videoWindowWidth - this.x) + 'px';
        this.el.style["max-height"] = (this.videoWindowHeight - this.y) + 'px';
    }

    canMove(x, y) {
        this.w = this.el.offsetWidth;
        this.h = this.el.offsetHeight;
        return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth;
    }

    setVisible(visible) {
        if (this.el) {
            this.el.style.display = visible ? 'block' : 'none';
        }
    }

    handleMouseDown(e) {
        e.stopPropagation()
        if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
        window.getSelection()?.removeAllRanges();
        var x = e.offsetX;
        var y = e.offsetY;
        if (this.canMove(x, y)) {
            startDrag(e, event => {
                this.x = event.offsetX - x + this.x
                this.y = event.offsetY - y + this.y
                this.applyRect();
            })
        }        
    }

    initEvents() {
        EventHandler.listen(this.el, 'mousedown', this.handleMouseDown.bind(this))
    }
}
