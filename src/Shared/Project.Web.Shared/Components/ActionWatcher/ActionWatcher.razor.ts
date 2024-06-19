import { BaseComponent } from '../../JsCore/baseComponent'
import { EventHandler } from '../../JsCore/eventHandler'
import { getComponentById } from '../../JsCore/componentStore'
export class ActionWatcher extends BaseComponent {
    instance: any
    type: number
    timeout: number
    target: Element
    timer?: number
    constructor(instance: any, type: number, timeout: number, target: Element = undefined) {
        super()
        this.instance = instance
        this.type = type
        this.timeout = timeout
        this.target = target || window.document.documentElement
    }

    start(): void {
        if (this.type == 1) {
            EventHandler.listen(this.target, "mousemove", this.debounce.bind(this))
            EventHandler.listen(this.target, "keydown", this.debounce.bind(this))
        } else if (this.type == 2) {
            EventHandler.listen(this.target, "mousemove", this.throttle.bind(this))
            EventHandler.listen(this.target, "keydown", this.throttle.bind(this))
        }
    }

    debounce() {
        clearTimeout(this.timer)
        this.timer = setTimeout(() => {
            this.invoke()
        }, this.timeout)
    }

    throttle() {
        if (!this.timer) {
            this.invoke()
            this.timer = setTimeout(() => {
                this.timer = undefined
            }, this.timeout)
        }
    }
    invoke() {
        this.instance.invokeMethodAsync('Call')
    }

    dispose() {
        EventHandler.remove(this.target, "mousemove")
        EventHandler.remove(this.target, "keydown")
        window.clearTimeout(this.timer)
        this.timer = undefined
    }

    static init(id: string, dotNetRef: any, type: number, timeout: number, element?: Element) {
        var watcher = getComponentById(id, () => {
            return new ActionWatcher(dotNetRef, type, timeout, element);
        });
        watcher.start();
    }
}