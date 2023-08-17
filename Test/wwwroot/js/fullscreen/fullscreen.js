import { getComponentById } from '../base/component-store.js'
import { BaseComponent } from "../base/base-component.js"

export class FullScreen extends BaseComponent {

    constructor() {
        super()
        this.element = document.documentElement
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
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
        else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        }
        else if (document.webkitExitFullscreen) {
            document.webkitExitFullscreen();
        }
        else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        }
    }

    isFullscreen() {
        return document.fullscreen ||
            document.webkitIsFullScreen ||
            document.webkitFullScreen ||
            document.mozFullScreen ||
            document.msFullScreent
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