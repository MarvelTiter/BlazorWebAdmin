import { BaseComponent } from "/_content/BlazorWeb.Shared/js/jscomponentbase/base-component.js";
import { getComponentById } from "/_content/BlazorWeb.Shared/js/jscomponentbase/component-store.js";
import { success, failed } from "/_content/BlazorWeb.Shared/js/jscomponentbase/utils.js"
export class Fetch extends BaseComponent {
    constructor() {
        super()
    }

    async request(option) {
        var req = {
            method: option.method, // *GET, POST, PUT, DELETE, etc.
            mode: "cors", // no-cors, *cors, same-origin
            cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
            credentials: "same-origin", // include, *same-origin, omit
            headers: {
                "Content-Type": "application/json",
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            redirect: "follow", // manual, *follow, error
            referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        }
        ''.toLowerCase
        if (option.method.toLowerCase() != 'get' && option.method.toLowerCase() != 'head') {
            //body: , // body data type must match "Content-Type" header
            req.body = JSON.stringify(option.body)
        }
        const response = await fetch(option.url, req)
        return response
    }

    //static init(id) {
    //    var com = getComponentById(id, () => {
    //        return new Fetch()
    //    })
    //}

    //static async request(id, option) {
    //    var com = getComponentById(id)
    //    try {
    //        var response = await com.request(option)
    //        /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Response/ok) */
    //        if (response.ok) {
    //            /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Request/json) */
    //            return success("", response.json())
    //        } else {
    //            /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Request/text) */
    //            return failed(response.text())
    //        }
    //    } catch (e) {
    //        return failed(`${e.name}:${e.message}`)
    //    }

    //}
}

export function init(id) {
    var com = getComponentById(id, () => {
        return new Fetch()
    })
}

export async function request(id, option) {
    var com = getComponentById(id)
    try {
        var response = await com.request(option)
        /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Response/ok) */
        if (response.ok) {
            /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Request/json) */
            return success("", response.json())
        } else {
            /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/Request/text) */
            return failed(response.text())
        }
    } catch (e) {
        return failed(`${e.name}:${e.message}`)
    }

}