export class NavTabs {
    static getMenuWidth() {
        var menu = window.document.querySelector(".nav-menu") as HTMLElement;
        return menu?.offsetWidth;
    }
}