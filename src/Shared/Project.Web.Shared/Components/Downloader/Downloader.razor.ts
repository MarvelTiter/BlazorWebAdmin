export class Downloader {
    static download(_, payload: any) {
        var tempform = document.createElement("form")
        tempform.action = "/api/download"
        tempform.method = "post"
        tempform.style.display = "none"

        var keys = Object.keys(payload)
        for (var i = 0; i < keys.length; i++) {
            var input = document.createElement("input")
            var key = keys[i]
            input.hidden = true
            input.name = key
            input.value = payload[key]
            tempform.appendChild(input)
        }

        var submit = document.createElement("input")
        submit.type = "submit"
        tempform.appendChild(submit)
        document.body.appendChild(tempform)
        tempform.submit()
        document.body.removeChild(tempform)
    }
}
