
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

// 生成半透明颜色
function withAlpha(hex, alpha) {
    const rgb = hexToRgb(hex);
    return `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, ${alpha})`;
}

// 与目标色按比例混合（weight = 目标色占比）
function mixColor(color, target, weight) {
    const a = hexToRgb(color);
    const b = hexToRgb(target);
    return rgbToHex(
        Math.round(a.r * (1 - weight) + b.r * weight),
        Math.round(a.g * (1 - weight) + b.g * weight),
        Math.round(a.b * (1 - weight) + b.b * weight)
    );
}

// Ant Design 颜色生成算法
// 1~5：向白色混合（index 越小越浅）；6：主色；7~10：向黑色混合（index 越大越深）。
function colorPalette(color, index) {
    const lightWeight = { 1: 0.9, 2: 0.7, 3: 0.5, 4: 0.3, 5: 0.1 };
    const darkWeight = { 7: 0.1, 8: 0.2, 9: 0.3, 10: 0.4 };

    if (index === 6 || index === 0) return color;
    if (lightWeight[index] !== undefined) return mixColor(color, '#ffffff', lightWeight[index]);
    if (darkWeight[index] !== undefined) return mixColor(color, '#000000', darkWeight[index]);
    return color;
};

// 主题色变更函数
// 方向：组件库 token（--ant-*）是上游，框架变量 --wb-* 通过
// bridge.ant.css（组件库侧 CSS）静态桥接到 var(--ant-*)。
// 这里只写 --ant-* 主色及其衍生色板，框架侧自动跟随。
window.changeColor = function (primaryColor) {
    const root = document.documentElement

    // 主色
    root.style.setProperty('--ant-primary-color', primaryColor)
    root.style.setProperty('--ant-primary-color-hover', colorPalette(primaryColor, 5))  // 比主色浅
    root.style.setProperty('--ant-primary-color-active', colorPalette(primaryColor, 7)) // 比主色深
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

function initializeColors() {
    const color = localStorage.getItem("blazor-admin-project-primary-color");

    // 框架变量 -> 组件库 token 的桥接由 bridge.ant.css（组件库侧 CSS）静态完成，
    // 这里仅在用户保存过自定义主题色时恢复 antd 色板。
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