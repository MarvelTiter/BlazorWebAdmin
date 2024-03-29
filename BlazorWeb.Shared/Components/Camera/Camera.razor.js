import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/BlazorWeb.Shared/js/jscomponentbase/component-store.js";
import { EventHandler } from "/_content/BlazorWeb.Shared/js/jscomponentbase/event-handler.js";
import { success, failed } from "/_content/BlazorWeb.Shared/js/jscomponentbase/utils.js";
import { startDrag } from "/_content/BlazorWeb.Shared/js/jscomponentbase/drag-helper.js";
export class Camera extends BaseComponent {
    constructor(video, canvas) {
        super()
        this.currentId = '';
        this.width = 0;
        this.height = 0;
        this.video = video;
        this.canvas = canvas;
        this.tracks = [];
        this.clipBox = null;
    }
    open(deviceId, width, height) {
        return new Promise(resolve => {
            if (navigator && navigator.mediaDevices) {
                navigator.mediaDevices.getUserMedia({
                    video: {
                        deviceId: { exact: deviceId },
                        width: { ideal: width },
                        height: { ideal: height },
                    },
                })
                    .then((stream) => {
                        /* 使用这个 stream stream */
                        this.width = width;
                        this.height = height;
                        this.currentId = deviceId;
                        try {
                            this.video.srcObject = stream;
                            this.tracks = stream.getTracks();
                            this.video.onloadedmetadata = (e) => {
                                this.video.play();
                                if (this.clipBox)
                                    this.clipBox.setVisible(true);
                                resolve(success('开始播放'));
                            };
                        } catch (e) {
                            resolve(failed(e.message));
                        }
                    })
                    .catch(function (err) {
                        /* 处理 error */
                        resolve(failed(err.message));
                    });
            } else {
                resolve(failed('浏览器不支持'))
            }
        })
    }

    close() {
        this.tracks.forEach(tra => tra.stop());
        if (this.video)
            this.video.srcObject = null;
        if (this.clipBox)
            this.clipBox.setVisible(false);
    }

    capture() {
        try {
            var data = ''
            if (this.video && this.canvas) {
                var ctx = this.canvas.getContext('2d');
                var x = 0, y = 0, w = this.video.videoWidth, h = this.video.videoHeight
                if (this.clipBox) {
                    this.clipBox.applyRect()
                    var scaleX = this.video.videoWidth / this.clipBox.videoWindowWidth
                    var scaleY = this.video.videoHeight / this.clipBox.videoWindowHeight
                    x = this.clipBox.x * scaleX
                    y = this.clipBox.y * scaleY
                    w = this.clipBox.w * scaleX
                    h = this.clipBox.h * scaleY
                }
                this.canvas.width = w
                this.canvas.height = h
                ctx.drawImage(this.video, x, y, w, h, 0, 0, w, h);

                var dataURL = this.canvas.toDataURL("image/jpeg", 1);
                //window.document.getElementById('test').src = dataURL
                if (dataURL.split(',').length > 1)
                    data = dataURL.split(',')[1];
                return success('', data)
            }
            return failed('')
        } catch (e) {
            return failed(e.message)
        }
    }

    dispose() {
        if (this.clipBox) {
            this.clipBox.dispose()
            this.clipBox = null
        }
    }
}

export function init(id, video, canvas, clip, width, height) {
    var component = getComponentById(id, () => {
        return new Camera(video, canvas)
    })
    if (clip) {
        component.clipBox = new ClipBox(clip, width, height)
        component.clipBox.initEvents()
    }
}


export async function enumerateDevices() {
    if (navigator && navigator.mediaDevices) {
        var devices = await navigator.mediaDevices.enumerateDevices()
        return success('', devices)
    }
    return failed('获取设备失败！请检查设备连接或者浏览器配置！')
}

export function loadUserMedia(id, deviceId, width, height) {
    try {
        const camera = getComponentById(id)
        return camera.open(deviceId, width, height)
    } catch (e) {
        return failed(e.message)
    }
}

export function closeUserMedia(id) {
    try {
        const camera = getComponentById(id)
        camera.close()
        return success()

    } catch (e) {
        return failed(e.message)
    }
}
export function capture(id) {
    const camera = getComponentById(id)
    return camera.capture()
}

class ClipBox extends BaseComponent {
    constructor(el, width, height) {
        super()
        this.el = el;
        this.w = width * 0.8;
        this.h = height * 0.8;
        this.x = width * 0.1;
        this.y = height * 0.1;
        this.videoWindowWidth = width;
        this.videoWindowHeight = height;
        this.el.style.width = this.w + 'px';
        this.el.style.height = this.h + 'px';
        this.scaleWidth = 10;
        this.applyRect();
    }

    applyRect() {
        if (this.x < 0)
            this.x = 0;
        if (this.y < 0)
            this.y = 0;
        if (this.x > this.videoWindowWidth - this.w)
            this.x = this.videoWindowWidth - this.w;
        if (this.y > this.videoWindowHeight - this.h)
            this.y = this.videoWindowHeight - this.h;
        //if (this.el.)
        if (this.el.offsetWidth > 0)
            this.w = this.el.offsetWidth;
        if (this.el.offsetHeight > 0)
            this.h = this.el.offsetHeight;
        this.el.style.top = this.y + 'px';
        this.el.style.left = this.x + 'px';
        // 用于缩放的最大限制
        this.el.style["max-width"] = (this.videoWindowWidth - this.x) + 'px';
        this.el.style["max-height"] = (this.videoWindowHeight - this.y) + 'px';
    }

    canMove(x, y) {
        this.w = this.el.offsetWidth;
        this.h = this.el.offsetHeight;
        return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth;
    }

    setVisible(visible) {
        if (this.el) {
            this.el.style.display = visible ? 'block' : 'none';
        }
    }

    handleMouseDown(e) {
        e.stopPropagation()
        if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
        window.getSelection()?.removeAllRanges();
        var x = e.offsetX;
        var y = e.offsetY;
        if (this.canMove(x, y)) {
            startDrag(e, event => {
                this.x = event.offsetX - x + this.x
                this.y = event.offsetY - y + this.y
                this.applyRect();
            })
        }
    }

    initEvents() {
        EventHandler.listen(this.el, 'mousedown', this.handleMouseDown.bind(this))
    }

    dispose() {
        EventHandler.remove(this.el, 'mousedown')
    }
}
