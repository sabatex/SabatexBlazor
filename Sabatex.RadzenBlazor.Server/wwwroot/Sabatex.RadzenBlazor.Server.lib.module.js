
export function beforeWebStart(options) {
    //var progress = document.getElementById('sabatex-wasm-load-progress');
    //if (progress) {
    //    progress.style.display = 'flex';
    //}
}

export function afterWebAssemblyStarted(blazor) {
    var progress = document.getElementById('sabatex-wasm-load-progress');
    if (progress) {
        progress.style.display = 'none';
    }
}