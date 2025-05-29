import { BaseComponent } from '../../JsCore/baseComponent'
import { EventHandler } from '../../JsCore/eventHandler'
import { getComponentById } from '../../JsCore/componentStore'
export class ActionWatcher extends BaseComponent {
    instance: any
    type: number
    timeout: number
    target: HTMLElement
    timer?: number
    constructor(options:any) {
        super()
        this.instance = options.instance
        this.type = options.type
        this.timeout = options.timeout
        this.target = options.target || window.document.documentElement
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
        this.timer = window.setTimeout(() => {
            this.invoke()
        }, this.timeout)
    }

    throttle() {
        if (!this.timer) {
            this.invoke()
            this.timer = window.setTimeout(() => {
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

    static init(id: string, options:any) {
        if (!id){
            console.log('id is not defined')
            return
        }
        const watcher = getComponentById(id, () => {
            return new ActionWatcher(options);
        });
        watcher.start();
    }
}
