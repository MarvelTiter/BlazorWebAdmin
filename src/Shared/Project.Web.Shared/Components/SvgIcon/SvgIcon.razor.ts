import { BaseComponent } from "../../JsCore/baseComponent";

export class SvgIcon extends BaseComponent {
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

    load() {
        fetch(`/icons/${this.iconName}.svg`).then(response => {
            response.text().then(content => {
                this.el.innerHTML = content
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
            })
        }).catch(error => {
            console.debug(error)
        })
    }

    static init(id, options) {
        new SvgIcon(options)
    }
}