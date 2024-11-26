import { BaseComponent } from "../../JsCore/baseComponent";
import { getComponentById } from "../../JsCore/componentStore";

export class WaterMark extends BaseComponent {
    wrapper: HTMLElement
    options?: any
    ob: MutationObserver
    mask?: HTMLElement
    constructor(wrapper: HTMLElement) {
        super()
        this.wrapper = wrapper || window.document.documentElement

        this.ob = new MutationObserver((entries) => {
            for (const entry of entries) {
                if (entry.type === 'childList') {
                    const removeNodes = entry.removedNodes;
                    removeNodes.forEach((node) => {
                        if (node === this.mask) {
                            this.setWatermark(this.options);
                        }

                        if (entry.target === this.mask) {
                            this.setWatermark(this.options);
                        }
                    });
                }
            }
        });

        this.ob.observe(this.wrapper, {
            attributes: true,
            childList: true,
            characterData: true,
            subtree: true,
        });
    }

    setWatermark(options) {
        if (this.mask) {
            this.mask.remove();
        }
        this.options = options;
        this.mask = createDiv();
        let url = drawMask(options)
        this.mask.style.backgroundSize = `${options.gapX + options.width}px`;
        this.mask.style.backgroundImage = `url(${url})`;
        this.wrapper.appendChild(this.mask);
    }
    dispose() {
        this.ob.disconnect();
    }

    static setWatermark(id, wrapper, options) {
        let com = getComponentById(id, () => {
            return new WaterMark(wrapper)
        })
        com.setWatermark(options);
    }

}

function createDiv(): HTMLElement {
    var d = document.createElement("div");
    d.style.position = "absolute";
    d.style.left = '0';
    d.style.top = '0';
    d.style.width = "100%";
    d.style.height = "100%";
    d.style.pointerEvents = "none";
    d.style.backgroundRepeat = "repeat";
    return d;
}

function getFontColor(color: string) {
    if (color) return color;
    const cssParameters = getComputedStyle(document.documentElement)
    const value = cssParameters.getPropertyValue('--watermark-color')
    return value
}

function drawMask({
    top,
    width,
    height,
    gapX,
    gapY,
    rotate,
    alpha,
    lineSpace,
    fontSize,
    fontColor,
    contents
}) {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    if (!ctx) {
        console.warn('Current environment does not support Canvas, cannot draw watermarks.');
        return;
    }
    const ratio = window.devicePixelRatio || 1;
    const canvasWidth = (gapX + width) * ratio;
    const canvasHeight = (gapY + height) * ratio;
    canvas.width = canvasWidth;
    canvas.height = canvasHeight;
    canvas.style.width = `${gapX + width}px`;
    canvas.style.height = `${gapY + height}px`;

    ctx.rotate((Math.PI / 180) * Number(rotate));
    ctx.globalAlpha = alpha;

    const markWidth = width * ratio;
    const markHeight = height * ratio;

    ctx.fillStyle = 'transparent';
    ctx.fillRect(0, 0, markWidth, markHeight);

    let fontFamily = 'sans-serif';
    let fontWeight = 'normal';
    let textAlign: CanvasTextAlign = 'start';
    let fontStyle = 'normal';
    const color = getFontColor(fontColor)
    const markSize = Number(fontSize) * ratio;
    for (var i = 0; i < contents.length; i++) {
        let text = contents[i];
        top += lineSpace;
        ctx.font = `${fontStyle} normal ${fontWeight} ${markSize}px/${markHeight}px ${fontFamily}`;
        ctx.textAlign = textAlign;
        ctx.textBaseline = 'top';
        ctx.fillStyle = color;
        ctx.fillText(text, 0, top * ratio);
    }
    return canvas.toDataURL();
}
