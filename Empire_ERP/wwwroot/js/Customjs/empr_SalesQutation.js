var empr_SalesQutation = {
    selectedItems: [],
    selectedGroupId: null,
    selectedGroupName: '',
    groupItemsCache: {},
    highlightToken: 0,
    InitEvents: function () {
        $(document).ready(function () {
            empr_SalesQutation.InitQuickSearchGrid();
            empr_SalesQutation.GetCurrentStock();
            empr_SalesQutation.ResetForm();
            empr_SalesQutation.RenderItemGroups();
            empr_SalesQutation.InitReportTypeDDL();
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
                        if (empr_SalesQutation.ValidateDetails()) {
                            $('#SaveQuotationModal').modal('show');
                        }
                    }
                } else {
                    if (empr_SalesQutation.ValidateDetails()) {
                        $('#SaveQuotationModal').modal('show');
                    }
                }
            });

            $('body').on('click', '#BtnModalSave', function () {
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
            });

            $('body').on('click', '#BtnQuickSearch', function () {
                empr_SalesQutation.InitQuickSearchGrid();
            });

            $('body').on('click', '.sq-group', function () {
                var groupId = $(this).data('item');
                empr_SalesQutation.SelectGroup(groupId, $(this).data('name'));
            });

            $('body').on('click', '.sq-item-card', function (e) {
                if ($(e.target).closest('.sq-rate-field').length) {
                    return;
                }
                var $card = $(this);
                var itemCode = $card.data('item');
                var rate = $card.find('.sq-rate-input').val();
                empr_SalesQutation.IncreaseItemQty(itemCode, rate);
            });

            $('body').on('click mousedown', '.sq-rate-field', function (e) {
                e.stopPropagation();
            });

            $('body').on('change', '.sq-rate-input', function () {
                var $card = $(this).closest('.sq-item-card');
                empr_SalesQutation.UpdateItemRate($card.data('item'), $(this).val());
            });

            $('body').on('click', '.sq-qty-minus', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var itemCode = $(this).closest('.sq-item-card').data('item');
                empr_SalesQutation.DecreaseItemQty(itemCode);
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
            ItemsGroup.forEach(function (item, index) {
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
            empr_SalesQutation.groupItemsCache[groupId] = data;
            var $list = $('#sqItemList');
            $list.empty();
            var items = data || [];
            if (items.length > 0) {
                var itemsHtml = $.map(items, function (item) {
                    var rate = item.salE_RATE != null && item.salE_RATE !== '' ? parseFloat(item.salE_RATE) : 0;
                    if (isNaN(rate)) {
                        rate = 0;
                    }
                    var selected = empr_SalesQutation.GetSelectedItem(item.iteM_CODE);
                    var qty = selected ? selected.qty : 0;
                    var displayRate = selected ? parseFloat(selected.rate) : rate;
                    if (isNaN(displayRate)) {
                        displayRate = 0;
                    }
                    return `
                        <div class="sq-item-card${qty > 0 ? ' selected' : ''}" data-item="${item.iteM_CODE}" data-rate="${rate}">
                            <div class="sq-qty-badge">
                                <button type="button" class="sq-qty-minus" title="Decrease">-</button>
                                <span class="sq-qty-value">${qty}</span>
                            </div>
                            ${empr_SalesQutation.ImageHtml(item.ipic, item.iteM_NAME, false)}
                            <div class="sq-item-body">
                                <p class="sq-item-name">${empr_SalesQutation.EscapeHtml(item.iteM_NAME || '')}</p>
                                <div class="sq-item-rate sq-rate-field">
                                    <label>Rate</label>
                                    <input type="number" class="sq-rate-input" value="${displayRate.toFixed(2)}" min="0" step="any">
                                </div>
                            </div>
                        </div>
                    `;
                });
                $list.append('<div class="sq-item-grid">' + itemsHtml.join('') + '</div>');
                $('.sq-group-count[data-count-for="' + groupId + '"]').text('(' + items.length + ' items)');
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
                empr_PurchaseBill.CurrentStock = data;
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
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

    IncreaseItemQty: function (itemCode, rate) {
        if (itemCode == null || itemCode === '') {
            return;
        }
        rate = parseFloat(rate);
        if (isNaN(rate) || rate < 0) {
            rate = 0;
        }
        var existing = empr_SalesQutation.GetSelectedItem(itemCode);
        if (existing) {
            existing.qty = (parseFloat(existing.qty) || 0) + 1;
            existing.rate = rate;
        } else {
            if (Limit != 0 && empr_SalesQutation.selectedItems.length >= Limit) {
                empr_helper.notify("You can only add  " + Limit + " records.", 2);
                return;
            }
            empr_SalesQutation.selectedItems.push({
                itemCode: itemCode,
                rate: rate,
                qty: 1
            });
        }
        empr_SalesQutation.RefreshCardQty(itemCode);
        empr_SalesQutation.UpdateSummary();
    },

    UpdateItemRate: function (itemCode, rate) {
        var $card = $('.sq-item-card[data-item="' + itemCode + '"]');
        rate = parseFloat(rate);
        if (isNaN(rate) || rate < 0) {
            empr_helper.notify("Please enter correct item rate.", 2);
            var selected = empr_SalesQutation.GetSelectedItem(itemCode);
            var fallback = selected ? selected.rate : ($card.data('rate') || 0);
            $card.find('.sq-rate-input').val((parseFloat(fallback) || 0).toFixed(2));
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
            var rate = parseFloat(item.rate);
            if (isNaN(qty) || qty <= 0) {
                return;
            }
            if (isNaN(rate) || rate < 0) {
                rate = 0;
            }
            empr_SalesQutation.selectedItems.push({
                itemCode: item.iteM_CODE,
                rate: rate,
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
                }
            }
        });
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

    ValidateMainInfo: function () {
        var valid = true;
        var data = empr_SalesQutation.GetDataToSave();

        if (!data.Master.V_DATE) {
            empr_helper.notify("Transaction date is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.PARTY_CODE == null || data.Master.PARTY_CODE === '') {
            empr_helper.notify("Please select Party Type.", 2);
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
            }
        }, false, true);
    },

    ResetForm: function () {
        $('#Code').val('');
        $('#ID').val('');
        empr_helper.selectedBill = '';
        $('#partyhidden').val('');
        $('#acthidden').val('');
        $('#VOUCHER_NO').val('');
        $('#REMARKS').val('');
        $('#BtnDelete').hide();
        $('#BtnPrint').hide();
        $('.Record').removeClass('customHighlightForModifiedCells');
        empr_SalesQutation.selectedItems = [];
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
                }
            }, false, true);
        });
    },
}
