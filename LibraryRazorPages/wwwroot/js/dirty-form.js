(function () {
    const form = document.getElementById('book-form');
    const saveBtn = document.getElementById('save-btn');

    if (!form || !saveBtn) {
        return;
    }

    let initialSnapshot = serialize(new FormData(form));
    let isSubmitting = false;

    function serialize(formData) {
        const arr = [];
        for (const [k, v] of formData.entries()) arr.push(k + '=' + v);
        return arr.sort().join('&');
    }

    function isDirty() {
        return serialize(new FormData(form)) !== initialSnapshot;
    }

    function updateButton() {
        saveBtn.disabled = !isDirty();
    }

    form.addEventListener('input', updateButton);
    form.addEventListener('change', updateButton);

    document.addEventListener('ckeditor:ready', () => {
        initialSnapshot = serialize(new FormData(form));
        updateButton();
    });

    form.addEventListener('submit', () => {
        isSubmitting = true;
    });

    window.addEventListener('beforeunload', (e) => {
        if (!isSubmitting && isDirty()) {
            e.preventDefault();
            e.returnValue = '';
        }
    });
})();