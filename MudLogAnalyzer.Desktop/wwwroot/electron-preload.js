// Polyfill for Node.js 'process' module that Blazor WebAssembly expects
window.process = {
    env: {},
    argv: [],
    platform: 'electron',
    version: '20.0.0',
    versions: {
        node: '20.0.0',
        electron: '20.0.0'
    },
    nextTick: function(callback) {
        setTimeout(callback, 0);
    },
    cwd: function() {
        return '/';
    },
    stdout: {
        write: function(data) {
            console.log(data);
        }
    },
    stderr: {
        write: function(data) {
            console.error(data);
        }
    }
};

// Add other Node.js polyfills that might be needed
window.global = window;
window.Buffer = window.Buffer || {};

const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronWindowControl', (action) => {
    ipcRenderer.send('window-control', action);
});

window.electronWindowControl = (action) => {
    ipcRenderer.send('window-control', action);
};