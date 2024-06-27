"use strict";function t(t){if(void 0===t)throw new ReferenceError("this hasn't been initialised - super() hasn't been called");return t}function e(t,e,i,n,o,s,r){try{var a=t[s](r),l=a.value}catch(t){return void i(t)}a.done?e(l):Promise.resolve(l).then(n,o)}function i(t){return function(){var i=this,n=arguments;return new Promise((function(o,s){var r=t.apply(i,n);function a(t){e(r,o,s,a,l,"next",t)}function l(t){e(r,o,s,a,l,"throw",t)}a(void 0)}))}}function n(t,e){if(!(t instanceof e))throw new TypeError("Cannot call a class as a function")}function o(t,e){for(var i=0;i<e.length;i++){var n=e[i];n.enumerable=n.enumerable||!1,n.configurable=!0,"value"in n&&(n.writable=!0),Object.defineProperty(t,n.key,n)}}function s(t,e,i){return e&&o(t.prototype,e),i&&o(t,i),t}function r(t){return r=Object.setPrototypeOf?Object.getPrototypeOf:function(t){return t.__proto__||Object.getPrototypeOf(t)},r(t)}function a(t,e){if("function"!=typeof e&&null!==e)throw new TypeError("Super expression must either be null or a function");t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,writable:!0,configurable:!0}}),e&&u(t,e)}function l(t,e){return null!=e&&"undefined"!=typeof Symbol&&e[Symbol.hasInstance]?!!e[Symbol.hasInstance](t):t instanceof e}function c(e,i){return!i||"object"!=((n=i)&&"undefined"!=typeof Symbol&&n.constructor===Symbol?"symbol":typeof n)&&"function"!=typeof i?t(e):i;var n}function u(t,e){return u=Object.setPrototypeOf||function(t,e){return t.__proto__=e,t},u(t,e)}function h(t){var e=function(){if("undefined"==typeof Reflect||!Reflect.construct)return!1;if(Reflect.construct.sham)return!1;if("function"==typeof Proxy)return!0;try{return Boolean.prototype.valueOf.call(Reflect.construct(Boolean,[],(function(){}))),!0}catch(t){return!1}}();return function(){var i,n=r(t);if(e){var o=r(this).constructor;i=Reflect.construct(n,arguments,o)}else i=n.apply(this,arguments);return c(this,i)}}function d(t,e){var i,n,o,s,r={label:0,sent:function(){if(1&o[0])throw o[1];return o[1]},trys:[],ops:[]};return s={next:a(0),throw:a(1),return:a(2)},"function"==typeof Symbol&&(s[Symbol.iterator]=function(){return this}),s;function a(s){return function(a){return function(s){if(i)throw new TypeError("Generator is already executing.");for(;r;)try{if(i=1,n&&(o=2&s[0]?n.return:s[0]?n.throw||((o=n.return)&&o.call(n),0):n.next)&&!(o=o.call(n,s[1])).done)return o;switch(n=0,o&&(s=[2&s[0],o.value]),s[0]){case 0:case 1:o=s;break;case 4:return r.label++,{value:s[1],done:!1};case 5:r.label++,n=s[1],s=[0];continue;case 7:s=r.ops.pop(),r.trys.pop();continue;default:if(!(o=r.trys,(o=o.length>0&&o[o.length-1])||6!==s[0]&&2!==s[0])){r=0;continue}if(3===s[0]&&(!o||s[1]>o[0]&&s[1]<o[3])){r.label=s[1];break}if(6===s[0]&&r.label<o[1]){r.label=o[1],o=s;break}if(o&&r.label<o[2]){r.label=o[2],r.ops.push(s);break}o[2]&&r.ops.pop(),r.trys.pop();continue}s=e.call(t,r)}catch(t){s=[6,t],n=0}finally{i=o=0}if(5&s[0])throw s[1];return{value:s[0]?s[1]:void 0,done:!0}}([s,a])}}}var v={closeWin:function(){window.close()}},f=new Map;function p(t,e){if(!f.has(t)&&void 0!==e)if(l(e,Function))f.set(t,e());else{if(!l(e,Object))throw console.error("初始化异常",e),new Error("初始化异常");f.set(t,e)}return f.has(t)?f.get(t):void 0}window.allComponentMap=f;var m=function(){function t(){n(this,t)}return s(t,[{key:"dispose",value:function(){}}],[{key:"dispose",value:function(t){var e=p(t);e&&(console.debug("dispose: ",t,e),e.dispose(),function(t){f.delete(t)}(t))}}]),t}(),y=1;function w(t){var e=arguments.length>1&&void 0!==arguments[1]?arguments[1]:void 0;return e&&"".concat(e,"::").concat(y++)||t.eventUid||y++}var g=function(){function t(e,i,o,s,r){var a=arguments.length>5&&void 0!==arguments[5]?arguments[5]:void 0;n(this,t),this.element=e,this.id=s,this.delegate=o,this.type=i,this.once=r,this.drop=a}return s(t,[{key:"action",value:function(t){this.once&&(this.off(),this.drop&&this.drop()),this.delegate.apply(this.element,[t])}},{key:"bind",value:function(){}},{key:"off",value:function(){}}]),t}(),k=function(t){a(i,t);var e=h(i);function i(t,o,s,r,a){var l=arguments.length>5&&void 0!==arguments[5]?arguments[5]:void 0;return n(this,i),e.call(this,t,o,s,r,a,l)}return s(i,[{key:"on",value:function(){this.element.addEventListener(this.type,this.action)}},{key:"off",value:function(){this.element.removeEventListener(this.type,this.action)}}]),i}(g),b=function(t){a(i,t);var e=h(i);function i(t,o,s,r){var a,l=arguments.length>4&&void 0!==arguments[4]?arguments[4]:void 0;return n(this,i),(a=e.call(this,t,"resize",o,s,r,l)).resizeObserver=new ResizeObserver((function(t){a.action(t)})),a}return s(i,[{key:"on",value:function(){this.resizeObserver.observe(this.element)}},{key:"off",value:function(){this.resizeObserver.disconnect()}}]),i}(g),x=new Map,W="resize";function z(t){var e=w(t);return t.eventUid=e,x[e]=x[e]||new S(t),x[e]}function E(t,e,i,n){var o=z(t);e==W?o.addResizeHandler(i,n):o.addHandler(i,e,n)}window.allEventsMap=x;var S=function(){function t(e){n(this,t),this.element=e,this.events=new Map}return s(t,[{key:"addHandler",value:function(t,e,i){var n=this,o=w(this.element,e),s=void 0;i&&(s=function(){return n.removeHandler(e,t)});var r=new k(this.element,e,t,o,i,s),a=this.events.get(e)||new Map;a.set(o,r),this.events.set(e,a),r.action=r.action.bind(r),r.on()}},{key:"addResizeHandler",value:function(t,e){var i=this,n=w(this.element,W),o=void 0;e&&(o=function(){return i.removeHandler(W,t)});var s=new b(this.element,t,n,e,o),r=this.events.get(W)||new Map;r.set(n,s),this.events.set(W,r),s.on()}},{key:"removeHandler",value:function(t,e){var i=this.events.get(t);if(null!=i){if(e)for(var n,o=i.values();!(n=o.next()).done;){var s=n.value;if(s.delegate==e){s.off(),i.delete(s.id);break}}else for(var r in i){var a;null===(a=i.get(r))||void 0===a||a.off(),i.delete(r)}var l,c;0==i.size&&this.events.delete(t),0==this.events.size&&(l=this.element,c=w(l),delete x[c])}}}]),t}(),C=function(t,e,i){t&&E(t,e,i,!1)},M=function(t,e,i){t&&E(t,e,i,!0)},B=function(t,e,i){t&&z(t).removeHandler(e,i)},H=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).instance=t.instance,o.type=t.type,o.timeout=t.timeout,o.target=t.target||window.document.documentElement,o}return s(i,[{key:"start",value:function(){1==this.type?(C(this.target,"mousemove",this.debounce.bind(this)),C(this.target,"keydown",this.debounce.bind(this))):2==this.type&&(C(this.target,"mousemove",this.throttle.bind(this)),C(this.target,"keydown",this.throttle.bind(this)))}},{key:"debounce",value:function(){var t=this;clearTimeout(this.timer),this.timer=window.setTimeout((function(){t.invoke()}),this.timeout)}},{key:"throttle",value:function(){var t=this;this.timer||(this.invoke(),this.timer=window.setTimeout((function(){t.timer=void 0}),this.timeout))}},{key:"invoke",value:function(){this.instance.invokeMethodAsync("Call")}},{key:"dispose",value:function(){B(this.target,"mousemove"),B(this.target,"keydown"),window.clearTimeout(this.timer),this.timer=void 0}}],[{key:"init",value:function(t,e){t?p(t,(function(){return new i(e)})).start():console.log("id is not defined")}}]),i}(m);function R(t,e){return{success:!0,message:t,payload:e}}function D(t){return{success:!1,message:t,payload:null}}var O={};function F(t,e,i){t.stopImmediatePropagation(),C(document.documentElement,"mousemove",e),M(document.documentElement,"mouseup",(function(t){i&&i(),B(document.documentElement,"mousemove",e),document.onselectstart!==O.onselectstart&&(document.onselectstart=O.onselectstart)})),O.onselectstart=document.onselectstart,document.onselectstart=function(){return!1}}var P=function(t){a(o,t);var e=h(o);function o(t){var i;return n(this,o),(i=e.call(this)).quality=1,i.deviceId="",i.width=0,i.height=0,i.video=t.video,i.tracks=[],i.quality=t.quality,t.clip&&(i.clipBox=new T(t),i.clipBox.initEvents()),i}return s(o,[{key:"open",value:function(t,e,i){var n=this;return new Promise((function(o){navigator&&navigator.mediaDevices?navigator.mediaDevices.getUserMedia({video:{deviceId:{exact:t},width:{ideal:e},height:{ideal:i}}}).then((function(s){n.width=e,n.height=i,n.deviceId=t;try{n.video.srcObject=s,n.tracks=s.getTracks(),n.video.onloadedmetadata=function(t){n.video.play(),n.clipBox&&n.clipBox.setVisible(!0),o(R("开始播放"))}}catch(t){o(D(t.message))}})).catch((function(t){o(D(t.message))})):o(D("浏览器不支持"))}))}},{key:"capture",value:function(t){try{if(this.video&&this.video.readyState>2){var e="",i=document.createElement("canvas"),n=i.getContext("2d");if(null==n)return D("获取Canvas Context失败");var o=0,s=0,r=this.video.videoWidth,a=this.video.videoHeight;if(this.clipBox){this.clipBox.applyRect();var l=this.video.videoWidth/this.clipBox.videoWindowWidth,c=this.video.videoHeight/this.clipBox.videoWindowHeight;o=this.clipBox.x*l,s=this.clipBox.y*c,r=this.clipBox.w*l,a=this.clipBox.h*c}var u=0,h=0;0==(t%=4)||2==t?(i.width=r,i.height=a,2==t&&(u=r,h=a)):(i.width=a,i.height=r,1==t?u=a:h=r);var d=90*t*Math.PI/180;n.translate(u,h),n.rotate(d),n.drawImage(this.video,o,s,r,a,0,0,r,a),n.rotate(-d),n.translate(-u,-h);var v=i.toDataURL();return v.split(",").length>1&&(e=v.split(",")[1]),R("",e)}return D("视频状态异常")}catch(t){return D(t.message)}}},{key:"close",value:function(){this.tracks.forEach((function(t){return t.stop()})),this.tracks=[],this.video&&(this.video.srcObject=null),this.clipBox&&this.clipBox.setVisible(!1)}},{key:"dispose",value:function(){this.clipBox&&(this.clipBox.dispose(),this.clipBox=void 0),this.close()}}],[{key:"init",value:function(t,e){p(t,(function(){return new o(e)}))}},{key:"useClipBox",value:function(t,e){var i,n=p(t);n&&null==n.clipBox&&(n.clipBox=new T(e),n.clipBox.initEvents()),null===(i=n.clipBox)||void 0===i||i.setVisible(!0)}},{key:"disableClipBox",value:function(t){var e=p(t);e&&e.clipBox&&(e.clipBox.setVisible(!1),e.clipBox.dispose(),e.clipBox=void 0)}},{key:"enumerateDevices",value:function(){return i((function(){return d(this,(function(t){switch(t.label){case 0:return navigator&&navigator.mediaDevices?[4,navigator.mediaDevices.enumerateDevices()]:[3,2];case 1:return[2,R("",t.sent())];case 2:return[2,D("获取设备失败！请检查设备连接或者浏览器配置！")]}}))}))()}},{key:"loadUserMedia",value:function(t,e,n,o){return i((function(){return d(this,(function(i){switch(i.label){case 0:return i.trys.push([0,2,,3]),[4,p(t).open(e,n,o)];case 1:return[2,i.sent()];case 2:return[2,D(i.sent().message)];case 3:return[2]}}))}))()}},{key:"closeUserMedia",value:function(t){try{var e=p(t);return e&&e.close(),R("")}catch(t){return D(t.message)}}},{key:"capture",value:function(t,e){return p(t).capture(e)}}]),o}(m),T=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).el=t.clip,o.w=.8*t.width,o.h=.8*t.height,o.x=.1*t.width,o.y=.1*t.height,o.videoWindowWidth=t.width,o.videoWindowHeight=t.height,o.el.style.width=o.w+"px",o.el.style.height=o.h+"px",o.scaleWidth=10,o.applyRect(),o}return s(i,[{key:"applyRect",value:function(){this.x<0&&(this.x=0),this.y<0&&(this.y=0),this.x>this.videoWindowWidth-this.w&&(this.x=this.videoWindowWidth-this.w),this.y>this.videoWindowHeight-this.h&&(this.y=this.videoWindowHeight-this.h),this.el.offsetWidth>0&&(this.w=this.el.offsetWidth),this.el.offsetHeight>0&&(this.h=this.el.offsetHeight),this.el.style.top=this.y+"px",this.el.style.left=this.x+"px",this.el.style["max-width"]=this.videoWindowWidth-this.x+"px",this.el.style["max-height"]=this.videoWindowHeight-this.y+"px"}},{key:"canMove",value:function(t,e){return this.w=this.el.offsetWidth,this.h=this.el.offsetHeight,t>this.scaleWidth&&t<this.w-this.scaleWidth&&e>this.scaleWidth&&e<this.h-this.scaleWidth}},{key:"setVisible",value:function(t){this.el&&(this.el.style.display=t?"block":"none")}},{key:"handleMouseDown",value:function(t){var e=this;if(t.stopPropagation(),!(t.ctrlKey||[1,2].indexOf(t.button)>-1)){var i;if(window&&window.getSelection())null===(i=window.getSelection())||void 0===i||i.removeAllRanges();var n=t.offsetX,o=t.offsetY;this.canMove(n,o)&&F(t,(function(t){e.x=t.offsetX-n+e.x,e.y=t.offsetY-o+e.y,e.applyRect()}))}}},{key:"initEvents",value:function(){C(this.el,"mousedown",this.handleMouseDown.bind(this))}},{key:"dispose",value:function(){B(this.el,"mousedown")}}]),i}(m),L=function(t){a(i,t);var e=h(i);function i(t){var o;n(this,i),(o=e.call(this)).mask=t.mask,o.childContentContainer=t.container,o.trigger=t.trigger;var s=o.childContentContainer.getBoundingClientRect();return o.contentWidth=s.width,o.childContentContainer.style.left=-o.getWidth()+"px",o.bindEvents(),o}return s(i,[{key:"getWidth",value:function(){return this.contentWidth}},{key:"bindEvents",value:function(){C(this.trigger,"click",this.toggle.bind(this)),C(this.mask,"click",this.toggle.bind(this))}},{key:"toggle",value:function(t){t.stopPropagation(),this.mask.classList.toggle("show"),this.childContentContainer.classList.toggle("show")}},{key:"dispose",value:function(){B(this.trigger,"click"),B(this.mask,"click")}}],[{key:"init",value:function(t,e){p(t,(function(){return new i(e)}))}}]),i}(m),j=function(t){a(o,t);var e=h(o);function o(){return n(this,o),e.call(this)}return s(o,[{key:"request",value:function(t){return i((function(){var e;return d(this,(function(i){switch(i.label){case 0:return e={method:t.method,mode:"cors",cache:"no-cache",credentials:"same-origin",headers:{"Content-Type":"application/json"},redirect:"follow",referrerPolicy:"no-referrer"},"get"!=t.method.toLowerCase()&&"head"!=t.method.toLowerCase()&&(e.body=JSON.stringify(t.body)),[4,fetch(t.url,e)];case 1:return[2,i.sent()]}}))}))()}}],[{key:"init",value:function(t){p(t,(function(){return new o}))}},{key:"request",value:function(t,e){return i((function(){var i,n,o;return d(this,(function(s){switch(s.label){case 0:i=p(t),s.label=1;case 1:return s.trys.push([1,3,,4]),[4,i.request(e)];case 2:return(n=s.sent()).ok?[2,R("",n.json())]:[2,D(n.text())];case 3:return o=s.sent(),[2,D("".concat(o.name,":").concat(o.message))];case 4:return[2]}}))}))()}}]),o}(m),V=function(t){a(i,t);var e=h(i);function i(){var t;return n(this,i),(t=e.call(this)).element=document.documentElement,t.document=document,t}return s(i,[{key:"toggle",value:function(){this.isFullscreen()?this.exit():this.enter()}},{key:"enter",value:function(){this.element.requestFullscreen()||this.element.webkitRequestFullscreen||this.element.mozRequestFullScreen||this.element.msRequestFullscreen}},{key:"exit",value:function(){this.document.exitFullscreen?this.document.exitFullscreen():this.document.mozCancelFullScreen?this.document.mozCancelFullScreen():this.document.webkitExitFullscreen?this.document.webkitExitFullscreen():this.document.msExitFullscreen&&this.document.msExitFullscreen()}},{key:"isFullscreen",value:function(){return this.document.fullscreen||this.document.webkitIsFullScreen||this.document.webkitFullScreen||this.document.mozFullScreen||this.document.msFullScreent}}],[{key:"init",value:function(t){p(t,(function(){return new i}))}},{key:"toggle",value:function(t){p(t).toggle()}}]),i}(m),Y=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).instance=t.dotNetRef,o.interval=t.interval,o}return s(i,[{key:"start",value:function(){var t=this;this.timer=window.setInterval((function(){t.instance.invokeMethodAsync("Call")}),this.interval)}},{key:"dispose",value:function(){this.timer&&(window.clearInterval(this.timer),this.timer=void 0)}}],[{key:"init",value:function(t,e){p(t,(function(){return new i(e)})).start()}}]),i}(m),I=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).always=!1,o.parent=t,o}return s(i,[{key:"update",value:function(){this.vertical&&this.vertical.thumb&&(this.vertical.thumb.style.height=this.parent.sizeHeight),this.horizontal&&this.horizontal.thumb&&(this.horizontal.thumb.style.width=this.parent.sizeWidth)}},{key:"handleScroll",value:function(t){if(this.parent.wrap){var e=this.parent.wrap.offsetHeight-4,i=this.parent.wrap.offsetWidth-4;this.vertical&&this.vertical.setMove(100*this.parent.wrap.scrollTop/e*this.parent.ratioY),this.horizontal&&this.horizontal.setMove(100*this.parent.wrap.scrollLeft/i*this.parent.ratioX)}}},{key:"setVisible",value:function(t){this.always||(this.vertical&&this.vertical.setVisible(t),this.horizontal&&this.horizontal.setVisible(t))}},{key:"dispose",value:function(){this.vertical&&this.vertical.dispose(),this.horizontal&&this.horizontal.dispose()}}]),i}(m),X={vertical:{offset:"offsetHeight",scroll:"scrollTop",scrollSize:"scrollHeight",size:"height",key:"vertical",axis:"Y",client:"clientY",direction:"top",ratio:"ratioY"},horizontal:{offset:"offsetWidth",scroll:"scrollLeft",scrollSize:"scrollWidth",size:"width",key:"horizontal",axis:"X",client:"clientX",direction:"left",ratio:"ratioX"}},q=function(t){a(i,t);var e=h(i);function i(t,o,s,r){var a;return n(this,i),(a=e.call(this)).root=t,a.position=o,a.cursorDown=!1,a.cursorLeave=!1,a.tracker=s,a.thumb=r,a.map=X[o],a.state={X:0,Y:0},a.setVisible(!1),a.initEvents(),a}return s(i,[{key:"mouseMoveDocumentHandler",value:function(t){if(this.tracker&&this.thumb&&!1!==this.cursorDown){var e=this.state[this.map.axis];if(e){var i=100*(-1*(this.tracker.getBoundingClientRect()[this.map.direction]-t[this.map.client])-(this.thumb[this.map.offset]-e))*this.root[this.map.ratio]/this.tracker[this.map.offset];this.root.wrap[this.map.scroll]=i*this.root.wrap[this.map.scrollSize]/100,this.updateMove()}}}},{key:"clickTrackerHandler",value:function(t){if(this.thumb&&this.tracker&&this.root.wrap){var e=100*(Math.abs(t.target.getBoundingClientRect()[this.map.direction]-t[this.map.client])-this.thumb[this.map.offset]/2)*this.root[this.map.ratio]/this.tracker[this.map.offset];this.root.wrap[this.map.scroll]=e*this.root.wrap[this.map.scrollSize]/100}}},{key:"clickThumbHandler",value:function(t){var e=this;if(t.stopPropagation(),!(t.ctrlKey||[1,2].indexOf(t.button)>-1)){var i;if(window.getSelection())null===(i=window.getSelection())||void 0===i||i.removeAllRanges();this.cursorDown=!0,F(t,this.mouseMoveDocumentHandler.bind(this),(function(){e.cursorDown=!1,e.state[e.map.axis]=0}));var n=t.currentTarget;n&&(this.state[this.map.axis]=n[this.map.offset]-(t[this.map.client]-n.getBoundingClientRect()[this.map.direction]))}}},{key:"mouseUpDocumentHandler",value:function(t){this.cursorDown=!1,this.state[this.map.axis]=0}},{key:"setMove",value:function(t){this.thumb.style.transform="translate".concat(this.map.axis,"(").concat(t,"%)")}},{key:"updateMove",value:function(){var t=this.root.wrap[this.map.offset]-4;this.setMove(100*this.root.wrap[this.map.scroll]/t*this.root[this.map.ratio])}},{key:"setVisible",value:function(t){this.tracker&&(this.tracker.style.display=t?"block":"none")}},{key:"initEvents",value:function(){C(this.tracker,"mousedown",this.clickTrackerHandler.bind(this)),C(this.thumb,"mousedown",this.clickThumbHandler.bind(this))}},{key:"dispose",value:function(){B(this.tracker,"mousedown"),B(this.thumb,"mousedown")}}]),i}(m),N=function(e){a(o,e);var i=h(o);function o(e){var s;n(this,o),(s=i.call(this)).ratioX=1,s.ratioY=1,s.sizeHeight="",s.sizeWidth="",s.root=e.scrollbar,s.wrap=e.wrap,s.resize=e.resize,s.minSize=e.minSize,s.bar=new I(t(s));var r=!0,a=!1,l=void 0;try{for(var c,u=e.bars[Symbol.iterator]();!(r=(c=u.next()).done);r=!0){var h=c.value,d=h.position,v=h.tracker,f=h.thumb,p=new q(t(s),d,v,f);s.bar[d]=p}}catch(t){a=!0,l=t}finally{try{r||null==u.return||u.return()}finally{if(a)throw l}}return e.always&&(s.bar.setVisible(!0),s.bar.always=e.always),s.initEvents(),s}return s(o,[{key:"update",value:function(){if(this.wrap){var t=this.wrap.offsetHeight-4,e=this.wrap.offsetWidth-4,i=t*t/this.wrap.scrollHeight,n=e*e/this.wrap.scrollWidth,o=Math.max(i,this.minSize),s=Math.max(n,this.minSize);this.ratioY=i/(t-i)/(o/(t-o)),this.ratioX=n/(e-n)/(s/(e-s)),this.sizeHeight=o+4<t?o+"px":"",this.sizeWidth=s+4<e?s+"px":"",this.bar.update()}}},{key:"handleMouseMove",value:function(t){this.bar.setVisible(!0)}},{key:"handleMouseLeave",value:function(t){this.bar.setVisible(!1)}},{key:"handleScroll",value:function(t){this.bar.handleScroll(t)}},{key:"initEvents",value:function(){var t=this;C(this.root,"mousemove",this.handleMouseMove.bind(this)),C(this.root,"mouseleave",this.handleMouseLeave.bind(this)),C(this.wrap,"scroll",this.handleScroll.bind(this)),C(this.wrap,"resize",(function(e){t.update()})),C(this.resize,"resize",(function(){t.update()}))}},{key:"dispose",value:function(){this.bar.dispose(),B(this.root,"mousemove"),B(this.root,"mouseleave"),B(this.wrap,"scroll"),B(this.wrap,"resize"),B(this.resize,"resize")}}],[{key:"init",value:function(t,e){p(t,(function(){return new o(e)}))}}]),o}(m),U=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).wheel="",o.scroll=function(t){o.wrap.clientWidth>=o.wrap.scrollWidth||(o.wrap.scrollLeft+=t.deltaY?t.deltaY:t.detail&&0!==t.detail?t.detail:-t.wheelDelta)},o.wrap=t,o.initEvents(),o}return s(i,[{key:"handleWheelEvent",value:function(){var t="";t="onmousewheel"in this.wrap?"mousewheel":"onwheel"in this.wrap?"wheel":"attachEvent"in window?"onmousewheel":"DOMMouseScroll",this.wheel=t,C(this.wrap,t,this.scroll)}},{key:"initEvents",value:function(){this.handleWheelEvent()}},{key:"dispose",value:function(){B(this.wrap,this.wheel,this.scroll)}}],[{key:"init",value:function(t,e){p(t,(function(){return new i(e)}))}}]),i}(m),_=function(t){a(i,t);var e=h(i);function i(t,o){var s;return n(this,i),(s=e.call(this)).drag=!1,s.panel1=t.panel1,s.panel2=t.panel2,s.separator=t.separator,s.direction=o.direction,s.max=o.max,s.min=o.min,s.init=o.initWidth,s.setup(),s}return s(i,[{key:"setup",value:function(){C(this.separator,"mousedown",this.handleMouseDown.bind(this)),this.panel1.parentElement&&C(this.panel1.parentElement,"resize",this.refresh.bind(this))}},{key:"refresh",value:function(){this.drag||(this.panel1.style.width=this.init)}},{key:"handleMouseDown",value:function(t){var e,i=this;t.stopPropagation();var n=null===(e=this.panel1.parentElement)||void 0===e?void 0:e.getBoundingClientRect(),o=this.separator.getBoundingClientRect(),s="row"===this.direction?t.pageX-o.left:t.pageY-o.top,r="row"===this.direction?this.modeRow:this.modeColumn;this.drag=!0,F(t,(function(t){r.call(i,t,n,o,s)}),(function(){i.drag=!1}))}},{key:"modeRow",value:function(t,e,i,n){var o=this.panel1.getBoundingClientRect(),s=A(t.pageX-o.left-n+i.width/2,e.width,this.max,this.min).toFixed(2);this.panel1.style.width="calc(".concat(s,"% - ").concat(i.width/2,"px)")}},{key:"modeColumn",value:function(t,e,i,n){var o=this.panel1.getBoundingClientRect(),s=A(t.pageY-o.top-n+i.height/2,e.height,this.max,this.min).toFixed(2);this.panel1.style.height="calc(".concat(s,"% - ").concat(i.height/2,"px)")}},{key:"dispose",value:function(){B(this.separator,"mousedown"),this.panel1.parentElement&&B(this.panel1.parentElement,"resize")}}],[{key:"init",value:function(t,e,n){p(t,(function(){return new i(e,n)}))}}]),i}(m);function A(t,e,i,n){var o=t/e*100;if(i.endsWith("%")){var s=Number(i.replace("%",""));o>s&&(o=s)}else if(i.endsWith("px")){var r=Number(i.replace("px",""));t>r&&(o=r/e*100)}if(n.endsWith("%")){var a=Number(n.replace("%",""));o<a&&(o=a)}else if(n.endsWith("px")){var l=Number(n.replace("px",""));t<l&&(o=l/e*100)}return o}var J=function(t){a(i,t);var e=h(i);function i(t){var o;return n(this,i),(o=e.call(this)).wrapper=t||window.document.documentElement,o.ob=new MutationObserver((function(t){var e=!0,i=!1,n=void 0;try{for(var s,r=function(){var t=s.value;"childList"===t.type&&t.removedNodes.forEach((function(e){e===o.mask&&o.setWatermark(o.options),t.target===o.mask&&o.setWatermark(o.options)}))},a=t[Symbol.iterator]();!(e=(s=a.next()).done);e=!0)r()}catch(t){i=!0,n=t}finally{try{e||null==a.return||a.return()}finally{if(i)throw n}}})),o.ob.observe(o.wrapper,{attributes:!0,childList:!0,characterData:!0,subtree:!0}),o}return s(i,[{key:"setWatermark",value:function(t){var e;this.mask&&this.mask.remove(),this.options=t,this.mask=((e=document.createElement("div")).style.position="absolute",e.style.left="0",e.style.top="0",e.style.width="100%",e.style.height="100%",e.style.pointerEvents="none",e.style.backgroundRepeat="repeat",e);var i=function(t){var e=t.top,i=t.width,n=t.height,o=t.gapX,s=t.gapY,r=t.rotate,a=t.alpha,l=t.lineSpace,c=t.fontSize,u=t.fontColor,h=t.contents,d=document.createElement("canvas"),v=d.getContext("2d");if(!v)return void console.warn("Current environment does not support Canvas, cannot draw watermarks.");var f=window.devicePixelRatio||1,p=(o+i)*f,m=(s+n)*f;d.width=p,d.height=m,d.style.width="".concat(o+i,"px"),d.style.height="".concat(s+n,"px"),v.rotate(Math.PI/180*Number(r)),v.globalAlpha=a;var y=i*f,w=n*f;v.fillStyle="transparent",v.fillRect(0,0,y,w);for(var g="sans-serif",k="normal",b="start",x="normal",W=Number(c)*f,z=0;z<h.length;z++){var E=h[z];e+=l,v.font="".concat(x," normal ").concat(k," ").concat(W,"px/").concat(w,"px ").concat(g),v.textAlign=b,v.textBaseline="top",v.fillStyle=u,v.fillText(E,0,e*f)}return d.toDataURL()}(t);this.mask.style.backgroundSize="".concat(t.gapX+t.width,"px"),this.mask.style.backgroundImage="url(".concat(i,")"),this.wrapper.appendChild(this.mask)}},{key:"dispose",value:function(){this.ob.disconnect()}}],[{key:"setWatermark",value:function(t,e,n){p(t,(function(){return new i(e)})).setWatermark(n)}}]),i}(m);var K=function(){function t(){n(this,t)}return s(t,null,[{key:"getMenuWidth",value:function(){var t=window.document.querySelector(".nav-menu");return null==t?void 0:t.offsetWidth}}]),t}(),G=function(){function t(){n(this,t)}return s(t,null,[{key:"download",value:function(t,e){var i=document.createElement("form");i.action="/api/download",i.method="post",i.style.display="none";for(var n=Object.keys(e),o=0;o<n.length;o++){var s=document.createElement("input"),r=n[o];s.hidden=!0,s.name=r,s.value=e[r],i.appendChild(s)}var a=document.createElement("input");a.type="submit",i.appendChild(a),document.body.appendChild(i),i.submit(),document.body.removeChild(i)}}]),t}();window.Utils=v,window.BlazorProject={ActionWatcher:H,Camera:P,EdgeWidget:L,Fetch:j,FullScreen:V,JsTimer:Y,ScrollBar:N,HorizontalScroll:U,SplitView:_,WaterMark:J,NavTabs:K,Downloader:G};//# sourceMappingURL=blazor-admin-project.js.map