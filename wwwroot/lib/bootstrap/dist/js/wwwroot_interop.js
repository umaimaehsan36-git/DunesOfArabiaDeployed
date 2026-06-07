// ============================================================
// wwwroot/js/interop.js
// Add this file to wwwroot/js/ and reference it in App.razor:
//   <script src="js/interop.js"></script>
// ============================================================

/**
 * FIX for BookingConfirmation.razor: DownloadReceipt
 * Called via JS.InvokeVoidAsync("downloadBase64File", base64, fileName, mimeType)
 * Triggers a browser file download from a base64 string without a server round-trip.
 */
window.downloadBase64File = function (base64, fileName, mimeType) {
    const byteChars = atob(base64);
    const byteArrays = [];
    for (let i = 0; i < byteChars.length; i += 512) {
        const slice = byteChars.slice(i, i + 512);
        const bytes = new Uint8Array(slice.length);
        for (let j = 0; j < slice.length; j++) {
            bytes[j] = slice.charCodeAt(j);
        }
        byteArrays.push(bytes);
    }
    const blob = new Blob(byteArrays, { type: mimeType });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};
