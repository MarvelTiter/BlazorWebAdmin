const allComponentMap = new Map<string, any>()
declare global {
    interface Window {
        allComponentMap: Map<string, any>
    }
}
window.allComponentMap = allComponentMap

export function getComponentById(id: string, init?: Function | Object) {
    if (!allComponentMap.has(id) && init !== undefined) {
        if (init instanceof Function) {
            allComponentMap.set(id, init())
        } else if (init) {
            allComponentMap.set(id, init)
        } else {
            console.error('初始化异常', init)
            throw new Error('初始化异常')
        }
    }
    if (allComponentMap.has(id))
        return allComponentMap.get(id)
    else
        return undefined
}

export function removeComponent(id: string): void {
    allComponentMap.delete(id)
}
