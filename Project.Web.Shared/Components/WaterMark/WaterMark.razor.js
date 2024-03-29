import { BaseComponent } from "/_content/Project.Web.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/Project.Web.Shared/js/jscomponentbase/component-store.js";
function createDiv() {
    var d = document.createElement("div");
    d.style.position = "absolute";
    d.style.left = 0;
    d.style.top = 0;
    d.style.width = "100%";
    d.style.height = "100%";
    d.style.pointerEvents = "none";
    d.style.backgroundRepeat = "repeat";
    return d;
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
    let textAlign = 'start';
    let fontStyle = 'normal';
    const markSize = Number(fontSize) * ratio;
    for (var i = 0; i < contents.length; i++) {
        let text = contents[i];
        top += lineSpace;
        ctx.font = `${fontStyle} normal ${fontWeight} ${markSize}px/${markHeight}px ${fontFamily}`;
        ctx.textAlign = textAlign;
        ctx.textBaseline = 'top';
        ctx.fillStyle = fontColor;
        ctx.fillText(text, 0, top * ratio);
    }
    return canvas.toDataURL();
}

export class Watermark extends BaseComponent {
    constructor(wrapper) {
        super()
        this.options = undefined;
        this.container = wrapper || window.document.body;
        this.mask = null;
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

        this.ob.observe(this.container, {
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
        this.container.appendChild(this.mask);
    }
    dispose() {
        this.ob.disconnect();
    }
}

export function setWatermark(id, wrapper, options) {
    let com = getComponentById(id, () => {
        return new Watermark(wrapper)
    })
    com.setWatermark(options);
}

export function dispose(id) {
    let com = getComponentById(id);
    com.dispose();
}

