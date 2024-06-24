const alls = new Map<string, any>()
window['a'] = alls

export function getComponentById(id: string, init: Function | Object | undefined = undefined) {
    if (!alls.has(id) && init !== undefined) {
        if (init instanceof Function) {
            alls.set(id, init())
        }
        else if (init instanceof Object) {
            alls.set(id, init)
        } else {
            console.error('初始化异常', init)
            throw new Error('初始化异常')
        }
    }
    if (alls.has(id))
        return alls.get(id)
    else
        return undefined
}

export function removeComponent(id: string): void {
    delete alls[id];
}
