import { BaseComponent } from "../../JsCore/baseComponent"
import { GAP } from "../../JsCore/utils.ts"
import { ScrollBar } from "./ScrollBar.razor.ts"
import { Thumb } from "./Thumb"

export class Bar extends BaseComponent {
    always: boolean = false
    vertical?: Thumb
    horizontal?: Thumb
    parent: ScrollBar
    constructor(scrollbar: ScrollBar) {
        super()
        this.parent = scrollbar
    }
    update() {
        if (this.vertical && this.vertical.thumb) {
            this.vertical.thumb.style['height'] = this.parent.sizeHeight
        }
        if (this.horizontal && this.horizontal.thumb) {
            this.horizontal.thumb.style['width'] = this.parent.sizeWidth
        }
    }

    handleScroll(e) {
        if (this.parent.wrap) {
            const offsetHeight = this.parent.wrap.offsetHeight - GAP
            const offsetWidth = this.parent.wrap.offsetWidth - GAP
            if (this.vertical)
                this.vertical.setMove(((this.parent.wrap.scrollTop * 100) / offsetHeight) * this.parent.ratioY)
            if (this.horizontal)
                this.horizontal.setMove(((this.parent.wrap.scrollLeft * 100) / offsetWidth) * this.parent.ratioX)
        }
    }

    setVisible(visible) {
        if (this.always) return
        if (this.vertical) this.vertical.setVisible(visible)
        if (this.horizontal) this.horizontal.setVisible(visible)
    }

    dispose() {
        if (this.vertical) this.vertical.dispose()
        if (this.horizontal) this.horizontal.dispose()
    }
}
