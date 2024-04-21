import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/Project.Web.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/Project.Web.Shared/js/jscomponentbase/event-handler.js";
import { success, failed } from "/_content/Project.Web.Shared/js/jscomponentbase/utils.js";
import { startDrag } from "/_content/Project.Web.Shared/js/jscomponentbase/drag-helper.js";

export class SplitView extends BaseComponent {
    constructor(doms, options) {
        super()
        this.panel1 = doms.panel1;
        this.panel2 = doms.panel2;
        this.separator = doms.separator;
        this.direction = options.direction;
        this.max = options.max;
        this.min = options.min;
        this.init = options.initWidth;
        this.drag = false;
        this.setup();
    }

    setup() {
        EventHandler.listen(this.separator, 'mousedown', this.handleMouseDown.bind(this));
        EventHandler.listen(this.panel1.parentNode, "resize", this.refresh.bind(this));
    }

    refresh() {
        if (this.drag) return;
        this.panel1.style.width = this.init;
    }

    handleMouseDown(e) {
        e.stopPropagation()
        const wrapRect = this.panel1.parentNode.getBoundingClientRect();
        const separatorRect = this.separator.getBoundingClientRect();
        const separatorOffset = this.direction === 'row' ? e.pageX - separatorRect.left : e.pageY - separatorRect.top;
        const handler = this.direction === 'row' ? this.modeRow : this.modeColumn;
        this.drag = true;
        startDrag(e, event => {
            handler.call(this, event, wrapRect, separatorRect, separatorOffset)
        }, () => {
            this.drag = false
        })
    }

    modeRow(e, wrapRect, spRect, spOffset) {
        const clientRect = this.panel1.getBoundingClientRect()
        const offset = e.pageX - clientRect.left - spOffset + spRect.width / 2;
        const offsetPercent = getFinalPercent(offset, wrapRect.width, this.max, this.min); //(offset / wrapRect.width) * 100;
        const paneLengthPercent = (offsetPercent).toFixed(2);
        this.panel1.style.width = `calc(${paneLengthPercent}% - ${spRect.width / 2}px)`;
    }

    modeColumn(e, wrapRect, spRect, spOffset) {
        const clientRect = this.panel1.getBoundingClientRect()
        const offset = e.pageY - clientRect.top - spOffset + spRect.height / 2;
        const offsetPercent = getFinalPercent(offset, wrapRect.height, this.max, this.min);
        const paneLengthPercent = (offsetPercent).toFixed(2);
        this.panel1.style.height = `calc(${paneLengthPercent}% - ${spRect.height / 2}px)`;
    }

    dispose() {
        EventHandler.remove(this.separator, 'mousedown');
        EventHandler.remove(this.panel1.parentNode, 'resize');
    }
}

function getFinalPercent(offset, total, max, min) {
    let p = offset / total * 100;
    if (max.endsWith("%")) {
        const l = Number(max.replace("%", ""));
        if (p > l) p = l;
    } else if (max.endsWith("px")) {
        const l = Number(max.replace("px", ""));
        if (offset > l) {
            p = l / total * 100;
        }
    }

    if (min.endsWith("%")) {
        const l = Number(min.replace("%", ""));
        if (p < l) p = l;
    } else if (min.endsWith("px")) {
        const l = Number(min.replace("px", ""));
        if (offset < l) {
            p = l / total * 100;
        }
    }

    return p;
}

export function init(id, doms, options) {
    getComponentById(id, () => {
        return new SplitView(doms, options);
    });
}
