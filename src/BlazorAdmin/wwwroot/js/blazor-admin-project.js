"use strict";
var __async = (__this, __arguments, generator) => {
  return new Promise((resolve, reject) => {
    var fulfilled = (value) => {
      try {
        step(generator.next(value));
      } catch (e) {
        reject(e);
      }
    };
    var rejected = (value) => {
      try {
        step(generator.throw(value));
      } catch (e) {
        reject(e);
      }
    };
    var step = (x) => x.done ? resolve(x.value) : Promise.resolve(x.value).then(fulfilled, rejected);
    step((generator = generator.apply(__this, __arguments)).next());
  });
};

// utilsAggregation.ts
function closeWin() {
  window.close();
}
function openWindow(url, width, height, target) {
  const cW = window.outerWidth;
  const cH = window.outerHeight;
  let w = cW, h = cH;
  if (width <= 1) {
    w = cW * width;
  }
  if (height <= 1) {
    h = cH * height;
  }
  const top = (cH - h) / 2 + window.screenTop;
  const left = (cW - w) / 2 + window.screenLeft;
  const features = `popup=yes, left=${left}, top=${top}, width=${w}, height=${h}`;
  console.log(features);
  window.open(url, target, features);
}
function getClient() {
  return __async(this, null, function* () {
    if (window.opener !== null) {
      return [null, null];
    }
    const response = yield fetch("/ip.client");
    const ip = yield response.text();
    return [ip, navigator.userAgent];
  });
}
var utilsAggregation_default = {
  closeWin,
  openWindow,
  getClient
};

// Shared/Project.Web.Shared/JsCore/componentStore.ts
var allComponentMap = /* @__PURE__ */ new Map();
window.allComponentMap = allComponentMap;
function getComponentById(id, init) {
  if (!allComponentMap.has(id) && init !== void 0) {
    if (init instanceof Function) {
      allComponentMap.set(id, init());
    } else if (init instanceof Object) {
      allComponentMap.set(id, init);
    } else {
      console.error("\u521D\u59CB\u5316\u5F02\u5E38", init);
      throw new Error("\u521D\u59CB\u5316\u5F02\u5E38");
    }
  }
  if (allComponentMap.has(id))
    return allComponentMap.get(id);
  else
    return void 0;
}
function removeComponent(id) {
  allComponentMap.delete(id);
}

// Shared/Project.Web.Shared/JsCore/baseComponent.ts
var BaseComponent = class {
  dispose() {
  }
  static dispose(id) {
    const com = getComponentById(id);
    if (com) {
      console.debug("dispose: ", id, com);
      com.dispose();
      removeComponent(id);
    }
  }
};

// Shared/Project.Web.Shared/JsCore/handlerBase.ts
var eventUid = 1;
function makeUid(el, uid = void 0) {
  return uid && `${uid}::${eventUid++}` || el.eventUid || eventUid++;
}
var HandlerBase = class {
  constructor(el, eventType, fn, id, once, drop = void 0) {
    this.element = el;
    this.id = id;
    this.delegate = fn;
    this.type = eventType;
    this.once = once;
    this.drop = drop;
  }
  action(event) {
    if (this.once) {
      this.off();
      if (this.drop) this.drop();
    }
    this.delegate.apply(this.element, [event]);
  }
  bind() {
  }
  off() {
  }
};

// Shared/Project.Web.Shared/JsCore/customEventHandler.ts
var CustomEventHandler = class extends HandlerBase {
  constructor(el, eventType, fn, id, once, drop = void 0) {
    super(el, eventType, fn, id, once, drop);
  }
  on() {
    this.element.addEventListener(this.type, this.action);
  }
  off() {
    this.element.removeEventListener(this.type, this.action);
  }
};

// Shared/Project.Web.Shared/JsCore/resizeHandler.ts
var ResizeHandler = class extends HandlerBase {
  constructor(el, fn, id, once, drop = void 0) {
    super(el, "resize", fn, id, once, drop);
    this.resizeObserver = new ResizeObserver((entries) => {
      this.action(entries);
    });
  }
  on() {
    this.resizeObserver.observe(this.element);
  }
  off() {
    this.resizeObserver.disconnect();
  }
};

