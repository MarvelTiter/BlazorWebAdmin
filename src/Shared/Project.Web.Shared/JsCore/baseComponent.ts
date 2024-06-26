import { getComponentById, removeComponent } from './componentStore'

export class BaseComponent {
    dispose(): void {

    }

    static dispose(id: string): void {
        const com = getComponentById(id)
        if (com) {
            console.debug('dispose: ', id, com)
            com.dispose()
            removeComponent(id)
        }
    }
}