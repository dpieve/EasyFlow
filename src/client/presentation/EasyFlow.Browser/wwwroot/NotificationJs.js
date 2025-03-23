export function showNotification(title, message) {

    if (!("Notification" in window)) {
        console.log('This browser does not support system notifications.');
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