// Shared/Project.Web.Shared/JsCore/eventHandlerSet.ts
var registry = /* @__PURE__ */ new Map();
var RESIZE_EVENT = "resize";
window.allEventsMap = registry;
function getElementEvents(el) {
  const uid = makeUid(el);
  el.eventUid = uid;
  registry[uid] = registry[uid] || new ElementHandlerSet(el);
  return registry[uid];
}
function removeRegistry(el) {
  const uid = makeUid(el);
  delete registry[uid];
}
function addListener(el, eventType, action, once) {
  const ets = getElementEvents(el);
  if (eventType == RESIZE_EVENT) {
    ets.addResizeHandler(action, once);
  } else {
    ets.addHandler(action, eventType, once);
  }
}
var ElementHandlerSet = class {
  constructor(el) {
    this.element = el;
    this.events = /* @__PURE__ */ new Map();
  }
  addHandler(fn, eventType, once) {
    const uid = makeUid(this.element, eventType);
    let dropHandler = void 0;
    if (once) {
      dropHandler = () => this.removeHandler(eventType, fn);
    }
    var handler = new CustomEventHandler(this.element, eventType, fn, uid, once, dropHandler);
    const handlers = this.events.get(eventType) || /* @__PURE__ */ new Map();
    handlers.set(uid, handler);
    this.events.set(eventType, handlers);
    handler.action = handler.action.bind(handler);
    handler.on();
  }
  addResizeHandler(fn, once) {
    const uid = makeUid(this.element, RESIZE_EVENT);
    let dropHandler = void 0;
    if (once) {
      dropHandler = () => this.removeHandler(RESIZE_EVENT, fn);
    }
    var handler = new ResizeHandler(this.element, fn, uid, once, dropHandler);
    const handlers = this.events.get(RESIZE_EVENT) || /* @__PURE__ */ new Map();
    handlers.set(uid, handler);
    this.events.set(RESIZE_EVENT, handlers);
    handler.on();
  }
  removeHandler(eventType, action) {
    var _a;
    const handlers = this.events.get(eventType);
    if (handlers == void 0) {
      return;
    }
    if (action) {
      const enumerator = handlers.values();
      let r;
      while (r = enumerator.next(), !r.done) {
        const handler = r.value;
        if (handler.delegate == action) {
          handler.off();
          handlers.delete(handler.id);
          break;
        }
      }
    } else {
      for (var h in handlers) {
        (_a = handlers.get(h)) == null ? void 0 : _a.off();
        handlers.delete(h);
      }
    }
    if (handlers.size == 0) {
      this.events.delete(eventType);
    }
    if (this.events.size == 0) {
      removeRegistry(this.element);
    }
  }
};

// Shared/Project.Web.Shared/JsCore/eventHandler.ts
var EventHandler = {
  listen: function(el, eventType, action) {
    if (!el) return;
    addListener(el, eventType, action, false);
  },
  once: function(el, eventType, action) {
    if (!el) return;
    addListener(el, eventType, action, true);
  },
  remove: function(el, eventType, action) {
    if (!el) return;
    const ets = getElementEvents(el);
    ets.removeHandler(eventType, action);
  }
};

// Shared/Project.Web.Shared/Components/ActionWatcher/ActionWatcher.razor.ts
var ActionWatcher = class _ActionWatcher extends BaseComponent {
  constructor(options) {
    super();
    this.instance = options.instance;
    this.type = options.type;
    this.timeout = options.timeout;
    this.target = options.target || window.document.documentElement;
  }
  start() {
    if (this.type == 1) {
      EventHandler.listen(this.target, "mousemove", this.debounce.bind(this));
      EventHandler.listen(this.target, "keydown", this.debounce.bind(this));
    } else if (this.type == 2) {
      EventHandler.listen(this.target, "mousemove", this.throttle.bind(this));
      EventHandler.listen(this.target, "keydown", this.throttle.bind(this));
    }
  }
  debounce() {
    clearTimeout(this.timer);
    this.timer = window.setTimeout(() => {
      this.invoke();
    }, this.timeout);
  }
  throttle() {
    if (!this.timer) {
      this.invoke();
      this.timer = window.setTimeout(() => {
        this.timer = void 0;
      }, this.timeout);
    }
  }
  invoke() {
    this.instance.invokeMethodAsync("Call");
  }
  dispose() {
    EventHandler.remove(this.target, "mousemove");
    EventHandler.remove(this.target, "keydown");
    window.clearTimeout(this.timer);
    this.timer = void 0;
  }
  static init(id, options) {
    if (!id) {
      console.log("id is not defined");
      return;
    }
    var watcher = getComponentById(id, () => {
      return new _ActionWatcher(options);
    });
    watcher.start();
  }
};

// Shared/Project.Web.Shared/JsCore/utils.ts
function success(msg, payload) {
  return {
    success: true,
    message: msg,
    payload
  };
}
function failed(msg) {
  return {
    success: false,
    message: msg,
    payload: null
  };
}
var GAP = 4;

// Shared/Project.Web.Shared/JsCore/dragHelper.ts
var storeValue = {};
function storeOnSelectstart() {
  storeValue["onselectstart"] = document.onselectstart;
  document.onselectstart = () => false;
}
function restoreOnSelectstart() {
  if (document.onselectstart !== storeValue["onselectstart"])
    document.onselectstart = storeValue["onselectstart"];
}
function startDrag(e, moveHandler, upHandler) {
  e.stopImmediatePropagation();
  EventHandler.listen(document.documentElement, "mousemove", moveHandler);
  EventHandler.once(document.documentElement, "mouseup", (e2) => {
    if (upHandler) {
      upHandler();
    }
    EventHandler.remove(document.documentElement, "mousemove", moveHandler);
    restoreOnSelectstart();
  });
  storeOnSelectstart();
}

