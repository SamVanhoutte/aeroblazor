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