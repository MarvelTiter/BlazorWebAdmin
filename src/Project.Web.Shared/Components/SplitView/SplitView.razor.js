import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/Project.Web.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/Project.Web.Shared/js/jscomponentbase/event-handler.js";
import { success, failed } from "/_content/Project.Web.Shared/js/jscomponentbase/utils.js";
import { startDrag } from "/_content/Project.Web.Shared/js/jscomponentbase/drag-helper.js";

export class SplitView extends BaseComponent {
    constructor(panel1, panel2, separator, direction) {
        super()
        this.panel1 = panel1;
        this.panel2 = panel2;
        this.separator = separator;
        this.direction = direction;
        this.setup();
    }

    setup() {
        EventHandler.listen(this.separator, 'mousedown', this.handleMouseDown.bind(this))
    }

    handleMouseDown(e) {
        e.stopPropagation()
        const wrapRect = this.panel1.parentNode.getBoundingClientRect();
        const separatorRect = this.separator.getBoundingClientRect();
        const separatorOffset = this.direction === 'row' ? e.pageX - separatorRect.left : e.pageY - separatorRect.top;
        const handler = this.direction === 'row' ? this.modeRow : this.modeColumn;
        startDrag(e, event => {
            handler.call(this, event, wrapRect, separatorRect, separatorOffset)
        })
    }

    modeRow(e, wrapRect, spRect, spOffset) {
        const clientRect = this.panel1.getBoundingClientRect()
        const offset = e.pageX - clientRect.left - spOffset + spRect.width / 2;
        const paneLengthPercent = ((offset / wrapRect.width) * 100).toFixed(2);
        this.panel1.style.width = `calc(${paneLengthPercent}% - ${spRect.width / 2}px)`;
    }

    modeColumn(e, wrapRect, spRect, spOffset) {
        const clientRect = this.panel1.getBoundingClientRect()
        const offset = e.pageY - clientRect.top - spOffset + spRect.height / 2;
        const paneLengthPercent = ((offset / wrapRect.height) * 100).toFixed(2);
        this.panel1.style.height = `calc(${paneLengthPercent}% - ${spRect.height / 2}px)`;
    }

    dispose() {
        EventHandler.remove(this.separator, 'mousedown');
    }
}

export function init(id, panel1, panel2, separator, direction) {
    getComponentById(id, () => {
        return new SplitView(panel1, panel2, separator, direction);
    });
}

export function dispose(id) {
    let com = getComponentById(id);
    com.dispose();
}