// Shared/Project.Web.Shared/Components/Camera/Camera.razor.ts
var Camera = class _Camera extends BaseComponent {
  constructor(options) {
    super();
    this.deviceId = "";
    this.width = 0;
    this.height = 0;
    this.video = options.video;
    this.tracks = [];
    this.quality = options.quality;
    this.format = options.format;
    if (options.clip) {
      this.clipBox = new ClipBox(options);
      this.clipBox.initEvents();
    }
  }
  open(deviceId, width, height) {
    return new Promise((resolve) => {
      if (navigator && navigator.mediaDevices) {
        navigator.mediaDevices.getUserMedia({
          video: {
            deviceId: { exact: deviceId },
            width: { ideal: width },
            height: { ideal: height }
          }
        }).then((stream) => {
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
              resolve(success("\u5F00\u59CB\u64AD\u653E"));
            };
          } catch (e) {
            resolve(failed(e.message));
          }
        }).catch(function(err) {
          resolve(failed(err.message));
        });
      } else {
        resolve(failed("\u6D4F\u89C8\u5668\u4E0D\u652F\u6301"));
      }
    });
  }
  capture(rotate) {
    try {
      if (this.video && this.video.readyState > 2) {
        var data = "";
        var canvas = document.createElement("canvas");
        var ctx = canvas.getContext("2d");
        if (ctx == null) {
          return failed("\u83B7\u53D6Canvas Context\u5931\u8D25");
        }
        var x = 0, y = 0, w = this.video.videoWidth, h = this.video.videoHeight;
        if (this.clipBox) {
          this.clipBox.applyRect();
          var scaleX = this.video.videoWidth / this.clipBox.videoWindowWidth;
          var scaleY = this.video.videoHeight / this.clipBox.videoWindowHeight;
          x = this.clipBox.x * scaleX;
          y = this.clipBox.y * scaleY;
          w = this.clipBox.w * scaleX;
          h = this.clipBox.h * scaleY;
        }
        rotate = rotate % 4;
        let tx = 0, ty = 0;
        if (rotate == 0 || rotate == 2) {
          canvas.width = w;
          canvas.height = h;
          if (rotate == 2) {
            tx = w;
            ty = h;
          }
        } else {
          canvas.width = h;
          canvas.height = w;
          if (rotate == 1) {
            tx = h;
          } else {
            ty = w;
          }
        }
        let angle = rotate * 90 * Math.PI / 180;
        ctx.translate(tx, ty);
        ctx.rotate(angle);
        ctx.drawImage(this.video, x, y, w, h, 0, 0, w, h);
        ctx.rotate(-angle);
        ctx.translate(-tx, -ty);
        var dataURL = canvas.toDataURL(this.format, this.quality);
        if (dataURL.split(",").length > 1)
          data = dataURL.split(",")[1];
        return success("", data);
      }
      return failed("\u89C6\u9891\u72B6\u6001\u5F02\u5E38");
    } catch (e) {
      return failed(e.message);
    }
  }
  close() {
    this.tracks.forEach((tra) => tra.stop());
    this.tracks = [];
    if (this.video)
      this.video.srcObject = null;
    if (this.clipBox)
      this.clipBox.setVisible(false);
  }
  dispose() {
    if (this.clipBox) {
      this.clipBox.dispose();
      this.clipBox = void 0;
    }
    this.close();
  }
  // video, quality, clip, width, height
  static init(id, options) {
    getComponentById(id, () => {
      return new _Camera(options);
    });
  }
  static useClipBox(id, options) {
    var _a;
    var c = getComponentById(id);
    if (c && c.clipBox == void 0) {
      c.clipBox = new ClipBox(options);
      c.clipBox.initEvents();
    }
    (_a = c.clipBox) == null ? void 0 : _a.setVisible(true);
  }
  static disableClipBox(id) {
    var c = getComponentById(id);
    if (c && c.clipBox) {
      c.clipBox.setVisible(false);
      c.clipBox.dispose();
      c.clipBox = void 0;
    }
  }
  static enumerateDevices() {
    return __async(this, null, function* () {
      if (navigator && navigator.mediaDevices) {
        var devices = yield navigator.mediaDevices.enumerateDevices();
        return success("", devices);
      }
      return failed("\u83B7\u53D6\u8BBE\u5907\u5931\u8D25\uFF01\u8BF7\u68C0\u67E5\u8BBE\u5907\u8FDE\u63A5\u6216\u8005\u6D4F\u89C8\u5668\u914D\u7F6E\uFF01");
    });
  }
  static loadUserMedia(id, deviceId, width, height) {
    return __async(this, null, function* () {
      try {
        const camera = getComponentById(id);
        return yield camera.open(deviceId, width, height);
      } catch (e) {
        return failed(e.message);
      }
    });
  }
  static closeUserMedia(id) {
    try {
      const camera = getComponentById(id);
      if (camera)
        camera.close();
      return success("");
    } catch (e) {
      return failed(e.message);
    }
  }
  static capture(id, rotate) {
    const camera = getComponentById(id);
    return camera.capture(rotate);
  }
};
var ClipBox = class extends BaseComponent {
  constructor(options) {
    super();
    this.el = options.clip;
    this.w = options.width * 0.8;
    this.h = options.height * 0.8;
    this.x = options.width * 0.1;
    this.y = options.height * 0.1;
    this.videoWindowWidth = options.width;
    this.videoWindowHeight = options.height;
    this.el.style.width = this.w + "px";
    this.el.style.height = this.h + "px";
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
    if (this.el.offsetWidth > 0)
      this.w = this.el.offsetWidth;
    if (this.el.offsetHeight > 0)
      this.h = this.el.offsetHeight;
    this.el.style.top = this.y + "px";
    this.el.style.left = this.x + "px";
    this.el.style["max-width"] = this.videoWindowWidth - this.x + "px";
    this.el.style["max-height"] = this.videoWindowHeight - this.y + "px";
  }
  canMove(x, y) {
    this.w = this.el.offsetWidth;
    this.h = this.el.offsetHeight;
    return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth;
  }
  setVisible(visible) {
    if (this.el) {
      this.el.style.display = visible ? "block" : "none";
    }
  }
  handleMouseDown(e) {
    var _a;
    e.stopPropagation();
    if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
    if (window && window.getSelection()) {
      (_a = window.getSelection()) == null ? void 0 : _a.removeAllRanges();
    }
    var x = e.offsetX;
    var y = e.offsetY;
    if (this.canMove(x, y)) {
      startDrag(e, (event) => {
        this.x = event.offsetX - x + this.x;
        this.y = event.offsetY - y + this.y;
        this.applyRect();
      });
    }
  }
  initEvents() {
    EventHandler.listen(this.el, "mousedown", this.handleMouseDown.bind(this));
  }
  dispose() {
    EventHandler.remove(this.el, "mousedown");
  }
};

