export function logValue(value) {
    console.log('Value:', value);
}

export function showNotification(title, message) {
    if (!("Notification" in window)) {
        alert("This browser does not support system notifications.");
        return;
    }

    if (Notification.permission === "granted") {
        new Notification(title, { body: message });
    } else if (Notification.permission !== "denied") {
        Notification.requestPermission().then(permission => {
            if (permission === "granted") {
                new Notification(title, { body: message });
            }
        });
    }
}

export function playAudio(audioFileName, volume) {
    let audio = new Audio(audioFileName);
    audio.volume = volume / 100.0;
    audio.play()
        .catch(error => console.error('Error playing audio:', error));
}

export function setBrowserTitle(title) {
    document.title = title;
}

export function openUrl(url) {
    window.open(url, "_blank");
}

export function getStorageItem(key) {
    return localStorage.getItem(key);
}

export function setStorageItem(key, value) {
    localStorage.setItem(key, value);
}