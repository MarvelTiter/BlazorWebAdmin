import { getComponentById, removeComponent } from "./component-store.js"

export class BaseComponent {

    dispose() {

    }
    static dispose(id) {
        const com = getComponentById(id);
        if (com) {
            //console.log(id, com)
            com.dispose();
            removeComponent(id);
        }
    }

    static init() {

    }
}