// Shared/Project.Web.Shared/Components/EdgeWidget/EdgeWidget.razor.ts
var EdgeWidget = class _EdgeWidget extends BaseComponent {
  //show: boolean
  constructor(options) {
    super();
    this.mask = options.mask;
    this.childContentContainer = options.container;
    this.trigger = options.trigger;
    var containerRect = this.childContentContainer.getBoundingClientRect();
    this.contentWidth = containerRect.width;
    this.childContentContainer.style.left = -this.getWidth() + "px";
    this.bindEvents();
  }
  getWidth() {
    return this.contentWidth;
  }
  bindEvents() {
    EventHandler.listen(this.trigger, "click", this.toggle.bind(this));
    EventHandler.listen(this.mask, "click", this.toggle.bind(this));
  }
  toggle(e) {
    e.stopPropagation();
    this.mask.classList.toggle("show");
    this.childContentContainer.classList.toggle("show");
  }
  dispose() {
    EventHandler.remove(this.trigger, "click");
    EventHandler.remove(this.mask, "click");
  }
  static init(id, options) {
    getComponentById(id, () => {
      return new _EdgeWidget(options);
    });
  }
};

// Shared/Project.Web.Shared/Components/Fetch/Fetch.razor.ts
var Fetch = class _Fetch extends BaseComponent {
  constructor() {
    super();
  }
  request(option) {
    return __async(this, null, function* () {
      var req = {
        method: option.method,
        // *GET, POST, PUT, DELETE, etc.
        mode: "cors",
        // no-cors, *cors, same-origin
        cache: "no-cache",
        // *default, no-cache, reload, force-cache, only-if-cached
        credentials: "same-origin",
        // include, *same-origin, omit
        headers: {
          "Content-Type": "application/json"
          // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: "follow",
        // manual, *follow, error
        referrerPolicy: "no-referrer"
        // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
      };
      "".toLowerCase;
      if (option.method.toLowerCase() != "get" && option.method.toLowerCase() != "head") {
        req.body = JSON.stringify(option.body);
      }
      const response = yield fetch(option.url, req);
      return response;
    });
  }
  static init(id) {
    var com = getComponentById(id, () => {
      return new _Fetch();
    });
  }
  static request(id, option) {
    return __async(this, null, function* () {
      var com = getComponentById(id);
      try {
        var response = yield com.request(option);
        if (response.ok) {
          return success("", response.json());
        } else {
          return failed(response.text());
        }
      } catch (e) {
        return failed(`${e.name}:${e.message}`);
      }
    });
  }
};

// Shared/Project.Web.Shared/Components/FullScreen/FullScreen.razor.ts
var FullScreen = class _FullScreen extends BaseComponent {
  constructor() {
    super();
    this.element = document.documentElement;
    this.document = document;
  }
  toggle() {
    if (this.isFullscreen()) {
      this.exit();
    } else {
      this.enter();
    }
  }
  enter() {
    this.element.requestFullscreen() || this.element.webkitRequestFullscreen || this.element.mozRequestFullScreen || this.element.msRequestFullscreen;
  }
  exit() {
    if (this.document.exitFullscreen) {
      this.document.exitFullscreen();
    } else if (this.document.mozCancelFullScreen) {
      this.document.mozCancelFullScreen();
    } else if (this.document.webkitExitFullscreen) {
      this.document.webkitExitFullscreen();
    } else if (this.document.msExitFullscreen) {
      this.document.msExitFullscreen();
    }
  }
  isFullscreen() {
    return this.document.fullscreen || this.document.webkitIsFullScreen || this.document.webkitFullScreen || this.document.mozFullScreen || this.document.msFullScreent;
  }
  static init(id) {
    getComponentById(id, () => {
      return new _FullScreen();
    });
  }
  static toggle(id) {
    const fullscreen = getComponentById(id);
    fullscreen.toggle();
  }
};

// Shared/Project.Web.Shared/Components/JsTimer/JsTimer.razor.ts
var JsTimer = class _JsTimer extends BaseComponent {
  constructor(options) {
    super();
    this.instance = options.dotNetRef;
    this.interval = options.interval;
  }
  start() {
    this.timer = window.setInterval(() => {
      this.instance.invokeMethodAsync("Call");
    }, this.interval);
  }
  dispose() {
    if (this.timer) {
      window.clearInterval(this.timer);
      this.timer = void 0;
    }
  }
  static init(id, options) {
    const timter = getComponentById(id, () => {
      return new _JsTimer(options);
    });
    timter.start();
  }
};

// Shared/Project.Web.Shared/Components/ScrollBar/Bar.ts
var Bar = class extends BaseComponent {
  constructor(scrollbar) {
    super();
    this.always = false;
    this.parent = scrollbar;
  }
  update() {
    if (this.vertical && this.vertical.thumb) {
      this.vertical.thumb.style["height"] = this.parent.sizeHeight;
    }
    if (this.horizontal && this.horizontal.thumb) {
      this.horizontal.thumb.style["width"] = this.parent.sizeWidth;
    }
  }
  handleScroll(e) {
    if (this.parent.wrap) {
      const offsetHeight = this.parent.wrap.offsetHeight - GAP;
      const offsetWidth = this.parent.wrap.offsetWidth - GAP;
      if (this.vertical)
        this.vertical.setMove(this.parent.wrap.scrollTop * 100 / offsetHeight * this.parent.ratioY);
      if (this.horizontal)
        this.horizontal.setMove(this.parent.wrap.scrollLeft * 100 / offsetWidth * this.parent.ratioX);
    }
  }
  setVisible(visible) {
    if (this.always) return;
    if (this.vertical) this.vertical.setVisible(visible);
    if (this.horizontal) this.horizontal.setVisible(visible);
  }
  dispose() {
    if (this.vertical) this.vertical.dispose();
    if (this.horizontal) this.horizontal.dispose();
  }
};

// Shared/Project.Web.Shared/Components/ScrollBar/Thumb.ts
var BAR_MAP = {
  vertical: {
    offset: "offsetHeight",
    scroll: "scrollTop",
    scrollSize: "scrollHeight",
    size: "height",
    key: "vertical",
    axis: "Y",
    client: "clientY",
    direction: "top",
    ratio: "ratioY"
  },
  horizontal: {
    offset: "offsetWidth",
    scroll: "scrollLeft",
    scrollSize: "scrollWidth",
    size: "width",
    key: "horizontal",
    axis: "X",
    client: "clientX",
    direction: "left",
    ratio: "ratioX"
  }
};
var Thumb = class extends BaseComponent {
  constructor(root, position, tracker, thumb) {
    super();
    this.root = root;
    this.position = position;
    this.cursorDown = false;
    this.cursorLeave = false;
    this.tracker = tracker;
    this.thumb = thumb;
    this.map = BAR_MAP[position];
    this.state = {
      X: 0,
      Y: 0
    };
    this.setVisible(false);
    this.initEvents();
  }
  mouseMoveDocumentHandler(e) {
    if (!this.tracker || !this.thumb) return;
    if (this.cursorDown === false) return;
    const prevPage = this.state[this.map.axis];
    if (!prevPage) return;
    const offset = (this.tracker.getBoundingClientRect()[this.map.direction] - e[this.map.client]) * -1;
    const thumbClickPosition = this.thumb[this.map.offset] - prevPage;
    const thumbPositionPercentage = (offset - thumbClickPosition) * 100 * this.root[this.map.ratio] / this.tracker[this.map.offset];
    this.root.wrap[this.map.scroll] = thumbPositionPercentage * this.root.wrap[this.map.scrollSize] / 100;
    this.updateMove();
  }
  clickTrackerHandler(e) {
    if (!this.thumb || !this.tracker || !this.root.wrap) return;
    const offset = Math.abs(e.target.getBoundingClientRect()[this.map.direction] - e[this.map.client]);
    const thumbHalf = this.thumb[this.map.offset] / 2;
    const thumbPositionPercentage = (offset - thumbHalf) * 100 * this.root[this.map.ratio] / this.tracker[this.map.offset];
    this.root.wrap[this.map.scroll] = thumbPositionPercentage * this.root.wrap[this.map.scrollSize] / 100;
  }
  clickThumbHandler(e) {
    var _a;
    e.stopPropagation();
    if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
    if (window.getSelection()) {
      (_a = window.getSelection()) == null ? void 0 : _a.removeAllRanges();
    }
    this.cursorDown = true;
    startDrag(e, this.mouseMoveDocumentHandler.bind(this), () => {
      this.cursorDown = false;
      this.state[this.map.axis] = 0;
    });
    const el = e.currentTarget;
    if (!el) return;
    this.state[this.map.axis] = el[this.map.offset] - (e[this.map.client] - el.getBoundingClientRect()[this.map.direction]);
  }
  mouseUpDocumentHandler(e) {
    this.cursorDown = false;
    this.state[this.map.axis] = 0;
  }
  setMove(move) {
    this.thumb.style.transform = `translate${this.map.axis}(${move}%)`;
  }
  updateMove() {
    const offset = this.root.wrap[this.map.offset] - GAP;
    this.setMove(this.root.wrap[this.map.scroll] * 100 / offset * this.root[this.map.ratio]);
  }
  setVisible(visible) {
    if (this.tracker) this.tracker.style.display = visible ? "block" : "none";
  }
  initEvents() {
    EventHandler.listen(this.tracker, "mousedown", this.clickTrackerHandler.bind(this));
    EventHandler.listen(this.thumb, "mousedown", this.clickThumbHandler.bind(this));
  }
  dispose() {
    EventHandler.remove(this.tracker, "mousedown");
    EventHandler.remove(this.thumb, "mousedown");
  }
};

// Shared/Project.Web.Shared/Components/ScrollBar/ScrollBar.razor.ts
var ScrollBar = class _ScrollBar extends BaseComponent {
  constructor(options) {
    super();
    this.ratioX = 1;
    this.ratioY = 1;
    this.sizeHeight = "";
    this.sizeWidth = "";
    this.root = options.scrollbar;
    this.wrap = options.wrap;
    this.resize = options.resize;
    this.minSize = options.minSize;
    this.bar = new Bar(this);
    for (const barOption of options.bars) {
      const { position, tracker, thumb } = barOption;
      const o = new Thumb(this, position, tracker, thumb);
      this.bar[position] = o;
    }
    if (options.always) {
      this.bar.setVisible(true);
      this.bar.always = options.always;
    }
    this.initEvents();
  }
  update() {
    if (!this.wrap) return;
    var offsetHeight = this.wrap.offsetHeight - GAP;
    var offsetWidth = this.wrap.offsetWidth - GAP;
    var scrollHeight = this.wrap.scrollHeight;
    var scrollWidth = this.wrap.scrollWidth;
    var originalHeight = offsetHeight * offsetHeight / scrollHeight;
    var originalWidth = offsetWidth * offsetWidth / scrollWidth;
    var height = Math.max(originalHeight, this.minSize);
    var width = Math.max(originalWidth, this.minSize);
    this.ratioY = originalHeight / (offsetHeight - originalHeight) / (height / (offsetHeight - height));
    this.ratioX = originalWidth / (offsetWidth - originalWidth) / (width / (offsetWidth - width));
    this.sizeHeight = height + GAP < offsetHeight ? height + "px" : "";
    this.sizeWidth = width + GAP < offsetWidth ? width + "px" : "";
    this.bar.update();
  }
  handleMouseMove(e) {
    this.bar.setVisible(true);
  }
  handleMouseLeave(e) {
    this.bar.setVisible(false);
  }
  handleScroll(e) {
    this.bar.handleScroll(e);
  }
  initEvents() {
    EventHandler.listen(this.root, "mousemove", this.handleMouseMove.bind(this));
    EventHandler.listen(this.root, "mouseleave", this.handleMouseLeave.bind(this));
    EventHandler.listen(this.wrap, "scroll", this.handleScroll.bind(this));
    EventHandler.listen(this.wrap, "resize", (entries) => {
      this.update();
    });
    EventHandler.listen(this.resize, "resize", () => {
      this.update();
    });
  }
  dispose() {
    this.bar.dispose();
    EventHandler.remove(this.root, "mousemove");
    EventHandler.remove(this.root, "mouseleave");
    EventHandler.remove(this.wrap, "scroll");
    EventHandler.remove(this.wrap, "resize");
    EventHandler.remove(this.resize, "resize");
  }
  // scrollbar, wrap, resize, minSize, always
  static init(id, options) {
    getComponentById(id, () => {
      return new _ScrollBar(options);
    });
  }
};

// Shared/Project.Web.Shared/Components/ScrollBar/HorizontalScroll.razor.ts
var HorizontalScroll = class _HorizontalScroll extends BaseComponent {
  constructor(element) {
    super();
    this.wheel = "";
    this.scroll = (event) => {
      if (this.wrap.clientWidth >= this.wrap.scrollWidth) {
        debugger;
        return;
      }
      this.wrap.scrollLeft += event.deltaY ? event.deltaY : event.detail && event.detail !== 0 ? event.detail : -event.wheelDelta;
    };
    this.wrap = element;
    this.initEvents();
  }
  handleWheelEvent() {
    let wheel = "";
    if ("onmousewheel" in this.wrap) {
      wheel = "mousewheel";
    } else if ("onwheel" in this.wrap) {
      wheel = "wheel";
    } else if ("attachEvent" in window) {
      wheel = "onmousewheel";
    } else {
      wheel = "DOMMouseScroll";
    }
    this.wheel = wheel;
    EventHandler.listen(this.wrap, wheel, this.scroll);
  }
  initEvents() {
    this.handleWheelEvent();
  }
  dispose() {
    EventHandler.remove(this.wrap, this.wheel, this.scroll);
  }
  static init(id, element) {
    getComponentById(id, () => {
      return new _HorizontalScroll(element);
    });
  }
};

// Shared/Project.Web.Shared/Components/SplitView/SplitView.razor.ts
var SplitView = class _SplitView extends BaseComponent {
  constructor(doms, options) {
    super();
    this.drag = false;
    this.panel1 = doms.panel1;
    this.panel2 = doms.panel2;
    this.separator = doms.separator;
    this.direction = options.direction;
    this.max = options.max;
    this.min = options.min;
    this.init = options.initWidth;
    this.setup();
  }
  setup() {
    EventHandler.listen(this.separator, "mousedown", this.handleMouseDown.bind(this));
    if (this.panel1.parentElement)
      EventHandler.listen(this.panel1.parentElement, "resize", this.refresh.bind(this));
  }
  refresh() {
    if (this.drag) return;
    this.panel1.style.width = this.init;
  }
  handleMouseDown(e) {
    var _a;
    e.stopPropagation();
    const wrapRect = (_a = this.panel1.parentElement) == null ? void 0 : _a.getBoundingClientRect();
    const separatorRect = this.separator.getBoundingClientRect();
    const separatorOffset = this.direction === "row" ? e.pageX - separatorRect.left : e.pageY - separatorRect.top;
    const handler = this.direction === "row" ? this.modeRow : this.modeColumn;
    this.drag = true;
    startDrag(e, (event) => {
      handler.call(this, event, wrapRect, separatorRect, separatorOffset);
    }, () => {
      this.drag = false;
    });
  }
  modeRow(e, wrapRect, spRect, spOffset) {
    const clientRect = this.panel1.getBoundingClientRect();
    const offset = e.pageX - clientRect.left - spOffset + spRect.width / 2;
    const offsetPercent = getFinalPercent(offset, wrapRect.width, this.max, this.min);
    const paneLengthPercent = offsetPercent.toFixed(2);
    this.panel1.style.width = `calc(${paneLengthPercent}% - ${spRect.width / 2}px)`;
  }
  modeColumn(e, wrapRect, spRect, spOffset) {
    const clientRect = this.panel1.getBoundingClientRect();
    const offset = e.pageY - clientRect.top - spOffset + spRect.height / 2;
    const offsetPercent = getFinalPercent(offset, wrapRect.height, this.max, this.min);
    const paneLengthPercent = offsetPercent.toFixed(2);
    this.panel1.style.height = `calc(${paneLengthPercent}% - ${spRect.height / 2}px)`;
  }
  dispose() {
    EventHandler.remove(this.separator, "mousedown");
    if (this.panel1.parentElement)
      EventHandler.remove(this.panel1.parentElement, "resize");
  }
  static init(id, doms, options) {
    getComponentById(id, () => {
      return new _SplitView(doms, options);
    });
  }
};
function getFinalPercent(offset, total, max, min) {
  let p = offset / total * 100;
  if (max.endsWith("%")) {
    const l = Number(max.replace("%", ""));
    if (p > l) p = l;
  } else if (max.endsWith("px")) {
    const l = Number(max.replace("px", ""));
    if (offset > l) {
      p = l / total * 100;
    }
  }
  if (min.endsWith("%")) {
    const l = Number(min.replace("%", ""));
    if (p < l) p = l;
  } else if (min.endsWith("px")) {
    const l = Number(min.replace("px", ""));
    if (offset < l) {
      p = l / total * 100;
    }
  }
  return p;
}

// Shared/Project.Web.Shared/Components/WaterMark/WaterMark.razor.ts
var WaterMark = class _WaterMark extends BaseComponent {
  constructor(wrapper) {
    super();
    this.wrapper = wrapper || window.document.documentElement;
    this.ob = new MutationObserver((entries) => {
      for (const entry of entries) {
        if (entry.type === "childList") {
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
      subtree: true
    });
  }
  setWatermark(options) {
    if (this.mask) {
      this.mask.remove();
    }
    this.options = options;
    this.mask = createDiv();
    let url = drawMask(options);
    this.mask.style.backgroundSize = `${options.gapX + options.width}px`;
    this.mask.style.backgroundImage = `url(${url})`;
    this.wrapper.appendChild(this.mask);
  }
  dispose() {
    this.ob.disconnect();
  }
  static setWatermark(id, wrapper, options) {
    let com = getComponentById(id, () => {
      return new _WaterMark(wrapper);
    });
    com.setWatermark(options);
  }
};
function createDiv() {
  var d = document.createElement("div");
  d.style.position = "absolute";
  d.style.left = "0";
  d.style.top = "0";
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
  const canvas = document.createElement("canvas");
  const ctx = canvas.getContext("2d");
  if (!ctx) {
    console.warn("Current environment does not support Canvas, cannot draw watermarks.");
    return;
  }
  const ratio = window.devicePixelRatio || 1;
  const canvasWidth = (gapX + width) * ratio;
  const canvasHeight = (gapY + height) * ratio;
  canvas.width = canvasWidth;
  canvas.height = canvasHeight;
  canvas.style.width = `${gapX + width}px`;
  canvas.style.height = `${gapY + height}px`;
  ctx.rotate(Math.PI / 180 * Number(rotate));
  ctx.globalAlpha = alpha;
  const markWidth = width * ratio;
  const markHeight = height * ratio;
  ctx.fillStyle = "transparent";
  ctx.fillRect(0, 0, markWidth, markHeight);
  let fontFamily = "sans-serif";
  let fontWeight = "normal";
  let textAlign = "start";
  let fontStyle = "normal";
  const markSize = Number(fontSize) * ratio;
  for (var i = 0; i < contents.length; i++) {
    let text = contents[i];
    top += lineSpace;
    ctx.font = `${fontStyle} normal ${fontWeight} ${markSize}px/${markHeight}px ${fontFamily}`;
    ctx.textAlign = textAlign;
    ctx.textBaseline = "top";
    ctx.fillStyle = fontColor;
    ctx.fillText(text, 0, top * ratio);
  }
  return canvas.toDataURL();
}

// Shared/Project.Web.Shared/Layouts/LayoutComponents/NavTabs.razor.ts
var NavTabs = class {
  static getMenuWidth() {
    var menu = window.document.querySelector(".nav-menu");
    return menu == null ? void 0 : menu.offsetWidth;
  }
};

// Shared/Project.Web.Shared/Components/Downloader/Downloader.razor.ts
var _Downloader = class _Downloader {
  static download(_, payload) {
    const tempform = document.createElement("form");
    tempform.action = "/api/download";
    tempform.method = "post";
    tempform.style.display = "none";
    const keys = Object.keys(payload);
    for (let i = 0; i < keys.length; i++) {
      const input = document.createElement("input");
      const key = keys[i];
      input.hidden = true;
      input.name = key;
      input.value = payload[key];
      tempform.appendChild(input);
    }
    const submit = document.createElement("input");
    submit.type = "submit";
    tempform.appendChild(submit);
    document.body.appendChild(tempform);
    tempform.submit();
    document.body.removeChild(tempform);
  }
};
_Downloader.downloadStream = (_, payload) => __async(_Downloader, null, function* () {
  try {
    const { filename, streamRef } = payload;
    const arrayBuffer = yield streamRef.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement("a");
    anchorElement.href = url;
    anchorElement.download = filename != null ? filename : "";
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
  } catch (e) {
    console.error(e);
  }
});
var Downloader = _Downloader;

// Shared/Project.Web.Shared/Components/SvgIcon/SvgIcon.razor.ts
var SvgIcon = class _SvgIcon extends BaseComponent {
  constructor(options) {
    super();
    this.el = options.container;
    this.iconName = options.iconName;
    this.className = options.className;
    this.style = options.style;
    this.fontSize = options.fontSize;
    this.load();
  }
  load() {
    fetch(`/icons/${this.iconName}.svg`).then((response) => {
      response.text().then((content) => {
        this.el.innerHTML = content;
        const svgEl = this.el.getElementsByTagName("svg").item(0);
        if (svgEl) {
          if (this.className) {
            var all = this.className.split(" ");
            for (const c of all) {
              if (c == null ? void 0 : c.trim()) svgEl.classList.add(c.trim());
            }
          }
          if (this.style) {
            svgEl.setAttribute("style", this.style);
          }
        }
      });
    }).catch((error) => {
      console.debug(error);
    });
  }
  static init(id, options) {
    new _SvgIcon(options);
  }
};

// main.ts
window.Utils = utilsAggregation_default;
window.BlazorProject = {
  ActionWatcher,
  Camera,
  EdgeWidget,
  Fetch,
  FullScreen,
  JsTimer,
  ScrollBar,
  HorizontalScroll,
  SplitView,
  WaterMark,
  NavTabs,
  Downloader,
  SvgIcon
};
//# sourceMappingURL=blazor-admin-project.js.map