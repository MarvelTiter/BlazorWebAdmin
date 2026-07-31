
// Hex 转 RGB
function hexToRgb(hex) {
    hex = hex.replace(/^#/, '');
    if (hex.length === 3) {
        hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
    }
    const num = parseInt(hex, 16);
    return {
        r: (num >> 16) & 255,
        g: (num >> 8) & 255,
        b: num & 255
    };
}

// RGB 转 Hex
function rgbToHex(r, g, b) {
    return '#' + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
}

// RGB 转 HSV
function rgbToHsv(r, g, b) {
    r /= 255;
    g /= 255;
    b /= 255;

    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    const delta = max - min;

    let h = 0;
    if (delta !== 0) {
        if (max === r) h = ((g - b) / delta) % 6;
        else if (max === g) h = (b - r) / delta + 2;
        else h = (r - g) / delta + 4;
        h = Math.round(h * 60);
        if (h < 0) h += 360;
    }

    const s = max === 0 ? 0 : delta / max;
    const v = max;

    return { h, s, v };
}

// HSV 转 RGB
function hsvToRgb(h, s, v) {
    h = h % 360;
    if (h < 0) h += 360;

    const i = Math.floor(h / 60);
    const f = h / 60 - i;
    const p = v * (1 - s);
    const q = v * (1 - f * s);
    const t = v * (1 - (1 - f) * s);

    let r, g, b;
    switch (i) {
        case 0: [r, g, b] = [v, t, p]; break;
        case 1: [r, g, b] = [q, v, p]; break;
        case 2: [r, g, b] = [p, v, t]; break;
        case 3: [r, g, b] = [p, q, v]; break;
        case 4: [r, g, b] = [t, p, v]; break;
        case 5: [r, g, b] = [v, p, q]; break;
    }

    return {
        r: Math.round(r * 255),
        g: Math.round(g * 255),
        b: Math.round(b * 255)
    };
}
// 计算亮度（YIQ 模型）
function calculateBrightness(hex) {
    const rgb = hexToRgb(hex);
    // 使用 YIQ 模型计算相对亮度
    return (rgb.r * 299 + rgb.g * 587 + rgb.b * 114) / 1000;
}
// 根据背景色计算文本颜色
function getTextColor(backgroundColor, alpha) {
    const brightness = calculateBrightness(backgroundColor);
    // 阈值 128，亮度大于 128 用黑色，小于等于 128 用白色
    return brightness > 128 ? 'rgba(0, 0, 0, ' + alpha + ')' : 'rgba(255, 255, 255, ' + alpha + ')';
}
// 调整颜色亮度
function adjustBrightness(hex, factor) {
    const rgb = hexToRgb(hex);
    const hsv = rgbToHsv(rgb.r, rgb.g, rgb.b);

    // 调整亮度
    hsv.v = Math.max(0, Math.min(1, hsv.v + factor));

    const newRgb = hsvToRgb(hsv.h, hsv.s, hsv.v);
    return rgbToHex(newRgb.r, newRgb.g, newRgb.b);
}

// 调整颜色饱和度
function adjustSaturation(hex, factor) {
    const rgb = hexToRgb(hex);
    const hsv = rgbToHsv(rgb.r, rgb.g, rgb.b);

    // 调整饱和度
    hsv.s = Math.max(0, Math.min(1, hsv.s + factor));

    const newRgb = hsvToRgb(hsv.h, hsv.s, hsv.v);
    return rgbToHex(newRgb.r, newRgb.g, newRgb.b);
}

// 生成半透明颜色
function withAlpha(hex, alpha) {
    const rgb = hexToRgb(hex);
    return `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, ${alpha})`;
}

// Ant Design 颜色生成算法
function colorPalette(color, index) {
    const isLight = index <= 6;

    if (isLight) {
        // 浅色系列：index 越小颜色越浅
        const lightnessMap = {
            1: 0.95,  // 最浅
            2: 0.85,
            3: 0.75,
            4: 0.65,
            5: 0.55,  // hover
            6: 0.45   // 主色
        };
        const factor = (lightnessMap[index] || 0.5) - 0.5;
        return adjustBrightness(color, factor);
    } else {
        // 深色系列：index 越大颜色越深
        const lightnessMap = {
            7: -0.15,  // active
            8: -0.30,
            9: -0.45,
            10: -0.60 // 最深
        };
        const factor = lightnessMap[index] || -0.15;
        return adjustBrightness(color, factor);
    }
};

// 主题色变更函数
window.changeColor = function (primaryColor) {
    const root = document.documentElement
    // 设置主色
    root.style.setProperty('--ant-primary-color', primaryColor)
    //root.style.setProperty('--major-color', primaryColor)
    // 计算相关颜色
    root.style.setProperty('--ant-primary-color-hover', colorPalette(primaryColor, 5))// 比主色浅
    root.style.setProperty('--ant-primary-color-active', colorPalette(primaryColor, 7))// 比主色深
    root.style.setProperty('--ant-primary-color-outline', withAlpha(primaryColor, 0.2))
    // 主色色板
    root.style.setProperty('--ant-primary-1', colorPalette(primaryColor, 1))
    root.style.setProperty('--ant-primary-2', colorPalette(primaryColor, 2))
    root.style.setProperty('--ant-primary-3', colorPalette(primaryColor, 3))
    root.style.setProperty('--ant-primary-4', colorPalette(primaryColor, 4))
    root.style.setProperty('--ant-primary-5', colorPalette(primaryColor, 5))
    root.style.setProperty('--ant-primary-6', colorPalette(primaryColor, 6))
    root.style.setProperty('--ant-primary-7', colorPalette(primaryColor, 7))
    // 信息色
    root.style.setProperty('--ant-info-color', primaryColor);
    root.style.setProperty('--ant-info-color-deprecated-bg', colorPalette(primaryColor, 1))
    root.style.setProperty('--ant-info-color-deprecated-border', colorPalette(primaryColor, 3))

    // 计算文本颜色
    const fontColor = getTextColor(primaryColor, 0.65)
    root.style.setProperty('--ant-text-color', fontColor)
    root.style.setProperty('--ant-text-color-secondary', getTextColor(primaryColor, 0.45))
    //root.style.setProperty('--font-color', fontColor)

    localStorage.setItem("blazor-admin-project-primary-color", primaryColor)

};

const defaultThemeValues = {
    '--ant-primary-color': '#1890ff',
    '--ant-primary-color-hover': '#40a9ff',
    '--ant-primary-color-active': '#096dd9',
    '--ant-primary-color-outline': 'rgba(24, 144, 255, 0.2)',
    '--ant-primary-1': '#e6f7ff',
    '--ant-primary-2': '#bae7ff',
    '--ant-primary-3': '#91d5ff',
    '--ant-primary-4': '#69c0ff',
    '--ant-primary-5': '#40a9ff',
    '--ant-primary-6': '#1890ff',
    '--ant-primary-7': '#096dd9',
    '--ant-primary-color-deprecated-pure': '',
    '--ant-primary-color-deprecated-l-35': '#cbe6ff',
    '--ant-primary-color-deprecated-l-20': '#7ec1ff',
    '--ant-primary-color-deprecated-t-20': '#46a6ff',
    '--ant-primary-color-deprecated-t-50': '#8cc8ff',
    '--ant-primary-color-deprecated-f-12': 'rgba(24, 144, 255, 0.12)',
    '--ant-primary-color-active-deprecated-f-30': 'rgba(230, 247, 255, 0.3)',
    '--ant-primary-color-active-deprecated-d-02': '#dcf4ff',
    '--ant-success-color': '#52c41a',
    '--ant-success-color-hover': '#73d13d',
    '--ant-success-color-active': '#389e0d',
    '--ant-success-color-outline': 'rgba(82, 196, 26, 0.2)',
    '--ant-success-color-deprecated-bg': '#f6ffed',
    '--ant-success-color-deprecated-border': '#b7eb8f',
    '--ant-error-color': '#ff4d4f',
    '--ant-error-color-hover': '#ff7875',
    '--ant-error-color-active': '#d9363e',
    '--ant-error-color-outline': 'rgba(255, 77, 79, 0.2)',
    '--ant-error-color-deprecated-bg': '#fff2f0',
    '--ant-error-color-deprecated-border': '#ffccc7',
    '--ant-warning-color': '#faad14',
    '--ant-warning-color-hover': '#ffc53d',
    '--ant-warning-color-active': '#d48806',
    '--ant-warning-color-outline': 'rgba(250, 173, 20, 0.2)',
    '--ant-warning-color-deprecated-bg': '#fffbe6',
    '--ant-warning-color-deprecated-border': '#ffe58f',
    '--ant-info-color': '#1890ff',
    '--ant-info-color-deprecated-bg': '#e6f7ff',
    '--ant-info-color-deprecated-border': '#91d5ff',
    '--ant-text-color': 'rgba(0, 0, 0, 0.65)',
    '--ant-text-color-secondary': 'rgba(0, 0, 0, 0.45)'
};
// 设置主题函数
window.resetThemeVariables = function () {
    const root = document.documentElement;

    Object.entries(defaultThemeValues).forEach(([key, value]) => {
        if (value !== null && value !== undefined && value !== '') {
            root.style.setProperty(key, value);
        }
    });

    localStorage.removeItem("blazor-admin-project-primary-color")
}

window.setDark = function () {
    resetThemeVariables();
    const root = document.documentElement;
    root.style.setProperty('--ant-text-color', 'rgba(255, 255, 255, 0.85)');
}

// 页面加载时初始化
//document.addEventListener('DOMContentLoaded', function () {
//    const color = localStorage.getItem("blazor-admin-project-primary-color")
//    const root = document.documentElement;
//    root.style.setProperty('--major-color', 'var(--ant-primary-color)')
//    root.style.setProperty('--font-color', 'var(--ant-text-color)')
//    root.style.setProperty('--hover-bg-color', 'var(--ant-primary-color-hover)')
//    if (color) {
//        changeColor(color);
//    }
//});
function initializeColors() {
    const color = localStorage.getItem("blazor-admin-project-primary-color");
    const root = document.documentElement;

    root.style.setProperty('--major-color', 'var(--ant-primary-color)');
    root.style.setProperty('--font-color', 'var(--ant-text-color)');
    root.style.setProperty('--hover-bg-color', 'var(--ant-primary-color-hover)');

    if (color) {
        changeColor(color);
    }
}

// 如果文档已经加载完成，立即执行
if (document.readyState === 'loading') {
    // 文档还在加载，等加载完成再执行
    document.addEventListener('DOMContentLoaded', initializeColors);
} else {
    // 文档已经加载完成，立即执行
    initializeColors();
}