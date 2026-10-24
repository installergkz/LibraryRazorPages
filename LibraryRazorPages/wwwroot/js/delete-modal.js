(function () {
    const modalEl = document.getElementById('deleteModal');
    if (!modalEl) return;

    const modal = new bootstrap.Modal(modalEl);
    const idInput = document.getElementById('deleteItemId');
    const nameSpan = document.getElementById('deleteItemName');

    document.querySelectorAll('.js-delete-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.preventDefault();
            idInput.value = btn.dataset.id;
            nameSpan.textContent = '«' + (btn.dataset.name || '') + '»';
            modal.show();
        });
    });
})();