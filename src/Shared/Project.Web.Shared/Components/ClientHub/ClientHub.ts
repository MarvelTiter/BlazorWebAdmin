import {BaseComponent} from "../../JsCore/baseComponent.ts";
import {EventHandler} from "../../JsCore/eventHandler.ts";
import {getComponentById} from "../../JsCore/componentStore.ts";

export class ClientHub extends BaseComponent {
    channel: BroadcastChannel
    mainKey: string = 'admin_project_ClientHub_main'
    id: string
    timer: number | undefined
    interval: number
    dotnetRef: any
    allClients: string[] = []

    constructor(id: string, options: any) {
        super();
        this.channel = new BroadcastChannel("admin_project_ClientHub")
        this.id = id
        const {interval, dotnetRef} = options
        this.interval = interval
        this.dotnetRef = dotnetRef
        this.init()
    }

    static async init(id: string, options: any) {
        getComponentById(id, () => new ClientHub(id, options))
        const response = await fetch('/ip.client')
        const ip = await response.text()
        return [ip, navigator.userAgent]
    }

    init() {
        const main = localStorage.getItem(this.mainKey)
        if (!main) {
            localStorage.setItem(this.mainKey, this.id)
        }
        const _ = this.send()
        EventHandler.listen(this.channel, 'message', this.receive.bind(this))
        this.timer = window.setInterval(async () => {
            await this.send()
        }, this.interval)
    }

    async send() {
        this.channel.postMessage({id: this.id, action: 'ping'})
        let mainId = localStorage.getItem(this.mainKey)
        if (!mainId || this.allClients.length === 0) {
            localStorage.setItem(this.mainKey, this.id)
            mainId = this.id
        }
        if (mainId == this.id) {
            // 只有保存在localStorage中的组件id才能发送心跳
            await this.dotnetRef.invokeMethodAsync("Tick")
        }
    }

    receive(data: any) {
        const {id, action} = data;
        if (action === 'ping' && this.allClients.find(v => v === id) === void 0) {
            this.allClients.push(id);
        } else if (action === 'dispose') {
            const index = this.allClients.indexOf(id);
            if (index > -1) {
                this.allClients = this.allClients.splice(index, 1);
            }
            if (localStorage.getItem(this.mainKey) === id) {
                localStorage.removeItem(this.mainKey);
            }
        }
    }

    dispose() {
        window.clearInterval(this.timer)
        EventHandler.remove(this.channel, 'message')
        this.channel.postMessage({id: this.id, action: 'dispose'})
        this.channel.close();
    }
}