import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { startDrag } from "/_content/Project.Web.Shared/js/jscomponentbase/drag-helper.js";
import { EventHandler } from "/_content/Project.Web.Shared/js/jscomponentbase/event-handler.js";
import { GAP } from "/_content/Project.Web.Shared/js/jscomponentbase/utils.js";

const BAR_MAP = {
    vertical: {
        offset: 'offsetHeight',
        scroll: 'scrollTop',
        scrollSize: 'scrollHeight',
        size: 'height',
        key: 'vertical',
        axis: 'Y',
        client: 'clientY',
        direction: 'top',
        ratio: 'ratioY'
    },
    horizontal: {
        offset: 'offsetWidth',
        scroll: 'scrollLeft',
        scrollSize: 'scrollWidth',
        size: 'width',
        key: 'horizontal',
        axis: 'X',
        client: 'clientX',
        direction: 'left',
        ratio: 'ratioX'
    },
}

export class Thumb extends BaseComponent {
    constructor(root, position, tracker, thumb) {
        super();
        this.root = root;
        this.position = position;
        this.cursorDown = false;
        this.cursorLeave = false;
        this.tracker = tracker;
        this.thumb = thumb;
        this.map = BAR_MAP[position];
        this.state = {
            X: 0,
            Y: 0,
        };
        this.bindEvents = {};
    }

    mouseMoveDocumentHandler(e) {
        if (!this.tracker || !this.thumb) return;
        if (this.cursorDown === false) return;

        const prevPage = this.state[this.map.axis];
        if (!prevPage) return;
        const offset = (this.tracker.getBoundingClientRect()[this.map.direction] - e[this.map.client]) * -1;
        const thumbClickPosition = this.thumb[this.map.offset] - prevPage;
        const thumbPositionPercentage = ((offset - thumbClickPosition) * 100 * this.root[this.map.ratio]) / this.tracker[this.map.offset];
        this.root.wrap[this.map.scroll] = (thumbPositionPercentage * this.root.wrap[this.map.scrollSize]) / 100;
        this.updateMove();
    }

    clickTrackerHandler(e) {
        if (!this.thumb || !this.tracker || !this.root.wrap) return;
        // 点击位置在滚动条中的偏移
        const offset = Math.abs(e.target.getBoundingClientRect()[this.map.direction] - e[this.map.client]);
        const thumbHalf = this.thumb[this.map.offset] / 2;
        const thumbPositionPercentage = ((offset - thumbHalf) * 100 * this.root[this.map.ratio]) / this.tracker[this.map.offset];
        this.root.wrap[this.map.scroll] = (thumbPositionPercentage * this.root.wrap[this.map.scrollSize]) / 100;
    }
    clickThumbHandler(e) {
        e.stopPropagation();
        if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
        if (window.getSelection()) {
            window.getSelection().removeAllRanges();
        }
        this.cursorDown = true
        startDrag(e, this.mouseMoveDocumentHandler.bind(this), () => {
            this.cursorDown = false
            this.state[this.map.axis] = 0
        })
        const el = e.currentTarget;
        if (!el) return;
        this.state[this.map.axis] = el[this.map.offset] - (e[this.map.client] - el.getBoundingClientRect()[this.map.direction]);
    }

    mouseUpDocumentHandler(e) {
        this.cursorDown = false
        this.state[this.map.axis] = 0
    }
    setMove(move) {
        this.thumb.style.transform = `translate${this.map.axis}(${move}%)`
    }
    updateMove() {
        const offset = this.root.wrap[this.map.offset] - GAP;
        this.setMove(((this.root.wrap[this.map.scroll] * 100) / offset) * this.root[this.map.ratio]);
    }
    setVisible(visible) {
        if (this.tracker) this.tracker.style.display = visible ? 'block' : 'none';
    }
    initEvents() {
        EventHandler.listen(this.tracker, 'mousedown', this.clickTrackerHandler.bind(this));
        EventHandler.listen(this.thumb, 'mousedown', this.clickThumbHandler.bind(this));
    }

    dispose() {
        //console.log(this.position, 'dispose thumb')
        EventHandler.remove(this.tracker, 'mousedown')
        EventHandler.remove(this.thumb, 'mousedown')
    }
}