"use strict";
function _assert_this_initialized(self) {
    if (self === void 0) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
    }
    return self;
}
function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) {
    try {
        var info = gen[key](arg);
        var value = info.value;
    } catch (error) {
        reject(error);
        return;
    }
    if (info.done) {
        resolve(value);
    } else {
        Promise.resolve(value).then(_next, _throw);
    }
}
function _async_to_generator(fn) {
    return function() {
        var self = this, args = arguments;
        return new Promise(function(resolve, reject) {
            var gen = fn.apply(self, args);
            function _next(value) {
                asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value);
            }
            function _throw(err) {
                asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err);
            }
            _next(undefined);
        });
    };
}
function _class_call_check(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
        throw new TypeError("Cannot call a class as a function");
    }
}
function _defineProperties(target, props) {
    for(var i = 0; i < props.length; i++){
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) descriptor.writable = true;
        Object.defineProperty(target, descriptor.key, descriptor);
    }
}
function _create_class(Constructor, protoProps, staticProps) {
    if (protoProps) _defineProperties(Constructor.prototype, protoProps);
    if (staticProps) _defineProperties(Constructor, staticProps);
    return Constructor;
}
function _get_prototype_of(o) {
    _get_prototype_of = Object.setPrototypeOf ? Object.getPrototypeOf : function getPrototypeOf(o) {
        return o.__proto__ || Object.getPrototypeOf(o);
    };
    return _get_prototype_of(o);
}
function _inherits(subClass, superClass) {
    if (typeof superClass !== "function" && superClass !== null) {
        throw new TypeError("Super expression must either be null or a function");
    }
    subClass.prototype = Object.create(superClass && superClass.prototype, {
        constructor: {
            value: subClass,
            writable: true,
            configurable: true
        }
    });
    if (superClass) _set_prototype_of(subClass, superClass);
}
function _instanceof(left, right) {
    if (right != null && typeof Symbol !== "undefined" && right[Symbol.hasInstance]) {
        return !!right[Symbol.hasInstance](left);
    } else {
        return left instanceof right;
    }
}
function _possible_constructor_return(self, call) {
    if (call && (_type_of(call) === "object" || typeof call === "function")) {
        return call;
    }
    return _assert_this_initialized(self);
}
function _set_prototype_of(o, p) {
    _set_prototype_of = Object.setPrototypeOf || function setPrototypeOf(o, p) {
        o.__proto__ = p;
        return o;
    };
    return _set_prototype_of(o, p);
}
function _type_of(obj) {
    "@swc/helpers - typeof";
    return obj && typeof Symbol !== "undefined" && obj.constructor === Symbol ? "symbol" : typeof obj;
}
function _is_native_reflect_construct() {
    if (typeof Reflect === "undefined" || !Reflect.construct) return false;
    if (Reflect.construct.sham) return false;
    if (typeof Proxy === "function") return true;
    try {
        Boolean.prototype.valueOf.call(Reflect.construct(Boolean, [], function() {}));
        return true;
    } catch (e) {
        return false;
    }
}
function _create_super(Derived) {
    var hasNativeReflectConstruct = _is_native_reflect_construct();
    return function _createSuperInternal() {
        var Super = _get_prototype_of(Derived), result;
        if (hasNativeReflectConstruct) {
            var NewTarget = _get_prototype_of(this).constructor;
            result = Reflect.construct(Super, arguments, NewTarget);
        } else {
            result = Super.apply(this, arguments);
        }
        return _possible_constructor_return(this, result);
    };
}
function _ts_generator(thisArg, body) {
    var f, y, t, g, _ = {
        label: 0,
        sent: function() {
            if (t[0] & 1) throw t[1];
            return t[1];
        },
        trys: [],
        ops: []
    };
    return g = {
        next: verb(0),
        "throw": verb(1),
        "return": verb(2)
    }, typeof Symbol === "function" && (g[Symbol.iterator] = function() {
        return this;
    }), g;
    function verb(n) {
        return function(v) {
            return step([
                n,
                v
            ]);
        };
    }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while(_)try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [
                op[0] & 2,
                t.value
            ];
            switch(op[0]){
                case 0:
                case 1:
                    t = op;
                    break;
                case 4:
                    _.label++;
                    return {
                        value: op[1],
                        done: false
                    };
                case 5:
                    _.label++;
                    y = op[1];
                    op = [
                        0
                    ];
                    continue;
                case 7:
                    op = _.ops.pop();
                    _.trys.pop();
                    continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) {
                        _ = 0;
                        continue;
                    }
                    if (op[0] === 3 && (!t || op[1] > t[0] && op[1] < t[3])) {
                        _.label = op[1];
                        break;
                    }
                    if (op[0] === 6 && _.label < t[1]) {
                        _.label = t[1];
                        t = op;
                        break;
                    }
                    if (t && _.label < t[2]) {
                        _.label = t[2];
                        _.ops.push(op);
                        break;
                    }
                    if (t[2]) _.ops.pop();
                    _.trys.pop();
                    continue;
            }
            op = body.call(thisArg, _);
        } catch (e) {
            op = [
                6,
                e
            ];
            y = 0;
        } finally{
            f = t = 0;
        }
        if (op[0] & 5) throw op[1];
        return {
            value: op[0] ? op[1] : void 0,
            done: true
        };
    }
}
// Shared/Project.Web.Shared/JsCore/componentStore.ts
var alls = /* @__PURE__ */ new Map();
window["a"] = alls;
function getComponentById(id, init) {
    if (!alls.has(id) && init !== void 0) {
        if (_instanceof(init, Function)) {
            alls.set(id, init());
        } else if (_instanceof(init, Object)) {
            alls.set(id, init);
        } else {
            console.error("\u521D\u59CB\u5316\u5F02\u5E38", init);
            throw new Error("\u521D\u59CB\u5316\u5F02\u5E38");
        }
    }
    if (alls.has(id)) return alls.get(id);
    else return void 0;
}
function removeComponent(id) {
    alls.delete(id);
}
// Shared/Project.Web.Shared/JsCore/baseComponent.ts
var BaseComponent = /*#__PURE__*/ function() {
    function BaseComponent() {
        _class_call_check(this, BaseComponent);
    }
    _create_class(BaseComponent, [
        {
            key: "dispose",
            value: function dispose() {}
        }
    ], [
        {
            key: "dispose",
            value: function dispose(id) {
                var com = getComponentById(id);
                if (com) {
                    console.debug("dispose: ", id, com);
                    com.dispose();
                    removeComponent(id);
                }
            }
        }
    ]);
    return BaseComponent;
}();
// Shared/Project.Web.Shared/JsCore/handlerBase.ts
var eventUid = 1;
function makeUid(el) {
    var uid = arguments.length > 1 && arguments[1] !== void 0 ? arguments[1] : void 0;
    return uid && "".concat(uid, "::").concat(eventUid++) || el.eventUid || eventUid++;
}
var HandlerBase = /*#__PURE__*/ function() {
    function HandlerBase(el, eventType, fn, id, once) {
        var drop = arguments.length > 5 && arguments[5] !== void 0 ? arguments[5] : void 0;
        _class_call_check(this, HandlerBase);
        this.element = el;
        this.id = id;
        this.delegate = fn;
        this.type = eventType;
        this.once = once;
        this.drop = drop;
    }
    _create_class(HandlerBase, [
        {
            key: "action",
            value: function action(event) {
                if (this.once) {
                    this.off();
                    if (this.drop) this.drop();
                }
                this.delegate.apply(this.element, [
                    event
                ]);
            }
        },
        {
            key: "bind",
            value: function bind() {}
        },
        {
            key: "off",
            value: function off() {}
        }
    ]);
    return HandlerBase;
}();
// Shared/Project.Web.Shared/JsCore/customEventHandler.ts
var CustomEventHandler = /*#__PURE__*/ function(HandlerBase) {
    _inherits(CustomEventHandler, HandlerBase);
    var _super = _create_super(CustomEventHandler);
    function CustomEventHandler(el, eventType, fn, id, once) {
        var drop = arguments.length > 5 && arguments[5] !== void 0 ? arguments[5] : void 0;
        _class_call_check(this, CustomEventHandler);
        return _super.call(this, el, eventType, fn, id, once, drop);
    }
    _create_class(CustomEventHandler, [
        {
            key: "on",
            value: function on() {
                this.element.addEventListener(this.type, this.action);
            }
        },
        {
            key: "off",
            value: function off() {
                this.element.removeEventListener(this.type, this.action);
            }
        }
    ]);
    return CustomEventHandler;
}(HandlerBase);
// Shared/Project.Web.Shared/JsCore/resizeHandler.ts
var ResizeHandler = /*#__PURE__*/ function(HandlerBase) {
    _inherits(ResizeHandler, HandlerBase);
    var _super = _create_super(ResizeHandler);
    function ResizeHandler(el, fn, id, once) {
        var drop = arguments.length > 4 && arguments[4] !== void 0 ? arguments[4] : void 0;
        _class_call_check(this, ResizeHandler);
        var _this;
        _this = _super.call(this, el, "resize", fn, id, once, drop);
        _this.resizeObserver = new ResizeObserver(function(entries) {
            _this.action(entries);
        });
        return _this;
    }
    _create_class(ResizeHandler, [
        {
            key: "on",
            value: function on() {
                this.resizeObserver.observe(this.element);
            }
        },
        {
            key: "off",
            value: function off() {
                this.resizeObserver.disconnect();
            }
        }
    ]);
    return ResizeHandler;
}(HandlerBase);
// Shared/Project.Web.Shared/JsCore/eventHandlerSet.ts
var registry = /* @__PURE__ */ new Map();
var RESIZE_EVENT = "resize";
window.r = registry;
function getElementEvents(el) {
    var uid = makeUid(el);
    el.eventUid = uid;
    registry[uid] = registry[uid] || new ElementHandlerSet(el);
    return registry[uid];
}
function removeRegistry(el) {
    var uid = makeUid(el);
    delete registry[uid];
}
function addListener(el, eventType, action, once) {
    var ets = getElementEvents(el);
    if (eventType == RESIZE_EVENT) {
        ets.addResizeHandler(action, once);
    } else {
        ets.addHandler(action, eventType, once);
    }
}
var ElementHandlerSet = /*#__PURE__*/ function() {
    function ElementHandlerSet(el) {
        _class_call_check(this, ElementHandlerSet);
        this.element = el;
        this.events = /* @__PURE__ */ new Map();
    }
    _create_class(ElementHandlerSet, [
        {
            key: "addHandler",
            value: function addHandler(fn, eventType, once) {
                var _this = this;
                var uid = makeUid(this.element, eventType);
                var dropHandler = void 0;
                if (once) {
                    dropHandler = function() {
                        return _this.removeHandler(eventType, fn);
                    };
                }
                var handler = new CustomEventHandler(this.element, eventType, fn, uid, once, dropHandler);
                var handlers = this.events.get(eventType) || /* @__PURE__ */ new Map();
                handlers.set(uid, handler);
                this.events.set(eventType, handlers);
                handler.action = handler.action.bind(handler);
                handler.on();
            }
        },
        {
            key: "addResizeHandler",
            value: function addResizeHandler(fn, once) {
                var _this = this;
                var uid = makeUid(this.element, RESIZE_EVENT);
                var dropHandler = void 0;
                if (once) {
                    dropHandler = function() {
                        return _this.removeHandler(RESIZE_EVENT, fn);
                    };
                }
                var handler = new ResizeHandler(this.element, fn, uid, once, dropHandler);
                var handlers = this.events.get(RESIZE_EVENT) || /* @__PURE__ */ new Map();
                handlers.set(uid, handler);
                this.events.set(RESIZE_EVENT, handlers);
                handler.on();
            }
        },
        {
            key: "removeHandler",
            value: function removeHandler(eventType, action) {
                var handlers = this.events.get(eventType);
                if (handlers == void 0) {
                    return;
                }
                if (action) {
                    var enumerator = handlers.values();
                    var r;
                    while(r = enumerator.next(), !r.done){
                        var handler = r.value;
                        if (handler.delegate == action) {
                            handler.off();
                            handlers.delete(handler.id);
                            break;
                        }
                    }
                } else {
                    for(var h in handlers){
                        var _handlers_get;
                        (_handlers_get = handlers.get(h)) === null || _handlers_get === void 0 ? void 0 : _handlers_get.off();
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
        }
    ]);
    return ElementHandlerSet;
}();
// Shared/Project.Web.Shared/JsCore/eventHandler.ts
var EventHandler = {
    listen: function listen(el, eventType, action) {
        if (!el) return;
        addListener(el, eventType, action, false);
    },
    once: function once(el, eventType, action) {
        if (!el) return;
        addListener(el, eventType, action, true);
    },
    remove: function remove(el, eventType, action) {
        if (!el) return;
        var ets = getElementEvents(el);
        ets.removeHandler(eventType, action);
    }
};
// Shared/Project.Web.Shared/Components/ActionWatcher/ActionWatcher.razor.ts
var ActionWatcher = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_ActionWatcher, BaseComponent);
    var _super = _create_super(_ActionWatcher);
    function _ActionWatcher(options) {
        _class_call_check(this, _ActionWatcher);
        var _this;
        _this = _super.call(this);
        _this.instance = options.instance;
        _this.type = options.type;
        _this.timeout = options.timeout;
        _this.target = options.target || window.document.documentElement;
        return _this;
    }
    _create_class(_ActionWatcher, [
        {
            key: "start",
            value: function start() {
                if (this.type == 1) {
                    EventHandler.listen(this.target, "mousemove", this.debounce.bind(this));
                    EventHandler.listen(this.target, "keydown", this.debounce.bind(this));
                } else if (this.type == 2) {
                    EventHandler.listen(this.target, "mousemove", this.throttle.bind(this));
                    EventHandler.listen(this.target, "keydown", this.throttle.bind(this));
                }
            }
        },
        {
            key: "debounce",
            value: function debounce() {
                var _this = this;
                clearTimeout(this.timer);
                this.timer = window.setTimeout(function() {
                    _this.invoke();
                }, this.timeout);
            }
        },
        {
            key: "throttle",
            value: function throttle() {
                var _this = this;
                if (!this.timer) {
                    this.invoke();
                    this.timer = window.setTimeout(function() {
                        _this.timer = void 0;
                    }, this.timeout);
                }
            }
        },
        {
            key: "invoke",
            value: function invoke() {
                this.instance.invokeMethodAsync("Call");
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.target, "mousemove");
                EventHandler.remove(this.target, "keydown");
                window.clearTimeout(this.timer);
                this.timer = void 0;
            }
        }
    ], [
        {
            key: "init",
            value: function init(id, options) {
                if (!id) {
                    console.log("id is not defined");
                    return;
                }
                var watcher = getComponentById(id, function() {
                    return new _ActionWatcher(options);
                });
                watcher.start();
            }
        }
    ]);
    return _ActionWatcher;
}(BaseComponent);
// Shared/Project.Web.Shared/JsCore/utils.ts
function success(msg, payload) {
    return {
        success: true,
        message: msg,
        payload: payload
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
    document.onselectstart = function() {
        return false;
    };
}
function restoreOnSelectstart() {
    if (document.onselectstart !== storeValue["onselectstart"]) document.onselectstart = storeValue["onselectstart"];
}
function startDrag(e, moveHandler, upHandler) {
    e.stopImmediatePropagation();
    EventHandler.listen(document.documentElement, "mousemove", moveHandler);
    EventHandler.once(document.documentElement, "mouseup", function(e2) {
        if (upHandler) {
            upHandler();
        }
        EventHandler.remove(document.documentElement, "mousemove", moveHandler);
        restoreOnSelectstart();
    });
    storeOnSelectstart();
}
// Shared/Project.Web.Shared/Components/Camera/Camera.razor.ts
var Camera = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_Camera, BaseComponent);
    var _super = _create_super(_Camera);
    function _Camera(options) {
        _class_call_check(this, _Camera);
        var _this;
        _this = _super.call(this);
        _this.quality = 1;
        _this.deviceId = "";
        _this.width = 0;
        _this.height = 0;
        _this.video = options.video;
        _this.tracks = [];
        _this.quality = options.quality;
        if (options.clip) {
            _this.clipBox = new ClipBox(options);
            _this.clipBox.initEvents();
        }
        return _this;
    }
    _create_class(_Camera, [
        {
            key: "open",
            value: function open(deviceId, width, height) {
                var _this = this;
                return new Promise(function(resolve) {
                    if (navigator && navigator.mediaDevices) {
                        navigator.mediaDevices.getUserMedia({
                            video: {
                                deviceId: {
                                    exact: deviceId
                                },
                                width: {
                                    ideal: width
                                },
                                height: {
                                    ideal: height
                                }
                            }
                        }).then(function(stream) {
                            _this.width = width;
                            _this.height = height;
                            _this.deviceId = deviceId;
                            try {
                                _this.video.srcObject = stream;
                                _this.tracks = stream.getTracks();
                                _this.video.onloadedmetadata = function(e) {
                                    _this.video.play();
                                    if (_this.clipBox) _this.clipBox.setVisible(true);
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
        },
        {
            key: "capture",
            value: function capture(rotate) {
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
                        var tx = 0, ty = 0;
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
                        var angle = rotate * 90 * Math.PI / 180;
                        ctx.translate(tx, ty);
                        ctx.rotate(angle);
                        ctx.drawImage(this.video, x, y, w, h, 0, 0, w, h);
                        ctx.rotate(-angle);
                        ctx.translate(-tx, -ty);
                        var dataURL = canvas.toDataURL();
                        if (dataURL.split(",").length > 1) data = dataURL.split(",")[1];
                        return success("", data);
                    }
                    return failed("\u89C6\u9891\u72B6\u6001\u5F02\u5E38");
                } catch (e) {
                    return failed(e.message);
                }
            }
        },
        {
            key: "close",
            value: function close() {
                this.tracks.forEach(function(tra) {
                    return tra.stop();
                });
                this.tracks = [];
                if (this.video) this.video.srcObject = null;
                if (this.clipBox) this.clipBox.setVisible(false);
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                if (this.clipBox) {
                    this.clipBox.dispose();
                    this.clipBox = void 0;
                }
                this.close();
            }
        }
    ], [
        {
            key: "init",
            value: // video, quality, clip, width, height
            function init(id, options) {
                getComponentById(id, function() {
                    return new _Camera(options);
                });
            }
        },
        {
            key: "useClipBox",
            value: function useClipBox(id, options) {
                var _c_clipBox;
                var c = getComponentById(id);
                if (c && c.clipBox == void 0) {
                    c.clipBox = new ClipBox(options);
                    c.clipBox.initEvents();
                }
                (_c_clipBox = c.clipBox) === null || _c_clipBox === void 0 ? void 0 : _c_clipBox.setVisible(true);
            }
        },
        {
            key: "disableClipBox",
            value: function disableClipBox(id) {
                var c = getComponentById(id);
                if (c && c.clipBox) {
                    c.clipBox.setVisible(false);
                    c.clipBox.dispose();
                    c.clipBox = void 0;
                }
            }
        },
        {
            key: "enumerateDevices",
            value: function enumerateDevices() {
                return _async_to_generator(function() {
                    var devices;
                    return _ts_generator(this, function(_state) {
                        switch(_state.label){
                            case 0:
                                if (!(navigator && navigator.mediaDevices)) return [
                                    3,
                                    2
                                ];
                                return [
                                    4,
                                    navigator.mediaDevices.enumerateDevices()
                                ];
                            case 1:
                                devices = _state.sent();
                                return [
                                    2,
                                    success("", devices)
                                ];
                            case 2:
                                return [
                                    2,
                                    failed("\u83B7\u53D6\u8BBE\u5907\u5931\u8D25\uFF01\u8BF7\u68C0\u67E5\u8BBE\u5907\u8FDE\u63A5\u6216\u8005\u6D4F\u89C8\u5668\u914D\u7F6E\uFF01")
                                ];
                        }
                    });
                })();
            }
        },
        {
            key: "loadUserMedia",
            value: function loadUserMedia(id, deviceId, width, height) {
                return _async_to_generator(function() {
                    var camera, e;
                    return _ts_generator(this, function(_state) {
                        switch(_state.label){
                            case 0:
                                _state.trys.push([
                                    0,
                                    2,
                                    ,
                                    3
                                ]);
                                camera = getComponentById(id);
                                return [
                                    4,
                                    camera.open(deviceId, width, height)
                                ];
                            case 1:
                                return [
                                    2,
                                    _state.sent()
                                ];
                            case 2:
                                e = _state.sent();
                                return [
                                    2,
                                    failed(e.message)
                                ];
                            case 3:
                                return [
                                    2
                                ];
                        }
                    });
                })();
            }
        },
        {
            key: "closeUserMedia",
            value: function closeUserMedia(id) {
                try {
                    var camera = getComponentById(id);
                    if (camera) camera.close();
                    return success("");
                } catch (e) {
                    return failed(e.message);
                }
            }
        },
        {
            key: "capture",
            value: function capture(id, rotate) {
                var camera = getComponentById(id);
                return camera.capture(rotate);
            }
        }
    ]);
    return _Camera;
}(BaseComponent);
var ClipBox = /*#__PURE__*/ function(BaseComponent) {
    _inherits(ClipBox, BaseComponent);
    var _super = _create_super(ClipBox);
    function ClipBox(options) {
        _class_call_check(this, ClipBox);
        var _this;
        _this = _super.call(this);
        _this.el = options.clip;
        _this.w = options.width * 0.8;
        _this.h = options.height * 0.8;
        _this.x = options.width * 0.1;
        _this.y = options.height * 0.1;
        _this.videoWindowWidth = options.width;
        _this.videoWindowHeight = options.height;
        _this.el.style.width = _this.w + "px";
        _this.el.style.height = _this.h + "px";
        _this.scaleWidth = 10;
        _this.applyRect();
        return _this;
    }
    _create_class(ClipBox, [
        {
            key: "applyRect",
            value: function applyRect() {
                if (this.x < 0) this.x = 0;
                if (this.y < 0) this.y = 0;
                if (this.x > this.videoWindowWidth - this.w) this.x = this.videoWindowWidth - this.w;
                if (this.y > this.videoWindowHeight - this.h) this.y = this.videoWindowHeight - this.h;
                if (this.el.offsetWidth > 0) this.w = this.el.offsetWidth;
                if (this.el.offsetHeight > 0) this.h = this.el.offsetHeight;
                this.el.style.top = this.y + "px";
                this.el.style.left = this.x + "px";
                this.el.style["max-width"] = this.videoWindowWidth - this.x + "px";
                this.el.style["max-height"] = this.videoWindowHeight - this.y + "px";
            }
        },
        {
            key: "canMove",
            value: function canMove(x, y) {
                this.w = this.el.offsetWidth;
                this.h = this.el.offsetHeight;
                return x > this.scaleWidth && x < this.w - this.scaleWidth && y > this.scaleWidth && y < this.h - this.scaleWidth;
            }
        },
        {
            key: "setVisible",
            value: function setVisible(visible) {
                if (this.el) {
                    this.el.style.display = visible ? "block" : "none";
                }
            }
        },
        {
            key: "handleMouseDown",
            value: function handleMouseDown(e) {
                var _this = this;
                e.stopPropagation();
                if (e.ctrlKey || [
                    1,
                    2
                ].indexOf(e.button) > -1) return;
                if (window && window.getSelection()) {
                    var _window_getSelection;
                    (_window_getSelection = window.getSelection()) === null || _window_getSelection === void 0 ? void 0 : _window_getSelection.removeAllRanges();
                }
                var x = e.offsetX;
                var y = e.offsetY;
                if (this.canMove(x, y)) {
                    startDrag(e, function(event) {
                        _this.x = event.offsetX - x + _this.x;
                        _this.y = event.offsetY - y + _this.y;
                        _this.applyRect();
                    });
                }
            }
        },
        {
            key: "initEvents",
            value: function initEvents() {
                EventHandler.listen(this.el, "mousedown", this.handleMouseDown.bind(this));
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.el, "mousedown");
            }
        }
    ]);
    return ClipBox;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/EdgeWidget/EdgeWidget.razor.ts
var EdgeWidget = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_EdgeWidget, BaseComponent);
    var _super = _create_super(_EdgeWidget);
    function _EdgeWidget(options) {
        _class_call_check(this, _EdgeWidget);
        var _this;
        _this = _super.call(this);
        _this.mask = options.mask;
        _this.childContentContainer = options.container;
        _this.trigger = options.trigger;
        var containerRect = _this.childContentContainer.getBoundingClientRect();
        _this.contentWidth = containerRect.width;
        _this.show = false;
        _this.childContentContainer.style.left = -_this.getWidth() + "px";
        return _this;
    }
    _create_class(_EdgeWidget, [
        {
            key: "getWidth",
            value: function getWidth() {
                return this.contentWidth;
            }
        },
        {
            key: "bindEvents",
            value: function bindEvents() {
                EventHandler.listen(this.trigger, "click", this.toggle.bind(this));
                EventHandler.listen(this.mask, "click", this.toggle.bind(this));
            }
        },
        {
            key: "toggle",
            value: function toggle(e) {
                e.stopPropagation();
                this.mask.classList.toggle("show");
                this.childContentContainer.classList.toggle("show");
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.trigger, "click");
                EventHandler.remove(this.mask, "click");
            }
        }
    ], [
        {
            key: "init",
            value: function init(id, options) {
                getComponentById(id, function() {
                    return new _EdgeWidget(options);
                });
            }
        }
    ]);
    return _EdgeWidget;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/Fetch/Fetch.razor.ts
var Fetch = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_Fetch, BaseComponent);
    var _super = _create_super(_Fetch);
    function _Fetch() {
        _class_call_check(this, _Fetch);
        return _super.call(this);
    }
    _create_class(_Fetch, [
        {
            key: "request",
            value: function request(option) {
                return _async_to_generator(function() {
                    var req, response;
                    return _ts_generator(this, function(_state) {
                        switch(_state.label){
                            case 0:
                                req = {
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
                                    },
                                    redirect: "follow",
                                    // manual, *follow, error
                                    referrerPolicy: "no-referrer"
                                };
                                "".toLowerCase;
                                if (option.method.toLowerCase() != "get" && option.method.toLowerCase() != "head") {
                                    req.body = JSON.stringify(option.body);
                                }
                                return [
                                    4,
                                    fetch(option.url, req)
                                ];
                            case 1:
                                response = _state.sent();
                                return [
                                    2,
                                    response
                                ];
                        }
                    });
                })();
            }
        }
    ], [
        {
            key: "init",
            value: function init(id) {
                var com = getComponentById(id, function() {
                    return new _Fetch();
                });
            }
        },
        {
            key: "request",
            value: function request(id, option) {
                return _async_to_generator(function() {
                    var com, response, e;
                    return _ts_generator(this, function(_state) {
                        switch(_state.label){
                            case 0:
                                com = getComponentById(id);
                                _state.label = 1;
                            case 1:
                                _state.trys.push([
                                    1,
                                    3,
                                    ,
                                    4
                                ]);
                                return [
                                    4,
                                    com.request(option)
                                ];
                            case 2:
                                response = _state.sent();
                                if (response.ok) {
                                    return [
                                        2,
                                        success("", response.json())
                                    ];
                                } else {
                                    return [
                                        2,
                                        failed(response.text())
                                    ];
                                }
                                return [
                                    3,
                                    4
                                ];
                            case 3:
                                e = _state.sent();
                                return [
                                    2,
                                    failed("".concat(e.name, ":").concat(e.message))
                                ];
                            case 4:
                                return [
                                    2
                                ];
                        }
                    });
                })();
            }
        }
    ]);
    return _Fetch;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/FullScreen/FullScreen.razor.ts
var FullScreen = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_FullScreen, BaseComponent);
    var _super = _create_super(_FullScreen);
    function _FullScreen() {
        _class_call_check(this, _FullScreen);
        var _this;
        _this = _super.call(this);
        _this.element = document.documentElement;
        _this.document = document;
        return _this;
    }
    _create_class(_FullScreen, [
        {
            key: "toggle",
            value: function toggle() {
                if (this.isFullscreen()) {
                    this.exit();
                } else {
                    this.enter();
                }
            }
        },
        {
            key: "enter",
            value: function enter() {
                this.element.requestFullscreen() || this.element.webkitRequestFullscreen || this.element.mozRequestFullScreen || this.element.msRequestFullscreen;
            }
        },
        {
            key: "exit",
            value: function exit() {
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
        },
        {
            key: "isFullscreen",
            value: function isFullscreen() {
                return this.document.fullscreen || this.document.webkitIsFullScreen || this.document.webkitFullScreen || this.document.mozFullScreen || this.document.msFullScreent;
            }
        }
    ], [
        {
            key: "init",
            value: function init(id) {
                getComponentById(id, function() {
                    return new _FullScreen();
                });
            }
        },
        {
            key: "toggle",
            value: function toggle(id) {
                var fullscreen = getComponentById(id);
                fullscreen.toggle();
            }
        }
    ]);
    return _FullScreen;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/JsTimer/JsTimer.razor.ts
var JsTimer = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_JsTimer, BaseComponent);
    var _super = _create_super(_JsTimer);
    function _JsTimer(options) {
        _class_call_check(this, _JsTimer);
        var _this;
        _this = _super.call(this);
        _this.instance = options.dotNetRef;
        _this.interval = options.interval;
        return _this;
    }
    _create_class(_JsTimer, [
        {
            key: "start",
            value: function start() {
                var _this = this;
                this.timer = window.setInterval(function() {
                    _this.instance.invokeMethodAsync("Call");
                }, this.interval);
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                if (this.timer) {
                    window.clearInterval(this.timer);
                    this.timer = void 0;
                }
            }
        }
    ], [
        {
            key: "init",
            value: function init(id, options) {
                var timter = getComponentById(id, function() {
                    return new _JsTimer(options);
                });
                timter.start();
            }
        }
    ]);
    return _JsTimer;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/ScrollBar/Bar.ts
