import { BaseComponent } from "../../JsCore/baseComponent";
import { getComponentById } from "../../JsCore/componentStore";
import { startDrag } from "../../JsCore/dragHelper";
import { EventHandler } from "../../JsCore/eventHandler";

export class SplitView extends BaseComponent {
    panel1: HTMLElement
    panel2: HTMLElement
    separator: HTMLElement
    direction: 'row' | 'column'
    max: number | string
    min: number | string
    init: string
    drag: boolean = false
    constructor(doms: any, options: any) {
        super()
        this.panel1 = doms.panel1
        this.panel2 = doms.panel2
        this.separator = doms.separator
        this.direction = options.direction
        this.max = options.max
        this.min = options.min
        this.init = options.initWidth
        this.setup()
    }
    setup() {
        EventHandler.listen(this.separator, 'mousedown', this.handleMouseDown.bind(this));
        if (this.panel1.parentElement)
            EventHandler.listen(this.panel1.parentElement, "resize", this.refresh.bind(this));
    }

    refresh() {
        if (this.drag) return;
        this.panel1.style.width = this.init;
    }

    handleMouseDown(e) {
        e.stopPropagation()
        const wrapRect = this.panel1.parentElement?.getBoundingClientRect();
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
        if (this.panel1.parentElement)
            EventHandler.remove(this.panel1.parentElement, 'resize');
    }

    static init(id, doms, options) {
        getComponentById(id, () => {
            return new SplitView(doms, options);
        });
    }
}

function getFinalPercent(offset: number, total: number, max: any, min: any) {
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
