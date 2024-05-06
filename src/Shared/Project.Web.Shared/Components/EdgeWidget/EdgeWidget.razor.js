import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/Project.Web.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/Project.Web.Shared/js/jscomponentbase/event-handler.js";
export class EdgeWidget extends BaseComponent {
    constructor(options) {
        super();
        this.mask = options.mask;
        this.childContentContainer = options.container;
        this.trigger = options.trigger;
        var containerRect = this.childContentContainer.getBoundingClientRect();
        this.contentWidth = containerRect.width;
        this.show = false;
        this.childContentContainer.style.left = -this.getWidth() + 'px';
        this.bindEvents()
    }

    getWidth() {
        return this.contentWidth;
    }

    bindEvents() {
        EventHandler.listen(this.trigger, 'click', this.toggle.bind(this));
        EventHandler.listen(this.mask, 'click', this.toggle.bind(this));
    }

    toggle(e) {
        e.stopPropagation();
        this.mask.classList.toggle('show');
        this.childContentContainer.classList.toggle('show');
    }

    dispose() {
        EventHandler.remove(this.trigger, 'click');
        EventHandler.remove(this.mask, 'click');
    }
}

export function init(id, options) {
    getComponentById(id, () => {
        return new EdgeWidget(options);
    });
}