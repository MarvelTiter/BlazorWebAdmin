//function Thumb(root, position, tracker, thumb) {
//    this.root = root;
//    this.position = position;
//    this.cursorDown = false;
//    this.cursorLeave = false;
//    this.tracker = tracker;
//    this.thumb = thumb;
//    this.map = BAR_MAP[position];
//    this.state = {
//        X: 0,
//        Y: 0,
//    };
//    this.mouseMoveDocumentHandler = function (e) {
//        if (!this.tracker || !this.thumb) return
//        if (this.cursorDown === false) return

//        const prevPage = this.state[this.map.axis]
//        if (!prevPage) return
//        const offset = (this.tracker.getBoundingClientRect()[this.map.direction] - e[this.map.client]) * -1
//        const thumbClickPosition = this.thumb[this.map.offset] - prevPage
//        const thumbPositionPercentage = ((offset - thumbClickPosition) * 100 * this.root[this.map.ratio]) / this.tracker[this.map.offset]
//        this.root.wrap[this.map.scroll] = (thumbPositionPercentage * this.root.wrap[this.map.scrollSize]) / 100
//        this.updateMove();
//    };

//    this.clickTrackerHandler = function (e) {
//        if (!this.thumb || !this.tracker || !this.root.wrap) return;
//        // 点击位置在滚动条中的偏移
//        const offset = Math.abs(e.target.getBoundingClientRect()[this.map.direction] - e[this.map.client]);
//        const thumbHalf = this.thumb[this.map.offset] / 2;
//        const thumbPositionPercentage = ((offset - thumbHalf) * 100 * this.root[this.map.ratio]) / this.tracker[this.map.offset]
//        this.root.wrap[this.map.scroll] = (thumbPositionPercentage * this.root.wrap[this.map.scrollSize]) / 100
//    };
//    this.clickThumbHandler = function (e) {
//        e.stopPropagation();
//        if (e.ctrlKey || [1, 2].indexOf(e.button) > -1) return;
//        window.getSelection()?.removeAllRanges();
//        startDrag(e, this)
//        const el = e.currentTarget;
//        if (!el) return
//        this.state[this.map.axis] = el[this.map.offset] - (e[this.map.client] - el.getBoundingClientRect()[this.map.direction])
//    };
//    this.mouseUpDocumentHandler = function (e) {
//        mouseUpDocumentHandler(e, this);
//    };
//    this.setMove = function (move) {
//        this.thumb.style.transform = `translate${this.map.axis}(${move}%)`
//    };
//    this.updateMove = function () {
//        const offset = this.root.wrap[this.map.offset] - GAP;
//        this.setMove(((this.root.wrap[this.map.scroll] * 100) / offset) * this.root[this.map.ratio]);
//    };
//    this.setVisible = function (visible) {
//        if (this.tracker) this.tracker.style.display = visible ? 'block' : 'none';
//    };
//    this.initEvents = function () {
//        this.tracker.addEventListener('mousedown', this.clickTrackerHandler.bind(this));
//        this.thumb.addEventListener('mousedown', this.clickThumbHandler.bind(this));
//    }
//}


//function Bar(parent) {
//    this.always = false;
//    this.vertical = null;
//    this.horizontal = null;
//    this.parent = parent;
//    this.update = function () {
//        if (this.vertical && this.vertical.thumb) {
//            this.vertical.thumb.style['height'] = this.parent.sizeHeight;
//        }
//        if (this.horizontal && this.horizontal.thumb) {
//            this.horizontal.thumb.style['width'] = this.parent.sizeWidth;
//        }
//    };
//    this.handleScroll = function (e) {
//        if (this.parent.wrap) {
//            const offsetHeight = this.parent.wrap.offsetHeight - GAP
//            const offsetWidth = this.parent.wrap.offsetWidth - GAP
//            if (this.vertical)
//                this.vertical.setMove(((this.parent.wrap.scrollTop * 100) / offsetHeight) * this.parent.ratioY);
//            if (this.horizontal)
//                this.horizontal.setMove(((this.parent.wrap.scrollLeft * 100) / offsetWidth) * this.parent.ratioX);
//        }
//    };
//    this.setVisible = function (visible) {
//        if (this.always) return;
//        if (this.vertical) this.vertical.setVisible(visible);
//        if (this.horizontal) this.horizontal.setVisible(visible);
//    }
//}

//function ScrollBar() {
//    this.root = null;
//    this.wrap = null;
//    this.resize = null;
//    this.minSize = 0;
//    this.ratioX = 1;
//    this.ratioY = 1;
//    this.sizeHeight = '';
//    this.sizeWidth = '';

//    this.bar = new Bar(this);

//    this.update = function () {
//        if (!this.wrap) return;
//        var offsetHeight = this.wrap.offsetHeight - GAP;
//        var offsetWidth = this.wrap.offsetWidth - GAP;
//        var scrollHeight = this.wrap.scrollHeight;
//        var scrollWidth = this.wrap.scrollWidth;
//        var originalHeight = offsetHeight * offsetHeight / scrollHeight;
//        var originalWidth = offsetWidth * offsetWidth / scrollWidth;
//        var height = Math.max(originalHeight, this.minSize);
//        var width = Math.max(originalWidth, this.minSize);

//        this.ratioY = originalHeight /
//            (offsetHeight - originalHeight) /
//            (height / (offsetHeight - height));
//        this.ratioX = originalWidth /
//            (offsetWidth - originalWidth) /
//            (width / (offsetWidth - width));

//        this.sizeHeight = height + GAP < offsetHeight ? height + 'px' : '';
//        this.sizeWidth = width + GAP < offsetWidth ? width + 'px' : '';
//        this.bar.update();
//    };

//    this.initEvents = function () {
//        this.root.addEventListener('mousemove', handleMouseMove.bind(this));
//        this.root.addEventListener('mouseleave', handleMouseLeave.bind(this));
//        this.wrap.addEventListener('scroll', handleScroll.bind(this));
//    };
//}