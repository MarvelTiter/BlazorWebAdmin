
function closeWin() {
    window.close()
}

function openWindow(url: string, width: number, height: number, target?: string) {
    const cW = window.outerWidth
    const cH = window.outerHeight
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

function setDarkStyleSheet(href: string) {
    const link: HTMLLinkElement = document.querySelector('link[data-dark]')!
    if (link) {
        link.href = href
    }
    //document.head.appendChild(link)
}

function removeDarkStyleSheet(href: string) {
    for (const link of document.querySelectorAll('link')) {
        if (link.getAttribute('href')?.match(href)) {
            link.removeAttribute('href')
            break;
        }
    }
}

function setTheme(theme: string, url: string) {
    if (theme === 'os') {
        followOsTheme(url)
    } else {
        changeTheme(theme, url)
    }
}

function changeTheme(theme: string, url: string) {
    document.documentElement.dataset.theme = theme
    if (theme === 'dark') {
        setDarkStyleSheet(url)
    } else {
        removeDarkStyleSheet(url)
    }
}

function followOsTheme(url: string) {

}

export default {
    closeWin,
    openWindow,
    getClient,
    setTheme
}