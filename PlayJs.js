export function playAudio(audioFileName, volume) {
    let audio = new Audio(audioFileName);
    audio.volume = volume / 100.0;

    audio.play()
        .then(() => console.log('Audio is playing'))
        .catch(error => console.error('Error playing audio:', error));
}

export function logValue(value) {
    console.log('Value:', value);
}

export function showNotification(title, message) {
    console.log('Showing notification:', title, message);

    if (!("Notification" in window)) {
        console.error("This browser does not support system notifications.");
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