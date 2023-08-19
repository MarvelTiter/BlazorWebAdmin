export function success(msg, payload) {
    return {
        success: true,
        message: msg,
        payload: payload
    }
}

export function failed(msg) {
    return {
        success: false,
        message: msg,
    }
}