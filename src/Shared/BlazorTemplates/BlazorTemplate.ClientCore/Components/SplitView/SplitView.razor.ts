import { BaseComponent } from "@/baseComponent";
import { getComponentById } from "@/componentStore";
import { startDrag } from "@/dragHelper";
import { EventHandler } from "@/eventHandler";

interface DragState {
    /** 按下时的起点坐标（clientX 或 clientY） */
    start: number
    /** 按下时面板的实际尺寸(px) */
    size: number
    /** 允许的最小尺寸(px) */
    min: number
    /** 允许的最大尺寸(px) */
    max: number
}

export class SplitView extends BaseComponent {
    panel1: HTMLElement
    panel2: HTMLElement
    separator: HTMLElement
    wrapper: HTMLElement
    direction: 'row' | 'column'
    /** 首个面板最大尺寸：<1 为比例，>=1 为像素 */
    max: number
    /** 首个面板最小尺寸：<1 为比例，>=1 为像素 */
    min: number
    /** 首个面板初始尺寸：<1 为比例，>=1 为像素 */
    init: number
    drag: boolean = false

    constructor(doms: any, options: any) {
        super()
        this.panel1 = doms.panel1
        this.panel2 = doms.panel2
        this.separator = doms.separator
        this.wrapper = this.panel1.parentElement!
        // 兼容 C# 侧 PascalCase 与 TS 侧 camelCase 两种传参
        this.direction = String(options.direction ?? options.Direction ?? 'row').toLowerCase() as 'row' | 'column'
        // Min/Max/InitWidth 统一为 double：<1 为比例，>=1 为像素
        this.max = Number(options.max ?? options.Max ?? 0.7)
        this.min = Number(options.min ?? options.Min ?? 0.3)
        this.init = Number(options.initWidth ?? options.InitWidth ?? 0.5)
        this.setup()
    }

    setup() {
        EventHandler.listen(this.separator, 'mousedown', this.handleMouseDown.bind(this));
        if (this.wrapper)
            EventHandler.listen(this.wrapper, "resize", this.refresh.bind(this));
    }

    private get isRow() {
        return this.direction === 'row'
    }

    /** 容器实际尺寸（px） */
    private getTotal(): number {
        const rect = this.wrapper.getBoundingClientRect()
        return this.isRow ? rect.width : rect.height
    }

    /** 面板1 的实际渲染尺寸（px） */
    private getPanelSize(): number {
        const rect = this.panel1.getBoundingClientRect()
        return this.isRow ? rect.width : rect.height
    }

    /** 面板1 的初始尺寸（px），按容器实际尺寸换算 */
    private getInitPx(total: number): number {
        return toPx(this.init, total, total * 0.5)
    }

    /**
     * 把 Min/Max 按容器实际尺寸换算成像素（<1 为比例，>=1 为像素）。
     * 必须保证 InitWidth 落在 [min, max] 内，否则 refresh/拖拽一夹取就把用户设置的
     * InitWidth 覆盖掉（表现为"初始尺寸不是设的值、且拖不回该位置"）。
     * 故 min 取 min(min, init)，max 取 max(max, init)。
     */
    private getBounds(total: number): { min: number; max: number } {
        const init = this.getInitPx(total)
        const min = Math.min(toPx(this.min, total, 0), init)
        const max = Math.max(toPx(this.max, total, total), init)
        return { min, max }
    }

    /** 容器尺寸变化时，把当前尺寸夹到合法区间（不重置为 InitWidth） */
    refresh() {
        if (this.drag || !this.wrapper) return
        const { min, max } = this.getBounds(this.getTotal())
        this.apply(clamp(this.getPanelSize(), min, max))
    }

    handleMouseDown(e) {
        e.stopPropagation()
        if (!this.wrapper) return
        if (e.preventDefault) e.preventDefault()

        const total = this.getTotal()
        const { min, max } = this.getBounds(total)
        const state: DragState = {
            start: this.isRow ? e.clientX : e.clientY,
            size: this.getPanelSize(),
            min,
            max,
        }
        this.drag = true
        startDrag(e, (event:MouseEvent) => {
            const current = this.isRow ? event.clientX : event.clientY
            // 以「起始尺寸 + 鼠标位移增量」计算：与页面滚动、padding、gap 均无关，按下瞬间不跳变
            this.apply(clamp(state.size + (current - state.start), state.min, state.max))
        }, () => {
            this.drag = false
        })
    }

    private apply(size: number) {
        if (this.isRow)
            this.panel1.style.width = `${size}px`
        else
            this.panel1.style.height = `${size}px`
    }

    dispose() {
        EventHandler.remove(this.separator, 'mousedown');
        if (this.wrapper)
            EventHandler.remove(this.wrapper, 'resize');
    }

    static init(id, doms, options) {
        getComponentById(id, () => {
            return new SplitView(doms, options);
        });
    }
}

function clamp(value: number, min: number, max: number): number {
    if (max < min) max = min
    return Math.max(min, Math.min(max, value))
}

/**
 * 把尺寸值换算成像素：`<1` 视为比例（0.3 → 容器 30%），`>=1` 视为像素（400 → 400px）。
 */
function toPx(value: number, total: number, fallback: number): number {
    if (typeof value !== 'number' || isNaN(value)) return fallback
    return value < 1 ? value * total : value
}
