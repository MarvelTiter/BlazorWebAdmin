import { BaseComponent } from "../../JsCore/baseComponent"
import { getComponentById } from "../../JsCore/componentStore"

export class JsTimer extends BaseComponent {
    instance: any
    interval: number
    timer?: number
    constructor(options: any) {
        super()
        this.instance = options.dotNetRef
        this.interval = options.interval
    }
    start() {
        this.timer = window.setInterval(() => {
            this.instance.invokeMethodAsync("Call")
        }, this.interval)
    }

    dispose() {
        if (this.timer) {
            window.clearInterval(this.timer)
            this.timer = undefined
        }
    }

    static init(id: string, options: any) {
        const timter: JsTimer = getComponentById(id, () => {
            return new JsTimer(options)
        })
        timter.start()
    }
}
