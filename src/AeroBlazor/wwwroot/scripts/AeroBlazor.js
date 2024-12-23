window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text)
            .catch(function (error) {
                alert(error);
            });
    }
};

window.getCoords = async () => {
    try {
        const pos = await new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition(resolve, reject, {timeout: 10000});
        });
        return [pos.coords.longitude, pos.coords.latitude];
    } catch (err) {
        console.log(err)
        return [0, 0];
    }
};

window.download = {
    downloadFile: function (filename, contentType, byteArray) {
        const blob = new Blob([new Uint8Array(byteArray)], { type: contentType });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
};