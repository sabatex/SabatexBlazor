
export function beforeWebStart(options) {
 
}

export function afterWebAssemblyStarted(blazor) {
    var progress = document.getElementById('sabatex-wasm-load-progress');
    progress.remove();
}