
var empr_POSTransaction = {
    isReturnMood: false,
    totalCount: 0,
    rowsCount: 0,
    Partydata: [],
    Salesmandata: [],
    Users: [],
    CustomerName: [],
    newGroupId: 0,
    MultipleTaxValue: 0,
    TotalAmtWithTax: 0,
    TaxAmt: 0,
    AccountTax: "Cash",
    cashtaxAmt: 0,
    banktaxAmt: 0,
    partytaxAmt: 0,
    barcodeCode: 0,
    billStatus: "",
    statusId: 0,
    return: false,
    return11: false,
    selectedWaiter: 0,
    selectedTable: 0,
    printStatus: "",
    billMode: "",
    previousCash: 0,
    previousBank: 0,
    previousBankAct: 0,
    previousCashAct: 0,
    previoustotal: 0,
    previousNetTotal: 0,
    dataLoaded: false,
    KotMsg: 0,
    InvoiceDate: "",
    code: 0,
    voucherNo: "",
    sizeGroups: {},
    userRights: {},
    selectedItems: [],
    payType: "Cash",
    rowCount: 0,
    kotDuplicate: 0,
    SER_CHARGES: 0,
    selectedParty: {},
    alreadyreturn: 0,
    connection: null,
    salesmanReq: 'N',
    pointStartValue: 0,
    maxPoints: 0,
    totalPoints: 0,
    CardDisc: 0,
    totalPayment: 0,
    returnCash: 0,
    //rateLock: 0,
    settlsign: '',
    pwindow: 0,
    billDisc: 0,
    biilDiscVal: 0,
    alreadyreturn1: false,
    RT_TYPES: new DevExpress.data.ArrayStore({
        key: "key",
        data: [
            { key: 1, value: '1 KG' },
            { key: 40, value: '40 kG' },
        ]
    }),
    Branch_RT_TYPE: '',
    isPay: 0,
    isPre: 0,
    isDup: 0,
    isPayQty: 0,
    isPreQty: 0,
    isDupQty: 0,
    isPayModal: 'Y',
    isPreModal: 'Y',
    isDupModal: 'Y',
    InitEvents() {
        $(document).ready(function () {

            //  //  
            empr_POSTransaction.GetUserRights();
            empr_POSTransaction.SignlocalStorage();
            empr_POSTransaction.InitBankAccount("");
            empr_POSTransaction.IntiAdvanceBankAct("");
            empr_POSTransaction.GetMapData();

            empr_POSTransaction.ItemsGroup();
            empr_POSTransaction.GetCustomerName();
            empr_POSTransaction.Tablesvisible();
            empr_POSTransaction.DineInvisible();
            empr_POSTransaction.PaymentMode();
            empr_POSTransaction.DynamicIcon();
            empr_POSTransaction.TodayDate();
            empr_POSTransaction.POSDesignScript();



            $('#AllBarcodePickModal').on('shown.bs.modal', function () {
                const gridElement = $('#BarcodePickGridContainer');

                const searchInput = gridElement.find(".dx-datagrid-search-panel .dx-texteditor-input");

                if (searchInput.length) {
                    setTimeout(() => {
                        searchInput.focus();
                    }, 100);

                }
            });


            $('#AllBarcode').on('click', function () {
                 
                //var ItemId = $(this).data('item');
                if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return == true) {
                    //if (BarcodeTextBoxVisible) {
                    if (true) {
                        empr_POSTransaction.InitAllBarcodePickGrid();
                    }
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }
            });
            $(document).on('input change', '.quantity-input, .total-price, .discountpercent,.discountvalue ,.tax ', function () {
                 
                empr_POSTransaction.updateSummary(null);
                empr_POSTransaction.PaymentMode();

            });

            $('body').on('click', '.gridDiv', function () {
                var ItemId = $(this).data('item');
                if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return == true) {
                    if (BarcodeTextBoxVisible) {
                        empr_POSTransaction.InitBarcodePickGrid(ItemId);
                    }
                    else {

                        empr_POSTransaction.loadItemsForData(ItemId, 0, "", 1);
                    }
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }
            });

            $('body').on('click', '.close', function () {
                //  //  
                if (empr_POSTransaction.CardDisc != 0) {
                    $('#billDiscValue').val(0);
                    empr_POSTransaction.updateSummary();
                }
                $('#settlementInput').val('');
            });

            $('body').on('click', '.PaymentModalClose', function () {
                //  //  
                if (empr_POSTransaction.CardDisc != 0) {
                    $('#billDiscValue').val(0);
                    empr_POSTransaction.updateSummary();
                }
                $('#settlementInput').val('');
            });

            $('#paymentModal').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.stopPropagation();
                    $('#payment').click();
                }
            });

            $("#SHOW_SELECTED").change(function () {
                if (!$(this).is(":checked")) {
                    empr_POSTransaction.refreshData(0);

                }
                else {
                    empr_POSTransaction.refreshData(3);
                }
            });

            $('#btnCash').on('click', function () {
                 

                empr_POSTransaction.selectTab(this, 0);
            });
            $('#btnCard').on('click', function () { empr_POSTransaction.selectTab(this, 1); });
            $('#btnParty').on('click', function () { empr_POSTransaction.selectTab(this, 2); });
            $('#btnSplit').on('click', function () { empr_POSTransaction.selectTab(this, 3); });

            $('.headers').on('click', '.icondiv', function () {
                //  //  
                if ($('#BILL_STATUS').val() != 'P') {
                    $('.headers .icondiv').css({
                        'background-color': '#055a87'
                    });

                    var iconId = $(this).attr('id');
                    var iconText = $(this).text().trim();
                    $(`.headers #${iconId}`).css({
                        'background-color': '#0e1832',
                    });

                    $('#INV_STATUS').val(iconId);
                    empr_POSTransaction.DeleveryInputvisible(iconText, iconId);

                    if (iconText == "Hold" || iconId == 1) {
                        empr_POSTransaction.HoldDataSave(iconId);
                    }

                    if (iconId != 2) {
                        $("#dineInInputContainer").hide();
                        //$("#inputContainer").hide();
                        empr_POSTransaction.ServiceChargesOnChangeFunction('clear');
                    }

                    if (iconId == 2 || iconId == 5) {
                        if (iconId == 2) {
                            empr_POSTransaction.DineInInputvisible(iconText, iconId);
                        }
                        empr_POSTransaction.DineInvisible();
                        document.getElementById("WaiterModal").style.display = "flex";
                        $("#inputContainer").hide();
                    }


                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }
                empr_POSTransaction.updateSummary();
            });

            $(document).on('click', '.kotSwalCancel', function () {
                empr_POSTransaction.SaveInfo("K");
                empr_POSTransaction.ResetForm();
            });

            $(document).on('click', '.discountswal', function () {
                empr_POSTransaction.totalPoints = 0;
                empr_POSTransaction.PaymentSetting();
                $('#TotalPaymentsett').val($("#totalPayment").text());
            });

            $(document).on('click', '.discardSwalCancel', function () {
                $('#orderTable').empty();
                $('.qrCodevisible').val('');
                $('.barcodevisible').val('');
                $('#billDelPersent').val(0);
                $('#billDelValue').val(0);
                empr_POSTransaction.updateSummary();
                empr_POSTransaction.ResetForm();
            });
            $('body').on('input', '.quantity-input', function () {

                let value = parseFloat($(this).val());
                let row = $(this).closest('tr');
                let btn = row.find('#tablechk');

                if (value < 0) {

                    if (!btn.hasClass('clicked')) {
                        btn.addClass('clicked');

                        let quantity = Math.abs(value);
                        let rate = Math.abs(parseFloat(row.find('.total-price').val()));

                        row.find('.quantity-input').val(-quantity);
                        row.find('.total-price').val(-rate);

                        empr_POSTransaction.updateSummary();
                    }
                }
                else {

                    if (btn.hasClass('clicked')) {
                        btn.removeClass('clicked');
                        let quantity = Math.abs(value);
                        let rate = Math.abs(parseFloat(row.find('.total-price').val()));
                        row.find('.quantity-input').val(quantity);
                        row.find('.total-price').val(rate);

                        empr_POSTransaction.updateSummary();
                    }
                }
            });
            document.querySelector('#orderTable').addEventListener('change', function (e) {
                //  //  

                if (e.target.classList.contains('quantity-input')) {
                    const quantityInput = e.target;

                    if (quantityInput.value == 0) {
                        quantityInput.value = 1;
                    }
                    empr_POSTransaction.updateSummary();
                }
            });


            $('body').on('click', '#DiscountBtn', function () {

                if ($('#BILL_STATUS').val() != 'P') {
                    // Clear previous data from modal to prevent duplication
                    $('.modalData').empty();

                    // AJAX call to fetch discounts
                    ajaxHelper.ajaxGetJson('/POSTransactions/GetAllDiscount', function (data) {


                        // Check if the response contains valid data
                        if (data != null) {
                            var itemsHtml = $.map(data, function (item) {
                                return `
                                <div class="discount-box selectthisdiv" data-item="${item.code}" style="cursor: pointer;">
                                    <p>${item.descr} (${item.disc}%)</p>
                                </div>
                            `;
                            }).join('');

                            // Append the discount items to the modal container
                            $('.modalData').html(itemsHtml);

                            // Show the modal
                            document.getElementById("discountModal").style.display = "flex";
                        } else {
                            // Handle case when no data is returned or msgType is not 1
                            $('.modalData').html('<p>No discounts available.</p>');
                        }
                    }, false, true);
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }

            });

            $(document).on("click", ".waiter-item", function () {
                $(".waiter-item").removeClass("selectedWaiter"); // Remove selection from all
                $(this).addClass("selectedWaiter"); // Add selection to clicked item
                empr_POSTransaction.selectedWaiter = $(this).data("item");
                var waiterDesc = $(this).find("p").text().trim();
                $('#Waiter').text('Waiter : ' + waiterDesc + ' |');
                $("#waiterName").val(waiterDesc);

                if ($('#INV_STATUS').val() == 2) {
                    empr_POSTransaction.Tablesvisible($('#TRANS_ID').val());
                    document.getElementById("WaiterModal").style.display = "none";
                    document.getElementById("TablesModal").style.display = "flex";
                }// Store selected waiter code
                document.getElementById("WaiterModal").style.display = "none";
            });

            $(document).on("click", ".table-item", function () {

                $(".table-item").removeClass("selectedWaiter"); // Remove selection from all
                $(this).addClass("selectedWaiter"); // Add selection to clicked item
                empr_POSTransaction.selectedTable = $(this).data("item");
                var tableDesc = $(this).find("p").text().trim();
                $('#Tables').text('Table : ' + tableDesc + ' |');
                $("#tableNum").val(tableDesc);
                if ($("#TRANS_ID").val() != '' && $("#TRANS_ID").val() != undefined && $("#TRANS_ID").val() != 0 && $("#TRANS_ID").val() != null) {
                    var dataModel = empr_POSTransaction.GetDataToSave(undefined, '');
                    ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/UpdateTablesAndWaiter", function (data) {
                        if (data.msgType == 1) {
                            empr_helper.notify(data.msg, data.msgType);
                            empr_POSTransaction.ResetForm();
                        } else {
                            empr_helper.notify(data.msg, data.msgType);
                        }

                    }, false, true);
                }
                document.getElementById("TablesModal").style.display = "none";// Store selected waiter code
            });

            $("#waiterSkip").click(function () {

                document.getElementById("WaiterModal").style.display = "none";
                if ($('#INV_STATUS').val() == 2) {
                    empr_POSTransaction.Tablesvisible(empr_POSTransaction.selectedWaiter);
                }// Store selected waiter code

            });

            $("#tableSelect").click(function () {
                if (empr_POSTransaction.selectedTable) {
                    document.getElementById("TablesModal").style.display = "none";
                } else {
                    empr_helper.notify("Please First Select Table Or Skip.", 2);
                }

            });
            $("#cardOption").click(function () {
                $("#PARTY_BACCOUNTS").show();
                $("#party_account").show();
            });
            $("#cashOption").click(function () {
                $("#PARTY_BACCOUNTS").hide();
                $("#party_account").hide();
            });
            $("#creditOption").click(function () {
                $("#PARTY_BACCOUNTS").hide();
                $("#party_account").hide();
            });

            $("#tableSkip").click(function () {
                document.getElementById("TablesModal").style.display = "none";
            });

            $('body').on('click', '#expenseButton', function () {

                empr_POSTransaction.GetExpenseRecord();
                document.getElementById("ExpenseModal").style.display = "flex";
                setTimeout(() => {
                    const dropdown = $('#ExpenseType').dxSelectBox('instance');
                    if (dropdown) {
                        dropdown.focus(); // This will trigger focus on the dropdown
                    }
                }, 300)
            });

            $('#historyBtn').on('click', function () {
                var CstNumber = $('#CMOB').val();
                if (CstNumber) {
                    empr_POSTransaction.InitCustomerHistoryGrid(CstNumber);
                }
                else {
                    empr_helper.notify('Please fill Contact Number first', 2)
                }
            });

            $("#SyncData").click(function () {

                if (!navigator.onLine) {
                    $("#internetModal").modal("show");
                    return;
                }

                $('#Loader').appendTo('body').css({ display: 'flex' });

                $.ajax({
                    url: '/POSTransactions/GetSyncData',
                    type: 'GET',
                    success: function (data) {
                        empr_helper.notify(data.msg, data.msgType);
                        location.reload();
                    },
                    error: function () {
                        empr_helper.notify("Server Error", 0);
                    },
                    complete: function () {
                        $("#Loader").hide();
                    }
                });

            });

            $("#settlementInput").on("change", function () {
                //  //  
                var PreviousTotal = parseInt($('#TotalPaymentsett').val()) || 0;
                var settValue = Math.abs(parseInt($(this).val())) || 0;

                var settlSign = $('#SettleSign').val();
                var selectedBtn = $(".payBtns.active");

                var rights = empr_POSTransaction.userRights;
                if (rights) {
                    if ((rights.settF > settValue && rights.settF != settValue) && settValue != 0) {
                        empr_helper.notify(`Please Type Greter Than or Equal This Value ${rights.settF}`, 2);
                        $("#totalPayment").text(PreviousTotal);
                         
                        if (selectedBtn.attr("id") === "btnCash")
                            $("#cashAmount").val(PreviousTotal);
                        else if (selectedBtn.attr("id") === "btnCard")
                            $("#bankrecv").val(PreviousTotal);
                        else
                            $("#partyrecv").val(PreviousTotal);

                        return
                    }
                    else if ((rights.settT < settValue && rights.settT != settValue) && settValue != 0) {
                        empr_helper.notify(`Please Type Less Than or Equal This Value ${rights.settT}`, 2);
                        $("#totalPayment").text(PreviousTotal);
                        if (selectedBtn.attr("id") === "btnCash")
                            $("#cashAmount").val(PreviousTotal);
                        else if (selectedBtn.attr("id") === "btnCard")
                            $("#bankrecv").val(PreviousTotal);
                        else
                            $("#partyrecv").val(PreviousTotal);

                        return

                    }

                    if (settValue < PreviousTotal || settlSign === '+') {

                        var finalTotal = PreviousTotal;

                        if (settValue > 0) {
                            if (settlSign === '+') {
                                finalTotal = PreviousTotal + settValue;
                            } else if (settlSign === '-') {
                                finalTotal = PreviousTotal - settValue;
                            }

                            $("#totalPayment").text(finalTotal);
                            if (selectedBtn.attr("id") === "btnCash")
                                $("#cashAmount").val(finalTotal);
                            else if (selectedBtn.attr("id") === "btnCard")
                                $("#bankrecv").val(finalTotal);
                            else if (selectedBtn.attr("id") === "btnParty")
                                $("#partyrecv").val(finalTotal);

                            $(this).val(`${settlSign}${settValue}`);
                        } else {
                            $("#totalPayment").text(PreviousTotal);
                            if (selectedBtn.attr("id") === "btnCash")
                                $("#cashAmount").val(PreviousTotal);
                            else if (selectedBtn.attr("id") === "btnCard")
                                $("#bankrecv").val(PreviousTotal);
                            else
                                $("#partyrecv").val(PreviousTotal);

                            $(this).val(`${settlSign}${settValue}`);
                        }

                        $('#cashReturn').val(0);
                    }
                    else {
                        empr_helper.notify('Please Type Less Than Total Payment Value ', 2)
                        $("#totalPayment").text(PreviousTotal);
                        if (selectedBtn.attr("id") === "btnCash")
                            $("#cashAmount").val(PreviousTotal);
                        else if (selectedBtn.attr("id") === "btnCard")
                            $("#bankrecv").val(PreviousTotal);
                        else
                            $("#partyrecv").val(PreviousTotal);
                    }
                }
                else {
                    if (settValue < PreviousTotal || settlSign === '+') {

                        var finalTotal = PreviousTotal;

                        if (settValue > 0) {
                            if (settlSign === '+') {
                                finalTotal = PreviousTotal + settValue;
                            } else if (settlSign === '-') {
                                finalTotal = PreviousTotal - settValue;
                            }

                            $("#totalPayment").text(finalTotal);
                            if (selectedBtn.attr("id") === "btnCash")
                                $("#cashAmount").val(finalTotal);
                            else if (selectedBtn.attr("id") === "btnCard")
                                $("#bankrecv").val(finalTotal);
                            else if (selectedBtn.attr("id") === "btnParty")
                                $("#partyrecv").val(finalTotal);

                            $(this).val(`${settlSign}${settValue}`);
                        } else {
                            $("#totalPayment").text(PreviousTotal);
                            if (selectedBtn.attr("id") === "btnCash")
                                $("#cashAmount").val(PreviousTotal);
                            else if (selectedBtn.attr("id") === "btnCard")
                                $("#bankrecv").val(PreviousTotal);
                            else
                                $("#partyrecv").val(PreviousTotal);

                            $(this).val(`${settlSign}${settValue}`);
                        }

                        $('#cashReturn').val(0);
                    }
                    else {
                        empr_helper.notify('Please Type Less Than Total Payment Value ', 2)
                        $("#totalPayment").text(PreviousTotal);
                        if (selectedBtn.attr("id") === "btnCash")
                            $("#cashAmount").val(PreviousTotal);
                        else if (selectedBtn.attr("id") === "btnCard")
                            $("#bankrecv").val(PreviousTotal);
                        else
                            $("#partyrecv").val(PreviousTotal);
                    }
                }
                empr_POSTransaction.updateSummary();
            });


            // PLUS CLICK
            $("#Plus").on("click", function () {
                 
                $("#SettleSign").val("+");
                localStorage.setItem("SettleSign", "+");
                // UI highlight
                $("#minus").removeClass("active");
                $("#Plus").addClass("active");

                empr_POSTransaction.applySettlement("+");
            });

            // MINUS CLICK
            $("#minus").on("click", function () {

                $("#SettleSign").val("-");
                localStorage.setItem("SettleSign", "-");

                // UI highlight
                $("#Plus").removeClass("active");
                $("#minus").addClass("active");

                empr_POSTransaction.applySettlement("-");
            });

            $(document).on('keyup change', '#CMOB', function () {

                var number = $('#CMOB').val().replace(/-/g, ''); // Remove hyphens from input
                var user = empr_POSTransaction.CustomerName.filter(b =>
                    (b.number || '').replace(/-/g, '') === number  // Remove hyphens from user numbers too
                );

                if (user.length > 0) {
                    var item = user[0];
                    $('#CNAME').val(item.name);
                    $('#CADD').val(item.address);
                } else {
                    $('#CNAME').val('');
                    $('#CADD').val('');
                }
            });

            $('#Des, #expamt').on('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    $('#ExpSave').trigger('click');
                }
            });


            $('body').on('click', '#ExpSave', function () {

                if (empr_POSTransaction.ExpenseValidate()) {
                    var dataModel = empr_POSTransaction.ExpenseData();
                    ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/ExpenseRecordSave", function (data) {

                        empr_helper.notify(data.msg, data.msgType);
                        if (data.msgType == 1) {

                            empr_POSTransaction.ResetForm();
                            empr_POSTransaction.GetExpenseRecord();
                        }
                        //Users = data.users;
                    }, false, true);
                }

            });

            $('body').on('click', '#ExpPrintSave', function () {


                var grid = $("#ExpgridContainer").dxDataGrid("instance"); // Get grid instance
                var gridData = grid.getDataSource().items(); // Get all data from the grid

                if (gridData.length === 0) {
                    alert("No data available in the expense grid!");
                    return;
                }

                $.ajax({
                    url: "/POSTransactions/PrintExpenses", // Controller ka URL
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(gridData), // Convert data to JSON
                    success: function (response) {
                        if (response.msgType == 1) {
                            setTimeout(function () {
                                $('#ExpenseSlipPreview').html(response.slipHtml);
                                const printContent = document.getElementById('ExpenseSlipPreview').innerHTML;

                                // Create a temporary iframe for printing
                                const printFrame = document.createElement('iframe');
                                printFrame.style.position = 'absolute';
                                printFrame.style.top = '-10000px'; // Hide the iframe
                                document.body.appendChild(printFrame);

                                // Write the content into the iframe's document
                                const frameDoc = printFrame.contentWindow || printFrame.contentDocument;
                                frameDoc.document.open();
                                frameDoc.document.write(`
                                        <html>  <head>
                                                    <style>
                                                        body {
                                                            width: 80mm;
                                                        }
                                                        table {
                                                            width: 100%;
                                                            border-collapse: collapse;
                                                        }
                                                        th, td {
                                                            padding: 3px;
                                                        }
                                                    </style>
                                                </head>
                                            <body>
                                            ${printContent}
                                            </body>
                                        </html>
                                    `);
                                frameDoc.document.close();

                                const images = frameDoc.document.images;
                                let imagesLoaded = 0;

                                function checkImagesLoaded() {
                                    imagesLoaded++;
                                    if (imagesLoaded === images.length) {
                                        frameDoc.focus();
                                        frameDoc.print();
                                        document.body.removeChild(printFrame);
                                    }
                                }

                                if (images.length > 0) {
                                    for (let img of images) {
                                        img.onload = checkImagesLoaded;
                                        img.onerror = checkImagesLoaded;
                                    }
                                } else {
                                    frameDoc.focus();
                                    frameDoc.print();
                                    document.body.removeChild(printFrame);
                                }

                            }, 500);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                        alert("Failed to save expenses.");
                    }
                });


                //if (empr_POSTransaction.ExpenseValidate()) {
                //    var dataModel = empr_POSTransaction.ExpenseData();
                //    ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/ExpenseRecordSave", function (data) {
                //         
                //        empr_helper.notify(data.msg, data.msgType);
                //        if (data.msgType == 1) {
                //             
                //            empr_POSTransaction.ResetForm();
                //            empr_POSTransaction.GetExpenseRecord();
                //        }
                //        //Users = data.users;
                //    }, false, true);
                //}

            });

            $('body').on('click', '.Exp_elm_edit', function () {

                empr_POSTransaction.ResetForm();
                empr_POSTransaction.ResetAllFields();
                var id = $(this).attr("reportid");
                $('#ExpTranId').val(id);
                ajaxHelper.ajaxGetJson('/POSTransactions/GetExpenseByCode?code=' + id, function (data) {

                    if (data.msgType == 1) {
                        $('#Des').val(data.data[0].descr);
                        $('#expamt').val(data.data[0].amount);
                        $("#ExpenseType").dxSelectBox('instance').option('value', parseInt(data.data[0].actcode));
                        empr_POSTransaction.ExpenseDropdown(parseInt(data.data[0].actcode));
                    }
                }, false, true);

            });

            $('body').on('click', '#tablechk', function () {
                 
                $(this).toggleClass('active'); // toggle class

                if ($(this).hasClass('active')) {
                    empr_POSTransaction.return11 = true;
                } else {
                    empr_POSTransaction.return11 = false;
                }
                if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return == true || empr_POSTransaction.alreadyreturn == 1) {
                    var row = $(this).closest('tr');
                    var quantity = parseFloat(row.find('.quantity-input').val());
                    var rate = parseFloat(row.find('.total-price').val());
                    let discPer = parseFloat(row.find('.discountpercent ').val());


                    var amt = parseFloat(row.find('.amt-input').val());
                    //var discountpercent = parseFloat(row.find('.discountpercent').val());
                    var netAmt = parseFloat(row.find('.net-amount').val());
                    //empr_POSTransaction
                    //var subTotal = parseFloat('#subTotal').text();
                    //const isClicked = $(this).toggleClass('clicked').hasClass('clicked');
                    //empr_POSTransaction.isReturnMode = isClicked;

                    var pickid = parseFloat(row.find('#pickid').val());
                    var stock = row.find('.stock').data('stockqty');
                    var stockStatus = row.find('#stockstatus').val();
                    var Id;
                    if (pickid == 0 || (pickid != 0 && empr_POSTransaction.alreadyreturn == 1)) {
                        if (BarcodeTextBoxVisible)
                            var Id = row.find('.BarcodeId').val();
                        else
                            var Id = row.find('.SelectedItemId').val();

                        if ($(this).toggleClass('clicked').hasClass('clicked')) {
                            row.find('.quantity-input').val(-Math.abs(quantity));
                            row.find('.total-price').val(-Math.abs(rate));
                            row.find('.discountpercent').val(-Math.abs(discPer));

                            row.find('.amt-input').val(-Math.abs(amt));
                            //row.find('.discountpercent').val(-Math.abs(discountpercent));
                            row.find('.net-amount').val(-Math.abs(netAmt));
                            //('#BILL_STATUS').val()



                            if (BarcodeTextBoxVisible) {
                                document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {


                                    //const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                    const binput = tableRow.querySelector('.BarcodeId');
                                    const rowBarcode = parseFloat(binput?.value || "0");
                                    if (rowBarcode === Id) {
                                        var stockElement = tableRow.querySelector('.stock');
                                        var totalStock = parseInt(stock) + quantity;
                                        stockElement.textContent = `HAS ONLY ${totalStock} IN STOCK`;
                                        $(stockElement).data('stockqty', totalStock);
                                        stockElement.style.color = "green";
                                    }
                                });
                            }

                        } else {
                            row.find('.quantity-input').val(Math.abs(quantity));
                            row.find('.total-price').val(Math.abs(rate));
                            row.find('.discountpercent').val(Math.abs(discPer));

                            row.find('.amt-input').val(Math.abs(amt));
                            //row.find('.discountpercent').val(-Math.abs(discountpercent));
                            row.find('.net-amount').val(Math.abs(netAmt));
                            //row.find('.discountpercent').val(Math.abs(discPer));

                            //row.find('.amt-input').val(Math.abs(amt));
                            //row.find('.discountpercent').val(Math.abs(discountpercent));
                            //row.find('.net-amount').val(Math.abs(netAmt));
                            var quantitys = parseFloat(row.find('.quantity-input').val());
                            //const isClicked = $(this).toggleClass('clicked').hasClass('clicked');
                            //empr_POSTransaction.isReturnMode = false;

                            if (stockStatus == 'Y') {
                                if (quantity > stock || quantity < stock) {
                                    if (BarcodeTextBoxVisible) {

                                        document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {

                                            const binput = tableRow.querySelector('.BarcodeId');
                                            const rowBarcode = parseFloat(binput?.value || "0");
                                            //const  = .querySelector('.BarcodeId').value;
                                            if (rowBarcode === Id) {
                                                var stockElement = tableRow.querySelector('.stock');
                                                var totalStock = parseInt(stock) - quantitys;
                                                stockElement.textContent = `HAS ONLY ${totalStock} IN STOCK`;
                                                $(stockElement).data('stockqty', totalStock);
                                                if (totalStock == 0)
                                                    stockElement.style.color = "red";
                                            }
                                        });
                                    }
                                    else {
                                        document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                            const rowBarcode = tableRow.querySelector('.SelectedItemId').value;
                                            if (rowBarcode === Id) {
                                                var stockElement = tableRow.querySelector('.stock');
                                                var totalStock = parseInt(stock) - quantitys;
                                                stockElement.textContent = `HAS ONLY ${totalStock} IN STOCK`;
                                                $(stockElement).data('stockqty', totalStock);
                                                if (totalStock == 0)
                                                    stockElement.style.color = "red";
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        //if (empr_POSTransaction.return == false)
                        empr_POSTransaction.updateSummary();
                    }
                    else {
                        empr_helper.notify("This invoice has already been returned once and cannot be returned again.", 2);

                    }
                    empr_POSTransaction.alreadyreturn = 0;
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }
            });


            $('body').on('click', '.modalData .selectthisdiv', function () {

                if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return != true) {
                    var TotalAmount = $('#TotalbillDisc').val();
                    var discountText = $(this).find('p').text();

                    // Extract the numeric value from the text (assuming it's the percentage, e.g., '20%')
                    var numericValue = parseFloat(discountText.match(/[\d\.]+/)[0]);

                    // Set the numeric value in the input field
                    $('#billDiscPercent').val(numericValue);

                    const disval = parseInt((numericValue / 100) * TotalAmount);

                    $('#billDiscValue').val(disval);

                    console.log("Selected Discount Value: ", numericValue);

                    // Hide modal after selection
                    document.getElementById("discountModal").style.display = "none";

                    empr_POSTransaction.updateSummary();
                    var TotalAmount = parseFloat($('#totalAmountGet').val()) || 0;
                    var DelPercent = parseFloat($('#billDelPercent').val()) || 0;
                    var calculatedValue = 0;

                    if (DelPercent != 0) {
                        calculatedValue = (TotalAmount * DelPercent) / 100;
                        $('#billDelValue').val(parseInt(calculatedValue));
                    }


                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }

            });

            $('body').on('click', '#BtnAddBarcodes', function () {

                empr_POSTransaction.BarcodePickGridData();
            });

            $('body').on('click', '#whtSend', function () {

                empr_POSTransaction.GetBookingGridData();
            });

            $('body').on('click', '#confirmSendBtn', function () {

                empr_POSTransaction.SentToWhatsApp();
            });

            document.querySelectorAll(".close").forEach(function (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    document.getElementById("discountModal").style.display = "none";
                    document.getElementById("WaiterModal").style.display = "none";
                    document.getElementById("TablesModal").style.display = "none";
                    document.getElementById("ExpenseModal").style.display = "none";
                    $('#editItemModal').modal('hide');
                    $('#printModal').modal('hide');
                });
            });

            $('#search-item').on('keyup', function () {
                var searchValue = $(this).val().toLowerCase();

                $('.search').filter(function () {
                    var itemName = $(this).find('.mid-itemname').text().toLowerCase();
                    $(this).toggle(itemName.includes(searchValue));
                });
            });

            $('#listView').on('click', function () {

                $('#items-container').removeClass('grid-view').addClass('list-view');
                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);
            });

            $('#gridView').on('click', function () {

                $('#items-container').removeClass('list-view').addClass('grid-view');
                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);

            });

            $('#getitems').on('click', function () {

                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);

            });

            $('#DiscardSale').on('click', function () {
                //  //  
                $('#btnCard, #btnParty, #btnSplit').closest('li').show();

                if ($('#TRANS_ID').val() == null || $('#TRANS_ID').val() == 0 || $('#TRANS_ID').val() == '' || $('#TRANS_ID').val() == undefined) {
                    $('#orderTable').empty();
                    $('.qrCodevisible').val('');
                    $('.barcodevisible').val('');
                    $('#billDelPersent').val(0);
                    $('#billDelValue').val(0);
                    empr_POSTransaction.updateSummary();
                    empr_POSTransaction.ResetForm();
                }
                else {
                    swal({
                        title: "Bill Deletion or Discard?",
                        text: "Do you want to permanently delete this bill, or just discard it temporarily?",
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#0CC27E',
                        cancelButtonColor: '#FF586B',
                        confirmButtonText: 'Yes, delete it!',
                        cancelButtonText: 'No, cancel!',
                        confirmButtonClass: 'btn btn-success mr-5',
                        cancelButtonClass: 'btn btn-danger discardSwalCancel',
                        buttonsStyling: false
                    }).then(function () {
                        if (Permissions != "Admin") {
                            if (Permissions.r_DLT) {
                                if ($('#SRBInvoiceId').val() == '' || $('#SRBInvoiceId').val() == null || $('#SRBInvoiceId').val() == undefined || $('#SRBInvoiceId').val().includes('Error')) {
                                    ajaxHelper.ajaxPostJsonData({ code: $('#TRANS_ID').val() }, "/POSTransactions/Delete", function (data) {
                                        empr_helper.notify(data.msg, data.msgType);
                                        if (data.msgType == 1) {
                                            $('#orderTable').empty();
                                            $('.qrCodevisible').val('');
                                            $('.barcodevisible').val('');
                                            $('#billDelPersent').val(0);
                                            $('#billDelValue').val(0);
                                            empr_POSTransaction.updateSummary();
                                            empr_POSTransaction.ResetForm();
                                        }
                                        else {
                                            empr_helper.notify(data.msg, data.msgType);
                                        }
                                    }, false, true);
                                }
                                else {
                                    empr_helper.notify('This invoice cannot be deleted because its tax has already been reported to the government.', 2);
                                }
                            }
                            else {
                                empr_helper.notify('You are not allowed to delete this bill. Please contact your administrator.', 2);
                            }
                        }
                        else {
                            if ($('#SRBInvoiceId').val() == '' || $('#SRBInvoiceId').val() == null || $('#SRBInvoiceId').val() == undefined || $('#SRBInvoiceId').val().includes('Error')) {
                                ajaxHelper.ajaxPostJsonData({ code: $('#TRANS_ID').val() }, "/POSTransactions/Delete", function (data) {
                                    empr_helper.notify(data.msg, data.msgType);
                                    if (data.msgType == 1) {
                                        $('#orderTable').empty();
                                        $('.qrCodevisible').val('');
                                        $('.barcodevisible').val('');
                                        $('#billDelPersent').val(0);
                                        $('#billDelValue').val(0);
                                        empr_POSTransaction.updateSummary();
                                        empr_POSTransaction.ResetForm();
                                    }
                                    else {
                                        empr_helper.notify(data.msg, data.msgType);
                                    }
                                }, false, true);
                            }
                            else {
                                empr_helper.notify('This invoice cannot be deleted because its tax has already been reported to the government.', 2);
                            }
                        }

                    });


                };
                empr_POSTransaction.GetMapData();
                empr_POSTransaction.GetUserRights();
                empr_POSTransaction.SetUserRights();
            });
            $('.settingEnter').on('click', function () {
                 
                if (empr_POSTransaction.isPay == 1) {
                    $('#preSetting').val(0);
                    $('#dupSetting').val(0);
                    let advAmount = parseFloat($("#advAmount").val()) || 0;
                    let advBankAmount = parseFloat($("#advBankAmount").val()) || 0;
                    let fnlAmount = parseFloat($("#fnlAmount").val()) || 0;
                    let fnlBankAmount = parseFloat($("#fnlBankAmount").val()) || 0;
                    let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
                    let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
                    let partyAmount = parseFloat($("#partyrecv2").val()) || 0;
                    let totalEntered;
                    let totalAmount;

                    empr_POSTransaction.printStatus = "";
                    $('#IsKot').val(0);

                    if ((cashAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cashAmount != 0) || empr_POSTransaction.AccountTax == 'Split') {
                        totalEntered = cashAmount + cardAmount + partyAmount;
                        totalAmount = $('#totalPayment').text();
                    }
                    else if (empr_POSTransaction.payType == 'Advance' && $('#BILL_STATUS').val() == 'P') {
                        totalEntered = advAmount + advBankAmount + fnlAmount + fnlBankAmount;
                        totalAmount = $('#totalPayment').text();
                    }
                    else {
                        totalEntered = $('#totalPayment').text();
                        totalAmount = $('#totalPayment').text();
                    }
                     
                    let tableRows = document.querySelectorAll('#orderTable tr');

                    for (let row of tableRows) {

                        let stockStatus = row.querySelector('.stockstatus')?.value;
                        let stockQty = row.querySelector('.stkqty')?.value;
                        if (stockStatus === "Y") {
                            if (Number(stockQty) < 0) {
                                empr_helper.notify("Insufficient stock. Kindly restock before proceeding.", 2);
                                return;
                            }
                        }
                         
                        let inputamt = row.querySelector('.amt-input')?.value;
                        if (Number(inputamt) == 0) {
                            empr_helper.notify("Amount must be grater than 0", 2);
                            return;

                        }
                    }
                    // DEBUG LOG: check all amounts and table rows
                    console.log({
                        totalAmount,
                        totalEntered,
                        cashAmount,
                        cardAmount,
                        partyAmount,
                        advAmount,
                        advBankAmount,
                        fnlAmount,
                        fnlBankAmount,
                        tableRowsLength: tableRows.length
                    });
                    if (tableRows.length === 0) {
                        empr_helper.notify("Please add at least one item.", 2);
                        return false;
                    }
                    //  
                    if (totalAmount <= totalEntered) {
                        if (empr_POSTransaction.Checkvalidation(totalAmount, totalEntered)) {
                            //$('#paymentModal').modal('hide');
                            $('#Loader').appendTo('body'); // modal ke upar le jao
                            $("#Loader").css({
                                display: 'flex'
                            });

                            setTimeout(function () {
                                if (Permissions != "Admin") {

                                    if (!$("#Code").val() && !Permissions.r_ADD) {
                                        empr_helper.notify("You are not allowed to add new record !", 2);
                                    }
                                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                                        empr_helper.notify("You are not allowed to edit records !", 2);
                                    }
                                    else {
                                        if (empr_POSTransaction.printStatus == "K") {
                                            empr_POSTransaction.SaveInfo("K");
                                        }
                                        else {
                                            empr_POSTransaction.SaveInfo("");
                                            empr_POSTransaction.ThanksMsg();
                                        }
                                    }
                                }
                                else {
                                    if (empr_POSTransaction.printStatus == "K") {
                                        empr_POSTransaction.SaveInfo("K");
                                    }
                                    else {
                                        $('#BILL_STATUS').val('P');
                                        empr_POSTransaction.updateSummary();
                                        empr_POSTransaction.SaveInfo("");
                                        empr_POSTransaction.ThanksMsg();
                                    }
                                }
                                empr_POSTransaction.ResetAllFields();
                                empr_POSTransaction.ResetForm();
                                setTimeout(function () {
                                    $("#Loader").hide();
                                }, 500);
                            }, 200);

                        } else {
                            var msg = "Please Select Required Fields";
                            empr_helper.notify(msg, 2);
                        }

                    } else {
                        var msg = "The total amount is greater than the entered amount. Please make sure all required fields are filled correctly.";
                        empr_helper.notify(msg, 2);
                    }
                    empr_POSTransaction.isPay = 0;
                }
                if (empr_POSTransaction.isPre == 1) {
                    $('#paySetting').val(0);
                    $('#dupSetting').val(0);
                    var PRNT_COPIES = $('#preSetting').val();
                    var id = empr_POSTransaction.idDup;
                    ajaxHelper.ajaxPostJsonData({ tranId: id, PWindow: empr_POSTransaction.pwindow, PRNT_COPIES: PRNT_COPIES }, "/POSTransactions/PrintModal", function (data) {
                        if (data.msgType == 1) {
                            if (data.pWindow == 1) {
                                $('#slipPreview').html(data.slipHtml);

                                const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                                if (qrCodeImg !== null && srbcodeimg !== null) {
                                    if (qrCodeImg.complete && srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        qrCodeImg.onload = () => {
                                            if (srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            }
                                        };
                                        srbcodeimg.onload = () => {
                                            if (qrCodeImg.complete) {
                                                empr_POSTransaction.printContent();
                                            }
                                        };
                                    }
                                } else if (qrCodeImg !== null) {
                                    if (qrCodeImg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        qrCodeImg.onload = () => {
                                            empr_POSTransaction.printContent();
                                        };
                                    }
                                } else if (srbcodeimg !== null) {
                                    if (srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        srbcodeimg.onload = () => {
                                            empr_POSTransaction.printContent();
                                        };
                                    }
                                } else {
                                    empr_POSTransaction.printContent();
                                }
                            }
                        }
                    }, false, true);
                    empr_POSTransaction.isPre = 0;
                    $('#paySetting').val(empr_POSTransaction.isPayQty);
                    $('#preSetting').val(empr_POSTransaction.isPreQty);
                    $('#dupSetting').val(empr_POSTransaction.isDupQty);
                }
                if (empr_POSTransaction.isDup == 1) {
                     
                    $('#paySetting').val(0);
                    $('#preSetting').val(0);
                    var PRNT_COPIES = $('#dupSetting').val();
                    var id = empr_POSTransaction.idDup;
                    $('#QuickSearchModal').modal('hide');
                    //var dataModels = empr_POSTransaction.GetDataToSave(undefined, "");
                    ajaxHelper.ajaxPostJsonData({ tranId: id, PWindow: empr_POSTransaction.pwindow, PRNT_COPIES: PRNT_COPIES }, "/POSTransactions/PrintModal", function (data) {
                         
                        /*empr_helper.notify(data.msg, data.msgType);*/
                        if (data.msgType == 1) {

                            if (data.pWindow == 1) {
                                $('#slipPreview').html(data.slipHtml);

                                const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                                if (qrCodeImg !== null && srbcodeimg !== null) {
                                    if (qrCodeImg.complete && srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        qrCodeImg.onload = () => {
                                            if (srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            }
                                        };
                                        srbcodeimg.onload = () => {
                                            if (qrCodeImg.complete) {
                                                empr_POSTransaction.printContent();
                                            }
                                        };
                                    }
                                } else if (qrCodeImg !== null) {
                                    if (qrCodeImg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        qrCodeImg.onload = () => {
                                            empr_POSTransaction.printContent();
                                        };
                                    }
                                } else if (srbcodeimg !== null) {
                                    if (srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    } else {
                                        srbcodeimg.onload = () => {
                                            empr_POSTransaction.printContent();
                                        };
                                    }
                                } else {
                                    empr_POSTransaction.printContent();
                                }
                            }

                        }
                    }, false, true);
                    empr_POSTransaction.isDup = 0;
                    $('#paySetting').val(empr_POSTransaction.isPayQty);
                    $('#preSetting').val(empr_POSTransaction.isPreQty);
                    $('#dupSetting').val(empr_POSTransaction.isDupQty);
                }
                $("#PrinterSetting").modal("hide");
            });

            $('#payment').on('click', function () {
                 
                var bstatus = $('#BILL_STATUS').val();
                if (bstatus == "P" && empr_POSTransaction.alreadyreturn1 == true) {
                    empr_helper.notify("This invoice has already been returned once and cannot be returned again.", 2);
                } else {
                    if (empr_POSTransaction.salesmanReq == 'Y') {
                        var value = $("#SalesmanName").dxSelectBox("instance").option("value");
                        if (!value) {
                            empr_helper.notify('Please Select First Salesman!', 2);
                            return;
                        }
                    }
                     
                    if (empr_POSTransaction.isPayModal == 'Y' && empr_POSTransaction.pwindow == 0) {
                        $('#paymentModal').modal('hide');
                        empr_POSTransaction.isPay = 1;

                        empr_POSTransaction.openPrinterSetting(true, false, false);
                    } else {

                        if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return == true) {
                            let advAmount = parseFloat($("#advAmount").val()) || 0;
                            let advBankAmount = parseFloat($("#advBankAmount").val()) || 0;
                            let fnlAmount = parseFloat($("#fnlAmount").val()) || 0;
                            let fnlBankAmount = parseFloat($("#fnlBankAmount").val()) || 0;
                            let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
                            let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
                            let partyAmount = parseFloat($("#partyrecv2").val()) || 0;
                            let totalEntered;
                            let totalAmount;

                            empr_POSTransaction.printStatus = "";
                            $('#IsKot').val(0);

                            if ((cashAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cashAmount != 0) || empr_POSTransaction.AccountTax == 'Split') {
                                totalEntered = cashAmount + cardAmount + partyAmount;
                                totalAmount = $('#totalPayment').text();
                            }
                            else if (empr_POSTransaction.payType == 'Advance' && $('#BILL_STATUS').val() == 'P') {
                                totalEntered = advAmount + advBankAmount + fnlAmount + fnlBankAmount;
                                totalAmount = $('#totalPayment').text();
                            }
                            else {
                                totalEntered = $('#totalPayment').text();
                                totalAmount = $('#totalPayment').text();
                            }
                             
                            let tableRows = document.querySelectorAll('#orderTable tr');

                            for (let row of tableRows) {

                                let stockStatus = row.querySelector('.stockstatus')?.value;
                                let stockQty = row.querySelector('.stkqty')?.value;
                      
                                if (stockStatus === "Y") {
                                    if (Number(stockQty) < 0) {
                                        empr_helper.notify("Insufficient stock. Kindly restock before proceeding.", 2);
                                        return;
                                    }
                                }
                               
                            }


                            // DEBUG LOG: check all amounts and table rows
                            console.log({
                                totalAmount,
                                totalEntered,
                                cashAmount,
                                cardAmount,
                                partyAmount,
                                advAmount,
                                advBankAmount,
                                fnlAmount,
                                fnlBankAmount,
                                tableRowsLength: tableRows.length
                            });
                            if (tableRows.length === 0) {
                                empr_helper.notify("Please add at least one item.", 2);
                                return false;
                            }
                            //  
                            if (totalAmount <= totalEntered) {
                                if (empr_POSTransaction.Checkvalidation(totalAmount, totalEntered)) {
                                    //$('#paymentModal').modal('hide');
                                    $('#Loader').appendTo('body'); // modal ke upar le jao
                                    $("#Loader").css({
                                        display: 'flex'
                                    });

                                    setTimeout(function () {
                                        if (Permissions != "Admin") {

                                            if (!$("#Code").val() && !Permissions.r_ADD) {
                                                empr_helper.notify("You are not allowed to add new record !", 2);
                                            }
                                            else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                                                empr_helper.notify("You are not allowed to edit records !", 2);
                                            }
                                            else {
                                                if (empr_POSTransaction.printStatus == "K") {
                                                    empr_POSTransaction.SaveInfo("K");
                                                }
                                                else {
                                                    empr_POSTransaction.SaveInfo("");
                                                    empr_POSTransaction.ThanksMsg();
                                                }
                                            }
                                        }
                                        else {
                                            if (empr_POSTransaction.printStatus == "K") {
                                                empr_POSTransaction.SaveInfo("K");
                                            }
                                            else {
                                                $('#BILL_STATUS').val('P');
                                                empr_POSTransaction.updateSummary();
                                                empr_POSTransaction.SaveInfo("");
                                                empr_POSTransaction.ThanksMsg();
                                            }
                                        }
                                        empr_POSTransaction.ResetAllFields();
                                        empr_POSTransaction.ResetForm();
                                        setTimeout(function () {
                                            $("#Loader").hide();
                                        }, 500);
                                    }, 200);

                                } else {
                                    var msg = "Please Select Required Fields";
                                    empr_helper.notify(msg, 2);
                                }

                            } else {
                                var msg = "The total amount is greater than the entered amount. Please make sure all required fields are filled correctly.";
                                empr_helper.notify(msg, 2);
                            }
                        } else {
                            empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot proceed.", 2);
                        }
                    }


                }



            });


            $('#validateAndOpenModal').on('click', function () {
                if (empr_POSTransaction.salesmanReq == 'Y') {
                    var value = $("#SalesmanName").dxSelectBox("instance").option("value");
                    if (!value) {
                        empr_helper.notify('Please Select First Salesman!', 2);
                        return;
                    }
                }

                // Check if points card is active
                let hasCardPoints = $(".points-section").is(":visible") && $("#slider").attr("max") > 0;
                let prevPoints = parseInt($("#prevPoints").text()) || 0;
                let currentPoints = parseInt($("#currentPoints").text()) || 0;
                let maxPoints = parseInt($("#slider").attr("max")) || 100;
                let totalPoints = prevPoints + currentPoints;

                if (hasCardPoints && totalPoints >= maxPoints) {
                    swal({
                        title: 'Hurry Up!',
                        text: `Do you want to use your ${totalPoints}  discount?`,
                        type: 'success',
                        showCancelButton: true,
                        confirmButtonColor: '#0CC27E',
                        cancelButtonColor: '#FF586B',
                        confirmButtonText: 'Yes, use it!',
                        cancelButtonText: 'No, maybe later',
                        confirmButtonClass: 'btn btn-success mr-5',
                        cancelButtonClass: 'btn btn-danger discountswal',
                        buttonsStyling: false
                    }).then(function () {
                        //  //  
                        // Assign totalPoints from server-side
                        empr_POSTransaction.totalPoints = totalPoints;

                        empr_POSTransaction.totalPayment = parseFloat($("#totalPayment").text());

                        $('#billDiscValue').val(totalPoints);
                        $('#BILL_STATUS').val('P');
                        empr_POSTransaction.updateSummary();
                        //// Calculate discount safely
                        //var discount = empr_POSTransaction.totalPayment < totalPoints ? 0 : empr_POSTransaction.totalPayment - empr_POSTransaction.totalPoints;

                        //$('#FinalAmount').html(`Value ${discount} /-`);

                        setTimeout(() => {
                            empr_POSTransaction.PaymentSetting();
                            $('#TotalPaymentsett').val(parseFloat($("#totalPayment").text()));
                        }, 300);

                        //empr_POSTransaction.updateSummary();
                    });
                    /*$('#usePointsModal').modal('show');*/
                    return;
                }
                empr_POSTransaction.PaymentSetting(undefined, totalPoints);
                $('#TotalPaymentsett').val($("#totalPayment").text());
            });

            $('#confirmUsePoints').on('click', function () {
                $('#usePointsModal').modal('hide');
                empr_POSTransaction.PaymentSetting();
                $('#TotalPaymentsett').val($("#totalPayment").text());
            });

            $('#cancelUsePoints').on('click', function () {
                $('#usePointsModal').modal('hide');
                // Continue without using points
                empr_POSTransaction.PaymentSetting();
                $('#TotalPaymentsett').val($("#totalPayment").text());
            });
            // Ammar Start
            $("#serviceChargesPercent, #serviceChargesValue").on("change", function () {

                empr_POSTransaction.ServiceChargesOnChangeFunction($(this).attr("id"));
                empr_POSTransaction.updateSummary();
            });
            // Ammar End
            $("#discountPercentage, #discountPerUnit").on("change", function () {
                //  //  
                let totalAmount = parseFloat($('#editrate').val()) || 0;
                let percentInput = $("#discountPercentage");
                let valueInput = $("#discountPerUnit");
                let AmountInput = $("#amount");
                if ($(this).attr("id") === "discountPercentage") {
                    // If user changes the percentage input
                    let percentage = parseFloat(percentInput.val()) || 0;
                    let calculatedValue = ((totalAmount * percentage) / 100).toFixed(2);
                    if (totalAmount < 0) {
                        valueInput.val(-Math.abs(calculatedValue));
                    }
                    else {
                        valueInput.val(calculatedValue);
                    }
                    // Update the value input
                    AmountInput.val(totalAmount - calculatedValue);
                } else if ($(this).attr("id") === "discountPerUnit") {
                    // If user changes the value input
                    if (totalAmount < 0) {
                        valueInput.val(-Math.abs(valueInput.val()));
                    }
                    let value = parseFloat(valueInput.val()) || 0;
                    let calculatedPercent = ((value / totalAmount) * 100).toFixed(2);
                    if (totalAmount < 0) {
                        percentInput.val(-Math.abs(calculatedPercent));
                    }
                    else {
                        percentInput.val(calculatedPercent);
                    }

                    AmountInput.val(totalAmount - value);// Update the percentage input
                }

                empr_POSTransaction.updateSummary(null);

            });

            $("#itemQuantity").on("change", function () {

                let totalQuantity = parseFloat($('#itemQuantity').val()) || 0;
                if (totalQuantity == 0) {
                    totalQuantity = 1;
                    $('#itemQuantity').val(totalQuantity);
                }
                let GetAmount = parseFloat($('#amount').val()) || 0;
                let totalAmount = totalQuantity * Math.abs(GetAmount); // GetAmount ko absolute lete hain

                // Condition ko adjust karte hain
                if (totalQuantity < 0) {
                    $('#amount').val(-Math.abs(totalAmount)); // Negative value bhejte hain
                } else {
                    $('#amount').val(Math.abs(totalAmount)); // Positive value bhejte hain
                }
            });

            $("#billDiscPercent, #billDiscValue, #billDisc ,#isBillDiscPercent").on("change keyup", function () {
                //  //  

                empr_POSTransaction.DiscountOnChangeFunction($(this).attr("id"));
                empr_POSTransaction.updateSummary();
                var TotalAmount = parseFloat($('#totalAmountGet').val()) || 0;
                var DelPercent = parseFloat($('#billDelPercent').val()) || 0;
                var calculatedValue = 0;

                if (DelPercent != 0) {
                    calculatedValue = (TotalAmount * DelPercent) / 100;
                    $('#billDelValue').val(parseInt(calculatedValue));
                }
                empr_POSTransaction.PaymentMode();


            });

            $("#billDelPercent, #billDelValue").on("change", function () {
                //  //  
                empr_POSTransaction.DeliveryOnChangeFunction($(this).attr("id"));
                empr_POSTransaction.updateSummary();
                //var TotalAmount = parseFloat($('#totalAmountGet').val()) || 0;
                //var DelPercent = parseFloat($('#billDelPercent').val()) || 0;
                //var calculatedValue = 0;

                //if (DelPercent != 0) {
                //    calculatedValue = (TotalAmount * DelPercent) / 100;
                //    $('#billDelValue').val(parseInt(calculatedValue));
                //}

            });

            $('#BtnQuickSearch').on('click', function () {

                $('.SendBtn').css('display', 'none');
                empr_POSTransaction.InitQuickSearchGrid();

            });

            $('#PayQuickSearch').on('click', function () {
                $('.SendBtn').css('display', 'none');
                empr_POSTransaction.InitPayQuickSearchGrid();

            });

            $('#AdvanceBtn').on('click', function () {
                $('.SendBtn').css('display', '');
                ajaxHelper.ajaxGetJson('/POSTransactions/AdvanceBookingRecords', function (data) {
                    if (data.msgType == 1) {
                        $('#AdvanceModal').modal('show');
                        empr_POSTransaction.AdvanceBookingGrid(data.data);
                    }
                    else {
                        empr_helper.notify(data.msg, data.msgType);
                    }
                }, false, true);

            });

            $('#kot').on('click', function () {
                //const orderTable = document.getElementById("orderTable");
                //if (orderTable.rows.length > 0) {
                //    $('#Loader').appendTo('body');
                //    $("#Loader").css({
                //        display: 'flex'
                //    });
                //    setTimeout(function () {
                //        if ($('.stock').text() == "" || !BarcodeTextBoxVisible) {
                //            if ($('#BILL_STATUS').val() != 'P') {
                //                $('#IsKot').val(1);
                //                if (empr_POSTransaction.return != true) {
                //                    $('#BILL_STATUS').val('K');
                //                    empr_POSTransaction.billStatus = "K";
                //                    $('#recvhidden').val('');
                //                    if (Permissions != "Admin") {
                //                        if (!$("#Code").val() && !Permissions.r_ADD) {
                //                            empr_helper.notify("You are not allowed to add new record !", 2);
                //                        }
                //                        else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                //                            empr_helper.notify("You are not allowed to edit records !", 2);
                //                        } else {
                //                            empr_POSTransaction.ResetAllFields();
                //                            empr_POSTransaction.PrintModal(36, empr_POSTransaction.billStatus);
                //                        }
                //                    } else {
                //                        empr_POSTransaction.ResetAllFields();
                //                        empr_POSTransaction.PrintModal(36, empr_POSTransaction.billStatus);
                //                        empr_POSTransaction.updateSummary();

                //                    }
                //                }
                //            }
                //            else {
                //                empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);

                //            }
                //        }
                //        else {
                //            empr_helper.notify("Insufficient inventory! Please restock before processing sales.", 2);
                //        }
                //        setTimeout(function () {
                //            $("#Loader").hide();
                //        }, 500);
                //    }, 200);

                //} else {
                //    var msg = "Items table is empty. Please add at least one item before proceeding.";
                //    empr_helper.notify(msg, 2);
                //}
                empr_POSTransaction.HoldDataSave(1);
            });

            //$('#payment').on('click', function () {
            //    let advAmount = parseFloat($("#advAmount").val()) || 0;
            //    let advBankAmount = parseFloat($("#advBankAmount").val()) || 0;
            //    let fnlAmount = parseFloat($("#fnlAmount").val()) || 0;
            //    let fnlBankAmount = parseFloat($("#fnlBankAmount").val()) || 0;
            //    let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
            //    let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
            //    let partyAmount = parseFloat($("#partyrecv2").val()) || 0;
            //    let totalEntered;
            //    let totalAmount;
            //    //  //  
            //    empr_POSTransaction.printStatus = "";
            //    $('#IsKot').val(0);

            //    if ((cashAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cardAmount != 0) || (partyAmount != 0 && cashAmount != 0) || empr_POSTransaction.AccountTax == 'Split') {
            //        totalEntered = cashAmount + cardAmount + partyAmount;
            //        totalAmount = $('#totalPayment').text();
            //    }
            //    else if (empr_POSTransaction.payType == 'Advance' && $('#BILL_STATUS').val() == 'P') {
            //        totalEntered = advAmount + advBankAmount + fnlAmount + fnlBankAmount;
            //        totalAmount = $('#totalPayment').text();
            //    }
            //    else {
            //        totalEntered = $('#totalPayment').text();
            //        totalAmount = $('#totalPayment').text();
            //    }


            //    if (totalAmount <= totalEntered) {
            //        if (empr_POSTransaction.Checkvalidation(totalAmount, totalEntered)) {
            //            $('#paymentModal').modal('hide');
            //            $('#Loader').appendTo('body'); // modal ke upar le jao
            //            $("#Loader").css({
            //                display: 'flex'
            //            });

            //            setTimeout(function () {
            //                if (Permissions != "Admin") {

            //                    if (!$("#Code").val() && !Permissions.r_ADD) {
            //                        empr_helper.notify("You are not allowed to add new record !", 2);
            //                    }
            //                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
            //                        empr_helper.notify("You are not allowed to edit records !", 2);
            //                    }
            //                    else {
            //                        if (empr_POSTransaction.printStatus == "K") {
            //                            empr_POSTransaction.SaveInfo("K");
            //                        }
            //                        else {
            //                            empr_POSTransaction.SaveInfo("");
            //                            empr_POSTransaction.ThanksMsg();
            //                        }
            //                    }
            //                }
            //                else {
            //                    if (empr_POSTransaction.printStatus == "K") {
            //                        empr_POSTransaction.SaveInfo("K");
            //                    }
            //                    else {
            //                        empr_POSTransaction.SaveInfo("");
            //                        empr_POSTransaction.ThanksMsg();
            //                    }
            //                }
            //                empr_POSTransaction.ResetAllFields();
            //                empr_POSTransaction.ResetForm();
            //                setTimeout(function () {
            //                    $("#Loader").hide();
            //                }, 500);
            //            }, 200);

            //        } else {
            //            var msg = "Please Select Required Fields";
            //            empr_helper.notify(msg, 2);
            //        }

            //    } else {
            //        var msg = "The total amount is greater than the entered amount. Please make sure all required fields are filled correctly.";
            //        empr_helper.notify(msg, 2);
            //    }

            //});

            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                empr_POSTransaction.ResetForm(id);
                empr_POSTransaction.ResetAllFields();
                $('#Code').val(id);
                $('.modal').modal('hide');
                $('#Loader').appendTo('body');
                $("#Loader").css({
                    display: 'flex'
                });
                setTimeout(function () {
                    empr_POSTransaction.GetPOSTransactionByCode(id);

                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 800);

                }, 400);

            });

            $('body').on('click', '.elm_pay', function () {
                var id = $(this).attr("reportid");
                empr_POSTransaction.ResetForm(id);
                empr_POSTransaction.ResetAllFields();
                $('#Code').val(id);
                $('.modal').modal('hide');
                $('#Loader').appendTo('body');
                $("#Loader").css({
                    display: 'flex'
                });
                setTimeout(function () {
                    empr_POSTransaction.GetPOSTransactionByCode(id);
                    setTimeout(function () {
                        $("#Loader").hide();
                    }, 800);
                }, 400);

                let waitForData = setInterval(() => {
                    if (empr_POSTransaction.dataLoaded === true) {
                        clearInterval(waitForData);
                        empr_POSTransaction.PaymentSetting();
                        empr_POSTransaction.dataLoaded = false;
                    }
                }, 200);

            });

            $('body').on('click', '.elm_print', function () {
                var id = $(this).attr("reportid");
                ajaxHelper.ajaxPostJsonData({ tranId: id }, "/POSTransactions/PrintModal", function (data) {
                    if (data.msgType == 1) {

                        $('#slipPreview').html(data.slipHtml);

                        const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                        const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                        if (qrCodeImg !== null && srbcodeimg !== null) {
                            if (qrCodeImg.complete && srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    if (srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                                srbcodeimg.onload = () => {
                                    if (qrCodeImg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                            }
                        } else if (qrCodeImg !== null) {
                            if (qrCodeImg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else if (srbcodeimg !== null) {
                            if (srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                srbcodeimg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else {
                            empr_POSTransaction.printContent();
                        }

                    }
                }, false, true);
            });

            $('body').on('click', '.elm_PayPrint', function () {
                var id = $(this).attr("reportid");
                $('#QuickSearchModal').modal('hide');
                //var dataModels = empr_POSTransaction.GetDataToSave(undefined, "");
                ajaxHelper.ajaxPostJsonData({ tranId: id }, "/POSTransactions/PrintModal", function (data) {
                    //  //  
                    /*empr_helper.notify(data.msg, data.msgType);*/
                    if (data.msgType == 1) {

                        $('#slipPreview').html(data.slipHtml);

                        const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                        const srbcodeimg = document.querySelector('#slipPreview img.srb-code'); // Corrected the selector to match the casing

                        if (qrCodeImg !== null && srbcodeimg !== null) {
                            if (qrCodeImg.complete && srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    if (srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                                srbcodeimg.onload = () => {
                                    if (qrCodeImg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                            }
                        } else if (qrCodeImg !== null) {
                            if (qrCodeImg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else if (srbcodeimg !== null) {
                            if (srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                srbcodeimg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else {
                            empr_POSTransaction.printContent();
                        }

                    }
                }, false, true);
            });

            $('body').on('click', '#BtnDelete', function () {
                empr_POSTransaction.Delete();
            });

            $('body').on('click', '#BtnNew', function () {
                empr_POSTransaction.ResetForm();
            });

            $('body').on('click', '#BtnAddSodaToDelivery', function () {
                var selectedSodas = $('#SodaPickGridContainer').dxDataGrid('instance').getSelectedRowKeys();
                if (selectedSodas.length > 0) {
                    empr_POSTransaction.AddSodaToDelivery();
                }
                else {
                    empr_helper.notify("Please select the items first.", 2);
                }
            });

            $('body').on('click', '#BtnPrint,#BtnGenerateReport', function () {
                empr_POSTransaction.GeneratePrintReport();
            });

            $('#CASH').on('input', function () {
                empr_POSTransaction.CalculateCashBack();
            });

            $('#SETT').on('input', function () {
                empr_POSTransaction.Calculate();
            });

            $('#print').on('click', function () {

                printInvoice();
                closeModal();
            });

            $('#CMOB').on('keyup change', function () {

                var number = String($('#CMOB').val());
                var user = empr_POSTransaction.CustomerName.filter(b => b.number == number);
                if (user.length > 0) {
                    var item = user[0];
                    $('#CNAME').val(item.name);
                    $('#CADD').val(item.address);
                }
                else {
                    $('#CNAME').val('');
                    $('#CADD').val('');
                }
            });

            $(document).on('keydown', '#barcode', function (e) {
                //  
                if (e.key === 'Enter') {
                    const barcode = $(this).val();

                    if (barcode !== '') {
                        e.preventDefault();
                        $('#barcodeEnter').click();
                    }
                }
            });

            $('#barcodeEnter').on('click', function () {
                var rights = empr_POSTransaction.userRights;
                var number;
                if (BarcodeTextBoxVisible) {
                    number = $('.barcodevisible').val().trim();
                } else {
                    number = $('.qrCodevisible').val().trim();
                }
                if (number != '') {
                    const specialCharPattern = /[^a-zA-Z0-9]/;
                    if (specialCharPattern.test(number)) {
                        number = number.toString();
                    } else {
                        number = parseInt(number, 10);
                    }
                    var barcode = Barcodes.filter(b => b.barcode == number);
                    if (barcode.length > 0) {
                        if ($('#BILL_STATUS').val() != 'P' || empr_POSTransaction.return != true) {
                            var item = barcode[0];
                            barcodeCode = item.key;

                            empr_POSTransaction.loadItemsForData(item.itemcode, barcodeCode, item.barcode, 1);
                            $('#barcode').val('');
                        }
                        else {
                            empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                        }
                    }
                    else {
                        if (number.includes('/')) {
                            ajaxHelper.ajaxGetJson('/POSTransactions/GetReturn', function (data) {
                                if (data.msgType == 1) {
                                    var voucher = data.data.filter(b => b.voucher == number && b.bill == 'P');
                                    if (voucher.length > 0) {
                                        if (rights) {
                                            if (rights.bReturn === 1) {
                                                empr_helper.notify("You are Not allowed To Bill Wise Return!", 2);
                                                return;
                                            }
                                        }
                                        empr_POSTransaction.ResetForm();
                                        empr_POSTransaction.return = true;
                                        empr_POSTransaction.GetPOSTransactionByCode(undefined, voucher[0].voucher);

                                    } else {
                                        ajaxHelper.ajaxGetJson('/POSTransactions/GetAdvance', function (data) {
                                            if (data.msgType == 1) {
                                                var vouchers = data.data.filter(b => b.voucher == number && b.bill == 'P' && b.complete == 0);
                                                if (vouchers.length > 0) {
                                                    empr_POSTransaction.ResetForm();
                                                    empr_POSTransaction.GetPOSTransactionByCode(undefined, vouchers[0].voucher);
                                                    let waitForData = setInterval(() => {
                                                        if (empr_POSTransaction.dataLoaded === true) {
                                                            clearInterval(waitForData);
                                                            empr_POSTransaction.updateSummary();
                                                            setTimeout(() => {
                                                                empr_POSTransaction.PaymentSetting();
                                                            }, 300);
                                                            empr_POSTransaction.dataLoaded = false;
                                                        }
                                                    }, 200); // checks every 200ms
                                                    $('#barcodevisible').val('');
                                                    $('#barcode').val('');
                                                    $('#qrCodevisible').val('');
                                                } else {
                                                    empr_helper.notify("Invalid voucher Number", 2);
                                                }
                                            }

                                        }, false, true);
                                    }
                                }

                            }, false, true);
                        }
                        else {

                        }

                    }
                }



            });

            $(document).on('click', '.barcodeBox', function () {
                $(this).toggleClass('selected'); // Add/Remove blue
            });

            $("#CardDisc").on("keydown", function (e) {
                if (e.key === 'Enter') {
                    let code = $(this).val().trim();

                    empr_POSTransaction.CardDicount(code);
                }
            });

            $(document).on('click', '.warehouse-icon', function () {
                const itemId = $(this).data('id');
                const quantityInput = $(this).siblings('.quantity-input');
                const currentQty = quantityInput.val();

                alert("Warehouse icon clicked for item ID: " + itemId + "\nCurrent Quantity: " + currentQty);

            });

            $('#printButton').on('click', function () {

                if (empr_POSTransaction.printStatus == "K") {
                    empr_POSTransaction.SaveInfo("K");
                }
                else {
                    empr_POSTransaction.SaveInfo("");
                }

                empr_POSTransaction.updateSummary();
                empr_POSTransaction.ResetForm();
                empr_POSTransaction.ResetAllFields();


            });

            var sizes;
            $(document).on('click', '.size-box', function () {

                sizes = $(this);
                var size = sizes.data('size');
                var items = empr_POSTransaction.sizeGroups[size];

                $('#modalSize').text(size);

                $('#colorOptions').empty();

                items.forEach(function (item) {
                    var isChecked = false;

                    empr_POSTransaction.selectedItems.forEach(function (selected) {
                        var bar = selected.barcode; // assuming selected item has barcode
                        if (item.barcode === bar) {
                            isChecked = true;
                            return;
                        }
                    });

                    var isChecked = empr_POSTransaction.selectedItems.some(function (selected) {
                        return selected.code === item.code;
                    });

                    // Quantity restore bhi karni ho toh:
                    var qty = 1;
                    var found = empr_POSTransaction.selectedItems.find(x => x.code === item.code);
                    if (found) {
                        qty = found.quantity;
                    }

                    // Build HTML for the item
                    var html = `
                        <div class="color-option">
                            <input type="checkbox" for="size-${item.size}" id="color-${item.code}" 
                                   value="${item.code}" class="color-checkbox ${item.size}" ${isChecked ? 'checked' : ''}>
                            <input type="number" id="barQty" class="form-control qty-input"
                                   value="${qty}" style="width: 43px; margin-left: 8px;"> 
                            <label for="color-${item.code}">
                                <span class="color-circle" style="background-color: ${empr_POSTransaction.getColorCode(item.color)
                        }"></span>
                                ${item.barcode}
                            </label>
                        </div>
                    `;

                    $('#colorOptions').append(html);
                });

                $('#colorModal').modal('show');
            });

            $(document).on("change", ".color-checkbox", function () {

                var code = $(this).val();
                var size = $(this).attr("class").split(" ")[1]; // e.g. "42"
                var qty = $(this).closest('.color-option').find('.qty-input').val();

                // Barcode ka record find karo
                for (const sz in empr_POSTransaction.sizeGroups) {
                    let items = empr_POSTransaction.sizeGroups[sz];
                    let matched = items.filter(x => x.code === parseInt(code));

                    if (matched.length > 0) {
                        if ($(this).is(":checked")) {
                            empr_POSTransaction.selectedItems = empr_POSTransaction.selectedItems.filter(x => x.code !== parseInt(code));

                            matched.forEach(function (item) {
                                empr_POSTransaction.selectedItems.push({
                                    size: sz,
                                    iteM_CODE: item.iteM_CODE,
                                    code: item.code,
                                    barcode: item.barcode,
                                    quantity: parseInt(qty) || 1
                                });
                            });
                        } else {
                            empr_POSTransaction.selectedItems = empr_POSTransaction.selectedItems.filter(x => x.code !== parseInt(code));
                        }
                    }
                }
            });

            $(document).on("input", ".qty-input", function () {
                var $parent = $(this).closest(".color-option");
                var $checkbox = $parent.find(".color-checkbox:checked");

                if ($checkbox.length > 0) {
                    var code = $checkbox.val();
                    var size = $checkbox.attr("class").split(" ")[1]; // e.g. "42"
                    var qty = $(this).val();

                    for (const sz in empr_POSTransaction.sizeGroups) {
                        let items = empr_POSTransaction.sizeGroups[sz];
                        let matched = items.filter(x => x.code === parseInt(code));

                        if (matched.length > 0) {
                            empr_POSTransaction.selectedItems = empr_POSTransaction.selectedItems.filter(x => x.code !== parseInt(code));

                            matched.forEach(function (item) {
                                empr_POSTransaction.selectedItems.push({
                                    size: sz,
                                    iteM_CODE: item.iteM_CODE,
                                    code: item.code,
                                    barcode: item.barcode,
                                    quantity: parseInt(qty) || 1
                                });
                            });
                        }
                    }
                }
            });

            $('#selectColor').click(function () {
                //  //  
                if (empr_POSTransaction.selectedItems.length > 0) {
                    empr_POSTransaction.selectedItems.forEach(function (item) {
                        empr_POSTransaction.loadItemsForData(
                            item.iteM_CODE,
                            item.code,
                            item.barcode,
                            item.quantity,
                            undefined,
                            item.size
                        );
                    });

                    $('#colorModal').modal('hide');
                    $('#BarcodePickModal').modal('hide');
                    $('#barcodeComplete').css('display', '');
                    sizes.addClass('selected');
                    $('#colorOptions').empty();
                } else {
                    if (!$('#paymentModal').is(':visible'))
                        empr_helper.notify("Please select at least one barcode.", 2);
                }
            });

            $('#barcodeComplete').click(function () {
                if (empr_POSTransaction.selectedItems.length > 0) {
                    empr_POSTransaction.selectedItems.forEach(function (item) {
                        empr_POSTransaction.loadItemsForData(
                            item.iteM_CODE,
                            item.code,
                            item.barcode,
                            item.quantity
                        );
                    });

                    $('#BarcodePickModal').modal('hide');
                    sizes.addClass('selected');
                    $('#colorOptions').empty();
                } else {
                    if (!$('#paymentModal').is(':visible'))
                        empr_helper.notify("Please select at least one barcode.", 2);
                }
            });

            $('#backColor').click(function () {
                $('#colorModal').modal('hide');
                var selected = [];

                $(".modal-body .color-checkbox:checked").each(function () {
                    var $row = $(this).closest(".color-option");

                    var barcode = $row.find("label").text().trim();
                    var qty = $row.find(".qty-input").val();

                    selected.push({
                        barcode: barcode,
                        qty: qty
                    });
                });

                if (selected.length > 0) {
                    sizes.addClass('selected');
                } else {
                    sizes.removeClass('selected');
                }
                $('#colorOptions').empty();
            });
        });
    },
    //uzair work here

    SignlocalStorage() {
        var savedSign = localStorage.getItem("SettleSign");

        if (savedSign === "+" || savedSign === "-") {

            $("#SettleSign").val(savedSign);

            if (savedSign === "+") {
                $("#Plus").addClass("active");
                $("#minus").removeClass("active");
                empr_POSTransaction.settlsign = 'P';
            } else {
                $("#minus").addClass("active");
                $("#Plus").removeClass("active");
                empr_POSTransaction.settlsign = 'M';
            }
        }
    },

    PaymentMode: function () {

        let typingTimer; // Timer for debounce
        const typingInterval = 500;// Replace with dynamic total if needed
        const typingInterval2 = 500;// Replace with dynamic total if needed
        var bankAmtwithtax = 0;
        var cashAmtwithtax = 0;
        var partyAmtWithtax = 0;

        $(document).on('input change',
            '.quantity-input, .total-price, .discountpercent,.discountvalue ,.tax',
            function () {

                //  

                //// agar tumhara existing function hai
                //if (typeof updateOrderTotal === "function") {
                //    updateOrderTotal();
                //}
                //empr_POSTransaction.updateSummary();
                recalculatePayment();
            });
        // Monitor changes in all relevant input fields
        $("#cashAmount2, #bankrecv2, #partyrecv2 ").on("input", function () {
            //  //  
            const totalAmount = $('#totalPayment').text();
            let TotalTax = 0;
            let amount = 0;
            let baqia = 0;

            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);

            typingTimer = setTimeout(function () {

                let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
                let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
                let partyAmount = parseFloat($("#partyrecv2").val()) || 0;
                let cashtax = 0;
                let banktax = 0;
                let partytax = 0;

                // Calculate remaining amount
                let totalEntered = cashAmount + cardAmount + partyAmount;
                let remaining = totalEntered - totalAmount;


                // Validate and adjust amounts
                if ($('#totalPayment').text() <= totalEntered) {
                    if (input.attr("id") === "cashAmount2") {
                        amount = cardAmount + partyAmount;
                        baqia = totalAmount - amount;
                        if (amount < totalAmount) {
                            input.val(baqia);
                            cashtaxAmt = Math.round(baqia * cashtax / 100);
                            $('#CashTax_Amt2').val(cashtaxAmt);
                            cashAmtwithtax = baqia;
                            $('#cashtaxAmt2').val(cashAmtwithtax);
                        } else {
                            input.val(baqia);
                            cashtaxAmt = Math.round(baqia * cashtax / 100);
                            $('#CashTax_Amt2').val(cashtaxAmt);
                            cashAmtwithtax = baqia;
                            $('#cashtaxAmt2').val(cashAmtwithtax);
                        }
                    } else if (input.attr("id") === "bankrecv2") {

                        amount = cashAmount + partyAmount;
                        baqia = totalAmount - amount;
                        if (amount < totalAmount) {
                            input.val(baqia);
                            banktaxAmt = Math.round(baqia * banktax / 100);
                            $('#BankTax_Amt2').val(banktaxAmt);
                            bankAmtwithtax = baqia;
                            $('#banktaxAmt2').val(bankAmtwithtax);
                        }
                        else {
                            input.val(baqia);
                            banktaxAmt = Math.round(baqia * banktax / 100);
                            $('#BankTax_Amt2').val(banktaxAmt);
                            bankAmtwithtax = baqia;
                            $('#banktaxAmt2').val(bankAmtwithtax);
                        }
                    } else if (input.attr("id") === "partyrecv2") {
                        //  //  
                        amount = cashAmount + cardAmount;
                        baqia = totalAmount - amount;
                        if (amount < parseInt(totalAmount)) {
                            input.val(baqia);
                            partytaxAmt = Math.round(baqia * partytax / 100);
                            $('#PartyTax_Amt2').val(partytaxAmt);
                            partyAmtwithtax = baqia;
                            $('#partytaxAmt2').val(partyAmtwithtax);
                        }
                        else {
                            input.val(baqia);
                            partytaxAmt = Math.round(baqia * partytax / 100);
                            $('#PartyTax_Amt2').val(partytaxAmt);
                            partyAmtwithtax = baqia;
                            $('#partytaxAmt2').val(partyAmtwithtax);
                        }
                    }
                } else {
                    // Update placeholder text for remaining fields
                    $("#cashAmount2").attr("placeholder", `Max: ${remaining}`);
                    cashtaxAmt = Math.round(cashAmount * cashtax / 100);
                    $('#CashTax_Amt2').val(cashtaxAmt);
                    cashAmtwithtax = cashAmount;
                    $("#bankrecv2").attr("placeholder", `Max: ${remaining}`);
                    banktaxAmt = Math.round(cardAmount * banktax / 100);
                    $('#BankTax_Amt2').val(banktaxAmt);
                    bankAmtwithtax = cardAmount;
                    $("#partyrecv2").attr("placeholder", `Max: ${remaining}`);
                    partytaxAmt = Math.round(partyAmount * partytax / 100);
                    $('#PartyTax_Amt2').val(partytaxAmt);
                    partyAmtWithtax = partyAmount;

                }
                var party = parseInt($('#partyrecv2').val() || 0);

                var Total = cashAmtwithtax + bankAmtwithtax + party;

                $('#TotalAmtWithTax').val(Total); // keeps 2 decimals
                $('#recvhidden').val(Total);
                //TotalTax = parseInt(totalAmount) + parseInt(cashtaxAmt) + parseInt(banktaxAmt) + parseInt(partytaxAmt);
                //$('#totalPayment').text(TotalTax);
            }, typingInterval2);

        });
        $('#billDisc,#isBillDiscPercent').on('input change keyup', function () {
            console.log("billDisc changed");
            //  

            empr_POSTransaction.updateSummary(null);
            recalculatePayment();
        });

        //bankrecv
        $("#partyrecv").on("input", function () {

            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            $('#Returnparty').val('');
            typingTimer = setTimeout(function () {
                let value = parseFloat(input.val());
                let amount = parseFloat($('#totalPayment').text());
                // Validate after user finishes typing
                if (value < amount) {
                    input.val(amount);
                    /*$('#cashReturn').val(value - amount);*/// Set it to the max allowable amount
                }
                else {
                    $('#Returnparty').val(amount - value);
                }

            }, typingInterval);
        });


        $("#cashAmount").on("input", function () {

            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            $('#cashReturn').val('');
            typingTimer = setTimeout(function () {
                let value = parseFloat(input.val());
                let amount = parseFloat($('#totalPayment').text());

                // Validate after user finishes typing
                if (value < amount) {
                    input.val(amount);
                    /*$('#cashReturn').val(value - amount);*/// Set it to the max allowable amount
                }
                else {
                    $('#cashReturn').val(amount - value);
                }

            }, typingInterval);
        });
        $("#advAmount").on("input", function () {

            $('#recvhidden').val(0);
            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            typingTimer = setTimeout(function () {
                let cashvalue = parseFloat(input.val()) || 0;
                let bankvalue = parseFloat($('#advBankAmount').val()) || 0;
                let value = cashvalue + bankvalue;
                let amount = parseFloat($('#totalPayment').text()) || 0;

                // Validate after user finishes typing
                if (value >= amount) {
                    $('#advAmount').val(amount - bankvalue);
                }

            }, typingInterval);
        });
        $("#advBankAmount").on("input", function () {

            $('#recvhidden').val(0);
            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            typingTimer = setTimeout(function () {
                let bankvalue = parseFloat(input.val()) || 0;
                let cashvalue = parseFloat($('#advAmount').val()) || 0;
                let value = cashvalue + bankvalue;
                let amount = parseFloat($('#totalPayment').text());

                // Validate after user finishes typing
                if (value >= amount) {
                    $('#advBankAmount').val(amount - cashvalue);
                }

            }, typingInterval);
        });
        $("#fnlAmount").on("input", function () {

            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            typingTimer = setTimeout(function () {
                let bankvalue = parseFloat($('#advBankAmount').val()) || 0;
                let cashvalue = parseFloat($('#advAmount').val()) || 0;
                let fnlvalue = parseFloat(input.val()) || 0;
                let fnlbankvalue = parseFloat($('#fnlBankAmount').val()) || 0;
                let subValue = cashvalue + bankvalue + fnlbankvalue;
                let value = cashvalue + bankvalue + fnlvalue + fnlbankvalue;
                let amount = parseFloat($('#totalPayment').text());

                // Validate after user finishes typing
                if (value >= amount) {
                    $('#fnlAmount').val(amount - subValue);
                    $('#completecheck').prop({
                        'checked': true,
                        'readonly': true,
                        'disabled': true
                    }).closest('.form-check').addClass('disabled');
                    $('#complete').val(1)
                }
                else {
                    $('#completecheck').prop({
                        'checked': false,
                        'readonly': true,
                        'disabled': true
                    }).closest('.form-check').addClass('disabled');
                    $('#complete').val(0)
                }

            }, typingInterval);
        });
        $("#fnlBankAmount").on("input", function () {

            clearTimeout(typingTimer); // Clear previous timer
            let input = $(this);
            typingTimer = setTimeout(function () {
                let bankvalue = parseFloat($('#advBankAmount').val()) || 0;
                let cashvalue = parseFloat($('#advAmount').val()) || 0;
                let fnlBankvalue = parseFloat(input.val()) || 0;
                let fnlvalue = parseFloat($('#fnlAmount').val()) || 0;
                let subValue = cashvalue + bankvalue + fnlvalue;
                let value = cashvalue + bankvalue + fnlvalue + fnlBankvalue;
                let amount = parseFloat($('#totalPayment').text());

                // Validate after user finishes typing
                if (value >= amount) {
                    $('#fnlBankAmount').val(amount - subValue);
                    $('#completecheck').prop({
                        'checked': true,
                        'readonly': true,
                        'disabled': true
                    }).closest('.form-check').addClass('disabled');
                    $('#complete').val(1)
                }
                else {
                    $('#completecheck').prop({
                        'checked': false,
                        'readonly': true,
                        'disabled': true
                    }).closest('.form-check').addClass('disabled');
                    $('#complete').val(0)
                }

            }, typingInterval);
        });

        $("#delDate").on("input", function () {
            setTimeout(function () {
                const today = new Date();
                const formattedDate = today.toISOString().split('T')[0];
                const delDate = document.getElementById('delDate');

                if (delDate.value < formattedDate) {
                    empr_helper.notify("You cannot select a past date!", 2);
                    delDate.value = formattedDate;
                }
            }, 500);
        });


        // Select buttons and payment sections (keep existing IDs for backend logic)
        const btnCash = document.getElementById("btnCash");
        const btnCard = document.getElementById("btnCard");
        const btnParty = document.getElementById("btnParty");
        const btnSplit = document.getElementById("btnSplit");

        const cashFields = document.getElementById("cashFields");
        const cardFields = document.getElementById("cardFields");
        const partyFields = document.getElementById("partyFields");
        const splitFields = document.getElementById("splitFields");
        const advanceFields = document.getElementById("advanceFields"); // legacy (no UI tab)

        const POS_PAYMENT_TABS = [
            { key: "Cash", btn: btnCash, panel: cashFields },
            { key: "Card", btn: btnCard, panel: cardFields },
            { key: "Party", btn: btnParty, panel: partyFields },
            { key: "Split", btn: btnSplit, panel: splitFields }
        ];

        function posSetActivePaymentTab(key) {
            const k = (key || "Cash").toString().trim().toLowerCase();
            POS_PAYMENT_TABS.forEach(t => {
                const isActive = t.key.toLowerCase() === k;
                if (t.btn) {
                    t.btn.classList.toggle("selected", isActive);
                    t.btn.setAttribute("aria-selected", isActive ? "true" : "false");
                }
                if (t.panel) {
                    if (isActive) {
                        t.panel.hidden = false;
                        t.panel.classList.remove("d-none");
                        requestAnimationFrame(() => t.panel.classList.add("pos-panel-active"));
                        t.panel.setAttribute("aria-hidden", "false");
                    } else {
                        t.panel.classList.remove("pos-panel-active");
                        t.panel.setAttribute("aria-hidden", "true");
                        // allow fade-out before hiding
                        setTimeout(() => {
                            t.panel.hidden = true;
                            t.panel.classList.add("d-none");
                        }, 180);
                    }
                }
            });
        }

        function posActivateAdvanceLegacy() {
            // Advance UI tab removed; keep backend restore flow safe if old records still send payType=Advance
            if (!advanceFields) {
                // fallback to Cash if Advance panel isn't present
                document.getElementById("btnCash")?.click();
                return;
            }
            $(".settlement-box").hide();
            $('#settlementInput').val('');
            empr_POSTransaction.payType = 'Advance';
            empr_POSTransaction.AccountTax = "Advance";
            MultipleTaxValue = $('#cashtax').val();
            TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * MultipleTaxValue) / 100);
            $('#CashTax_Amt2').val(TaxAmt);
            TotalAmtWithTax = Math.round(parseFloat($('#TotalPaymenthidden').val()) + TaxAmt);
            var CardAmt = empr_POSTransaction.totalPoints
            var payment = TotalAmtWithTax;
            $('#totalPayment').text(payment);
            $('#recvhidden').val(payment);
            empr_POSTransaction.CardDisc = CardAmt;
            $('#recvhidden').val(0);
            if ($('#BILL_STATUS').val() != 'P') {
                $('#fnlAmount').prop('readonly', true);
                $('#fnlBankAmount').prop('readonly', true);
            }
            empr_POSTransaction.hideAllFields();
            empr_POSTransaction.ResetAllFields();
            advanceFields.hidden = false;
            advanceFields.classList.remove("d-none");
            requestAnimationFrame(() => advanceFields.classList.add("pos-panel-active"));
            advanceFields.setAttribute("aria-hidden", "false");
        }
        function recalculatePayment() {
             
            $(".settlement-box").show();

            let type = empr_POSTransaction.payType || 'Cash';

            let taxSelector = '';
            let taxField = '';
             

            if (type === 'Cash') {
                taxSelector = '#cashtax';
                taxField = '#CashTax_Amt2';
            }
            else if (type === 'Bank') {
                taxSelector = '#banktax';
                taxField = '#BankTax_Amt2';
            }
            else if (type === 'Party') {
                taxSelector = '#partytax';
                taxField = '#PartyTax_Amt2';
            }
            else if (type === 'Split') {
                taxSelector = '#cashtax';
            }

            let total = parseFloat($('#totalPayment').text()) || 0;
            let tax = parseFloat($(taxSelector).val()) || 0;

            let TaxAmt = Math.round((total * tax) / 100);

            if (taxField)
                $(taxField).val(TaxAmt);

            let TotalAmtWithTax = Math.round(total + TaxAmt);

            let CardAmt = empr_POSTransaction.totalPoints || 0;
            empr_POSTransaction.CardDisc = CardAmt;

            let payment = TotalAmtWithTax - CardAmt;

            let sett = Math.abs(parseInt($('#settlementInput').val())) || 0;

            let final = (empr_POSTransaction.settlsign === 'P')
                ? (payment + sett)
                : (payment - sett);

            // 🎯 UI Update
            $('#totalPayment').text(payment);
            $('#TotalPaymentsett').val(payment);

            //let TaxWithsett = (empr_POSTransaction.settlsign === 'P')
            //    ? (TotalAmtWithTax + sett)
            //    : (TotalAmtWithTax - sett);

            $('#recvhidden').val(payment);

            if (type === 'Cash') {
                $('#cashAmount').val(payment);
            }
            else if (type === 'Bank') {
                $('#bankrecv').val(payment);
            }
            else if (type === 'Party') {
                $('#partyrecv').val(payment);
            }

            if ($('#BILL_STATUS').val() == 'P') {
                $('#cashAmount, #settlementInput').prop('readonly', true);
                $('#cashReturn').val(empr_POSTransaction.returnCash);
            } else {
                $('#cashAmount, #settlementInput').prop('readonly', false);
                $('#cashReturn').val(0);
            }

            // 🎯 UI cleanup
            //empr_POSTransaction.hideAllFields();
            //empr_POSTransaction.ResetAllFields();

            $("#PARTY_BACCOUNTS").hide();
            $("#party_account").hide();
            empr_POSTransaction.updateSummary();

        }

        btnCash?.addEventListener("click", () => {
            empr_POSTransaction.ResetAllFields();
            empr_POSTransaction.payType = 'Cash';
            empr_POSTransaction.AccountTax = "Cash";
            posSetActivePaymentTab("Cash");
            empr_POSTransaction.updateSummary();

            recalculatePayment();
        });

        btnCard?.addEventListener("click", () => {
             
            empr_POSTransaction.ResetAllFields();

            empr_POSTransaction.payType = 'Bank';
            empr_POSTransaction.AccountTax = "Bank";
            posSetActivePaymentTab("Card");
            empr_POSTransaction.updateSummary();
            recalculatePayment();


            $('#BACCOUNTS').dxSelectBox('option', 'readOnly', false);
        });

        btnParty?.addEventListener("click", () => {
            empr_POSTransaction.ResetAllFields();
            empr_POSTransaction.payType = 'Party';
            empr_POSTransaction.AccountTax = "Party";
            //  
            posSetActivePaymentTab("Party");
            empr_POSTransaction.updateSummary();

            recalculatePayment();

            if (empr_POSTransaction.billMode === 'Card') {
                $("#PARTY_BACCOUNTS, #party_account").show();
            }
        });

        btnSplit?.addEventListener("click", () => {
            empr_POSTransaction.ResetAllFields();
            empr_POSTransaction.payType = 'Split';
            empr_POSTransaction.AccountTax = "Split";
            posSetActivePaymentTab("Split");
            empr_POSTransaction.updateSummary();
            recalculatePayment();
            const dropdownInstancebanks = $("#BACCOUNTS2").dxSelectBox("instance");
            dropdownInstancebanks.reset();
        });

        ajaxHelper.ajaxGetJson('/POSTransactions/GetBanksName', function (data) {
            empr_POSTransaction.bindDxDdl("bankName", data, null, "key", "value", "Select", function (d) {
                var SelectedId = d.value;
                var selectedItem = data.find(item => item.key === SelectedId);
                var selectedText = selectedItem ? selectedItem.value : "";
                $('#bankhidden').val(d.value);
                $('#banknamehidden').val(selectedText);
            });
            empr_POSTransaction.bindDxDdl("bankName2", data, null, "key", "value", "Select", function (d) {
                var SelectedId = d.value;
                var selectedItem = data.find(item => item.key === SelectedId);
                var selectedText = selectedItem ? selectedItem.value : "";
                $('#bankhidden2').val(d.value);
                $('#banknamehidden2').val(selectedText);
            });
        });
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPartyName', function (data) {
            empr_POSTransaction.Partydata = data;
            empr_POSTransaction.bindDxDdl_New("partyName", data, null, "key", "value", "Select", function (d) {
                //  //  
                var SelectedId = d.value;
                var selectedItem = data.find(item => item.value === SelectedId);
                var selectedText = SelectedId;
                var partycode = selectedItem ? selectedItem.code : "";
                var partyId = selectedItem ? selectedItem.key : "";
                var filter = empr_POSTransaction.Partydata.find(e => e.key === partyId && e.code === partycode);
                $('#partyhidden').val(partyId);
                if (filter != null) {
                    $('#partynamehidden').val(filter.value);
                    $('#partycellhidden').val(filter.cell);
                }
                $('#partycode').val(partycode);
                ajaxHelper.ajaxGetJson('/POSTransactions/GetDueDate?partycode=' + partycode + '&actcode=' + d.value, function (data) {

                    if (data && data.length > 0) {
                        var dateString = data[0].value; // Assuming dateString is in the format "28-Feb-2025"
                        var dateParts = dateString.split('-');
                        var day = dateParts[0];
                        var month = dateParts[1];
                        var year = dateParts[2];

                        // Convert month name to month number
                        var monthNumber = new Date(Date.parse(month + " 1, 2000")).getMonth() + 1;
                        monthNumber = monthNumber < 10 ? '0' + monthNumber : monthNumber; // Ensure two digits

                        // Construct the date string in YYYY-MM-DD format
                        var formattedDate = `${year}-${monthNumber}-${day}`;
                        $('#dueDate').val(formattedDate);
                    }
                    else {
                        var today = new Date();
                        var day = String(today.getDate()).padStart(2, '0');
                        var month = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
                        var year = today.getFullYear();

                        // Format the date as YYYY-MM-DD
                        var formattedDate = year + '-' + month + '-' + day;

                        // Set the value of the input field with id 'dueDate'
                        $('#dueDate').val(formattedDate);
                    }
                }, false, true);
            });
            empr_POSTransaction.bindDxDdl_New("partyName2", data, null, "key", "value", "Select", function (d) {
                var SelectedId = d.value;
                var selectedItem = data.find(item => item.value === SelectedId);
                var partycode = selectedItem ? selectedItem.code : "";
                var partyId = selectedItem ? selectedItem.key : "";
                $('#partyhidden2').val(partyId);
                $('partynamehidden2').val(SelectedId);
                $('#partycode2').val(partycode);
            });
        });

        ajaxHelper.ajaxGetJson('/POSTransactions/GetAcountName', function (data) {
            empr_POSTransaction.bindDxDdl("AcountName", data, null, "key", "value", "Select", function (d) {
                $('#Acounthidden').val(d.value);
            });
            empr_POSTransaction.bindDxDdl("AcountName2", data, null, "key", "value", "Select", function (d) {
                $('#Acounthidden2').val(d.value);
            });
        });

        ajaxHelper.ajaxGetJson("/POSTransactions/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    var selectedValue = data.data[0].mD_ID;
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
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);

        empr_POSTransaction.ExpenseDropdown("");
        empr_POSTransaction.SalesmanDropdown("");
        setTimeout(function () {
            $('#btnCash').click();
        }, 500);
    },

    DynamicIcon: function () {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetDynamicIcon', function (data) {
            if (data.length > 0) {

                var itemsHtml = $.map(data, function (item) {
                    return `
                                     <div id="${item.grouP_CODE}" class="icondiv" style="cursor: pointer; width:100% ;">
                                     <img src="${item.gpic}" style="height:50px ; width:100% ;"/>
                                     <br> ${item.grouP_NAME}</div>
                            `;
                });
                $('.headers').append(itemsHtml);

            } else {
                $('.headers').html('<p>No items available for this group.</p>');
            }

            $('.icondiv').each(function () {
                const groupName = $(this).text().trim().toLowerCase();
                const groupCode = $(this).attr('id');

                if (groupName.includes('hold') || groupCode === '1') {
                    $(this).attr('title', 'key F7');
                }
            });

        });

        $('#3.icondiv').trigger('click');

        empr_POSTransaction.billMode = '';
    },

    POSDesignScript: function () {
         
        function openEditModal(itemData, dtcode) {
            let calculatedValue;
            if (itemData.discountAmount == "") {
                calculatedValue = Math.round((itemData.amount * itemData.discountpersentage) / 100);
            }
            else {
                calculatedValue = itemData.discountAmount;
            }
            document.getElementById('modalTitle').innerText = itemData.name;
            document.getElementById('modalId').value = itemData.Id;
            document.getElementById('BarcodeId').value = itemData.BarcodeId;
            document.getElementById('itemQuantity').value = itemData.quantity;
            document.getElementById('discountPercentage').value = itemData.discountpersentage;
            document.getElementById('discountPerUnit').value = calculatedValue;
            document.getElementById('itemUnit').value = itemData.unit;
            document.getElementById('amount').value = itemData.amount;
            //document.getElementById('tempAmount').value = itemData.tempAmount;
            $('#remarks.form-control').val(itemData.remarks);
            document.getElementById('editrate').value = itemData.rate;


            if ($('#BILL_STATUS').val() != 'P' || dtcode == "") {
                if (Permissions != 'Admin') {
                    if (Permissions.r_EDIT) {
                        document.getElementById('itemQuantity').readOnly = false;
                        document.getElementById('discountPercentage').readOnly = false;
                        document.getElementById('discountPerUnit').readOnly = false;
                        document.getElementById('itemUnit').readOnly = false;
                        document.getElementById('amount').readOnly = false;
                        document.getElementById('editrate').readOnly = false;
                        $('#remarks.form-control').prop('readonly', false);
                    }
                    else {
                        document.getElementById('itemQuantity').readOnly = true;
                        document.getElementById('discountPercentage').readOnly = true;
                        document.getElementById('discountPerUnit').readOnly = true;
                        document.getElementById('itemUnit').readOnly = true;
                        document.getElementById('amount').readOnly = true;
                        document.getElementById('billDisc').readOnly = true;
                        document.getElementById('editrate').readOnly = true;
                        $('#remarks.form-control').prop('readonly', true);
                    }
                }
                else {
                    document.getElementById('itemQuantity').readOnly = false;
                    document.getElementById('discountPercentage').readOnly = false;
                    document.getElementById('discountPerUnit').readOnly = false;
                    document.getElementById('itemUnit').readOnly = false;
                    document.getElementById('amount').readOnly = false;
                    document.getElementById('editrate').readOnly = false;
                    $('#remarks.form-control').prop('readonly', false);
                }
            }
            else {
                document.getElementById('itemQuantity').readOnly = true;
                document.getElementById('discountPercentage').readOnly = true;
                document.getElementById('discountPerUnit').readOnly = true;
                document.getElementById('itemUnit').readOnly = true;
                document.getElementById('amount').readOnly = true;
                document.getElementById('editrate').readOnly = true;
                $('#remarks.form-control').prop('readonly', true);
            }

            // Show the modal
            $('#editItemModal').modal('show');
        }
        $("#CMOB").mask("9999-9999999");

        document.addEventListener('click', function (e) {
            //const quantityInput = document.querySelector('.quantity-input');
            //const targetIcon = document.getElementById('tablechk');

            //if (quantityInput && parseFloat(quantityInput.value) < 0) {

            //    targetIcon.click();

            //    console.log("Negative value detect hui, icon click kar diya gaya.");
            //}
            if (e.target.closest('.delete-icon')) {
                //  //  
                let row = e.target.closest('tr');
                if ($('#BILL_STATUS').val() != 'P' || row.querySelector('#dtcode').value == "0") {

                    let selectedItemId = row.querySelector('.BarcodeId').value == '0' ? row.querySelector('.SelectedItemId').value : row.querySelector('.BarcodeId').value;
                    let selectedItemRate = row.querySelector('.count').value;
                    let tableRows = document.querySelectorAll('#orderTable tr');
                    if (row.querySelector('#dtcode').value == "0" || row.querySelector('#dtcode').value == "") {
                        tableRows.forEach(function (currentRow) {
                             
                            //let currentSelectedItem = currentRow.querySelector('.BarcodeId').value == 0
                            //    ? currentRow.querySelector('.SelectedItemId').value
                            //    : currentRow.querySelector('.BarcodeId').value;
                            let currentSelectedItem = (currentRow.querySelector('.BarcodeId')?.value == 0)
                                ? currentRow.querySelector('.SelectedItemId')?.value
                                : currentRow.querySelector('.BarcodeId')?.value;
                            let currentSelectedRate = currentRow.querySelector('.count')?.value

                            if (currentSelectedItem == selectedItemId && selectedItemRate == currentSelectedRate) {
                                currentRow.remove();
                            }
                        });
                        empr_POSTransaction.updateSummary();
                    }
                    else {
                        let itemData = { Id: row.querySelector('.SelectedItemId')?.value };
                        var ItemId = itemData.Id;

                        DeleteRow(ItemId, $('#TRANS_ID').val());

                        function DeleteRow(ItemId, TransId) {
                            ajaxHelper.ajaxGetJson('/POSTransactions/GetDeleterow', function (data) {
                                if (data.msgType == 1) {
                                    var Exist = data.data.filter(b => b.tranId == TransId && b.itemcode == ItemId);

                                    if (Exist.length == 0) {
                                        let tableRows = document.querySelectorAll('#orderTable tr');
                                        tableRows.forEach(function (currentRow) {
                                            let currentSelectedRate = currentRow.querySelector('.count')?.value
                                            let currentSelectedItem = currentRow.querySelector('.SelectedItemId')?.value;

                                            // Check if selectedItemId matches the current row's selectedItem
                                            if (currentSelectedItem == row.querySelector('.SelectedItemId')?.value && currentSelectedRate == selectedItemRate) {
                                                currentRow.remove(); // Remove the matching row
                                            }
                                        });
                                        empr_POSTransaction.updateSummary();
                                    }
                                    else {
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
                                            if (Permissions != 'Admin') {
                                                //  //  
                                                if (Permissions.r_DLT) {
                                                    var dataModel = empr_POSTransaction.GetDataToSave();
                                                    let tableRows = document.querySelectorAll('#orderTable tr');
                                                    tableRows.forEach(function (currentRow) {
                                                        let currentSelectedRate = currentRow.querySelector('.count').value
                                                        let currentSelectedItem = currentRow.querySelector('.SelectedItemId').value;

                                                        // Check if selectedItemId matches the current row's selectedItem
                                                        if (currentSelectedItem == row.querySelector('.SelectedItemId').value && currentSelectedRate == selectedItemRate) {
                                                            currentRow.remove(); // Remove the matching row
                                                        }
                                                    });
                                                    empr_POSTransaction.updateSummary();
                                                    ajaxHelper.ajaxPostJsonData({ modelRecord: dataModel, code: TransId, ItemId: ItemId, dtcode: row.querySelector('#dtcode').value }, "/POSTransactions/DeletePOSTransactionDetailByCode", function (data) {
                                                        empr_helper.notify(data.msg, data.msgType);
                                                        if (data.msgType == 1) {

                                                        }
                                                    }, false, true);
                                                }
                                                else {
                                                    empr_helper.notify("You are not allowed to remove this row. please contact your admin.", 2);
                                                }
                                            }
                                            else {
                                                var dataModel = empr_POSTransaction.GetDataToSave();
                                                let tableRows = document.querySelectorAll('#orderTable tr');
                                                tableRows.forEach(function (currentRow) {
                                                    let currentSelectedRate = currentRow.querySelector('.count').value
                                                    let currentSelectedItem = currentRow.querySelector('.SelectedItemId').value;

                                                    if (currentSelectedItem == row.querySelector('.SelectedItemId').value && currentSelectedRate == selectedItemRate) {
                                                        currentRow.remove(); // Remove the matching row
                                                    }
                                                });
                                                empr_POSTransaction.updateSummary();
                                                ajaxHelper.ajaxPostJsonData({ modelRecord: dataModel, code: TransId, ItemId: ItemId, dtcode: row.querySelector('#dtcode').value }, "/POSTransactions/DeletePOSTransactionDetailByCode", function (data) {
                                                    empr_helper.notify(data.msg, data.msgType);
                                                    if (data.msgType == 1) {


                                                    }
                                                }, false, true);
                                            }
                                        });
                                    }

                                }


                            }, false, true);

                        }

                    }
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }

            }

            if (e.target.closest('.edit-icon')) {
                let row = e.target.closest('tr');
                if (empr_POSTransaction.return == true && row.querySelector('#dtcode').value != "") {
                    $('#discountPercentage').prop('disabled', true);
                    $('#discountPerUnit').prop('disabled', true);
                    $('#itemQuantity').prop('disabled', true);
                }


                let quantity = parseFloat(row.querySelector('.quantity-input').value) || 0;
                let discountAmount = parseFloat(row.querySelector('.discountvalue').value) || 0;
                let rate = parseFloat(row.querySelector('.total-price').value) || 0;

                let amount = Math.round(Math.abs(quantity) * Math.abs(rate - discountAmount));

                // Adjust the amount based on the signs of quantity and rate
                if ((quantity < 0 && rate < 0)) {
                    amount = -amount;
                }

                let itemData = {
                    name: row.cells[0].innerText,
                    BarcodeId: row.cells[8].innerText.split(' - ')[0],
                    Id: row.cells[7].innerText.split(' - ')[0],
                    quantity: row.querySelector('.quantity-input').value,
                    discountpersentage: row.querySelector('.discountpercent').value,
                    discountAmount: row.querySelector('.discountvalue').value,
                    unit: row.querySelector('.unit').value,
                    rate: row.querySelector('.total-price').value,
                    amount: amount || 0,
                    remarks: row.querySelector('.remarks').value,
                };
                openEditModal(itemData, row.querySelector('#dtcode').value);
            }

        });
        function updateTotal() {
            const discountper = parseFloat($('#discountPercentage').val()) || 0;
            const amount = parseFloat($('#amount').val()) || 0;
            const discvalue = ((discountper / 100) * amount).toFixed(2);
            $('#discountPerUnit').val(discvalue);
            if (discvalue < 0) {
                $('#discountPercentage').val(-Math.abs(discountper));
            }
        }

        $('#discountPercentage').on('change', updateTotal);



        document.getElementById('updateItem').addEventListener('click', function () {

            let updatedQuantity = document.getElementById('itemQuantity').value;
            let updatedRate = document.getElementById('editrate').value;
            let updatedAmount = document.getElementById('amount').value;
            let updatedRemarks = $('#remarks.form-control').val();
            let updateddiscountper = document.getElementById('discountPercentage').value;
            let updateddiscountunit = document.getElementById('discountPerUnit').value;

            let itemName = document.getElementById('modalTitle').innerText;
            let itemId = document.getElementById('modalId').value;
            let BarcodeId = document.getElementById('BarcodeId').value;
            let tableRows = document.querySelectorAll('#orderTable tr');
            let targetRow;
            if (BarcodeId == 0 || BarcodeId == null) {
                targetRow = Array.from(tableRows).find(row => row.cells[7].innerText.includes(itemId));
            }
            else {
                targetRow = Array.from(tableRows).find(row => row.cells[8].innerText.includes(BarcodeId) && row.cells[0].innerText == itemName);
            }


            if (targetRow) {
                let price = parseFloat(targetRow.querySelector('.quantity-input').dataset.price);
                let stock = parseFloat(targetRow.querySelector('.stock').dataset.stockqty);
                let barcodeId = targetRow.querySelector('.BarcodeId').value;
                let stockStatus = targetRow.querySelector('#stockstatus').value;
                targetRow.querySelector('.remarks').value = updatedRemarks;
                targetRow.querySelector('.quantity-input').value = updatedQuantity;
                targetRow.querySelector('.discountpercent').value = updateddiscountper;
                targetRow.querySelector('.discountvalue').value = updateddiscountunit;
                targetRow.querySelector('.discountcount').value = 1;
                const icon = targetRow.querySelector('.row-checkbox');

                if (updatedQuantity < 0) {
                    if (icon) {
                        icon.classList.add('clicked');
                    }
                    if (updatedQuantity.includes('-')) {
                        targetRow.querySelector('.total-price').value = -Math.abs(updatedRate);
                    }
                } else {
                    if (icon) {
                        icon.classList.remove('clicked');
                    }
                    if (updatedQuantity > 0) {
                        targetRow.querySelector('.total-price').value = Math.abs(updatedRate);
                    }
                }
                if (stockStatus == 'Y') {
                    if (updatedQuantity <= stock) {
                        document.querySelectorAll(`#orderTable tr`).forEach(tableRows => {
                            const rowBarcode = tableRows.querySelector('.BarcodeId').value;
                            if (rowBarcode === barcodeId) {
                                tableRows.querySelector('.stock').textContent = '';
                            }
                        });
                    }
                    else {
                        if (!isNaN(stock)) {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRows => {
                                const rowBarcode = tableRows.querySelector('.BarcodeId').value;
                                if (rowBarcode === barcodeId) {
                                    tableRows.querySelector('.stock').textContent = `HAS ONLY ${stock} IN STOCK`;
                                }
                            });
                        }
                    }
                }
            }
            $('#editItemModal').modal('hide');
            $('#remarks.form-control').val('');
            empr_POSTransaction.updateSummary(null);

        });
        function updateTotalPayment() {
            const subTotal = 840;
            let discountValue = parseFloat(billDiscValueInput.value) || 0;
            let discountPercent = parseFloat(billDiscPercentInput.value) || 0;

            if (discountPercent > 0) {
                discountValue = (subTotal * discountPercent) / 100;
            }

            const finalValue = subTotal - discountValue;
            totalPaymentElement.textContent = `Value ${finalValue.toFixed(2)}/-`;
        }
        //const listViewButton = document.querySelector('.toggle-view-list');
        //const gridViewButton = document.querySelector('.toggle-view-grid');
        //const refreshButton = document.querySelector('.toggle-referesh');
        //const middleSection = document.querySelector('.middle');

        //listViewButton.addEventListener('click', () => {
        //    middleSection.classList.add('list-view');
        //    middleSection.classList.remove('grid-view');
        //});

        //gridViewButton.addEventListener('click', () => {
        //    middleSection.classList.add('grid-view');
        //    middleSection.classList.remove('list-view');
        //});
        //refreshButton.addEventListener('click', () => {

        //    empr_POSTransaction.refreshData(newGroupId);
        //});
        $('#btnCash').click();
    },

    Checkvalidation: function (totalAmount, totalEntered) {
        //  
        let advAmount = parseFloat($("#advAmount").val()) || 0;
        var IsValidate = true
        var payType = empr_POSTransaction.payType;
        //  
        if (($('#partyrecv2').val() != 0 || $('#bankrecv2').val() != 0) && totalAmount >= totalEntered) {
            if ($('#partyrecv2').val() != 0) {
                if ($('#partyhidden2').val() == '') {
                    $('#partyName2').css('border', '2px solid red');
                    IsValidate = false;
                    return IsValidate
                }
                else {
                    $('#partyName2').css('border', '');
                }
            }
            if ($('#bankrecv2').val() != 0) {
                var bankDDL = $('#BACCOUNTS2').dxSelectBox('option', 'value');
                if (bankDDL == '' || bankDDL == null) {
                    $('#BACCOUNTS2').css('border', '2px solid red');
                    IsValidate = false;
                    return IsValidate
                }
                else {
                    $('#BACCOUNTS2').css('border', '');
                }
            }

        }
        if (payType == "Party") {
            if ($('#partyrecv').val() != '') {
                if ($('#partyhidden').val() == '') {
                    $('#partyName').css('border', '2px solid red');
                    IsValidate = false;
                    return IsValidate
                }
                else
                    $('#partyName').css('border', '');
            }
        }

        if (payType == "Split") {
            var total = parseFloat($('#totalPayment').text()) || 0;
            var cash = parseFloat($('#cashAmount2').val()) || 0;
            var bAmt = parseFloat($('#bankrecv2').val()) || 0;
            var pAmt = parseFloat($('#partyrecv2').val()) || 0;
            var bankDDL = $('#BACCOUNTS2').dxSelectBox('option', 'value');
            var partyDDL = $('#partyName2').dxSelectBox('option', 'value');

            //  
            var splitTotal = cash + bAmt + pAmt;
            if (cash = '' || cash == 0 || cash == null) {
                empr_helper.notify("Payment is incomplete", 2);
                $('#cashAmount2').css('border', '2px solid red');
                $('#bankrecv2').css('border', '2px solid red');
                $('#partyrecv2').css('border', '2px solid red');

                IsValidate = false;
                return IsValidate
            }

            if (bankDDL && (isNaN(bAmt) || bAmt <= 0)) {
                $('#BACCOUNTS2').css('border', '2px solid red');
                IsValidate = false;
                return IsValidate;
            }
            if (partyDDL && (isNaN(pAmt) || pAmt <= 0)) {
                $('#partyName2').css('border', '2px solid red');
                IsValidate = false;
                return IsValidate;
            }
            if (splitTotal > total) {
                empr_helper.notify("You cannot enter a value greater than the total payment.", 2);
                IsValidate = false;
                return IsValidate

            }
            if (splitTotal < total) {
                empr_helper.notify("Payment is incomplete", 2);
                IsValidate = false;
                return IsValidate
            }


        }

        if ($('#bankrecv').val() != '') {
            var bankDDL = $('#BACCOUNTS').dxSelectBox('option', 'value');
            if (bankDDL == '' || bankDDL == null) {
                $('#BACCOUNTS').css('border', '2px solid red');
                IsValidate = false;
                return IsValidate
            }
            else {
                $('#BACCOUNTS').css('border', '');
            }
        }
        if (payType == "Party") {
            if ($('#partyrecv').val() != '') {
                var bankDDL = $('#partyName').dxSelectBox('option', 'value');
                if (bankDDL == '' || bankDDL == null) {
                    $('#partyName').css('border', '2px solid red');
                    IsValidate = false;
                    return IsValidate
                }
                else {
                    $('#partyName').css('border', '');
                }
            }
        }
        if ($('#advBankAmount').val() != '' && $('#advBankAmount').val() != 0) {
            var bankDDL = $('#BACCOUNT').dxSelectBox('option', 'value');
            if (bankDDL == '' || bankDDL == null) {
                $('#BACCOUNT').css('border', '2px solid red');
                IsValidate = false;
                return IsValidate
            }
            else {
                $('#BACCOUNT').css('border', '');
            }
        }
        if (($('#fnlBankAmount').val() != '' && $('#fnlBankAmount').val() != 0) && $('#complete').val() == 1) {
            var bankDDL = $('#AdvAccount').dxSelectBox('option', 'value');
            if (bankDDL == '' || bankDDL == null) {
                $('#AdvAccount').css('border', '2px solid red');
                IsValidate = false;
                return IsValidate
            }
            else {
                $('#AdvAccount').css('border', '');
            }
        }
        if ($('#advAmount').val() == '' || $('#advAmount').val() == 0 || ($('#advAmount').val() != '' && totalAmount < advAmount)) {
            if (totalAmount < advAmount && empr_POSTransaction.AccountTax == "Advance") {
                $('#advAmount').css('border', '2px solid red');
                empr_helper.notify("You cannot enter a value greater than the total payment.", 2);
                IsValidate = false;
                return IsValidate
            }
            else {
                $('#advAmount').css('border', '');
            }
        }
        else {
            IsValidate = true
        }
        return IsValidate
    },

    getColorCode: function (colorName) {
        const colors = {
            'yellow': '#ffff00',
            'blue': '#0000ff',
            'green': '#008000',
            'red': '#ff0000',
        };
        return colors[colorName.toLowerCase()] || '#cccccc';
    },

    ItemsGroup: function () {
        //  //  
        if (Permissions != "Admin") {
            !Permissions.r_VIEW && $('#PayQuickSearch').hide();
            !Permissions.r_VIEW && $('#AdvanceBtn').hide();
            !Permissions.r_ADD && $('#BtnQuickSearch').hide();
            //!Permissions.r_ADD && $('#validateAndOpenModal').hide();
        };

        if (BarcodeTextBoxVisible) {
            $('#barcode').show();
            $('.qrCodevisible').hide();
        }
        else {
            $('#barcode').hide();
            $('.qrCodevisible').show();
        }

        if (ItemsGroup && ItemsGroup.length > 0) {
            setTimeout(function () {

                var GroupImage = $('#GroupImg').val();
                ItemsGroup.forEach(function (item, index) {
                    //Generate the card HTML dynamically
                    var cardHtml = `
                   
                        <div class="box selectdiv boxImageDesign" data-item="${item.grouP_CODE}">
                             <img src="${item.ipic}" alt="" onerror="this.onerror=null;this.src='${GroupImage}';">
                            <p>
                                ${item.grouP_NAME}
                            </p>
                        </div>
                        </div>
                    `;

                    // Append the generated card to the #cards-container  ye change ki hai 
                    $('.Top-left-body .grid').append(cardHtml);

                    if (index === 0) {
                        newGroupId = item.grouP_CODE;
                        if (!$("#SHOW_SELECTED").prop("checked")) {
                            empr_POSTransaction.refreshData(0);
                        } else {
                            empr_POSTransaction.loadItemsForGroup(item.grouP_CODE);
                        }

                    }
                });

                $('.Top-left-body').on('click', '.selectdiv', function () {

                    $("#SHOW_SELECTED").prop("checked", true);
                    var groupId = $(this).data('item');
                    newGroupId = groupId;
                    empr_POSTransaction.loadItemsForGroup(groupId);
                });
            }, 200);
        } else {
            $('.Top-left-body').html('<p>No items to display.</p>');
        }
    },

    sendOrder: function (orderData) {
        //  //  
        empr_POSTransaction.connection.invoke("SendOrder", JSON.stringify(orderData));
    },

    PaymentSetting: function (DuplicateBill) {
        //  //  
        let allPositive = $('.stock').toArray().every(el => $(el).data('stockqty') > 0);
        if (allPositive > 0 || !BarcodeTextBoxVisible) {
            if ($('#BILL_STATUS').val() != "P" || empr_POSTransaction.return == true) {
                $("#partyName").dxSelectBox("instance").option("disabled", false);
                $('#partyName2').dxSelectBox('option', 'readOnly', false);
                $('#cashAmount2').prop('readonly', false);

                $('#bankrecv2').prop('readonly', false);
                $('#BACCOUNT').dxSelectBox('option', 'readOnly', false);
                $('#PARTY_BACCOUNTS').dxSelectBox('option', 'readOnly', false);
                $('#BACCOUNTS2').dxSelectBox('option', 'readOnly', false);
                $('#partyrecv2').prop('readonly', false);
                $('#partyrecv').prop('readonly', false);
                $('#cashAmount').prop('readonly', false);
                $('#btnAdvance').prop('disabled', false);
                $('#btnCash').prop('disabled', false);
                $('#btnCard').prop('disabled', false);
                $('#btnParty').prop('disabled', false);
                $('#btnSplit').prop('disabled', false);
                $('#cashOption').prop('disabled', false);
                $('#cardOption').prop('disabled', false);
                $('#creditOption').prop('disabled', false);
                $("#Plus, #minus").prop("disabled", false);
                $("#settlementInput").prop("readonly", false);
                $('#btnSplit').css('display', '');
                empr_POSTransaction.ResetAllFields();
                $('#completecheck').prop({
                    'checked': false,
                    'readonly': false,
                    'disabled': false
                }).closest('.form-check').addClass('disabled');
                if (empr_POSTransaction.return == true) {
                    $('#advAmount').val('');
                    $('#fnlAmount').val($('#totalPayment').text());
                }
                document.getElementById('btnCash').click();
                $('.partyradioBtn input[type="radio"]').prop('disabled', false);
            }
            var clickable = 0;
            var cash = parseInt($('#cashAmount').val()) || 0;
            var recv = parseInt($('#recvhidden').val()) || 0;
            var advance = parseInt($('#advAmount').val()) || 0;
            var advbank = parseInt($('#advAmountbank').val()) || 0;
            var cash2 = parseInt($('#cashAmount2').val()) || 0;
            var bank2 = parseInt($('#bankrecv2').val()) || 0;
            var party2 = parseInt($('#partyrecv2').val()) || 0;
            var bank = parseInt($('#bankrecv').val()) || 0;
            var party = parseInt($('#partyrecv').val()) || 0;
            var partyaccountcode2 = parseInt($('#partyhidden2').val()) || 0;
            var partyaccountcode = parseInt($('#partycode').val()) || 0;
            var partyId = parseInt($('#partyhidden').val()) || 0;
            var totalPayment = parseInt($('#totalPayment').text()) || 0;
            ajaxHelper.ajaxGetJson('/POSTransactions/GetMapData', function (data) {
                if (data.msgType == 1) {

                    /*empr_POSTransaction.ResetAllFields();*/
                    if ($('#cashAmount').val() == "" || $('#cashAmount').val() == null) {
                        $('#cashAmount').val($('#totalPayment').text());
                    }
                    const orderTable = document.getElementById("orderTable");
                    if (data.data.length > 0) {
                        if (($('#BACCOUNT').val() == '' || $('#BACCOUNT').val() == null) || ($('#BACCOUNTS').val() == '' || $('#BACCOUNTS').val() == null) || ($('#PARTY_BACCOUNTS').val() == '' || $('#PARTY_BACCOUNTS').val() == null) || ($('#BACCOUNTS2').val() == '' || $('#BACCOUNTS2').val() == null)) {
                            if ($('#BILL_STATUS').val() != 'P') {
                                $('#BACCOUNT').dxSelectBox('instance').option("value", data.data[0].banK_ACT);
                                $('#BACCOUNTS').dxSelectBox('instance').option("value", data.data[0].banK_ACT);
                                $('#PARTY_BACCOUNTS').dxSelectBox('instance').option("value", data.data[0].banK_ACT);
                                $('#BACCOUNTS2').dxSelectBox('instance').option("value", data.data[0].banK_ACT);
                                empr_POSTransaction.IntiAdvanceBankAct('');
                                $('#AdvAccount').dxSelectBox('option', 'readOnly', true);
                            }
                            else {
                                if (empr_POSTransaction.payType == 'Advance') {
                                    if (advbank == 0)
                                        empr_POSTransaction.InitBankAccount('');
                                    if ($('#complete').val() == 1 && advbank == 0) {
                                        empr_POSTransaction.IntiAdvanceBankAct('');
                                        $('#AdvAccount').dxSelectBox('option', 'readOnly', true);
                                    }
                                    else {
                                        if ($('#complete').val() != 1) {
                                            empr_POSTransaction.IntiAdvanceBankAct(data.data[0].banK_ACT);
                                            $('#AdvAccount').dxSelectBox('option', 'readOnly', false);
                                        }
                                    }
                                }
                            }
                        }
                        $('#bankcharges').val(data.data[0].bcharges);
                        $('#Acounthidden').val(data.data[0].casH_ACT);
                        $('#bankhidden').val(data.data[0].banK_ACT);
                        $('#bankhidden2').val(data.data[0].banK_ACT);
                        //var pType = empr_POSTransaction.payType;


                        //} else if (pType == "Split") {

                        //}
                        $('#cashtax').val(data.data[0].casH_TAX);
                        $('#advcashtax').val(data.data[0].casH_TAX);
                        $('#advbanktax').val(data.data[0].casH_TAX);
                        $('#partytax').val(data.data[0].partY_TAX);
                        $('#banktax').val(data.data[0].banK_TAX);
                        $('#cashtax2').val(data.data[0].casH_TAX);
                        $('#partytax2').val(data.data[0].partY_TAX);
                        $('#banktax2').val(data.data[0].banK_TAX);
                        $('#srbName').val(data.data[0].srbname);
                        $('#srbNtn').val(data.data[0].srbntn);
                        $('#posUser').val(data.data[0].posuser);
                        $('#posPass').val(data.data[0].pospass);
                        $('#srbId').val(data.data[0].srbid);
                        $('#srbStatus').val(data.data[0].srbstatus);
                        $('#srbUrl').val(data.data[0].srburl);
                        //$('#ratelock').val(data.data[0].rate);
                    }
                    if (DuplicateBill == undefined) {
                        if (orderTable.rows.length > 0) {

                            const modal = new bootstrap.Modal($("#paymentModal"));
                            modal.show();

                            document.getElementById('btnCash').click();

                        } else {
                            var msg = "Items table is empty. Please add at least one item before proceeding.";
                            empr_helper.notify(msg, 2);
                        }
                    }
                    /*if ((cash2 != 0 && bank2 != 0) || (cash2 != 0 && party2 != 0) || (bank2 != 0 && party2 != 0)) {*/
                    if (empr_POSTransaction.payType == 'Split') {
                        document.getElementById('btnSplit').click();
                        let dropdownInstance = $("#partyName2").dxSelectBox("instance");
                        dropdownInstance.option("value", partyaccountcode2);
                        let cashtaxAmt = Math.round(cash2 * data.data[0].casH_TAX / 100);
                        $('#CashTax_Amt2').val(cashtaxAmt);
                        let cashAmtwithtax = cash2 + cashtaxAmt;
                        $('#cashtaxAmt2').val(cashAmtwithtax);
                        let banktaxAmt = Math.round(bank2 * data.data[0].banK_TAX / 100);
                        $('#BankTax_Amt2').val(banktaxAmt);
                        let bankAmtwithtax = bank2 + banktaxAmt;
                        $('#banktaxAmt2').val(bankAmtwithtax);
                        let partytaxAmt = Math.round(party2 * data.data[0].partY_TAX / 100);
                        $('#PartyTax_Amt2').val(partytaxAmt);
                        let partyAmtWithtax = party2 + partytaxAmt;
                        $('#partytaxAmt2').val(partyAmtWithtax);
                        var partywithtax = $('#partytaxAmt2').val();
                        var Total = cashAmtwithtax + bankAmtwithtax + parseInt(partywithtax);
                        $('#TotalAmtWithTax').val(Total);
                        $('#recvhidden').val(Total);
                    }
                    /*else if (party != 0)*/
                    else if (empr_POSTransaction.payType == 'Party') {
                        //  //  
                        document.getElementById('btnParty').click();
                        var filter = empr_POSTransaction.Partydata.find(e => e.key === partyId && e.code === partyaccountcode);
                        if (filter != null) {
                            let dropdownInstance = $("#partyName").dxSelectBox("instance");
                            dropdownInstance.option("value", filter.value);
                        }
                    }
                    else if (empr_POSTransaction.payType === 'Bank') {
                        $('#btnCard').trigger('click');
                    } else {
                        $('#btnCash').trigger('click');
                    }
                    //  //  
                    var SelectedBtn = $.trim($('.paymentbtn.selected').first().text());
                    if (!SelectedBtn) {
                        // Fallback for backend-driven activation (e.g., legacy Advance, or when UI changes)
                        const pt = (empr_POSTransaction.payType || "").toString().trim();
                        if (pt === "Bank") SelectedBtn = "Card";
                        else if (pt) SelectedBtn = pt;
                        else SelectedBtn = "Cash";
                    }
                    empr_POSTransaction.AccountTax = SelectedBtn;
                    //  //  
                    if (SelectedBtn == "Advance") {
                        MultipleTaxValue = $('#cashtax').val();
                        TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * MultipleTaxValue) / 100);
                        $('#CashTax_Amt2').val(TaxAmt);
                        TotalAmtWithTax = Math.round(parseFloat($('#TotalPaymenthidden').val()) + TaxAmt);
                        $('#totalPayment').text(TotalAmtWithTax);
                        $('#cashAmount').val(TotalAmtWithTax);
                        var total = parseFloat($('#totalPayment').text()) || 0;
                        advance = parseFloat($('#advAmount').val()) || 0;
                        var final = total - advance;
                        $('#recvhidden').val(final);
                    }
                    else if (SelectedBtn == "Cash") {
                        MultipleTaxValue = $('#cashtax').val();
                        TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * MultipleTaxValue) / 100);
                        $('#CashTax_Amt2').val(TaxAmt);
                        //TotalAmtWithTax = Math.round(parseFloat($('#TotalPaymenthidden').val()) + TaxAmt);
                        //var sett = parseInt($('#settlementInput').val()) || 0;
                        //$('#totalPayment').text(TotalAmtWithTax - sett);
                        //var cash = parseInt($('#cashAmount').val()) || 0;
                        //var recv = parseInt($('#recvhidden').val()) || 0;
                        //if (cash > recv) {
                        //    $('#cashAmount').val();
                        //}
                        //else {
                        //    $('#cashAmount').val(TotalAmtWithTax - sett);
                        //    $('#recvhidden').val(TotalAmtWithTax - sett);
                        //}
                        //$('#recvhidden').val(TotalAmtWithTax - sett);

                    }
                    else if (SelectedBtn == "Card") {
                        MultipleTaxValue = $('#banktax').val();
                        TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * MultipleTaxValue) / 100);
                        $('#BankTax_Amt2').val(TaxAmt);
                        //TotalAmtWithTax = Math.round(parseFloat($('#TotalPaymenthidden').val()) + TaxAmt);
                        //$('#totalPayment').text(TotalAmtWithTax);
                        //$('#bankrecv').val(TotalAmtWithTax);
                        //$('#recvhidden').val(TotalAmtWithTax);
                    }
                    else if (SelectedBtn == "Party") {
                        MultipleTaxValue = $('#partytax').val();
                        TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * MultipleTaxValue) / 100);
                        $('#PartyTax_Amt2').val(TaxAmt);
                        //TotalAmtWithTax = Math.round(parseFloat($('#TotalPaymenthidden').val()) + TaxAmt);
                        //$('#totalPayment').text(TotalAmtWithTax);
                        //$('#partyrecv').val(TotalAmtWithTax);
                        //$('#recvhidden').val(TotalAmtWithTax);
                    }

                    if (data.data[0].radvance == 'N' || empr_POSTransaction.return == true) {
                        $('#btnAdvance').css('display', 'none');
                        //$('#btnAdvance').prop('disabled', true).addClass('disabled');
                    }
                    else {
                        if (empr_POSTransaction.payType == 'Advannce')
                            $('#btnAdvance').prop('disabled', false).removeClass('disabled');
                        $('#btnAdvance').css('display', '');
                    }
                    if (data.data[0].rcash == 'N') {
                        $('#btnCash').css('display', 'none');
                        $('.SplitCash').css('display', 'none');
                    }
                    else {
                        $('#btnCash').css('display', '');
                        $('.SplitCash').css('display', '');
                        if (clickable == 0) {
                            document.getElementById('btnCash').click();
                            clickable = 1
                        }
                    }
                    if (data.data[0].rcard == 'N') {
                        $('#btnCard').css('display', 'none');
                        $('.SplitBank').css('display', 'none');
                    }
                    else {
                        $('#btnCard').css('display', '');
                        $('.SplitBank').css('display', '');
                        if (clickable == 0) {
                            document.getElementById('btnCard').click();
                            clickable = 1
                        };
                    }
                    if (data.data[0].rparty == 'N') {
                        $('#btnParty').css('display', 'none');
                        $('.SplitParty').css('display', 'none');
                    }
                    else {
                        $('#btnParty').css('display', '');
                        $('.SplitParty').css('display', '');
                        if (clickable == 0) {
                            document.getElementById('btnParty').click();
                            var filter = empr_POSTransaction.Partydata.find(e => e.key === partyId && e.code === partyaccountcode);
                            if (filter != null) {
                                let dropdownInstance = $("#partyName").dxSelectBox("instance");
                                dropdownInstance.option("value", filter.value);
                            }
                            clickable = 1
                        };
                    }
                    if (data.data[0].rsplit == 'N' || $('#TotalPaymenthidden').val().includes('-') || empr_POSTransaction.return == true)
                        $('#btnSplit').css('display', 'none');
                    else
                        $('#btnSplit').css('display', '');
                    if ($('#BILL_STATUS').val() == 'P') {
                        $('.quantity-input').prop('readonly', true);
                        $('.total-price').prop('readonly', true);
                        $('.discountpercent').prop('readonly', true);
                        $("#partyName").dxSelectBox("instance").option("readOnly", true);
                        /*$("#partyName2").dxSelectBox("instance").option("disabled", true);*/
                        $('#partyName2').dxSelectBox('option', 'readOnly', true);
                        $('#BACCOUNTS2').dxSelectBox('option', 'readOnly', true);
                        $('#BACCOUNTS').dxSelectBox('option', 'readOnly', true);
                        $('#PARTY_BACCOUNTS').dxSelectBox('option', 'readOnly', true);
                        $('#cashAmount2').prop('readonly', true);
                        $('#bankrecv2').prop('readonly', true);
                        $('#partyrecv2').prop('readonly', true);
                        $('#partyrecv').prop('readonly', true);
                        $('#bankrecv').prop('readonly', true);
                        $('#cashAmount').prop('readonly', true);
                        $('#CardDisc').prop('readonly', true);
                        $('#billDiscPercent').prop('readonly', true);
                        $('#billDiscValue').prop('readonly', true);
                        $('#creditOption').prop('disabled', true)
                        $('.partyradioBtn input[type="radio"]').prop('disabled', true);
                    }
                    else {
                        $('#CardDisc').prop('readonly', false);
                        $('#billDiscPercent').prop('readonly', false);
                        $('#billDiscValue').prop('readonly', false);
                        $("#partyName").dxSelectBox("instance").option("readOnly", false);
                    }
                    ///*empr_POSTransaction.updateSummary(MultipleTaxValue);*/
                    //empr_POSTransaction.hideAllFields();
                    ///*empr_POSTransaction.ResetAllFields();*/
                    //cashFields.classList.remove("d-none");
                    //empr_POSTransaction.CreatePayQuickSearchGrid(data.data);
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
            }, false, true);
            //  //  
            var roundedDown = Math.floor(totalPayment / 10) * 10;

            var difference = totalPayment - roundedDown;

            if (difference > 0) {
                $("#settlementInput").attr("placeholder", difference);
            } else {
                $("#settlementInput").attr("placeholder", "");
            }

            if ($('#totalPayment').text().includes('-')) {
                $('#cashAmount').prop('readonly', true);
                $('#bankrecv').prop('readonly', true);
                $('#partyrecv').prop('readonly', true);
                $('#btnSplit').css('display', 'none');
            }

        }
        else {
            empr_helper.notify("Insufficient inventory! Please restock before processing sales.", 2);
        }
        if ($('#BILL_STATUS').val() == "P") {
            //$('#completecheck').click();
            $('#advAmount').prop('readonly', true);
        }
        else {
            $('#completecheck').prop({
                'checked': false,
                'readonly': true,
                'disabled': true
            }).closest('.form-check').addClass('disabled');
        }
    },

    CardDicount: function (code) {
        if (code !== "") {
            ajaxHelper.ajaxGetJson('/POSTransactions/GetCardDisc?CardNum=' + code, function (data) {
                console.log("AJAX response:", data);
                //  //  
                if (data.msgType == 1 && data.data.length > 0) {
                    let cardData = data.data[0];
                    const prevPoints = parseInt(cardData.pointRate) || 0;
                    const maxPoints = parseInt(cardData.maxPoint) || 100;
                    const pointStartValue = cardData.startValue || 0;
                    empr_POSTransaction.pointStartValue = cardData.startValue;
                    empr_POSTransaction.maxPoints = cardData.maxPoint;

                    let subTotalText = $('#subTotal').text().trim().replace(/[^0-9.]/g, '');
                    let subTotal = parseFloat(subTotalText) || 0;
                    let currentPoints = Math.floor(subTotal / empr_POSTransaction.pointStartValue) || 0;

                    //if (prevPoints + currentPoints > maxPoints)
                    //    currentPoints = maxPoints - prevPoints;
                    //if (currentPoints < 0) currentPoints = 0;


                    $('#CMOB').val(cardData.phoneNo);
                    $('#CNAME').val(cardData.fisrtName + ' ' + cardData.lastName);
                    $(".points-section").css("display", "");

                    $("#slider").attr("max", maxPoints).val(prevPoints + currentPoints);

                    $("#prevPoints").text(prevPoints);
                    $("#currentPoints").text(currentPoints);

                    setTimeout(() => {
                        empr_POSTransaction.updateSliderFill(prevPoints + currentPoints, maxPoints);
                    }, 50);
                }
                else {
                    $(".points-section").css("display", "none");
                    empr_helper.notify('Please enter Valid Card Number!', 2);
                }
                empr_POSTransaction.updateSummary();
            }, false, true);
        }
    },

    updateSliderFill: function (totalPoints, maxPoints) {
        const slider = document.getElementById('slider');
        if (!slider) return;
        const percentage = (totalPoints / maxPoints) * 100;
        slider.style.background = `linear-gradient(to right, #055a87 0%, #055a87 ${percentage}%, #d3d3d3 ${percentage}%, #d3d3d3 100%)`;
    },

    InitBankAccount: function (selectedValue) {
        $('#BACCOUNT').dxSelectBox({
            dataSource: BankAccount,
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
            searchTimeout: 500
        });
        $('#BACCOUNTS').dxSelectBox({
            dataSource: BankAccount,
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
            searchTimeout: 500
        });
        $('#PARTY_BACCOUNTS').dxSelectBox({
            dataSource: BankAccount,
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
            searchTimeout: 500
        });
        $('#BACCOUNTS2').dxSelectBox({
            dataSource: BankAccount,
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
            searchTimeout: 500
        });

    },

    IntiAdvanceBankAct(selectedValue) {
        $('#AdvAccount').dxSelectBox({
            dataSource: BankAccount,
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
            searchTimeout: 500
        });
    },

    BarcodePickGridData() {
        const gridElement = document.getElementById('BarcodePickGridContainer');
        const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);
        gridInstance.saveEditData().then(() => {
            var selectedBarcodes = gridInstance.getSelectedRowsData();
             
            if (selectedBarcodes.length > 0) {
                selectedBarcodes.forEach(function (barcode) {
                    empr_POSTransaction.loadItemsForData(barcode.iteM_CODE, barcode.code, barcode.barcode, barcode.quantity, barcode.disc);
                });


                $('#BarcodePickModal').modal('hide');
                $('#AllBarcodePickModal').modal('hide');
                gridInstance.dispose();
            } else {
                empr_helper.notify("Please select the items first.", 2);
            }
        });
    },

    GetBookingGridData() {

        const gridElement = document.getElementById('AdvancegridContainer');
        const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);
        const WhatsappMsg = $('#WHT_MSG').val();
        /*const formattedMessage = empr_POSTransaction.formatMessage(WhatsappMsg);*/
        //const formattedMessage = $('#WHT_MSG').val();

        gridInstance.saveEditData().then(() => {
            const selectedBarcodes = gridInstance.getSelectedRowsData();

            if (selectedBarcodes.length > 0) {
                console.log(selectedBarcodes);

                const uniqueMobileData = [];
                const seenMobiles = new Set();

                selectedBarcodes.forEach(item => {
                    const mobile = item.mobile?.trim();
                    const tranId = item.traN_ID;

                    if (mobile && !seenMobiles.has(mobile)) {
                        seenMobiles.add(mobile);
                        uniqueMobileData.push({ mobile, traN_ID: tranId });
                    }
                });

                // Clear previous tags
                $('#mobileNumbersContainer').empty();

                // Add mobile number tags to UI
                uniqueMobileData.forEach(item => {
                    const tag = $(`
                    <div class="mobile-tag">
                        <span class="remove-tag" data-number="${item.mobile}">&times;</span>
                        <span>${item.mobile}</span>
                    </div>
                `);
                    $('#mobileNumbersContainer').append(tag);
                });

                // Set hidden input values
                $('#mobileData').val(JSON.stringify(uniqueMobileData)); // for server use
                $('#mobileNumbers').val(uniqueMobileData.map(x => x.mobile).join(', ')); // optional

                // Tag removal logic
                $('#mobileNumbersContainer').off('click').on('click', '.remove-tag', function () {
                    const numToRemove = $(this).data('number');
                    $(this).parent().remove();

                    const updatedMobiles = $('#mobileNumbersContainer .mobile-tag span:last-child')
                        .map(function () { return $(this).text(); }).get();

                    const updatedData = uniqueMobileData.filter(x => updatedMobiles.includes(x.mobile));

                    $('#mobileData').val(JSON.stringify(updatedData));
                    $('#mobileNumbers').val(updatedMobiles.join(', '));
                });
                $('#messageText').val('');
                // Set default message text
                $('#messageText').val("Hello, Dear [Customer Name],\n\n" +
                    WhatsappMsg + "\n\n" +
                    "Thank you!\n\n" +
                    "Best Regards,\n" +
                    "[CompanyName]\n\n" +
                    "Contact:\n" +
                    "[Number]\n" +
                    "[Email]\n\n");

                // Show modal popup
                $('#sendMessageModal').modal('show');

            } else {
                empr_helper.notify("Please select the items first.", 2);
            }
        });
    },

    SentToWhatsApp() {

        $('#Loader').appendTo('body');
        $("#Loader").css({
            display: 'flex'
        });

        setTimeout(function () {

            const gridElement = document.getElementById('AdvancegridContainer');
            const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);
            var WhatsappToken = $('#WHT_TOKEN').val();
            var WhatsappUrl = $('#WHT_URL').val();

            const message = $('#messageText').val().trim();
            if (!message) {
                empr_helper.notify("Please enter a message.", 2);
                $("#Loader").hide();
                return;
            }

            const mobileDataJson = $('#mobileData').val();
            let mobileData = [];

            try {
                mobileData = JSON.parse(mobileDataJson);
            } catch (e) {
                empr_helper.notify("Failed to parse mobile data.", 2);
                $("#Loader").hide();
                return;
            }

            if (mobileData.length === 0) {
                empr_helper.notify("No mobile data found.", 2);
                $("#Loader").hide();
                return;
            }

            // Merge message into each entry
            const payload = mobileData.map(x => ({
                mobile: x.mobile,
                traN_ID: x.traN_ID,
                message: message,
                WhatsappUrl: WhatsappUrl,
                WhatsappToken: WhatsappToken
            }));

            // Disable button while sending
            $('#confirmSendBtn').prop('disabled', true).text('Sending...');

            $.ajax({
                url: '/POSTransactions/SendWhatsapp',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(payload),
                success: function (response) {
                    $('#confirmSendBtn').prop('disabled', false).text('Send');
                    $('#AdvanceModal').modal('hide');
                    $('#sendMessageModal').modal('hide');
                    gridInstance.dispose();
                    if (response.success == true) {
                        empr_helper.notify("Messages sent successfully!", 1);
                    }
                    else {
                        empr_helper.notify("Please Set Whatsapp Credentials!", 2);
                    }
                    $('#messageText').val('');
                    $("#Loader").hide();
                },
                complete: function () {
                    $("#Loader").hide();   // ✅ GUARANTEED HIDE
                },
                error: function () {
                    empr_helper.notify("Something went wrong while sending messages.", 2);
                },
            });
        }, 200);
    },

    formatMessage(rawText) {
        return rawText
            .replace("Hello, Dear", "Hello, Dear\n\n")
            .replace("pickup.", "pickup.\n")
            .replace("order is", "order is")
            .replace("balance is", "balance is")
            .replace("convenience.", "convenience.\n\n")
            .replace("Thank you!", "Thank you!\n\n")
            .replace("Best Regards,", "Best Regards,\n")
            .replace("Contact:", "\nContact:")
            .replace("[Email]", "[Email]\n\n")
            .replace("Empire Tecknologies", "Empire Tecknologies");
    },

    TodayDate() {
        var today = new Date();
        var LocalDate = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear()
        $('#V_DATE').text(LocalDate);
    },

    TodayDateTime() {
        var today = new Date();
        var date = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear();

        var hours = today.getHours();
        var minutes = today.getMinutes();
        var seconds = today.getSeconds();

        // Ensure 2-digit format
        hours = hours < 10 ? '0' + hours : hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;

        var time = hours + ':' + minutes + ':' + seconds;

        var LocalDateTime = date + ' ' + time;

        if (empr_POSTransaction.InvoiceDate != "") {
            $('#V_DATE').text(empr_POSTransaction.InvoiceDate);
        }
        else {
            $('#V_DATE').text(LocalDateTime);
        }


    },

    HoldDataSave(Id) {
        $('#paymentModal').modal('hide');
        // Ensure loader is above modal
        $('#Loader').appendTo('body'); // modal ke upar le jao
        $("#Loader").css({
            display: 'flex'
        });

        setTimeout(function () {
            let allPositive = $('.stock').toArray().every(el => $(el).data('stockqty') > 0);
            if (allPositive > 0 || !BarcodeTextBoxVisible) {
                if ($('#BILL_STATUS').val() == '' || $('#BILL_STATUS').val() == 'H') {
                    if (empr_POSTransaction.return != true) {
                        $('#BILL_STATUS').val('H');
                        empr_POSTransaction.billStatus = "H";
                        $('#IsKot').val(0);
                        $('#recvhidden').val('');
                        if (Permissions != "Admin") {
                            if (!$("#Code").val() && !Permissions.r_ADD) {
                                empr_helper.notify("You are not allowed to add new record !", 2);
                            } else {
                                empr_POSTransaction.SaveInfo(empr_POSTransaction.billStatus);
                            }
                        } else {
                            empr_POSTransaction.SaveInfo(empr_POSTransaction.billStatus);
                        }
                        empr_POSTransaction.updateSummary();
                        empr_POSTransaction.ResetForm();
                    }
                }
                else if ($('#BILL_STATUS').val() == 'K') {
                    empr_helper.notify("This invoice is Already Saved. if any update your record then press KOT", 2);
                }
                else {
                    empr_helper.notify("This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.", 2);
                }
            }
            else {
                empr_helper.notify("Insufficient inventory! Please restock before processing sales.", 2);
            }
            setTimeout(function () {
                $("#Loader").hide();
            }, 200);
        }, 100);
    },

    hideAllFields() {
        const panels = [cashFields, cardFields, partyFields, splitFields, advanceFields].filter(Boolean);
        panels.forEach(p => {
            p.classList.remove("pos-panel-active");
            p.classList.add("d-none");
            p.hidden = true;
            p.setAttribute("aria-hidden", "true");
        });
    },

    GetMapData() {
        console.log('GetMapData', MapData);
        if (MapData.msgType == 1) {
            //  //  
            if ($('#cashAmount').val() == "" || $('#cashAmount').val() == null) {
                $('#cashAmount').val($('#totalPayment').text());
            }
            const orderTable = document.getElementById("orderTable");
            if (MapData.data.length > 0) {
                $('#bankcharges').val(MapData.data[0].bcharges);
                $('#Acounthidden').val(MapData.data[0].casH_ACT);
                $('#bankhidden').val(MapData.data[0].banK_ACT);
                $('#bankhidden2').val(MapData.data[0].banK_ACT);
                $('#BACCOUNT').dxSelectBox('instance').option("value", MapData.data[0].banK_ACT);
                $('#BACCOUNTS').dxSelectBox('instance').option("value", MapData.data[0].banK_ACT);
                $('#PARTY_BACCOUNTS').dxSelectBox('instance').option("value", MapData.data[0].banK_ACT);
                $('#BACCOUNTS2').dxSelectBox('instance').option("value", MapData.data[0].banK_ACT);
                $('#AdvAccount').dxSelectBox('instance').option("value", MapData.data[0].banK_ACT);
                $('#cashtax').val(MapData.data[0].casH_TAX);
                $('#advcashtax').val(MapData.data[0].casH_TAX);
                $('#advbanktax').val(MapData.data[0].casH_TAX);
                $('#partytax').val(MapData.data[0].partY_TAX);
                $('#banktax').val(MapData.data[0].banK_TAX);
                $('#cashtax2').val(MapData.data[0].casH_TAX);
                $('#partytax2').val(MapData.data[0].partY_TAX);
                $('#banktax2').val(MapData.data[0].banK_TAX);
                $('#srbName').val(MapData.data[0].srbname);
                $('#srbNtn').val(MapData.data[0].srbntn);
                $('#posUser').val(MapData.data[0].posuser);
                $('#posPass').val(MapData.data[0].pospass);
                $('#srbId').val(MapData.data[0].srbid);
                $('#srbStatus').val(MapData.data[0].srbstatus);
                $('#srbUrl').val(MapData.data[0].srburl);
                $('#GroupImg').val(MapData.data[0].groupimg);
                $('#ItemImg').val(MapData.data[0].itemimg);
                $('#WaiterImg').val(MapData.data[0].waiterimg);
                $('#TableImg').val(MapData.data[0].tableimg);
                $('#WHT_URL').val(MapData.data[0].whatsappurl);
                $('#WHT_TOKEN').val(MapData.data[0].whatsapptoken);
                $('#WHT_MSG').val(MapData.data[0].whatsappmsg);
                $('#WHT_CC_MSG').val(MapData.data[0].whatsappccmsg);
                $('#WHT_MSG_RETURN').val(MapData.data[0].whtmsG_RETURN);
                $('#WHT_MSG_ADV').val(MapData.data[0].whtmsG_ADV);
                $('#WHT_ADV_COM').val(MapData.data[0].whT_ADVCOM);
                $('#WHT_PARTY_MSG').val(MapData.data[0].whT_PARTYMSG);
                empr_POSTransaction.pwindow = MapData.data[0].pwindow;
                $('#serviceChargesRate').val(MapData.data[0].seR_CHARGES);
                //  //  

                empr_POSTransaction.isPayQty = MapData.data[0].paY_COPIES;
                empr_POSTransaction.isPreQty = MapData.data[0].prE_COPIES;
                empr_POSTransaction.isDupQty = MapData.data[0].duP_COPIES;
                //empr_POSTransaction.isPayModal = MapData.data[0].paY_MODAL;
                //empr_POSTransaction.isPreModal = MapData.data[0].prE_MODAL;
                //empr_POSTransaction.isDupModal = MapData.data[0].duP_MODAL;
                empr_POSTransaction.isPayModal = (MapData.data[0].paY_MODAL == 1) ? "Y" : "N";
                empr_POSTransaction.isPreModal = (MapData.data[0].prE_MODAL == 1) ? "Y" : "N";
                empr_POSTransaction.isDupModal = (MapData.data[0].duP_MODAL == 1) ? "Y" : "N";

                MapData.data[0].salesmanreq == 'Y' ? empr_POSTransaction.salesmanReq = 'Y' : empr_POSTransaction.salesmanReq = 'N';
                //empr_POSTransaction.rateLock = MapData.data[0].rate;
                if (MapData.data[0].advancebtn == 'Y') {
                    if (Permissions != 'Admin') {
                        if (Permissions.r_VIEW)
                            $('#AdvanceBtn').show();
                    }
                    else
                        $('#AdvanceBtn').show();

                } else {
                    $('#AdvanceBtn').hide();
                }
                if (MapData.data[0].kotbtn == 'Y') {
                    $('#kot').show();
                } else {
                    $('#kot').hide();
                }
            }
        }
        else {
            empr_helper.notify(MapData.msg, MapData.msgType);
        }
        $("#prevPoints").text(0);
    },

    GetExpenseRecord() {
        ajaxHelper.ajaxGetJson('/POSTransactions/ExpenseRecord', function (data) {


            if (data != null) {

                empr_POSTransaction.ExpenseGrid(data);
            }
        }, false, true);
    },

    loadItemsForGroup(groupId) {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetItemsMasterByGroup?groupId=' + groupId, function (data) {
            setTimeout(function () {
                var ItemImage = $('#ItemImg').val();
                console.log(ItemImage);
                if (data.length > 0) {
                    $('#items-container').html('');
                    $('.middle').empty();

                    var itemsHtml = $.map(data, function (item) {
                        return `
                                    <div class="box gridDiv boxImageDesign" data-item="${item.iteM_CODE}">
                                    <img src="${item.ipic}" alt="" onerror="this.onerror=null;this.src='${ItemImage}';">
                                    <p>
                                        ${item.iteM_NAME}
                                    </p>
                                    </div>
                            `;
                    });
                    $('.middle').append(itemsHtml);

                } else {
                    $('.middle').html('<p>No items available for this group.</p>');
                }
            }, 300);
        });
    },

    refreshData(Id) {
        empr_POSTransaction.loadItemsForGroup(Id);
    },

    ResetAllFields() {
        const dropdownInstanceparty = $("#partyName").dxSelectBox("instance");
        dropdownInstanceparty.reset();
        const dropdownInstanceparty2 = $("#partyName2").dxSelectBox("instance");
        const dropdownInstanBank2 = $("#BACCOUNTS2").dxSelectBox("instance");
        dropdownInstanBank2.reset();
        dropdownInstanceparty2.reset();
        $('#cashAmount').prop('readonly', false);
        $('#cashAmount2').prop('readonly', false);
        $('#partyName').css('border', 'none');
        $('#BACCOUNTS2').css('border', 'none');
        $('#partyName2').css('border', 'none');
        $('#cashAmount2').css('border', 'none');
        $('#bankrecv2').css('border', 'none');
        $('#partyrecv2').css('border', 'none');



        /*$('#advAmount').prop('readonly', false);*/
        $('#cashtaxAmt2').val('');
        $('#banktaxAmt2').val('');
        $('#partytaxAmt2').val('');
        $('#Acounthidden2').val('');
        $('#cashAmount2').val('');
        $('#cashReturn').val('');
        //$('#isBillDiscPercent').val('');
        /*$('#cashremark').val('');*/
        $('#bankhidden2').val('');
        $('#banknamehidden2').val('');
        $('#banknamehidden').val('');
        $('#Returnbank').val('');
        $('#remarkbank').val('');
        $('#partyhidden2').val('');
        $('#partycode').val('');
        $('#partycode2').val('');
        $('#partynamehidden').val('');
        $('#partynamehidden2').val('');
        $('#partyBalance').val(0);
        $('#partyBalance2').val(0);
        $('#Returnparty').val('');
        $('#remarkparty').val('');
        $('#bankrecv').val('');
        $('#bankrecv2').val('');
        $('#partyrecv').val('');
        $('#partyrecv2').val('');
        if ($('#BILL_STATUS').val() != 'P') {
            $('#advAmount').val('');
            $('#fnlAmount').val('')
        }

        $('#advReturn').val('');
        //$('#CashTax_Amt2').val('');
        //$('#BankTax_Amt2').val('');
        //$('#PartyTax_Amt2').val('');
    },
    updateSummary(TaxValue) {
         
        const table = document.getElementById('tableGrid');
        let rows = table.getElementsByTagName('tbody')[0].rows;

        let visibleRows;
        let returnvisibleRows;
        if (empr_POSTransaction.return !== true) {
            visibleRows = Array.from(rows).filter(row => {
                const itemNameCell = row.querySelector('.itemname');
                return itemNameCell && itemNameCell.style.display !== 'none';
            });
        } else {
            visibleRows = Array.from(rows).filter(row => {

                const priceInput = row.querySelector('.total-price');
                const price = parseFloat(priceInput?.value || "0");
                const returnitemInput = row.querySelector('.returnitem');
                const returnitem = parseFloat(returnitemInput?.value || "0");
                return price < 0 || returnitem === 1;
            });
        }
        returnvisibleRows = Array.from(rows).filter(row => {

            const priceInput = row.querySelector('.price');
            const price = parseFloat(priceInput?.value || "0");
            //const price = parseFloat(row.querySelector('.total-price').value) || 0;
            const returnitemInput = row.querySelector('.returnitem');
            const returnitem = parseFloat(returnitemInput?.value || "0");
            //const returnitem = parseFloat(row.querySelector('.returnitem').value) || 0;
            return price < 0 || returnitem === 1;
        });

        let totalTax = 0;
        let totalAmount = 0;
        let stotalAmount = 0;
        let totalNetTotal = 0;


        let totalQuantity = 0;
        let totalItemsDiscval = 0;
        const totalItems = visibleRows.length;
        rows = empr_POSTransaction.return == true ? visibleRows : rows;
        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];
            //  

            //const quantityRaw = parseFloat(row.querySelector('.quantity-input').value) || 0;
            //const unitPriceRaw = parseFloat(row.querySelector('.total-price').value) || 0;
            //const quantity = empr_POSTransaction.isReturnMode ? -Math.abs(quantityRaw) : Math.abs(quantityRaw);
            //const unitPrice = empr_POSTransaction.isReturnMode ? -Math.abs(unitPriceRaw) : Math.abs(unitPriceRaw);
            //const quantity = parseFloat(row.querySelector('.quantity-input').value) || 0;
            const quantityInput = row.querySelector('.quantity-input');
            const quantity = parseFloat(quantityInput?.value || "0");
            const unitPriceInput = row.querySelector('.total-price');


            const unitPrice = parseFloat(unitPriceInput?.value || "0");
            //const valAmt = parseFloat(row.querySelector('.  -price').value) || 0;


            let discPercentEl = row.querySelector('.discountpercent');
            let discValueEl = row.querySelector('.discountvalue');

            let discPercent = parseFloat(discPercentEl?.value || "0");
            let discValue = parseFloat(discValueEl?.value || "0");

            let taxPInput = row.querySelector('.tax');


            const taxPercent = Math.abs(parseFloat(taxPInput?.value || "0"));

            const amt = quantity * unitPrice;
            //discValue = (discPercent / 100) * amt;
            //discValueEl.value = discValue.toFixed(2);


            if (discPercentEl && discValueEl) {
                if (document.activeElement === discValueEl) {
                    discValue = parseFloat(discValueEl.value) || 0;
                    discPercent = amt !== 0 ? (discValue / amt) * 100 : 0;
                    discPercentEl.value = discPercent.toFixed(2);

                } else if (document.activeElement === discPercentEl) {
                    discPercent = parseFloat(discPercentEl.value) || 0;
                    discValue = (discPercent / 100) * amt;
                    discValueEl.value = discValue.toFixed(2);

                } else {
                    // Default calculation if neither input is active
                    discPercent = parseFloat(discPercentEl.value) || 0;
                    discValue = (discPercent / 100) * amt;
                    discValueEl.value = discValue.toFixed(2);
                }
            }

             
            //var amtss = Math.abs(amt).toFixed(2);
            //var disccc=
            const taxAmount = (((amt - discValue) * taxPercent) / (100 + taxPercent)).toFixed(2);
            //const netAmount = (amt - discValue) + taxAmount;
            const netAmount = (amt - Math.abs(discValue));
            // Accumulate Totals based on Mode
            // Return Mode Logic
            if (empr_POSTransaction.return === true) {
                if (quantity < 0 || ritem === 1) {
                    //totalQuantity += quantity;
                    //totalAmount += amt;
                    //totalTax += taxAmount;
                    //totalItemsDiscval += discValue;
                    //totalNetTotal += netAmount;
                    totalQuantity -= Math.abs(quantity);
                    totalAmount -= Math.abs(amt);
                    //totalAmount -= Math.abs(amt);
                    totalTax -= Math.abs(taxAmount);
                    totalItemsDiscval -= Math.abs(discValue);
                    totalNetTotal -= Math.abs(netAmount);

                    row.querySelector('.amt-input').value = "-" + Math.abs(amt).toFixed(2);
                    row.querySelector('.net-amount').value = "-" + Math.abs(netAmount).toFixed(2);
                    row.querySelector('.tax').value = "-" + Math.abs(taxPercent).toFixed(2);
                    row.querySelector('.tax-amount').value = "-" + Math.abs(taxAmount).toFixed(2);
                    row.querySelector('.discountvalue').value = "-" + Math.abs(discValue).toFixed(2);
                    row.querySelector('.discountvalue').value = "-" + Math.abs(discValue).toFixed(2);
                    row.querySelector('.discountpercent').value = "-" + Math.abs(discPercent).toFixed(2);
                }
                else if (quantity !== 0) {
                    //totalQuantity += quantity;
                    //totalAmount += amt;
                    //totalNetTotal += netAmount;
                    //row.querySelector('.amt-input').value = amt.toFixed(2);
                    //row.querySelector('.net-amount').value = netAmount.toFixed(2);
                    //row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);

                    //row.querySelector('.tax').value = Math.abs(taxPercent).toFixed(2);
                    //var taxAMtAfter = row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);
                    //totalTax += taxAMtAfter;
                    //const isValueActive = document.activeElement === discValueEl;
                    //const isPercentActive = document.activeElement === discPercentEl;

                    //if (!isValueActive) {
                    //    row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);
                    //}

                    //if (!isPercentActive) {
                    //    row.querySelector('.discountpercent').value = Math.abs(discPercent).toFixed(2);
                    //}
                    //var taxDuiscAfter = row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);


                    //totalItemsDiscval += taxDuiscAfter;

                    totalQuantity += quantity;
                    totalAmount += amt;

                    row.querySelector('.amt-input').value = amt.toFixed(2);
                    row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);

                    row.querySelector('.tax').value = Math.abs(taxPercent).toFixed(2);
                    //var taxAMtAfter = row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);
                    var taxAMt = Math.abs(taxAmount);
                    row.querySelector('.tax-amount').value = taxAMt.toFixed(2);
                    totalTax += taxAMt;
                    const isValueActive = document.activeElement === discValueEl;
                    const isPercentActive = document.activeElement === discPercentEl;

                    if (!isValueActive) {
                        row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);
                    }

                    if (!isPercentActive) {
                        row.querySelector('.discountpercent').value = Math.abs(discPercent).toFixed(2);
                    }
                    //var taxDuiscAfter = 

                    var taxDuiscAfter = Math.abs(discValue); // numeric value
                    row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);
                    const NnetAmount = (amt - taxDuiscAfter);
                    row.querySelector('.net-amount').value = NnetAmount.toFixed(2);

                    totalNetTotal += NnetAmount;
                    totalItemsDiscval += taxDuiscAfter;


                }
            }
            // Normal Sale Mode Logic
            else {
                 
                if (quantity < 0) {
                    totalQuantity -= Math.abs(quantity);
                    totalAmount -= Math.abs(amt);
                    //totalAmount -= Math.abs(amt);
                    totalTax -= Math.abs(taxAmount);
                    totalItemsDiscval -= Math.abs(discValue);
                    //totalNetTotal -= Math.abs(netAmount);

                    row.querySelector('.amt-input').value = "-" + Math.abs(amt).toFixed(2);
                    //row.querySelector('.net-amount').value = "-" + Math.abs(netAmount).toFixed(2);
                    row.querySelector('.tax').value = "-" + Math.abs(taxPercent).toFixed(2);
                    row.querySelector('.tax-amount').value = "-" + Math.abs(taxAmount).toFixed(2);
                    row.querySelector('.discountvalue').value = "-" + Math.abs(discValue).toFixed(2);
                    row.querySelector('.discountvalue').value = "-" + Math.abs(discValue).toFixed(2);
                    row.querySelector('.discountpercent').value = "-" + Math.abs(discPercent).toFixed(2);
                    const NnetAmount = (amt - Math.abs(discValue));
                    row.querySelector('.net-amount').value = NnetAmount.toFixed(2);
                    totalNetTotal -= NnetAmount;


                } else if (quantity !== 0) {
                    totalQuantity += quantity;
                    totalAmount += amt;

                    row.querySelector('.amt-input').value = amt.toFixed(2);
                    row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);

                    row.querySelector('.tax').value = Math.abs(taxPercent).toFixed(2);
                    //var taxAMtAfter = row.querySelector('.tax-amount').value = Math.abs(taxAmount).toFixed(2);
                    var taxAMt = Math.abs(taxAmount);
                    row.querySelector('.tax-amount').value = taxAMt.toFixed(2);
                    totalTax += taxAMt;
                    const isValueActive = document.activeElement === discValueEl;
                    const isPercentActive = document.activeElement === discPercentEl;

                    if (!isValueActive) {
                        row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);
                    }

                    if (!isPercentActive) {
                        row.querySelector('.discountpercent').value = Math.abs(discPercent).toFixed(2);
                    }
                    //var taxDuiscAfter = 

                    var taxDuiscAfter = Math.abs(discValue); // numeric value
                    row.querySelector('.discountvalue').value = Math.abs(discValue).toFixed(2);
                    const NnetAmount = (amt - taxDuiscAfter);
                    row.querySelector('.net-amount').value = NnetAmount.toFixed(2);

                    totalNetTotal += NnetAmount;
                    totalItemsDiscval += taxDuiscAfter;


                }
            }
            //if (empr_POSTransaction.return !== true && quantity > 0) {
            //totalQuantity += quantity;
            //totalAmount += amt;
            //totalTax += taxAmount;
            //totalItemsDiscval += discValue;
            //}
        }

        const TotalItemDisc = Math.round(totalItemsDiscval);
        let TaxAMTs = Math.round(totalTax);
        let SubTotalAmount = Math.round(totalAmount);

        document.getElementById('totalItems').textContent = totalItems;
        document.getElementById('totalQty').textContent = totalQuantity;
        document.getElementById('subTotal').textContent = SubTotalAmount;
        //  
        let billdiscountValue = parseFloat($('#billDiscValue').val()) || 0;

        //if (empr_POSTransaction.totalPoints !== 0 || $('#CardDisc').val() !== '') {
        //    SubTotalAmount = Math.max(0, SubTotalAmount - billdiscountValue);
        //}

        // --- Mapdata Tax Calculation ---


        let TaxPercentage;
        if (TaxValue != null) {
            TaxPercentage = TaxValue;
            $('#taxperhidden').val(TaxPercentage);
            TaxAMTs = Math.round((TaxPercentage / 100) * totalAmount);
        } else {
            TaxPercentage = SubTotalAmount !== 0 ? Math.round((TaxAMTs / SubTotalAmount) * 100) : 0;
            $('#taxperhidden').val(TaxPercentage);
        }

        document.getElementById('totalTax').textContent = `Total Tax: ${TaxPercentage}%`;
        document.getElementById('subtotalTax').textContent = TaxAMTs;
        //var CalculatedNetTotal = SubTotalAmount - TotalItemDisc + totalTax;

        const CalculatedNetTotal = SubTotalAmount - TotalItemDisc;
        $('#NetTotal').text(CalculatedNetTotal);


        const mapDataItem = MapData.data[0];
        const payType = empr_POSTransaction.payType;

        let mapTaxPercent = 0;
        if (payType === "Cash") mapTaxPercent = parseFloat(mapDataItem.casH_TAX) || 0;
        else if (payType === "Bank") mapTaxPercent = parseFloat(mapDataItem.banK_TAX) || 0;
        else if (payType === "Party") mapTaxPercent = parseFloat(mapDataItem.partY_TAX) || 0;
        else if (payType === "Split") mapTaxPercent = parseFloat(mapDataItem.casH_TAX) || 0;



        const mapTaxAmount = Math.round((mapTaxPercent / 100) * CalculatedNetTotal);

        // Update UI
        $('#taxPer').text(mapTaxPercent + '%');
        $('#taxVal').text(mapTaxAmount);

        const Amount = Math.round(SubTotalAmount);
        document.getElementById('itemDiscount').textContent = TotalItemDisc;
        document.getElementById('TItemTax').textContent = TaxAMTs;

        //  
        const TotalAmountGet = Amount - TotalItemDisc;
        const isPercent = $('#isBillDiscPercent').is(':checked');
        const discInputVal = parseFloat($('#billDisc').val()) || 0;
        let billDisc = 0;
        let billDiscVal = 0;
        if (isPercent) {
            // User entered %
            billDisc = discInputVal;
            billDiscVal = (discInputVal / 100) * TotalAmountGet;
        } else {
            billDiscVal = discInputVal;
            //if (empr_POSTransaction.return == false) {
            //    billDisc = TotalAmountGet !== 0
            //        ? (discInputVal / TotalAmountGet) * 100
            //        : 0;
            //}
            //else {
            //    var total = Math.abs(parseFloat(TotalAmountGet) || 0);
            //    var discPercentage = parseFloat($('#billDiscPer').val()) || 0;

            //    billDisc = total !== 0
            //        ? (discPercentage / 100) * total
            //        : 0;

            //    $('#billDisc').val(Math.round(billDisc));
            //}
            billDisc = TotalAmountGet !== 0
                ? (discInputVal / TotalAmountGet) * 100
                : 0;

        }
         
        billDiscVal = Math.round(billDiscVal);
        billDisc = parseFloat(billDisc.toFixed(2));
        empr_POSTransaction.billDiscVal = billDiscVal;
        empr_POSTransaction.billDiscVal = billDiscVal;
        empr_POSTransaction.billDisc = billDisc;
        var totalamtget = TotalAmountGet.toFixed(2);

        let withDicsAmount = totalamtget.includes('-') ? -Math.abs(Math.abs(totalamtget) - billDiscVal) : totalamtget - billDiscVal;
        var fnlDics = returnvisibleRows.length > 0
            ? -Math.abs(Number(withDicsAmount))
            : Number(withDicsAmount);

        const deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;
        const serviceChargesPercent = parseFloat($('#serviceChargesPercent').val()) || 0;

        const getServiceCharges = empr_POSTransaction.SER_CHARGES === 0
            ? Math.round((serviceChargesPercent / 100) * fnlDics)
            : empr_POSTransaction.SER_CHARGES;

        $('#serviceChargesValue').val(getServiceCharges);

        let deliveryAmount = parseInt($('#billDelValue').val()) || 0;

        if (deliveryAmount === 0 && deliveryPercent > 0) {
            deliveryAmount = Math.round((deliveryPercent / 100) * fnlDics);
            $('#billDelValue').val(deliveryAmount);
        }
         
        let settlement = Math.abs(parseFloat($('#settlementInput').val())) || 0;
        let settleSign = $("#SettleSign").val() || "";
        let finalAmount =
            (parseFloat(fnlDics) || 0) +
            (parseFloat(deliveryAmount) || 0) +
            (parseFloat(getServiceCharges) || 0);
        let value;

        if (settleSign !== "") {
            if (settleSign === "-") {
                value = finalAmount - settlement;
            } else if (settleSign === "+") {
                value = finalAmount + settlement;
            }
        }

        const TotalValue = Math.round(value);


        $('#totalAmountGet').val(fnlDics);
        $('#FinalAmount').html(`Value ${TotalValue} /-`);
        $('#totalPayment').text(TotalValue);
        $('#TotalPaymenthidden').val(finalAmount);
        $('#cashAmount').val(TotalValue);
        $('#bankrecv').val(TotalValue);
        $('#partyrecv').val(TotalValue);
        //$('#recvhidden').val(TotalValue);
        $('#BILL_STATUS').val() === 'H'
            ? $('#recvhidden').val(0)
            : $('#recvhidden').val(TotalValue);

        //const mapDataItem = MapData.data[0];
        //var pType = empr_POSTransaction.payType;
        // 
        //if (pType == "Cash") {
        //    tTax = $('#taxPer').text(mapDataItem.casH_TAX ,"%");
        //} else if (pType == "Bank") {
        //    tTax = $('#taxPer').text(mapDataItem.banK_TAX,"%");
        //} else if (pType == "Party") {
        //    tTax = $('#taxPer').text(mapDataItem.partY_TAX,"%");
        //}
        empr_POSTransaction.AccountTax = "Cash";
        empr_POSTransaction.CardDiscountupdate();
    },


    //uzair here //////
    //updateSummary(TaxValue) {
    //      //  
    //    const table = document.getElementById('tableGrid');
    //    const rows = table.getElementsByTagName('tbody')[0].rows;
    //    var visibleRows = null;
    //    if (empr_POSTransaction.return != true) {
    //        visibleRows = Array.from(rows).filter(row => {
    //            const itemNameCell = row.querySelector('.itemname');
    //            return itemNameCell && itemNameCell.style.display !== 'none';
    //        });
    //    }
    //    else {
    //        visibleRows = Array.from(rows).filter(row => {
    //            const price = row.querySelector('.total-price').value;
    //            const returnitem = row.querySelector('#returnitem').value;
    //            return price < 0 || returnitem == 1;
    //        });
    //    }
    //    let totalTax = 0;
    //    let totalAmount = 0;
    //    let totalQuantity = 0;
    //    let totalItems = visibleRows.length;
    //    let totalItemsDiscval = 0;
    //    let discountPercent = 0;
    //    let billdiscountValue = 0;
    //    let Amount = 0

    //    for (let i = 0; i < rows.length; i++) {
    //        // Inputs lena (Aapke bataye hue indexes par)
    //        const quantity = parseFloat(rows[i].cells[2].querySelector('input').value) || 0;
    //        const unitPrice = parseFloat(rows[i].cells[5].querySelector('input').value) || 0;
    //        const discPercent = parseFloat(rows[i].cells[6].querySelector('input').value) || 0;
    //        const taxPercent = parseFloat(rows[i].cells[8].querySelector('input').value) || 0;

    //        // 1. Gross Amount (Amt) = Qty * Price
    //        const amt = quantity * unitPrice;

    //        // 2. Discount Calculation
    //        const disval = (discPercent / 100) * amt;

    //        // 3. Tax Amount (Discount ke baad wale amount par)
    //        const taxAmount = (taxPercent / 100) * (amt - disval);

    //        // 4. Net Amount
    //        const netAmount = (amt - disval) + taxAmount;

    //        rows[i].cells[7].querySelector('input').value = amt.toFixed(2);

    //        // Nayi fields (Net Amount waghera) agar aage cells hain toh:
    //        if (rows[i].cells[9]) {
    //            rows[i].cells[9].querySelector('input').value = netAmount.toFixed(2);
    //        }

    //        // Global Totals update karna
    //        if (empr_POSTransaction.return !== true) {
    //            if (quantity > 0) {
    //                totalQuantity += quantity;
    //                totalAmount += amt;
    //                totalTax += taxAmount;
    //                totalItemsDiscval += disval;
    //            }
    //        }
    //    }
    //    var TotalItemDisc = parseInt(totalItemsDiscval);
    //    var TaxAMTs = parseInt(totalTax);
    //    var SubTotalAmount = Math.round(totalAmount);
    //    document.getElementById('totalItems').textContent = totalItems; 
    //    document.getElementById('totalQty').textContent = totalQuantity; 
    //    document.getElementById('subTotal').textContent = SubTotalAmount;
    //    billdiscountValue = parseFloat($('#billDiscValue').val()) || 0;
    //    if (empr_POSTransaction.totalPoints != 0 || $('#CardDisc').val() != '') {
    //        if (billdiscountValue > SubTotalAmount) {
    //            SubTotalAmount = 0;
    //        }
    //        else {
    //            SubTotalAmount = SubTotalAmount - billdiscountValue;
    //        }
    //    }
    //    if (TaxValue != null) {
    //        var TaxPercentage = TaxValue;
    //        $('#taxperhidden').val(TaxPercentage);
    //        document.getElementById('totalTax').textContent = `Total Tax: ${TaxPercentage}%`;
    //        TaxAMTs = Math.round((TaxPercentage / 100) * totalAmount);
    //        document.getElementById('subtotalTax').textContent = TaxAMTs;
    //        totalTax = TaxAMTs
    //    }
    //    else {
    //        var TaxPercentage = Math.round((TaxAMTs / SubTotalAmount) * 100);
    //        $('#taxperhidden').val(TaxPercentage);
    //        document.getElementById('totalTax').textContent = `Total Tax: ${TaxPercentage}%`;
    //        document.getElementById('subtotalTax').textContent = TaxAMTs;
    //        totalTax = TaxAMTs
    //    }
    //    var CalculatedNetTotal = SubTotalAmount - TotalItemDisc + totalTax;

    //    $('#NetTotal').text(CalculatedNetTotal);
    //    Amount = parseInt(totalTax + SubTotalAmount) || 0;
    //    document.getElementById('itemDiscount').textContent = TotalItemDisc;



    //    //document.getElementById('itemDiscount').textContent = TotalItemDisc;


    //    //  //  
    //    //var TotalAmountGet = (Amount - TotalItemDisc);

    //    //$('#totalAmountGet').val(TotalAmountGet);
    //    //var isPercent = $('#isBillDiscPercent').is(':checked');
    //    //var discInputValue = parseFloat($('#billDisc').val()) || 0;
    //    //var calculatedBillDisc = 0;

    //    //if (isPercent) {

    //    //    calculatedBillDisc = (discInputValue / 100) * TotalAmountGet;
    //    //    $('#billDiscValue').val(calculatedBillDisc.toFixed(2));
    //    //} else {
    //    //    calculatedBillDisc = discInputValue;
    //    //    $('#billDiscValue').val(calculatedBillDisc);
    //    //}
    //    //billdiscountValue = calculatedBillDisc;
    //    //// --- NEW LOGIC END ---

    //    ////$('#TotalbillDisc').val(TotalAmountGet);
    //    ////var deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;

    //    //$('#TotalbillDisc').val(TotalAmountGet);
    //    //var deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;
    //    //var serviceChargesPercent = parseFloat($('#serviceChargesPercent').val()) || 0;
    //    //if (Amount == 0) {
    //    //    discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
    //    //    $('#billDiscPercent').val(discountPercent);
    //    //    $('#billDiscValue').val(billdiscountValue);
    //    //}
    //    //discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
    //    //if (discountPercent == 0 && deliveryPercent == 0 && serviceChargesPercent == 0) {
    //    //    $('#FinalAmount').html(`Value ${TotalAmountGet} /-`);
    //    //    $('#totalPayment').text(TotalAmountGet);
    //    //    $('#TotalPaymenthidden').val(TotalAmountGet);
    //    //    if ($('#cashAmount').val() == "" || $('#cashAmount').val() == null) {
    //    //        $('#cashAmount').val(TotalAmountGet);
    //    //    }
    //    //    $('#BILL_STATUS').val() == 'K' ? $('#recvhidden').val(0) : $('#recvhidden').val(TotalAmountGet);
    //    //    empr_POSTransaction.AccountTax = "Cash";
    //    //}
    //    //else {
    //    //    var getdiscountAmount = parseInt((discountPercent / 100) * TotalAmountGet);
    //    //    if (billdiscountValue == 0)
    //    //        billdiscountValue = getdiscountAmount;
    //    //    var discountAmount = Math.round(billdiscountValue);
    //    //    var withDicsAmount = TotalAmountGet - discountAmount;
    //    //    var getServiceCharges = empr_POSTransaction.SER_CHARGES == 0 ? parseInt((serviceChargesPercent / 100) * withDicsAmount) : empr_POSTransaction.SER_CHARGES;
    //    //    $('#serviceChargesValue').val(getServiceCharges);
    //    //    var getdeliveryCharges = $('#billDelValue').val();
    //    //    if (getdeliveryCharges == 0) {
    //    //        var getdeliveryCharges = parseInt((deliveryPercent / 100) * withDicsAmount);
    //    //        $('#billDelValue').val(getdeliveryCharges);
    //    //    }
    //    //    $('#totalAmountGet').val(withDicsAmount);

    //    //    var deliveryAmount = parseInt($('#billDelValue').val()) || 0;
    //    //    var serviceChargesAmount = parseInt($('#serviceChargesValue').val()) || 0;
    //    //    var finalAmount = withDicsAmount + deliveryAmount + serviceChargesAmount;
    //    //    var TotalValue = Math.floor(finalAmount);
    //    //    $('#FinalAmount').html(`Value ${TotalValue} /-`);
    //    //    $('#totalPayment').text(TotalValue);
    //    //    $('#TotalPaymenthidden').val(TotalValue);
    //    //    if ($('#cashAmount').val() == "" || $('#cashAmount').val() == null) {
    //    //        $('#cashAmount').val(TotalValue);
    //    //    }
    //    //    $('#BILL_STATUS').val() == 'K' ? $('#recvhidden').val(0) : $('#recvhidden').val(TotalValue);
    //    //    empr_POSTransaction.AccountTax = "Cash";
    //    //}

    //  //  
    //  //  
    //    var TotalAmountGet = (Amount - TotalItemDisc);


    //    var isPercent = $('#isBillDiscPercent').is(':checked');
    //    var discInputVal = parseFloat($('#billDisc').val()) || 0;
    //    var calculatedBillDisc = 0;

    //    if (isPercent) {
    //        calculatedBillDisc = (discInputVal / 100) * TotalAmountGet;
    //    } else {
    //        calculatedBillDisc = discInputVal;
    //    }

    //     billdiscountValue = Math.round(calculatedBillDisc);
    //    var withDicsAmount = TotalAmountGet - billdiscountValue;

    //    var deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;
    //    var serviceChargesPercent = parseFloat($('#serviceChargesPercent').val()) || 0;

    //    var getServiceCharges = empr_POSTransaction.SER_CHARGES == 0 ?
    //        parseInt((serviceChargesPercent / 100) * withDicsAmount) :
    //        empr_POSTransaction.SER_CHARGES;
    //    $('#serviceChargesValue').val(getServiceCharges);

    //    var deliveryAmount = parseInt($('#billDelValue').val()) || 0;
    //    if (deliveryAmount == 0 && deliveryPercent > 0) {
    //        deliveryAmount = parseInt((deliveryPercent / 100) * withDicsAmount);
    //        $('#billDelValue').val(deliveryAmount);
    //    }

    //    // 4. Final Amount (Net Total + Charges)
    //    var finalAmount = withDicsAmount + deliveryAmount + getServiceCharges;
    //    var TotalValue = Math.floor(finalAmount);


    //    $('#totalAmountGet').val(withDicsAmount);

    //    $('#FinalAmount').html(`Value ${TotalValue} /-`);

    //    // Agar aapko specific kisi jagah sirf discount minus wala amount chahiye:
    //    $('#totalPayment').text(TotalValue);
    //    $('#TotalPaymenthidden').val(TotalValue);

    //    //if ($('#cashAmount').val() == "" || $('#cashAmount').val() == null || $('#cashAmount').val() == 0) {
    //        $('#cashAmount').val(TotalValue);
    //    //}

    //    // Status aur Card Update
    //    $('#BILL_STATUS').val() == 'K' ? $('#recvhidden').val(0) : $('#recvhidden').val(TotalValue);
    //    empr_POSTransaction.AccountTax = "Cash";
    //    empr_POSTransaction.CardDiscountupdate();
    //},

    CardDiscountupdate() {
        if (empr_POSTransaction.pointStartValue != 0 && empr_POSTransaction.maxPoints != 0) {
            const prevPoints = parseInt($("#prevPoints").text()) || 0;

            let subTotalText = $('#subTotal').text().trim().replace(/[^0-9.]/g, '');
            let subTotal = parseFloat(subTotalText) || 0;
            let currentPoints = Math.floor(subTotal / empr_POSTransaction.pointStartValue);

            //if (prevPoints + currentPoints > empr_POSTransaction.maxPoints)
            //    currentPoints = empr_POSTransaction.maxPoints - prevPoints;
            if (currentPoints < 0) currentPoints = 0;

            $(".points-section").css("display", "");

            $("#slider").attr("max", empr_POSTransaction.maxPoints).val(prevPoints + currentPoints);

            $("#prevPoints").text(prevPoints);
            $("#currentPoints").text(currentPoints);

            setTimeout(() => {
                empr_POSTransaction.updateSliderFill(prevPoints + currentPoints, empr_POSTransaction.maxPoints);
            }, 50);
            //empr_POSTransaction.CardDicount(CardNum);
        }
    },

    updateHiddenField(selectedId) {
        const cardOption = document.getElementById("cardOption");
        //const advBank = document.getElementById("advbank");
        //const advCard = document.getElementById("advcard");

        //if (cardOption.checked) {
        //    advBank.style.display = "flex";
        //    advCard.style.gap = "77px";
        //    advCard.style.marginLeft = "50px";
        //} else {
        //    advBank.style.setProperty("display", "none", "important");
        //    advCard.style.gap = "177px";
        //    advCard.style.marginLeft = "126px";
        //}

        let labelText = document.querySelector('label[for="' + selectedId + '"]').innerText;

        empr_POSTransaction.billMode = labelText;
    },

    selectRadioByLabel(labelText) {
        let labelElement = Array.from(document.querySelectorAll('.form-check-label'))
            .find(label => label.innerText.trim() === labelText);

        if (labelElement) {
            let selectedId = labelElement.getAttribute('for');

            document.getElementById(selectedId).checked = true;

            empr_POSTransaction.billMode = labelText;
            /*document.getElementById('selectedPaymentMode').value = empr_POSTransaction.billMode;*/

        }
    },

    DeliveryOnChangeFunction(Id) {
        //  //  
        let totalAmount = parseFloat($('#totalAmountGet').val()) || 0;
        let percentInput = $("#billDelPercent");
        let valueInput = $("#billDelValue");
        if (Id === "billDelPercent") {
            let percentage = parseFloat(percentInput.val()) || 0;
            let calculatedValue = Math.round((totalAmount * percentage) / 100);
            valueInput.val(calculatedValue);
        } else if (Id === "billDelValue") {
            let value = parseFloat(valueInput.val()) || 0;
            let calculatedPercent = Math.round((value / totalAmount) * 100);
            percentInput.val(calculatedPercent);
        }
    },

    applySettlement(sign) {
        var PreviousTotal = parseInt($('#TotalPaymenthidden').val()) || 0;
        var settValue = Math.abs(Number($('#settlementInput').val())) || 0;
        var selectedBtn = $(".pay-tab.active");
        var rights = empr_POSTransaction.userRights;
        if (rights) {
            if ((rights.settF > settValue && rights.settF != settValue) && settValue != 0) {
                empr_helper.notify(`Please Type Greter Than or Equal This Value ${rights.settF}`, 2);
                $("#totalPayment").text(PreviousTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(PreviousTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(PreviousTotal);
                else
                    $("#partyrecv").val(PreviousTotal);

                $('#recvhidden').val(PreviousTotal);
                return
            }
            else if ((rights.settT < settValue && rights.settT != settValue) && settValue != 0) {
                empr_helper.notify(`Please Type Less Than or Equal This Value ${rights.settT}`, 2);
                $("#totalPayment").text(PreviousTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(PreviousTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(PreviousTotal);
                else
                    $("#partyrecv").val(PreviousTotal);

                $('#recvhidden').val(PreviousTotal);
                return

            }
            if (settValue < PreviousTotal) {

                var finalTotal = PreviousTotal;

                if (settValue > 0) {
                    finalTotal = (sign === '+')
                        ? PreviousTotal + settValue
                        : PreviousTotal - settValue;
                }

                $("#totalPayment").text(finalTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(finalTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(finalTotal);
                else
                    $("#partyrecv").val(finalTotal);
                $('#settlementInput').val(`${sign}${settValue}`);
                $('#recvhidden').val(finalTotal);
                $('#cashReturn').val(0);
            }
            else {
                empr_helper.notify('Please Type Less Than Total Payment Value ', 2)
                $("#totalPayment").text(PreviousTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(PreviousTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(PreviousTotal);
                else
                    $("#partyrecv").val(PreviousTotal);
                $('#recvhidden').val(PreviousTotal);
            }
        }
        else {
            if (settValue < PreviousTotal) {

                var finalTotal = PreviousTotal;

                if (settValue > 0) {
                    finalTotal = (sign === '+')
                        ? PreviousTotal + settValue
                        : PreviousTotal - settValue;
                }

                $("#totalPayment").text(finalTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(finalTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(finalTotal);
                else
                    $("#partyrecv").val(finalTotal);
                $('#settlementInput').val(`${sign}${settValue}`);
                $('#recvhidden').val(finalTotal);
                $('#cashReturn').val(0);
            }
            else {
                empr_helper.notify('Please Type Less Than Total Payment Value ', 2)
                $("#totalPayment").text(PreviousTotal);
                if (selectedBtn.attr("id") === "btnCash")
                    $("#cashAmount").val(PreviousTotal);
                else if (selectedBtn.attr("id") === "btnCard")
                    $("#bankrecv").val(PreviousTotal);
                else
                    $("#partyrecv").val(PreviousTotal);
                $('#recvhidden').val(PreviousTotal);
            }
        }


    },

    // Ammar Start
    ServiceChargesOnChangeFunction(Id) {
        //  //  
        let totalAmount = parseFloat($('#totalAmountGet').val()) || 0;
        let percentInput = $("#serviceChargesPercent");
        let valueInput = $("#serviceChargesValue");
        if (Id === "serviceChargesPercent") {
            // If user changes the percentage input
            let percentage = parseInt(percentInput.val()) || 0;
            let calculatedValue = Math.round((totalAmount * percentage) / 100);
            valueInput.val(calculatedValue); // Update the value input
        } else if (Id === "serviceChargesValue") {
            // If user changes the value input
            let value = parseFloat(valueInput.val()) || 0;
            let calculatedPercent = Math.round((value / totalAmount) * 100);
            percentInput.val(calculatedPercent); // Update the percentage input
        }
        else if (Id === "clear") {
            valueInput.val('');
            percentInput.val('');
        }
    },
    // Ammar End
    DiscountOnChangeFunction(Id) {
        let totalAmount = parseFloat($('#TotalbillDisc').val()) || 0;
        let percentInput = $("#billDiscPercent");
        let valueInput = $("#billDiscValue");
        let DelpercentInput = $("#billDelPercent");
        let DelvalueInput = $("#billDelValue");
        if (Id === "billDiscPercent") {
            // If user changes the percentage input
            let percentage = parseFloat(percentInput.val()) || 0;
            let calculatedValue = Math.round((totalAmount * percentage) / 100);
            valueInput.val(calculatedValue);
            let delpercentage = parseFloat(DelpercentInput.val()) || 0;
            let delcalculatedValue = Math.round((totalAmount * delpercentage) / 100);
            DelvalueInput.val(delcalculatedValue); // Update the value input
        } else if (Id === "billDiscValue") {
            // If user changes the value input
            let value = parseFloat(valueInput.val()) || 0;
            let calculatedPercent = ((value / totalAmount) * 100).toFixed(2);
            percentInput.val(calculatedPercent);
            let delvalue = parseFloat(DelvalueInput.val()) || 0;
            let delcalculatedPercent = Math.round((delvalue / totalAmount) * 100);
            DelpercentInput.val(delcalculatedPercent); // Update the percentage input
        }
    },

    printContent_QZ(kotPath, barcodePath, KotPrinter, StickerPrinter) {

        //ajaxHelper.ajaxGetJson('/POSTransactions/PrintPdfThermal', function (data) {

        //}, false, true);

        fetch("http://localhost:5055/print", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                kotFilePath: kotPath,
                barcodeFilePath: barcodePath,
                KotprinterName: KotPrinter,
                BarprinterName: StickerPrinter
            })
        })
            .then(r => r.json())
            .then(console.log)
            .catch(console.error);

    },

    printContent() {
        // Permissions check same as before...
        //  //  
        const printContent = document.getElementById('slipPreview').innerHTML;

        const printFrame = document.createElement('iframe');
        printFrame.style.position = 'absolute';
        printFrame.style.top = '-10000px';
        document.body.appendChild(printFrame);

        const frameDoc = printFrame.contentWindow || printFrame.contentDocument;
        frameDoc.document.open();
        frameDoc.document.write(`
        <html>
            <head>
                <style>
                    @media print {
                        @page {
                            size: 80mm auto;
                            margin: 0;
                        }
                        body {
                            width: 80mm;
                            margin: 0;
                            padding: 5px;
                            font-size: 12px;
                        }
                        table {
                            width: 100%;
                            border-collapse: collapse;
                        }
                        th, td {
                            padding: 3px;
                            word-wrap: break-word;
                        }
                        #body tr:last-child {
                            border-top: 0.5px solid black !important;
                        }
                        .page-break {
                            page-break-before: always;
                        }
                        * {
                            box-sizing: border-box;
                        }
                        textarea {
                            border: none !important;
                            resize: none !important;
                            overflow: hidden !important;
                            outline: none !important;
                        }
                    }
                    body {
                        width: 80mm;
                    }
                </style>
            </head>
            <body>
                ${printContent}
            </body>
        </html>
    `);
        frameDoc.document.close();

        $('#printModal').modal('hide');

        const images = frameDoc.document.images;
        let imagesLoaded = 0;

        function checkImagesLoaded() {
            imagesLoaded++;
            if (imagesLoaded === images.length) {
                frameDoc.focus();
                frameDoc.print();
                document.body.removeChild(printFrame);
            }
        }

        if (images.length > 0) {
            for (let img of images) {
                img.onload = checkImagesLoaded;
                img.onerror = checkImagesLoaded;
            }
        } else {
            frameDoc.focus();
            frameDoc.print();
            document.body.removeChild(printFrame);
        }

        // Reset part same...
    },

    SetItemsEditData(newData) {
        debugger;
        let tableBody = document.getElementById('orderTable');
        var ReturnItem = 0;
        console.log(newData);
        var rights = empr_POSTransaction.userRights;
        newData.forEach(function (item) {
            empr_POSTransaction.rowCount++;
            $('#DT_CODE').val(item.dT_CODE);
            if (BarcodeTextBoxVisible) {
                //  
                const selectedItem = Barcodes.filter(u => u.key == item.iteM_CODE);
                var Barcode = selectedItem[0].barcode;
                console.log(selectedItem);
 

                if (false) {
                    let quantityInput = existingRow.querySelector('.quantity-input');
                    let newQuantity = parseInt(quantityInput.value) + 1;
                    quantityInput.value = newQuantity;
                    tableBody.insertBefore(existingRow, tableBody.rows[0]);

                    if (item.stockstatus == 'Y') {
                        if (newQuantity <= item.stockqty) {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === Barcode) {
                                    tableRow.querySelector('.stock').textContent = '';
                                }
                            });
                        }
                        else {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === Barcode) {
                                    tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                                }
                            });
                        }
                    }

                } else {
                    let barcodeRow = tableBody.insertRow(0);
                     
                    let stockColor = item.stockqty <= 0 ? 'red' : 'green';
                    let stkstatus = item.stockqty <= 0 ? 'OUT STOCK' : 'IN STOCK';
                    barcodeRow.innerHTML = `
                                       
                                        <td style="display:none;">
                                            <input type="hidden" class="SelectedItemId" value="${item.iteM_CODE}">
                                            <input type="hidden" class="stock" data-stockqty="${item.stockqty}" value="${item.stock}">
                                        </td>

                                      
                                         
                                            <input type="hidden" class="selectedSize" value="${item.sizE_CODE}">
                                            <input type="hidden" class="selectedColor" value="${item.coloR_CODE}">
                                        
                                        

                                     
                                        
                                         
                                        <td style="display: none;"><input type="hidden" id="returnitem" value="0">0</td>
                                        

                                        <td style="display: none;"><input type="hidden" id="ritem" value="${item.ritem}">${item.ritem}</td>
                                        <td style="display: none;"><input type="hidden" id="returnitem" value="${ReturnItem}">$${ReturnItem}</td>

                                        <td style="display:none;"><input type="number" class="dtcode" id="dtcode" value="${item.dT_CODE ?? 0}">${item.dT_CODE ?? 0}</td>
                                        <td style="display: none;"><input type="hidden" id="pickid" value="0">0</td>
                                     <td style="display: none;"><input type="hidden" class="remarks" id="remarks" value=""></td>
                                        <td style="display: none;"><input type="hidden" class="kotprint" id="kotprint" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="count" value="${empr_POSTransaction.rowCount}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                    `;

                    rowindex = 1;
                    let newRow = tableBody.insertRow(rowindex);
                    newRow.innerHTML = `
                                  <tr>
                                        <td class="itemname" style=" font-size: 11px;">
                                            <strong>${item.iteM_NAME}</strong><br>
                                        <input type="hidden" class="BarcodeId" value="${item.iteM_CODE}">

                                           
                                        </td>
                                        <td>
                                         <small>${item.barcode}</small>
                                        </td>
                                  
                                       <td>
                                            <input type="number" class="quantity-input text-center w-100" value="${item.qty}" >
                                        </td>
                                        <td>
                                            <strong>${item.size}</strong><br>

                                        </td>
                                         <td>
                                            <strong>${item.color}</strong><br>

                                        </td>
                                          <td>
                                            <input type="number" class="total-price text-center w-100" value="${item.rate}"  style=" border:none;"  readonly>
                                        </td> 
                                          <td>
                                            <input type="number" class="amt-input  text-center w-100" value="${item.amt}" style=" border:none;" readonly>
                                        </td>


                                        <td>
                                          <input type="number" class="discountpercent text-center w-100"
                                                 value="${item.disc}" 
                                                 style="display: block; border:none; margin-bottom: 5px;" readonly>
                                          <input type="number" class="discountvalue text-center w-100" 
                                                 value="${item.disC_AMT}" 
                                                 style="display: block; border:none;" readonly >
                                        </td>

                                        <td>
                                          <input type="number" class="tax text-center w-100"
                                                 value="${item.tax}" readonly 
                                                 style="display: block; border:none; margin-bottom: 5px;" readonly>
                                          <input type="number" class="tax-amount text-center w-100" 
                                                 value="${item.taX_AMT}" readonly
                                                 style="display: block; border:none;"readonly >
                                            
                                        </td>

                                        <td style="position: relative; padding-top: 20px !important;">
                                            <div style="position: absolute; top: 0; left: 0; width: 100%; background: #ffffff;
                                                        color: ${stockColor}; font-size: 9px; padding: 1px 5px; 
                                                        text-align: right; border-bottom: 1px solid #ddd; font-weight: 600;">
                                              HAS ONLY ${item.stockqty} 
                                            </div>
                                            <input type="number" class="net-amount text-center w-100" value="${Math.round(item.neT_AMT)}" style="font-weight:bold; border:none;" readonly>
                                        </td>

                                        <td class="icons" style="position: relative; padding-top: 20px !important;">
                                            <div style="position: absolute; top: 0; left: 0; width: 100%; background: #ffffff;border:none; 
                                                        color: ${stockColor}; font-size: 9px; padding: 1px 5px; 
                                                        text-align: left; border-bottom: 1px solid #ddd; font-weight: 600;">
                                                        ${stkstatus}
                                            </div>
                                            <div class="tablecheckbox d-flex gap-3 align-items-center justify-content-center">
                                                <i id="tablechk" class="fas ms-5 fa-sign-out-alt row-checkbox tablechk"></i>
                                                <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                            </div>
                                        </td>
                                        <td class="icons d-none" style="display:flex ;">
                                            <div class="tablecheckbox">
                                            <i id="tablechks" class="fas fa-sign-out-alt row-checkbox tablechks
                                               ${rights ? rights.iReturn === 1 ? 'disabled-btn' : '' : ''}">
                                            </i>
                                            </div>

                                        </td>
                                        <td style="display: none;" colspan="2" class="Barcode">${item.barcode}</td>
                                        <td style="display: none;"><input type="hidden" id="returnitem" value="${ReturnItem}">$${ReturnItem}</td>
                                        <td style="display: none;"><input type="hidden" id="ritem" value="${item.ritem}">${item.ritem}</td>
                                        <td style="display: none;"> <input type="hidden" id="ritem1" value="${item.riteM1}"></td>

                                        <td style="display: none;"><input type="number" class="dtcode" id="dtcode" value="${item.dT_CODE}">${item.dT_CODE}</td>
                                        <td style="display: none; colspan="3" class="stock" data-stockqty="${item.stockqty}" style="color: red; text-align: right;"></td>
                                        <td style="display: none;"><input type="hidden" id="stockstatus" value="${item.stockstatus}"></td>
                                        <td style="display: none;"><input type="hidden" id="pickid" value="${item.pickid}">${item.pickid}</td>
                                        <td style="display: none;"><input type="hidden" class="remarks" id="remarks" value="${item.remarks}"></td>
                                        <td style="display: none;"><input type="hidden" class="kotprint" id="kotprint" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="count" value="${empr_POSTransaction.rowCount}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                        
                                    </tr>
                                `;
                }
            }
            else {
                let tableBody = document.getElementById('orderTable');
                let existingRow = Array.from(tableBody.rows).find(row => row.cells[7].innerText.includes(item.iteM_CODE) && parseInt(row.cells[3].querySelector('input')?.value) === parseInt(item.rate) && parseInt(row.cells[18].querySelector('input')?.value) != 1 && row.cells[13].innerText.includes(item.dT_CODE));
                var Barcode = '';
                //  //  
                if (existingRow) {
                    let quantityInput = existingRow.querySelector('.quantity-input');
                    let newQuantity = parseInt(quantityInput.value) + 1;
                    quantityInput.value = newQuantity;

                    if (item.stockstatus == 'Y') {
                        if (newQuantity <= item.stockqty) {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === Barcode) {
                                    tableRow.querySelector('.stock').textContent = '';
                                }
                            });
                        }
                        else {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === Barcode) {
                                    tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                                }
                            });
                        }
                    }

                } else {
                    let newRow = tableBody.insertRow();
                    newRow.innerHTML = `
                                  <tr>
                                        <td class="itemname">${item.iteM_NAME}</td>
                                        <td style="display: none;"><input type="hidden" class="tax" value="${item.tax}"></td>
                                        <td><input type="number" class="quantity-input" value="${item.qty}" data-price="${item.rate}"></td>
                                        <td><input type="number" class="total-price" value="${item.rate}" style="font-size: 11px; font-weight: bold; width: 85px;"></td>
                                        <td style="display: none;"><input type="hidden" class="unit" data-price="${item.unit}" value="${item.uniT_NAME}"></td>
                                        <td><input type="number" class="discountpercent" value="${item.disc}" style="font-size: 11px; font-weight: bold; width: 50px;"></td>
                                        <td style="display: none;"><input type="hidden" class="discountvalue" value="${item.disC_AMT}"></td>
                                        <td style="display: none;"><input type="hidden" class="SelectedItemId" value="${item.iteM_CODE}">${item.iteM_CODE}</td>
                                        <td style="display: none;"><input type="hidden" class="BarcodeId" value="${item.iteM_CODE}">${item.iteM_CODE}</td>
                                        <td  class="icons" style="display:flex;">
                                           <div class="tablecheckbox">
                                            <i id="tablechk" class="fas fa-sign-out-alt row-checkbox tablechk
                                               ${rights ? rights.iReturn === 1 ? 'disabled-btn' : '' : ''}">
                                            </i>
                                            </div>
                                            <span class="icon edit-icon" ><i class="fas fa-edit"></i></span>
                                            <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                        </td>
                                        <td style="display: none;" colspan="2" class="Barcode">${item.barcode}</td>
                                        <td style="display: none;"><input type="hidden" id="returnitem" value="${ReturnItem}">$${ReturnItem}</td>
                                        <td style="display: none;"><input type="hidden" id="ritem" value="${item.ritem}">${item.ritem}</td>
                                        <td style="display: none;"><input type="number" class="dtcode" id="dtcode" value="${item.dT_CODE}">${item.dT_CODE}</td>
                                        <td style="display: none; colspan="3" class="stock" data-stockqty="${item.stockqty}" style="color: red; text-align: right;"></td>
                                        <td style="display: none;"><input type="hidden" id="stockstatus" value="${item.stockstatus}"></td>
                                        <td style="display: none;"><input type="hidden" id="pickid" value="${item.pickid}">${item.pickid}</td>
                                        <td style="display: none;"><input type="hidden" class="remarks" id="remarks" value="${item.remarks}"></td>
                                        <td style="display: none;"><input type="hidden" class="kotprint" id="kotprint" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="count" value="${empr_POSTransaction.rowCount}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                    </tr>
                                `;

                }
            }
            //  //  
            if (item.stockstatus == 'Y') {
                if (item.qty <= item.stockqty) {
                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                        const rowBarcode = tableRow.querySelector('.BarcodeId')?.value;
                        if (rowBarcode === Barcode) {
                            tableRow.querySelector('.stock').textContent = '';
                        }
                    });
                }
                else {
                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                        const rowBarcode = tableRow.querySelector('.BarcodeId')?.value;
                        if (rowBarcode === Barcode) {
                            tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                        }
                    });
                }
            }

            //let ritem1 = item.riteM1
        });
        //Array.from(tableBody.rows).forEach(row => {
        //    //  //  


        //    let rateInput = row.querySelector('.total-price')?.value;
        //    let returnInput = document.getElementById('rItem')?.value;
        //    //let rateInput = row.cells[3]?.querySelector('input');
        //    //let returnInput = row.cells[12]?.querySelector('input');
        //    let qtyInput = row.querySelector('.quantity-input')?.value;
        //    let dtcodeInput = row.querySelector('.dtcode')?.value;

        //    if (rateInput) {
        //        let rate = parseFloat(rateInput.value.trim());
        //        let return1 = parseFloat(returnInput?.value?.trim() || "0");
        //        let tableChkIcon = row.cells[9]?.querySelector('#tablechk');

        //        if (rate < 0 || return1 == 1) {
        //            empr_POSTransaction.alreadyreturn = 1;
        //            tableChkIcon?.click();
        //        } else {
        //            tableChkIcon?.classList.remove('clicked');
        //        }
        //    }
        //});
        Array.from(tableBody.rows).forEach(row => {
            debugger;
            let rateInput = row.querySelector('.total-price')?.value || "0";
            let returnInput = document.getElementById('rItem')?.value || "0";
            let qtyInput = row.querySelector('.quantity-input')?.value || "0";
            let dtcodeInput = row.querySelector('.dtcode')?.value || "0";
            let rate = parseFloat(rateInput.trim());
            let return1 = parseFloat(returnInput.trim());
            let ritem1 = document.getElementById('ritem1')?.value === "true";

            if (ritem1 == true) {
                empr_POSTransaction.alreadyreturn1 = ritem1;
                console.log("already return", empr_POSTransaction.alreadyreturn1);
            }
            let tableChkIcon = row.querySelector('.tablechk');

            if (rate < 0 || return1 === 1) {
                empr_POSTransaction.alreadyreturn = 1;
                if (tableChkIcon) {
                    tableChkIcon.classList.add("active");

                    tableChkIcon.click();

                    tableChkIcon.style.pointerEvents = "none";
                    tableChkIcon.style.opacity = "0.5";
                    tableChkIcon.style.cursor = "not-allowed";
                }
            } else {
                if (tableChkIcon) {
                    tableChkIcon.classList.remove("active");

                    tableChkIcon.style.pointerEvents = "";
                    tableChkIcon.style.opacity = "";
                    tableChkIcon.style.cursor = "";
                }
            }
        });
         
        if (empr_POSTransaction.return == false)
            empr_POSTransaction.updateSummary(null);

        empr_POSTransaction.SetUserRights();
    },

    GetSubmitedData() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSRecords', function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.Users = data.data;
            }

        }, false, true);
    },

    GetCustomerName() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSCustomerName', function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.CustomerName = data.data;
            }

        }, false, true);
    },

    loadItemsForData(itemId, Barcode, BarcodeNum, barQuantity, barDisc, barSize) {

        if (Barcode != 0) {
            itemId = Barcode
        }
        var TranId = $('#TRANS_ID').val();
        ajaxHelper.ajaxGetJson('/POSTransactions/GetItemsMasterByCode?itemId=' + itemId + '&barcode=' + BarcodeNum + '&Qty=' + barQuantity + '&TranId=' + TranId, function (newData) {
            if (newData != null) {
                var rights = empr_POSTransaction.userRights;
                var rowindex;
                var quantity = barQuantity;
                const tableDataArray = [];
                let tableBody;
                var ReturnItem = 0;

                if (empr_POSTransaction.return == true) {
                    ReturnItem = 1
                }
                //  
                console.log("datattaaaa", newData);

                newData.forEach(function (item) {
                    empr_POSTransaction.rowCount++;
                    $('#DT_CODE').val(item.dT_CODE);
                    tableBody = document.getElementById('orderTable');
                    let barcodeExistRow;
                    //  //  
                    let existingRow = Array.from(tableBody.rows).find(row => {
                        if (Barcode == 0) {
                            return (
                                parseInt(row.cells[7]?.innerText || 0) === item.iteM_CODE &&
                                parseInt(row.cells[13]?.querySelector('input')?.value || 0) === 0 &&
                                parseInt(row.cells[3]?.querySelector('input')?.value || 0) === parseInt(item.salE_RATE || 0) &&
                                parseInt(row.cells[18]?.querySelector('input')?.value || 0) !== 1
                            );
                        } else {
                            return (
                                parseInt(row.cells[8]?.innerText || 0) === Barcode &&
                                parseInt(row.cells[13]?.querySelector('input')?.value || 0) === 0 &&
                                row.cells[0]?.innerText !== '' &&
                                parseInt(row.cells[3]?.querySelector('input')?.value || 0) === parseInt(item.salE_RATE || 0) &&
                                parseInt(row.cells[18]?.querySelector('input')?.value || 0) !== 1
                            );
                        }
                    });

                    if (Barcode != 0) {
                        barcodeExistRow = Array.from(tableBody.rows).find(row => {
                            return (
                                parseInt(row.cells[8]?.innerText || 0) === Barcode &&
                                parseInt(row.cells[13]?.querySelector('input')?.value || 0) === 0 &&
                                parseInt(row.cells[3]?.querySelector('input')?.value || 0) === parseInt(item.salE_RATE || 0)
                            );
                        });
                    }


                    if (existingRow) {

                        if (Barcode == 0) {
                            let quantityInput = existingRow.querySelector('.quantity-input');
                            let newQuantity = parseInt(quantityInput.value) + quantity;
                            quantityInput.value = newQuantity;
                            tableBody.insertBefore(existingRow, tableBody.rows[0]);
                            if (item.stockstatus == 'Y') {
                                if (newQuantity <= item.stockqty) {
                                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                        const rowBarcode = parseInt(tableRow.querySelector('.BarcodeId').value);
                                        if (rowBarcode === Barcode) {
                                            tableRow.querySelector('.stock').textContent = '';
                                        }
                                    });
                                }
                                else {
                                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                        const rowBarcode = parseInt(tableRow.querySelector('.BarcodeId').value);
                                        if (rowBarcode === Barcode) {
                                            tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                                        }
                                    });
                                }
                            }
                        }
                        else {
                            let quantityInput = existingRow.querySelector('.quantity-input');
                            let stockInput = $(existingRow).find('.stock').data('stockqty');
                            let newQuantity = parseInt(quantityInput.value) + quantity;
                            if (stockInput > newQuantity || item.stockstatus == 'Y')
                                quantityInput.value = newQuantity;
                            else
                                empr_helper.notify('More This Item Not available In Your Stock', 2);
                            tableBody.insertBefore(barcodeExistRow, tableBody.rows[0]);
                            tableBody.insertBefore(existingRow, tableBody.rows[1]);
                            //  //  
                            if (item.stockstatus == 'Y') {
                                if (newQuantity <= item.stockqty) {
                                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                        const rowBarcode = parseInt(tableRow.querySelector('.BarcodeId').value);
                                        if (rowBarcode === Barcode) {
                                            tableRow.querySelector('.stock').textContent = '';
                                        }
                                    });
                                }
                                else {
                                    document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                        const rowBarcode = parseInt(tableRow.querySelector('.BarcodeId').value);
                                        if (rowBarcode === Barcode) {
                                            tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                                        }
                                    });
                                }
                            }
                        }

                    } else {
                        //  //  
                        if (BarcodeNum != 0) {
                            //  //  
                            //// 1. Calculations
                            let rate = parseFloat(item.salE_RATE) || 0;
                            // barDisc check karein agar maujood ho
                            let discPer = parseFloat(barDisc == undefined ? item.disc : barDisc) || 0;
                            //let taxPer = parseFloat(item.tax) || 0;
                            let taxPer = item.tax;
                            let amt = quantity * rate;
                            let discVal = Math.abs((discPer / 100) * amt);
                            let taxAmt = (taxPer / 100) * (amt - discVal);
                            let netAmt = (amt - discVal) + taxAmt;
                            let stockColor = item.stockqty <= 0 ? 'red' : 'green';
                            let stkstatus = item.stockqty <= 0 ? 'OUT OF STOCK' : 'IN STOCK';

                            let barcodeRow = tableBody.insertRow(0);
                            barcodeRow.innerHTML = `
                                        <td class="itemname" style=" font-size: 11px;">
                                            <strong>${item.iteM_NAME}</strong><br>
                                           
                                        </td>
                                       
                                        <td>
                                         <small>${item.barcode}</small>
                                            <input type="hidden" class="BarcodeId" value="${Barcode}">
                                        </td>
                                        <td style="display:none;">
                                            <input type="hidden" class="SelectedItemId" value="${item.iteM_CODE}">
                                            <input type="hidden" class="stock" data-stockqty="${item.stockqty}" value="${item.stock}">
                                            <input type="hidden" class="stkqty" value="${item.stockqty}">
                                        </td>

                                        <td>
                                            <input type="number" class="quantity-input text-center w-100" value="${quantity}" >
                                        </td>
                                         
                                         <td>
                                            <strong>${item.size}</strong><br>
                                            <input type="hidden" class="selectedSize" value="${item.sizE_CODE}">
                                        </td>
                                         <td>
                                            <strong>${item.color}</strong><br>
                                            <input type="hidden" class="selectedColor" value="${item.coloR_CODE}">
                                        </td>
                                        <td>
                                            <input type="number" class="total-price text-center w-100" value="${rate}"  style=" border:none;" >
                                        </td> 
                                          <td>
                                            <input type="number" class="amt-input  text-center w-100" value="${Math.round(amt)}" style=" border:none;" readonly>
                                        </td>
                                        <td>
                                          <input type="number" class="discountpercent text-center w-100"
                                                 value="${discPer}" 
                                                 style="display: block; border:none; margin-bottom: 5px;">
                                          <input type="number" class="discountvalue text-center w-100" 
                                                 value="${discVal.toFixed(2)}" 
                                                 style="display: block; border:none;" >
                                        </td>

                                        <td>
                                          <input type="number" class="tax text-center w-100"
                                                 value="${item.tax}" readonly 
                                                 style="display: block; border:none; margin-bottom: 5px;">
                                          <input type="number" class="tax-amount text-center w-100" 
                                                 value="${taxAmt.toFixed(2)}" readonly
                                                 style="display: block; border:none;" >
                                            
                                        </td>
                                        <td style="position: relative; padding-top: 20px !important;">
                                            <div style="position: absolute; top: 0; left: 0; width: 100%; background: #ffffff;
                                                        color: ${stockColor}; font-size: 9px; padding: 1px 5px; 
                                                        text-align: right; border-bottom: 1px solid #ddd; font-weight: 600;">
                                               ${item.stock} 
                                            </div>
                                            <input type="number" class="net-amount text-center w-100" value="${Math.round(netAmt)}" style="font-weight:bold; border:none;" readonly>
                                        </td>

                                        <td class="icons" style="position: relative; padding-top: 20px !important;">
                                            <div style="position: absolute; top: 0; left: 0; width: 100%; background: #ffffff;border:none; 
                                                        color: ${stockColor}; font-size: 9px; padding: 1px 5px; 
                                                        text-align: left; border-bottom: 1px solid #ddd; font-weight: 600;">
                                                        ${stkstatus}
                                            </div>
                                            <div class="tablecheckbox d-flex gap-3 align-items-center justify-content-center">
                                                <i id="tablechk" class="fas ms-5 fa-sign-out-alt row-checkbox"></i>
                                                <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                            </div>
                                        </td>

                                        <td style="display: none;"><input type="hidden" id="returnitem" value="0">0</td>
                                        <td style="display: none;"><input type="hidden" id="ritem" value="0">0</td>
                                        <td style="display:none;"><input type="number" class="dtcode" id="dtcode" value="${item.dT_CODE ?? 0}">${item.dT_CODE ?? 0}</td>
                                        <td style="display: none;"><input type="hidden" id="pickid" value="0">0</td>
                                     <td style="display: none;"><input type="hidden" class="remarks" id="remarks" value=""></td>
                                        <td style="display: none;"><input type="hidden" class="kotprint" id="kotprint" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="count" value="${empr_POSTransaction.rowCount}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                        <td style="display: none;"><input class="stockstatus" type="hidden" id="stockstatus" value="${item.stockstatus}">${item.stockstatus}</td>


                                         <td style="display: none;"><input type="hidden" id="returnitem" value="${ReturnItem}">$${ReturnItem}</td>
                                        <td style="display: none;"><input type="hidden" id="ritem" value="${item.ritem}">${item.ritem}</td>
                                    `;
                        }
                        else {
                            rowindex = 0;
                            let newRow = tableBody.insertRow(rowindex);
                            newRow.innerHTML = `
                                  <tr>
                                        <td class="itemname" style="width: 100px;" >
                                          ${item.iteM_NAME} 
                                        </td>
                                        <td style="display: none;"><input type="hidden" class="tax" value="${item.tax}"></td>
                                        <td><input type="number" class="quantity-input" value="${quantity}" data-price="${item.salE_RATE}" ${item.dT_CODE != 0 && item.dT_CODE != null && item.dT_CODE != undefined ? 'readonly' : ''}></td>
                                        <td><input type="number" class="total-price" value="${item.salE_RATE}" style="font-size: 11px; font-weight: bold; width: 85px;"></td>
                                        <td style="display: none;"><input type="hidden" class="unit" data-price="${item.unitid}" value="${item.unit}"></td>
                                        <td><input type="number" class="discountpercent" value="${item.disc}" style="font-size: 11px; font-weight: bold; width: 50px;"></td>
                                        <td style="display: none;"><input type="hidden" class="discountvalue" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="SelectedItemId" value="${item.iteM_CODE}">${item.iteM_CODE}</td>
                                        <td style="display: none;"><input type="hidden" class="BarcodeId" value="${Barcode}">${Barcode}</td>
                                        <td class="icons" style="display:flex;">
                                            <div class="tablecheckbox">
                                            <i id="tablechk" class="fas fa-sign-out-alt row-checkbox tablechk
                                               ${rights ? rights.iReturn === 1 ? 'disabled-btn' : '' : ''}">
                                            </i>
                                            </div>
                                            <span class="icon edit-icon" ><i class="fas fa-edit"></i></span>
                                            <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                        </td>
                                        <td style="display: none;" colspan="2" class="Barcode">${item.barcode}</td>
                                        <td style="display: none;" colspan="3" class="stock" style="color: red; text-align: right;">${item.stock}</td>
                                        <td style="display: none;"><input type="hidden" id="stockstatus" value="${item.stockstatus}">${item.stockstatus}</td>
                                        <td style="display: none;"><input type="hidden" id="returnitem" value="${ReturnItem}">$${ReturnItem}</td>
                                        <td style="display: none;"><input type="hidden" id="ritem" value="${item.ritem}">${item.ritem}</td>
                                        <td style="display:none;"><input type="number" class="dtcode" id="dtcode" value="${item.dT_CODE ?? 0}">${item.dT_CODE ?? 0}</td>
                                        <td style="display: none;"><input type="hidden" id="pickid" value="0">0</td>
                                        <td style="display: none;"><input type="hidden" class="remarks" id="remarks" value=""></td>
                                        <td style="display: none;"><input type="hidden" class="kotprint" id="kotprint" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="count" value="${empr_POSTransaction.rowCount}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountcount" value="0"></td>
                                    </tr>
                                `;
                        }
                        if (item.stockstatus == 'Y') {
                            if (quantity <= item.stockqty) {
                                document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                    const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                    if (rowBarcode === Barcode) {
                                        tableRow.querySelector('.stock').textContent = '';
                                    }
                                });
                            }
                            else {
                                document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                    const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                    if (rowBarcode === Barcode) {
                                        tableRow.querySelector('.stock').textContent = `HAS ONLY ${item.stockqty} IN STOCK`;
                                    }
                                });
                            }
                        }
                    }
                });
                //  //  
                //if (empr_POSTransaction.rateLock === 1) {
                //    $('.total-price').prop('readonly', true);
                //}
                empr_POSTransaction.SetUserRights();
                empr_POSTransaction.updateSummary();
                empr_POSTransaction.PaymentSetting();

            } else {
                console.log("No data found for the given itemId.");
            }
        });
    },

    ResetForm(code) {
        empr_POSTransaction.Tablesvisible(code);
        empr_POSTransaction.GetUserRights();
        empr_POSTransaction.SetUserRights();
        empr_POSTransaction.kotDuplicate = 0;
        empr_POSTransaction.SER_CHARGES = 0;
        empr_POSTransaction.totalPoints = 0;
        empr_POSTransaction.CardDisc = 0;
        empr_POSTransaction.alreadyreturn = 0;
        empr_POSTransaction.dataLoaded = false;
        empr_POSTransaction.KotMsg = 0;
        empr_POSTransaction.selectedItems = [];
        empr_POSTransaction.rowCount = 0;
        empr_POSTransaction.InvoiceDate = "";
        empr_POSTransaction.maxPoints = 0;
        empr_POSTransaction.totalPayment = 0;
        empr_POSTransaction.pointStartValue = 0;
        empr_POSTransaction.payType = '';
        empr_POSTransaction.returnCash = 0;
        $('#creditOption').prop('disabled', true)
        $('.partyradioBtn input[type="radio"]').prop('disabled', true);
        $("#isBillDiscPercent").prop("disabled", false);
        $("#billDisc").prop("readonly", false);
        $(".points-section").css("display", "none");
        $('#orderTable').empty();
        this.billStatus = "";
        this.return = false;
        $("#PARTY_BACCOUNTS").hide();
        $("#party_account").hide();
        $('#BILL_STATUS').val('');
        $('#cashremark').val('');
        $('#billDisc').val('');

        $('#TRANS_ID').val(0);
        $('#settlementInput').val('');
        $('#serviceChargesValue').val(0);
        $('#serviceChargesPercent').val(0);
        $('#recvAmount').val('');
        $('#BACCOUNT').dxSelectBox('option', 'readOnly', false);
        $('#BACCOUNTS2').dxSelectBox('option', 'readOnly', false);
        $(".quantity-input").prop("readonly", false);
        $(".total-price").prop("readonly", false);
        $(".discountpercent").prop("readonly", false);
        $(".delete-icon").prop("readonly", true);
        $("#slider").attr("max", 0).val(0);
        $("#prevPoints").text(0);
        $("#currentPoints").text(0);
        $('#advAmountbank').val('');
        $('#BILL_MODE').val('');
        $('#CardDisc').val('');
        $('#INV_STATUS').val('');
        $('#IsKot').val(0);
        $('#complete').val(0);
        $('#TRANS_ID').val(0);
        $('#barcodevisible').val('');
        $('#fnlBankAmount').val('');
        $('#advBankAmount').val('');
        $('#barcode').val('');
        $('#cashReturn').val(0);
        $('#qrCodevisible').val('');
        /*$('#BILL_STATUS').val('');*/
        $('#billDelPercent').val(0);
        $('#billDelValue').val(0);
        $('#billDiscPercent').val(0);
        $('#billDiscValue').val(0);
        $('#CNAME').val('');
        $('#CMOB').val('');
        $('#CADD').val('');
        $('#VOUCHER_NO').text('POS/000000');
        /*$('#TRANS_ID').val(0);*/
        $('#Des').val('');
        $('#expamt').val('');
        $('#expamt').val('');
        $('#Dec').val('');
        $('.headers .icondiv').css({
            'background-color': '#055a87'
        });
        $('.quantity-input').prop('readonly', false);
        $('.total-price').prop('readonly', false);
        $('#advBankAmount').prop('readonly', false);
        $('#fnlAmount').prop('readonly', false);
        $('#fnlBankAmount').prop('readonly', false);
        $('#discountPercentage').prop('disabled', false);
        $('#discountPerUnit').prop('disabled', false);
        $('#itemQuantity').prop('disabled', false);
        $('#CardDisc').prop('readonly', false);
        $('#billDiscPercent').prop('readonly', false);
        $('#billDiscValue').prop('readonly', false);
        $('#srbName').val('');
        $('#srbNtn').val('');
        $('#posUser').val('');
        $('#posPass').val('');
        $('#srbId').val('');
        $('#TotalAmtWithTax').val('');
        $('#BILL_STATUS').val() == 'P' ? $('#srbStatus').val('') : $('#srbStatus').val();
        $('#MD_ID').val('');
        $('#Salesmanhidden').val('');
        $('#SACTCODE').val('');
        $('.search-bar').val('');
        const boxes = document.querySelectorAll('.middle .box');
        boxes.forEach(box => box.style.display = 'flex');
        $('#3.icondiv').trigger('click');
        $("#dineInInputContainer").hide();
        empr_POSTransaction.updateSummary();
        empr_POSTransaction.TodayDate();
        //$('#BACCOUNT').dxSelectBox('instance').option('value', '');
        //$('#BACCOUNTS').dxSelectBox('instance').option('value', '');
        //$('#BACCOUNTS2').dxSelectBox('instance').option('value', '');
        $("#SalesmanName").dxSelectBox("instance").option("value", '');
        $('#Salesman').val('');
        $('#Username').val('');
        $('#Salesmanhidden').val('');
        $('#SACTCODE').val('');
        $('#COMM').val('');
        $('#waiterName').val('');
        $('#tableNum').val('');
        $('#advAmount').val('');
        $('#cashAmount').val('');
        $('#fnlAmount').val('');
        $('#SRBInvoiceId').val('');
        $('#VOUCHERNO').val('POS/000000');
        $('#Tables').text('');
        $('#Waiter').text('');
        $('#complete').val(0);
        /*$('.close').click();*/
        $('#paymentModal').modal('hide');
        $('#advAmount').css('border', '');
        $('#completecheck').prop({
            'checked': false,
            'readonly': false,
            'disabled': false
        }).closest('.form-check').removeClass('disabled');
        $('#delDate').prop('readonly', false);
        $('#delDate').val(new Date().toISOString().split('T')[0]);
        $('#advAmount').prop('readonly', false);
        empr_POSTransaction.billMode = '';
        setTimeout(() => {
            const gridElement = document.getElementById('gridContainer');
            const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);
            if (gridInstance) {
                gridInstance.dispose();
            } else {
                console.log("DataGrid instance not found");
            }
        }, 0);
        //$('#ExpenseType').dxSelectBox("instance").option("value", '');
        empr_POSTransaction.selectedTable = 0;
        empr_POSTransaction.selectedWaiter = 0;
        $("#partyName").dxSelectBox("instance").option("disabled", false);
        $('#partyName2').dxSelectBox('option', 'readOnly', false);
        $('#cashAmount2').prop('readonly', false);
        $('#bankrecv2').prop('readonly', false);
        $('#partyrecv2').prop('readonly', false);
        $('#partyrecv').prop('readonly', false);
        $('#cashAmount').prop('readonly', false);
        $('#btnAdvance').prop('disabled', false);
        $('#btnCash').prop('disabled', false);
        $('#btnCard').prop('disabled', false);
        $('#btnParty').prop('disabled', false);
        $('#btnSplit').prop('disabled', false);
        $('#cashOption').prop('disabled', false);
        $('#cardOption').prop('disabled', false);
        $('#creditOption').prop('disabled', false);
        $('#btnSplit').css('display', '');
        $('#cashAmount2').val('')
        $('#cashAmount').val('')
        $('#advAmount').val('')
        $('#bankrecv2').val('')
        $('#partyrecv2').val('')
        $('#bankrecv').val('')
        $('#partyrecv').val('')
        $('#partyhidden2').val('')
        $('#partyhidden').val('')
        setTimeout(function () {
            empr_POSTransaction.GetSubmitedData();
            empr_POSTransaction.GetCustomerName();
        }, 200);
        $('#btnCash').click();
    },

    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    bindDxDdl_New: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle_New(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    InitAllBarcodePickGrid: function () {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetAllBarcodeList', function (data) {
            //  //  
            console.log('data :' + data);
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    $('#AllBarcodePickModal').modal('show');
                    empr_POSTransaction.CreateBarcodeGrid(data.data);

                } else {
                    empr_helper.notify("No barcodes found.", 2);
                }
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);

    },
    InitBarcodePickGrid: function (ItemId) {
        empr_POSTransaction.sizeGroups = {};
        empr_POSTransaction.selectedItems = [];
        ajaxHelper.ajaxGetJson('/POSTransactions/GetBarcodeList?ItemId=' + ItemId, function (data) {
            console.log(data)
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    // Group by size first
                    $('#barcodeComplete').css('display', 'block');


                    data.data.forEach(function (item) {
                        if (!empr_POSTransaction.sizeGroups[item.size]) {
                            empr_POSTransaction.sizeGroups[item.size] = [];
                        }
                        empr_POSTransaction.sizeGroups[item.size].push(item);
                    });

                    // Create HTML for each size group
                    var html = '';
                    Object.keys(empr_POSTransaction.sizeGroups).forEach(function (size) {
                        var items = empr_POSTransaction.sizeGroups[size];
                        var firstItem = items[0];
                        var IsSelected = false;
                        var Isruning = false;

                        $('#orderTable tr').each(function () {

                            const row = $(this);

                            const isItemNameVisible = row.find('.itemname').css('display') !== 'none';
                            if (isItemNameVisible) {
                                var bar = row.find('.Barcode').text();
                                console.log(bar);
                                FilteredRow = items.filter(x => x.barcode == bar)
                                if (FilteredRow.length > 0 && !Isruning) {
                                    let sizeValue = FilteredRow[0].size;
                                    let size = !isNaN(sizeValue) && sizeValue !== "" ? parseInt(sizeValue) : sizeValue;
                                    html += `
                                        <div class="size-box selected" data-size="${size}">
                                            <div class="size-header">
                                                <strong>${size}</strong><br>
                                                <span class="rate">@${firstItem.srate}</span>
                                            </div>
                                        </div>
                                    `;
                                    IsSelected = true;
                                    Isruning = true;
                                }
                            }


                        });
                        if (!IsSelected) {
                            html += `
                                        <div class="size-box" data-size="${size}">
                                            <div class="size-header">
                                                <strong>${size}</strong><br>
                                                <span class="rate">@${firstItem.srate}</span>
                                            </div>
                                        </div>
                                    `;
                        }

                    });

                    $('.barcode-container').html(html);
                    $('#BarcodePickModal').modal('show');


                } else {
                    empr_helper.notify("No barcodes found.", 2);
                }
            } else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);

    },

    CreateBarcodeGrid: function (dataSrc) {
        dataSrc.forEach(function (row) {

            var qty = parseFloat(row.quantity) || 0;
            var rate = parseFloat(row.srate) || 0;
            var disc = parseFloat(row.disc) || 0;

            // Amount
            row.amt = (qty * rate).toFixed(2);

            // Discount Amount
            row.disC_AMT = ((row.amt * disc) / 100).toFixed(2);

            // Net Amount
            row.neT_AMT = (row.amt - row.disC_AMT).toFixed(2);

        });
        var col = [
            { dataField: 'iteM_CODE', caption: 'Item Code', allowEditing: false, visible: false },
            { dataField: 'iteM_NAME', caption: 'Item Name', allowEditing: false, visible: true },
            { dataField: 'barcode', caption: 'Barcode', allowEditing: false },
            { dataField: 'color', caption: 'Color', allowEditing: false },
            { dataField: 'size', caption: 'Size', allowEditing: false },
            {
                dataField: 'quantity', caption: 'Quantity', allowEditing: true, width: 80,
                setCellValue: function (newData, value, currentRowData) {
                    newData.quantity = value;
                    var qty = parseFloat(value) || 0;
                    var rate = parseFloat(currentRowData.srate) || 0;
                    if (!isNaN(qty) && !isNaN(rate)) {
                        newData.amt = (qty * rate).toFixed(2);
                    } else {
                        newData.amt = 0;
                    }
                    var disc = parseFloat(currentRowData.disc) || 0;
                    newData.disC_AMT = ((newData.amt * disc) / 100).toFixed(2);
                    newData.neT_AMT = (newData.amt - newData.disC_AMT).toFixed(2);
                }
            },
            {
                dataField: 'srate', caption: 'Rate', allowEditing: true, width: 70,
                setCellValue: function (newData, value, currentRowData) {
                    newData.srate = value;
                    var rate = parseFloat(newData.srate) || 0;
                    var qty = parseFloat(currentRowData.quantity) || 0;
                    if (!isNaN(qty) && !isNaN(rate)) {
                        newData.amt = (qty * rate).toFixed(2);
                    } else {
                        newData.amt = 0;
                    }
                    var disc = parseFloat(currentRowData.disc) || 0;
                    newData.disC_AMT = ((newData.amt * disc) / 100).toFixed(2);
                    newData.neT_AMT = (newData.amt - newData.disC_AMT).toFixed(2);
                }
            },
            {
                dataField: 'amt', caption: 'Amount', allowEditing: false, width: 80
            },
            {
                dataField: 'disc', caption: 'Disc %', allowEditing: true, width: 70,
                setCellValue: function (newData, value, currentRowData) {
                    newData.disc = value;
                    var Amount = parseFloat(currentRowData.amt) || 0;
                    var Discount = parseFloat(newData.disc) || 0;

                    if (!isNaN(Amount) && !isNaN(Discount)) {
                        newData.disC_AMT = (Amount * Discount / 100).toFixed(2);
                    } else {
                        newData.disC_AMT = 0;
                    }
                    var DiscountAmount = parseFloat(newData.disC_AMT) || 0;
                    var NetAmount = Amount - DiscountAmount || 0;
                    if (!isNaN(NetAmount)) {
                        newData.neT_AMT = (NetAmount).toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }
            },
            {
                dataField: 'disC_AMT', caption: 'Disc Amt', allowEditing: true, width: 80,
                setCellValue: function (newData, value, currentRowData) {
                    newData.disC_AMT = value;
                    var Amount = parseFloat(currentRowData.amt) || 0;
                    var DiscountAmt = parseFloat(newData.disC_AMT) || 0;

                    if (!isNaN(Amount) && !isNaN(DiscountAmt)) {
                        newData.disc = ((DiscountAmt / Amount) * 100).toFixed(2);
                    } else {
                        newData.disc = 0;
                    }

                    var DiscountAmount = parseFloat(newData.disC_AMT) || 0;
                    var NetAmount = Amount - DiscountAmount || 0;
                    if (!isNaN(NetAmount)) {
                        newData.neT_AMT = (NetAmount).toFixed(2);
                    } else {
                        newData.neT_AMT = 0;
                    }
                }

            },
            { dataField: 'neT_AMT', caption: 'Net Amount', allowEditing: false, width: 80 },
        ];
        empr_helper.editableDxGridbinding('#BarcodePickGridContainer', col, dataSrc, "POSBarcodes");
        //empr_helper.dxGridbinding('#BarcodePickGridContainer', col, dataSrc, "POS");
        setTimeout(function () {
            $('#BarcodePickGridContainer').dxDataGrid('instance').resize();
        }, 500);
    },

    CreateGrid(dataSrc) {

        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE); // Convert string to Date object
        });

        var today = new Date();


        if (dataSrc.length > 0) {
            empr_POSTransaction.rowsCount = dataSrc.length - 1;
        }
        var Caption = "";
        if (Type == "I") {
            Caption = "Item";
        }
        else {
            Caption = "Bar Code";
        }
        var col = [
            {
                dataField: "Action",
                width: 120,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    if (Permissions != "Admin") {
                        const copyAction = !Permissions.r_COPY
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Clone" onclick="empr_POSTransaction.CloneRow(${options.rowIndex})" title="Duplicate"><i class="fa fa-clone"></i></a>`;
                        const addAction = (!Permissions.r_ADD && !Permissions.r_EDIT)
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_POSTransaction.AddRow()" title="Add"><i class="fa fa-add"></i></a>`;
                        const deleteAction = !Permissions.r_DLT
                            ? ''
                            : `<a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_POSTransaction.DeleteRow(${options.rowIndex})" title="Delete"><i class="fa fa-trash"></i></a>`;
                        const actions = `<div class="btn-group btn-group-sm">${copyAction}${addAction}${deleteAction}</div>`;
                        $(actions).appendTo(container);
                    } else {
                        $(`<div class="btn-group btn-group-sm">
                           <a href="javascript:;" class="grid-action-icon Clone" onclick="empr_POSTransaction.CloneRow(`+ options.rowIndex + `)" title="Duplicate"><i class="fa fa-clone"></i></a>
                           <a href="javascript:;" class="grid-action-icon Add" style="margin-left: 8px" onclick="empr_POSTransaction.AddRow()" title="Add"><i class="fa fa-add"></i></a>
                           <a href="javascript:;" class="grid-action-icon Delete" style="margin-left: 8px" onclick="empr_POSTransaction.DeleteRow(`+ options.rowIndex + `)" title="Delete"><i class="fa fa-trash"></i></a>
                           </div>`).appendTo(container);
                    }
                }
            },
            {
                dataField: 'dT_CODE',
                caption: 'Code',
                visible: false,
            },
            {
                dataField: 'iteM_CODE',
                caption: Caption,
                width: 150,
                allowSorting: false,
                lookup: {
                    dataSource: {
                        store: Barcodes,
                        paginate: true,
                        pageSize: 50
                    },
                    displayExpr: 'value',
                    valueExpr: 'key',
                    searchEnabled: true,
                    showClearButton: true,
                    paging: {
                        enabled: true,
                        pageSize: 50,
                    }
                },
                setCellValue: function (newData, value, currentRowData) {

                    newData.iteM_CODE = value;
                    if (Type != "I") {
                        if (value != '') {
                            var selectedItem = Barcodes.filter(u => u.key == value);
                            if (selectedItem.length > 0) {
                                newData.rate = selectedItem[0].rate;
                            }
                        }
                        else {
                            newData.rate = 0;
                        }
                    }
                }
            },
            {
                dataField: 'qty',
                caption: 'Quantity',
                allowEditing: true,
                setCellValue: function (newData, value, currentRowData) {

                    if (value != '') {
                        newData.qty = value;
                        var qty = parseFloat(newData.qty) || 0;
                        var rate = parseFloat(currentRowData.rate) || 0;
                        newData.amt = qty * rate;
                        if (qty == 1) {
                            newData.chK1 = true;
                            var gridInstance = $('#detailContainer').dxDataGrid('instance');
                            gridInstance.columnOption('qty', 'allowEditing', false);
                        }
                    }
                    else {
                        newData.unit = 0;
                        newData.rate = 0;
                    }

                    empr_POSTransaction.Calculate();
                },

            },
            {
                dataField: 'unit',
                caption: 'Unit',
                visible: false,
            },
            {
                dataField: 'rate',
                caption: 'Rate',
                allowEditing: false,
            },
            {
                dataField: 'amt',
                caption: 'Amount',
                allowEditing: false,
            },
            {
                dataField: 'disc',
                caption: 'Disc %',
                setCellValue: function (newData, value, currentRowData) {

                    if (value != '') {
                        newData.disc = value;
                        var per = parseFloat(value) || 0;
                        var amt = parseFloat(currentRowData.amt) || 0;
                        newData.neT_AMT = amt - ((amt * per) / 100);
                    }
                    else {
                        newData.unit = 0;
                        newData.rate = 0;
                    }

                    empr_POSTransaction.Calculate();
                },
            },
            {
                dataField: 'neT_AMT',
                caption: 'Net AMT',
                allowEditing: false,
            },
            {
                dataField: 'ritem',
                caption: 'Return',
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var $cell = $("<div>").addClass("custom-cell");
                    var isChecked = options.data.riteM1;
                    var $checkbox = $("<input type='checkbox'>")
                        .prop('checked', isChecked)
                        .on('change', function () {
                            var item = options.data;
                            if (this.checked) {
                                item.ritem = "1";
                            }
                            else {
                                item.ritem = "0";
                            }
                            item.riteM1 = this.checked;

                            var neT_AMT = parseFloat(item.neT_AMT) || 0;
                            item.neT_AMT = -neT_AMT;

                            var amt = parseFloat(item.amt) || 0;
                            item.amt = -amt;

                            var qty = parseFloat(item.qty) || 0;
                            item.qty = -qty;

                            var gridInstance = $('#detailContainer').dxDataGrid('instance');
                            var dataSource = gridInstance.option("dataSource");
                            dataSource[options.rowIndex] = item;
                            gridInstance.option("dataSource", dataSource);
                        });

                    $cell.append($checkbox);
                    container.append($cell);
                    empr_POSTransaction.Calculate();
                    //empr_POSTransaction.CalculateCashBack();
                }
            },
            {
                dataField: 'deL_DATE',
                caption: 'Del Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                visible: false,
                showInColumnChooser: false
            },
        ];
        empr_POSTransaction.editableDxGridbindingForTransactions('#detailContainer', col, dataSrc, "POSTransaction");
        if (dataSrc.length == 0) {
            $('#detailContainer').dxDataGrid('instance').addRow().done(function () {
                $('#detailContainer').dxDataGrid('instance').saveEditData();
            });
        }
    },

    CloneRow(index) {

        if ($('#detailContainer').dxDataGrid('instance').hasEditData()) {
            $('#detailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_POSTransaction.rowsCount += 1;
                const gridInstance = $('#detailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    //let clonedRowData = dataSource[index];
                    let clonedRowData = $.extend(true, {}, dataSource[index]);

                    if (clonedRowData.hasOwnProperty('dT_CODE')) {
                        delete clonedRowData.dT_CODE;
                    }
                    //delete clonedRowData.dT_CODE;
                    clonedRowData.__KEY__ = empr_POSTransaction.GenerateKey(36);
                    let newDataSource = [clonedRowData].concat(dataSource);
                    //delete newDataSource[0].dT_CODE;
                    gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                    gridInstance.refresh(); // Refresh the grid
                }
            });
        }
        else {
            empr_POSTransaction.rowsCount += 1;
            const gridInstance = $('#detailContainer').dxDataGrid('instance');
            var dataSource = gridInstance.option("dataSource");
            if (dataSource.length > 0) {
                //let clonedRowData = dataSource[index];
                let clonedRowData = $.extend(true, {}, dataSource[index]);

                if (clonedRowData.hasOwnProperty('dT_CODE')) {
                    delete clonedRowData.dT_CODE;
                }
                clonedRowData.__KEY__ = empr_POSTransaction.GenerateKey(36);
                //delete clonedRowData.dT_CODE;
                let newDataSource = [clonedRowData].concat(dataSource);
                //delete newDataSource[0].dT_CODE;
                gridInstance.option("dataSource", newDataSource); // Update the grid's dataSource
                gridInstance.refresh(); // Refresh the grid
            }
        }
    },

    AddRow() {

        if ($('#detailContainer').dxDataGrid('instance').hasEditData()) {
            $('#detailContainer').dxDataGrid('instance').saveEditData().done(function () {
                empr_POSTransaction.rowsCount += 1;
                const gridInstance = $('#detailContainer').dxDataGrid('instance');
                const dataSource = gridInstance.option("dataSource");
                dataSource.unshift({ __KEY__: empr_POSTransaction.GenerateKey(36), ritem: false, chK1: true, qty: 1 });
                gridInstance.option("dataSource", dataSource);
                gridInstance.refresh();
            });
        }
        else {
            empr_POSTransaction.rowsCount += 1;
            const gridInstance = $('#detailContainer').dxDataGrid('instance');
            const dataSource = gridInstance.option("dataSource");

            dataSource.unshift({ __KEY__: empr_POSTransaction.GenerateKey(36), ritem: false, chK1: true, qty: 1 });
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();
        }
    },

    DeleteRow(ItemId, TransId) {
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
            ajaxHelper.ajaxPostJsonData({ code: TransId, ItemId: ItemId }, "/POSTransactions/DeletePOSTransactionDetailByCode", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_POSTransaction.SaveInfo();
                }
            }, false, true);
        });

    },

    ExpenseDeleteRow(TransId) {
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
            ajaxHelper.ajaxPostJsonData({ code: TransId }, "/POSTransactions/DeleteExpenseRow", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_POSTransaction.GetExpenseRecord();
                }
            }, false, true);
        });

    },

    GenerateKey(keyLength) {

        var key = "";
        var characters = "abcdef0123456789";
        for (var i = 0; i < keyLength; i++) {
            if (i === 8 || i === 13 || i === 18 || i === 23) {
                key += "-";
            } else {
                key += characters.charAt(Math.floor(Math.random() * characters.length));
            }
        }
        return key;
    },

    InitQuickSearchGrid() {
        empr_POSTransaction.GetPOSTransactions();
    },

    InitCustomerHistoryGrid(CstNumber) {
        ajaxHelper.ajaxGetJson(`/POSTransactions/GetCustomerHistory?CstNumber=${CstNumber}`, function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
                    console.log(data.data);
                    $('#CustomerHistory').modal('show');
                    empr_POSTransaction.CreateCustomerHistoryGrid(data.data);
                }
                else {
                    empr_helper.notify("This is the customer's first purchase at our store.", 2);
                }

            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    InitPayQuickSearchGrid() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPayQuickSearch', function (data) {
            if (data.msgType == 1) {
                $('#QuickSearchModal').modal('show');
                empr_POSTransaction.CreatePayQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetPOSTransactions() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSTransactions', function (data) {
            if (data.msgType == 1) {
                $('#QuickSearchModal').modal('show');
                empr_POSTransaction.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid(dataSrc) {
        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE); // Convert string to Date object
        });
        var today = new Date();
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                if (Permissions != 'Admin') {
                    if (Permissions.r_ADD) {
                        $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;" class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
                    }
                    if (Permissions.r_ADD) {
                        if (options.data.status != 'H') {
                            $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_pay" reportid=${options.data.traN_ID} title="Pay"><i class="fa-solid fa-money-bill"></i></a>
                                   </div>`).appendTo(container);
                            if (Permissions.r_PRINT) {
                                $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_print" reportid=${options.data.traN_ID} title="Print"><i class="fa-solid fa-print"></i></a>
                                   </div>`).appendTo(container);
                            }

                        }
                    }
                }
                else {
                    $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;" class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
                    if (options.data.status != 'H') {
                        $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_pay" reportid=${options.data.traN_ID} title="Pay"><i class="fa-solid fa-money-bill"></i></a>
                                   </div>`).appendTo(container);
                        $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_print" reportid=${options.data.traN_ID} title="Print"><i class="fa-solid fa-print"></i></a>
                                   </div>`).appendTo(container);
                    }
                }

            }
        },
        { dataField: 'traN_ID', caption: 'Code', visible: false, },
        {
            dataField: 'v_DATE',
            caption: 'Date',
            dataType: 'date',
            format: 'dd-MM-yyyy',
            sortOrder: 'desc',
            sortIndex: 0,
            filterValue: today
        },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'cname', caption: 'Name', },
        { dataField: 'cmob', caption: 'Contact #', },
        { dataField: 'waiter', caption: 'Waiter', },
        { dataField: 'table', caption: 'Table', },
        { dataField: 'neT_TOTAL', caption: 'Total Value', },
        { dataField: 'bilL_STATUS', caption: 'Status', },
        { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
        { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
        { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
        { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
        { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
        { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
        { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
        { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
        { dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },

        ];
        localStorage.removeItem("POSTransactionQS");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "POSTransactionQS", "single");
    },

    CreatePayQuickSearchGrid(dataSrc) {
        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE); // Convert string to Date object
        });
        var today = new Date();
        var columns = [
            {
                dataField: "Action",
                width: 100,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {
                    if (Permissions != 'Admin') {
                        if (Permissions.r_EDIT) {
                            $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
                        }
                        if (Permissions.r_EDIT && Permissions.r_ADD) {
                            //    if (options.data.complete == "0") {
                            //        $(`<div class="btn-group btn-group-sm ">
                            //           <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_pay" reportid=${options.data.traN_ID} title="Pay"><i class="fa-solid fa-money-bill"></i></a>
                            //           </div>`).appendTo(container);
                            //    }
                        }
                        if (Permissions.r_PRINT) {
                            $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_PayPrint" reportid=${options.data.traN_ID} title="Pay"><i class="fa fa-print"></i></a>
                                   </div>`).appendTo(container);
                        }

                    }
                    else {
                        $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
                        //if (options.data.complete == "0") {
                        //    $(`<div class="btn-group btn-group-sm ">
                        //           <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_pay" reportid=${options.data.traN_ID} title="Pay"><i class="fa-solid fa-money-bill"></i></a>
                        //           </div>`).appendTo(container);
                        //}
                        $(`<div class="btn-group btn-group-sm ">
                                   <a href="javascript:;" style="margin-left: 8px;"  class="grid-action-icon elm_PayPrint" reportid=${options.data.traN_ID} title="Pay"><i class="fa fa-print"></i></a>
                                   </div>`).appendTo(container);
                    }


                }
            },
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            {
                dataField: 'v_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                sortOrder: 'desc',
                sortIndex: 0,
                filterValue: today
            },
            { dataField: 'voucheR_NO', caption: 'Voucher No', },
            { dataField: 'cname', caption: 'Name', },
            { dataField: 'cmob', caption: 'Contact #', },
            { dataField: 'waiter', caption: 'Waiter', },
            { dataField: 'table', caption: 'Table', },
            { dataField: 'billmode', caption: 'Mode', },
            { dataField: 'recv', caption: 'Total Value', },
            { dataField: 'bilL_STATUS', caption: 'Status', },
            { dataField: 'closing', caption: 'Closing Status', filterValue: 'Opened' },
            { dataField: 'remarks', caption: 'Remarks' },
            { dataField: 'adD_USER_ID', caption: 'Created By', visible: false, },
            { dataField: 'adD_DATE', caption: 'Created Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'adD_COMPUTER_NAME', caption: 'Created Computer', visible: false, },
            { dataField: 'adD_POSTALCODE', caption: 'Created Postal Code', visible: false, },
            { dataField: 'adD_IP_ADDRESS', caption: 'Created IP', visible: false, },
            { dataField: 'ediT_USER_ID', caption: 'Updated By', visible: false, },
            { dataField: 'ediT_DATE', caption: 'Updated Date', dataType: 'date', visible: false, format: 'dd-MM-yyy' },
            { dataField: 'ediT_COMPUTER_NAME', caption: 'Updated Computer', visible: false, },
            { dataField: 'ediT_IP_ADDRESS', caption: 'Updated IP', visible: false, },
            { dataField: 'ediT_POSTALCODE', caption: 'Updated Postal Code', visible: false, },

        ];
        localStorage.removeItem("POSTransactionQSP");
        empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "POSTransactionQSP", "single");
        //setTimeout(function () {
        //    empr_helper.dxGridbindingVouchers('#gridContainer', columns, dataSrc, "POSTransactionQSP", "single");
        //}, 300);
    },

    AdvanceBookingGrid(dataSrc) {
        col = [
            { dataField: 'voucherDate', caption: 'Date', dataType: 'date', format: 'dd/MM/yyyy', width: 110 },
            {
                dataField: 'voucherNo', caption: 'Transaction #', alignment: 'center', width: 130
            },
            { dataField: 'mobile', caption: 'Mobile' },
            { dataField: 'cName', caption: 'Name', width: 150 },
            { dataField: 'cAdd', caption: 'Address' },
            { dataField: 'netAmt', caption: 'Net Amount', dataType: 'number', width: 100 },
            { dataField: 'deliveryCharges', caption: 'Delivery Charges', dataType: 'number', width: 100 },
            { dataField: 'advance', caption: 'Advance', dataType: 'number', width: 100 },
            { dataField: 'total', caption: 'Remaining', dataType: 'number', width: 100 },
            { dataField: 'mode', caption: 'Mode', width: 110 }
        ];
        detailColumns = [
            { dataField: 'itemName', caption: 'Item', width: 150 },
            { dataField: 'qty', caption: 'Qty', dataType: 'number', width: 50 },
        ]
        empr_helper.MasterDetailDxGridBinding('#AdvancegridContainer', col, detailColumns, dataSrc, "Advance");
        setTimeout(function () {
            empr_helper.MasterDetailDxGridBinding('#AdvancegridContainer', col, detailColumns, dataSrc, "Advance");
        }, 300);
    },

    CreateCustomerHistoryGrid(dataSrc) {
        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE);
        });
        var columns = [
            {
                dataField: 'v_DATE',
                caption: 'Date',
                dataType: 'date',
                format: 'dd-MM-yyyy',
                sortOrder: 'desc',
                sortIndex: 0,
                width: 80
            },
            { dataField: 'voucheR_NO', caption: 'Transaction No', width: 150 },
            { dataField: 'cname', caption: 'Name', width: 150 },
            { dataField: 'cmob', caption: 'Contact No', width: 150 },
            { dataField: 'cadd', caption: 'Address', width: 300 },
            { dataField: 'recv', caption: 'Bill Amount', },

        ];
        localStorage.removeItem("POSTransactionQSP");
        empr_helper.dxGridbindingVouchers('#CustomergridContainer', columns, dataSrc, "POSTransactionQSP", "single");
    },

    ExpenseDropdown(selectedValue) {

        ajaxHelper.ajaxGetJson("/POSTransactions/GetExpenseType", function (data) {

            if (data.length === 0) {
                data = []; // Ensure data is an empty array if no data is returned
            }

            if (selectedValue == "" && data.length > 0) {
                selectedValue = data[0].key;
            }

            $('#ExpenseType').dxSelectBox({
                dataSource: data,
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
                onValueChanged: function (e) {
                    // Handle value change if needed
                },
            });
        }, false, true);



    },

    SalesmanDropdown(selectedValue) {
        ajaxHelper.ajaxGetJson("/POSTransactions/GetSalesmanName", function (data) {
            console.log("Dropdown Data:", data);
            var dataSrc;
            if (data.length > 0) {
                dataSrc = data;
            }
            else {
                dataSrc = []
            }
            empr_POSTransaction.Salesmandata = dataSrc;
            empr_POSTransaction.bindDxDdl("SalesmanName", dataSrc, selectedValue, "key", "value", "Salesman", function (d) {
                var SelectedId = d.value;
                var selectedItem = dataSrc.find(item => item.key === SelectedId);
                var selectedText = selectedItem ? selectedItem.value : "";
                var partycode = selectedItem ? selectedItem.code : "";
                $('#Salesman').val(selectedText);
                $('#Salesmanhidden').val(SelectedId);
                $('#SACTCODE').val(partycode);
                var comm = Commisions.filter(b => b.partycode == SelectedId && b.actcode == partycode);
                if (comm.length > 0) {
                    $('#COMM').val(comm[0].comm);
                }
                else {
                    $('#COMM').val('');
                }

            });

        }, false, true);
    },

    ExpenseGrid(dataSrc) {

        dataSrc.forEach(item => {
            item.v_DATE = new Date(item.v_DATE); // Convert string to Date object
        });

        var today = new Date(); // Get today's date
        var columns = [
            {
                dataField: 'v_DATE',
                caption: 'Date',
                width: 100,
                dataType: 'date',
                format: 'dd-MM-yyyy',
                sortOrder: 'desc',
                sortIndex: 0,
                filterValue: today
            },
            { dataField: 'traN_ID', caption: 'Code', visible: false, },
            { dataField: 'voucheR_NO', caption: 'Voucher No', visible: false, },
            { dataField: 'descr', caption: 'Description', },
            { dataField: 'actcode', caption: 'Account', },
            { dataField: 'amount', caption: 'Amount', },
            {
                dataField: "Action",
                width: 80,
                alignment: 'center',
                fixed: true,
                fixedPosition: "left",
                allowExporting: false,
                cellTemplate: function (container, options) {


                    $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;"  class="grid-action-icon Exp_elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   <a href="javascript:;" class="grid-action-icon Exp_Delete" style="margin-left: 8px" onclick="empr_POSTransaction.ExpenseDeleteRow(`+ options.data.traN_ID + `)" title="Delete"><i class="fa fa-trash"></i></a>
                                   </div>`).appendTo(container);
                }
            },
        ];
        //empr_helper.dxGridbindingWithoutFeatures('#ExpgridContainer', columns, dataSrc, "POSTransaction");
        localStorage.removeItem("POSTransactionExpenseGrid");
        empr_helper.dxGridbindingVouchers('#ExpgridContainer', columns, dataSrc, "POSTransactionExpenseGrid", "single");
        //grid.filter(["v_DATE", "=", today]);
    },

    GetDataToSave(MD_ID, status, kotMsg = false) {
         
        var ReturnTotal;
        var ReturnNetTotal;
        var PWINDOW = empr_POSTransaction.pwindow;

        var TRANSID = $("#TRANS_ID").val();
        var BILL_STATUS = status == "" ? 'P' : $('#BILL_STATUS').val();
        var V_DATE = $('#V_DATE').text();
        var MD_ID = MD_ID == undefined ? $('#MD_ID').val() : MD_ID;
        var VOUCHER_NO = $('#VOUCHERNO').val() || $("#VOUCHER_NO").text();
        var SRB_VN = $('#SRBVN').val();
        var CNAME = $('#CNAME').val();
        var CMOB = $('#CMOB').val();
        var CADD = $('#CADD').val();
        var INV_STATUS = BILL_STATUS == "H" ? 1 : 3;
        //var INV_STATUS = ($('#INV_STATUS').val() || "").trim() === "" ? 3 : $('#INV_STATUS').val();
        //var billDisc = parseFloat($('#billDiscValue').val()) || 0;
        //var billDisc =  0;

        //var DISC_AMT = empr_POSTransaction.totalPoints > empr_POSTransaction.totalPayment
        //    ? empr_POSTransaction.totalPayment
        //    : billDisc;
        ReturnTotal = parseFloat($('#subTotal').text().replace(/[^\d.-]/g, '')) || 0
        var SER_CHARGES = parseFloat($('#serviceChargesValue').val()) || 0;
        var DEL = $('#billDelPercent').val();
        var DEL_CHARGES = parseFloat($('#billDelValue').val()) || 0;
        ReturnNetTotal = (ReturnTotal + SER_CHARGES + DEL_CHARGES) - DISC_AMT;
        var TOTAL = ReturnTotal;
        var DISC = empr_POSTransaction.billDisc;
        var DISC_AMT = empr_POSTransaction.billDiscVal;
        //  

        var ITEM_DISCOUNT = $('#itemDiscount').text();
        //  //  
        let NET_TOTAL = $('#NetTotal').text();
        var DEL = $('#billDelPercent').val();
        var DEL_CHARGES = $('#billDelValue').val();
        var TAX_AMT = empr_POSTransaction.TaxAMT;
        var SALESMAN = $('#Salesmanhidden').val();
        var USERNAME = $('#Username').val();
        var SACT_CODE = $('#SACTCODE').val();
        var SALESMANNAME = $('#Salesman').val();
        var COMMISION = parseFloat($('#COMM').val().replace('%', '')) || 0;
        var CardDiscValue = empr_POSTransaction.CardDisc;
        var TAX = '';
        var SETT = Math.abs(parseFloat($('#settlementInput').val())) || 0;
        var SETTl = Math.abs(parseInt($('#settlementInput').val())) || 0;
        var receivedHidden = parseFloat($('#recvhidden').val()) || 0;
        var sign = $('#SettleSign').val();   // "+" or "-"
        var SETTL_SIGN = sign == '+' ? 'P' : 'M';
        //var RECV = (sign === '+')
        //    ? (receivedHidden + SETTl)
        //    : (receivedHidden - SETTl);
        var RECV = receivedHidden;

        if (empr_POSTransaction.payType == "Advance") {
            //  //  
            let advAmount = parseFloat($("#advAmount").val()) || 0;
            let advBankAmount = parseFloat($("#advBankAmount").val()) || 0;
            var ADVANCE = advAmount;
            var CACT_CODE = advAmount != 0 ? $('#Acounthidden').val() : 0;
            var ADV_BANK = advBankAmount;
            var ADV_BOOK_TYPE = advBankAmount != 0 ? $('#BACCOUNT').dxSelectBox('option', 'value') : 0;
            var CASH = parseFloat($("#fnlAmount").val()) || 0;
            var CACT_CODE = CASH != 0 || advAmount != 0 ? $('#Acounthidden').val() : 0;
            var BANK = parseFloat($("#fnlBankAmount").val()) || 0;
            RECV = CASH + BANK;
            if (RECV != 0 && BANK != 0)
                var BACT_CODE = $('#AdvAccount').dxSelectBox('option', 'value');
            else
                var BACT_CODE = 0;
            var COMPLETE = $('#complete').val();
            var CASH_TAX = advAmount != 0 || CASH != 0 ? $('#advcashtax').val() : 0;
            var BANK_TAX = BANK != 0 || advBankAmount != 0 ? $('#advbanktax').val() : 0;
            var CashTaxAmt = Math.round((NET_TOTAL * CASH_TAX) / 100);
            var TaxAmt = Math.round(CashTaxAmt);
            var TAX_AMT = TaxAmt;
            var CASH_BACK = $('#advReturn').val();
            var CASHTAX_AMT = $('#CashTax_Amt2').val();
            var DEL_DATE = $('#delDate').val();
            var REMARK = $('#advremark').val();
        }

        else if (empr_POSTransaction.payType == "Cash") {

            var CASH = $('#cashAmount').val();
            var COMPLETE = 1;
            var ADVANCE = 0;
            var CACT_CODE = $('#Acounthidden').val();
            var CASH_TAX = $('#cashtax').val();
            TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * CASH_TAX) / 100);
            var TAX_AMT = TaxAmt;
            $('#CashTax_Amt2').val(TaxAmt);
            var CASH_BACK = $('#cashReturn').val();
            var CASHTAX_AMT = $('#CashTax_Amt2').val();
            var REMARK = $('#cashremark').val();
        }
        else if (empr_POSTransaction.payType == "Bank" || empr_POSTransaction.payType == "Card") {
            var BANK = $('#bankrecv').val();
            var COMPLETE = 1;
            var ADVANCE = 0;
            var BACT_CODE = $('#BACCOUNTS').dxSelectBox('option', 'value');
            var BANK_TAX = $('#banktax').val();
            var BANKTAX_AMT = $('#BankTax_Amt2').val();
            var REMARK = $('#bankremark').val();
        }
        else if (empr_POSTransaction.payType == "Party") {
            var PARTY = $('#partyrecv').val();
            var PARTY_CODE = $('#partyhidden').val();
            var COMPLETE = 1;
            var PARTY_TAX = $('#partytax').val();
            var ACT_CODE = $('#partycode').val();
            var PARTY_NAME = $('#partynamehidden').val();
            var PARTY_NUMBER = $('#partycellhidden').val();
            var PARTYTAX_AMT = $('#PartyTax_Amt2').val();
            var DUE_DATE = $('#dueDate').val();
            var BILLMODE = empr_POSTransaction.billMode;
            if (BILLMODE == 'Cash')
                var CACT_CODE = $('#Acounthidden').val();
            if (BILLMODE == 'Card')
                var BACT_CODE = $('#PARTY_BACCOUNTS').dxSelectBox('option', 'value');
            var REMARK = $('#partyremark').val();
        }
        else if (empr_POSTransaction.payType == "Split") {
            var CASH = parseInt($('#cashAmount2').val()) || 0;
            if (CASH == 0) {
                var CACT_CODE = 0;
            }
            else {
                var CACT_CODE = $('#Acounthidden').val();
            }
            var COMPLETE = 1;
            var CASH_TAX = $('#cashtax').val();
            TaxAmt = Math.round((parseFloat($('#TotalPaymenthidden').val()) * CASH_TAX) / 100);
            var TAX_AMT = TaxAmt;
            $('#CashTax_Amt2').val(TaxAmt);
            var CASHTAX_AMT = $('#CashTax_Amt2').val();
            var BANK = parseInt($('#bankrecv2').val()) || 0;
            if (BANK == 0) {
                var BACT_CODE = 0;
            }
            else {
                var BACT_CODE = $('#BACCOUNTS2').dxSelectBox('option', 'value');
            }
            var BANK_TAX = $('#cashtax').val();
            var BANKTAX_AMT = $('#BankTax_Amt2').val();
            var PARTY = parseInt($('#partyrecv2').val()) || 0;
            if (PARTY == 0) {
                var PARTY_CODE = 0;
                var ACT_CODE = 0;
            }
            else {
                var PARTY_CODE = $('#partyhidden2').val();
                var ACT_CODE = $('#partycode2').val();
            }
            var PARTY_TAX = $('#cashtax').val();
            var PARTY_NAME = $('#partynamehidden').val();
            var PARTYTAX_AMT = $('#PartyTax_Amt2').val();
        }
        else {
            var CASH = 0;
            var CACT_CODE = 0;
            var CASH_TAX = 0;
            var CASHTAX_AMT = 0;
            var BANK = 0;
            var BACT_CODE = 0;
            var BANK_TAX = 0;
            var BANKTAX_AMT = 0;
            var PARTY = 0;
            var PARTY_CODE = 0;
            var PARTY_TAX = 0;
            var PARTY_NAME = 0;
            var PARTYTAX_AMT = 0;
            var ACT_CODE = 0;
            var ADVANCE = 0;
            var COMPLETE = 0;
        }
        //  
        var bankCharges = BILL_STATUS == 'K' ? 0 : $('#bankcharges').val();
        var WAITERNAME = INV_STATUS == 2 ? $('#waiterName').val() : '';
        var TABLENUM = INV_STATUS == 2 ? $('#tableNum').val() : '';
        var WAITER = INV_STATUS == 2 ? empr_POSTransaction.selectedWaiter : 0;
        var TABLE = INV_STATUS == 2 ? empr_POSTransaction.selectedTable : 0;
        var SRBINV = $('#SRBInvoiceId').val() == "" || null ? "" : $('#SRBInvoiceId').val();
        var SRBSTATUS = '';
        if ($('#srbStatus').val() == 'Y' && $('#TAX_PAID').is(':checked'))
            SRBSTATUS = 'Y';
        else
            SRBSTATUS = 'N';

        //let isPercent = $('#isBillDiscPercent').is(':checked');
        //let inputVal = parseFloat($('#billDisc').val()) || 0;

        //if (isPercent) {

        //    DISC = inputVal;    
        //    billDisc = 0;       
        //} else {
        //    billDisc = inputVal;
        //    DISC = 0;            
        //}

        var CARD_NO = $('#CardDisc').val();
        var CURR_POINTS = parseInt($("#currentPoints").text()) || 0;
        var PREV_POINTS = parseInt($("#prevPoints").text()) || 0;
        var PAY_TYPE = empr_POSTransaction.payType;
        var PAY_QTY = parseInt($('#paySetting').val()) || 0;
        var PRE_QTY = parseInt($('#preSetting').val()) || 0;
        var DUP_QTY = parseInt($('#dupSetting').val()) || 0;
        var PRINT_COPIES = 0;
        if (PAY_QTY > 0) {
            PRINT_COPIES = PAY_QTY;
        }
        if (PRE_QTY > 0) {
            PRINT_COPIES = PRE_QTY;
        }
        if (DUP_QTY > 0) {
            PRINT_COPIES = DUP_QTY;
        }
        $('#paySetting').val(empr_POSTransaction.isPayQty);
        $('#preSetting').val(empr_POSTransaction.isPreQty);
        $('#dupSetting').val(empr_POSTransaction.isDupQty);
        var masterRecord = {
            PWINDOW: PWINDOW,
            TRAN_ID: TRANSID,
            V_DATE: V_DATE,
            MD_ID: MD_ID,
            VOUCHER_NO: VOUCHER_NO,
            SRB_VN: SRB_VN,
            CNAME: CNAME,
            CMOB: CMOB,
            CADD: CADD,
            INV_STATUS: INV_STATUS,
            BILL_STATUS: BILL_STATUS,
            TOTAL: TOTAL,
            DISC: DISC,
            ITEM_DISCOUNT: ITEM_DISCOUNT,
            DISC_AMT: DISC_AMT,
            NET_TOTAL: NET_TOTAL,
            DEL: DEL,
            DEL_CHARGES: DEL_CHARGES,
            TAX: TAX,
            TAX_AMT: TAX_AMT,
            SALESMAN: SALESMAN,
            SACT_CODE: SACT_CODE,
            SALESMANNAME: SALESMANNAME,
            USERNAME: USERNAME,
            COMMISION: COMMISION,
            CASHTAX_AMT: CASHTAX_AMT,
            BANKTAX_AMT: BANKTAX_AMT,
            PARTYTAX_AMT: PARTYTAX_AMT,
            BILL_STATUS: BILL_STATUS,
            CASH: CASH,
            CASH_BACK: CASH_BACK,
            CACT_CODE: CACT_CODE,
            BANK: BANK,
            BACT_CODE: BACT_CODE,
            ADV_BOOK_TYPE: ADV_BOOK_TYPE,
            PARTY: PARTY,
            PARTY_NAME: PARTY_NAME,
            PARTY_NUMBER: PARTY_NUMBER,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            DUE_DATE: DUE_DATE,
            RECV: RECV,
            BCHARGES: bankCharges,
            CASH_TAX: CASH_TAX,
            BANK_TAX: BANK_TAX,
            PARTY_TAX: PARTY_TAX,
            SRBInvoiceId: SRBINV,
            REMARK: REMARK,
            WAITER: WAITER,
            TABLE: TABLE,
            WAITERNAME: WAITERNAME,
            TABLENUM: TABLENUM,
            BILLMODE: BILLMODE,
            ADVANCE: ADVANCE,
            DEL_DATE: DEL_DATE,
            COMPLETE: COMPLETE,
            SRBSTATUS: SRBSTATUS,
            ADV_BANK: ADV_BANK,
            CARD_NO: CARD_NO,
            PREV_POINTS: PREV_POINTS,
            CURR_POINTS: CURR_POINTS,
            PAY_TYPE: PAY_TYPE,
            SER_CHARGES: SER_CHARGES,
            SETT: SETT,
            SETTL_SIGN: SETTL_SIGN,
            CardDiscValue: CardDiscValue,
            Return: empr_POSTransaction.return,
            PRINT_COPIES: PRINT_COPIES
        }
        empr_POSTransaction.KotMsg = 0;
        var detailRecords = [];
        //  //  
        //if (BILL_STATUS == 'K') {
        //    detailRecords.length = 0;
        //    detailRecords = empr_POSTransaction.DetailKOTPrintSet();
        //}
        //else {
        detailRecords.length = 0;

        detailRecords = empr_POSTransaction.DetailRecordsSet();
        //}
        //  
        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;
    },

    DetailKOTPrintSet() {
        var detailRecords = [];
        empr_POSTransaction.IsKot = 1;
        empr_POSTransaction.kotDuplicate = 0;
        $('#orderTable tr').each(function () {

            const row = $(this);
            let ritem = null;

            let AMT = row.find('.quantity-input').val() * row.find('.total-price').val();
            var itemcode = parseInt(row.find('td:eq(8)').text().trim()) > 0
                ? row.find('td:eq(8)').text().trim()
                : row.find('td:eq(7)').text().trim();

            const isItemNameVisible = row.find('.itemname').css('display') !== 'none';

            if (isItemNameVisible) {

                var rate = row.find('.total-price').val();
                var returnitem = row.find('#returnitem').val();
                var ritems = row.find('#ritem').val();
                var dtcode = row.find('#dtcode').val();
                if (dtcode == null || dtcode == '' || dtcode == 0) {
                    empr_POSTransaction.kotDuplicate = 1;
                }
                var discount = Math.abs(parseFloat(row.find('.discountvalue').val()) || 0);
                var qty = row.find('.quantity-input').val();
                var kotprint = row.find('.kotprint').val();
                var IsKot = $('#IsKot').val();
                //  //  
                var quantity = qty;
                var IsKotPrint = IsKot == 1 ? 1 : kotprint;
                if (empr_POSTransaction.return == true) {
                    if (returnitem == 1 || rate < 0) {
                        if (ritems == 0 || ritems == "undefined") {
                            if (rate.includes('-') || returnitem == 1) {
                                ritem = 1;
                                AMT = -(Math.abs(row.find('.quantity-input').val() * rate));
                                discount = -(Math.abs(discount));
                            } else {
                                ritem = 0;
                            }
                            let rowData = {
                                DT_CODE: parseInt(dtcode) || 0,
                                ITEM_CODE: itemcode,
                                TAX: row.find('.tax').val(),
                                TAX_AMT: 0,
                                ITEM_NAME: row.find('.itemname').text(),
                                QTY: quantity,
                                RATE: rate,
                                AMT: AMT,
                                UNIT: row.find('.unit').data('price') || row.find('.unit').attr('data-price'),
                                DISC: row.find('.discountpercent').val(),
                                RITEM: ritem,
                                DISC_AMT: discount,
                                TEM_CODE: row.find('.code').val(),
                                NET_AMT: Math.round(AMT - (discount)),
                                BARCODE: row.find('.Barcode').text(),
                                REMARKS: row.find('#remarks').val(),
                                ISKOTPRINT: IsKotPrint
                            };
                            detailRecords.push(rowData);
                        }

                    }
                }
                else {
                    if (rate.includes('-') || returnitem == 1) {
                        ritem = 1;
                        AMT = -(Math.abs(row.find('.quantity-input').val() * rate));
                        discount = -(Math.abs(discount));
                    } else {
                        ritem = 0;
                    }
                    let rowData = {
                        DT_CODE: parseInt(row.find('#dtcode').val()) || 0,
                        ITEM_CODE: itemcode,
                        TAX: row.find('.tax').val(),
                        TAX_AMT: 0,
                        ITEM_NAME: row.find('.itemname').text(),
                        QTY: quantity,
                        RATE: rate,
                        AMT: AMT,
                        UNIT: row.find('.unit').data('price') || row.find('.unit').attr('data-price'),
                        DISC: row.find('.discountpercent').val(),
                        RITEM: ritem,
                        DISC_AMT: discount,
                        TEM_CODE: row.find('.code').val(),
                        NET_AMT: Math.round(AMT - (discount)),
                        BARCODE: row.find('.Barcode').text(),
                        REMARKS: row.find('#remarks').val(),
                        ISKOTPRINT: IsKotPrint
                    };
                    detailRecords.push(rowData);
                }
            }
        });
        return detailRecords;
    },

    DetailRecordsSet() {
        var detailRecords = [];
        empr_POSTransaction.kotDuplicate = 0;
        $('#orderTable tr').each(function () {
             
            const row = $(this);
            let ritem = null;

            let AMT = row.find('.quantity-input').val() * row.find('.total-price').val();
            //var itemcode = parseInt(row.find('td:eq(8)').text().trim()) > 0
            //    ? row.find('td:eq(8)').text().trim()
            //    : row.find('td:eq(7)').text().trim();
            //var itemcode = parseInt(row.find('td:eq(2) input.SelectedItemId').val()) || 0;
            var itemcode = parseInt(row.find('.BarcodeId').val()) > 0
                ? row.find('.BarcodeId').val()
                : row.find('.Barcode').val();
            const isItemNameVisible = row.find('.itemname').css('display') !== 'none';
             
            if (isItemNameVisible) {
                 

                var rate = row.find('.total-price').val();

                if (!rate) {
                    return; // skip this row
                }
                var returnitem = row.find('#returnitem').val();
                var ritems = row.find('#ritem').val();
                var dtcode = row.find('#dtcode').val();
                if (dtcode == null || dtcode == '' || dtcode == 0) {
                    empr_POSTransaction.kotDuplicate = 1;
                }
                var discount = Math.abs(parseFloat(row.find('.discountvalue').val()) || 0);
                var qty = row.find('.quantity-input').val();
                var kotprint = row.find('.kotprint').val();
                var size = row.find('.selectedColor').val();
                var color = row.find('.selectedSize').val();

                //var taxAmts = row.find('.tax-amount').val();
                //var taxAmt = Math.abs(parseFloat(row.find('.tax-amount').val()) || 0);
                //  
                var IsKot = $('#IsKot').val();

                if (true) {
                    empr_POSTransaction.KotMsg = 1;
                    var quantity = qty;
                    var IsKotPrint = IsKot == 1 ? 1 : kotprint;
                    if (empr_POSTransaction.return == true) {
                        if ((returnitem == 1 && (dtcode == "" || dtcode == "undefined")) || rate < 0) {
                            if (ritems == 0 || ritems == "undefined") {
                                //  
                                if (rate.includes('-') || (returnitem == 1 && dtcode != "")) {
                                    ritem = 1;
                                    AMT = -(Math.abs(row.find('.quantity-input').val() * rate));
                                    discount = -(Math.abs(discount));
                                } else {
                                    ritem = 0;
                                }
                                 

                                let qty = quantity;
                                let rateVal = rate;
                                let taxPercent = parseFloat(row.find('.tax').val()) || 0;
                                let baseAmt = (qty * rateVal) - discount;
                                const taxAmt = ((baseAmt * taxPercent) / (100 + taxPercent)).toFixed(2);
                                let finalAmt = baseAmt;
                                let netAmt = Math.round(finalAmt);
                                let rowData = {
                                    PICK_ID: parseInt(dtcode) || 0,
                                    DT_CODE: 0,
                                    ITEM_CODE: itemcode,
                                    TAX: row.find('.tax').val(),
                                    TAX_AMT: taxAmt,
                                    ITEM_NAME: row.find('.itemname').text(),
                                    COLOR: size,
                                    SIZE: color,
                                    QTY: quantity,
                                    RATE: rate,
                                    AMT: AMT,
                                    UNIT: row.find('.unit').data('price') || row.find('.unit').attr('data-price'),
                                    DISC: row.find('.discountpercent').val(),
                                    RITEM: ritem,
                                    DISC_AMT: discount,
                                    TEM_CODE: row.find('.code').val(),
                                    NET_AMT: netAmt,
                                    BARCODE: row.find('.Barcode').text(),
                                    REMARKS: row.find('#remarks').val(),
                                    ISKOTPRINT: IsKotPrint
                                };
                                detailRecords.push(rowData);
                            }

                        }
                    }
                    else {

                         
                        //var rateInput = row.find('.total-price');
                        //if (rateInput.length === 0 || rateInput.val() === undefined) {
                        //    return; 
                        //}
                        var rate = row.find('.total-price').val();
                        let netAmt;
                        let taxAmt;
                        if ((rate && rate.includes('-')) || returnitem == 1) {
                            ritem = 1;
                            AMT = -(Math.abs(row.find('.quantity-input').val() * rate));
                            discount = -(Math.abs(discount));
                            let qty = quantity;
                            let rateVal = rate;
                            let taxPercent = parseFloat(row.find('.tax').val()) || 0;
                            let baseAmt = (qty * rateVal) - discount;
                            taxAmt = ((baseAmt * taxPercent) / (100 + taxPercent)).toFixed(2);
                            let finalAmt = baseAmt;
                            netAmt = -Math.abs(Math.round(finalAmt));
                        } else {
                            ritem = 0;
                            let qty = quantity;
                            let rateVal = rate;
                            let taxPercent = parseFloat(row.find('.tax').val()) || 0;
                            let baseAmt = (qty * rateVal) - discount;
                            taxAmt = ((baseAmt * taxPercent) / (100 + taxPercent)).toFixed(2);
                            let finalAmt = baseAmt;
                            netAmt = Math.round(finalAmt);
                        }



                        let rowData = {
                            DT_CODE: parseInt(row.find('#dtcode').val()) || 0,
                            ITEM_CODE: itemcode,
                            TAX: row.find('.tax').val(),
                            TAX_AMT: taxAmt,
                            ITEM_NAME: row.find('.itemname').text(),
                            QTY: quantity,
                            COLOR: color,
                            SIZE: size,
                            RATE: rate,
                            AMT: AMT,
                            UNIT: row.find('.unit').data('price') || row.find('.unit').attr('data-price'),
                            DISC: row.find('.discountpercent').val(),
                            RITEM: ritem,
                            DISC_AMT: discount,
                            TEM_CODE: row.find('.code').val(),
                            NET_AMT: netAmt,
                            BARCODE: row.find('.Barcode').text(),
                            REMARKS: row.find('#remarks').val(),
                            ISKOTPRINT: IsKotPrint
                        };
                        detailRecords.push(rowData);
                    }

                }
            }
        });
        return detailRecords;
    },

    ExpenseValidate() {
        var valid = true
        var Desc = $('#Des').val();
        var Amt = parseInt($('#expamt').val()) || 0;
        var ExpDrp = $('#ExpenseType').dxSelectBox('option', 'value');
        if (Desc == '') {
            empr_helper.notify("Please Enter Description.", 2);
            valid = false;
            return valid;
        }
        else if (ExpDrp == '') {
            empr_helper.notify("Please Select Expense Dropdown.", 2);
            valid = false;
            return valid;
        }
        else if (Amt == 0) {
            empr_helper.notify("Please Enter Amount.", 2);
            valid = false;
            return valid;
        }
        else {
            return valid;
        }
    },

    ExpenseData() {
        empr_POSTransaction.TodayDate()

        console.log(MapTable);

        var TRANSID = parseInt($("#ExpTranId").val()) || 0;
        var ExpDate = $('#V_DATE').text();// Default to 0 if input is invalid
        var VOUCHER_NO = $("#EXPVOUCHER_NO").val();
        var ExpAmount = parseInt($('#expamt').val()) || 0;
        var Descr = $('#Des').val();
        var BOOK_TYPE = MapTable[0]?.cashAct;
        var ACT_CODE = $('#ExpenseType').dxSelectBox('option', 'value');

        var masterRecord = {
            TRAN_ID: TRANSID,
            EXPDATE: ExpDate,
            ExpAmount: ExpAmount,
            Descr: Descr,
            VOUCHER_NO: VOUCHER_NO,
            BOOK_TYPE: BOOK_TYPE,
            ACT_CODE: ACT_CODE,
        }

        var modelRecord = {
            Master: masterRecord,
        };
        return modelRecord;

    },

    GetGridData: async function () {
        return await $('#detailContainer').dxDataGrid('instance').option("dataSource");
    },

    ValidateInfo() {

        var valid = true;
        var data = empr_POSTransaction.GetDataToSave();

        if (data.Master.BOOK_TYPE == "" || data.Master.BOOK_TYPE == null || data.Master.BOOK_TYPE == undefined) {
            empr_helper.notify("Please select book type.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.INV_STATUS == "" || data.Master.INV_STATUS == null || data.Master.INV_STATUS == undefined) {
            empr_helper.notify("Please select Invoice Status.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.CMOB == "" || data.Master.CMOB == null) {
            empr_helper.notify("Phone number is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.CNAME == "" || data.Master.CNAME == null) {
            empr_helper.notify("Customer name is required.", 2);
            valid = false;
            return valid;
        }

        if (data.Master.CASH == "" || data.Master.CASH == null) {
            empr_helper.notify("Cash amount is required..", 2);
            valid = false;
            return valid;
        }

        if (data.Master.RECV == "" || data.Master.RECV == null || data.Master.RECV == 0) {
            empr_helper.notify("Receive amount is required..", 2);
            valid = false;
            return valid;
        }

        data.Detail = $('#detailContainer').dxDataGrid('instance').option("dataSource");

        if (data.Detail.length == 0) {
            empr_helper.notify("Please add items.", 2);
            valid = false;
            return valid;
        }
        $.each(data.Detail, function (index, item) {
            if (item.iteM_CODE == "" || item.iteM_CODE == null || item.iteM_CODE == undefined) {
                empr_helper.notify("Please select item at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty ItemCode.");
            }
            if (item.qty == "" || item.qty == null || item.qty == undefined) {
                empr_helper.notify("Please enter item quantity at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty Quantity.");
            }

            if (item.rate != "" && item.rate != null && item.rate != undefined && item.rate <= 0) {
                empr_helper.notify("Please enter correct rate at index " + index, 2);
                valid = false;
                return valid;
                console.log("Item at index " + index + " has empty rate.");
            }
        });

        return valid;
    },

    PrintModal(MD_ID, billStatus) {
        var kotMsg;
        if (billStatus == "K") {
            empr_POSTransaction.printStatus = "K";
            kotMsg = true;
        }
        else {
            empr_POSTransaction.printStatus = "";
            kotMsg = false;
        }
        empr_POSTransaction.SaveInfo("K");
        empr_POSTransaction.updateSummary();
        empr_POSTransaction.ResetForm();
        empr_POSTransaction.ResetAllFields();

    },

    SaveInfo(status) {
         
        var Id = $("#TRANS_ID").val();
        empr_POSTransaction.TodayDate();
        var dataModel = empr_POSTransaction.GetDataToSave(undefined, status);
        console.log("before save data ", dataModel);
        //  
        if (dataModel.Master.PAY_TYPE == "Party") {

            empr_POSTransaction.selectedParty = {
                partyCode: parseInt(dataModel.Master.PARTY_CODE),
                actCode: parseInt(dataModel.Master.ACT_CODE)
            };

        }
        var isDuplicate = dataModel.Detail.filter(x => !x.DT_CODE || x.DT_CODE == 0);
        if (isDuplicate.length === 0 && dataModel.Master.BILL_STATUS === 'K') {
            swal({
                title: 'This item is already added!',
                text: "Please select a different KOT item or create a duplicate bill.",
                type: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#0CC27E',
                cancelButtonColor: '#FF586B',
                confirmButtonText: 'Yes, duplicate it!',
                cancelButtonText: 'No, cancel!',
                confirmButtonClass: 'btn btn-success mr-5',
                cancelButtonClass: 'btn btn-danger kotSwalCancel',
                buttonsStyling: false
            }).then(function () {
                 
                ajaxHelper.ajaxPostJsonData({ tranId: Id, BillStatus: 'K' }, "/POSTransactions/PrintModal", function (data) {
                    if (data.msgType == 1) {
                        //  //  
                        $('#slipPreview').html(data.slipHtml);
                        empr_POSTransaction.KOTPrint();
                        empr_helper.notify(data.msg, data.msgType);
                    }
                });
            });
        }
        else if (empr_POSTransaction.kotDuplicate == 0 && dataModel.Master.BILL_STATUS == 'H') {
            empr_helper.notify("This Record Is already has been Saved", 1);
        }
        else {
            if (empr_POSTransaction.return == false) {
                if ($('#srbStatus').val() == 'Y' && $('#TAX_PAID').is(':checked') && dataModel.Master.BILL_STATUS == "P" && ($('#SRBInvoiceId').val() == "" || $('#SRBInvoiceId').val() == null)) {
                    empr_POSTransaction.SRB_PostData(dataModel);
                }
                else {
                     
                    ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/Save", function (data) {
                        if (data.msgType == 1) {
                             
                            empr_POSTransaction.code = data.data.code;
                            $('#slipPreview').html(data.slipHtml);
                            if (dataModel.Master.BILL_STATUS == 'K') {
                                //  //  
                                if (data.pWindow == 1) {
                                    empr_POSTransaction.KOTPrint();
                                }
                                else {
                                    empr_POSTransaction.printContent_QZ(data.slipFilePath, data.barFilePath, data.kotPrinter, data.stickerPrinter);
                                }
                                empr_helper.notify(data.msg, data.msgType);
                            }
                            else if (dataModel.Master.BILL_STATUS == 'H') {
                                empr_helper.notify(data.msg, data.msgType);
                            }
                            else {
                                if (data.pWindow == 1) {
                                    const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                    const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                                    if (qrCodeImg !== null && srbcodeimg !== null) {
                                        if (qrCodeImg.complete && srbcodeimg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            qrCodeImg.onload = () => {
                                                if (srbcodeimg.complete) {
                                                    empr_POSTransaction.printContent();
                                                }
                                            };
                                            srbcodeimg.onload = () => {
                                                if (qrCodeImg.complete) {
                                                    empr_POSTransaction.printContent();
                                                }
                                            };
                                        }
                                    } else if (qrCodeImg !== null) {
                                        if (qrCodeImg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            qrCodeImg.onload = () => {
                                                empr_POSTransaction.printContent();
                                            };
                                        }
                                    } else if (srbcodeimg !== null) {
                                        if (srbcodeimg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            srbcodeimg.onload = () => {
                                                empr_POSTransaction.printContent();
                                            };
                                        }
                                    } else {
                                        empr_POSTransaction.printContent();
                                    }
                                }

                                empr_helper.notify(data.msg, data.msgType);
                            }

                        }
                        else if (data.msgType == 2 && data.msg.includes('This invoice has either been paid')) {
                            //  //  
                            //empr_POSTransaction.TodayDateTime();
                            //var dataModels = empr_POSTransaction.GetDataToSave(undefined, status);
                            ajaxHelper.ajaxPostJsonData({ tranId: data.data.code }, "/POSTransactions/PrintModal", function (printdata) {
                                if (printdata.msgType == 1) {

                                    console.log(printdata.slipHtml);
                                    $('#slipPreview').html(printdata.slipHtml);

                                    const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                    const srbcodeimg = document.querySelector('#slipPreview img.srb-code'); // Corrected the selector to match the casing

                                    if (qrCodeImg !== null && srbcodeimg !== null) {
                                        if (qrCodeImg.complete && srbcodeimg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            qrCodeImg.onload = () => {
                                                if (srbcodeimg.complete) {
                                                    empr_POSTransaction.printContent();
                                                }
                                            };
                                            srbcodeimg.onload = () => {
                                                if (qrCodeImg.complete) {
                                                    empr_POSTransaction.printContent();
                                                }
                                            };
                                        }
                                    } else if (qrCodeImg !== null) {
                                        if (qrCodeImg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            qrCodeImg.onload = () => {
                                                empr_POSTransaction.printContent();
                                            };
                                        }
                                    } else if (srbcodeimg !== null) {
                                        if (srbcodeimg.complete) {
                                            empr_POSTransaction.printContent();
                                        } else {
                                            srbcodeimg.onload = () => {
                                                empr_POSTransaction.printContent();
                                            };
                                        }
                                    } else {
                                        empr_POSTransaction.printContent();
                                    }
                                }
                            }, false, true);
                            empr_helper.notify(data.msg, 2);
                        }
                        else {
                            empr_helper.notify(data.msg, 2);
                        }
                    }, false, true);
                }
            }
            else {
                //  //  
                ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/Save", function (data) {
                    if (data.msgType == 1) {
                        //  //  
                        empr_POSTransaction.code = data.data.code;
                        $('#slipPreview').html(data.slipHtml);

                        const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                        const srbcodeimg = document.querySelector('#slipPreview img.srb-code'); // Corrected the selector to match the casing

                        if (qrCodeImg !== null && srbcodeimg !== null) {
                            if (qrCodeImg.complete && srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    if (srbcodeimg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                                srbcodeimg.onload = () => {
                                    if (qrCodeImg.complete) {
                                        empr_POSTransaction.printContent();
                                    }
                                };
                            }
                        } else if (qrCodeImg !== null) {
                            if (qrCodeImg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                qrCodeImg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else if (srbcodeimg !== null) {
                            if (srbcodeimg.complete) {
                                empr_POSTransaction.printContent();
                            } else {
                                srbcodeimg.onload = () => {
                                    empr_POSTransaction.printContent();
                                };
                            }
                        } else {
                            empr_POSTransaction.printContent();
                        }
                        empr_helper.notify(data.msg, data.msgType);

                    }
                    else {
                        empr_helper.notify(data.msg, 2);
                    }
                }, false, true);
            }
        }
    },

    SRB_PostData(dataModel) {
        var SrbName = $('#srbName').val();
        var SrbId = $('#srbId').val();
        var SrbNtn = $('#srbNtn').val();
        var SrbUser = $('#posUser').val();
        var SrbPass = $('#posPass').val();
        var SrbUrl = $('#srbUrl').val();

        ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_POSTransaction.code = data.data.code;
                empr_POSTransaction.TodayDateTime();
                var dataModels = empr_POSTransaction.GetDataToSave(undefined, "");
                dataModels.Master.VOUCHER_NO = data.data.voucherNo;

                var datePart = data.data.srbInvoiceDate;
                var now = new Date();
                var timePart = now.toLocaleTimeString('en-US');
                var formattedDateTime = datePart + ' ' + timePart;

                //  //  
                var taxAmount =
                    parseFloat(dataModel.Master.CASHTAX_AMT) ||
                    parseFloat(dataModel.Master.BANKTAX_AMT) ||
                    parseFloat(dataModel.Master.PARTYTAX_AMT) ||
                    0;
                var taxRate =
                    parseFloat(dataModel.Master.CASH_TAX) ||
                    parseFloat(dataModel.Master.BANK_TAX) ||
                    parseFloat(dataModel.Master.PARTY_TAX) ||
                    0;
                var itemdiscount = parseInt(dataModel.Master.ITEM_DISCOUNT);

                if (data.data.srbNoucherNo != '' && data.data.srbNoucherNo != null && data.data.srbNoucherNo != undefined) {
                    var jsonText = {
                        posId: SrbId, name: SrbName, ntn: SrbNtn, invoiceID: data.data.srbNoucherNo,
                        invoiceDateTime: formattedDateTime, invoiceType: 1, rateValue: taxRate, saleValue: dataModel.Master.NET_TOTAL - itemdiscount,
                        taxAmount: taxAmount, consumerName: dataModel.Master.CNAME || "N/A",
                        consumerNTN: dataModel.Master.CMOB || "N/A", address: dataModel.Master.CADD || "N/A",
                        tariffCode: "N/A", extraInf: "N/A", pos_user: SrbUser, pos_pass: SrbPass, SrbUrl, BillVoucher: data.data.voucherNo
                    };

                    $.ajax({
                        type: "POST",
                        url: "/POSTransactions/PostToSRB",
                        contentType: "application/json",
                        data: JSON.stringify(jsonText),
                        success: function (srbRes) {

                            if (srbRes.resCode === '00') {
                                console.log("This is Invoice: " + srbRes.srbInvoceId);
                                dataModel.Master.SRBInvoiceId = srbRes.srbInvoceId;
                                dataModels.Master.SRBInvoiceId = srbRes.srbInvoceId;

                                ajaxHelper.ajaxPostJsonData({ tranId: data.data.code }, "/POSTransactions/PrintModal", function (data) {
                                    if (data.msgType == 1) {
                                        $('#slipPreview').html(data.slipHtml);

                                        const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                        const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                                        if (qrCodeImg !== null && srbcodeimg !== null) {
                                            if (qrCodeImg.complete && srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                qrCodeImg.onload = () => {
                                                    if (srbcodeimg.complete) {
                                                        empr_POSTransaction.printContent();
                                                    }
                                                };
                                                srbcodeimg.onload = () => {
                                                    if (qrCodeImg.complete) {
                                                        empr_POSTransaction.printContent();
                                                    }
                                                };
                                            }
                                        } else if (qrCodeImg !== null) {
                                            if (qrCodeImg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                qrCodeImg.onload = () => {
                                                    empr_POSTransaction.printContent();
                                                };
                                            }
                                        } else if (srbcodeimg !== null) {
                                            if (srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                srbcodeimg.onload = () => {
                                                    empr_POSTransaction.printContent();
                                                };
                                            }
                                        } else {
                                            empr_POSTransaction.printContent();
                                        }
                                    }
                                }, false, true);
                            } else {
                                console.log("Error: " + srbRes.err);
                                dataModel.Master.SRBInvoiceId = `Error: ${srbRes.err}`;
                                empr_helper.notify("SRB Site Data Send And Return an Error.", 2);

                                ajaxHelper.ajaxPostJsonData({ tranId: data.data.code }, "/POSTransactions/PrintModal", function (data) {
                                    if (data.msgType == 1) {
                                        $('#slipPreview').html(data.slipHtml);

                                        const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                                        const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                                        if (qrCodeImg !== null && srbcodeimg !== null) {
                                            if (qrCodeImg.complete && srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                qrCodeImg.onload = () => {
                                                    if (srbcodeimg.complete) {
                                                        empr_POSTransaction.printContent();
                                                    }
                                                };
                                                srbcodeimg.onload = () => {
                                                    if (qrCodeImg.complete) {
                                                        empr_POSTransaction.printContent();
                                                    }
                                                };
                                            }
                                        } else if (qrCodeImg !== null) {
                                            if (qrCodeImg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                qrCodeImg.onload = () => {
                                                    empr_POSTransaction.printContent();
                                                };
                                            }
                                        } else if (srbcodeimg !== null) {
                                            if (srbcodeimg.complete) {
                                                empr_POSTransaction.printContent();
                                            } else {
                                                srbcodeimg.onload = () => {
                                                    empr_POSTransaction.printContent();
                                                };
                                            }
                                        } else {
                                            empr_POSTransaction.printContent();
                                        }
                                    }
                                }, false, true);
                            }

                        },
                        error: function (xhr) {
                            console.log("SRB Server Error:", xhr.responseText);
                            dataModel.Master.SRBInvoiceId = "Error contacting SRB server";
                            empr_helper.notify("SRB Integration Failed at Server level.", 2);
                        }
                    });
                }
                else {
                    ajaxHelper.ajaxPostJsonData({ tranId: data.data.code }, "/POSTransactions/PrintModal", function (data) {
                        if (data.msgType == 1) {
                            $('#slipPreview').html(data.slipHtml);

                            const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
                            const srbcodeimg = document.querySelector('#slipPreview img.srb-code');

                            if (qrCodeImg !== null && srbcodeimg !== null) {
                                if (qrCodeImg.complete && srbcodeimg.complete) {
                                    empr_POSTransaction.printContent();
                                } else {
                                    qrCodeImg.onload = () => {
                                        if (srbcodeimg.complete) {
                                            empr_POSTransaction.printContent();
                                        }
                                    };
                                    srbcodeimg.onload = () => {
                                        if (qrCodeImg.complete) {
                                            empr_POSTransaction.printContent();
                                        }
                                    };
                                }
                            } else if (qrCodeImg !== null) {
                                if (qrCodeImg.complete) {
                                    empr_POSTransaction.printContent();
                                } else {
                                    qrCodeImg.onload = () => {
                                        empr_POSTransaction.printContent();
                                    };
                                }
                            } else if (srbcodeimg !== null) {
                                if (srbcodeimg.complete) {
                                    empr_POSTransaction.printContent();
                                } else {
                                    srbcodeimg.onload = () => {
                                        empr_POSTransaction.printContent();
                                    };
                                }
                            } else {
                                empr_POSTransaction.printContent();
                            }
                        }
                    }, false, true);
                }
            }
        }, false, true);


    },
    DeleveryInputvisible(code, Id) {

        if (code == "Delivery" || Id == 4) {
            $('#INV_STATUS').val(Id);
            $("#inputContainer").show();
            $("#dineInInputContainer").hide(); // Ammar

        }
        else {
            $("#inputContainer").hide();
            $('#billDelPercent').val(0);
            $('#billDelValue').val(0);

        }
    },

    DineInInputvisible() {
        var serviceChargesRate = $('#serviceChargesRate').val() == 0 ? 0 : $('#serviceChargesRate').val();
        $('#serviceChargesPercent').val(serviceChargesRate);
        empr_POSTransaction.ServiceChargesOnChangeFunction('serviceChargesPercent');
        $("#dineInInputContainer").show();
    },

    DineInvisible() {
        if (empr_POSTransaction.billStatus != 'P') {
            ajaxHelper.ajaxGetJson('/POSTransactions/GetAllWaiter', function (data) {

                empr_POSTransaction.waitersData = data;
                if (data != null) {
                    empr_POSTransaction.waiterRecords = data;
                    var WaiterImage = $('#WaiterImg').val();
                    var itemsHtml = $.map(data, function (item) {
                        var isSelected = item.code === empr_POSTransaction.selectedWaiter ? " selected-waiter" : "";
                        return `
                                <div class="box waiter-item boxImageDesign${isSelected}" data-item="${item.code}">
                                <img src="${item.ipic}" alt="" onerror="this.onerror=null;this.src='${WaiterImage}';">
                                <p>
                                    ${item.descr}
                                </p>
                                </div>
                            `;
                    }).join('');

                    $('.waiterModalData').html(itemsHtml);

                } else {
                    $('.waiterModalData').html('<p>No Waiters available.</p>');
                }
            }, false, true);

        }
    },

    Tablesvisible(code) {
        if (empr_POSTransaction.billStatus != 'P') {
            ajaxHelper.ajaxGetJson('/POSTransactions/GetAllTables?Tran_Id=' + encodeURIComponent(code), function (data) {
                empr_POSTransaction.tablesData = data;
                // Check if the response contains valid data
                if (data != null) {
                    empr_POSTransaction.tableRecords = data;
                    var TableImage = $('#TableImg').val()
                    var itemsHtml = $.map(data, function (item) {
                        var isSelected = item.code === empr_POSTransaction.selectedTable ? " selected-table" : "";
                        return `
                                <div class="box table-item boxImageDesign${isSelected}" data-item="${item.code}">
                                <img src="${item.ipic}" alt="" onerror="this.onerror=null;this.src='${TableImage}';">
                                <p>
                                    ${item.descr}
                                </p>
                                </div>
                            `;
                    }).join('');

                    $('.tableModalData').html(itemsHtml);

                } else {
                    // Handle case when no data is returned or msgType is not 1
                    $('.tableModalData').html('<p>No Tables available.</p>');
                }
            }, false, true);
        }
    },

    GetPOSTransactionByCode(code, voucher) {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSTransactionByCode?code=' + code + '&voucher=' + voucher, function (data) {
            if (data.master.msgType == 1) {
                 

                var masterData = data.master.data;
                if (data.detail.msgType == 1) {
                     
                    let hasNegativeQty = data.detail.data.some(item => Number(item.qty) < 0);
                    empr_POSTransaction.SetItemsEditData(data.detail.data);


                    if (hasNegativeQty) {
                        empr_POSTransaction.return11 = true;
                        //$("#tablechk").addClass("active");
                        
                        //$("#tablechk").trigger("click");

                        //$("#tablechk").css({
                        //    "pointer-events": "none",
                        //    "opacity": "0.5",
                        //    "cursor": "not-allowed"
                        //});
                    }
                    if (Permissions != 'Admin') {
                        if (!Permissions.Edit) {
                            $(".quantity-input").prop("readonly", true);
                            $(".total-price").prop("readonly", true);
                            //$(".discountpercent").prop("readonly", true);
                            $(".delete-icon").prop("readonly", true);
                        }
                    }
                    else {
                        $(".quantity-input").prop("readonly", false);
                        $(".total-price").prop("readonly", false);
                        //$(".discountpercent").prop("readonly", false);
                        $(".delete-icon").prop("readonly", false);
                    }

                }
                if (masterData.length == 1) {

                    $('.headers .icondiv').css({
                        'background-color': '#055a87'
                    });
                    var response = masterData[0];
                    console.log("masterresponsense", response);
                    //  //  

                    //if (response.disc != null && response.disc !== 0) {

                    //    document.getElementById("isBillDiscPercent").checked = true;   // checkbox check
                    //    $('#billDisc').val(response.disc);

                    //} else {

                    //}
                    empr_POSTransaction.billStatus = response.bilL_STATUS;
                    empr_POSTransaction.statusId = response.inV_STATUS;
                    empr_POSTransaction.SER_CHARGES = response.seR_CHARGES == 0 ? empr_POSTransaction.SER_CHARGES : response.seR_CHARGES;
                    $('#serviceChargesValue').val(empr_POSTransaction.SER_CHARGES);
                    /*$(`#${empr_POSTransaction.statusId}`, '.headers').trigger('click');*/
                    if (empr_POSTransaction.statusId == 2) {
                        $(`.headers #${empr_POSTransaction.statusId}`).css({
                            'background-color': '#0e1832',
                        });
                        //empr_POSTransaction.DineInInputvisible();
                        empr_POSTransaction.ServiceChargesOnChangeFunction('serviceChargesValue');
                        $("#dineInInputContainer").show();
                    }
                    else if (empr_POSTransaction.statusId == 4) {
                        $(`.headers #${empr_POSTransaction.statusId}`).css({
                            'background-color': '#0e1832',
                        });
                        empr_POSTransaction.DeleveryInputvisible("Delivery", empr_POSTransaction.statusId);
                    }
                    else {
                        $(`.headers #${empr_POSTransaction.statusId}`).css({
                            'background-color': '#0e1832',
                        });
                    }
                    $('#TRANS_ID').val(response.traN_ID);
                    $('#V_DATE').text(response.v_DATE);

                    empr_POSTransaction.settlsign = response.settL_SIGN;
                    empr_POSTransaction.InvoiceDate = response.v_DATE;
                    $('#BILL_STATUS').val(response.bilL_STATUS);
                    $('#BILL_MODE').val(response.bilL_MODE);
                    empr_POSTransaction.payType = response.paY_TYPE;
                    empr_POSTransaction.billMode = response.bilL_MODE;
                    $('#VOUCHER_NO').text(response.voucheR_NO);
                    $('#VOUCHERNO').val(response.voucheR_NO);
                    $('#SRBVN').val(response.srB_VN);
                    $('#CNAME').val(response.cname);
                    $('#CMOB').val(response.cmob);
                    $('#CADD').val(response.cadd);
                    $('#TOTAL').val(response.total);
                    $('#recvAmount').val(response.recv);
                    $('#billDiscPer').val(response.disc);
                    //$('#billDiscPer').val(response.disC_AMT);
                    $('#NET_TOTAL').val(response.neT_TOTAL);
                    $("#SalesmanName").dxSelectBox("instance").option("value", response.salesman);
                    $('#Username').val(response.ediT_USERID);
                    $('#SACTCODE').val(response.sacT_CODE);
                    $('#SCOMM').val(response.commision);
                    $('#remark').val(response.remarks);
                    $('#SRBInvoiceId').val(response.srbinv);
                    $('#CardDisc').val(response.cardno);
                    /*if ((response.cash != 0 && response.bank != 0) || (response.cash != 0 && response.party != 0) || (response.bank != 0 && response.party != 0)) {*/
                    if (empr_POSTransaction.payType == 'Split' && empr_POSTransaction.return == false) {
                        empr_POSTransaction.AccountTax = "Split";
                        $('#cashAmount2').val(response.cash);
                        $('#bankrecv2').val(response.bank);
                        $('#partyrecv2').val(response.party);
                        $('#partycode2').val(response.acT_CODE);
                        $('#partyhidden2').val(response.partY_CODE);
                        $('#cashremark').val(response.remarks);
                        $('#btnAdvance').prop('disabled', true);
                        $('#btnCash').prop('disabled', true);
                        $('#btnCard').prop('disabled', true);
                        $('#btnParty').prop('disabled', true);
                        empr_POSTransaction.selectTab(document.getElementById('btnSplit'), 3)
                    }
                    /*else if (response.party != 0) {*/
                    else if (empr_POSTransaction.payType == 'Party' && empr_POSTransaction.return == false) {
                        empr_POSTransaction.AccountTax = "Party";
                        $('#partyrecv').val(response.party);
                        $('#partyhidden').val(response.partY_CODE);
                        $('#partycode').val(response.acT_CODE);
                        let rawDate = response.duedate; // "01-Jul-25 12:00:00 AM"
                        let parsedDate = new Date(rawDate);
                        let formattedDate = parsedDate.toISOString().split('T')[0];
                        $('#dueDate').val(formattedDate);
                        $('#btnAdvance').prop('disabled', true);
                        $('#btnCash').prop('disabled', true);
                        $('#btnCard').prop('disabled', true);
                        $('#btnSplit').prop('disabled', true);
                        $('#partyremark').val(response.remarks);
                        empr_POSTransaction.selectRadioByLabel(response.billmode);
                        if (response.bilL_MODE == "Cash")
                            empr_POSTransaction.updateHiddenField('cashOption');
                        if (response.bilL_MODE == "Card") {
                            empr_POSTransaction.updateHiddenField('cardOption');
                            $("#PARTY_BACCOUNTS").show();
                            $("#party_account").show();
                        }
                        if (response.bilL_MODE == "Credit")
                            empr_POSTransaction.updateHiddenField('creditOption');
                        empr_POSTransaction.selectTab(document.getElementById('btnParty'), 2)
                    }
                    /*else if ((response.bank != 0 && response.advbank == 0) && response.advance == 0) {*/
                    else if (empr_POSTransaction.payType == 'Bank' && empr_POSTransaction.return == false) {
                         
                        empr_POSTransaction.AccountTax = "Bank";
                        $('#bankrecv').val(response.bank);
                        $('#btnAdvance').prop('disabled', true);
                        $('#btnCash').prop('disabled', true);
                        $('#btnParty').prop('disabled', true);
                        $('#btnSplit').prop('disabled', true);
                        $('#bankremark').val(response.remarks);
                        $('#btnParty, #btnSplit, #btnCash').closest('li').hide();
                        empr_POSTransaction.selectTab(document.getElementById('btnCard'), 1);
                        $('#btnCard').click();
                    }
                    /*else if (response.advance != 0 || response.advbank != 0) {*/
                    else if (empr_POSTransaction.payType == 'Advance' && empr_POSTransaction.return == false) {
                        empr_POSTransaction.AccountTax = "Advance";
                        $('#advAmount').val(response.advance).prop('readonly', true);
                        $('#advBankAmount').val(response.advbank).prop('readonly', true);
                        $('#advAmountbank').val(response.advbank).prop('readonly', true);
                        $('#delDate').val(response.deldate);
                        $('#delDate').prop('readonly', true);
                        $('#btnCash').prop('disabled', true);
                        $('#btnCard').prop('disabled', true);
                        $('#btnParty').prop('disabled', true);
                        $('#btnSplit').prop('disabled', true);
                        $('#advremark').val(response.remarks);
                        if (response.complete == 0) {
                            empr_POSTransaction.InitBankAccount(parseInt(response.adV_BOOKTYPE));
                            $('#BACCOUNT').dxSelectBox('option', 'readOnly', true);
                            $('#fnlBankAmount').val(response.bank);
                            $('#fnlAmount').val(response.cash);
                        }
                        else {
                            $('#completecheck').prop({
                                'checked': true,
                                'readonly': true,
                                'disabled': true
                            }).closest('.form-check').addClass('disabled');
                            $('#complete').val(response.complete);
                            $('#fnlBankAmount').val(response.bank).prop('readonly', true);
                            $('#fnlAmount').val(response.cash).prop('readonly', true);
                        }
                    }
                    else if (empr_POSTransaction.payType == 'Cash' && empr_POSTransaction.return == false) {
                        empr_POSTransaction.AccountTax = "Cash";
                        $('#btnAdvance').prop('disabled', true);
                        $('#btnCard').prop('disabled', true);
                        $('#btnParty').prop('disabled', true);
                        $('#btnSplit').prop('disabled', true);
                        $('#Cash').val(response.cash);
                        $('#cashAmount').val(response.cash);
                        if (response.recv < response.cash) {
                            empr_POSTransaction.returnCash = response.recv - response.cash;
                        }
                        $('#cashremark').val(response.remarks);
                        empr_POSTransaction.selectTab(document.getElementById('btnCash'), 0)
                        $('#btnCash').click();
                    }
                    $('#billDelPercent').val(response.del);
                    $('#billDelValue').val(response.deL_AMT);
                    if (response.inV_STATUS == 1) {
                        $('#INV_STATUS').val('');
                    }
                    else {
                        $('#INV_STATUS').val(response.inV_STATUS);
                    }
                     
                    //$('#subtotalTax').text(response.taX_AMT);
                    //$('#totalTax').text(`Total Tax: ${response.taX_AMT}`);
                    $('#returntotalAmountGet').val(response.total);
                    if (empr_POSTransaction.return == false)
                        $('#totalAmountGet').val(response.total);

                    var amt = parseFloat($('#totalAmountGet').val()) || 0;
                    var discPercent = parseFloat(response.disc) || 0;
                    $('#billDiscPer').val(response.disc);
                    // discount amount calculate
                    var discountAmount = Math.round((amt * discPercent) / 100);

                    // value set karo
                    if (response.disC_AMT != 0 && empr_POSTransaction.Return == false)
                        $('#billDisc').val(response.disC_AMT);
                    else
                        $('#billDisc').val(discountAmount);

                    // controls disable / readonly
                    $("#isBillDiscPercent").prop("disabled", true);
                    $("#billDisc").prop("readonly", true);

                    //var sett = response.settle == 0 || response.bilL_STATUS != 'P' ? '' : amt == 0 ? 0 : response.settle
                    var sett = amt == 0 ? 0 : response.settle
                    $('#settlementInput').val(sett);

                    $('#totalAmount').text(response.total);
                    $('#totalPayment').val(response.neT_TOTAL);
                    var SummaryTotal = parseInt(response.neT_TOTAL) + parseInt(response.deL_AMT);
                    $('#FinalAmount').html(`Value ${SummaryTotal} /-`);
                    //empr_POSTransaction.DeleveryInputvisible(response.inV_STATUS);
                    if (empr_POSTransaction.return == false)
                        empr_POSTransaction.updateSummary();
                    empr_POSTransaction.selectedWaiter = response.waiter;
                    empr_POSTransaction.selectedTable = response.table;

                    var waiterRecord = empr_POSTransaction.waitersData.filter(r => r.code == empr_POSTransaction.selectedWaiter);
                    if (waiterRecord.length > 0) {
                        $('#Waiter').text('Waiter : ' + waiterRecord[0].descr + ' |');
                        $("#waiterName").val(waiterRecord[0].descr);
                    }

                    var tableRecord = TablesData.filter(r => r.code == empr_POSTransaction.selectedTable);
                    if (tableRecord.length > 0) {
                        $('#Tables').text('Table : ' + tableRecord[0].name + ' |'); // ✅ corrected here
                        $("#tableNum").val(tableRecord[0].name);
                    }


                    if (empr_POSTransaction.billStatus == 'P') {
                        $('.quantity-input').prop('readonly', true);
                        $('.total-price').prop('readonly', true);
                        //$('.discountpercent').prop('readonly', true);
                        $("#partyName").dxSelectBox("instance").option("disabled", true);
                        /*$("#partyName2").dxSelectBox("instance").option("disabled", true);*/
                        $('#partyName2').dxSelectBox('option', 'readOnly', true);
                        $('#BACCOUNTS2').dxSelectBox('option', 'readOnly', true);
                        $('#BACCOUNTS').dxSelectBox('option', 'readOnly', true);
                        $('#PARTY_BACCOUNTS').dxSelectBox('option', 'readOnly', true);
                        $('#cashAmount2').prop('readonly', true);
                        $('#bankrecv2').prop('readonly', true);
                        $('#partyrecv2').prop('readonly', true);
                        $('#partyrecv').prop('readonly', true);
                        $('#bankrecv').prop('readonly', true);
                        $('#cashAmount').prop('readonly', true);
                        $('#CardDisc').prop('readonly', true);
                        $('#billDiscPercent').prop('readonly', true);
                        $("#Plus, #minus").prop("disabled", true);
                        $("#settlementInput").prop("readonly", true);
                        if (response.complete == 1)
                            $('#BACCOUNT').dxSelectBox('option', 'readOnly', true);
                        $('#billDiscValue').prop('readonly', true);
                        if (response.complete == 1) {

                            empr_POSTransaction.IntiAdvanceBankAct(parseInt(response.bacT_CODE));
                        }
                        $('#creditOption').prop('disabled', true)
                        $('.partyradioBtn input[type="radio"]').prop('disabled', true);
                        if (empr_POSTransaction.return == true) {
                            $('#btnCash').click();
                            $('#btnCard').prop('disabled', true);
                            $('#btnParty').prop('disabled', true);
                            $('#btnSplit').prop('disabled', true);
                        }

                    }
                    else {
                        $('#CardDisc').prop('readonly', false);
                        $('#billDiscPercent').prop('readonly', false);
                        $('#billDiscValue').prop('readonly', false);
                    }
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }

                let tableBody = document.getElementById('orderTable');
                Array.from(tableBody.rows).forEach(row => {
                    //  //  
                    let qtyInput = row.cells[2]?.querySelector('input');
                    let dtcodeInput = row.cells[13]?.querySelector('input');
                    if (qtyInput && dtcodeInput) {
                        const isReadonly = dtcodeInput.value && dtcodeInput.value.toString().trim() !== "0";
                        qtyInput.readOnly = isReadonly;
                    }
                });

            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
            empr_POSTransaction.dataLoaded = true;



        }, false, true);

    },

    selectTab(btn, idx) {
        // Tabs
        const payTabs = ['cashFields', 'cardFields', 'partyFields', 'splitFields'];
        document.querySelectorAll('.pay-tab').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.arrow').forEach(a => a.classList.remove('show'));
        btn.classList.add('active');
        document.getElementById('arr-' + idx).classList.add('show');

        // Sections
        payTabs.forEach(id => $('#' + id).addClass('d-none').removeAttr('hidden'));
        $('#' + payTabs[idx]).removeClass('d-none');

        if (payTabs[idx] === 'partyFields') {

            if (!empr_POSTransaction.billMode || empr_POSTransaction.billMode.trim() === "") {
                debugger
                empr_POSTransaction.selectRadioByLabel("Credit");
                empr_POSTransaction.updateHiddenField("creditOption");
                console.log("party field credit option clicked", empr_POSTransaction.billMode)
            }
        }
    },
    GetPOSTransactionDetailsByCode(code) {
        ajaxHelper.ajaxGetJson('/POSTransaction/GetPOSTransactionDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            } confirmSendBtn
        }, false, true);
    },

    Delete() {
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
            ajaxHelper.ajaxPostJsonData({ code: $('#Code').val() }, "/POSTransactions/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_POSTransaction.ResetForm();
                    $('#BtnDelete').hide();
                }
            }, false, true);
        });
    },

    InitDropdowns() {
        empr_POSTransaction.InitInvDDL();
        $('#INV_STATUS').dxSelectBox('instance').option('value', 'N');
        ajaxHelper.ajaxGetJson("/POSTransactions/GetBookTypes", function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.InitBookTypeDDL(data.data);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitDropdownsWithValue(booK_TYPE, inV_STATUS) {
        empr_POSTransaction.InitInvDDL(inV_STATUS);
        ajaxHelper.ajaxGetJson("/POSTransactions/GetBookTypes", function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.InitBookTypeDDL(data.data, booK_TYPE);
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    InitBookTypeDDL(dataSource, selectedValue) {
        $('#BOOK_TYPE').dxSelectBox({
            dataSource: dataSource,
            displayExpr: 'value',
            valueExpr: 'key',
            value: parseInt(selectedValue),
            searchEnabled: true,
            width: '100%',
            placeholder: 'Search',
            showClearButton: true,
            dropDownOptions: {
                height: 'auto',
            },
            pagingEnabled: true,
            searchTimeout: 500
        });
    },

    InitInvDDL(selectedValue) {

        var dataSource = [
            { key: 'N', value: 'Normal' },
            { key: 'H', value: 'Hold' },
            { key: 'C', value: 'Cancel' },
        ];

        $('#INV_STATUS').dxSelectBox({
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
            //onValueChanged: function (e) {
            //    if (e.value == 'Cr' || e.value == 'CrD') {
            //        $(".ConditionDiv").show();
            //    }
            //    else {
            //        $(".ConditionDiv").hide();
            //        $("#CreditDays").val('');
            //        $('#DueDate').val('');
            //    }
            //},
        });
    },

    InitReportTypeDDL(selectedValue) {
        ajaxHelper.ajaxGetJson("/POSTransaction/GetReportTypes", function (data) {
            if (data.msgType == 1) {
                if (data.data.length > 0) {
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
            else {
                empr_helper.notify(data.data, data.msgType);
            }
        }, false, true);
    },

    GeneratePrintReport() {

        //let TRAN_ID = $("#TRANS_ID").val();
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        //if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
        //    empr_helper.notify("Please open the delivery in edit mode.", 2);
        //    return;
        //}
        //var dataModel = {
        //    TRAN_ID: TRAN_ID,
        //    MD_ID: MD_ID,
        //}
        $('#MD_ID').val(MD_ID);
        var dataModel = empr_POSTransaction.GetDataToSave(MD_ID, "");
        ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/GetPrintReport", function (data) {

            if (data.msgType == 1) {
                setTimeout(function () {
                    $('#slipPreview').html(data.slipHtml);
                    $('#printModal').modal('show');
                }, 1000);

                $('#printModal').on('shown.bs.modal', function () {
                    setTimeout(function () {
                        var modalBody = $('#printModal .modal-body'); // Scroll the modal body, not the footer
                        console.log(modalBody[0].scrollHeight); // Check if scrollHeight is valid
                        if (modalBody[0].scrollHeight > modalBody.height()) {
                            modalBody.animate({
                                scrollTop: modalBody[0].scrollHeight
                            }, 200); // Scroll to the bottom of the modal body
                        } // Scroll to the bottom of the modal body
                    }, 100); // Delay to ensure the modal is fully rendered
                });

                //$('#ModalBody').empty();
                //setTimeout(function () {
                //    $('#ModalBody').html("<center><object id='objReport' data='" + window.location.origin + data.data + "' width='1100' height='600'></object></center>");
                //    $('#ShowReportModal').show();
                //    $('#ShowReportModal').modal('show');
                //}, 100);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    KOTPrint() {

        if (empr_POSTransaction.printStatus == "K") {
            const qrCodeImg = document.querySelector('#slipPreview img.qr-code');
            const srbcodeimg = document.querySelector('#slipPreview img.srb-code');
            if (qrCodeImg !== null && srbcodeimg !== null) {
                if (qrCodeImg.complete && srbcodeimg.complete) {
                    empr_POSTransaction.printContent();
                } else {
                    qrCodeImg.onload = () => {
                        if (srbcodeimg.complete) {
                            empr_POSTransaction.printContent();
                        }
                    };
                    srbcodeimg.onload = () => {
                        if (qrCodeImg.complete) {
                            empr_POSTransaction.printContent();
                        }
                    };
                }
            } else if (qrCodeImg !== null) {
                if (qrCodeImg.complete) {
                    empr_POSTransaction.printContent();
                } else {
                    qrCodeImg.onload = () => {
                        empr_POSTransaction.printContent();
                    };
                }
            } else if (srbcodeimg !== null) {
                if (srbcodeimg.complete) {
                    empr_POSTransaction.printContent();
                } else {
                    srbcodeimg.onload = () => {
                        empr_POSTransaction.printContent();
                    };
                }
            } else {
                empr_POSTransaction.printContent();
            }
            empr_POSTransaction.ResetAllFields();
        }

    },

    Calculate() {
        setTimeout(function () {
            var totalAmount = $('#detailContainer').dxDataGrid('instance').getTotalSummaryValue('AmountTotal');
            var netAmount = $('#detailContainer').dxDataGrid('instance').getTotalSummaryValue('NetTotal');
            var sett = parseFloat($('#SETT').val()) || 0;
            $('#TOTAL').val(totalAmount);
            $('#NET_TOTAL').val(netAmount);
            if (sett > 0) {
                $('#NET_TOTAL').val(Math.abs(netAmount) - sett);
            }
            if (netAmount < 0 && sett > 0) {
                $('#NET_TOTAL').val(-(Math.abs(netAmount) - sett));
            }
            var disc = Math.abs(totalAmount) - Math.abs(netAmount);
            $('#DISC').val(disc);
            empr_POSTransaction.CalculateCashBack();
        }, 500);
    },

    CalculateCashBack() {
        var cash = parseFloat($('#CASH').val());
        var netAmount = parseFloat($('#NET_TOTAL').val());
        if (netAmount < 0) {
            $('#CHANGE').val(Math.abs(netAmount));
            $('#RECV').val(0);
        } else if (cash >= netAmount) {
            $('#CHANGE').val(cash - netAmount);
            $('#RECV').val(netAmount);
        } else {
            $('#CHANGE').val(0);
            $('#RECV').val(0);
        }
    },

    highlightDiscountBox() {
        const $items = $('.discount-box.selectthisdiv:visible');
        $items.removeClass('highlighted');
        if ($items.length > 0 && selectedIndex < $items.length) {
            $items.eq(selectedIndex).addClass('highlighted');
        }
    },

    EnableShortCutKeys(saveElement, newElement, deleteElement, quickSearchElement, idElement, gridElement, rowIndex, dataField) {
        let selectedIndex = 0;
        const columns = 6;
        let selecteddiscIndex = 0;
        const discountcolumns = 4; // Ek row mein 6 items
        const focusableElements = '.quantity-input, .discountpercent, .row-checkbox';

        $("#BarcodePickModal").on("keydown", function (e) {

            if (e.key === 'Enter') {
                if ($('#barcode').val() == '') {
                    const gridElement = document.getElementById('BarcodePickGridContainer');
                    const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);

                    if (gridInstance) {
                        var selectedRows = gridInstance.getSelectedRowsData();

                        if (selectedRows.length > 0) {
                            setTimeout(() => {
                                e.preventDefault();
                                $("#BtnAddBarcodes").trigger('focus').trigger('click');
                                $items.eq(selectedIndex).removeClass('highlighted');
                            }, 500);

                        }
                        else {
                            e.preventDefault();
                            const $highlighted = $('.gridDiv.highlighted:visible');
                            if ($highlighted.length > 0) {
                                $highlighted.click();
                                setTimeout(() => {
                                    const grid = $('#BarcodePickGridContainer').dxDataGrid('instance');
                                    if (grid) {
                                        const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                                        if (cell && cell.length > 0) {
                                            cell.focus(); // Focus the top-left cell
                                        }
                                    }
                                }, 1000); // Delay ta'ke grid render ho jaye
                            }
                            return;
                        }

                    } else {
                        e.preventDefault();
                        const $highlighted = $('.gridDiv.highlighted:visible');
                        if ($highlighted.length > 0) {
                            $highlighted.click();
                            setTimeout(() => {
                                const grid = $('#BarcodePickGridContainer').dxDataGrid('instance');
                                if (grid) {
                                    const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                                    if (cell && cell.length > 0) {
                                        cell.focus(); // Focus the top-left cell
                                    }
                                }
                            }, 1000); // Delay ta'ke grid render ho jaye
                        }
                        return;
                    }
                }
                else {

                }
            }
        });


        $(document).keydown(function (e) {

            const searchBar = document.querySelector('.search-bar');
            const boxes = document.querySelectorAll('.middle .box');
            let debounceTimer;
            let isFirstLetter = true;
            let isRefreshed = false;
            let selectedIndexDelete = -1;
            const isModalOpen = $('.modal').is(':visible');

            //if ($('#CardDisc').is(':focus') && e.which === 13) {
            //    e.preventDefault(); // Stop default Enter behavior
            //    $('#CardEnter').click();
            //}

            let $firstVisibleRow = $('#orderTable tr').filter(function () {
                return $(this).find('.quantity-input:visible').length > 0;
            }).first();

            // ✅ Yeh event listener sirf ek dafa lagega
            //searchBar.addEventListener('input', () => {
            //    const searchText = searchBar.value.toLowerCase();

            //    if (searchText !== '') {
            //        boxes.forEach(box => {
            //            const text = box.querySelector('p').textContent.toLowerCase();
            //            box.style.display = text.includes(searchText) ? 'flex' : 'none';
            //        });
            //    }
            //    else {
            //        boxes.forEach(box => box.style.display = 'flex');
            //    }
            //});

            document.querySelector('#orderTable').addEventListener('input', function (e) {

                if (e.target.classList.contains('quantity-input') || e.target.classList.contains('total-price') || e.target.classList.contains('discountpercent')) {
                    const value = parseFloat(e.target.value);
                    const row = e.target.closest('tr');
                    const icon = row.querySelector('.row-checkbox');
                    const quantityInput = row.querySelector('.quantity-input');
                    const stockQty = parseFloat(row.querySelector('.stock').dataset.stockqty);
                    const barcodeId = row.querySelector('.BarcodeId').value;
                    const stockStatus = row.querySelector('#stockstatus').value;
                    const priceInput = row.querySelector('.total-price');
                    const discountcount = row.querySelector('.discountcount');

                    if (value < 0) {
                        if (icon) {
                            icon.classList.add('clicked');
                        }
                        if (e.target.classList.contains('quantity-input')) {
                            priceInput.value = -Math.abs(priceInput.value);
                        } else if (e.target.classList.contains('total-price')) {
                            quantityInput.value = -Math.abs(quantityInput.value);
                        }
                    } else {
                        if (icon) {
                            icon.classList.remove('clicked');
                        }
                        if (e.target.classList.contains('quantity-input')) {
                            priceInput.value = Math.abs(priceInput.value);
                        } else if (e.target.classList.contains('total-price')) {
                            quantityInput.value = Math.abs(quantityInput.value);
                        } else if (e.target.classList.contains('discountpercent')) {
                            discountcount.value = 0;
                        }
                    }
                    empr_POSTransaction.updateSummary();
                    if (stockStatus == 'Y') {
                        if (quantityInput.value <= stockQty) {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === barcodeId) {
                                    tableRow.querySelector('.stock').textContent = '';
                                }
                            });
                        }
                        else {
                            document.querySelectorAll(`#orderTable tr`).forEach(tableRow => {
                                const rowBarcode = tableRow.querySelector('.BarcodeId').value;
                                if (rowBarcode === barcodeId) {
                                    tableRow.querySelector('.stock').textContent = `HAS ONLY ${stockQty} IN STOCK`;
                                }
                            });
                            /*row.querySelector('.stock').textContent = `HAS ONLY ${stockQty} IN STOCK`;*/
                        }
                    }
                }

            });

            if (!isModalOpen) {
                if ((e.altKey || e.metaKey) && e.key === 'n') {
                    console.log('ALT+N is pressed');
                    e.preventDefault();
                    $(newElement).click();
                    return false;
                }

                if ((e.ctrlKey || e.metaKey) && e.key === 's') {
                    console.log('CTRL+S is pressed');
                    e.preventDefault();
                    $(saveElement).click();
                    return false;
                }

                if (e.ctrlKey && e.key.toLowerCase() === 'i') {
                    e.preventDefault();
                    const $checkbox = $('#SHOW_SELECTED');
                    $checkbox.prop('checked', !$checkbox.prop('checked')); // toggle
                    $checkbox.trigger('change');
                }

                if (e.ctrlKey && e.key.toLowerCase() === 'g') {
                    e.preventDefault();
                    const $checkbox = $('#SHOW_SELECTED');
                    $checkbox.prop('checked', !$checkbox.prop('checked')); // toggle
                    $checkbox.trigger('change');
                }

                if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
                    console.log('CTRL+F is pressed');
                    e.preventDefault();
                    $(quickSearchElement).click();
                    return false;
                }
                if (e.ctrlKey && e.key.toLowerCase() === 'e') {
                    e.preventDefault();
                    $('#expenseButton').click();
                }
                if (e.ctrlKey && e.key.toLowerCase() === 'h') {
                    e.preventDefault();
                    console.log('Ctrl + H pressed');
                    $('#historyBtn').trigger('click');
                }
                if (e.ctrlKey && e.key.toLowerCase() === 'd') {
                    e.preventDefault();
                    $('#DiscountBtn').click();
                    setTimeout(() => {
                        selecteddiscIndex = 0;
                        highlightDiscountBox();
                    }, 200);
                }
                if (e.key === 'F1') {
                    e.preventDefault();
                    $('#CMOB').focus();
                }
                if (e.key === 'F2') {
                    if (BarcodeTextBoxVisible) {
                        e.preventDefault();
                        $('.barcodevisible').focus();
                    }
                    else {
                        e.preventDefault();
                        $('.search-bar').focus();
                    }

                }
                if (e.key === 'F3') {
                    e.preventDefault();
                    $('#DiscardSale').click();
                }
                if (e.key === 'F4') {
                    e.preventDefault();
                    $('#BtnQuickSearch').click();
                    setTimeout(() => {
                        const grid = $('#gridContainer').dxDataGrid('instance');
                        if (grid) {
                            const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                            if (cell && cell.length > 0) {
                                cell.focus(); // Focus the top-left cell
                            }
                        }
                    }, 1000);
                }
                if (e.key === 'F5') {
                    e.preventDefault();
                    $('#kot').click();
                }
                if (e.key === 'F6') {
                    const isPaymentModalOpen = $('#paymentModal').is(':visible');
                    if (!isPaymentModalOpen) {
                        e.preventDefault();
                        $('#validateAndOpenModal').click();

                        setTimeout(() => {
                            const firstFocusable = $('#paymentModal').find('input:visible, button:visible, select:visible').first();
                            if (firstFocusable.length) {
                                firstFocusable.focus();
                            }
                        }, 100);
                    }
                    else {
                        e.preventDefault();
                        $('#payment').click();
                    }
                    // else: modal already open, let modal-specific handler handle it
                }

                if (e.key === 'F7') {
                    e.preventDefault();
                    $('#1').click();
                }
                if (e.key === 'F8') {
                    e.preventDefault();
                    $firstVisibleRow.find('.row-checkbox').click();
                }
                if (e.key === 'F9') {
                    e.preventDefault();
                    $firstVisibleRow.find('.quantity-input').focus();
                }
                if (e.key === 'F10') {
                    e.preventDefault();
                    $firstVisibleRow.find('.discountpercent').focus();
                }
                if (e.key === 'F11') {
                    e.preventDefault();
                    $('#PayQuickSearch').click();
                    setTimeout(() => {
                        const grid = $('#gridContainer').dxDataGrid('instance');
                        if (grid) {
                            const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                            if (cell && cell.length > 0) {
                                cell.focus(); // Focus the top-left cell
                            }
                        }
                    }, 1000);
                }
                if (e.key === 'Escape') {
                    e.preventDefault();
                    $firstVisibleRow.find('.delete-icon').click();
                }
                const $barcode = $('#barcode');
                const $searchBar = $('.search-bar');
                const $items = $('.gridDiv:visible');
                const $qrcode = $('.qrCodevisible');


                if ($items.length === 0) return;

                const isBarcodeModalOpen = $('#BarcodePickModal').is(':visible');

                if ($barcode.is(':focus') || $searchBar.is(':focus') || isBarcodeModalOpen || $qrcode.is(':focus')) {

                    if (e.key === 'ArrowDown') {
                        e.preventDefault();
                        const nextIndex = selectedIndex + columns;
                        if (nextIndex < $items.length) {
                            selectedIndex = nextIndex;
                        }
                    }
                    else if (e.key === 'ArrowUp') {
                        e.preventDefault();
                        const prevIndex = selectedIndex - columns;
                        if (prevIndex >= 0) {
                            selectedIndex = prevIndex;
                        }
                    }
                    else if (e.key === 'ArrowRight') {
                        e.preventDefault();
                        if (selectedIndex + 1 < $items.length) {
                            selectedIndex++;
                        }
                    }
                    else if (e.key === 'ArrowLeft') {
                        e.preventDefault();
                        if (selectedIndex - 1 >= 0) {
                            selectedIndex--;
                        }
                    }
                    else if (e.key === 'Enter') {
                        //  
                        if ($('#barcode').val() == '' && $qrcode.val() == '') {
                            const gridElement = document.getElementById('BarcodePickGridContainer');
                            const gridInstance = DevExpress.ui.dxDataGrid.getInstance(gridElement);

                            if (gridInstance) {
                                var selectedRows = gridInstance.getSelectedRowsData();

                                if (selectedRows.length > 0) {
                                    setTimeout(() => {
                                        e.preventDefault();
                                        $("#BtnAddBarcodes").trigger('focus').trigger('click');
                                        $items.eq(selectedIndex).removeClass('highlighted');
                                    }, 500);

                                }
                                else {
                                    e.preventDefault();
                                    const $highlighted = $('.gridDiv.highlighted:visible');
                                    if ($highlighted.length > 0) {
                                        $highlighted.click();
                                        setTimeout(() => {
                                            const grid = $('#BarcodePickGridContainer').dxDataGrid('instance');
                                            if (grid) {
                                                const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                                                if (cell && cell.length > 0) {
                                                    cell.focus(); // Focus the top-left cell
                                                }
                                            }
                                        }, 1000); // Delay ta'ke grid render ho jaye
                                    }
                                    return;
                                }

                            } else {
                                e.preventDefault();
                                const $highlighted = $('.gridDiv.highlighted:visible');
                                if ($highlighted.length > 0) {
                                    $highlighted.click();
                                    setTimeout(() => {
                                        const grid = $('#BarcodePickGridContainer').dxDataGrid('instance');
                                        if (grid) {
                                            const cell = grid.getCellElement(0, 0); // Row 0, Column 0

                                            if (cell && cell.length > 0) {
                                                cell.focus(); // Focus the top-left cell
                                            }
                                        }
                                    }, 1000); // Delay ta'ke grid render ho jaye
                                }
                                return;
                            }
                        }
                        else {
                            e.preventDefault(); // Prevent default behavior (like tab navigation)
                            //$('#barcodeEnter').click();
                            if (!$('#barcodeEnter').hasClass('disabled-btn')) {
                                $('#barcodeEnter').click();
                            }

                        }
                    }

                    // Highlight the selected item
                    $items.removeClass('highlighted');
                    $items.eq(selectedIndex).addClass('highlighted');
                }


            }

            const isPaymentModalOpen = true;

            if (e.key === 'F6') {

                if (isPaymentModalOpen) {
                    e.preventDefault();
                    $('#payment').click();
                }
                // else: modal already open, let modal-specific handler handle it
            }

            if (isPaymentModalOpen) {
                //  //  
                const currentActive = $(".paymentbtn.selected").attr("id");
                const paymentButtons = ["btnCash", "btnCard", "btnParty", "btnSplit"];
                let currentIndex = paymentButtons.indexOf(currentActive);

                // RIGHT ARROW - Next button
                if (e.key === "ArrowRight") {
                    e.preventDefault();
                    if (currentIndex < paymentButtons.length - 1) {
                        currentIndex++;
                    } else {
                        currentIndex = 0; // Wrap around (optional)
                    }
                    $(`#${paymentButtons[currentIndex]}`).trigger("click");
                }

                // LEFT ARROW - Previous button
                else if (e.key === "ArrowLeft") {
                    e.preventDefault();
                    if (currentIndex > 0) {
                        currentIndex--;
                    } else {
                        currentIndex = paymentButtons.length - 1; // Wrap around (optional)
                    }
                    $(`#${paymentButtons[currentIndex]}`).trigger("click");
                }

                // DOWN ARROW - Move to input/dropdown
                else if (e.key === "ArrowDown") {
                    const activeSection = $(".payment-section:not(.d-none)");

                    // Special case: Party button
                    if (currentActive === "btnParty") {
                        setTimeout(() => {
                            const dropdown = $('#partyName').dxSelectBox('instance');
                            if (dropdown) {
                                dropdown.focus(); // Focus the DevExtreme SelectBox
                            }
                        }, 300);
                    }

                    // Default: Focus first input or select
                    const firstInput = activeSection.find("input:visible:first, select:visible:first");
                    if (firstInput.length) {
                        firstInput.focus();
                    }
                }

                else if (e.key === 'Escape') {
                    var modal = document.getElementById('paymentModal');
                    if (modal.classList.contains('show')) {
                        e.preventDefault(); // default behavior roko agar koi aur hai
                        let modalInstance = bootstrap.Modal.getInstance(modal);
                        modalInstance.hide(); // apne code se close karo
                    }
                }
            }

            const isDiscountModalOpen = $('#discountModal').is(':visible');

            if (isDiscountModalOpen) {

                const $items = $('.discount-box.selectthisdiv:visible');
                if ($items.length === 0) return;

                if (e.key === 'ArrowRight') {
                    e.preventDefault();
                    if (selecteddiscIndex + 1 < $items.length) selecteddiscIndex++;
                } else if (e.key === 'ArrowLeft') {
                    e.preventDefault();
                    if (selecteddiscIndex - 1 >= 0) selecteddiscIndex--;
                } else if (e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = selecteddiscIndex + discountcolumns;
                    if (nextIndex < $items.length) selecteddiscIndex = nextIndex;
                } else if (e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = selecteddiscIndex - discountcolumns;
                    if (prevIndex >= 0) selecteddiscIndex = prevIndex;
                } else if (e.key === 'Enter') {
                    e.preventDefault();
                    $items.eq(selecteddiscIndex).trigger('click');
                }

                highlightDiscountBox();
            }

            const $modal = $('.swal2-modal.swal2-show');

            if ($modal.length > 0) {
                const $confirmBtn = $modal.find('.swal2-confirm');
                const $cancelBtn = $modal.find('.swal2-cancel');
                const isConfirmFocused = document.activeElement === $confirmBtn[0];
                const isCancelFocused = document.activeElement === $cancelBtn[0];

                // On Right Arrow → focus Cancel
                if (e.key === 'ArrowRight') {
                    e.preventDefault();
                    if (isConfirmFocused) {
                        $cancelBtn.focus();
                    } else {
                        $confirmBtn.focus();
                    }
                }

                // On Left Arrow ← focus Confirm
                else if (e.key === 'ArrowLeft') {
                    e.preventDefault();
                    if (isCancelFocused) {
                        $confirmBtn.focus();
                    } else {
                        $cancelBtn.focus();
                    }
                }

                // On Enter → Click focused button
                else if (e.key === 'Enter') {
                    e.preventDefault();
                    if (isConfirmFocused) {
                        $confirmBtn.trigger('click');
                    } else if (isCancelFocused) {
                        $cancelBtn.trigger('click');
                    }
                }

                // Set initial focus on Cancel for UX (optional)
                if (!$confirmBtn.is(':focus') && !$cancelBtn.is(':focus')) {
                    $cancelBtn.focus();
                }
            }

            if (e.key === 'Escape') {
                const isAnyModalOpen = $('.modal').toArray().some(modal => {
                    const $modal = $(modal);
                    return $modal.css('display') === 'flex' || $modal.hasClass('show');
                });
                if (isAnyModalOpen) {
                    var modal = document.getElementById('colorModal');
                    if (!modal.classList.contains('show')) {
                        e.preventDefault(); // default behavior roko agar koi aur hai
                        $('.closed').click();
                    }
                    else {

                        e.preventDefault();
                        $('.close').click();
                    }
                }
            }

        });

        //if (window.location.href.includes('POSTransactions')) {
        //    const checkElement = setInterval(function () {
        //        const toggleButton = $('.toggle_sidebar');
        //        if (toggleButton.length > 0) {
        //            clearInterval(checkElement); // Stop checking once found
        //            toggleButton.trigger('click'); // Trigger the click event
        //            console.log("Sidebar toggle clicked.");
        //        }
        //    }, 100); // Check every 100ms
        //}

        function highlightDiscountBox() {
            const $items = $('.discount-box.selectthisdiv:visible');
            $items.removeClass('highlighteds');
            if ($items.length > 0 && selecteddiscIndex < $items.length) {
                $items.eq(selecteddiscIndex).addClass('highlighteds');
            }
        }
    },

    openVoucherPage(link, tranID) {
        var newWindow = window.open(link, '_blank');

        newWindow.addEventListener('load', function () {
            try {
                if (newWindow.empr_POSTransaction) {
                    newWindow.empr_POSTransaction.GetPOSTransactionByCode(tranID);
                } else {
                    empr_helper.notify("Something went wrong. Please try again.", 2);
                }
            } catch (e) {
                console.error("Error accessing newWindow:", e);
            }
        });
    },

    ThanksMsg() {
        //  //  
        var CMOB = '';
        var CNAME = '';
        var totalPayment = $('#TotalPaymenthidden').val()

        if (empr_POSTransaction.payType == "Party") {
            var partyObj = empr_POSTransaction.Partydata.filter(x => x.code == empr_POSTransaction.selectedParty.actCode && x.key == empr_POSTransaction.selectedParty.partyCode);
            console.log('partyObj', partyObj);
            CMOB = partyObj[0]?.cell;
            CNAME = partyObj[0]?.value;
        }
        else {
            CMOB = $("#CMOB").val();
            CNAME = $("#CNAME").val();
        }

        var WHT_URL = $('#WHT_URL').val();
        var WHT_TOKEN = $('#WHT_TOKEN').val();
        var WHT_MSG = '';


        if (empr_POSTransaction.payType == 'Advance' && $('#complete').val() == 0) {
            WHT_MSG = $('#WHT_MSG_ADV').val();
        }
        else if (empr_POSTransaction.payType == 'Advance' && $('#complete').val() == 1) {
            WHT_MSG = $('#WHT_ADV_COM').val();
        }
        else if (totalPayment.includes('-') || empr_POSTransaction.return == true) {
            WHT_MSG = $('#WHT_MSG_RETURN').val();
        }
        else if (empr_POSTransaction.payType == "Party") {
            WHT_MSG = $('#WHT_PARTY_MSG').val();
        }
        else {
            WHT_MSG = $('#WHT_CC_MSG').val();
        }
        //console.log('message', WHT_MSG);
        var TRAN_ID = empr_POSTransaction.code;

        if (WHT_URL != '' && CMOB != '' && TRAN_ID != 0) {
            if (empr_POSTransaction.payType == "Party") {
                $('#messageText').val("");
                $('#messageText').val(
                    "Hello, Dear " + CNAME + ",\n\n" +
                    WHT_MSG + "\n\n" +
                    "Thank you!\n\n" +
                    "Best Regards,\n" +
                    "[CompanyName]\n\n" +
                    "Contact:\n" +
                    "[Number]\n" +
                    "[Email]"
                );
            }
            else {
                $('#messageText').val("");
                $('#messageText').val(
                    "Hello, Dear [Customer Name],\n\n" +
                    WHT_MSG + "\n\n" +
                    "Thank you!\n\n" +
                    "Best Regards,\n" +
                    "[CompanyName]\n\n" +
                    "Contact:\n" +
                    "[Number]\n" +
                    "[Email]"
                );
            }

            const message = $('#messageText').val().trim();

            const payload = [{
                name: CNAME,
                mobile: CMOB,
                message: message,
                WhatsappUrl: WHT_URL,
                WhatsappToken: WHT_TOKEN,
                TRAN_ID: TRAN_ID,
            }];

            console.log('payload', payload);

            $.ajax({
                url: '/POSTransactions/SendWhatsapp',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(payload),
                success: function (response) {
                    if (response.success == true) {
                        console.log(response.url);
                        empr_helper.notify("Messages sent successfully!", 1);
                    }
                    else {
                        console.log(response);
                    }
                },
            });
        }
    },

    GetUserRights: function () {
        //  //  
        ajaxHelper.ajaxGetJson("/POSTransactions/GetUserRights", function (data) {
            console.log('user rights', data);
            //  //  
            empr_POSTransaction.userRights = data.data[0];
            empr_POSTransaction.SetUserRights();

        }, false, true);

    },

    SetUserRights: function () {
        //  //  

        var rights = empr_POSTransaction.userRights;
        if (!rights) return;

        if (rights) {
            if (rights.mDisc === 1) {
                $('#billDiscPercent').prop('disabled', true);
                $('#billDiscValue').prop('disabled', true);
            } else {
                $('#billDiscPercent').prop('disabled', false);
                $('#billDiscValue').prop('disabled', false);
            }

            if (rights.rate === 1) {
                $('.total-price').prop('readonly', true);
            } else {
                $('.total-price').prop('readonly', false);
            }

            if (rights.dDisc === 1) {
                $('.discountpercent').prop('readonly', true);

            } else {
                $('.discountpercent').prop('readonly', false);
            }

            if (rights.cardDisc === 1) {
                $('#CardDisc').prop('disabled', true);
            } else {
                $('#CardDisc').prop('disabled', false);
            }

            if (rights.exp === 1) {
                $('#expenseButton').addClass('disabled-btn');
            } else {
                $('#expenseButton').removeClass('disabled-btn');
            }

            if (rights.comm === 1) {
                $('#COMM').prop('readonly', true);
            } else {
                $('#COMM').prop('readonly', false);
            }

            if (rights.serviceCharges === 1) {
                $('#serviceChargesPercent').prop('disabled', true);
                $('#serviceChargesValue').prop('disabled', true);
            } else {
                $('#serviceChargesPercent').prop('disabled', false);
                $('#serviceChargesValue').prop('disabled', false);
            }
        }
        if (rights.serviceCharges === 1) {
            $('#serviceChargesPercent').prop('disabled', true);
            $('#serviceChargesValue').prop('disabled', true);
        } else {
            $('#serviceChargesPercent').prop('disabled', false);
            $('#serviceChargesValue').prop('disabled', false);
        }




    },

    openPrinterSetting(showPay, showPre, showDup) {
         
        // Hide all fields first
        $("#divPaySetting").hide();
        $("#divPreSetting").hide();
        $("#divDupSetting").hide();

        // Show based on condition
        if (showPay === true) {
            $("#divPaySetting").show();
             
            $("#paySetting").val(empr_POSTransaction.isPayQty);
        }

        if (showPre === true) {
            $("#divPreSetting").show();
        }

        if (showDup === true) {
            $("#divDupSetting").show();
        }

        // Open Modal
        $("#PrinterSetting").modal("show");
    }
}