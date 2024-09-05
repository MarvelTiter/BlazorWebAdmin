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
    otherClients: string[] = []
    ip: string | undefined
    uuid:string

    constructor(id: string, options: any) {
        super();
        this.channel = new BroadcastChannel("admin_project_ClientHub")
        this.id = id
        this.uuid = getUUID()
        const {interval, dotnetRef} = options
        this.interval = interval
        this.dotnetRef = dotnetRef
    }

    static async init(id: string, options: any) {
        var hub: ClientHub = getComponentById(id, () => new ClientHub(id, options))
        const response = await fetch('/ip.client')
        const ip = await response.text()
        hub.ip = ip
        await hub.init()
    }

    async init() {
         //当前是否有正在发送心跳的ClientHub，如果没有，将当前实例设置为发送心跳的ClientHub
        const main = localStorage.getItem(this.mainKey)
        if (!main) {
            localStorage.setItem(this.mainKey, this.id)
        }
        EventHandler.listen(this.channel, 'message', e => this.receive(e))
        window.onunload = e => this.dispose()
        await this.send()
        this.timer = window.setInterval(async () => {
            await this.send()
        }, this.interval)
    }

    async send() {
        // 广播通知其他ClientHub
        this.channel.postMessage({ id: this.id, action: 'ping' })
        // 检查当前是否有正在发送心跳的ClientHub
        let mainId = localStorage.getItem(this.mainKey)
        if (!mainId || this.otherClients.length === 0) {
            localStorage.setItem(this.mainKey, this.id)
            mainId = this.id
        }
        if (mainId == this.id) {
            // 只有保存在localStorage中的组件id才能发送心跳
            await this.dotnetRef.invokeMethodAsync("Tick", [this.uuid, this.ip, navigator.userAgent])
        }
    }

    receive(e: any) {
        const {id, action} = e.data;
        if (action === 'ping' && this.otherClients.find(v => v === id) === void 0) {
            this.otherClients.push(id);
        } else if (action === 'dispose') {
            const index = this.otherClients.indexOf(id);
            if (index > -1) {
                this.otherClients = this.otherClients.splice(index, 1);
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



function getUUID() {

    let uuid = localStorage.getItem('admin_project_Client_uuid')
    if (uuid) {
        return uuid
    }
    let d = new Date().getTime()
    uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = (d + Math.random() * 16) % 16 | 0
        d = Math.floor(d / 16)
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16)
    });
    localStorage.setItem('admin_project_Client_uuid', uuid)
    return uuid
};