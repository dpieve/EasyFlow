export function setBrowserTitle(title) {
    if (typeof title === "string") {
        document.title = title;
    } else {
        console.error("Title must be a string.");
    }
}