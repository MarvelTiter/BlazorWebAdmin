import { BaseComponent } from "../../JsCore/baseComponent";

export class SvgIcon extends BaseComponent {
    static caches: Map<string, string> = new Map<string, string>()
    iconName: string
    className: string
    style: string
    fontSize: string
    el: HTMLElement
    constructor(options: any) {
        super()
        this.el = options.container
        this.iconName = options.iconName
        this.className = options.className
        this.style = options.style
        this.fontSize = options.fontSize

        this.load()
    }

    static async getIcon(name: string) {
        if (!name) return ""
        let icon = SvgIcon.caches.get(name)
        if (icon) {
            return icon
        }
        const response = await fetch(`/icons/${name}.svg`)
        icon = await response.text()
        if (!SvgIcon.caches.has(name)) {
            SvgIcon.caches.set(name, icon)
        }
        return icon
    }

    async load() {
        const icon = await SvgIcon.getIcon(this.iconName)
        this.el.innerHTML = icon
        const svgEl = this.el.getElementsByTagName('svg').item(0)
        if (svgEl) {
            if (this.className) {
                var all = this.className.split(' ')
                for (const c of all) {
                    if (c?.trim()) svgEl.classList.add(c.trim())
                }
            }
            if (this.style) {
                svgEl.setAttribute('style', this.style)
            }
            
        }

    }

    static init(id, options) {
        new SvgIcon(options)
    }
}


//interface SvgInfo {
//    content: string | undefined
//    width: number | undefined
//    height: number | undefined
//}
//export class SvgIcon extends BaseComponent {
//    static caches: Map<string, SvgInfo> = new Map<string, SvgInfo>()

//    static async getIcon(_: string, name: string) {
//        let info = SvgIcon.caches.get(name)
//        if (info) {
//            return info
//        }
//        const response = await fetch(`/icons/${name}.svg`)
//        const content = await response.text()
//        const div = document.createElement('div')
//        div.innerHTML = content
//        const svgEl = div.getElementsByTagName('svg').item(0)
//        const svgPath = div.getElementsByTagName('path').item(0)
//        if (svgPath) {
//            const content = svgPath.outerHTML
//            const viewBox = svgEl?.viewBox
//            info = {
//                content,
//                width: viewBox?.baseVal.width,
//                height: viewBox?.baseVal.height
//            }
//            if (!SvgIcon.caches.has(name)) {
//                SvgIcon.caches.set(name, info)
//            }
//            return success('', info)
//        }
//        return failed('')
//    }
//}