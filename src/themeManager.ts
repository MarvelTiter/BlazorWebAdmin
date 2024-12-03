const darkQuery = window.matchMedia("(prefers-color-scheme: dark)")

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

function getOsThemeChangedHandler(url: string): () => void {
    const darkUrl = url
    return function () {
        if (darkQuery.matches) {
            changeTheme('dark', darkUrl)
        } else {
            changeTheme('light', darkUrl)
        }
    }
}

let osThemeChangedHandler: (() => void) | undefined;

export function setTheme(theme: string, url: string) {
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