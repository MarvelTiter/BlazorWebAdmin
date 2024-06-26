export interface IJsActionResult {
    success: boolean,
    message: string | undefined,
    payload: any
}
export function success(msg: string | undefined, payload?: any): IJsActionResult {
    return {
        success: true,
        message: msg,
        payload: payload
    }
}

export function failed(msg: string | undefined): IJsActionResult {
    return {
        success: false,
        message: msg,
        payload: null
    }
}

export const GAP = 4;
