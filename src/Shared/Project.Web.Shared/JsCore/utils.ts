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
