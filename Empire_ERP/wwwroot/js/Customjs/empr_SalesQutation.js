var empr_SalesQutation = {
    selectedItems: [],
    selectedGroupId: null,
    selectedGroupName: '',
    groupItemsCache: {},
    highlightToken: 0,
    searchToken: 0,
    searchPrefetchStarted: false,
    searchPrefetchCallbacks: [],
    CurrentStock: [],
    partyLastItemData: [],
    excelItemLookup: [],
    excelSaving: false,
    excelPanelOpen: false,
    excelState: {
        fileName: '',
        branches: [],
        masters: [],
        failedRows: [],
        canComplete: false
    },
    InitEvents: function () {
        $(document).ready(function () {
            empr_SalesQutation.InitQuickSearchGrid();
            empr_SalesQutation.GetCurrentStock();
            empr_SalesQutation.ResetForm();
            empr_SalesQutation.RenderItemGroups();
            empr_SalesQutation.InitReportTypeDDL();
            $('#SaveQuotationModal').modal('show');
            window.addEventListener('message', function (event) {
                if (event.origin !== window.location.origin) {
                    return;
                }
                var data = event.data;
                if (data && data.traN_ID) {
                    empr_SalesQutation.ResetForm();
                    $('#Code').val(data.traN_ID);
                    empr_SalesQutation.GetSalesQutationByCode(data.traN_ID);
                }
            });
            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                empr_SalesQutation.ResetForm();
                $('#Code').val(id);
                empr_helper.selectedBill = id;
                $('.modal').modal('hide');
                empr_SalesQutation.GetSalesQutationByCode(id);
            });

            $('body').on('click', '#BtnSave', function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_SalesQutation.ValidateMainInfo()) {
                            empr_SalesQutation.Save();
                        }
                    }
                } else {
                    if (empr_SalesQutation.ValidateMainInfo()) {
                        empr_SalesQutation.Save();
                    }
                }
            });

            $('body').on('click', '#BtnModalSave', function () {
                if (empr_SalesQutation.HasUploadedExcel()) {
                    return;
                }
                if (empr_SalesQutation.ValidatePartySelection()) {
                    empr_SalesQutation.UpdatePartyNameDisplay();
                    empr_SalesQutation.LoadPartyLastItemData();
                    $('#SaveQuotationModal').modal('hide');
                }
            });

            $('body').on('click', '#BtnUploadFile', function () {
                empr_SalesQutation.ShowExcelUploadPanel();
            });

            $('body').on('click', '#BtnSelectExcel', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $('#sqExcelFile').val('');
                $('#sqExcelFile').trigger('click');
            });

            $('body').on('click', '#sqExcelDrop', function (e) {
                if ($(e.target).closest('#BtnSelectExcel, #BtnExcelClear, #sqExcelFile').length) {
                    return;
                }
                $('#sqExcelFile').val('');
                $('#sqExcelFile').trigger('click');
            });

            $('body').on('change', '#sqExcelFile', function () {
                var file = this.files && this.files[0] ? this.files[0] : null;
                empr_SalesQutation.ProcessExcelFile(file);
            });

            $('body').on('dragover dragenter', '#sqExcelDrop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $(this).addClass('sq-excel-dragover');
            });

            $('body').on('dragleave drop', '#sqExcelDrop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $(this).removeClass('sq-excel-dragover');
            });

            $('body').on('drop', '#sqExcelDrop', function (e) {
                var files = e.originalEvent && e.originalEvent.dataTransfer
                    ? e.originalEvent.dataTransfer.files
                    : null;
                var file = files && files[0] ? files[0] : null;
                empr_SalesQutation.ProcessExcelFile(file);
            });

            $('body').on('click', '#BtnExcelClear', function (e) {
                e.preventDefault();
                e.stopPropagation();
                empr_SalesQutation.ResetExcelUpload(false, true);
            });

            $('body').on('click', '#BtnExcelComplete', function () {
                empr_SalesQutation.CompleteExcelUpload();
            });

            $('#SaveQuotationModal').on('shown.bs.modal', function () {
                var partyBox = $('#PARTY_CODE').dxSelectBox('instance');
                if (partyBox) {
                    partyBox.option('dropDownOptions', { container: '#SaveQuotationModal' });
                    partyBox.repaint();
                }
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_SalesQutation.GeneratePrintReport();
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_SalesQutation.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_SalesQutation.ResetForm();
                $('#SaveQuotationModal').modal('show');
            });

            $('body').on('click', '#BtnChangeParty', function () {
                $('#SaveQuotationModal').modal('show');
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_SalesQutation.InitQuickSearchGrid();
            });

            $('body').on('click', '.sq-group', function () {
                var groupId = $(this).data('item');
                empr_SalesQutation.SelectGroup(groupId, $(this).data('name'));
            });

            $('body').on('input', '#sqItemSearch', function () {
                empr_SalesQutation.SearchItemsAcrossGroups();
            });

            $('body').on('click', '.sq-item-card', function (e) {
                if ($(e.target).closest('.sq-rate-field, .sq-barcode-field').length) {
                    return;
                }
                var $card = $(this);
                var itemCode = $card.data('item');
                var rate = $card.find('.sq-rate-input').val();
                var barcode = $card.find('.sq-barcode-input').val();
                empr_SalesQutation.IncreaseItemQty(itemCode, rate, barcode);
            });

            $('body').on('click mousedown', '.sq-rate-field, .sq-barcode-field', function (e) {
                e.stopPropagation();
            });

            $('body').on('change', '.sq-rate-input', function () {
                var $card = $(this).closest('.sq-item-card');
                empr_SalesQutation.UpdateItemRate($card.data('item'), $(this).val());
            });

            $('body').on('change input', '.sq-barcode-input', function () {
                var $card = $(this).closest('.sq-item-card');
                empr_SalesQutation.UpdateItemBarcode($card.data('item'), $(this).val());
            });

            $('body').on('click', '.sq-qty-minus', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var itemCode = $(this).closest('.sq-item-card').data('item');
                empr_SalesQutation.DecreaseItemQty(itemCode);
            });

            $('body').on('click', '#BtnViewCart', function () {
                empr_SalesQutation.RenderCartItems();
                $('#ViewCartModal').modal('show');
            });

            $('body').on('click', '.sq-cart-remove', function () {
                var itemCode = $(this).data('item');
                empr_SalesQutation.RemoveCartItem(itemCode);
            });

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            }
        });
    },

    RenderItemGroups: function () {
        var $list = $('#sqGroupList');
        $list.empty();
        if (ItemsGroup && ItemsGroup.length > 0) {
            var groups = ItemsGroup.slice().sort(function (a, b) {
                return String(a.grouP_NAME || '').localeCompare(String(b.grouP_NAME || ''), undefined, { numeric: true, sensitivity: 'base' });
            });
            groups.forEach(function (item, index) {
                var groupCode = item.grouP_CODE;
                var groupName = item.grouP_NAME || '';
                var groupNameHtml = empr_SalesQutation.EscapeHtml(groupName);
                var cardHtml = `
                    <button type="button" class="sq-group${index === 0 ? ' active' : ''}" data-item="${groupCode}" data-name="${groupNameHtml}">
                        ${empr_SalesQutation.ImageHtml(item.ipic, groupName, true)}
                        <div>
                            <div class="sq-group-name">${groupNameHtml}</div>
                            <div class="sq-group-count" data-count-for="${groupCode}"></div>
                        </div>
                    </button>
                `;
                $list.append(cardHtml);
                if (index === 0) {
                    empr_SalesQutation.SelectGroup(groupCode, groupName);
                }
            });
        } else {
            $list.html('<p class="sq-empty">No item groups to display.</p>');
            $('#sqItemList').html('<p class="sq-empty">No items to display.</p>');
        }
    },

    SelectGroup: function (groupId, groupName) {
        empr_SalesQutation.selectedGroupId = groupId;
        empr_SalesQutation.selectedGroupName = groupName || '';
        $('.sq-group').removeClass('active');
        $('.sq-group[data-item="' + groupId + '"]').addClass('active');
        $('#sqItemListTitle').text(groupName ? ('Item List – ' + groupName) : 'Item List');
        empr_SalesQutation.loadItemsForGroup(groupId);
    },

    loadItemsForGroup: function (groupId) {
        var applyItems = function (data) {
            if (!$.isArray(data)) {
                data = [];
            }
            data = data.slice().sort(function (a, b) {
                return String(a.iteM_NAME || '').localeCompare(String(b.iteM_NAME || ''), undefined, { numeric: true, sensitivity: 'base' });
            });
            empr_SalesQutation.groupItemsCache[groupId] = data;
            var $list = $('#sqItemList');
            $list.empty();
            var items = data || [];
            if (items.length > 0) {
                var itemsHtml = $.map(items, function (item) {
                    var values = empr_SalesQutation.GetRateBarcodeForItem(item.iteM_CODE);
                    var selected = empr_SalesQutation.GetSelectedItem(item.iteM_CODE);
                    var qty = selected ? selected.qty : 0;
                    var rateValue = empr_SalesQutation.FormatRateDisplay(values.rate);
                    var barcodeValue = values.barcode || '';
                    var stockQty = empr_SalesQutation.GetStockQty(item);
                    var itemName = item.iteM_NAME || '';
                    var itemCode = item.iteM_ID || '';
                    return `
                        <div class="sq-item-card${qty > 0 ? ' selected' : ''}" data-item="${item.iteM_CODE}" data-itemid="${empr_SalesQutation.EscapeHtml(itemCode)}" data-itemname="${empr_SalesQutation.EscapeHtml(itemName)}" data-itemcode="${empr_SalesQutation.EscapeHtml(itemCode)}" data-rate="${empr_SalesQutation.EscapeHtml(rateValue)}">
                            <div class="sq-qty-badge">
                                <button type="button" class="sq-qty-minus" title="Decrease">-</button>
                                <span class="sq-qty-value">${qty}</span>
                            </div>
                            ${empr_SalesQutation.ImageHtml(item.ipic, item.iteM_NAME, false)}
                            <div class="sq-item-body">
                                <p class="sq-item-name">${empr_SalesQutation.EscapeHtml(itemName)}</p>
                                <div class="sq-item-rate sq-rate-field">
                                    <label>Rate</label>
                                    <input type="number" class="sq-rate-input" value="${rateValue}" min="0" step="any">
                                </div>
                                <div class="sq-item-barcode sq-barcode-field">
                                    <label>Barcode</label>
                                    <input type="text" class="sq-barcode-input" value="${empr_SalesQutation.EscapeHtml(barcodeValue)}" autocomplete="off">
                                </div>
                                <div class="sq-item-stock${stockQty < 0 ? ' sq-item-stock-negative' : ''}">Stock Qty: <span class="sq-item-stock-value">${stockQty}</span></div>
                            </div>
                        </div>
                    `;
                });
                $list.append('<div class="sq-item-grid">' + itemsHtml.join('') + '</div>');
                $('.sq-group-count[data-count-for="' + groupId + '"]').text('(' + items.length + ' items)');
                empr_SalesQutation.FilterVisibleItems();
            } else {
                $list.html('<p class="sq-empty">No items available for this group.</p>');
                $('.sq-group-count[data-count-for="' + groupId + '"]').text('(0 items)');
            }
        };
        if (empr_SalesQutation.groupItemsCache.hasOwnProperty(groupId)) {
            applyItems(empr_SalesQutation.groupItemsCache[groupId]);
            return;
        }
        ajaxHelper.ajaxGetJson('/SalesQutation/GetItemsMasterByGroup?groupId=' + groupId, applyItems, false, true);
    },
    GetCurrentStock: function () {
        ajaxHelper.ajaxGetJson('/PurchaseBill/GetCurrentStock', function (data) {
            debugger;
            if (data.length > 0) {
                empr_SalesQutation.CurrentStock = data;
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
            empr_SalesQutation.RefreshVisibleStockQty();
        }, false, true);
    },

    GetStockQty: function (item) {
        var stocks = empr_SalesQutation.CurrentStock || [];
        var itemKey = item && item.iteM_CODE != null ? item.iteM_CODE : item;
        var itemId = item && item.iteM_ID != null ? item.iteM_ID : '';
        var stockRecord = stocks.find(function (s) {
            var stockItemId = s.itemId != null ? s.itemId : s.ItemId;
            return stockItemId == itemKey || (itemId !== '' && stockItemId == itemId);
        });
        if (!stockRecord) {
            return 0;
        }
        var balance = stockRecord.balance != null ? stockRecord.balance : stockRecord.Balance;
        var qty = parseFloat(balance);
        return isNaN(qty) ? 0 : qty;
    },

    RefreshVisibleStockQty: function () {
        $('#sqItemList .sq-item-card').each(function () {
            var $card = $(this);
            var qty = empr_SalesQutation.GetStockQty({
                iteM_CODE: $card.data('item'),
                iteM_ID: $card.attr('data-itemid')
            });
            $card.find('.sq-item-stock-value').text(qty);
            $card.find('.sq-item-stock').toggleClass('sq-item-stock-negative', qty < 0);
        });
    },

    FilterVisibleItems: function () {
        var term = $.trim($('#sqItemSearch').val() || '').toLowerCase();
        var $cards = $('#sqItemList .sq-item-card');
        if (!$cards.length) {
            return;
        }
        if (!term) {
            $cards.removeClass('sq-item-hidden');
            return;
        }
        $cards.each(function () {
            var $card = $(this);
            var name = String($card.attr('data-itemname') || '').toLowerCase();
            var code = String($card.attr('data-itemcode') || '').toLowerCase();
            var itemKey = String($card.attr('data-item') || '').toLowerCase();
            var match = name.indexOf(term) !== -1 || code.indexOf(term) !== -1 || itemKey.indexOf(term) !== -1;
            $card.toggleClass('sq-item-hidden', !match);
        });
    },

    ItemMatchesTerm: function (item, term) {
        if (!term) {
            return true;
        }
        var name = String(item.iteM_NAME || '').toLowerCase();
        var code = String(item.iteM_ID || '').toLowerCase();
        var itemKey = String(item.iteM_CODE || '').toLowerCase();
        return name.indexOf(term) !== -1 || code.indexOf(term) !== -1 || itemKey.indexOf(term) !== -1;
    },

    SearchItemsAcrossGroups: function () {
        var term = $.trim($('#sqItemSearch').val() || '').toLowerCase();
        var token = ++empr_SalesQutation.searchToken;
        if (!term) {
            empr_SalesQutation.FilterVisibleItems();
            return;
        }
        empr_SalesQutation.EnsureGroupItemsLoaded(function () {
            if (token !== empr_SalesQutation.searchToken) {
                return;
            }
            empr_SalesQutation.ActivateMatchingSearchGroup(term);
        });
    },

    EnsureGroupItemsLoaded: function (done) {
        var allCached = true;
        $.each(ItemsGroup || [], function (index, group) {
            if (!empr_SalesQutation.groupItemsCache.hasOwnProperty(group.grouP_CODE)) {
                allCached = false;
                return false;
            }
        });
        if (allCached) {
            if (done) {
                done();
            }
            return;
        }
        if (done) {
            empr_SalesQutation.searchPrefetchCallbacks.push(done);
        }
        if (empr_SalesQutation.searchPrefetchStarted) {
            return;
        }
        empr_SalesQutation.searchPrefetchStarted = true;
        var pending = 0;
        var finish = function () {
            var callbacks = empr_SalesQutation.searchPrefetchCallbacks.slice();
            empr_SalesQutation.searchPrefetchCallbacks = [];
            $.each(callbacks, function (index, callback) {
                callback();
            });
        };
        $.each(ItemsGroup || [], function (index, group) {
            var groupId = group.grouP_CODE;
            if (empr_SalesQutation.groupItemsCache.hasOwnProperty(groupId)) {
                return;
            }
            pending += 1;
            ajaxHelper.ajaxGetJson('/SalesQutation/GetItemsMasterByGroup?groupId=' + groupId, function (data) {
                if (!$.isArray(data)) {
                    data = [];
                }
                data = data.slice().sort(function (a, b) {
                    return String(a.iteM_NAME || '').localeCompare(String(b.iteM_NAME || ''), undefined, { numeric: true, sensitivity: 'base' });
                });
                empr_SalesQutation.groupItemsCache[groupId] = data;
                pending -= 1;
                if (pending === 0) {
                    finish();
                }
            }, false, true);
        });
        if (pending === 0) {
            finish();
        }
    },

    ActivateMatchingSearchGroup: function (term) {
        var currentGroupId = empr_SalesQutation.selectedGroupId;
        var currentGroupMatches = false;
        var targetGroup = null;
        var groups = (ItemsGroup || []).slice().sort(function (a, b) {
            return String(a.grouP_NAME || '').localeCompare(String(b.grouP_NAME || ''), undefined, { numeric: true, sensitivity: 'base' });
        });
        $.each(groups, function (index, group) {
            var items = empr_SalesQutation.groupItemsCache[group.grouP_CODE] || [];
            var hasMatch = false;
            $.each(items, function (itemIndex, item) {
                if (empr_SalesQutation.ItemMatchesTerm(item, term)) {
                    hasMatch = true;
                    return false;
                }
            });
            if (!hasMatch) {
                return;
            }
            if (group.grouP_CODE == currentGroupId) {
                currentGroupMatches = true;
            }
            if (targetGroup == null) {
                targetGroup = group;
            }
        });
        if (currentGroupMatches || !targetGroup) {
            empr_SalesQutation.FilterVisibleItems();
            return;
        }
        empr_SalesQutation.SelectGroup(targetGroup.grouP_CODE, targetGroup.grouP_NAME || '');
        var $groupBtn = $('.sq-group[data-item="' + targetGroup.grouP_CODE + '"]');
        if ($groupBtn.length && $groupBtn[0].scrollIntoView) {
            $groupBtn[0].scrollIntoView({ block: 'nearest', inline: 'nearest' });
        }
    },
    HighlightRelatedGroups: function () {
        var token = ++empr_SalesQutation.highlightToken;
        $('.sq-group').removeClass('has-data');
        var selectedCodes = {};
        var firstItemCode = null;
        $.each(empr_SalesQutation.selectedItems, function (index, item) {
            if (item.itemCode != null && item.itemCode !== '') {
                selectedCodes[String(item.itemCode)] = true;
                if (firstItemCode == null) {
                    firstItemCode = String(item.itemCode);
                }
            }
        });
        if ($.isEmptyObject(selectedCodes)) {
            return;
        }
        var groupOpened = false;
        var markGroupIfRelated = function (groupId, items, groupName) {
            if (token !== empr_SalesQutation.highlightToken) {
                return;
            }
            var hasRelatedItem = false;
            var hasFirstItem = false;
            $.each(items || [], function (index, item) {
                if (selectedCodes[String(item.iteM_CODE)]) {
                    hasRelatedItem = true;
                    if (firstItemCode != null && String(item.iteM_CODE) === firstItemCode) {
                        hasFirstItem = true;
                    }
                }
            });
            if (hasRelatedItem) {
                $('.sq-group[data-item="' + groupId + '"]').addClass('has-data');
            }
            if (hasFirstItem && !groupOpened) {
                groupOpened = true;
                empr_SalesQutation.SelectGroup(groupId, groupName);
                var $groupBtn = $('.sq-group[data-item="' + groupId + '"]');
                if ($groupBtn.length && $groupBtn[0].scrollIntoView) {
                    $groupBtn[0].scrollIntoView({ block: 'nearest' });
                }
            }
        };
        $.each(ItemsGroup || [], function (index, group) {
            var groupId = group.grouP_CODE;
            var groupName = group.grouP_NAME || '';
            if (empr_SalesQutation.groupItemsCache.hasOwnProperty(groupId)) {
                markGroupIfRelated(groupId, empr_SalesQutation.groupItemsCache[groupId], groupName);
                return;
            }
            ajaxHelper.ajaxGetJson('/SalesQutation/GetItemsMasterByGroup?groupId=' + groupId, function (data) {
                if (!$.isArray(data)) {
                    data = [];
                }
                empr_SalesQutation.groupItemsCache[groupId] = data;
                markGroupIfRelated(groupId, data, groupName);
            }, false, true);
        });
    },

    EscapeHtml: function (text) {
        if (text == null || text === undefined) {
            return '';
        }
        return $('<div/>').text(text).html();
    },

    ImageHtml: function (src, alt, isGroup) {
        var css = isGroup ? 'sq-group-thumb' : 'sq-item-thumb';
        var icon = isGroup ? 'fa-th-large' : 'fa-cube';
        var safeAlt = empr_SalesQutation.EscapeHtml(alt || '');
        if (src && src !== 'null' && src !== 'undefined') {
            return `<div class="${css}">
                <img src="${src}" alt="${safeAlt}" onerror="this.onerror=null;this.style.display='none';this.nextElementSibling.style.display='flex';">
                <div class="sq-thumb-fallback"><i class="fa ${icon}"></i></div>
            </div>`;
        }
        return `<div class="${css}"><div class="sq-thumb-fallback" style="display:flex"><i class="fa ${icon}"></i></div></div>`;
    },

    GetSelectedItem: function (itemCode) {
        return empr_SalesQutation.selectedItems.find(function (item) {
            return item.itemCode == itemCode;
        });
    },

    IncreaseItemQty: function (itemCode, rate, barcode) {
        if (itemCode == null || itemCode === '') {
            return;
        }
        var parsedRate = empr_SalesQutation.ParseRateValue(rate);
        if (barcode === undefined) {
            var $card = $('.sq-item-card[data-item="' + itemCode + '"]');
            barcode = $card.find('.sq-barcode-input').val() || '';
        }
        barcode = barcode != null ? String(barcode) : '';
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (existing) {
            existing.qty = (parseFloat(existing.qty) || 0) + 1;
            existing.rate = parsedRate;
            existing.barcode = barcode;
        } else {
            if (Limit != 0 && empr_SalesQutation.selectedItems.length >= Limit) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            empr_SalesQutation.selectedItems.push({
                itemCode: itemCode,
                rate: parsedRate,
                barcode: barcode,
                qty: 1
            });
        }
        empr_SalesQutation.RefreshCardQty(itemCode);
        empr_SalesQutation.UpdateSummary();
    },

    ParseRateValue: function (rate) {
        if (rate === null || rate === undefined || (typeof rate === 'string' && $.trim(rate) === '')) {
            return '';
        }
        var parsed = parseFloat(rate);
        if (isNaN(parsed) || parsed < 0) {
            return '';
        }
        return parsed;
    },

    FormatRateDisplay: function (rate) {
        var parsed = empr_SalesQutation.ParseRateValue(rate);
        if (parsed === '') {
            return '';
        }
        return parsed.toFixed(2);
    },

    GetPartyLastItem: function (itemCode) {
        var data = empr_SalesQutation.partyLastItemData || [];
        return data.find(function (item) {
            var code = item.iteM_CODE != null ? item.iteM_CODE : item.item_CODE;
            return code == itemCode;
        });
    },

    GetRateBarcodeForItem: function (itemCode) {
        var selected = empr_SalesQutation.GetSelectedItem(itemCode);
        if (selected) {
            return {
                rate: selected.rate,
                barcode: selected.barcode || ''
            };
        }
        var last = empr_SalesQutation.GetPartyLastItem(itemCode);
        if (last) {
            return {
                rate: last.rate,
                barcode: last.barcode || ''
            };
        }
        return {
            rate: '',
            barcode: ''
        };
    },

    LoadPartyLastItemData: function () {
        var partyCode = $('#partyhidden').val();
        var actCode = $('#acthidden').val();
        if (!partyCode) {
            empr_SalesQutation.partyLastItemData = [];
            empr_SalesQutation.ApplyPartyLastItemData();
            return;
        }
        ajaxHelper.ajaxGetJson('/SalesQutation/GetPartyLastItemRates?partyCode=' + partyCode + '&actCode=' + (actCode || 0), function (data) {
            if (data && data.msgType == 1) {
                empr_SalesQutation.partyLastItemData = data.data || [];
            }
            else {
                empr_SalesQutation.partyLastItemData = [];
            }
            empr_SalesQutation.ApplyPartyLastItemData();
        }, false, true);
    },

    ApplyPartyLastItemData: function () {
        $('#sqItemList .sq-item-card').each(function () {
            var $card = $(this);
            var itemCode = $card.data('item');
            var values = empr_SalesQutation.GetRateBarcodeForItem(itemCode);
            var rateValue = empr_SalesQutation.FormatRateDisplay(values.rate);
            $card.attr('data-rate', rateValue);
            $card.data('rate', rateValue);
            $card.find('.sq-rate-input').val(rateValue);
            $card.find('.sq-barcode-input').val(values.barcode || '');
        });
    },

    UpdateItemRate: function (itemCode, rate) {
        var $card = $('.sq-item-card[data-item="' + itemCode + '"]');
        if (rate === null || rate === undefined || (typeof rate === 'string' && $.trim(rate) === '')) {
            $card.attr('data-rate', '');
            $card.data('rate', '');
            var selectedEmpty = empr_SalesQutation.GetSelectedItem(itemCode);
            if (selectedEmpty) {
                selectedEmpty.rate = '';
                empr_SalesQutation.UpdateSummary();
            }
            return;
        }
        rate = parseFloat(rate);
        if (isNaN(rate) || rate < 0) {
            empr_helper.notify("Please enter correct item rate.", 2);
            var selected = empr_SalesQutation.GetSelectedItem(itemCode);
            var fallback = selected ? selected.rate : '';
            $card.find('.sq-rate-input').val(empr_SalesQutation.FormatRateDisplay(fallback));
            return;
        }
        $card.attr('data-rate', rate);
        $card.data('rate', rate);
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (existing) {
            existing.rate = rate;
            empr_SalesQutation.UpdateSummary();
        }
    },

    UpdateItemBarcode: function (itemCode, barcode) {
        barcode = barcode != null ? String(barcode) : '';
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (existing) {
            existing.barcode = barcode;
        }
    },

    DecreaseItemQty: function (itemCode) {
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (!existing) {
            return;
        }
        existing.qty = (parseFloat(existing.qty) || 0) - 1;
        if (existing.qty <= 0) {
            empr_SalesQutation.selectedItems = empr_SalesQutation.selectedItems.filter(function (item) {
                return item.itemCode != itemCode;
            });
        }
        empr_SalesQutation.RefreshCardQty(itemCode);
        empr_SalesQutation.UpdateSummary();
    },

    RefreshCardQty: function (itemCode) {
        var $card = $('.sq-item-card[data-item="' + itemCode + '"]');
        if (!$card.length) {
            return;
        }
        var selected = empr_SalesQutation.GetSelectedItem(itemCode);
        var qty = selected ? selected.qty : 0;
        $card.find('.sq-qty-value').text(qty);
        if (qty > 0) {
            $card.addClass('selected');
        } else {
            $card.removeClass('selected');
        }
    },

    ApplySelectedQuantities: function () {
        $('.sq-item-card').each(function () {
            var itemCode = $(this).data('item');
            empr_SalesQutation.RefreshCardQty(itemCode);
        });
    },

    LoadSelectedItemsFromDetail: function (detailRows) {
        empr_SalesQutation.selectedItems = [];
        if (!$.isArray(detailRows)) {
            detailRows = [];
        }
        $.each(detailRows || [], function (index, item) {
            if (!item.iteM_CODE) {
                return;
            }
            var qty = parseFloat(item.qty);
            if (isNaN(qty) || qty <= 0) {
                return;
            }
            var rate = empr_SalesQutation.ParseRateValue(item.rate);
            empr_SalesQutation.selectedItems.push({
                itemCode: item.iteM_CODE,
                rate: rate,
                barcode: item.barcode || '',
                qty: qty,
                dT_CODE: item.dT_CODE
            });
        });
        empr_SalesQutation.ApplySelectedQuantities();
        empr_SalesQutation.UpdateSummary();
        empr_SalesQutation.HighlightRelatedGroups();
    },

    UpdateSummary: function () {
        var totalItems = 0;
        var totalAmount = 0;
        $.each(empr_SalesQutation.selectedItems, function (index, item) {
            var qty = parseFloat(item.qty) || 0;
            var rate = parseFloat(item.rate) || 0;
            if (qty > 0) {
                totalItems += 1;
                totalAmount += qty * rate;
            }
        });
        $('#sqSummary').text('Total Items: ' + totalItems + ' | Total Amount: ' + totalAmount.toFixed(2));
        $('#sqMobileTotalItems').text(totalItems);
        $('#sqMobileTotalAmount').text(totalAmount.toFixed(2));
        $('#sqCartTotalItems').text(totalItems);
        $('#sqCartTotalAmount').text(totalAmount.toFixed(2));
    },

    GetItemName: function (itemCode) {
        var $card = $('.sq-item-card[data-item="' + itemCode + '"]');
        if ($card.length) {
            var cardName = $card.attr('data-itemname');
            if (cardName) {
                return cardName;
            }
        }
        var foundName = '';
        $.each(empr_SalesQutation.groupItemsCache || {}, function (groupId, items) {
            if (foundName) {
                return;
            }
            $.each(items || [], function (index, item) {
                if (item.iteM_CODE == itemCode) {
                    foundName = item.iteM_NAME || '';
                    return false;
                }
            });
        });
        if (foundName) {
            return foundName;
        }
        if (typeof Items !== 'undefined' && Items && Items.length) {
            var dropItem = $.grep(Items, function (item) {
                return item.key == itemCode;
            })[0];
            if (dropItem && dropItem.value) {
                return dropItem.value;
            }
        }
        return itemCode != null ? String(itemCode) : '';
    },

    RenderCartItems: function () {
        var $body = $('#sqCartBody');
        var items = (empr_SalesQutation.selectedItems || []).filter(function (item) {
            return item.itemCode && parseFloat(item.qty) > 0;
        });
        if (!items.length) {
            $body.html('<p class="sq-empty sq-cart-empty">No items in cart.</p>');
            return;
        }
        var html = $.map(items, function (item) {
            var qty = parseFloat(item.qty) || 0;
            var rate = parseFloat(item.rate) || 0;
            var amount = qty * rate;
            var name = empr_SalesQutation.GetItemName(item.itemCode);
            return `
                <div class="sq-cart-row">
                    <div class="sq-cart-row-info">
                        <div class="sq-cart-item-name">${empr_SalesQutation.EscapeHtml(name)}</div>
                        <div class="sq-cart-item-meta">
                            <span>Rate: ${rate.toFixed(2)}</span>
                            <span>Qty: ${qty}</span>
                            <span>Amount: ${amount.toFixed(2)}</span>
                        </div>
                    </div>
                    <button type="button" class="btn btn-outline-danger btn-sm sq-cart-remove" data-item="${item.itemCode}">Remove</button>
                </div>
            `;
        });
        $body.html(html.join(''));
    },

    RemoveCartItem: function (itemCode) {
        if (itemCode == null || itemCode === '') {
            return;
        }
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (!existing) {
            return;
        }
        empr_SalesQutation.selectedItems = empr_SalesQutation.selectedItems.filter(function (item) {
            return item.itemCode != itemCode;
        });
        empr_SalesQutation.RefreshCardQty(itemCode);
        empr_SalesQutation.UpdateSummary();
        empr_SalesQutation.RenderCartItems();
    },

    InitPartyType: function () {
        empr_SalesQutation.bindDxDdl("PARTY_CODE", PartyType, null, "key", "value", "Select", function (d) {
            if (d.value == null || d.value == '') {
                $('#partyhidden').val('');
                $('#acthidden').val('');
            }
            else {
                var filteredData = $.grep(PartyType, function (item) {
                    return item.key === d.value;
                });
                if (filteredData.length > 0) {
                    $('#partyhidden').val(filteredData[0].partyCode);
                    $('#acthidden').val(filteredData[0].accountCode);
                    empr_SalesQutation.SetPartyDropdownError(false);
                }
            }
            empr_SalesQutation.UpdatePartyNameDisplay();
            if (empr_SalesQutation.excelState.fileName) {
                empr_SalesQutation.RevalidateLoadedExcel();
            }
        });
        empr_SalesQutation.UpdatePartyNameDisplay();
    },

    UpdatePartyNameDisplay: function () {
        var partyName = '';
        var partyBox = $('#PARTY_CODE').dxSelectBox('instance');
        if (partyBox) {
            var value = partyBox.option('value');
            if (value != null && value !== '') {
                var filteredData = $.grep(PartyType || [], function (item) {
                    return item.key === value;
                });
                if (filteredData.length > 0) {
                    partyName = filteredData[0].value || '';
                }
            }
        }
        $('#PARTY_NAME_DISPLAY').val(partyName);
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {
        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);
    },

    InitQuickSearchGrid: function () {
        empr_SalesQutation.GetSalesQutations();
    },

    GetSalesQutations: function () {
        ajaxHelper.ajaxGetJson('/SalesQutation/GetSalesQutations', function (data) {
            if (data.msgType == 1) {
                empr_SalesQutation.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid: function (dataSrc) {
        var col = [{
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
            cellTemplate: function (container, options) {
                    $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                           </div>`).appendTo(container);
                }
            },
            { dataField: 'id', caption: 'Code', },
            { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            { dataField: 'voucheR_NO', caption: 'Transaction #', },
            { dataField: 'partY_NAME', caption: 'Party Name', },
            { dataField: 'remarks', caption: 'Remarks', },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc || [], "SalesQutationQS");
        setTimeout(function () {
            var grid = $('#gridContainer').dxDataGrid('instance');
            if (grid) {
                grid.resize();
            }
        }, 500);
    },

    GetSalesQutationByCode: function (code) {
        $('#SaveQuotationModal').modal('hide');
        ajaxHelper.ajaxGetJson('/SalesQutation/GetSalesQutationByCode?code=' + code, function (data) {
            if (!data || !data.master) {
                empr_helper.notify(typeof data === 'string' ? data : 'Unable to load record.', 2);
                return;
            }
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if ($.isArray(masterData) && masterData.length == 1) {
                    var response = masterData[0];
                    var actCode = response.acT_CODE != null ? String(response.acT_CODE) : '';
                    var filteredData = $.grep(PartyType || [], function (item) {
                        return item.partyCode == response.partY_CODE && String(item.accountCode) === actCode;
                    });
                    empr_helper.selectedBill = response.id;
                    $('#Code').val(response.id);
                    var astatusBox = $('#ASTATUS').dxSelectBox('instance');
                    if (astatusBox) {
                        astatusBox.option('value', response.astatus);
                    }
                    if (filteredData.length > 0) {
                        var partyBox = $('#PARTY_CODE').dxSelectBox('instance');
                        if (partyBox) {
                            partyBox.option('value', filteredData[0].key);
                        }
                        $('#partyhidden').val(filteredData[0].partyCode);
                        $('#acthidden').val(filteredData[0].accountCode);
                    }
                    empr_SalesQutation.UpdatePartyNameDisplay();
                    empr_SalesQutation.LoadPartyLastItemData();
                    $('#REMARKS').val(response.remarks);
                    $('#V_DATE').val(response.v_DATE);
                    $('#VOUCHER_NO').val(response.voucheR_NO);

                    if (Permissions != "Admin") {
                        if (Permissions.r_DLT) {
                            $('#BtnDelete').show();
                        }
                        if (Permissions.r_EDIT) {
                            $('#BtnSave').show();
                        }
                        else {
                            $('#BtnSave').hide();
                        }
                    } else {
                        $('#BtnSave').show();
                        $('#BtnDelete').show();
                    }
                    $('#BtnPrint').show();
                }

                if (data.detail && data.detail.msgType == 1) {
                    empr_SalesQutation.LoadSelectedItemsFromDetail(data.detail.data);
                    $('.Record').addClass('customHighlightForModifiedCells');
                }
                else if (data.detail) {
                    empr_helper.notify(data.detail.msg || data.msg, data.detail.msgType || data.msgType);
                }
            }
            else {
                empr_helper.notify(data.master.msg || data.msg, data.master.msgType);
            }
        }, false, true);
    },

    GetSalesQutationDetailByCode: function (code) {
        ajaxHelper.ajaxGetJson('/SalesQutation/GetSalesQutationDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_SalesQutation.LoadSelectedItemsFromDetail(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GeneratePrintReport: function () {
        let TRAN_ID = empr_helper.selectedBill || $('#Code').val();
        let MD_ID = null;
        var reportType = $('#ReportType').data('dxSelectBox');
        if (reportType) {
            MD_ID = reportType.option('value');
        }
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the bill in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/SalesQutation/GetPrintReport", function (data) {
            if (data.msgType == 1) {
                $('#ModalBody').empty();
                setTimeout(function () {
                    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                    $('#ShowReportModal').show();
                    $('#ShowReportModal').modal('show');
                }, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitReportTypeDDL: function (selectedValue) {
        ajaxHelper.ajaxGetJson("/SalesQutation/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data && data.data.length > 0) {
                    selectedValue = data.data[0].mD_ID;
                }
                $('#ReportType').dxSelectBox({
                    dataSource: data.data,
                    displayExpr: 'mD_NAME',
                    valueExpr: 'mD_ID',
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
                    onValueChanged: function (e) {
                    },
                });
            }
        }, false, true);
    },

    GetDataToSave: function () {
        var ID = $("#Code").val();
        var V_DATE = $("#V_DATE").val();
        var VOUCHER_NO = $("#VOUCHER_NO").val();
        var PARTY_CODE = $("#partyhidden").val();
        var ACT_CODE = $("#acthidden").val();
        var REMARKS = $("#REMARKS").val();
        var ASTATUS = 'Y';
        var astatusBox = $('#ASTATUS').dxSelectBox('instance');
        if (astatusBox) {
            ASTATUS = astatusBox.option('value');
        }
        var masterRecord = {
            TRAN_ID: ID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            REMARKS: REMARKS,
            ASTATUS: ASTATUS
        };
        var detailRecords = [];
        $.each(empr_SalesQutation.selectedItems, function (index, item) {
            if (!item.itemCode || !(parseFloat(item.qty) > 0)) {
                return;
            }
            detailRecords.push({
                iteM_CODE: item.itemCode,
                qty: item.qty,
                rate: item.rate,
                barcode: item.barcode || '',
                dT_CODE: item.dT_CODE
            });
        });
        return {
            Master: masterRecord,
            Detail: detailRecords
        };
    },

    ValidateDetails: function () {
        var valid = true;
        var data = empr_SalesQutation.selectedItems.filter(function (item) {
            return item.itemCode && parseFloat(item.qty) > 0;
        });
        if (data.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }
        $.each(data, function (index, item) {
            if (item.qty === "" || item.qty == null || item.qty === undefined) {
                empr_helper.notify("Please enter item quantity at index " + index, 2);
                valid = false;
                return valid;
            }
            if (item.qty <= 0) {
                empr_helper.notify("Please enter correct item quantity at index " + index, 2);
                valid = false;
                return valid;
            }
            var rateVal = parseFloat(item.rate);
            if (item.rate === null || item.rate === undefined || (typeof item.rate === 'string' && $.trim(item.rate) === '') || isNaN(rateVal) || rateVal < 0) {
                empr_helper.notify("Please enter correct item rate at index " + index, 2);
                valid = false;
                return valid;
            }
        });
        return valid;
    },

    ValidatePartySelection: function () {
        var partyCode = $("#partyhidden").val();
        if (partyCode == null || partyCode === '') {
            empr_helper.notify("Please select Party Type.", 2);
            empr_SalesQutation.SetPartyDropdownError(true);
            return false;
        }
        empr_SalesQutation.SetPartyDropdownError(false);
        return true;
    },

    SetPartyDropdownError: function (isInvalid) {
        $('#PARTY_CODE').toggleClass('sq-party-invalid', !!isInvalid);
    },

    ValidateMainInfo: function () {
        var valid = true;
        var data = empr_SalesQutation.GetDataToSave();

        if (!data.Master.V_DATE) {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }

        if (!empr_SalesQutation.ValidatePartySelection()) {
            $('#SaveQuotationModal').modal('show');
            valid = false;
            return valid;
        }

        if (!empr_SalesQutation.ValidateDetails()) {
            valid = false;
            return valid;
        }

        return valid;
    },

    Save: function () {
        var dataModel = empr_SalesQutation.GetDataToSave();
        if (dataModel.Master.TRAN_ID == 0
            || dataModel.Master.TRAN_ID == null
            || dataModel.Master.TRAN_ID == undefined
            || dataModel.Master.TRAN_ID == "") {
            dataModel.Detail.reverse();
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/SalesQutation/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                $('#SaveQuotationModal').modal('hide');
                empr_SalesQutation.ResetForm();
                setTimeout(function () {
                    $('#SaveQuotationModal').modal('show');
                }, 2500);
            }
        }, false, true);
    },

    ResetForm: function () {
        $('#Code').val('');
        $('#ID').val('');
        empr_helper.selectedBill = '';
        $('#partyhidden').val('');
        $('#acthidden').val('');
        $('#PARTY_NAME_DISPLAY').val('');
        empr_SalesQutation.SetPartyDropdownError(false);
        $('#VOUCHER_NO').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        $('#BtnPrint').hide();
        $('.Record').removeClass('customHighlightForModifiedCells');
        empr_SalesQutation.selectedItems = [];
        empr_SalesQutation.partyLastItemData = [];
        empr_SalesQutation.ResetExcelUpload();
        empr_SalesQutation.highlightToken += 1;
        $('.sq-group').removeClass('has-data');
        if (empr_SalesQutation.selectedGroupId != null && empr_SalesQutation.selectedGroupId !== '') {
            empr_SalesQutation.loadItemsForGroup(empr_SalesQutation.selectedGroupId);
        } else {
            empr_SalesQutation.ApplySelectedQuantities();
        }
        empr_SalesQutation.UpdateSummary();
        empr_SalesQutation.InitPartyType();
        var partyBox = $('#PARTY_CODE').dxSelectBox('instance');
        if (partyBox) {
            partyBox.option('value', '');
        }
        $('#V_DATE').val(todayDate);
        var astatusBox = $('#ASTATUS').dxSelectBox('instance');
        if (astatusBox) {
            var defaultStatus = (typeof statusApp !== 'undefined' && statusApp) ? statusApp : 'Y';
            astatusBox.option('value', defaultStatus);
        }
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#BtnSave').show();
            } else {
                $('#BtnSave').hide();
            }
        } else {
            $('#BtnSave').show();
        }
    },

    Delete: function () {
        swal({
            title: 'Are you sure you want to remove this record?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#0CC27E',
            cancelButtonColor: '#FF586B',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            confirmButtonClass: 'btn btn-success mr-5',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function () {
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/SalesQutation/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_SalesQutation.ResetForm();
                    $('#BtnDelete').hide();
                    $('#SaveQuotationModal').modal('show');
                }
            }, false, true);
        });
    },

    ResetExcelUpload: function (keepStatus, keepPanel) {
        empr_SalesQutation.excelSaving = false;
        empr_SalesQutation.excelState = {
            fileName: '',
            branches: [],
            masters: [],
            failedRows: [],
            canComplete: false,
            rawRows: null
        };
        $('#sqExcelFile').val('');
        $('#sqExcelFileName').text('');
        $('#BtnExcelClear').hide();
        $('#BtnExcelComplete').hide();
        $('#sqExcelReview').hide().empty();
        $('#SaveQuotationModal').removeClass('sq-excel-review-open');
        $('#SaveQuotationModal .modal-dialog').removeClass('modal-xl').addClass('modal-md');
        if (!keepStatus) {
            empr_SalesQutation.SetExcelStatus('', '');
        }
        if (keepPanel) {
            empr_SalesQutation.RefreshExcelModalFooter();
        }
        else {
            empr_SalesQutation.HideExcelUploadPanel();
        }
    },

    ShowExcelUploadPanel: function () {
        var $panel = $('#sqExcelUpload');
        empr_SalesQutation.excelPanelOpen = true;
        if ($panel.is(':visible')) {
            empr_SalesQutation.RefreshExcelModalFooter();
            return;
        }
        $panel.stop(true, true).css({ overflow: 'hidden', opacity: 0 }).slideDown(320, function () {
            $panel.css('overflow', 'visible');
        }).animate({ opacity: 1 }, { duration: 320, queue: false });
        empr_SalesQutation.RefreshExcelModalFooter();
    },

    HideExcelUploadPanel: function () {
        var $panel = $('#sqExcelUpload');
        empr_SalesQutation.excelPanelOpen = false;
        if (!$panel.is(':visible')) {
            empr_SalesQutation.RefreshExcelModalFooter();
            return;
        }
        $panel.stop(true, true).animate({ opacity: 0 }, { duration: 220, queue: false }).slideUp(280, function () {
            $panel.css('opacity', '');
            empr_SalesQutation.RefreshExcelModalFooter();
        });
    },

    HasUploadedExcel: function () {
        return !!(empr_SalesQutation.excelState && empr_SalesQutation.excelState.fileName);
    },

    RefreshExcelModalFooter: function () {
        var hasFile = empr_SalesQutation.HasUploadedExcel();
        var canAdd = Permissions == "Admin" || (Permissions && Permissions.r_ADD);
        if (hasFile) {
            $('#BtnModalSave').hide();
            if (canAdd) {
                $('#BtnExcelComplete').show();
            }
            else {
                $('#BtnExcelComplete').hide();
            }
        }
        else {
            $('#BtnModalSave').show();
            $('#BtnExcelComplete').hide();
        }
        $('#SaveQuotationModal .modal-footer').show();
    },

    SetExcelStatus: function (message, type) {
        var $status = $('#sqExcelStatus');
        $status.removeClass('sq-excel-error sq-excel-success');
        if (type) {
            $status.addClass('sq-excel-' + type);
        }
        $status.text(message || '');
    },

    NormalizeExcelHeader: function (value) {
        return String(value == null ? '' : value).replace(/\s+/g, '').toLowerCase();
    },

    GetExcelCellValue: function (cell) {
        if (!cell || cell.value === null || cell.value === undefined) {
            return '';
        }
        var value = cell.value;
        if (typeof value === 'object') {
            if (value.result !== undefined && value.result !== null) {
                return value.result;
            }
            if (value.text !== undefined && value.text !== null) {
                return value.text;
            }
            if (value.richText && value.richText.length) {
                return value.richText.map(function (part) {
                    return part && part.text ? part.text : '';
                }).join('');
            }
            if (value.hyperlink) {
                return value.text || value.hyperlink;
            }
            return '';
        }
        return value;
    },

    TrimExcelValue: function (value) {
        if (value === null || value === undefined) {
            return '';
        }
        return $.trim(String(value));
    },

    ParseExcelNumber: function (value, allowEmpty) {
        var text = empr_SalesQutation.TrimExcelValue(value);
        if (text === '') {
            return allowEmpty ? { ok: true, value: null, empty: true } : { ok: false, value: null, empty: true };
        }
        text = text.replace(/,/g, '');
        var parsed = parseFloat(text);
        if (isNaN(parsed)) {
            return { ok: false, value: null, empty: false };
        }
        return { ok: true, value: parsed, empty: false };
    },

    FindExcelItem: function (itemCodeValue) {
        var code = empr_SalesQutation.TrimExcelValue(itemCodeValue);
        if (!code) {
            return null;
        }
        var lookup = empr_SalesQutation.excelItemLookup || [];
        var found = lookup.find(function (item) {
            return String(item.itemCode) === code || String(item.itemId || '').toLowerCase() === code.toLowerCase();
        });
        if (found) {
            return found;
        }
        if (typeof Items !== 'undefined' && Items && Items.length) {
            var dropItem = $.grep(Items, function (item) {
                return String(item.key) === code;
            })[0];
            if (dropItem) {
                return {
                    itemCode: dropItem.key,
                    itemId: dropItem.key,
                    itemName: dropItem.value
                };
            }
        }
        return null;
    },

    GetPartyBranchNames: function (done) {
        var partyCode = $('#partyhidden').val();
        var actCode = $('#acthidden').val() || 0;
        if (!partyCode) {
            done([]);
            return;
        }
        ajaxHelper.ajaxGetJson('/SalesQutation/GetPartyBranches?partyCode=' + partyCode + '&actCode=' + actCode, function (data) {
            if (data && data.msgType == 1) {
                done(data.data || []);
            }
            else {
                empr_helper.notify((data && data.msg) ? data.msg : "Unable to load party branches.", 2);
                done([]);
            }
        }, false, true);
    },

    EnsureExcelItemLookup: function (done) {
        if (empr_SalesQutation.excelItemLookup && empr_SalesQutation.excelItemLookup.length) {
            done();
            return;
        }
        ajaxHelper.ajaxGetJson('/SalesQutation/GetExcelItemLookup', function (data) {
            if (data && data.msgType == 1) {
                empr_SalesQutation.excelItemLookup = data.data || [];
            }
            else {
                empr_SalesQutation.excelItemLookup = [];
            }
            done();
        }, false, true);
    },

    ProcessExcelFile: function (file) {
        if (!file) {
            return;
        }
        if (!empr_SalesQutation.ValidatePartySelection()) {
            $('#sqExcelFile').val('');
            return;
        }
        var fileName = file.name || '';
        var ext = fileName.split('.').pop().toLowerCase();
        if (ext !== 'xlsx') {
            empr_SalesQutation.ResetExcelUpload(false, true);
            empr_SalesQutation.SetExcelStatus('Please select a valid Excel file (.xlsx).', 'error');
            return;
        }
        if (typeof ExcelJS === 'undefined') {
            empr_SalesQutation.SetExcelStatus('Excel reader is not available.', 'error');
            return;
        }

        empr_SalesQutation.excelState.fileName = fileName;
        $('#sqExcelFileName').text(fileName);
        $('#BtnExcelClear').show();
        $('#sqExcelReview').hide().empty();
        empr_SalesQutation.RefreshExcelModalFooter();
        empr_SalesQutation.SetExcelStatus('Reading Excel file...', '');

        var reader = new FileReader();
        reader.onload = function (e) {
            var workbook = new ExcelJS.Workbook();
            workbook.xlsx.load(e.target.result, { ignoreNodes: ['drawing', 'legacyDrawing'] }).then(function () {
                var parsed = empr_SalesQutation.ParseExcelWorkbook(workbook);
                if (!parsed.ok) {
                    empr_SalesQutation.excelState.rawRows = null;
                    empr_SalesQutation.excelState.masters = [];
                    empr_SalesQutation.excelState.failedRows = parsed.failedRows || [];
                    empr_SalesQutation.excelState.canComplete = false;
                    empr_SalesQutation.SetExcelModalSize(true);
                    empr_SalesQutation.RenderExcelReview();
                    empr_SalesQutation.SetExcelStatus(parsed.message || 'Excel validation failed.', 'error');
                    return;
                }
                empr_SalesQutation.excelState.rawRows = parsed;
                empr_SalesQutation.EnsureExcelItemLookup(function () {
                    empr_SalesQutation.GetPartyBranchNames(function (branches) {
                        empr_SalesQutation.ValidateExcelData(parsed, branches);
                    });
                });
            }).catch(function () {
                empr_SalesQutation.SetExcelStatus('Unable to read the Excel file. Please upload a valid .xlsx file.', 'error');
            });
        };
        reader.onerror = function () {
            empr_SalesQutation.SetExcelStatus('Unable to read the selected file.', 'error');
        };
        reader.readAsArrayBuffer(file);
    },

    RevalidateLoadedExcel: function () {
        if (!empr_SalesQutation.excelState.rawRows) {
            return;
        }
        if (!empr_SalesQutation.ValidatePartySelection()) {
            return;
        }
        empr_SalesQutation.SetExcelStatus('Validating Excel file...', '');
        empr_SalesQutation.EnsureExcelItemLookup(function () {
            empr_SalesQutation.GetPartyBranchNames(function (branches) {
                empr_SalesQutation.ValidateExcelData(empr_SalesQutation.excelState.rawRows, branches);
            });
        });
    },

    ParseExcelWorkbook: function (workbook) {
        var worksheet = workbook.worksheets && workbook.worksheets.length ? workbook.worksheets[0] : null;
        if (!worksheet) {
            return { ok: false, message: 'Excel file does not contain a worksheet.' };
        }

        var headerRow = worksheet.getRow(1);
        var headers = [];
        var maxCol = worksheet.columnCount || 0;
        headerRow.eachCell({ includeEmpty: false }, function (cell, colNumber) {
            if (colNumber > maxCol) {
                maxCol = colNumber;
            }
        });
        for (var col = 1; col <= maxCol; col++) {
            headers.push({
                col: col,
                text: empr_SalesQutation.TrimExcelValue(empr_SalesQutation.GetExcelCellValue(headerRow.getCell(col))),
                key: empr_SalesQutation.NormalizeExcelHeader(empr_SalesQutation.GetExcelCellValue(headerRow.getCell(col)))
            });
        }
        while (headers.length && !headers[headers.length - 1].key) {
            headers.pop();
        }
        if (!headers.length) {
            return { ok: false, message: 'Required Excel headers are missing.' };
        }

        var required = [
            { key: 'itemcode', label: 'ItemCode' },
            { key: 'items', label: 'Items' },
            { key: 'groupname', label: 'GroupName' },
            { key: 'salerate', label: 'SaleRate' },
            { key: 'doc', label: 'Doc' },
            { key: 'currentstock', label: 'CurrentStock' }
        ];
        var headerMap = {};
        var failedRows = [];
        $.each(headers, function (index, header) {
            if (header.key) {
                headerMap[header.key] = header;
            }
        });

        var missingHeaders = [];
        $.each(required, function (index, item) {
            if (!headerMap[item.key]) {
                missingHeaders.push(item.label);
            }
        });
        if (missingHeaders.length) {
            failedRows.push({
                rowNo: 1,
                itemCode: '',
                itemName: '',
                reason: 'Required Excel headers missing: ' + missingHeaders.join(', ') + '.'
            });
            return {
                ok: false,
                message: 'Required Excel headers missing: ' + missingHeaders.join(', ') + '.',
                failedRows: failedRows
            };
        }

        var requiredKeys = {};
        $.each(required, function (index, item) {
            requiredKeys[item.key] = true;
        });
        var branchHeaders = [];
        $.each(headers, function (index, header) {
            if (!header.key) {
                failedRows.push({
                    rowNo: 1,
                    itemCode: '',
                    itemName: '',
                    reason: 'Invalid Branch Qty column at column ' + header.col + '.'
                });
                return;
            }
            if (!requiredKeys[header.key]) {
                branchHeaders.push(header);
            }
        });
        if (!branchHeaders.length) {
            failedRows.push({
                rowNo: 1,
                itemCode: '',
                itemName: '',
                reason: 'At least one Branch Qty column is required.'
            });
            return {
                ok: false,
                message: 'At least one Branch Qty column is required.',
                failedRows: failedRows
            };
        }

        var rows = [];
        var rowCount = worksheet.rowCount || 0;
        for (var rowNo = 2; rowNo <= rowCount; rowNo++) {
            var row = worksheet.getRow(rowNo);
            var values = {};
            var hasAnyValue = false;
            $.each(headers, function (index, header) {
                var cellValue = empr_SalesQutation.GetExcelCellValue(row.getCell(header.col));
                values[header.key || ('col' + header.col)] = cellValue;
                if (empr_SalesQutation.TrimExcelValue(cellValue) !== '') {
                    hasAnyValue = true;
                }
            });
            if (!hasAnyValue) {
                continue;
            }
            rows.push({
                rowNo: rowNo,
                values: values
            });
        }

        if (!rows.length) {
            failedRows.push({
                rowNo: 2,
                itemCode: '',
                itemName: '',
                reason: 'No item rows found in the Excel file.'
            });
            return {
                ok: false,
                message: 'No item rows found in the Excel file.',
                failedRows: failedRows
            };
        }

        return {
            ok: true,
            headers: headers,
            headerMap: headerMap,
            branchHeaders: branchHeaders,
            rows: rows,
            failedRows: failedRows
        };
    },

    GetBranchColumnName: function (headerText) {
        var name = empr_SalesQutation.TrimExcelValue(headerText);
        name = name.replace(/\s*qty\s*$/i, '');
        return $.trim(name);
    },

    ValidateExcelData: function (parsed, partyBranches) {
        var failedRows = (parsed.failedRows || []).slice();
        var branchHeaders = parsed.branchHeaders || [];
        var validBranches = [];
        var partyBranchNames = (partyBranches || []).map(function (branch) {
            return empr_SalesQutation.TrimExcelValue(branch.branchName || branch.brancH_NAME);
        }).filter(function (name) {
            return name !== '';
        });

        $.each(branchHeaders, function (index, header) {
            var branchName = empr_SalesQutation.GetBranchColumnName(header.text);
            if (!branchName) {
                failedRows.push({
                    rowNo: 1,
                    itemCode: '',
                    itemName: header.text || '',
                    reason: 'Invalid Branch Qty column at column ' + header.col + '.'
                });
                return;
            }
            var matched = partyBranchNames.find(function (name) {
                return name.toLowerCase() === branchName.toLowerCase();
            });
            if (partyBranchNames.length && !matched) {
                failedRows.push({
                    rowNo: 1,
                    itemCode: '',
                    itemName: header.text,
                    reason: 'Branch Qty column "' + header.text + '" is not valid for the selected party.'
                });
                return;
            }
            validBranches.push({
                header: header,
                branchName: matched || branchName
            });
        });

        if (!validBranches.length) {
            empr_SalesQutation.excelState.masters = [];
            empr_SalesQutation.excelState.failedRows = failedRows;
            empr_SalesQutation.excelState.canComplete = false;
            empr_SalesQutation.SetExcelModalSize(true);
            empr_SalesQutation.RenderExcelReview();
            empr_SalesQutation.SetExcelStatus('No valid Branch Qty columns found.', 'error');
            return;
        }

        var seenItemCodes = {};
        var masters = validBranches.map(function (branch) {
            return {
                branchName: branch.branchName,
                headerKey: branch.header.key,
                items: [],
                totalQty: 0,
                totalAmount: 0
            };
        });

        $.each(parsed.rows, function (index, row) {
            var itemCodeValue = row.values.itemcode;
            var itemNameValue = empr_SalesQutation.TrimExcelValue(row.values.items);
            var reasons = [];
            var itemCodeText = empr_SalesQutation.TrimExcelValue(itemCodeValue);
            if (!itemCodeText) {
                reasons.push('ItemCode is required.');
            }
            var item = itemCodeText ? empr_SalesQutation.FindExcelItem(itemCodeText) : null;
            if (itemCodeText && !item) {
                reasons.push('ItemCode does not exist.');
            }
            if (itemCodeText) {
                var duplicateKey = item ? String(item.itemCode) : itemCodeText.toLowerCase();
                if (seenItemCodes[duplicateKey]) {
                    reasons.push('Duplicate ItemCode.');
                }
                else {
                    seenItemCodes[duplicateKey] = true;
                }
            }

            var rateParsed = empr_SalesQutation.ParseExcelNumber(row.values.salerate, false);
            if (!rateParsed.ok || rateParsed.value < 0) {
                reasons.push('SaleRate must be a valid numeric value.');
            }

            var qtyByBranch = {};
            $.each(validBranches, function (branchIndex, branch) {
                var qtyParsed = empr_SalesQutation.ParseExcelNumber(row.values[branch.header.key], true);
                if (!qtyParsed.ok) {
                    reasons.push(branch.branchName + ' Qty must be a valid numeric value.');
                    return;
                }
                if (qtyParsed.empty || qtyParsed.value === 0) {
                    qtyByBranch[branch.header.key] = null;
                    return;
                }
                if (qtyParsed.value < 0) {
                    reasons.push(branch.branchName + ' Qty must be a valid numeric value.');
                    return;
                }
                qtyByBranch[branch.header.key] = qtyParsed.value;
            });

            if (reasons.length) {
                failedRows.push({
                    rowNo: row.rowNo,
                    itemCode: itemCodeText,
                    itemName: itemNameValue || (item ? item.itemName : ''),
                    reason: reasons.join(' ')
                });
                return;
            }

            $.each(masters, function (masterIndex, master) {
                var qty = qtyByBranch[master.headerKey];
                if (qty == null || !(qty > 0)) {
                    return;
                }
                if (Limit != 0 && master.items.length >= Limit) {
                    failedRows.push({
                        rowNo: row.rowNo,
                        itemCode: itemCodeText,
                        itemName: itemNameValue || item.itemName,
                        reason: 'Item limit exceeded for ' + master.branchName + '.'
                    });
                    return;
                }
                var rate = rateParsed.value;
                master.items.push({
                    itemCode: item.itemCode,
                    itemId: item.itemId,
                    itemName: itemNameValue || item.itemName,
                    qty: qty,
                    rate: rate
                });
                master.totalQty += qty;
                master.totalAmount += qty * rate;
            });
        });

        masters = masters.filter(function (master) {
            return master.items.length > 0;
        });

        empr_SalesQutation.excelState.masters = masters;
        empr_SalesQutation.excelState.failedRows = failedRows;
        empr_SalesQutation.excelState.canComplete = masters.length > 0;
        empr_SalesQutation.SetExcelModalSize(true);
        empr_SalesQutation.RenderExcelReview();

        if (!masters.length) {
            empr_SalesQutation.SetExcelStatus('Excel validated. No Master Records are ready to save.', 'error');
            return;
        }
        var message = 'Excel validated. Review ' + masters.length + ' Master Record' + (masters.length > 1 ? 's' : '') + ' before Complete.';
        if (failedRows.length) {
            message += ' ' + failedRows.length + ' row(s) failed and will not be saved.';
        }
        empr_SalesQutation.SetExcelStatus(message, failedRows.length ? 'error' : 'success');
    },

    SetExcelModalSize: function (expanded) {
        var $modal = $('#SaveQuotationModal');
        var $dialog = $modal.find('.modal-dialog');
        if (expanded) {
            $modal.addClass('sq-excel-review-open');
            $dialog.removeClass('modal-md').addClass('modal-xl');
        }
        else {
            $modal.removeClass('sq-excel-review-open');
            $dialog.removeClass('modal-xl').addClass('modal-md');
        }
        var partyBox = $('#PARTY_CODE').dxSelectBox('instance');
        if (partyBox) {
            partyBox.repaint();
        }
    },

    RenderExcelReview: function () {
        var masters = empr_SalesQutation.excelState.masters || [];
        var failedRows = empr_SalesQutation.excelState.failedRows || [];
        var totalItems = 0;
        $.each(masters, function (index, master) {
            totalItems += master.items.length;
        });

        var html = '<div class="sq-excel-summary">';
        html += '<span>Master Records: <strong>' + masters.length + '</strong></span>';
        html += '<span>Valid Items: <strong>' + totalItems + '</strong></span>';
        html += '<span>Failed Rows: <strong>' + failedRows.length + '</strong></span>';
        html += '</div>';

        html += '<h6 class="sq-excel-section-title">Master Records</h6>';
        if (!masters.length) {
            html += '<p class="sq-empty">No Master Records to save.</p>';
        }
        else {
            $.each(masters, function (index, master) {
                html += '<div class="sq-excel-master">';
                html += '<div class="sq-excel-master-head">';
                html += '<span>' + empr_SalesQutation.EscapeHtml(master.branchName) + '</span>';
                html += '<span>Items: ' + master.items.length + ' | Qty: ' + master.totalQty + ' | Amount: ' + master.totalAmount.toFixed(2) + '</span>';
                html += '</div>';
                html += '<div class="sq-excel-table-wrap"><table class="table table-sm sq-excel-table"><thead><tr>';
                html += '<th>ItemCode</th><th>Items</th><th>Qty</th><th>SaleRate</th><th>Amount</th>';
                html += '</tr></thead><tbody>';
                $.each(master.items, function (itemIndex, item) {
                    var amount = (parseFloat(item.qty) || 0) * (parseFloat(item.rate) || 0);
                    html += '<tr>';
                    html += '<td>' + empr_SalesQutation.EscapeHtml(item.itemId || item.itemCode) + '</td>';
                    html += '<td>' + empr_SalesQutation.EscapeHtml(item.itemName) + '</td>';
                    html += '<td>' + item.qty + '</td>';
                    html += '<td>' + (parseFloat(item.rate) || 0).toFixed(2) + '</td>';
                    html += '<td>' + amount.toFixed(2) + '</td>';
                    html += '</tr>';
                });
                html += '</tbody></table></div></div>';
            });
        }

        html += '<h6 class="sq-excel-section-title">Failed Rows</h6>';
        if (!failedRows.length) {
            html += '<p class="sq-empty">No failed rows.</p>';
        }
        else {
            html += '<div class="sq-excel-table-wrap"><table class="table table-sm sq-excel-table sq-excel-failed"><thead><tr>';
            html += '<th>Excel Row</th><th>ItemCode</th><th>Items</th><th>Failed Reason</th>';
            html += '</tr></thead><tbody>';
            $.each(failedRows, function (index, row) {
                html += '<tr>';
                html += '<td>' + empr_SalesQutation.EscapeHtml(row.rowNo) + '</td>';
                html += '<td>' + empr_SalesQutation.EscapeHtml(row.itemCode) + '</td>';
                html += '<td>' + empr_SalesQutation.EscapeHtml(row.itemName) + '</td>';
                html += '<td class="sq-excel-reason">' + empr_SalesQutation.EscapeHtml(row.reason) + '</td>';
                html += '</tr>';
            });
            html += '</tbody></table></div>';
        }

        $('#sqExcelReview').html(html).show();
        empr_SalesQutation.RefreshExcelModalFooter();
    },

    GetExcelDataToSave: function () {
        var partyCode = parseInt($("#partyhidden").val(), 10);
        var actCode = parseInt($("#acthidden").val(), 10);
        if (isNaN(partyCode)) {
            partyCode = null;
        }
        if (isNaN(actCode)) {
            actCode = 0;
        }
        var vDate = $("#V_DATE").val();
        var remarks = $("#REMARKS").val() || '';
        var aStatus = 'Y';
        var astatusBox = $('#ASTATUS').dxSelectBox('instance');
        if (astatusBox) {
            aStatus = astatusBox.option('value') || 'Y';
        }
        var records = [];
        $.each(empr_SalesQutation.excelState.masters || [], function (index, master) {
            var masterRemarks = remarks;
            if (masterRemarks) {
                masterRemarks = masterRemarks + ' - ' + master.branchName;
            }
            else {
                masterRemarks = master.branchName;
            }
            var details = [];
            $.each(master.items, function (itemIndex, item) {
                details.push({
                    ITEM_CODE: item.itemCode,
                    QTY: item.qty,
                    RATE: item.rate,
                    BARCODE: ''
                });
            });
            if (!details.length) {
                return;
            }
            records.push({
                Master: {
                    TRAN_ID: null,
                    V_DATE: vDate,
                    VOUCHER_NO: '',
                    PARTY_CODE: partyCode,
                    ACT_CODE: actCode,
                    REMARKS: masterRemarks,
                    ASTATUS: aStatus
                },
                Detail: details
            });
        });
        return records;
    },

    CompleteExcelUpload: function () {
        if (empr_SalesQutation.excelSaving) {
            return;
        }
        if (Permissions != "Admin") {
            if (!Permissions.r_ADD) {
                empr_helper.notify("You are not allowed to add new record !", 2);
                return;
            }
        }
        if (!empr_SalesQutation.ValidatePartySelection()) {
            return;
        }
        if (!empr_SalesQutation.excelState.canComplete || !(empr_SalesQutation.excelState.masters || []).length) {
            empr_helper.notify("Please upload and validate an Excel file first.", 2);
            return;
        }
        var vDate = $("#V_DATE").val();
        if (!vDate) {
            empr_helper.notify("Transaction date is required.", 2);
            return;
        }

        var records = empr_SalesQutation.GetExcelDataToSave();
        if (!records.length) {
            empr_helper.notify("No valid Master Records to save.", 2);
            return;
        }

        empr_SalesQutation.excelSaving = true;
        $('#BtnExcelComplete').prop('disabled', true);
        empr_SalesQutation.SetExcelStatus('Saving Master Records...', '');
        $.ajax({
            type: 'POST',
            url: '/SalesQutation/SaveExcelBatch',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(records),
            cache: false,
            success: function (data) {
                ajaxHelper.isSessionExpired(data);
                empr_SalesQutation.excelSaving = false;
                $('#BtnExcelComplete').prop('disabled', false);
                if (typeof data === 'string') {
                    empr_helper.notify(data, 2);
                    empr_SalesQutation.SetExcelStatus(data, 'error');
                    return;
                }
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    $('#SaveQuotationModal').modal('hide');
                    empr_SalesQutation.ResetForm();
                    setTimeout(function () {
                        $('#SaveQuotationModal').modal('show');
                    }, 2500);
                }
                else {
                    empr_SalesQutation.SetExcelStatus(data.msg || 'Unable to save Excel data.', 'error');
                }
            },
            error: function () {
                empr_SalesQutation.excelSaving = false;
                $('#BtnExcelComplete').prop('disabled', false);
                empr_SalesQutation.SetExcelStatus('Unable to save Excel data. No records were saved.', 'error');
                empr_helper.notify('Unable to save Excel data. No records were saved.', 2);
            }
        });
    }
}
