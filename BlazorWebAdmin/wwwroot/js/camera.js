/**
 * clipBox.init => 初始化截取框，标志enable
 * 
 */

const camera = {
    id: '',
    width: 0,
    height: 0,
    video: null,
    canvas: null,
    tracks: [],
    open: function (stream) {
        return new Promise(resolve => {
            try {
                this.video.srcObject = stream;
                this.tracks = stream.getTracks();
                this.video.onloadedmetadata = function (e) {
                    this.play();
                    clipBox.show();
                    resolve(true);
                };
            } catch (e) {
                console.error(e);
                resolve(false);
            }
        })
    },
    close: function () {
        this.tracks.forEach(tra => tra.stop());
        if (this.video)
            this.video.srcObject = null;
        clipBox.hide();
    },
    reset: function () {
        this.id = '';
        this.width = 0;
        this.height = 0;
        this.video = null;
        this.canvas = null;
        this.tracks = [];
    },
    draw: function () {
        //return new Promise(resolve => {
        //})
        if (this.video && this.canvas) {
            var ctx = this.canvas.getContext('2d');
            var x = 0, y = 0, w = this.video.videoWidth, h = this.video.videoHeight
            if (clipBox.enable) {
                clipBox.applyRect()
                var scaleX = this.video.videoWidth / clipBox.videoWindowWidth
                var scaleY = this.video.videoHeight / clipBox.videoWindowHeight
                x = clipBox.x * scaleX
                y = clipBox.y * scaleY
                w = clipBox.w * scaleX
                h = clipBox.h * scaleY
            }
            this.canvas.width = w
            this.canvas.height = h
            ctx.drawImage(this.video, x, y, w, h, 0, 0, w, h);

            var dataURL = this.canvas.toDataURL("image/jpeg", 1);
            //window.document.getElementById('test').src = dataURL
            if (dataURL.split(',').length > 1)
                return dataURL.split(',')[1];
            else
                return "";
        }
        else {
            return "";
        }
    }
}

const clipBox = {
    el: null,
    x: 0,
    y: 0,
    w: 0,
    h: 0,
    videoWindowWidth: 0,
    videoWindowHeight: 0,
    enable: false,
    originX: 0,
    originY: 0,
    scaleWidth: 10,
    mouseaction: null,
    show: function () {
        if (!this.enable) return
        if (this.el) {
            this.el.style.display = 'block';
        }
    },
    hide: function () {
        if (!this.enable) return
        if (this.el) {
            this.el.style.display = 'none';
        }
    },
    init: function (width, height) {
        this.enable = true;
        this.w = width * 0.8;
        this.h = height * 0.8;
        this.x = width * 0.1;
        this.y = height * 0.1;
        this.videoWindowWidth = width;
        this.videoWindowHeight = height;
        this.el.style.width = this.w + 'px';
        this.el.style.height = this.h + 'px';
        this.applyRect();
    },
    applyRect: function () {
        if (this.x < 0) this.x = 0
        if (this.y < 0) this.y = 0
        if (this.x > this.videoWindowWidth - this.w) this.x = this.videoWindowWidth - this.w
        if (this.y > this.videoWindowHeight - this.h) this.y = this.videoWindowHeight - this.h
        //if (this.el.)
        if (this.el.offsetWidth > 0)
            this.w = this.el.offsetWidth
        if (this.el.offsetHeight > 0)
            this.h = this.el.offsetHeight
        this.el.style.top = this.y + 'px';
        this.el.style.left = this.x + 'px';
        // 用于缩放的最大限制
        this.el.style["max-width"] = (this.videoWindowWidth - this.x) + 'px';
        this.el.style["max-height"] = (this.videoWindowHeight - this.y) + 'px';
    },
    canMove: function (x, y) {
        this.w = this.el.offsetWidth
        this.h = this.el.offsetHeight
        return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth
    },
    //canScale: function (x, y) {
    //    return x > this.w - 20 && x < this.w && y > this.h - 20 && y < this.h
    //}
    //getScaleX: function () {
    //    if (isNaN(this.))
    //},
    //getScaleY: function () { },
}

export function init(video, canvas) {
    camera.video = video;
    camera.canvas = canvas;
}

export function enumerateDevices() {
    if (navigator && navigator.mediaDevices)
        return navigator.mediaDevices.enumerateDevices()
    return []
}
/**
* 加载视频
* @param video 
* @param deviceId 视频窗口宽度
* @param width 视频分辨率宽
* @param height 视频分辨率高
*/
export function loadUserMedia(deviceId, width, height) {
    return new Promise(resolve => {
        debugger
        if (navigator && navigator.mediaDevices) {
            navigator.mediaDevices.getUserMedia({
                video: {
                    deviceId: { exact: deviceId },
                    width: { ideal: width },
                    height: { ideal: height },
                },
            })
                .then(function (stream) {
                    /* 使用这个 stream stream */
                    camera.width = width;
                    camera.height = height;
                    camera.id = deviceId;
                    camera.open(stream).then(r => {
                        resolve(r);
                    });
                })
                .catch(function (err) {
                    /* 处理 error */
                    console.error("loadUserMedia", err);
                    resolve(false);
                });
        } else {
            resolve(false)
        }
    })
}

export function closeUserMedia() {
    camera.close();
    return true;
}

/**
* 初始化截取框
* @param id 
* @param width 视频窗口宽度
* @param height 视频窗口高度
*/
export function initClipBox(clip, width, height) {
    clipBox.el = clip;
    clipBox.init(width, height);
    clipBox.show();
    clipBox.el.addEventListener("mouseover", handleClipMouseOver);
}

export function capture() {
    var imgData = camera.draw();
    return imgData;
}

function handleClipMouseOver(e) {
    e.target.addEventListener("mousemove", handleClipMouseMove);
    e.target.addEventListener("mouseout", handleClipMouseOut);
    e.target.addEventListener("mousedown", handleMouseDown);
    e.target.addEventListener("mouseup", handleMouseUp);
}

function handleClipMouseMove(e) {
    var x = e.offsetX;
    var y = e.offsetY;
    if (clipBox.canMove(x, y)) {
        clipBox.el.style.cursor = 'move'
    }
    //else if (clipBox.canScale(x, y)) {//if (clipBox.canScale(x, y))
    //    clipBox.el.style.cursor = 'nwse-resize';
    //} 
    else
        clipBox.el.style.cursor = 'default';

    if (clipBox.mouseaction) {
        clipBox.mouseaction(x, y)
    }
}

function handleMouseDown(e) {
    var x = e.offsetX;
    var y = e.offsetY;
    clipBox.originX = x
    clipBox.originY = y
    if (clipBox.canMove(x, y)) {
        clipBox.mouseaction = clipBoxMove
    }
    //else if (clipBox.canScale(x, y)) {
    //    clipBox.el.addEventListener('resize', clipBoxScale)
    //    //clipBox.mouseaction = clipBoxScale
    //}
    else {
        clipBox.mouseaction = null
    }
}

function handleMouseUp() {
    clipBox.mouseaction = null
}

function clipBoxMove(x, y) {
    clipBox.x = x - clipBox.originX + clipBox.x
    clipBox.y = y - clipBox.originY + clipBox.y
    clipBox.applyRect();
}

function handleClipMouseOut(e) {
    e.target.removeEventListener("mousemove", handleClipMouseMove);
    e.target.removeEventListener("mouseout", handleClipMouseOut);
    e.target.removeEventListener("mousedown", handleMouseDown);
}