var Bar = /*#__PURE__*/ function(BaseComponent) {
    _inherits(Bar, BaseComponent);
    var _super = _create_super(Bar);
    function Bar(scrollbar) {
        _class_call_check(this, Bar);
        var _this;
        _this = _super.call(this);
        _this.always = false;
        _this.parent = scrollbar;
        return _this;
    }
    _create_class(Bar, [
        {
            key: "update",
            value: function update() {
                if (this.vertical && this.vertical.thumb) {
                    this.vertical.thumb.style["height"] = this.parent.sizeHeight;
                }
                if (this.horizontal && this.horizontal.thumb) {
                    this.horizontal.thumb.style["width"] = this.parent.sizeWidth;
                }
            }
        },
        {
            key: "handleScroll",
            value: function handleScroll(e) {
                if (this.parent.wrap) {
                    var offsetHeight = this.parent.wrap.offsetHeight - GAP;
                    var offsetWidth = this.parent.wrap.offsetWidth - GAP;
                    if (this.vertical) this.vertical.setMove(this.parent.wrap.scrollTop * 100 / offsetHeight * this.parent.ratioY);
                    if (this.horizontal) this.horizontal.setMove(this.parent.wrap.scrollLeft * 100 / offsetWidth * this.parent.ratioX);
                }
            }
        },
        {
            key: "setVisible",
            value: function setVisible(visible) {
                if (this.always) return;
                if (this.vertical) this.vertical.setVisible(visible);
                if (this.horizontal) this.horizontal.setVisible(visible);
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                if (this.vertical) this.vertical.dispose();
                if (this.horizontal) this.horizontal.dispose();
            }
        }
    ]);
    return Bar;
}(BaseComponent);
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
var Thumb = /*#__PURE__*/ function(BaseComponent) {
    _inherits(Thumb, BaseComponent);
    var _super = _create_super(Thumb);
    function Thumb(root, position, tracker, thumb) {
        _class_call_check(this, Thumb);
        var _this;
        _this = _super.call(this);
        _this.root = root;
        _this.position = position;
        _this.cursorDown = false;
        _this.cursorLeave = false;
        _this.tracker = tracker;
        _this.thumb = thumb;
        _this.map = BAR_MAP[position];
        _this.state = {
            X: 0,
            Y: 0
        };
        _this.setVisible(false);
        _this.initEvents();
        return _this;
    }
    _create_class(Thumb, [
        {
            key: "mouseMoveDocumentHandler",
            value: function mouseMoveDocumentHandler(e) {
                if (!this.tracker || !this.thumb) return;
                if (this.cursorDown === false) return;
                var prevPage = this.state[this.map.axis];
                if (!prevPage) return;
                var offset = (this.tracker.getBoundingClientRect()[this.map.direction] - e[this.map.client]) * -1;
                var thumbClickPosition = this.thumb[this.map.offset] - prevPage;
                var thumbPositionPercentage = (offset - thumbClickPosition) * 100 * this.root[this.map.ratio] / this.tracker[this.map.offset];
                this.root.wrap[this.map.scroll] = thumbPositionPercentage * this.root.wrap[this.map.scrollSize] / 100;
                this.updateMove();
            }
        },
        {
            key: "clickTrackerHandler",
            value: function clickTrackerHandler(e) {
                if (!this.thumb || !this.tracker || !this.root.wrap) return;
                var offset = Math.abs(e.target.getBoundingClientRect()[this.map.direction] - e[this.map.client]);
                var thumbHalf = this.thumb[this.map.offset] / 2;
                var thumbPositionPercentage = (offset - thumbHalf) * 100 * this.root[this.map.ratio] / this.tracker[this.map.offset];
                this.root.wrap[this.map.scroll] = thumbPositionPercentage * this.root.wrap[this.map.scrollSize] / 100;
            }
        },
        {
            key: "clickThumbHandler",
            value: function clickThumbHandler(e) {
                var _this = this;
                e.stopPropagation();
                if (e.ctrlKey || [
                    1,
                    2
                ].indexOf(e.button) > -1) return;
                if (window.getSelection()) {
                    var _window_getSelection;
                    (_window_getSelection = window.getSelection()) === null || _window_getSelection === void 0 ? void 0 : _window_getSelection.removeAllRanges();
                }
                this.cursorDown = true;
                startDrag(e, this.mouseMoveDocumentHandler.bind(this), function() {
                    _this.cursorDown = false;
                    _this.state[_this.map.axis] = 0;
                });
                var el = e.currentTarget;
                if (!el) return;
                this.state[this.map.axis] = el[this.map.offset] - (e[this.map.client] - el.getBoundingClientRect()[this.map.direction]);
            }
        },
        {
            key: "mouseUpDocumentHandler",
            value: function mouseUpDocumentHandler(e) {
                this.cursorDown = false;
                this.state[this.map.axis] = 0;
            }
        },
        {
            key: "setMove",
            value: function setMove(move) {
                this.thumb.style.transform = "translate".concat(this.map.axis, "(").concat(move, "%)");
            }
        },
        {
            key: "updateMove",
            value: function updateMove() {
                var offset = this.root.wrap[this.map.offset] - GAP;
                this.setMove(this.root.wrap[this.map.scroll] * 100 / offset * this.root[this.map.ratio]);
            }
        },
        {
            key: "setVisible",
            value: function setVisible(visible) {
                if (this.tracker) this.tracker.style.display = visible ? "block" : "none";
            }
        },
        {
            key: "initEvents",
            value: function initEvents() {
                EventHandler.listen(this.tracker, "mousedown", this.clickTrackerHandler.bind(this));
                EventHandler.listen(this.thumb, "mousedown", this.clickThumbHandler.bind(this));
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.tracker, "mousedown");
                EventHandler.remove(this.thumb, "mousedown");
            }
        }
    ]);
    return Thumb;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/ScrollBar/ScrollBar.razor.ts
