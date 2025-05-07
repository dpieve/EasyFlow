export function setBrowserTitle(title) {
    if (typeof title === "string") {
        document.title = title;
    } else {
        console.error("Title must be a string.");
    }
}
export function logValue(value) {
    console.log('Value:', value);
}

export function openUrl(url) {
    console.log(url);
    if (typeof url === "string") {
        const formattedUrl = url.startsWith('http://') || url.startsWith('https://')
            ? url
            : `https://${url}`;

        window.open(formattedUrl, "_blank");
    } else {
        console.error("URL must be a string.");
    }
}

