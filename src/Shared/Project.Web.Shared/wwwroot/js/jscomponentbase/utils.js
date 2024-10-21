export function success(msg, payload) {
    return {
        isSuccess: true,
        message: msg,
        payload: payload
    }
}

export function failed(msg) {
    return {
        isSuccess: false,
        message: msg,
    }
}

export const GAP = 4;