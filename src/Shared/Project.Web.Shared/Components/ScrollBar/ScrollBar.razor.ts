import { BaseComponent } from "../../JsCore/baseComponent"
import { getComponentById } from "../../JsCore/componentStore"
import { EventHandler } from "../../JsCore/eventHandler"
import { GAP } from "../../JsCore/utils"
import { Bar } from "./Bar"
import { Thumb } from "./Thumb"

export class ScrollBar extends BaseComponent {
    root: HTMLElement
    wrap: HTMLElement
    resize: HTMLElement
    minSize: number
    ratioX: number = 1
    ratioY: number = 1
    sizeHeight: string = ''
    sizeWidth: string = ''
    bar: Bar

    constructor(options: any) {
        super()
        this.root = options.scrollbar
        this.wrap = options.wrap
        this.resize = options.resize
        this.minSize = options.minSize
        this.bar = new Bar(this)
        for (const barOption of options.bars) {
            const { position, tracker, thumb } = barOption
            const o = new Thumb(this, position, tracker, thumb)
            this.bar[position] = o
        }
        if (options.always) {
            this.bar.setVisible(true)
            this.bar.always = options.always
        }
    }

    update() {
        if (!this.wrap) return
        var offsetHeight = this.wrap.offsetHeight - GAP
        var offsetWidth = this.wrap.offsetWidth - GAP
        var scrollHeight = this.wrap.scrollHeight
        var scrollWidth = this.wrap.scrollWidth
        var originalHeight = offsetHeight * offsetHeight / scrollHeight
        var originalWidth = offsetWidth * offsetWidth / scrollWidth
        var height = Math.max(originalHeight, this.minSize)
        var width = Math.max(originalWidth, this.minSize)

        this.ratioY = originalHeight /
            (offsetHeight - originalHeight) /
            (height / (offsetHeight - height))
        this.ratioX = originalWidth /
            (offsetWidth - originalWidth) /
            (width / (offsetWidth - width))

        this.sizeHeight = height + GAP < offsetHeight ? height + 'px' : ''
        this.sizeWidth = width + GAP < offsetWidth ? width + 'px' : ''
        this.bar.update()
    }

    handleMouseMove(e) {
        this.bar.setVisible(true)
    }

    handleMouseLeave(e) {
        this.bar.setVisible(false)
    }

    handleScroll(e) {
        this.bar.handleScroll(e)
    }

    initEvents() {
        EventHandler.listen(this.root, 'mousemove', this.handleMouseMove.bind(this))
        EventHandler.listen(this.root, 'mouseleave', this.handleMouseLeave.bind(this))
        EventHandler.listen(this.wrap, 'scroll', this.handleScroll.bind(this))
        EventHandler.listen(this.wrap, 'resize', (entries) => {
            this.update()
        })
        EventHandler.listen(this.resize, 'resize', () => {
            this.update()
        })
    }

    dispose() {
        //console.log('dispose scrollbar')
        this.bar.dispose()
        EventHandler.remove(this.root, 'mousemove')
        EventHandler.remove(this.root, 'mouseleave')
        EventHandler.remove(this.wrap, 'scroll')
        EventHandler.remove(this.wrap, 'resize')
        EventHandler.remove(this.resize, 'resize')
    }

    // scrollbar, wrap, resize, minSize, always
    static init(id: string, options: any) {
        getComponentById(id, () => {
            return new ScrollBar(options)
        })
    }
}
