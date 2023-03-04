//async function downloadFileFromStream(fileName, contentStreamReference) {
//    const arrayBuffer = await contentStreamReference.arrayBuffer();
//    const blob = new Blob([arrayBuffer]);
//    const url = URL.createObjectURL(blob);
//    triggerFileDownload(fileName, url);
//    URL.revokeObjectURL(url);
//}

//function triggerFileDownload(fileName, url) {
//    const anchorElement = document.createElement('a');
//    anchorElement.href = url;
//    if (!fileName) fileName = "未命名";
//    anchorElement.download = fileName;
//    anchorElement.click();
//    anchorElement.remove();
//}

function clickElement(dom) {
    dom.click();
}

function elementOperation(dom, funcName, ...args) {
    var func = dom[funcName];
    if (func) {
        window.t = func
        window.d = dom
        window.a = args
        var r = func.call(dom, ...args);
        console.log(r);
        return r;
    }
}

function elementProperty(dom, prop) {
    return dom[prop];
}