var ScrollBar = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_ScrollBar, BaseComponent);
    var _super = _create_super(_ScrollBar);
    function _ScrollBar(options) {
        _class_call_check(this, _ScrollBar);
        var _this;
        _this = _super.call(this);
        _this.ratioX = 1;
        _this.ratioY = 1;
        _this.sizeHeight = "";
        _this.sizeWidth = "";
        _this.root = options.scrollbar;
        _this.wrap = options.wrap;
        _this.resize = options.resize;
        _this.minSize = options.minSize;
        _this.bar = new Bar(_assert_this_initialized(_this));
        var _iteratorNormalCompletion = true, _didIteratorError = false, _iteratorError = undefined;
        try {
            for(var _iterator = options.bars[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true){
                var barOption = _step.value;
                var position = barOption.position, tracker = barOption.tracker, thumb = barOption.thumb;
                var o = new Thumb(_assert_this_initialized(_this), position, tracker, thumb);
                _this.bar[position] = o;
            }
        } catch (err) {
            _didIteratorError = true;
            _iteratorError = err;
        } finally{
            try {
                if (!_iteratorNormalCompletion && _iterator.return != null) {
                    _iterator.return();
                }
            } finally{
                if (_didIteratorError) {
                    throw _iteratorError;
                }
            }
        }
        if (options.always) {
            _this.bar.setVisible(true);
            _this.bar.always = options.always;
        }
        _this.initEvents();
        return _this;
    }
    _create_class(_ScrollBar, [
        {
            key: "update",
            value: function update() {
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
        },
        {
            key: "handleMouseMove",
            value: function handleMouseMove(e) {
                this.bar.setVisible(true);
            }
        },
        {
            key: "handleMouseLeave",
            value: function handleMouseLeave(e) {
                this.bar.setVisible(false);
            }
        },
        {
            key: "handleScroll",
            value: function handleScroll(e) {
                this.bar.handleScroll(e);
            }
        },
        {
            key: "initEvents",
            value: function initEvents() {
                var _this = this;
                EventHandler.listen(this.root, "mousemove", this.handleMouseMove.bind(this));
                EventHandler.listen(this.root, "mouseleave", this.handleMouseLeave.bind(this));
                EventHandler.listen(this.wrap, "scroll", this.handleScroll.bind(this));
                EventHandler.listen(this.wrap, "resize", function(entries) {
                    _this.update();
                });
                EventHandler.listen(this.resize, "resize", function() {
                    _this.update();
                });
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                this.bar.dispose();
                EventHandler.remove(this.root, "mousemove");
                EventHandler.remove(this.root, "mouseleave");
                EventHandler.remove(this.wrap, "scroll");
                EventHandler.remove(this.wrap, "resize");
                EventHandler.remove(this.resize, "resize");
            }
        }
    ], [
        {
            key: "init",
            value: // scrollbar, wrap, resize, minSize, always
            function init(id, options) {
                getComponentById(id, function() {
                    return new _ScrollBar(options);
                });
            }
        }
    ]);
    return _ScrollBar;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/ScrollBar/HorizontalScroll.razor.ts
var HorizontalScroll = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_HorizontalScroll, BaseComponent);
    var _super = _create_super(_HorizontalScroll);
    function _HorizontalScroll(element) {
        _class_call_check(this, _HorizontalScroll);
        var _this;
        _this = _super.call(this);
        _this.wheel = "";
        _this.scroll = function(event) {
            if (_this.wrap.clientWidth >= _this.wrap.scrollWidth) {
                debugger;
                return;
            }
            _this.wrap.scrollLeft += event.deltaY ? event.deltaY : event.detail && event.detail !== 0 ? event.detail : -event.wheelDelta;
        };
        _this.wrap = element;
        _this.initEvents();
        return _this;
    }
    _create_class(_HorizontalScroll, [
        {
            key: "handleWheelEvent",
            value: function handleWheelEvent() {
                var wheel = "";
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
        },
        {
            key: "initEvents",
            value: function initEvents() {
                this.handleWheelEvent();
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.wrap, this.wheel, this.scroll);
            }
        }
    ], [
        {
            key: "init",
            value: function init(id, element) {
                getComponentById(id, function() {
                    return new _HorizontalScroll(element);
                });
            }
        }
    ]);
    return _HorizontalScroll;
}(BaseComponent);
// Shared/Project.Web.Shared/Components/SplitView/SplitView.razor.ts
var SplitView = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_SplitView, BaseComponent);
    var _super = _create_super(_SplitView);
    function _SplitView(doms, options) {
        _class_call_check(this, _SplitView);
        var _this;
        _this = _super.call(this);
        _this.drag = false;
        _this.panel1 = doms.panel1;
        _this.panel2 = doms.panel2;
        _this.separator = doms.separator;
        _this.direction = options.direction;
        _this.max = options.max;
        _this.min = options.min;
        _this.init = options.initWidth;
        _this.setup();
        return _this;
    }
    _create_class(_SplitView, [
        {
            key: "setup",
            value: function setup() {
                EventHandler.listen(this.separator, "mousedown", this.handleMouseDown.bind(this));
                if (this.panel1.parentElement) EventHandler.listen(this.panel1.parentElement, "resize", this.refresh.bind(this));
            }
        },
        {
            key: "refresh",
            value: function refresh() {
                if (this.drag) return;
                this.panel1.style.width = this.init;
            }
        },
        {
            key: "handleMouseDown",
            value: function handleMouseDown(e) {
                var _this = this;
                var _this_panel1_parentElement;
                e.stopPropagation();
                var wrapRect = (_this_panel1_parentElement = this.panel1.parentElement) === null || _this_panel1_parentElement === void 0 ? void 0 : _this_panel1_parentElement.getBoundingClientRect();
                var separatorRect = this.separator.getBoundingClientRect();
                var separatorOffset = this.direction === "row" ? e.pageX - separatorRect.left : e.pageY - separatorRect.top;
                var handler = this.direction === "row" ? this.modeRow : this.modeColumn;
                this.drag = true;
                startDrag(e, function(event) {
                    handler.call(_this, event, wrapRect, separatorRect, separatorOffset);
                }, function() {
                    _this.drag = false;
                });
            }
        },
        {
            key: "modeRow",
            value: function modeRow(e, wrapRect, spRect, spOffset) {
                var clientRect = this.panel1.getBoundingClientRect();
                var offset = e.pageX - clientRect.left - spOffset + spRect.width / 2;
                var offsetPercent = getFinalPercent(offset, wrapRect.width, this.max, this.min);
                var paneLengthPercent = offsetPercent.toFixed(2);
                this.panel1.style.width = "calc(".concat(paneLengthPercent, "% - ").concat(spRect.width / 2, "px)");
            }
        },
        {
            key: "modeColumn",
            value: function modeColumn(e, wrapRect, spRect, spOffset) {
                var clientRect = this.panel1.getBoundingClientRect();
                var offset = e.pageY - clientRect.top - spOffset + spRect.height / 2;
                var offsetPercent = getFinalPercent(offset, wrapRect.height, this.max, this.min);
                var paneLengthPercent = offsetPercent.toFixed(2);
                this.panel1.style.height = "calc(".concat(paneLengthPercent, "% - ").concat(spRect.height / 2, "px)");
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                EventHandler.remove(this.separator, "mousedown");
                if (this.panel1.parentElement) EventHandler.remove(this.panel1.parentElement, "resize");
            }
        }
    ], [
        {
            key: "init",
            value: function init(id, doms, options) {
                getComponentById(id, function() {
                    return new _SplitView(doms, options);
                });
            }
        }
    ]);
    return _SplitView;
}(BaseComponent);
function getFinalPercent(offset, total, max, min) {
    var p = offset / total * 100;
    if (max.endsWith("%")) {
        var l = Number(max.replace("%", ""));
        if (p > l) p = l;
    } else if (max.endsWith("px")) {
        var l1 = Number(max.replace("px", ""));
        if (offset > l1) {
            p = l1 / total * 100;
        }
    }
    if (min.endsWith("%")) {
        var l2 = Number(min.replace("%", ""));
        if (p < l2) p = l2;
    } else if (min.endsWith("px")) {
        var l3 = Number(min.replace("px", ""));
        if (offset < l3) {
            p = l3 / total * 100;
        }
    }
    return p;
}
// Shared/Project.Web.Shared/Components/WaterMark/WaterMark.razor.ts
var WaterMark = /*#__PURE__*/ function(BaseComponent) {
    _inherits(_WaterMark, BaseComponent);
    var _super = _create_super(_WaterMark);
    function _WaterMark(wrapper) {
        _class_call_check(this, _WaterMark);
        var _this;
        _this = _super.call(this);
        _this.wrapper = wrapper || window.document.documentElement;
        _this.ob = new MutationObserver(function(entries) {
            var _iteratorNormalCompletion = true, _didIteratorError = false, _iteratorError = undefined;
            try {
                var _loop = function() {
                    var entry = _step.value;
                    if (entry.type === "childList") {
                        var removeNodes = entry.removedNodes;
                        removeNodes.forEach(function(node) {
                            if (node === _this.mask) {
                                _this.setWatermark(_this.options);
                            }
                            if (entry.target === _this.mask) {
                                _this.setWatermark(_this.options);
                            }
                        });
                    }
                };
                for(var _iterator = entries[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true)_loop();
            } catch (err) {
                _didIteratorError = true;
                _iteratorError = err;
            } finally{
                try {
                    if (!_iteratorNormalCompletion && _iterator.return != null) {
                        _iterator.return();
                    }
                } finally{
                    if (_didIteratorError) {
                        throw _iteratorError;
                    }
                }
            }
        });
        _this.ob.observe(_this.wrapper, {
            attributes: true,
            childList: true,
            characterData: true,
            subtree: true
        });
        return _this;
    }
    _create_class(_WaterMark, [
        {
            key: "setWatermark",
            value: function setWatermark(options) {
                if (this.mask) {
                    this.mask.remove();
                }
                this.options = options;
                this.mask = createDiv();
                var url = drawMask(options);
                this.mask.style.backgroundSize = "".concat(options.gapX + options.width, "px");
                this.mask.style.backgroundImage = "url(".concat(url, ")");
                this.wrapper.appendChild(this.mask);
            }
        },
        {
            key: "dispose",
            value: function dispose() {
                this.ob.disconnect();
            }
        }
    ], [
        {
            key: "setWatermark",
            value: function setWatermark(id, wrapper, options) {
                var com = getComponentById(id, function() {
                    return new _WaterMark(wrapper);
                });
                com.setWatermark(options);
            }
        }
    ]);
    return _WaterMark;
}(BaseComponent);
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
function drawMask(param) {
    var top = param.top, width = param.width, height = param.height, gapX = param.gapX, gapY = param.gapY, rotate = param.rotate, alpha = param.alpha, lineSpace = param.lineSpace, fontSize = param.fontSize, fontColor = param.fontColor, contents = param.contents;
    var canvas = document.createElement("canvas");
    var ctx = canvas.getContext("2d");
    if (!ctx) {
        console.warn("Current environment does not support Canvas, cannot draw watermarks.");
        return;
    }
    var ratio = window.devicePixelRatio || 1;
    var canvasWidth = (gapX + width) * ratio;
    var canvasHeight = (gapY + height) * ratio;
    canvas.width = canvasWidth;
    canvas.height = canvasHeight;
    canvas.style.width = "".concat(gapX + width, "px");
    canvas.style.height = "".concat(gapY + height, "px");
    ctx.rotate(Math.PI / 180 * Number(rotate));
    ctx.globalAlpha = alpha;
    var markWidth = width * ratio;
    var markHeight = height * ratio;
    ctx.fillStyle = "transparent";
    ctx.fillRect(0, 0, markWidth, markHeight);
    var fontFamily = "sans-serif";
    var fontWeight = "normal";
    var textAlign = "start";
    var fontStyle = "normal";
    var markSize = Number(fontSize) * ratio;
    for(var i = 0; i < contents.length; i++){
        var text = contents[i];
        top += lineSpace;
        ctx.font = "".concat(fontStyle, " normal ").concat(fontWeight, " ").concat(markSize, "px/").concat(markHeight, "px ").concat(fontFamily);
        ctx.textAlign = textAlign;
        ctx.textBaseline = "top";
        ctx.fillStyle = fontColor;
        ctx.fillText(text, 0, top * ratio);
    }
    return canvas.toDataURL();
}
// Shared/Project.AppCore/Layouts/LayoutComponents/NavTabs.razor.ts
var NavTabs = /*#__PURE__*/ function() {
    function NavTabs() {
        _class_call_check(this, NavTabs);
    }
    _create_class(NavTabs, null, [
        {
            key: "getMenuWidth",
            value: function getMenuWidth() {
                var menu = window.document.querySelector(".nav-menu");
                return menu === null || menu === void 0 ? void 0 : menu.offsetWidth;
            }
        }
    ]);
    return NavTabs;
}();
// Shared/Project.Web.Shared/Components/Downloader/Downloader.razor.ts
var Downloader = /*#__PURE__*/ function() {
    function Downloader() {
        _class_call_check(this, Downloader);
    }
    _create_class(Downloader, null, [
        {
            key: "download",
            value: function download(_, payload) {
                var tempform = document.createElement("form");
                tempform.action = "/api/download";
                tempform.method = "post";
                tempform.style.display = "none";
                var keys = Object.keys(payload);
                for(var i = 0; i < keys.length; i++){
                    var input = document.createElement("input");
                    var key = keys[i];
                    input.hidden = true;
                    input.name = key;
                    input.value = payload[key];
                    tempform.appendChild(input);
                }
                var submit = document.createElement("input");
                submit.type = "submit";
                tempform.appendChild(submit);
                document.body.appendChild(tempform);
                tempform.submit();
                document.body.removeChild(tempform);
            }
        }
    ]);
    return Downloader;
}();
// main.ts
window.components = {
    ActionWatcher: ActionWatcher,
    Camera: Camera,
    EdgeWidget: EdgeWidget,
    Fetch: Fetch,
    FullScreen: FullScreen,
    JsTimer: JsTimer,
    ScrollBar: ScrollBar,
    HorizontalScroll: HorizontalScroll,
    SplitView: SplitView,
    WaterMark: WaterMark,
    NavTabs: NavTabs,
    Downloader: Downloader
};
//# sourceMappingURL=blazor-admin-project.js.map