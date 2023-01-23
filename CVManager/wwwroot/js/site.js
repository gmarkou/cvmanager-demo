
(function setupFlash() {
    const el = document.getElementById("flash");
    if (el) {
        const cancelToken = setTimeout(() => {
            el.style.opacity = "0";
        }, 10000);
        el.addEventListener("click", () => {
            el.style.opacity = "0";
            if (cancelToken) {
                clearTimeout(cancelToken);
            }
        })
    }
})();

(function setupFileUpload() {
    const el = document.getElementById('CancelFileUpload');
    if (el) {
        el.addEventListener('click', (e) => {
            e.preventDefault();
            const b = document.getElementById('FileUpload');
            b.value = null;              
        }); 
    }
})();


(function setupFileViewEdit() {
    const fvd = document.getElementById('fg-file-view-delete');
    if (fvd === null) {
        return;
    }
    const fu = document.getElementById('fg-file-upload');
    if (fu) {
         fu.style.display = 'none';
    }
    
    const dfc = document.getElementById('DeleteFileCheck');
    if (dfc) {
        dfc.addEventListener('change', () => {
            if (dfc.checked) {
                document.getElementById('current-file').style.display = 'none';
                if (fu) {
                    fu.style.display = 'block';
                }
            }
            else {
                document.getElementById('current-file').style.display = 'inline-block';
                if (fu) {
                    const b = document.getElementById('FileUpload');
                    if (b) b.value = null;
                    fu.style.display = 'none';
                }
            }
        });
    }
    
})();
