import { IJsActionResult, failed, success } from '../../JsCore/utils'
import { BaseComponent } from '../../JsCore/baseComponent'
import { startDrag } from '../../JsCore/dragHelper'
import { EventHandler } from '../../JsCore/eventHandler'
import { getComponentById } from '../../JsCore/componentStore'

export class Camera extends BaseComponent {
    deviceId: string
    width: number
    height: number
    video: HTMLVideoElement
    tracks: MediaStreamTrack[]
    clipBox?: ClipBox
    quality: number
    format: string
    constructor(options: any) {
        super()
        this.deviceId = ''
        this.width = 0
        this.height = 0
        this.video = options.video
        this.tracks = []
        this.quality = options.quality
        this.format = options.format
        if (options.clip) {
            this.clipBox = new ClipBox(options)
            this.clipBox.initEvents()
        }
    }

    open(deviceId: string, width: number, height: number): Promise<IJsActionResult> {
        return new Promise(resolve => {
            if (navigator && navigator.mediaDevices) {
                navigator.mediaDevices.getUserMedia({
                    video: {
                        deviceId: { ideal: deviceId },
                        width: { ideal: width },
                        height: { ideal: height },
                    },
                })
                    .then((stream) => {
                        /* 使用这个 stream stream */
                        this.width = width;
                        this.height = height;
                        this.deviceId = deviceId;
                        try {
                            this.video.srcObject = stream;
                            this.tracks = stream.getTracks();
                            this.video.onloadedmetadata = (e) => {
                                this.video.play();
                                if (this.clipBox)
                                    this.clipBox.setVisible(true);
                                resolve(success('开始播放'));
                            };
                        } catch (e: any) {
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

    capture(rotate: number) {
        try {
            if (this.video && this.video.readyState > 2) {
                var data = ''
                var canvas = document.createElement('canvas');
                var ctx = canvas.getContext('2d');
                if (ctx == null) {
                    return failed('获取Canvas Context失败')
                }
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
                rotate = rotate % 4
                let tx = 0, ty = 0
                if (rotate == 0 || rotate == 2) {
                    canvas.width = w
                    canvas.height = h
                    if (rotate == 2) {
                        tx = w
                        ty = h
                    }
                } else {
                    // 竖屏
                    canvas.width = h
                    canvas.height = w
                    if (rotate == 1) {
                        tx = h
                    } else {
                        ty = w
                    }
                }
                let angle = rotate * 90 * Math.PI / 180
                ctx.translate(tx, ty)
                ctx.rotate(angle)
                ctx.drawImage(this.video, x, y, w, h, 0, 0, w, h)
                ctx.rotate(-angle)
                ctx.translate(-tx, -ty)

                var dataURL = canvas.toDataURL(this.format, this.quality);
                //window.document.getElementById('test').src = dataURL
                if (dataURL.split(',').length > 1)
                    data = dataURL.split(',')[1]
                return success('', data)
            }
            return failed('视频状态异常')
        } catch (e: any) {
            return failed(e.message)
        }
    }

    close() {
        this.tracks.forEach(tra => tra.stop())
        this.tracks = []
        if (this.video)
            this.video.srcObject = null;
        if (this.clipBox)
            this.clipBox.setVisible(false)
    }
    dispose() {
        if (this.clipBox) {
            this.clipBox.dispose()
            this.clipBox = undefined
        }
        this.close()
    }

    // video, quality, clip, width, height
    static init(id: string, options: any) {
        getComponentById(id, () => {
            return new Camera(options)
        })
    }
    static useClipBox(id: string, options: any) {
        var c: Camera = getComponentById(id)
        if (c && c.clipBox == undefined) {
            c.clipBox = new ClipBox(options)
            c.clipBox.initEvents()
        }
        c.clipBox?.setVisible(true)
    }

    static disableClipBox(id: string) {
        var c: Camera = getComponentById(id)
        if (c && c.clipBox) {
            c.clipBox.setVisible(false)
            c.clipBox.dispose()
            c.clipBox = undefined
        }
    }
    static async enumerateDevices() {
        if (navigator && navigator.mediaDevices) {
            var devices = await navigator.mediaDevices.enumerateDevices()
            return success('', devices)
        }
        return failed('获取设备失败！请检查设备连接或者浏览器配置！')
    }

    static async loadUserMedia(id: string, deviceId: string, width: number, height: number) {
        try {
            const camera: Camera = getComponentById(id)
            return await camera.open(deviceId, width, height)
        } catch (e: any) {
            return failed(e.message)
        }
    }
    static closeUserMedia(id: string) {
        try {
            const camera = getComponentById(id)
            if (camera)
                camera.close()
            return success('')
        } catch (e: any) {
            return failed(e.message)
        }
    }
    static capture(id: string, rotate: number) {
        const camera: Camera = getComponentById(id)
        return camera.capture(rotate)
    }
}
class ClipBox extends BaseComponent {
    el: HTMLElement
    w: number
    h: number
    x: number
    y: number
    videoWindowWidth: number
    videoWindowHeight: number
    scaleWidth: number
    constructor(options: any) {
        super()
        this.el = options.clip
        this.w = options.width * 0.8
        this.h = options.height * 0.8
        this.x = options.width * 0.1
        this.y = options.height * 0.1
        this.videoWindowWidth = options.width
        this.videoWindowHeight = options.height
        this.el.style.width = this.w + 'px'
        this.el.style.height = this.h + 'px'
        this.scaleWidth = 10
        this.applyRect()
    }

    applyRect() {
        if (this.x < 0)
            this.x = 0
        if (this.y < 0)
            this.y = 0
        if (this.x > this.videoWindowWidth - this.w)
            this.x = this.videoWindowWidth - this.w
        if (this.y > this.videoWindowHeight - this.h)
            this.y = this.videoWindowHeight - this.h
        //if (this.el.)
        if (this.el.offsetWidth > 0)
            this.w = this.el.offsetWidth
        if (this.el.offsetHeight > 0)
            this.h = this.el.offsetHeight
        this.el.style.top = this.y + 'px'
        this.el.style.left = this.x + 'px'
        // 用于缩放的最大限制
        this.el.style["max-width"] = (this.videoWindowWidth - this.x) + 'px'
        this.el.style["max-height"] = (this.videoWindowHeight - this.y) + 'px'
    }

    canMove(x: number, y: number) {
        this.w = this.el.offsetWidth
        this.h = this.el.offsetHeight
        return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth
    }

    setVisible(visible) {
        if (this.el) {
            this.el.style.display = visible ? 'block' : 'none'
        }
    }

    handleMouseDown(e) {
        e.stopPropagation()
        if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return
        if (window && window.getSelection()) {
            window.getSelection()?.removeAllRanges()
        }
        var x = e.offsetX
        var y = e.offsetY
        if (this.canMove(x, y)) {
            startDrag(e, event => {
                this.x = event.offsetX - x + this.x
                this.y = event.offsetY - y + this.y
                this.applyRect()
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
