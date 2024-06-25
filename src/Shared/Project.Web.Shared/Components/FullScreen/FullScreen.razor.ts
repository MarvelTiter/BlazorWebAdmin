import { BaseComponent } from "../../JsCore/baseComponent";
import { getComponentById } from "../../JsCore/componentStore";

export class FullScreen extends BaseComponent {
    element: any
    document: any
    constructor() {
        super()
        this.element = document.documentElement
        this.document = document
    }

    toggle() {
        if (this.isFullscreen()) {
            this.exit()
        } else {
            this.enter()
        }
    }

    enter() {
        this.element.requestFullscreen() ||
            this.element.webkitRequestFullscreen ||
            this.element.mozRequestFullScreen ||
            this.element.msRequestFullscreen;
    }

    exit() {
        if (this.document.exitFullscreen) {
            this.document.exitFullscreen();
        }
        else if (this.document.mozCancelFullScreen) {
            this.document.mozCancelFullScreen();
        }
        else if (this.document.webkitExitFullscreen) {
            this.document.webkitExitFullscreen();
        }
        else if (this.document.msExitFullscreen) {
            this.document.msExitFullscreen();
        }
    }

    isFullscreen() {
        return this.document.fullscreen ||
            this.document.webkitIsFullScreen ||
            this.document.webkitFullScreen ||
            this.document.mozFullScreen ||
            this.document.msFullScreent
    }

    static init(id) {
        getComponentById(id, () => {
            return new FullScreen()
        })
    }

    static toggle(id) {
        const fullscreen = getComponentById(id)
        fullscreen.toggle()
    }
}
