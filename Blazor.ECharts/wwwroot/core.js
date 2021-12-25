// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}

export function renderChart(id, option) {
    let container = document.getElementById(id)
    let chart = echarts.init(container)
    chart.showLoading()
    let opt = JSON.parse(option)
    console.log('opt', opt)
    chart.setOption(opt)
    chart.hideLoading()
}
