import { BaseComponent } from "../../JsCore/baseComponent.ts";
import { getComponentById } from "../../JsCore/componentStore.ts";
import { EventHandler } from "../../JsCore/eventHandler.ts";

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
        this.observer.observe(this.tabsContainer, { childList: true, attributes: true, subtree: true })
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

        if (activeTab === null) return
        if (this.activeTab !== activeTab) {
            this.activeTab = activeTab
            //console.log('active changed', this.activeTab)
            this.fixedPosition()
            this.setButtonState()
        } else if (this.activeTab) {
            this.fixedPosition()
        }
    }

    fixedPosition() {
        if (!this.activeTab) return
        const rect = this.activeTab.getBoundingClientRect()
        const box = this.tabsContainer.getBoundingClientRect()
        //console.log('rect:', rect, 'box:', box)
        // 判断是否左侧被遮挡
        if (rect.left < box.left) {
            this.tabsContainer.scrollBy({
                left: rect.left - box.left,
                behavior: 'smooth'
            });
        }
        // 判断是否右侧被遮挡
        else if (rect.right > box.right) {
            this.tabsContainer.scrollBy({
                left: rect.right - box.right,
                behavior: 'smooth'
            });
        }
    }

    setButtonState() {
        const allTabs = this.tabsContainer.querySelectorAll<HTMLElement>('.nav-top')
        const max = allTabs.length
        let current = 0
        allTabs.forEach((e, i) => {
            if (e === this.activeTab) {
                current = i
            }
        })
        if (current === 0) {
            this.leftButton.classList.add('forbidden')
        } else {
            this.leftButton.classList.remove('forbidden')
        }

        if (current === max - 1) {
            this.rightButton.classList.add('forbidden')
        } else {
            this.rightButton.classList.remove('forbidden')
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