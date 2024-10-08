export class Downloader {
    static download(_, payload: any) {
        const tempform = document.createElement("form")
        tempform.action = "/api/download"
        tempform.method = "post"
        tempform.style.display = "none"

        const keys = Object.keys(payload)
        for (let i = 0; i < keys.length; i++) {
            const input = document.createElement("input")
            const key = keys[i]
            input.hidden = true
            input.name = key
            input.value = payload[key]
            tempform.appendChild(input)
        }

        const submit = document.createElement("input")
        submit.type = "submit"
        tempform.appendChild(submit)
        document.body.appendChild(tempform)
        tempform.submit()
        document.body.removeChild(tempform)
    }

    static downloadStream = async (_, payload: any) => {
        try {
            const {filename, streamRef} = payload;
            // window['Stream'] = streamRef
            // const arrayBuffer = new Uint8Array(buffer)
            const arrayBuffer = await streamRef.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const url = URL.createObjectURL(blob);
            const anchorElement = document.createElement('a');
            // window.document.body.append(anchorElement);
            anchorElement.href = url;
            anchorElement.download = filename ?? '';
            anchorElement.click();
            anchorElement.remove();
            URL.revokeObjectURL(url);
        } catch (e) {
            console.error(e);
        }
    }
}
