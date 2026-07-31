const darkQuery = window.matchMedia("(prefers-color-scheme: dark)")
const COMPACT_KEY = "blazor-admin-ant-style-is-compact";
const STYLE_KEY = "ant-blazor-css-style-mode";
function setDarkStyleSheet(href) {
    const link = document.querySelector('link[data-dark]')
    if (link) {
        link.href = href
        localStorage.setItem(STYLE_KEY, JSON.stringify({
            mode: 'dark',
            url: href
        }))
    }
}

function removeDarkStyleSheet(href) {
    const link = document.querySelector('link[data-dark]')
    if (link) {
        link.removeAttribute('href')
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

window.setCompactStyleSheet = function (href) {
    const link = document.querySelector('link[data-compact]')
    if (link) {
        link.href = href
        localStorage.setItem(STYLE_KEY, JSON.stringify({
            mode: 'compact',
            url: href
        }))
    }
}

window.removeCompactStyleSheet = function (href) {
    const link = document.querySelector('link[data-compact]')
    if (link) {
        link.removeAttribute('href')
        localStorage.removeItem(STYLE_KEY)
    }
}

function initializeStyle() {
    const style = localStorage.getItem(STYLE_KEY);
    if (!style) {
        return;
    }
    const styleObj = JSON.parse(style);
    if (styleObj.mode === 'dark') {
        changeTheme('dark', styleObj.url);
    } else if (styleObj.mode === 'compact') {
        setCompactStyleSheet(styleObj.url);
    }
}

// 如果文档已经加载完成，立即执行
if (document.readyState === 'loading') {
    // 文档还在加载，等加载完成再执行
    document.addEventListener('DOMContentLoaded', initializeStyle);
} else {
    // 文档已经加载完成，立即执行
    initializeStyle();
}

