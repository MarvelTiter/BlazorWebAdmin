declare global {
    interface Window {
        BlazorProject: any
        Utils: any
    }
}

export interface IJsActionResult {
    isSuccess: boolean,
    message: string | undefined,
    payload: any
}
export function success(msg: string | undefined, payload?: any): IJsActionResult {
    return {
        isSuccess: true,
        message: msg,
        payload: payload
    }
}

export function failed(msg: string | undefined): IJsActionResult {
    return {
        isSuccess: false,
        message: msg,
        payload: null
    }
}

export const GAP = 4;

export function mergeUtils(utilsObject: object) {
    if (window.Utils) {
        const ou = window.Utils
        window.Utils = {
            ...ou,
            ...utilsObject
        }
    } else {
        window.Utils = utilsObject
    }
}

export function mergeComponents(components: object) {
    if (window.BlazorProject) {
        const ob = window.BlazorProject
        window.BlazorProject = {
            ...ob,
            ...components
        }
    } else {
        window.BlazorProject = components;
    }
}
