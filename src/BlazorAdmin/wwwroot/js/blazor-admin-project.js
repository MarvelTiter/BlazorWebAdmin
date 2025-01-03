"use strict";var t,e=(t,e,i)=>new Promise(((s,n)=>{var o=t=>{try{r(i.next(t))}catch(t){n(t)}},h=t=>{try{r(i.throw(t))}catch(t){n(t)}},r=t=>t.done?s(t.value):Promise.resolve(t.value).then(o,h);r((i=i.apply(t,e)).next())})),i=window.matchMedia("(prefers-color-scheme: dark)");function s(t,e){document.documentElement.dataset.theme=t,"dark"===t?function(t){const e=document.querySelector("link[data-dark]");e&&(e.href=t)}(e):function(t){var e;for(const i of document.querySelectorAll("link"))if(null==(e=i.getAttribute("href"))?void 0:e.match(t)){i.removeAttribute("href");break}}(e)}var n={closeWin:function(){window.close()},openWindow:function(t,e,i,s){const n=window.outerWidth,o=window.outerHeight;let h=n,r=o;e<=1&&(h=n*e),i<=1&&(r=o*i);const a=(o-r)/2+window.screenTop,l=`popup=yes, left=${(n-h)/2+window.screenLeft}, top=${a}, width=${h}, height=${r}`;console.log(l),window.open(t,s,l)},getClient:function(){return e(this,null,(function*(){if(null!==window.opener)return[null,null];const t=yield fetch("/ip.client");return[yield t.text(),navigator.userAgent]}))},setTheme:function(e,n){"os"===e?(t=function(t){const e=t;return function(){i.matches?s("dark",e):s("light",e)}}(n),i.addEventListener("change",t),t()):(t&&(i.addEventListener("change",t),t=void 0),s(e,n))}},o=new Map;function h(t,e){if(!o.has(t)&&void 0!==e)if(e instanceof Function)o.set(t,e());else{if(!e)throw console.error("初始化异常",e),new Error("初始化异常");o.set(t,e)}return o.has(t)?o.get(t):void 0}window.allComponentMap=o;var r=class{dispose(){}static dispose(t){const e=h(t);e&&(console.debug("dispose: ",t,e),e.dispose(),function(t){o.delete(t)}(t))}},a=1;function l(t,e=void 0){return e&&`${e}::${a++}`||t.eventUid||a++}var c=class{constructor(t,e,i,s,n,o=void 0){this.element=t,this.id=s,this.delegate=i,this.type=e,this.once=n,this.drop=o}action(t){this.once&&(this.off(),this.drop&&this.drop()),this.delegate.apply(this.element,[t])}bind(){}off(){}},d=class extends c{constructor(t,e,i,s,n,o=void 0){super(t,e,i,s,n,o)}on(){this.element.addEventListener(this.type,this.action)}off(){this.element.removeEventListener(this.type,this.action)}},u=class extends c{constructor(t,e,i,s,n=void 0){super(t,"resize",e,i,s,n),this.resizeObserver=new ResizeObserver((t=>{this.action(t)}))}on(){this.element instanceof Element&&this.resizeObserver.observe(this.element)}off(){this.resizeObserver.disconnect()}},m=new Map,p="resize";function w(t){const e=l(t);return t.eventUid=e,m[e]=m[e]||new f(t),m[e]}function v(t,e,i,s){const n=w(t);e==p?n.addResizeHandler(i,s):n.addHandler(i,e,s)}window.allEventsMap=m;var f=class{constructor(t){this.element=t,this.events=new Map}addHandler(t,e,i){const s=l(this.element,e);let n;i&&(n=()=>this.removeHandler(e,t));var o=new d(this.element,e,t,s,i,n);const h=this.events.get(e)||new Map;h.set(s,o),this.events.set(e,h),o.action=o.action.bind(o),o.on()}addResizeHandler(t,e){const i=l(this.element,p);let s;e&&(s=()=>this.removeHandler(p,t));var n=new u(this.element,t,i,e,s);const o=this.events.get(p)||new Map;o.set(i,n),this.events.set(p,o),n.on()}removeHandler(t,e){var i;const s=this.events.get(t);if(null!=s){if(e){const t=s.values();let i;for(;i=t.next(),!i.done;){const t=i.value;if(t.delegate==e){t.off(),s.delete(t.id);break}}}else for(var n in s)null==(i=s.get(n))||i.off(),s.delete(n);0==s.size&&this.events.delete(t),0==this.events.size&&function(t){const e=l(t);delete m[e]}(this.element)}}},g=function(t,e,i){t&&v(t,e,i,!1)},x=function(t,e,i){t&&v(t,e,i,!0)},y=function(t,e,i){if(!t)return;w(t).removeHandler(e,i)};function b(t,e){return{isSuccess:!0,message:t,payload:e}}function k(t){return{isSuccess:!1,message:t,payload:null}}var W={};function C(t,e,i){t.stopImmediatePropagation(),g(document.documentElement,"mousemove",e),x(document.documentElement,"mouseup",(t=>{i&&i(),y(document.documentElement,"mousemove",e),document.onselectstart!==W.onselectstart&&(document.onselectstart=W.onselectstart)})),W.onselectstart=document.onselectstart,document.onselectstart=()=>!1}var z=class extends r{constructor(t){super(),this.el=t.clip,this.w=.8*t.width,this.h=.8*t.height,this.x=.1*t.width,this.y=.1*t.height,this.videoWindowWidth=t.width,this.videoWindowHeight=t.height,this.el.style.width=this.w+"px",this.el.style.height=this.h+"px",this.scaleWidth=10,this.applyRect()}applyRect(){this.x<0&&(this.x=0),this.y<0&&(this.y=0),this.x>this.videoWindowWidth-this.w&&(this.x=this.videoWindowWidth-this.w),this.y>this.videoWindowHeight-this.h&&(this.y=this.videoWindowHeight-this.h),this.el.offsetWidth>0&&(this.w=this.el.offsetWidth),this.el.offsetHeight>0&&(this.h=this.el.offsetHeight),this.el.style.top=this.y+"px",this.el.style.left=this.x+"px",this.el.style["max-width"]=this.videoWindowWidth-this.x+"px",this.el.style["max-height"]=this.videoWindowHeight-this.y+"px"}canMove(t,e){return this.w=this.el.offsetWidth,this.h=this.el.offsetHeight,t>this.scaleWidth&&t<this.w-this.scaleWidth&&e>this.scaleWidth&&e<this.h-this.scaleWidth}setVisible(t){this.el&&(this.el.style.display=t?"block":"none")}handleMouseDown(t){var e;if(t.stopPropagation(),!(t.ctrlKey||[1,2].indexOf(t.button)>-1)){window&&window.getSelection()&&(null==(e=window.getSelection())||e.removeAllRanges());var i=t.offsetX,s=t.offsetY;this.canMove(i,s)&&C(t,(t=>{this.x=t.offsetX-i+this.x,this.y=t.offsetY-s+this.y,this.applyRect()}))}}initEvents(){g(this.el,"mousedown",this.handleMouseDown.bind(this))}dispose(){y(this.el,"mousedown")}},S=class extends r{constructor(t){super(),this.always=!1,this.parent=t}update(){this.vertical&&this.vertical.thumb&&(this.vertical.thumb.style.height=this.parent.sizeHeight),this.horizontal&&this.horizontal.thumb&&(this.horizontal.thumb.style.width=this.parent.sizeWidth)}handleScroll(t){if(this.parent.wrap){const t=this.parent.wrap.offsetHeight-4,e=this.parent.wrap.offsetWidth-4;this.vertical&&this.vertical.setMove(100*this.parent.wrap.scrollTop/t*this.parent.ratioY),this.horizontal&&this.horizontal.setMove(100*this.parent.wrap.scrollLeft/e*this.parent.ratioX)}}setVisible(t){this.always||(this.vertical&&this.vertical.setVisible(t),this.horizontal&&this.horizontal.setVisible(t))}dispose(){this.vertical&&this.vertical.dispose(),this.horizontal&&this.horizontal.dispose()}},E={vertical:{offset:"offsetHeight",scroll:"scrollTop",scrollSize:"scrollHeight",size:"height",key:"vertical",axis:"Y",client:"clientY",direction:"top",ratio:"ratioY"},horizontal:{offset:"offsetWidth",scroll:"scrollLeft",scrollSize:"scrollWidth",size:"width",key:"horizontal",axis:"X",client:"clientX",direction:"left",ratio:"ratioX"}},M=class extends r{constructor(t,e,i,s){super(),this.root=t,this.position=e,this.cursorDown=!1,this.cursorLeave=!1,this.tracker=i,this.thumb=s,this.map=E[e],this.state={X:0,Y:0},this.setVisible(!1),this.initEvents()}mouseMoveDocumentHandler(t){if(!this.tracker||!this.thumb)return;if(!1===this.cursorDown)return;const e=this.state[this.map.axis];if(!e)return;const i=100*(-1*(this.tracker.getBoundingClientRect()[this.map.direction]-t[this.map.client])-(this.thumb[this.map.offset]-e))*this.root[this.map.ratio]/this.tracker[this.map.offset];this.root.wrap[this.map.scroll]=i*this.root.wrap[this.map.scrollSize]/100,this.updateMove()}clickTrackerHandler(t){if(!this.thumb||!this.tracker||!this.root.wrap)return;const e=100*(Math.abs(t.target.getBoundingClientRect()[this.map.direction]-t[this.map.client])-this.thumb[this.map.offset]/2)*this.root[this.map.ratio]/this.tracker[this.map.offset];this.root.wrap[this.map.scroll]=e*this.root.wrap[this.map.scrollSize]/100}clickThumbHandler(t){var e;if(t.stopPropagation(),t.ctrlKey||[1,2].indexOf(t.button)>-1)return;window.getSelection()&&(null==(e=window.getSelection())||e.removeAllRanges()),this.cursorDown=!0,C(t,this.mouseMoveDocumentHandler.bind(this),(()=>{this.cursorDown=!1,this.state[this.map.axis]=0}));const i=t.currentTarget;i&&(this.state[this.map.axis]=i[this.map.offset]-(t[this.map.client]-i.getBoundingClientRect()[this.map.direction]))}mouseUpDocumentHandler(t){this.cursorDown=!1,this.state[this.map.axis]=0}setMove(t){this.thumb.style.transform=`translate${this.map.axis}(${t}%)`}updateMove(){const t=this.root.wrap[this.map.offset]-4;this.setMove(100*this.root.wrap[this.map.scroll]/t*this.root[this.map.ratio])}setVisible(t){this.tracker&&(this.tracker.style.display=t?"block":"none")}initEvents(){g(this.tracker,"mousedown",this.clickTrackerHandler.bind(this)),g(this.thumb,"mousedown",this.clickThumbHandler.bind(this))}dispose(){y(this.tracker,"mousedown"),y(this.thumb,"mousedown")}};function B(t,e,i,s){let n=t/e*100;if(i.endsWith("%")){const t=Number(i.replace("%",""));n>t&&(n=t)}else if(i.endsWith("px")){const s=Number(i.replace("px",""));t>s&&(n=s/e*100)}if(s.endsWith("%")){const t=Number(s.replace("%",""));n<t&&(n=t)}else if(s.endsWith("px")){const i=Number(s.replace("px",""));t<i&&(n=i/e*100)}return n}var H=class{static download(t,e){const i=document.createElement("form");i.action="/api/download",i.method="post",i.style.display="none";const s=Object.keys(e);for(let t=0;t<s.length;t++){const n=document.createElement("input"),o=s[t];n.hidden=!0,n.name=o,n.value=e[o],i.appendChild(n)}const n=document.createElement("input");n.type="submit",i.appendChild(n),document.body.appendChild(i),i.submit(),document.body.removeChild(i)}};H.downloadStream=(t,i)=>e(H,null,(function*(){try{const{filename:t,streamRef:e}=i,s=yield e.arrayBuffer(),n=new Blob([s]),o=URL.createObjectURL(n),h=document.createElement("a");h.href=o,h.download=null!=t?t:"",h.click(),h.remove(),URL.revokeObjectURL(o)}catch(t){console.error(t)}}));var R=H;window.Utils=n,window.BlazorProject={ActionWatcher:class t extends r{constructor(t){super(),this.instance=t.instance,this.type=t.type,this.timeout=t.timeout,this.target=t.target||window.document.documentElement}start(){1==this.type?(g(this.target,"mousemove",this.debounce.bind(this)),g(this.target,"keydown",this.debounce.bind(this))):2==this.type&&(g(this.target,"mousemove",this.throttle.bind(this)),g(this.target,"keydown",this.throttle.bind(this)))}debounce(){clearTimeout(this.timer),this.timer=window.setTimeout((()=>{this.invoke()}),this.timeout)}throttle(){this.timer||(this.invoke(),this.timer=window.setTimeout((()=>{this.timer=void 0}),this.timeout))}invoke(){this.instance.invokeMethodAsync("Call")}dispose(){y(this.target,"mousemove"),y(this.target,"keydown"),window.clearTimeout(this.timer),this.timer=void 0}static init(e,i){e?h(e,(()=>new t(i))).start():console.log("id is not defined")}},Camera:class t extends r{constructor(t){super(),this.deviceId="",this.width=0,this.height=0,this.video=t.video,this.tracks=[],this.quality=t.quality,this.format=t.format,t.clip&&(this.clipBox=new z(t),this.clipBox.initEvents())}open(t,e,i){return new Promise((s=>{navigator&&navigator.mediaDevices?navigator.mediaDevices.getUserMedia({video:{deviceId:{exact:t},width:{ideal:e},height:{ideal:i}}}).then((n=>{this.width=e,this.height=i,this.deviceId=t;try{this.video.srcObject=n,this.tracks=n.getTracks(),this.video.onloadedmetadata=t=>{this.video.play(),this.clipBox&&this.clipBox.setVisible(!0),s(b("开始播放"))}}catch(t){s(k(t.message))}})).catch((function(t){s(k(t.message))})):s(k("浏览器不支持"))}))}capture(t){try{if(this.video&&this.video.readyState>2){var e="",i=document.createElement("canvas"),s=i.getContext("2d");if(null==s)return k("获取Canvas Context失败");var n=0,o=0,h=this.video.videoWidth,r=this.video.videoHeight;if(this.clipBox){this.clipBox.applyRect();var a=this.video.videoWidth/this.clipBox.videoWindowWidth,l=this.video.videoHeight/this.clipBox.videoWindowHeight;n=this.clipBox.x*a,o=this.clipBox.y*l,h=this.clipBox.w*a,r=this.clipBox.h*l}let d=0,u=0;0==(t%=4)||2==t?(i.width=h,i.height=r,2==t&&(d=h,u=r)):(i.width=r,i.height=h,1==t?d=r:u=h);let m=90*t*Math.PI/180;s.translate(d,u),s.rotate(m),s.drawImage(this.video,n,o,h,r,0,0,h,r),s.rotate(-m),s.translate(-d,-u);var c=i.toDataURL(this.format,this.quality);return c.split(",").length>1&&(e=c.split(",")[1]),b("",e)}return k("视频状态异常")}catch(t){return k(t.message)}}close(){this.tracks.forEach((t=>t.stop())),this.tracks=[],this.video&&(this.video.srcObject=null),this.clipBox&&this.clipBox.setVisible(!1)}dispose(){this.clipBox&&(this.clipBox.dispose(),this.clipBox=void 0),this.close()}static init(e,i){h(e,(()=>new t(i)))}static useClipBox(t,e){var i,s=h(t);s&&null==s.clipBox&&(s.clipBox=new z(e),s.clipBox.initEvents()),null==(i=s.clipBox)||i.setVisible(!0)}static disableClipBox(t){var e=h(t);e&&e.clipBox&&(e.clipBox.setVisible(!1),e.clipBox.dispose(),e.clipBox=void 0)}static enumerateDevices(){return e(this,null,(function*(){return navigator&&navigator.mediaDevices?b("",yield navigator.mediaDevices.enumerateDevices()):k("获取设备失败！请检查设备连接或者浏览器配置！")}))}static loadUserMedia(t,i,s,n){return e(this,null,(function*(){try{const e=h(t);return yield e.open(i,s,n)}catch(t){return k(t.message)}}))}static closeUserMedia(t){try{const e=h(t);return e&&e.close(),b("")}catch(t){return k(t.message)}}static capture(t,e){return h(t).capture(e)}},EdgeWidget:class t extends r{constructor(t){super(),this.mask=t.mask,this.childContentContainer=t.container,this.trigger=t.trigger;var e=this.childContentContainer.getBoundingClientRect();this.contentWidth=e.width,this.childContentContainer.style.left=-this.getWidth()+"px",this.bindEvents()}getWidth(){return this.contentWidth}bindEvents(){g(this.trigger,"click",this.toggle.bind(this)),g(this.mask,"click",this.toggle.bind(this))}toggle(t){t.stopPropagation(),this.mask.classList.toggle("show"),this.childContentContainer.classList.toggle("show")}dispose(){y(this.trigger,"click"),y(this.mask,"click")}static init(e,i){h(e,(()=>new t(i)))}},Fetch:class t extends r{constructor(){super()}request(t){return e(this,null,(function*(){var e={method:t.method,mode:"cors",cache:"no-cache",credentials:"same-origin",headers:{"Content-Type":"application/json"},redirect:"follow",referrerPolicy:"no-referrer"};"get"!=t.method.toLowerCase()&&"head"!=t.method.toLowerCase()&&(e.body=JSON.stringify(t.body));return yield fetch(t.url,e)}))}static init(e){h(e,(()=>new t))}static request(t,i){return e(this,null,(function*(){var e=h(t);try{var s=yield e.request(i);return s.ok?b("",s.json()):k(s.text())}catch(t){return k(`${t.name}:${t.message}`)}}))}},FullScreen:class t extends r{constructor(){super(),this.element=document.documentElement,this.document=document}toggle(){this.isFullscreen()?this.exit():this.enter()}enter(){this.element.requestFullscreen()||this.element.webkitRequestFullscreen||this.element.mozRequestFullScreen||this.element.msRequestFullscreen}exit(){this.document.exitFullscreen?this.document.exitFullscreen():this.document.mozCancelFullScreen?this.document.mozCancelFullScreen():this.document.webkitExitFullscreen?this.document.webkitExitFullscreen():this.document.msExitFullscreen&&this.document.msExitFullscreen()}isFullscreen(){return this.document.fullscreen||this.document.webkitIsFullScreen||this.document.webkitFullScreen||this.document.mozFullScreen||this.document.msFullScreent}static init(e){h(e,(()=>new t))}static toggle(t){h(t).toggle()}},JsTimer:class t extends r{constructor(t){super(),this.instance=t.dotNetRef,this.interval=t.interval}start(){this.timer=window.setInterval((()=>{this.instance.invokeMethodAsync("Call")}),this.interval)}dispose(){this.timer&&(window.clearInterval(this.timer),this.timer=void 0)}static init(e,i){h(e,(()=>new t(i))).start()}},ScrollBar:class t extends r{constructor(t){super(),this.ratioX=1,this.ratioY=1,this.sizeHeight="",this.sizeWidth="",this.root=t.scrollbar,this.wrap=t.wrap,this.resize=t.resize,this.minSize=t.minSize,this.bar=new S(this);for(const e of t.bars){const{position:t,tracker:i,thumb:s}=e,n=new M(this,t,i,s);this.bar[t]=n}t.always&&(this.bar.setVisible(!0),this.bar.always=t.always),this.initEvents()}static init(e,i){h(e,(()=>new t(i)))}update(){if(!this.wrap)return;const t=this.wrap.offsetHeight-4,e=this.wrap.offsetWidth-4,i=t*t/this.wrap.scrollHeight,s=e*e/this.wrap.scrollWidth,n=Math.max(i,this.minSize),o=Math.max(s,this.minSize);this.ratioY=i/(t-i)/(n/(t-n)),this.ratioX=s/(e-s)/(o/(e-o)),this.sizeHeight=n+4<t?n+"px":"",this.sizeWidth=o+4<e?o+"px":"",this.bar.update()}handleMouseMove(t){this.bar.setVisible(!0)}handleMouseLeave(t){this.bar.setVisible(!1)}handleScroll(t){this.bar.handleScroll(t)}initEvents(){g(this.root,"mousemove",this.handleMouseMove.bind(this)),g(this.root,"mouseleave",this.handleMouseLeave.bind(this)),g(this.wrap,"scroll",this.handleScroll.bind(this)),g(this.wrap,"resize",(t=>{this.update()})),g(this.resize,"resize",(()=>{this.update()}))}dispose(){this.bar.dispose(),y(this.root,"mousemove"),y(this.root,"mouseleave"),y(this.wrap,"scroll"),y(this.wrap,"resize"),y(this.resize,"resize")}},HorizontalScroll:class t extends r{constructor(t){super(),this.wheel="",this.scroll=t=>{this.wrap.clientWidth>=this.wrap.scrollWidth||(this.wrap.scrollLeft+=t.deltaY?t.deltaY:t.detail&&0!==t.detail?t.detail:-t.wheelDelta)},this.wrap=t,this.initEvents()}handleWheelEvent(){let t="";t="onmousewheel"in this.wrap?"mousewheel":"onwheel"in this.wrap?"wheel":"attachEvent"in window?"onmousewheel":"DOMMouseScroll",this.wheel=t,g(this.wrap,t,this.scroll)}initEvents(){this.handleWheelEvent()}dispose(){y(this.wrap,this.wheel,this.scroll)}static init(e,i){h(e,(()=>new t(i)))}},SplitView:class t extends r{constructor(t,e){super(),this.drag=!1,this.panel1=t.panel1,this.panel2=t.panel2,this.separator=t.separator,this.direction=e.direction,this.max=e.max,this.min=e.min,this.init=e.initWidth,this.setup()}setup(){g(this.separator,"mousedown",this.handleMouseDown.bind(this)),this.panel1.parentElement&&g(this.panel1.parentElement,"resize",this.refresh.bind(this))}refresh(){this.drag||(this.panel1.style.width=this.init)}handleMouseDown(t){var e;t.stopPropagation();const i=null==(e=this.panel1.parentElement)?void 0:e.getBoundingClientRect(),s=this.separator.getBoundingClientRect(),n="row"===this.direction?t.pageX-s.left:t.pageY-s.top,o="row"===this.direction?this.modeRow:this.modeColumn;this.drag=!0,C(t,(t=>{o.call(this,t,i,s,n)}),(()=>{this.drag=!1}))}modeRow(t,e,i,s){const n=this.panel1.getBoundingClientRect(),o=B(t.pageX-n.left-s+i.width/2,e.width,this.max,this.min).toFixed(2);this.panel1.style.width=`calc(${o}% - ${i.width/2}px)`}modeColumn(t,e,i,s){const n=this.panel1.getBoundingClientRect(),o=B(t.pageY-n.top-s+i.height/2,e.height,this.max,this.min).toFixed(2);this.panel1.style.height=`calc(${o}% - ${i.height/2}px)`}dispose(){y(this.separator,"mousedown"),this.panel1.parentElement&&y(this.panel1.parentElement,"resize")}static init(e,i,s){h(e,(()=>new t(i,s)))}},WaterMark:class t extends r{constructor(t){super(),this.wrapper=t||window.document.documentElement,this.ob=new MutationObserver((t=>{for(const e of t)if("childList"===e.type){e.removedNodes.forEach((t=>{t===this.mask&&this.setWatermark(this.options),e.target===this.mask&&this.setWatermark(this.options)}))}})),this.ob.observe(this.wrapper,{attributes:!0,childList:!0,characterData:!0,subtree:!0})}setWatermark(t){var e;this.mask&&this.mask.remove(),this.options=t,this.mask=((e=document.createElement("div")).style.position="absolute",e.style.left="0",e.style.top="0",e.style.width="100%",e.style.height="100%",e.style.pointerEvents="none",e.style.backgroundRepeat="repeat",e);let i=function({top:t,width:e,height:i,gapX:s,gapY:n,rotate:o,alpha:h,lineSpace:r,fontSize:a,fontColor:l,contents:c}){const d=document.createElement("canvas"),u=d.getContext("2d");if(!u)return void console.warn("Current environment does not support Canvas, cannot draw watermarks.");const m=window.devicePixelRatio||1,p=(s+e)*m,w=(n+i)*m;d.width=p,d.height=w,d.style.width=`${s+e}px`,d.style.height=`${n+i}px`,u.rotate(Math.PI/180*Number(o)),u.globalAlpha=h;const v=e*m,f=i*m;u.fillStyle="transparent",u.fillRect(0,0,v,f);let g="sans-serif",x="normal",y="start",b="normal";const k=function(t){if(t)return t;const e=getComputedStyle(document.documentElement);return e.getPropertyValue("--watermark-color")}(l),W=Number(a)*m;for(var C=0;C<c.length;C++){let e=c[C];t+=r,u.font=`${b} normal ${x} ${W}px/${f}px ${g}`,u.textAlign=y,u.textBaseline="top",u.fillStyle=k,u.fillText(e,0,t*m)}return d.toDataURL()}(t);this.mask.style.backgroundSize=`${t.gapX+t.width}px`,this.mask.style.backgroundImage=`url(${i})`,this.wrapper.appendChild(this.mask)}dispose(){this.ob.disconnect()}static setWatermark(e,i,s){h(e,(()=>new t(i))).setWatermark(s)}static refreshWatermark(t){const e=h(t);e&&e.setWatermark(e.options)}},NavTabs:class{static getMenuWidth(){var t=window.document.querySelector(".nav-menu");return null==t?void 0:t.offsetWidth}},Downloader:R,ClientHub:class t extends r{constructor(t,e){super(),this.mainKey="admin_project_ClientHub_main",this.otherClients=[],this.channel=new BroadcastChannel("admin_project_ClientHub"),this.id=t,this.uuid=function(){let t=localStorage.getItem("admin_project_Client_uuid");if(t)return t;let e=(new Date).getTime();return t="xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,(function(t){const i=(e+16*Math.random())%16|0;return e=Math.floor(e/16),("x"==t?i:3&i|8).toString(16)})),localStorage.setItem("admin_project_Client_uuid",t),t}();const{interval:i,dotnetRef:s}=e;this.interval=i,this.dotnetRef=s}static init(i,s){return e(this,null,(function*(){var e=h(i,(()=>new t(i,s)));const n=yield fetch("/ip.client"),o=yield n.text();e.ip=o,yield e.init()}))}init(){return e(this,null,(function*(){localStorage.getItem(this.mainKey)||localStorage.setItem(this.mainKey,this.id),g(this.channel,"message",(t=>this.receive(t))),window.onunload=t=>this.dispose(),yield this.send(),this.timer=window.setInterval((()=>e(this,null,(function*(){yield this.send()}))),this.interval)}))}send(){return e(this,null,(function*(){this.channel.postMessage({id:this.id,action:"ping"});let t=localStorage.getItem(this.mainKey);t&&0!==this.otherClients.length||(localStorage.setItem(this.mainKey,this.id),t=this.id),t==this.id&&(yield this.dotnetRef.invokeMethodAsync("Tick",[this.uuid,this.ip,navigator.userAgent]))}))}receive(t){const{id:e,action:i}=t.data;if("ping"===i&&void 0===this.otherClients.find((t=>t===e)))this.otherClients.push(e);else if("dispose"===i){const t=this.otherClients.indexOf(e);t>-1&&(this.otherClients=this.otherClients.splice(t,1)),localStorage.getItem(this.mainKey)===e&&localStorage.removeItem(this.mainKey)}}dispose(){window.clearInterval(this.timer),y(this.channel,"message"),this.channel.postMessage({id:this.id,action:"dispose"}),this.channel.close()}}};//# sourceMappingURL=blazor-admin-project.js.map