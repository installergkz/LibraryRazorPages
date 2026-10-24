(function () {
    const select = document.getElementById('key');
    const termWrapper = document.getElementById('term-wrapper');
    const form = document.getElementById('report-form');
    const overlay = document.getElementById('loading-overlay');
    const resultBlock = document.getElementById('report-result');

    // Логика show/hide поля term — работает всегда
    if (select && termWrapper) {
        select.addEventListener('change', () => {
            const option = select.options[select.selectedIndex];
            const hasParams = option?.dataset.hasParams === 'true';

            termWrapper.classList.toggle('d-none', !hasParams);

            if (!hasParams) {
                const term = document.getElementById('term');
                if (term) term.value = '';
            }
        });
    }

    // Показ спиннера при отправке формы
    if (form && overlay) {
        form.addEventListener('submit', () => {
            overlay.classList.remove('d-none');
            if (resultBlock) {
                resultBlock.classList.add('d-none');
            }
        });
    }
})();