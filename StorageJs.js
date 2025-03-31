export function logValue(value) {
    console.log('Value:', value);
}

export function getStorageItem(key) {
    return localStorage.getItem(key);
}

export function setStorageItem(key, value) {
    localStorage.setItem(key, value);
}