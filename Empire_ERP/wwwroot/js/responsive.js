/**
 * Empire ERP — Responsive UI behavior (frontend-only)
 * Phase 1: mobile nav, touch submenus, sidebar overlay
 * Phase 3: DevExtreme column hiding on narrow viewports
 */
(function (window, document, $) {
    'use strict';

    var MQ_LG = '(max-width: 991.98px)';

    function isNarrow() {
        return window.matchMedia(MQ_LG).matches;
    }

    function closeNav() {
        var bar = document.querySelector('.erp-nav-bar');
        if (bar) bar.classList.remove('erp-nav-open');
        document.body.classList.remove('erp-nav-open');
        var toggle = document.getElementById('erpNavToggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'false');
    }

    function openNav() {
        closeSidebar();
        var bar = document.querySelector('.erp-nav-bar');
        if (bar) bar.classList.add('erp-nav-open');
        document.body.classList.add('erp-nav-open');
        var toggle = document.getElementById('erpNavToggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'true');
    }

    function toggleNav() {
        var bar = document.querySelector('.erp-nav-bar');
        if (!bar) return;
        if (bar.classList.contains('erp-nav-open')) closeNav();
        else openNav();
    }

    function closeSidebar() {
        var wrap = document.querySelector('.main_wrap');
        if (!wrap) return;
        wrap.classList.add('collapse_sidebar');
        wrap.classList.remove('erp-sidebar-open');
        document.body.classList.remove('erp-sidebar-open');
    }

    function openSidebar() {
        closeNav();
        var wrap = document.querySelector('.main_wrap');
        if (!wrap) return;
        wrap.classList.remove('collapse_sidebar');
        wrap.classList.add('erp-sidebar-open');
        document.body.classList.add('erp-sidebar-open');
    }

    function toggleSidebarOverlay() {
        var wrap = document.querySelector('.main_wrap');
        if (!wrap) return;
        if (wrap.classList.contains('erp-sidebar-open')) closeSidebar();
        else openSidebar();
    }

    function ensureChrome() {
        var navUl = document.querySelector('.headertopbar ul.nav.ul-navbar');
        if (navUl) {
            var menuHost = navUl.closest('.container-fluid');
            if (menuHost && !menuHost.classList.contains('erp-nav-bar')) {
                menuHost.classList.add('erp-nav-bar');
                var collapse = document.createElement('div');
                collapse.className = 'erp-nav-collapse';
                collapse.id = 'erpNavCollapse';
                while (menuHost.firstChild) {
                    collapse.appendChild(menuHost.firstChild);
                }
                var btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'erp-nav-toggle';
                btn.id = 'erpNavToggle';
                btn.setAttribute('aria-label', 'Toggle navigation');
                btn.setAttribute('aria-expanded', 'false');
                btn.innerHTML = '<i class="fa fa-bars" aria-hidden="true"></i>';
                menuHost.appendChild(btn);
                menuHost.appendChild(collapse);
            }
        }

        if (!document.getElementById('erpNavBackdrop')) {
            var nb = document.createElement('div');
            nb.id = 'erpNavBackdrop';
            nb.className = 'erp-nav-backdrop';
            document.body.appendChild(nb);
        }
        if (!document.getElementById('erpSidebarBackdrop')) {
            var sb = document.createElement('div');
            sb.id = 'erpSidebarBackdrop';
            sb.className = 'erp-sidebar-backdrop';
            document.body.appendChild(sb);
        }
        if (!document.getElementById('erpSidebarFab')) {
            var fab = document.createElement('button');
            fab.type = 'button';
            fab.id = 'erpSidebarFab';
            fab.className = 'erp-sidebar-fab';
            fab.setAttribute('aria-label', 'Open quick links');
            fab.innerHTML = '<i class="ti-angle-right" aria-hidden="true"></i>';
            document.body.appendChild(fab);
        }
    }

    function bindNav() {
        $(document).on('click', '#erpNavToggle', function (e) {
            e.preventDefault();
            toggleNav();
        });
        $(document).on('click', '#erpNavBackdrop', closeNav);
        $(document).on('click', '#erpSidebarBackdrop', closeSidebar);
        $(document).on('click', '#erpSidebarFab', function (e) {
            e.preventDefault();
            openSidebar();
        });

        $(document).on('click', '.ul-navbar .nav-item > a.dropdown-toggle', function (e) {
            if (!isNarrow()) return;
            e.preventDefault();
            e.stopPropagation();
            var $item = $(this).closest('.nav-item');
            $item.siblings('.nav-item').removeClass('erp-submenu-open');
            $item.toggleClass('erp-submenu-open');
        });

        $(document).on('click', '.ul-navbar .dropdown-submenu > a.dropdown-toggle, .ul-navbar .dropdown-submenu > a.dropdown-item.dropdown-toggle', function (e) {
            if (!isNarrow()) return;
            var $sub = $(this).closest('.dropdown-submenu');
            if ($sub.children('.dropdown-menu, .submenu').length) {
                e.preventDefault();
                e.stopPropagation();
                $sub.siblings('.dropdown-submenu').removeClass('erp-submenu-open');
                $sub.toggleClass('erp-submenu-open');
            }
        });

        $(document).on('click', '.ul-navbar a.dropdown-item:not(.dropdown-toggle)', function () {
            if (isNarrow()) closeNav();
        });

        // Capture phase: custom.js stops propagation on .dropdown-menu
        document.addEventListener('click', function (e) {
            if (!isNarrow()) return;
            var leaf = e.target.closest && e.target.closest('#erpNavCollapse a.dropdown-item:not(.dropdown-toggle)');
            if (leaf && leaf.getAttribute('href') && leaf.getAttribute('href') !== 'javascript:;') {
                closeNav();
            }
        }, true);
    }

    function applyNarrowShellDefaults() {
        if (!isNarrow()) {
            document.body.classList.remove('erp-nav-open', 'erp-sidebar-open');
            closeNav();
            var wrapWide = document.querySelector('.main_wrap');
            if (wrapWide) wrapWide.classList.remove('erp-sidebar-open');
            var fab = document.getElementById('erpSidebarFab');
            if (fab) fab.style.display = 'none';
            return;
        }
        var wrap = document.querySelector('.main_wrap');
        if (wrap) {
            wrap.classList.add('collapse_sidebar');
            wrap.classList.remove('erp-sidebar-open');
            document.body.classList.remove('erp-sidebar-open');
        }
        closeNav();
    }

    function patchDxDataGrid() {
        if (!$ || !$.fn || typeof $.fn.dxDataGrid !== 'function') return;
        if ($.fn.dxDataGrid.__erpResponsivePatched) return;

        var original = $.fn.dxDataGrid;
        function wrapped(options) {
            if (options && typeof options === 'object' && !Array.isArray(options)) {
                if (options.columnHidingEnabled === undefined && isNarrow()) {
                    options.columnHidingEnabled = true;
                }
            }
            return original.apply(this, arguments);
        }
        wrapped.__erpResponsivePatched = true;
        for (var key in original) {
            if (Object.prototype.hasOwnProperty.call(original, key)) {
                try { wrapped[key] = original[key]; } catch (ex) { /* ignore */ }
            }
        }
        $.fn.dxDataGrid = wrapped;
    }

    function fixLegacyModalDismiss() {
        $(document).on('click', '[data-dismiss="modal"]', function () {
            var modalEl = this.closest('.modal');
            if (!modalEl) return;
            if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                var inst = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                inst.hide();
            } else if ($ && $(modalEl).modal) {
                $(modalEl).modal('hide');
            }
        });
    }

    // Expose for custom.js sidebar toggle
    window.erpResponsive = {
        isNarrow: isNarrow,
        toggleSidebarOverlay: toggleSidebarOverlay,
        closeSidebar: closeSidebar,
        closeNav: closeNav
    };

    $(function () {
        ensureChrome();
        bindNav();
        applyNarrowShellDefaults();
        fixLegacyModalDismiss();

        setTimeout(function () {
            if (isNarrow()) applyNarrowShellDefaults();
        }, 1200);
        setTimeout(function () {
            if (isNarrow()) applyNarrowShellDefaults();
        }, 2500);

        var resizeTimer;
        $(window).on('resize', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(applyNarrowShellDefaults, 150);
        });
    });

    // Patch DX as soon as plugin exists (layout loads this after dx.all.js)
    if ($ && $.fn && $.fn.dxDataGrid) {
        patchDxDataGrid();
    } else {
        $(function () { patchDxDataGrid(); });
    }
})(window, document, window.jQuery);
