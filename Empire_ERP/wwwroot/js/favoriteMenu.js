$(document).ready(function () {

    $(".dropdown-item[data-menutype='3']").attr("draggable", true).on("dragstart", onMenuDrag);

    function onMenuDrag(e) {
        e.originalEvent.dataTransfer.setData("menuid", $(this).data("menuid"));
        e.originalEvent.dataTransfer.setData("menuname", $(this).text().trim());
        e.originalEvent.dataTransfer.setData("menuurl", $(this).attr("href"));
        e.originalEvent.dataTransfer.setData("menutype", $(this).data("menutype"));
        e.originalEvent.dataTransfer.setData("mtype", $(this).data("mtype"));
        OpenSidebarOnDrag();
    }
    document.addEventListener('drop', function (e) {
        if (!e.target.closest('.leftsidebar')) return;
        e.preventDefault();
        e.stopPropagation();

        const menuId = e.dataTransfer.getData("menuid");
        const menuName = e.dataTransfer.getData("menuname");
        const menuUrl = e.dataTransfer.getData("menuurl");
        const menutype = e.dataTransfer.getData("menutype");
        const mtype = e.dataTransfer.getData("mtype");
        console.log('mtype', mtype);

        if (mtype == 'S') {
            openFavoriteModal(menuId, menuName, menuUrl);
        } else {
            empr_helper.notify('You cannot add this into Favorite', 2);
        }
    });
    function onMenuDrop(e) {
        e.preventDefault();
        const menuId = e.dataTransfer.getData("menuid");
        const menuName = e.dataTransfer.getData("menuname");
        const menuUrl = e.dataTransfer.getData("menuurl");
        const menutype = e.dataTransfer.getData("menutype");
        console.log('menuId', menuId, menutype);
        if (menutype == 3) {
            openFavoriteModal(menuId, menuName, menuUrl);
        } else {
            empr_helper.notify('You cannot add this into Favorite', 2);
        }
    }
    function openFavoriteModal(menuId, menuName, menuUrl) {
        $("#txtMenuID").val(menuId);
        $("#txtCustomName").val(menuName);
        $("#favoriteModal").modal("show");
        $("#menuURLHidden").val(menuUrl);
    }

    $("#btnSaveFavorite").on("click", function () {
        const data = {
            menuCode: $("#txtMenuID").val(),
            customName: $("#txtCustomName").val(),
            shortcutKey: $('#SHORTCUT').dxSelectBox('option', 'value'),
            menuUrl: $("#menuURLHidden").val()
        };

        var xhr = ajaxHelper.ajaxPostJsonData(data, "/FavoriteMenu/AddFavorite", function (data) {
            if (data.msgType == 1) {
                resetModal();
                loadFavorites();
            }
            else {
                /*empr_helper.notify(data.msg, data.msgType);*/
                resetModal();
            }
        }, false, true);
    });
    function resetModal() {
        $("#favoriteModal").modal("hide");
        $('#SHORTCUT').dxSelectBox('instance').reset();
    }
    //function loadFavorites() {

    //    ajaxHelper.ajaxGetJson("/FavoriteMenu/GetFavorites", function (data) {
    //        debugger;
    //        if (data.msgType == 1) {
    //            let html = "<ul class='ul-leftside'>";
    //            data.data.forEach(item => {
    //                html += `<li><a href='/${item.menU_URL}' title='${item.menU_NAME}'>${item.menU_NAME}</a></li>`;
    //            });
    //            html += "</ul>";
    //            $(".favMenu").html(html);
    //        } else {
    //            /*empr_helper.notify(data.msg, data.msgType);*/
    //        }
    //    }, false, true);

    //}

    loadFavorites();

    InitCommissionAmtDDL();
    function InitCommissionAmtDDL(selectedValue) {
        var dataSource = [
            { key: 'F1', value: 'F1' },
            { key: 'F2', value: 'F2' },
            { key: 'F3', value: 'F3' },
            { key: 'F4', value: 'F4' },
            { key: 'F5', value: 'F5' },
            { key: 'F6', value: 'F6' },
            { key: 'F7', value: 'F7' },
            { key: 'F8', value: 'F8' },
            { key: 'F9', value: 'F9' },
            { key: 'F10', value: 'F10' }
        ];

        $('#SHORTCUT').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'key',
            value: selectedValue,
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500,
        });
    }

    const sidebar = document.querySelector('.leftsidebar');

    sidebar.addEventListener('dragover', e => {
        e.preventDefault();
        sidebar.classList.add('highlight');
    });

    sidebar.addEventListener('dragleave', e => {
        sidebar.classList.remove('highlight');
    });

    sidebar.addEventListener('drop', e => {
        e.preventDefault();
        sidebar.classList.remove('highlight');
        onMenuDrop(e);
    });

    let favoriteShortcuts = [];
    function loadFavorites() {
        $.get("/FavoriteMenu/GetFavorites", function (data) {
            if (data.msgType == 1 && data.data && data.data.length > 0) {
                favoriteShortcuts = data.data.map(item => ({
                    key: item.shortcuT_KEY?.toUpperCase(),
                    url: item.menU_URL
                }));

                let html = "<ul class='ul-leftside'>";
                data.data.forEach(item => {
                    html += `<li><a class='favorite-item' href='${item.menU_URL}' title='Short Key : ${item.shortcuT_KEY}' data-menucode="${item.menU_CODE}">${item.menU_NAME}</a></li>`;
                });
                html += "</ul>";
                $(".favMenu").html(html);
            }
            CheckSidebarState();
        });
    }
    function CheckSidebarState() {
        const sidebar = document.querySelector('.main_wrap');

        // Ajax call to get favorites
        $.get("/FavoriteMenu/GetFavorites", function (data) {
            //if (!data.data || data.data.length === 0) {
            //    // No favorites → collapse sidebar
            //    if (!sidebar.classList.contains('collapse_sidebar')) {
            //        sidebar.querySelector('.toggle_sidebar').click();
            //    }
            //    return;
            //}

            // Favorites exist → check IS_SIDEBAR_OPEN from first favorite (all same user)
            if (data.data.length > 0) {
                const isOpen = data.data[0].iS_SIDEBAR_OPEN;
                const isCollapsed = sidebar.classList.contains('collapse_sidebar');

                if (isOpen)
                    sidebar.classList.remove('collapse_sidebar');
                if (!isOpen && !isCollapsed)
                    sidebar.classList.add('collapse_sidebar')
            }
            else {
                sidebar.classList.add('collapse_sidebar')
            }
        });
    }

    // manually click sidebar
    document.querySelector('.toggle_sidebar').addEventListener('click', function () {
        const sidebar = document.querySelector('.main_wrap');

        const isCurrentlyCollapsed = sidebar.classList.contains('collapse_sidebar');

        const state = isCurrentlyCollapsed ? 0 : 1;

        ajaxHelper.ajaxPostJsonData({ state }, "/FavoriteMenu/UpdateSidebarState", function (data) {
            sidebar.classList.toggle('collapsed');
        }, false, true);
    });
    function OpenSidebarOnDrag() {
        const sidebar = document.querySelector('.main_wrap');
        if (sidebar.classList.contains('collapse_sidebar')) {
            sidebar.classList.remove('collapse_sidebar');
            sidebar.classList.remove('collapsed');

            ajaxHelper.ajaxPostJsonData({ state: 1 }, "/FavoriteMenu/UpdateSidebarState", function (data) {
            }, false, true);
        }
    }

    //shortcut key logic
    document.addEventListener("keydown", function (e) {

        if (e.key === "F5" || e.key === "F3") {
            e.preventDefault();
        }

        const fav = favoriteShortcuts.find(f => f.key === e.key.toUpperCase());
        if (fav) {
            e.preventDefault();
            window.location.href = fav.url;
        }
    });

    document.addEventListener('contextmenu', function (e) {
        const favItem = e.target.closest('.favorite-item');
        if (!favItem) return;

        e.preventDefault();

        document.querySelectorAll('.custom-context-menu').forEach(m => m.remove());

        // Create menu
        const menu = document.createElement('div');
        menu.className = 'custom-context-menu';
        menu.style.position = 'absolute';
        menu.style.top = `${e.pageY}px`;
        menu.style.left = `${e.pageX}px`;
        menu.style.background = '#fff';
        menu.style.border = '1px solid #ccc';
        menu.style.padding = '5px 10px';
        menu.style.borderRadius = '5px';
        menu.style.boxShadow = '0 2px 6px rgba(0,0,0,0.2)';
        menu.style.cursor = 'pointer';
        //menu.innerText = '🗑 Remove';
        menu.innerHTML = '<i class="fa fa-trash" style="margin-right:6px;color:#055a87;"></i> Remove';

        // On click -> delete favorite
        menu.addEventListener('click', function () {
            const menuCode = favItem.getAttribute('data-menucode');
            console.log('menuCode', menuCode);

            fetch("/FavoriteMenu/DeleteFavorite", {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ menuCode })
            })
                .then(res => res.json())
                .then(resp => {
                    /*empr_helper.notify(resp.msg, resp.msgType);*/
                    if (resp.msgType === 1) {
                        favItem.remove();
                        CheckSidebarState();
                    }
                    
                })
                .catch(err => console.error(err));

            menu.remove();
        });

        document.body.appendChild(menu);

        // Hide when clicking elsewhere
        document.addEventListener('click', () => menu.remove(), { once: true });
    });

});
