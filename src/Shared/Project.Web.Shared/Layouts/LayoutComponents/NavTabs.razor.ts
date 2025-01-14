import {BaseComponent} from "../../JsCore/baseComponent.ts";
import {getComponentById} from "../../JsCore/componentStore.ts";
import {EventHandler} from "../../JsCore/eventHandler.ts";

export class NavTabs extends BaseComponent {
    tabsContainer: HTMLElement
    leftButton: HTMLElement
    rightButton: HTMLElement
    observer: MutationObserver | null = null
    activeTab: HTMLElement | null = null

    constructor(options: any) {
        super();
        this.tabsContainer = options.tabsContainer
        this.leftButton = options.leftButton
        this.rightButton = options.rightButton
        this.setup()
    }

    setup() {
        EventHandler.listen(this.tabsContainer, 'resize', (e) => {
            this.sizeChanged()
        })

        this.observer = new MutationObserver((mutationsList) => {
            for (const mutation of mutationsList) {
                // 只关心容器的元素数量变化
                if (mutation.type === 'childList'
                    && mutation.target === this.tabsContainer) {
                    this.sizeChanged()
                    this.checkActiveChanged(undefined)
                } else if (mutation.type === 'attributes') {
                    // 只关心子div子元素的class变化
                    if (mutation.target.nodeName === 'DIV'
                        && mutation.attributeName === 'class') {
                        const target = mutation.target as HTMLElement
                        this.checkActiveChanged(target)
                    }
                }
            }
        })
        this.observer.observe(this.tabsContainer, {childList: true, attributes: true, subtree: true})
    }

    sizeChanged() {
        const maxWidth = this.tabsContainer.clientWidth
        const actualWidth = this.tabsContainer.scrollWidth
        if (actualWidth > maxWidth) {
            this.leftButton.classList.remove('hidden')
            this.rightButton.classList.remove('hidden')
        } else if (actualWidth <= maxWidth) {
            this.leftButton.classList.add('hidden')
            this.rightButton.classList.add('hidden')
        }

    }

    checkActiveChanged(target: HTMLElement | undefined) {
        const activeTab =
            target === undefined ?
                this.tabsContainer.querySelector<HTMLElement>('.nav-top.active')
                : (target.classList.contains('active') ? target : null)
        if (this.activeTab) {
            this.fixedPosition()
        }
        if (activeTab === null) return
        if (this.activeTab !== activeTab) {
            // console.log('active changed')
            this.activeTab = activeTab
            window['activeTab'] = this.activeTab
            this.fixedPosition()
        }
        // console.log('active tab', this.activedTab)
    }

    fixedPosition() {
        if (!this.activeTab) return
        const totalWidth = this.tabsContainer.scrollWidth
        const winWidth = this.tabsContainer.clientWidth
        const scrollLeft = this.tabsContainer.scrollLeft
        if (winWidth === totalWidth) return
        const width = this.activeTab.clientWidth
        const left = this.activeTab.offsetLeft
        const rect = this.activeTab.getBoundingClientRect()
        const box = this.tabsContainer.getBoundingClientRect()
        console.log(`offsetLeft: ${left}, clientWidth: ${width}, totalWidth: ${totalWidth}, viewportWidth: ${winWidth}, scrollLeft: ${scrollLeft})`)
        console.log('activeTab BoundingClientRect', rect)
        console.log('tabsContainer BoundingClientRect', box)
        if (left + width > winWidth) {
            console.log('右边遮挡')
            this.tabsContainer.scrollTo({
                left: left + width,
                behavior: "smooth"
            })
        } else if (totalWidth - left > winWidth) {
            console.log('左边遮挡')
            this.tabsContainer.scrollTo({
                left,
                behavior: "smooth"
            })
        }
    }

    dispose() {
        EventHandler.remove(this.tabsContainer, 'resize');
        this.observer?.disconnect()
        this.observer = null;
    }

    static getMenuWidth() {
        const menu = window.document.querySelector(".nav-menu") as HTMLElement;
        return menu?.offsetWidth
    }

    static init(id: string, options: any) {
        const com = getComponentById(id, () => {
            return new NavTabs(options)
        })
    }

}