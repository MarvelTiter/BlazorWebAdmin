
function closeWin() {
    window.close()
}

function openWindow(url: string, width: number, height: number, target?: string) {
    const cW = document.body.clientWidth
    const cH = document.body.clientHeight
    let w = cW, h = cH
    if (width <= 1) {
        w = cW * width
    }
    if (height <= 1) {
        h = cH * height
    }
    const top = (cH - h) / 2 + window.screenTop
    const left = (cW - w) / 2 + window.screenLeft
    const features = `popup=yes, left=${left}, top=${top}, width=${w}, height=${h}`
    console.log(features)
    window.open(url, target, features)
}

async function getClient() {
    if (window.opener !== null) {
        return [null, null]
    }
    const response = await fetch('/ip.client')
    const ip = await response.text()
    return [ip, navigator.userAgent]
}

export default {
    closeWin,
    openWindow,
    getClient
}