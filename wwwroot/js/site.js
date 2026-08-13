// Script chung toàn site
document.addEventListener('DOMContentLoaded', function () {
    // Xử lý toggle dropdown khi click nút user menu
    const userMenuDropdowns = document.querySelectorAll('.user-menu-dropdown');

    userMenuDropdowns.forEach(function (dropdown) {
        const btn = dropdown.querySelector('.btn-user-menu');
        const menu = dropdown.querySelector('.dropdown-menu-custom');

        if (btn && menu) {
            btn.addEventListener('click', function (e) {
                e.stopPropagation();
                const isOpen = menu.classList.contains('show');

                // Đóng các dropdown khác nếu đang mở
                document.querySelectorAll('.dropdown-menu-custom.show').forEach(function (otherMenu) {
                    if (otherMenu !== menu) {
                        otherMenu.classList.remove('show');
                        const otherBtn = otherMenu.parentElement.querySelector('.btn-user-menu');
                        if (otherBtn) otherBtn.setAttribute('aria-expanded', 'false');
                    }
                });

                menu.classList.toggle('show', !isOpen);
                btn.setAttribute('aria-expanded', !isOpen ? 'true' : 'false');
            });
        }
    });

    // Đóng dropdown khi click ra ngoài màn hình
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.user-menu-dropdown')) {
            document.querySelectorAll('.dropdown-menu-custom.show').forEach(function (menu) {
                menu.classList.remove('show');
                const btn = menu.parentElement.querySelector('.btn-user-menu');
                if (btn) btn.setAttribute('aria-expanded', 'false');
            });
        }
    });
});
