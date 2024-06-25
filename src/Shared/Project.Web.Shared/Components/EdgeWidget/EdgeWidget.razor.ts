import { BaseComponent } from "../../JsCore/baseComponent";
import { getComponentById } from "../../JsCore/componentStore";
import { EventHandler } from "../../JsCore/eventHandler";

export class EdgeWidget extends BaseComponent {
    mask: Element
    childContentContainer: HTMLElement
    trigger: Element
    contentWidth: number
    show: boolean
    constructor(options: any) {
        super()
        this.mask = options.mask
        this.childContentContainer = options.container
        this.trigger = options.trigger
        var containerRect = this.childContentContainer.getBoundingClientRect()
        this.contentWidth = containerRect.width
        this.show = false
        this.childContentContainer.style.left = -this.getWidth() + 'px'
    }
    getWidth(): number {
        return this.contentWidth
    }
    bindEvents() {
        EventHandler.listen(this.trigger, 'click', this.toggle.bind(this))
        EventHandler.listen(this.mask, 'click', this.toggle.bind(this))
    }
    toggle(e: Event) {
        e.stopPropagation()
        this.mask.classList.toggle('show');
        this.childContentContainer.classList.toggle('show');
    }
    dispose(): void {
        EventHandler.remove(this.trigger, 'click')
        EventHandler.remove(this.mask, 'click')
    }
    static init(id: string, options: any) {
        getComponentById(id, () => {
            return new EdgeWidget(options);
        });
    }
}
