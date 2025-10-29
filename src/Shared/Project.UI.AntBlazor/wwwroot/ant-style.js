const darkQuery = window.matchMedia("(prefers-color-scheme: dark)")
const COMPACT_KEY = "blazor-admin-ant-style-is-compact";
function setDarkStyleSheet(href) {
    const link = document.querySelector('link[data-dark]')
    if (link) {
        link.href = href
    }
    //document.head.appendChild(link)
}

function removeDarkStyleSheet(href) {
    for (const link of document.querySelectorAll('link')) {
        if (link.getAttribute('href')?.match(href)) {
            link.removeAttribute('href')
            break;
        }
    }
}

function changeTheme(theme, url) {
    document.documentElement.dataset.theme = theme
    if (theme === 'dark') {
        setDarkStyleSheet(url)
    } else {
        removeDarkStyleSheet(url)
    }
}

function getOsThemeChangedHandler(url) {
    const darkUrl = url
    return function () {
        if (darkQuery.matches) {
            changeTheme('dark', darkUrl)
        } else {
            changeTheme('light', darkUrl)
        }
    }
}

let osThemeChangedHandler = undefined;

window.setTheme = function (theme, url) {
    if (theme === 'os') {
        osThemeChangedHandler = getOsThemeChangedHandler(url)
        darkQuery.addEventListener("change", osThemeChangedHandler)
        osThemeChangedHandler()
    } else {
        if (osThemeChangedHandler) {
            darkQuery.addEventListener("change", osThemeChangedHandler)
            osThemeChangedHandler = undefined
        }
        changeTheme(theme, url)
    }
}

window.updateTheme = function (map) {
    const keys = Object.keys(map)
    for (let i = 0; i < keys.length; i++) {
        const myName = keys[i]
        const otherName = map[myName]
        document.documentElement.style.setProperty(myName, `var(${otherName})`)
    }
}

// 页面加载时初始化
document.addEventListener('DOMContentLoaded', function () {

});

window.setCompactStyleSheet = function (href) {
    const link = document.querySelector('link[data-compact]')
    if (link) {
        link.href = href
    }
}

window.removeCompactStyleSheet = function (href) {
    const link = document.querySelector('link[data-compact]')
    if (link) {
        link.removeAttribute('href')
    }
}

