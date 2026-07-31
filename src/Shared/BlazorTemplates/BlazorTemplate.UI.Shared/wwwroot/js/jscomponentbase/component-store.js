const alls = {}
window.a = alls
export function getComponentById(id, init) {
    if (!alls[id] && init) {
        if (typeof init == 'function') {
            alls[id] = init();
        } else if (typeof init == 'object') {
            alls[id] = init;
        } else {
            console.error('初始化异常', init);
            throw new Error('初始化异常');
        }
    }
    return alls[id];
}

export function removeComponent(id) {
    delete alls[id];
}