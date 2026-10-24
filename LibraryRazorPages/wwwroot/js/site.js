document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.app-nav__toggle').forEach(btn => {
        btn.addEventListener('click', () => btn.classList.toggle('open'));
    });

    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 3000);
    });
});