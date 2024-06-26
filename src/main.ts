import * as u from './utilsAggregation.ts'
import { ActionWatcher } from './Shared/Project.Web.Shared/Components/ActionWatcher/ActionWatcher.razor.ts'
import { Camera } from './Shared/Project.Web.Shared/Components/Camera/Camera.razor.ts'
import { EdgeWidget } from './Shared/Project.Web.Shared/Components/EdgeWidget/EdgeWidget.razor.ts'
import { Fetch } from './Shared/Project.Web.Shared/Components/Fetch/Fetch.razor.ts'
import { FullScreen } from './Shared/Project.Web.Shared/Components/FullScreen/FullScreen.razor.ts'
import { JsTimer } from './Shared/Project.Web.Shared/Components/JsTimer/JsTimer.razor.ts'
import { ScrollBar } from './Shared/Project.Web.Shared/Components/ScrollBar/ScrollBar.razor.ts'
import { HorizontalScroll } from './Shared/Project.Web.Shared/Components/ScrollBar/HorizontalScroll.razor.ts'
import { SplitView } from './Shared/Project.Web.Shared/Components/SplitView/SplitView.razor.ts'
import { WaterMark } from './Shared/Project.Web.Shared/Components/WaterMark/WaterMark.razor.ts'
import { NavTabs } from './Shared/Project.AppCore/Layouts/LayoutComponents/NavTabs.razor.ts'
import { Downloader } from './Shared/Project.Web.Shared/Components/Downloader/Downloader.razor.ts'

declare global {
    interface Window {
        BlazorAdminProject: any
        Utils: any
    }
}

window.Utils = u

window.BlazorAdminProject = {
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
    Downloader
}
