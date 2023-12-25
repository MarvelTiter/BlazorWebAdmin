import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/Project.Web.Shared/js/jscomponentbase/component-store.js";

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

    //static init(id) {
    //    getComponentById(id, () => {
    //        return new FullScreen()
    //    })
    //}

    //static toggle(id) {
    //    const fullscreen = getComponentById(id)
    //    fullscreen.toggle()
    //}
}

export function init(id) {
    getComponentById(id, () => {
        return new FullScreen()
    })
}

export function toggle(id) {
    const fullscreen = getComponentById(id)
    fullscreen.toggle()
}