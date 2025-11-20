module.exports = {
    presets: [
        [
            '@babel/preset-env',
            {
                // 或者直接指定 ES 版本
                targets: {
                    chrome: 78
                },

                useBuiltIns: false,
                corejs: 3,
                modules: false,

                // 针对 ES2015 的优化配置
                shippedProposals: true,
                bugfixes: true,
                loose: false,

                // 排除 ES2015 已经支持的特性
                exclude: [
                    'transform-typeof-symbol',
                    'transform-regenerator'
                ]
            }
        ]
    ],

    // 可选：添加特定插件
    plugins: [
        // 添加对 ES2015+ 特性的额外支持
    ],

    // 移除注释减小文件大小
    comments: false
};// JavaScript source code
