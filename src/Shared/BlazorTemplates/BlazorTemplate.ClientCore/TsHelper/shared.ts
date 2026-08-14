import * as u from './utilsAggregation.ts'
import { ActionWatcher } from '../Components/ActionWatcher/ActionWatcher.razor.ts'
import { Camera } from '../Components/Camera/Camera.razor.ts'
import { EdgeWidget } from '../Components/EdgeWidget/EdgeWidget.razor.ts'
import { Fetch } from '../Components/Fetch/Fetch.razor.ts'
import { FullScreen } from '../Components/FullScreen/FullScreen.razor.ts'
import { JsTimer } from '../Components/JsTimer/JsTimer.razor.ts'
import { ScrollBar } from '../Components/ScrollBar/ScrollBar.razor.ts'
import { HorizontalScroll } from '../Components/ScrollBar/HorizontalScroll.razor.ts'
import { SplitView } from '../Components/SplitView/SplitView.razor.ts'
import { WaterMark } from '../Components/WaterMark/WaterMark.razor.ts'
import { NavTabs } from '../Layouts/LayoutComponents/NavTabs.razor.ts'
import { Downloader } from '../Components/Downloader/Downloader.razor.ts'
import { ClientHub } from '../Components/ClientHub/ClientHub.ts'
import { mergeUtils, mergeComponents } from '@/utils.ts'


mergeUtils(u.default)

const shareComponents = {
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
    //SvgIcon,
    ClientHub
}

mergeComponents(shareComponents)