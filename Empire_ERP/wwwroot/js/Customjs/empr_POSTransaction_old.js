
var empr_POSTransaction = {
    totalCount: 0,
    rowsCount: 0,
    Partydata: [],
    RT_TYPES: new DevExpress.data.ArrayStore({
        key: "key",
        data: [
            { key: 1, value: '1 KG' },
            { key: 40, value: '40 kG' },
        ]
    }),
    Branch_RT_TYPE: '',

    InitEvents() {
        $(document).ready(function () {

            $('body').on('click', '.gridDiv', function () {
                var ItemId = $(this).data('item');
                empr_POSTransaction.loadItemsForData(ItemId);
            });


            PaymentMode();

            TodayDate();

            POSDesignScript();

            DynamicIcon();

            if (ItemsGroup && ItemsGroup.length > 0) {
                debugger;

                ItemsGroup.forEach(function (item, index) {
                    // Generate the card HTML dynamically
                    var cardHtml = `
                        <div class="box selectdiv" data-item="${item.grouP_CODE}">
                            <img src="${item.ipic}" alt="">
                            <p>
                                ${item.grouP_NAME}
                            </p>
                        </div>
                    `;

                    // Append the generated card to the #cards-container
                    $('.Top-left-body').append(cardHtml);

                    if (index === 0) {
                        loadItemsForGroup(item.grouP_CODE); // Load items for the first group by default
                    }
                });

                // Add click event listener for the dynamically generated cards
                $('.Top-left-body').on('click', '.selectdiv', function () {
                    debugger;
                    var groupId = $(this).data('item');
                    loadItemsForGroup(groupId);
                });
            } else {
                $('.Top-left-body').html('<p>No items to display.</p>'); // Display fallback message if no data
            }


            function loadItemsForGroup(groupId) {

                ajaxHelper.ajaxGetJson('/POSTransactions/GetItemsMasterByGroup?groupId=' + groupId, function (data) {
                    if (data.length > 0) {
                        $('#items-container').html('');
                        $('.middle').empty();
                        var itemsHtml = $.map(data, function (item) {
                            return `
                                    <div class="box gridDiv" data-item="${item.iteM_CODE}">
                                    <img src="${item.ipic}" alt="">
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
                });

            }

            function updateSummary() {
                debugger;
                const table = document.getElementById('tableGrid');
                const rows = table.getElementsByTagName('tbody')[0].rows;
                let totalTax = 0;
                let totalAmount = 0;
                let totalQuantity = 0;
                let totalItems = rows.length;
                let totalItemsDiscval = 0;
                let discountPercent = 0;

                // Loop through rows to sum up values
                for (let i = 0; i < rows.length; i++) {
                    const tax = parseInt(rows[i].cells[1].querySelector('input').value);
                    const quantity = parseInt(rows[i].cells[2].querySelector('input').value);
                    const amount = quantity * parseFloat(rows[i].cells[3].querySelector('input').value);
                    const discper = parseFloat(rows[i].cells[5].querySelector('input').value);
                    const disval = (discper / 100) * amount;
                    const taxamount = (tax / 100) * amount;

                    totalTax += taxamount;
                    totalAmount += amount;
                    totalQuantity += quantity;
                    totalItemsDiscval += disval;
                }

                // Update total values in summary
                document.getElementById('totalItems').textContent = `Total Items: ${totalItems}`;
                document.getElementById('totalQty').textContent = `Total Qty: ${totalQuantity}`;
                document.getElementById('subTotal').textContent = `Sub Total: ${totalAmount}`;
                document.getElementById('totalTax').textContent = `Total Tax: ${totalTax}`;

                document.getElementById('subtotalTax').textContent = totalTax;
                var Amount = (totalTax + totalAmount);
                document.getElementById('totalAmount').textContent = Amount;
                document.getElementById('itemDiscount').textContent = totalItemsDiscval;
                var TotalAmountGet = (Amount - totalItemsDiscval);
                $('#TotalbillDisc').val(TotalAmountGet);
                var deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;
                if (Amount == 0) {
                    discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
                    $('#billDiscPercent').val(0);
                    $('#billDiscValue').val(0);
                }
                discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
                if (discountPercent == 0 && deliveryPercent == 0) {
                    $('#FinalAmount').html(`Value ${TotalAmountGet} /-`);
                    $('#totalPayment').text(TotalAmountGet);
                    $('#cashAmount').val(TotalAmountGet);
                    $('#recvhidden').val(TotalAmountGet);
                }
                else {
                    var discountAmount = parseInt($('#billDiscValue').val()) || 0; // Ensure discountAmount is a number
                    var withDicsAmount = TotalAmountGet - discountAmount; // Subtract discount
                    $('#totalAmountGet').val(withDicsAmount);

                    var deliveryAmount = parseInt($('#billDelValue').val()) || 0; // Fixed: Removed extra parentheses
                    var finalAmount = withDicsAmount + deliveryAmount;
                    var TotalValue = parseInt(finalAmount);
                    $('#FinalAmount').html(`Value ${TotalValue} /-`);  // Update the final amount display
                    $('#totalPayment').text(TotalValue); // Update total payment display
                    $('#cashAmount').val(TotalValue); // Update cash amount input
                    $('#recvhidden').val(TotalValue); // Update hidden field with final amount

                }




            }

            function PaymentMode() {


                let typingTimer; // Timer for debounce
                const typingInterval = 1000;// Replace with dynamic total if needed
                const typingInterval2 = 1000;// Replace with dynamic total if needed

                // Monitor changes in all relevant input fields
                $("#cashAmount2, #bankrecv2, #partyrecv2").on("input", function () {
                    const totalAmount = $('#totalPayment').text();
                    clearTimeout(typingTimer); // Clear previous timer
                    let input = $(this);
                    debugger;
                    // Parse values from each input field
                    typingTimer = setTimeout(function () {
                        debugger;
                        let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
                        let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
                        let partyAmount = parseFloat($("#partyrecv2").val()) || 0;

                        // Calculate remaining amount
                        let totalEntered = cashAmount + cardAmount + partyAmount;
                        let remaining = totalEntered - totalAmount;

                        let amount = 0;
                        let baqia = 0;

                        // Validate and adjust amounts
                        if ($('#totalPayment').text() < totalEntered) {
                            if (input.attr("id") === "cashAmount2") {
                                amount = cardAmount + partyAmount;
                                baqia = totalAmount - amount;
                                if (amount < totalAmount) {
                                    input.val(baqia);
                                }
                            } else if (input.attr("id") === "bankrecv2") {
                                amount = cashAmount + partyAmount;
                                baqia = totalAmount - amount;
                                if (amount < totalAmount) {
                                    input.val(baqia);
                                }
                            } else if (input.attr("id") === "partyrecv2"){
                                amount = cashAmount + cardAmount;
                                baqia = totalAmount - amount;
                                if (amount < totalAmount) {
                                    input.val(baqia);
                                }
                            } 

                        } else {
                            // Update placeholder text for remaining fields
                            $("#cashAmount2").attr("placeholder", `Max: ${remaining}`);
                            $("#bankrecv2").attr("placeholder", `Max: ${remaining}`);
                            $("#partyrecv2").attr("placeholder", `Max: ${remaining}`);
                        }
                    }, typingInterval2);
                });


                $("#cashAmount").on("input", function () {
                    clearTimeout(typingTimer); // Clear previous timer
                    let input = $(this);
                    typingTimer = setTimeout(function () {
                        let value = parseFloat(input.val());
                        let amount = parseFloat($('#totalPayment').text());

                        // Validate after user finishes typing
                        if (value < amount) {
                            input.val(amount); // Set it to the max allowable amount
                        }
                    }, typingInterval);
                });

                $('.paymentbtn').on('click', function () {
                    $('.paymentbtn').removeClass('selected');

                    $(this).addClass('selected');
                });

                // Select buttons and payment sections
                const btnCash = document.getElementById("btnCash");
                const btnCard = document.getElementById("btnCard");
                const btnParty = document.getElementById("btnParty");
                const btnSplit = document.getElementById("btnSplit");

                const cashFields = document.getElementById("cashFields");
                const cardFields = document.getElementById("cardFields");
                const partyFields = document.getElementById("partyFields");
                const splitFields = document.getElementById("splitFields");

                // Helper function to hide all sections
                function hideAllFields() {
                    cashFields.classList.add("d-none");
                    cardFields.classList.add("d-none");
                    partyFields.classList.add("d-none");
                    splitFields.classList.add("d-none");
                }
                
                // Event listeners for buttons
                btnCash.addEventListener("click", () => {
                    hideAllFields();
                    ResetAllFields();
                    cashFields.classList.remove("d-none");
                });

                btnCard.addEventListener("click", () => {
                    hideAllFields();
                    ResetAllFields();
                    $('#bankrecv').val($('#totalPayment').text());
                    cardFields.classList.remove("d-none");
                });

                btnParty.addEventListener("click", () => {
                    hideAllFields();
                    ResetAllFields();
                    $('#partyrecv').val($('#totalPayment').text());
                    partyFields.classList.remove("d-none");
                });

                btnSplit.addEventListener("click", () => {
                    hideAllFields();
                    ResetAllFields();
                    splitFields.classList.remove("d-none");
                });

                ajaxHelper.ajaxGetJson('/POSTransactions/GetBanksName', function (data) {
                    debugger;
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
                    debugger;
                    empr_POSTransaction.Partydata = data;
                    empr_POSTransaction.bindDxDdl("partyName", data, null, "key", "value", "Select", function (d) {
                        var SelectedId = d.value;
                        var selectedItem = data.find(item => item.key === SelectedId);
                        var selectedText = selectedItem ? selectedItem.value : "";
                        var partycode = selectedItem ? selectedItem.code : "";
                        $('#partyhidden').val(d.value);
                        $('partynamehidden').val(selectedText);
                        $('#partycode').val(partycode);
                    });
                    empr_POSTransaction.bindDxDdl("partyName2", data, null, "key", "value", "Select", function (d) {
                        var SelectedId = d.value;
                        var selectedItem = data.find(item => item.key === SelectedId);
                        var selectedText = selectedItem ? selectedItem.value : "";
                        var partycode = selectedItem ? selectedItem.code : "";
                        $('#partyhidden2').val(d.value);
                        $('partynamehidden2').val(selectedText);
                        $('#partycode2').val(partycode);
                    });
                });

                ajaxHelper.ajaxGetJson('/POSTransactions/GetAcountName', function (data) {
                    debugger;
                    empr_POSTransaction.bindDxDdl("AcountName", data, null, "key", "value", "Select", function (d) {
                        $('#Acounthidden').val(d.value);
                    });
                    empr_POSTransaction.bindDxDdl("AcountName2", data, null, "key", "value", "Select", function (d) {
                        $('#Acounthidden2').val(d.value);
                    });
                });

                ajaxHelper.ajaxGetJson("/POSTransactions/GetReportTypes", function (data) {
                    debugger
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


            }

            function ResetAllFields() {
                $('#cashAmount').val('');
                const dropdownInstanceAcount = $("#AcountName").dxSelectBox("instance");
                dropdownInstanceAcount.reset();
                const dropdownInstanceparty = $("#partyName").dxSelectBox("instance");
                dropdownInstanceparty.reset();
                const dropdownInstancebank = $("#bankName").dxSelectBox("instance");
                dropdownInstancebank.reset();
                const dropdownInstanceAcount2 = $("#AcountName2").dxSelectBox("instance");
                dropdownInstanceAcount2.reset();
                const dropdownInstanceparty2 = $("#partyName2").dxSelectBox("instance");
                dropdownInstanceparty2.reset();
                const dropdownInstancebank2 = $("#bankName2").dxSelectBox("instance");
                dropdownInstancebank2.reset();
                $('#Acounthidden').val('');
                $('#Acounthidden2').val('');
                $('#cashAmount2').val('');
                $('#cashReturn').val('');
                $('#cashremark').val('');
                $('#bankhidden').val('');
                $('#bankhidden2').val('');
                $('#banknamehidden2').val('');
                $('#banknamehidden').val('');
                $('#Returnbank').val('');
                $('#remarkbank').val('');
                $('#partyhidden').val('');
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
            }


            function DynamicIcon() {
                ajaxHelper.ajaxGetJson('/POSTransactions/GetDynamicIcon', function (data) {
                    if (data.length > 0) {
                        debugger;
                        var itemsHtml = $.map(data, function (item) {
                            return `
                                     <div id="${item.grouP_CODE}" class="icondiv" style="cursor: pointer;">
                                     <img src="${item.gpic}" style="height:50px ; width:60px ;"/>
                                     <br> ${item.grouP_NAME}</div>
                            `;
                        });
                        $('.headers').append(itemsHtml);

                    } else {
                        $('.headers').html('<p>No items available for this group.</p>');
                    }
                });
            }


            $('.headers').on('click', '.icondiv', function () {
                debugger;
                // Get the clicked group's ID
                var iconId = $(this).attr('id');
                $('#INV_STATUS').val(iconId);
                empr_POSTransaction.DeleveryInputvisible(iconId);
                empr_POSTransaction.HoldDataSave(iconId);
            });

            //$("#toggleButton,#DineInBtn").click(function () {
            //    $("#inputContainer").toggle();// Hide or show the input container
            //});
            //$("#DineInBtn,#TakeAway,#DrieThrough,#Hold").click(function () {
            //    $("#inputContainer").hide();// Hide or show the input container
            //});

            $('body').on('click', '#DiscountBtn', function () {
                debugger;
                // Clear previous data from modal to prevent duplication
                $('.modalData').empty();

                // AJAX call to fetch discounts
                ajaxHelper.ajaxGetJson('/POSTransactions/GetAllDiscount', function (data) {
                    debugger;

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
            });

            // Handle click event on dynamically created discount items
            $('body').on('click', '.modalData .selectthisdiv', function () {
                debugger;

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
            });


            document.querySelectorAll(".close").forEach(function (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    document.getElementById("discountModal").style.display = "none";
                    $('#editItemModal').modal('hide');
                });
            });

            // Search functionality
            $('#search-item').on('keyup', function () {
                var searchValue = $(this).val().toLowerCase();

                $('.search').filter(function () {
                    var itemName = $(this).find('.mid-itemname').text().toLowerCase();
                    $(this).toggle(itemName.includes(searchValue));
                });
            });
            $('#listView').on('click', function () {
                debugger;
                $('#items-container').removeClass('grid-view').addClass('list-view');
                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);
            });
            $('#gridView').on('click', function () {
                debugger;
                $('#items-container').removeClass('list-view').addClass('grid-view');
                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);

            });
            $('#getitems').on('click', function () {
                debugger;
                var groupId = localStorage.getItem('groupId');
                loadItemsForGroup(groupId);

            });
            $('#DiscardSale').on('click', function () {
                $('#orderTable').empty();
                $('#billDelPersent').val(0);
                $('#billDelValue').val(0);
                empr_POSTransaction.updateSummary();
            });
            $('#validateAndOpenModal').on('click', function () {
                debugger;

                ResetAllFields();
                $('#cashAmount').val($('#totalPayment').text());
                const orderTable = document.getElementById("orderTable");

                if (orderTable.rows.length > 0) {
                    const modal = new bootstrap.Modal($("#paymentModal"));
                    modal.show();

                } else {
                    var msg = "Items table is empty. Please add at least one item before proceeding.";
                    empr_helper.notify(msg, 2);
                }
            });

            $("#billDelPercent, #billDelValue").on("change", function () {
                debugger;
                let totalAmount = parseFloat($('#totalAmountGet').val()) || 0;
                let percentInput = $("#billDelPercent");
                let valueInput = $("#billDelValue");
                if ($(this).attr("id") === "billDelPercent") {
                    // If user changes the percentage input
                    let percentage = parseFloat(percentInput.val()) || 0;
                    let calculatedValue = Math.round((totalAmount * percentage) / 100);
                    valueInput.val(calculatedValue); // Update the value input
                } else if ($(this).attr("id") === "billDelValue") {
                    // If user changes the value input
                    let value = parseFloat(valueInput.val()) || 0;
                    let calculatedPercent = Math.round((value / totalAmount) * 100);
                    percentInput.val(calculatedPercent); // Update the percentage input
                }
                empr_POSTransaction.updateSummary();

            });
            $("#discountPercentage, #discountPerUnit").on("change", function () {
                debugger;
                let totalAmount = parseFloat($('#amount').val()) || 0;
                let percentInput = $("#discountPercentage");
                let valueInput = $("#discountPerUnit");
                if ($(this).attr("id") === "discountPercentage") {
                    // If user changes the percentage input
                    let percentage = parseFloat(percentInput.val()) || 0;
                    let calculatedValue = Math.round((totalAmount * percentage) / 100);
                    valueInput.val(calculatedValue); // Update the value input
                } else if ($(this).attr("id") === "discountPerUnit") {
                    // If user changes the value input
                    let value = parseFloat(valueInput.val()) || 0;
                    let calculatedPercent = Math.round((value / totalAmount) * 100);
                    percentInput.val(calculatedPercent); // Update the percentage input
                }
                empr_POSTransaction.updateSummary();

            });
            $("#billDiscPercent, #billDiscValue").on("change", function () {
                debugger;
                let totalAmount = parseFloat($('#TotalbillDisc').val()) || 0;
                let percentInput = $("#billDiscPercent");
                let valueInput = $("#billDiscValue");
                if ($(this).attr("id") === "billDiscPercent") {
                    // If user changes the percentage input
                    let percentage = parseFloat(percentInput.val()) || 0;
                    let calculatedValue = Math.round((totalAmount * percentage) / 100);
                    valueInput.val(calculatedValue); // Update the value input
                } else if ($(this).attr("id") === "billDiscValue") {
                    // If user changes the value input
                    let value = parseFloat(valueInput.val()) || 0;
                    let calculatedPercent = Math.round((value / totalAmount) * 100);
                    percentInput.val(calculatedPercent); // Update the percentage input
                }
                empr_POSTransaction.updateSummary();

            });
            $('#BtnQuickSearch').on('click', function () {
                debugger;
                empr_POSTransaction.InitQuickSearchGrid();

            });

            $('#kot').on('click', function () {
                debugger;
                $('#BILL_STATUS').val('K');
                $('#recvhidden').val('');
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        //if (empr_POSTransaction.ValidateInfo()) {
                        //    empr_POSTransaction.SaveInfo();
                        //}
                        empr_POSTransaction.SaveInfo();
                    }
                } else {
                    //if (empr_POSTransaction.ValidateInfo()) {
                    //    empr_POSTransaction.SaveInfo();
                    //}
                    empr_POSTransaction.SaveInfo();
                    empr_POSTransaction.updateSummary();

                }

            });
            $('#payment').on('click', function () {
                debugger;
                let cashAmount = parseFloat($("#cashAmount2").val()) || 0;
                let cardAmount = parseFloat($("#bankrecv2").val()) || 0;
                let partyAmount = parseFloat($("#partyrecv2").val()) || 0;

                // Calculate remaining amount
                let totalEntered = cashAmount + cardAmount + partyAmount;
                let totalAmount = $('#totalPayment').text();

                debugger;

                if (Checkvalidation()) {

                    if (totalAmount <= totalEntered || $('#cashAmount').val() != '' || $('#bankrecv').val() != '' || $('#partyrecv').val() != '') {
                        $('#BILL_STATUS').val('P');
                        if (Permissions != "Admin") {
                            if (!$("#Code").val() && !Permissions.r_ADD) {
                                empr_helper.notify("You are not allowed to add new record !", 2);
                            }
                            else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                                empr_helper.notify("You are not allowed to edit records !", 2);
                            } else {
                                //if (empr_POSTransaction.ValidateInfo()) {
                                //    empr_POSTransaction.SaveInfo();
                                //}
                                empr_POSTransaction.SaveInfo();
                            }
                        } else {
                            //if (empr_POSTransaction.ValidateInfo()) {
                            //    empr_POSTransaction.SaveInfo();
                            //}
                            empr_POSTransaction.SaveInfo();
                            empr_POSTransaction.updateSummary();

                        }
                        //empr_POSTransaction.GenerateSlip();
                        $('.PaymentModalClose').click();    
                        empr_POSTransaction.ResetForm();
                        ResetAllFields();
                    }
                    else {
                        var msg = "The total amount is greater than the entered amount. Please make sure all required fields are filled correctly.";
                        empr_helper.notify(msg, 2);
                    }
                    
                    
                }
                else {
                    var msg = "Please Select Required Fields";
                    empr_helper.notify(msg, 2);
                }
            });

            


            $('body').on('click', '.elm_edit', function () {
                var id = $(this).attr("reportid");
                $('#Code').val(id);
                $('.modal').modal('hide');
                empr_POSTransaction.GetPOSTransactionByCode(id);
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
                debugger;

                printInvoice();
                closeModal();
            });
            $('#CMOB').on('input', function () {
                var number = $('#CMOB').val();
                var user = Users.filter(b => b.number == number);
                if (user.length > 0) {
                    var item = user[0];
                    $('#CMOB').val(item.name);
                }
            });

            $('#printButton').on('click', function () {
                debugger;

                // Get the content to print
                const printContent = document.getElementById('slipPreview').innerHTML;

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
                                                            width: auto;
                                                        }
                                                        table {
                                                            border-collapse: collapse;
                                                        }
                                                        th, td {
                                                            padding: 3px;
                                                        }
                                                        tbody tr:nth-last-child(2) {
                                                            border-bottom: 0.5px solid black;
                                                        }
                                                    </style>
                                                </head>
                                            <body>
                                            ${printContent}
                                            </body>
                                        </html>
                                    `);
                frameDoc.document.close();

                // Trigger print in the iframe
                frameDoc.focus(); // Focus on the iframe
                frameDoc.print(); // Trigger the print

                // Remove the iframe after printing
                setTimeout(() => {
                    document.body.removeChild(printFrame);
                }, 1000);
            });


            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#BtnNew').hide();
                !Permissions.r_VIEW && $('#BtnQuickSearch').hide();
                !Permissions.r_PRINT && $('#BtnPrint').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#BtnSave').hide();
            };


            function TodayDate() {
                var today = new Date();
                $('#V_DATE').text(today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear());
            };
            function POSDesignScript() {
                // Show the modal and populate fields dynamically
                function openEditModal(itemData) {
                    document.getElementById('modalTitle').innerText = itemData.name;
                    document.getElementById('itemQuantity').value = itemData.quantity;
                    document.getElementById('discountPercentage').value = itemData.discountpersentage;
                    document.getElementById('discountPerUnit').value = itemData.discountAmount;
                    document.getElementById('itemUnit').value = itemData.unit;
                    document.getElementById('amount').value = itemData.amount;
                    document.getElementById('remarks').value = itemData.remarks;

                    // Show the modal
                    $('#editItemModal').modal('show');
                }

                // Example: On clicking edit button
                document.addEventListener('click', function (e) {
                    if (e.target.closest('.delete-icon')) {
                        // Find the parent row of the clicked delete icon
                        let rowToDelete = e.target.closest('tr');
                        rowToDelete.remove();
                        empr_POSTransaction.updateSummary();

                    }
                    if (e.target.closest('.edit-icon')) {
                        debugger;
                        let row = e.target.closest('tr');
                        let itemData = {
                            name: row.cells[0].innerText.split(' - ')[0],
                            quantity: row.querySelector('.quantity-input').value,
                            discountpersentage: row.querySelector('.discountpercent').value,
                            discountAmount: row.querySelector('.discountvalue').value,
                            unit: row.querySelector('.unit').value, 
                            rate: parseFloat(row.querySelector('.total-price').value),
                            amount: parseFloat(row.querySelector('.quantity-input').value) * parseFloat(row.querySelector('.total-price').value) || 0,
                            remarks: ""
                        };
                        openEditModal(itemData);

                    }
                });
                function updateTotal() {
                    debugger;
                    const discountper = parseFloat($('#discountPercentage').val()) || 0; // Default to 0 if empty or invalid
                    const amount = parseFloat($('#amount').val()) || 0; // Default to 0 if empty or invalid
                    const discvalue = (discountper / 100) * amount;
                    $('#discountPerUnit').val(discvalue); // Set total to 2 decimal places
                }

                // Attach onchange event to quantity and price fields
                $('#discountPercentage').on('change', updateTotal);

                // Handle update button click
                document.getElementById('updateItem').addEventListener('click', function () {
                    debugger;
                    let updatedQuantity = document.getElementById('itemQuantity').value;
                    let updatedAmount = document.getElementById('amount').value;
                    let updateddiscountper = document.getElementById('discountPercentage').value;
                    let updateddiscountunit = (updateddiscountper / 100) * updatedAmount;

                    let itemName = document.getElementById('modalTitle').innerText;
                    let tableRows = document.querySelectorAll('#orderTable tr');
                    let targetRow = Array.from(tableRows).find(row => row.cells[0].innerText.includes(itemName));

                    if (targetRow) {
                        // Update quantity and total price
                        let price = parseFloat(targetRow.querySelector('.quantity-input').dataset.price);
                        targetRow.querySelector('.quantity-input').value = updatedQuantity;
                        /*targetRow.querySelector('.total-amount').value = (updatedAmount * updatedQuantity).toFixed(2);*/
                        /*targetRow.querySelector('.total-price').value = updatedAmount;*/
                        targetRow.querySelector('.discountpercent').value = updateddiscountper;
                        targetRow.querySelector('.discountvalue').value = updateddiscountunit;
                    }

                    $('#editItemModal').modal('hide');
                    empr_POSTransaction.updateSummary();

                });



                // Handle row deletion
                document.querySelectorAll('.delete-icon').forEach(icon => {
                    icon.addEventListener('click', function () {
                        const row = this.closest('tr');
                        row.remove();
                    });
                    empr_POSTransaction.updateSummary();

                });
                //const billDiscPercentInput = document.getElementById('billDiscPercent');
                //const billDiscValueInput = document.getElementById('billDiscValue');
                //const totalPaymentElement = document.querySelector('.total-payment');

                //billDiscPercentInput.addEventListener('input', updateTotalPayment);
                //billDiscValueInput.addEventListener('input', updateTotalPayment);

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
                // Get the buttons and the middle section
                const listViewButton = document.querySelector('.toggle-view-list');
                const gridViewButton = document.querySelector('.toggle-view-grid');
                const middleSection = document.querySelector('.middle');

                // Add event listeners for the buttons
                listViewButton.addEventListener('click', () => {
                    middleSection.classList.add('list-view');
                    middleSection.classList.remove('grid-view');
                });

                gridViewButton.addEventListener('click', () => {
                    middleSection.classList.add('grid-view');
                    middleSection.classList.remove('list-view');
                });


            };

            function Checkvalidation() {

                debugger;
                var IsValidate = true
                if ($('#cashAmount').val() != '') {
                    if ($('#Acounthidden').val() == '') {
                        $('#AcountName').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else
                        $('#AcountName').css('border', '');
                }
                else if ($('#bankrecv').val() != '')
                {
                    if ($('#bankhidden').val() == '') {
                        $('#bankName').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else
                        $('#bankName').css('border', '');
                }
                else if ($('#partyrecv').val() != '')
                {
                    if ($('#partyhidden').val() == '') {
                        $('#partyName').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else
                        $('#partyName').css('border', '');
                }
                else if ($('#cashAmount2').val() != '' || $('#bankrecv2').val() != '' || $('#partyrecv2').val() != '') {
                    if ($('#Acounthidden2').val() == '') {
                        $('#AcountName2').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else {
                        $('#AcountName2').css('border', '');
                    }
                    if ($('#bankhidden2').val() == '') {
                        $('#bankName2').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else {
                        $('#bankName2').css('border', '');
                    }
                    if ($('#partyhidden2').val() == '') {
                        $('#partyName2').css('border', '2px solid red');
                        IsValidate = false;
                    }
                    else {
                        $('#partyName2').css('border', '');
                    } 
                }
                else {
                    
                    IsValidate = true
                }
                return IsValidate
            }

        });
    },
    HoldDataSave() {
        $('#BILL_STATUS').val('H');
        $('#recvhidden').val('');
        if (Permissions != "Admin") {
            if (!$("#Code").val() && !Permissions.r_ADD) {
                empr_helper.notify("You are not allowed to add new record !", 2);
            }
            else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                empr_helper.notify("You are not allowed to edit records !", 2);
            } else {
                //if (empr_POSTransaction.ValidateInfo()) {
                //    empr_POSTransaction.SaveInfo();
                //}
                empr_POSTransaction.SaveInfo();
            }
        } else {
            //if (empr_POSTransaction.ValidateInfo()) {
            //    empr_POSTransaction.SaveInfo();
            //}
            empr_POSTransaction.SaveInfo();
            empr_POSTransaction.updateSummary();

        }
    },

    updateSummary() {
        debugger;
        const table = document.getElementById('tableGrid');
        const rows = table.getElementsByTagName('tbody')[0].rows;
        let totalTax = 0;
        let totalAmount = 0;
        let totalQuantity = 0;
        let totalItems = rows.length;
        let totalItemsDiscval = 0;
        let discountPercent = 0;
        let Amount = 0

        // Loop through rows to sum up values
        for (let i = 0; i < rows.length; i++) {
            const tax = parseInt(rows[i].cells[1].querySelector('input').value);
            const quantity = parseInt(rows[i].cells[2].querySelector('input').value);
            const amount = quantity * parseFloat(rows[i].cells[3].querySelector('input').value);
            const discper = parseFloat(rows[i].cells[5].querySelector('input').value);
            const disval = (discper / 100) * amount;
            const taxamount = (tax / 100) * amount;

            totalTax += taxamount;
            totalAmount += amount;
            totalQuantity += quantity;
            totalItemsDiscval += disval;
        }
        var TotalItemDisc = parseInt(totalItemsDiscval);
        var TaxAMT = Math.floor(totalTax);
        // Update total values in summary
        document.getElementById('totalItems').textContent = `Total Items: ${totalItems}`;
        document.getElementById('totalQty').textContent = `Total Qty: ${totalQuantity}`;
        document.getElementById('subTotal').textContent = `Sub Total: ${totalAmount}`;
        document.getElementById('totalTax').textContent = `Total Tax: ${TaxAMT}`;

        document.getElementById('subtotalTax').textContent = TaxAMT;
        Amount = parseInt(totalTax + totalAmount) || 0;
        document.getElementById('totalAmount').textContent = Amount;
        document.getElementById('itemDiscount').textContent = TotalItemDisc;
        var TotalAmountGet = (Amount - totalItemsDiscval);
        $('#totalAmountGet').val(TotalAmountGet);
        $('#TotalbillDisc').val(TotalAmountGet);
        var deliveryPercent = parseFloat($('#billDelPercent').val()) || 0;
        if (Amount == 0) {
            discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
            $('#billDiscPercent').val(0);
            $('#billDiscValue').val(0);
        }
        discountPercent = parseFloat($('#billDiscPercent').val()) || 0;
        if (discountPercent == 0 && deliveryPercent == 0) {
            $('#FinalAmount').html(`Value ${TotalAmountGet} /-`);
            $('#totalPayment').text(TotalAmountGet);
            $('#cashAmount').val(TotalAmountGet);
            $('#recvhidden').val(TotalAmountGet);
        }
        else {
            var discountAmount = parseInt($('#billDiscValue').val()) || 0; // Ensure discountAmount is a number
            var withDicsAmount = TotalAmountGet - discountAmount; // Subtract discount
            $('#totalAmountGet').val(withDicsAmount);

            var deliveryAmount = parseInt($('#billDelValue').val()) || 0; // Fixed: Removed extra parentheses
            var finalAmount = withDicsAmount + deliveryAmount;
            var TotalValue = Math.floor(finalAmount);
            $('#FinalAmount').html(`Value ${TotalValue} /-`);  // Update the final amount display
            $('#totalPayment').text(TotalValue); // Update total payment display
            $('#cashAmount').val(TotalValue);
            $('#recvhidden').val(TotalValue); // Update hidden field with final amount

        }

    },

   

    SetItemsEditData(newData) {
        debugger;
        newData.forEach(function (item) {
            let tableBody = document.getElementById('orderTable');
            // Check if the item already exists in the table
            let existingRow = Array.from(tableBody.rows).find(row => row.cells[0].innerText.includes(item.iteM_NAME));

            if (existingRow) {
                // Update the quantity and total price if item exists
                let quantityInput = existingRow.querySelector('.quantity-input');
                let newQuantity = parseInt(quantityInput.value) + 1; // Increment quantity
                quantityInput.value = newQuantity;

                //let price = parseFloat(quantityInput.dataset.price);
                //existingRow.querySelector('.total-price').value = (price * newQuantity).toFixed(2);
            } else {
                // Add a new row if item doesn't exist
                let newRow = tableBody.insertRow();
                newRow.innerHTML = `
                                  <tr>
                                        <td>${item.iteM_NAME}</td>
                                        <td style="display: none;"><input type="hidden" class="tax" value="${item.tax}"></td>
                                        <td><input type="number" class="quantity-input" value="${item.qty}" data-price="${item.rate}"></td>
                                        <td><input type="number" class="total-price" value="${item.rate}"></td>
                                        <td style="display: none;"><input type="hidden" class="unit" data-price="${item.unit}" value="${item.uniT_NAME}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountpercent" value="${item.disc}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountvalue" value="${item.disC_AMT}"></td>
                                        <td style="display: none;">${item.iteM_CODE}</td>
                                        <td class="icons">
                                            <span class="icon edit-icon" ><i class="fas fa-edit"></i></span>
                                            <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                        </td>
                                    </tr>
                                `;
            }
        });
        empr_POSTransaction.updateSummary();
    },

    loadItemsForData(itemId) {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetItemsMasterByCode?itemId=' + itemId, function (newData) {
            if (newData != null) {
                debugger;
                newData.forEach(function (item) {
                    let tableBody = document.getElementById('orderTable');

                    // Check if the item already exists in the table
                    let existingRow = Array.from(tableBody.rows).find(row => row.cells[0].innerText.includes(item.iteM_NAME));

                    if (existingRow) {
                        debugger;
                        // Update the quantity and total price if item exists
                        let quantityInput = existingRow.querySelector('.quantity-input');
                        let newQuantity = parseInt(quantityInput.value) + 1; // Increment quantity
                        quantityInput.value = newQuantity;

                        /*let price = parseFloat(quantityInput.dataset.price);*/
                        /*existingRow.querySelector('.total-price').value = (price * newQuantity).toFixed(2);*/
                    } else {
                        debugger;
                        // Add a new row if item doesn't exist
                        let newRow = tableBody.insertRow();
                        newRow.innerHTML = `
                                  <tr>
                                        <td class="itemname">${item.iteM_NAME}</td>
                                        <td style="display: none;"><input type="hidden" class="tax" value="${item.tax}"></td>
                                        <td><input type="number" class="quantity-input" value="1" data-price="${item.salE_RATE}"></td>
                                        <td><input type="number" class="total-price" value="${item.salE_RATE}"></td>
                                        <td style="display: none;"><input type="hidden" class="unit" data-price="${item.unitid}" value="${item.unit}"></td>
                                        <td style="display: none;"><input type="hidden" class="discountpercent" value="0"></td>
                                        <td style="display: none;"><input type="hidden" class="discountvalue" value="0"></td>
                                        <td style="display: none;">${item.iteM_CODE}</td>
                                        <td class="icons">
                                            <span class="icon edit-icon" ><i class="fas fa-edit"></i></span>
                                            <span class="icon delete-icon"><i class="fas fa-times"></i></span>
                                        </td>
                                    </tr>
                                `;
                    }
                });
                empr_POSTransaction.updateSummary();
            } else {
                console.log("No data found for the given itemId.");
            }
        });
    },

    ResetForm() {
        $('#orderTable').empty();
        $('#billDelPercent').val('');
        $('#billDelValue').val('');
        $('#CNAME').val('');
        $('#CMOB').val('');
        $('#CADD').val('');
        $('#VOUCHER_NO').text('POS/000000');
        empr_POSTransaction.updateSummary();

    },
    bindDxDdl: function (divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun) {

        ati_dxHelper.createDropdownSingle(divId, data, selectedvalues, keyExp, dataField, placeholder, onvalueChangeFun);

    },

    CreateGrid(dataSrc) {
        debugger;
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
                    debugger;
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
                //cellTemplate: function (container, options) {
                //    var $cell = $("<div>").addClass("custom-cell");
                //    var isChecked = options.data.chK1;
                //    var quantity = options.data.qty;
                //    if (quantity == 0 || quantity == '' || quantity == null || quantity == undefined) {
                //        quantity = '';
                //    }
                //    var $checkbox = $("<input type='checkbox'>")
                //        .prop('checked', isChecked)
                //        .on('change', function () {
                //            var item = options.data;
                //            var qty = parseFloat(item.qty) || 0;
                //            if (isNaN(qty)) {
                //                empr_helper.notify("Please enter the correct quantity.", 2);
                //            }
                //            if (!isNaN(qty)) {
                //                if (this.checked) {
                //                    item.qty = 1;
                //                }
                //                else {
                //                    item.qty = qty;
                //                    var gridInstance = $('#detailContainer').dxDataGrid('instance');
                //                    gridInstance.columnOption('qty', 'allowEditing', true);
                //                }
                //                item.chK1 = this.checked;
                //                container.find('span').text(item.qty);
                //            }

                //            var qty = parseFloat(item.qty) || 0;
                //            var rate = parseFloat(item.rate) || 0;
                //            item.amt = qty * rate;

                //            var gridInstance = $('#detailContainer').dxDataGrid('instance');
                //            var dataSource = gridInstance.option("dataSource");
                //            dataSource[options.rowIndex] = item;
                //            gridInstance.option("dataSource", dataSource);
                //        });

                //    var $span = $('<span class="px-4">' + quantity + '</span>');
                //    $cell.append($checkbox).append($span);
                //    container.append($cell);
                //    empr_POSTransaction.Calculate();
                //}
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
        debugger;
        if ($('#detailContainer').dxDataGrid('instance').hasEditData()) {
            $('#detailContainer').dxDataGrid('instance').saveEditData().done(function () {

                empr_POSTransaction.rowsCount += 1;
                const gridInstance = $('#detailContainer').dxDataGrid('instance');
                var dataSource = gridInstance.option("dataSource");
                if (dataSource.length > 0) {
                    //let clonedRowData = dataSource[index];
                    let clonedRowData = $.extend(true, {}, dataSource[index]);
                    //gridInstance.addRow();
                    //$.each(clonedRowData, function (key, value) {
                    //    if (key == 'dT_CODE') {
                    //        gridInstance.cellValue(0, 'dT_CODE', '');
                    //    }
                    //    else {
                    //        gridInstance.cellValue(0, key, value);
                    //    }
                    //});
                    // After adding the row, insert it at the first position
                    //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                    //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
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
                //gridInstance.addRow();
                //$.each(clonedRowData, function (key, value) {
                //    if (key == 'dT_CODE') {
                //        gridInstance.cellValue(0, 'dT_CODE', '');
                //    }
                //    else {
                //        gridInstance.cellValue(0, key, value);
                //    }
                //});
                // After adding the row, insert it at the first position
                //gridInstance.insertRow(clonedRowData, 0); // Assuming you want to insert at the first position
                //dataSource.unshift(clonedRowData); // Add the cloned row data at the beginning of the dataSource array
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
        debugger;
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

    DeleteRow(index) {
        debugger;
        const gridInstance = $('#detailContainer').dxDataGrid('instance');
        var dataSource = gridInstance.option("dataSource");
        if (dataSource.length > 0) {
            if (dataSource.length > 1) {
                var row = dataSource[index];
                if (row.dT_CODE == '' || row.dT_CODE == null || row.dT_CODE == undefined) {
                    gridInstance.deleteRow(index);
                    empr_POSTransaction.rowsCount -= 1;
                    gridInstance.saveEditData();
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
                        ajaxHelper.ajaxPostJsonData({ code: row.dT_CODE }, "/POSTransactions/DeletePOSTransactionDetailByCode", function (data) {
                            empr_helper.notify(data.msg, data.msgType);
                            if (data.msgType == 1) {
                                gridInstance.deleteRow(index);
                                empr_POSTransaction.rowsCount -= 1;
                                gridInstance.saveEditData();
                            }
                        }, false, true);
                    });
                }
            }
            else {
                empr_helper.notify("You are not allowed to delete the last row.", 2);
            }
        }
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

    GetPOSTransactions() {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSTransactions' + , function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.CreateQuickSearchGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    CreateQuickSearchGrid(dataSrc) {
        var columns = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger

                $(`<div class="btn-group btn-group-sm">
                                   <a href="javascript:;"  class="grid-action-icon elm_edit" reportid=${options.data.traN_ID} title="Edit"><i class="fa fa-edit"></i></a>
                                   </div>`).appendTo(container);
            }
        },
        { dataField: 'traN_ID', caption: 'Code', visible: false, },
        { dataField: 'v_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
        { dataField: 'voucheR_NO', caption: 'Voucher No', },
        { dataField: 'cname', caption: 'Name', },
        { dataField: 'cmob', caption: 'Contact #', },
        { dataField: 'neT_TOTAL', caption: 'Total Value',},
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
            //{
            //    dataField: 'detail', calculateFilterExpression: function (value) {
            //        return [function (data) {
            //            var details = data.detail,
            //                detail;

            //            for (var i = 0; i < details.length; i++) {
            //                detail = details[i];
            //                for (var fieldName in detail) {
            //                    if (detail[fieldName] && detail[fieldName].toString().toLowerCase().indexOf(value.toLowerCase()) >= 0) {
            //                        return true;
            //                    }
            //                }
            //            }
            //            return false;
            //        }, "=", true]
            //    }
            //}
        ];
        //var detailColumns = [
        //    { dataField: 'iteM_CODE', caption: 'Item Name' },
        //    { dataField: 'qty', caption: 'Qty', },
        //    { dataField: 'unit', caption: 'Unit', },
        //    { dataField: 'qtY2', caption: 'Qty2', },
        //    { dataField: 'baL_QTY', caption: 'Balance Quantity', },
        //    { dataField: 'rate', caption: 'Rate', },
        //    { dataField: 'rT_TYPE', caption: 'RT Type', },
        //    { dataField: 'amt', caption: 'Amount', },
        //    { dataField: 'dT_DESC', caption: 'Description', visible: false },
        //    { dataField: 'bR_AMOUNT_SELLER', caption: 'Brk Amt (Sellr)', },
        //    { dataField: 'bR_AMOUNT_BUYER', caption: 'Brk Amt (Buyr)', },
        //    { dataField: 'wT_AMOUNT_SELLER', caption: 'Wt Amt (Sellr)', },
        //    { dataField: 'wT_AMOUNT_BUYER', caption: 'Wt Amt (Buyr)', },
        //    { dataField: 'neT_AMT', caption: 'Net Amount', },
        //    { dataField: 'trucK_NO', caption: 'Truck No', visible: false },
        //    { dataField: 'conT_NO', caption: 'Cont No', visible: false },
        //    { dataField: 'scomp', caption: 'SComp', },
        //];
        //empr_helper.MasterDetailDxGridBinding('#gridContainer', columns, detailColumns, dataSrc, "POSTransaction");
        empr_helper.dxGridbinding('#gridContainer', columns, dataSrc, "POSTransaction");
    },

    GetDataToSave() {

        var TRANSID = $("#TRANS_ID").val();
        var V_DATE = $("#V_DATE").text();
        var VOUCHER_NO = $("#VOUCHER_NO").text();
        var CNAME = $('#CNAME').val();
        var CMOB = $('#CMOB').val();
        var CADD = $('#CADD').val();
        var INV_STATUS = $('#INV_STATUS').val();
        var TOTAL = parseFloat($('#subTotal').text().replace(/[^\d.]/g, ''));
        var DISC = $('#billDiscPercent').val();
        var DISC_AMT = $('#billDiscValue').val();
        var NET_TOTAL = TOTAL - DISC_AMT;
        var DEL = $('#billDelPercent').val();
        var DEL_CHARGES = $('#billDelValue').val();
        var TAX_AMT = parseFloat($('#totalTax').text().replace(/[^\d.]/g, ''));
        var TAX_PER = NET_TOTAL > 0 ? (TAX_AMT / NET_TOTAL) * 100 : 0;
        var TAX = Math.floor(TAX_PER);
        var BILL_STATUS = $('#BILL_STATUS').val();
        var CASH = $('#cashAmount').val() || $('#cashAmount2').val() || 0;
        var CACT_CODE = $('#Acounthidden').val() || $('#Acounthidden2').val() || 0;
        var BANK = $('#bankrecv').val() || $('#bankrecv2').val() || 0;
        var BACT_CODE = $('#bankhidden').val() || $('#bankhidden2').val() || 0;
        var PARTY = $('#partyrecv').val() || $('#partyrecv2').val() || 0;
        var PARTY_CODE = $('#partycode').val() || $('#partycode2').val() || 0;
        var ACT_CODE = $('#partyhidden').val() || $('#partyhidden2').val() || 0;
        var RECV = $('#recvhidden').val();

        var masterRecord = {
            TRAN_ID: TRANSID,
            V_DATE: V_DATE,
            VOUCHER_NO: VOUCHER_NO,
            CNAME: CNAME,
            CMOB: CMOB,
            CADD: CADD,
            INV_STATUS: INV_STATUS,
            TOTAL: TOTAL,
            DISC: DISC,
            DISC_AMT: DISC_AMT,
            NET_TOTAL: NET_TOTAL,
            DEL: DEL,
            DEL_CHARGES: DEL_CHARGES,
            TAX: TAX,
            TAX_AMT: TAX_AMT,
            BILL_STATUS: BILL_STATUS,
            CASH: CASH,
            CACT_CODE: CACT_CODE,
            BANK: BANK,
            BACT_CODE: BACT_CODE,
            PARTY: PARTY,
            PARTY_CODE: PARTY_CODE,
            ACT_CODE: ACT_CODE,
            RECV: RECV
        }

        var detailRecords = [];
        $('#orderTable tr').each(function () {
            debugger;
            const row = $(this);
            const AMT = row.find('.quantity-input').val() * row.find('.total-price').val();// Get the current row

            // Extract row data into an object
            let rowData = {
                ITEM_CODE: row.find('td:eq(7)').text().trim(),
                TAX: row.find('.tax').val(),
                TAX_AMT: parseInt($('#totalTax').text().match(/\d+/)[0]) || 0,
                ITEM_NAME: row.find('.itemname').text(),// Get the value of the hidden tax input
                QTY: row.find('.quantity-input').val(), // Get the quantity input value
                RATE: row.find('.total-price').val(),
                AMT: AMT,
                UNIT: row.find('.unit').data('price') || row.find('.unit').attr('data-price'), // Extract data-price
                DISC: row.find('.discountpercent').val(), // Discount percent
                DISC_AMT: row.find('.discountvalue').val(),
                TEM_CODE: row.find('.code').val(),
                NET_AMT: AMT - row.find('.discountvalue').val()
            };

            // Add the row object to the array
            detailRecords.push(rowData);
        });

        var modelRecord = {
            Master: masterRecord,
            Detail: detailRecords
        };
        return modelRecord;

        //if (masterRecord.length > 0 && detailRecords.length > 0) {
        //    debugger;

        //}
        //else {
        //    var modelRecord = {
        //        Master: masterRecord,
        //        Detail: detailRecords,
        //    };
        //    return modelRecord;
        //}
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

    SaveInfo() {
        var dataModel = empr_POSTransaction.GetDataToSave();
        debugger;
        //if (dataModel.Master.TRAN_ID == 0
        //    || dataModel.Master.TRAN_ID == null#code
        //    || dataModel.Master.TRAN_ID == undefined
        //    || dataModel.Master.TRAN_ID == "") {
        //    dataModel.Detail.reverse();
        //}
        ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransactions/Save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                debugger;
                if (dataModel.Master.BILL_STATUS == "P") {
                    $('#slipPreview').html(data.slipHtml);
                    $('#printModal').modal('show');
                }
                $('.PaymentModalClose').click();
                empr_POSTransaction.ResetForm();
            }
        }, false, true);
    },

    
    
    DeleveryInputvisible(code) {

        
        if (code == 4) {
            $('#INV_STATUS').val(code);
            $("#inputContainer").toggle();

        } else {
            $("#inputContainer").hide();
        }
    },

    GetPOSTransactionByCode(code) {
        ajaxHelper.ajaxGetJson('/POSTransactions/GetPOSTransactionByCode?code=' + code, function (data) {
            debugger;
            if (data.master.msgType == 1) {
                var masterData = data.master.data;
                if (data.detail.msgType == 1) {
                    empr_POSTransaction.SetItemsEditData(data.detail.data);
                    //empr_POSTransaction.CreateGrid(data.detail.data);
                    //$('.card-body').addClass('customHighlightForModifiedCells');
                }
                if (masterData.length == 1) {
                    var response = masterData[0];
                    $('#TRANS_ID').val(response.traN_ID);
                    $('#V_DATE').text(response.v_DATE);
                    $('#VOUCHER_NO').text(response.voucheR_NO);
                    $('#CNAME').val(response.cname);
                    $('#CMOB').val(response.cmob);
                    $('#CADD').val(response.cadd);
                    $('#TOTAL').val(response.total);
                    $('#billDiscPercent').val(response.disc);
                    $('#billDiscValue').val(response.disC_AMT);
                    $('#NET_TOTAL').val(response.neT_TOTAL);
                    $('#CASH').val(response.cash);
                    $('#billDelPercent').val(response.del);
                    $('#billDelValue').val(response.deL_AMT);
                    //$('#subtotalTax').text(response.taX_AMT);
                    //$('#totalTax').text(`Total Tax: ${response.taX_AMT}`);
                    $('#totalAmountGet').val(response.total);
                    $('#totalAmount').text(response.total);
                    $('#totalPayment').val(response.neT_TOTAL);
                    var SummaryTotal = parseInt(response.neT_TOTAL) + parseInt(response.deL_AMT);
                    $('#FinalAmount').html(`Value ${SummaryTotal} /-`);
                    empr_POSTransaction.DeleveryInputvisible(response.inV_STATUS);
                    //empr_POSTransaction.InitDropdownsWithValue(response.booK_TYPE, );
                    //$('#BtnDelete').show();
                    //if (Permissions != "Admin") {
                    //    if (Permissions.r_DLT) {
                    //        $('#BtnDelete').show();
                    //    }
                    //    if (Permissions.r_EDIT) {
                    //        $('#BtnSave').show();
                    //    }
                    //    else {
                    //        $('#BtnSave').hide();
                    //    }
                    //} else {
                    //    $('#BtnSave').show();
                    //    $('#BtnDelete').show();
                    //}
                    empr_POSTransaction.updateSummary();
                }
                else {
                    empr_helper.notify(data.msg, data.msgType);
                }
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
        }, false, true);
    },

    GetPOSTransactionDetailsByCode(code) {
        ajaxHelper.ajaxGetJson('/POSTransaction/GetPOSTransactionDetailByCode?code=' + code, function (data) {
            if (data.msgType == 1) {
                empr_POSTransaction.CreateGrid(data.data);
            }
            else {
                empr_helper.notify(data.msg, data.msgType);
            }
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

        let TRAN_ID = $("#Code").val();
        let MD_ID = $('#ReportType').dxSelectBox('option', 'value');
        if (TRAN_ID == 0 || TRAN_ID == null || TRAN_ID == undefined || TRAN_ID == "") {
            empr_helper.notify("Please open the delivery in edit mode.", 2);
            return;
        }
        var dataModel = {
            TRAN_ID: TRAN_ID,
            MD_ID: MD_ID,
        }
        ajaxHelper.ajaxPostJsonData(dataModel, "/POSTransaction/GetPrintReport", function (data) {
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

    EnableShortCutKeys(saveElement, newElement, deleteElement, quickSearchElement, idElement, gridElement, rowIndex, dataField) {

        $(document).keydown(function (e) {
            
            // Add event delegation for dynamically added quantity inputs
            document.querySelector('#orderTable').addEventListener('input', function (e) {
                debugger;
                if (e.target.classList.contains('quantity-input')) {
                    empr_POSTransaction.updateSummary();
                }
                if (e.target.classList.contains('total-price')) {
                    debugger;
                    empr_POSTransaction.updateSummary();
                }
               
            });





            const searchBar = document.querySelector('.search-bar');
            const boxes = document.querySelectorAll('.middle .box');

            // Add an event listener to the search bar
            searchBar.addEventListener('input', () => {
                debugger;
                const searchText = searchBar.value.toLowerCase();

                boxes.forEach(box => {
                    const text = box.textContent.toLowerCase();
                    if (text.includes(searchText)) {
                        box.style.display = 'flex';
                    } else {
                        box.style.display = 'none';
                    }
                });
            });

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

            if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
                console.log('CTRL+D is pressed');
                e.preventDefault();
                if ($(idElement).val() != '' && $(idElement).val() != null && $(idElement).val() != undefined) {
                    $(deleteElement).click();
                }
                return false;
            }

            if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
                console.log('CTRL+F is pressed');
                e.preventDefault();
                $(quickSearchElement).click();
                return false;
            }

            if (e.key === 'Enter') {
                console.log('Enter is pressed');
                e.preventDefault();
                $($('a.grid-action-icon[title="Add"]')[0]).click();
                setTimeout(function () {
                    var nextElement = $(gridElement).dxDataGrid('instance').getCellElement(rowIndex, dataField);
                    $(nextElement).click();
                    $(gridElement).dxDataGrid('instance').focus(nextElement);
                }, 1500);
                return false;
            }
        });
    },

    editableDxGridbindingForTransactions(div, columns, datasrc, fileName, selectionMode) {
        const columnChooserModes = [{
            "key": 'dragAndDrop',
            "name": 'Drag and drop',
        }, {
            "key": 'select',
            "name": 'Select',
        }];
        const dataGrid = $(div).dxDataGrid({
            "dataSource": datasrc,
            "columns": columns,
            "remoteOperations": false,
            "height": 300,
            "rowAlternationEnabled": true,
            "groupPanel": { visible: true },
            "columnAutoWidth": true,
            "allowColumnResizing": true,
            "headerFilter": {
                "visible": true,
                "search": {
                    "enabled": false,
                    "editorOptions": {
                        "placeholder": 'Search',
                    },
                },
            },
            "paging": {
                "pageSize": 10,
                "enabled": false
            },
            "pager": {
                "visible": false,
                "allowedPageSizes": [100, 200, 300, 'all'],
                "showPageSizeSelector": false,
                "showInfo": false,
                "showNavigationButtons": false,
            },
            "searchPanel": {
                "visible": true,
                "highlightCaseSensitive": true,
            },
            "filterRow": {
                "visible": false,
                "applyFilter": 'auto',
            },
            "columnChooser": {
                "enabled": true,
                "mode": columnChooserModes[1].key,
                "position": {
                    "my": 'right top',
                    "at": 'right bottom',
                    "of": '.dx-datagrid-column-chooser-button',
                },
                "search": {
                    "enabled": true,
                    "editorOptions": { placeholder: 'Search column' },
                },
                "selection": {
                    "recursive": true,
                    "selectByClick": true,
                    "allowSelectAll": true,
                },
            },
            "scrolling": {
                "mode": "both"
            },
            "columnFixing": {
                "enabled": true,
            },
            "export": {
                "enabled": false
            },
            "selection": {
                "mode": selectionMode == '' || selectionMode == null || selectionMode == undefined ? '' : selectionMode,
                "showCheckBoxesMode": 'always',
            },
            editing: {
                mode: 'batch',
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: false,
                selectTextOnEditStart: true,
                startEditAction: 'click',
                useIcons: true,
                newRowPosition: 'first',
            },
            showBorders: true,
            onExporting(e) {
                if (e.format === 'pdf') {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const doc = new jsPDF({
                            orientation: 'portrait', // or 'landscape' depending on your requirement
                            unit: 'pt', // units of measurement: points
                            format: 'a1' // set the page size to A3
                        });

                        DevExpress.pdfExporter.exportDataGrid({
                            "jsPDFDocument": doc,
                            "component": e.component,
                            indent: 5,
                            selectedRowsOnly: true,
                            margin: {
                                top: 10,
                                right: 10,
                                bottom: 10,
                                left: 10,
                            },
                            topLeft: { x: 5, y: 5 },
                            onExporting: function (e) {
                                e.component.beginUpdate();
                            },
                            onExported: function (e) {
                                e.component.endUpdate();
                            },
                            customizeCell: function (options) {
                                if (options.gridCell.rowType === "data" && options.gridCell.column.caption === "Image") {
                                    var img = new Image();
                                    img.src = options.gridCell.value;
                                    doc.addImage(img, 'PNG', 5, 5, 50, 50);
                                    return false;
                                }
                            }
                        }).then(function () {
                            doc.save(fileName + '.pdf');
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
                else {
                    if (e.component.getSelectedRowsData().length > 0) {
                        const workbook = new ExcelJS.Workbook();
                        const worksheet = workbook.addWorksheet(fileName);

                        DevExpress.excelExporter.exportDataGrid({
                            component: e.component,
                            worksheet,
                            autoFilterEnabled: true,
                            selectedRowsOnly: true
                        }).then(() => {
                            workbook.xlsx.writeBuffer().then((buffer) => {
                                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName + '.xlsx');
                            });
                        });
                    } else {
                        empr_helper.notify('Please first the select rows.', 2);
                    }
                }
            },
            onCellPrepared(e) {
                const column = e.column;
                const rowData = e.data;
                const value = rowData && column && column.dataField ? rowData[column.dataField] : null;

                if (column.caption && (column.caption.toLowerCase().includes('date') || column.caption.toLowerCase().includes('exp')) && (value == '1900-01-01' || value == '01-01-1900' || value == '01-Jan-1900' || value == '1/1/1900 12:00:00 AM' || value == '1/1/1900')) {
                    $(e.cellElement).text('')
                }
            },
            onEditorPreparing: function (e) {
                if (e.dataType === "number") {
                    e.editorOptions.onInput = function (e) {
                        var element = e.element;
                        var value = $(element).find('input').val();
                        value = value.replace(/[^0-9.]/g, '');
                        value = value.replace(/\.(?=.*\.)/g, '');
                        value = value.replace(/(\.\d{3})\d+/g, '$1');
                        if (value === '' || parseFloat(value) >= 0) {
                            $(element).val(value);
                        } else {
                            $(element).val($(element).data('lastValid') || '');
                        }
                        $(element).data('lastValid', $(element).val());
                    };
                }
            },
            onRowUpdating: function (e) {
                var editedRow = e.newData;
                if (editedRow.credit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.debit < 0) {
                    e.cancel = true;
                    empr_helper.notify("Amount must be greater than 0.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0 && editedRow.debit > 0) {
                    e.cancel = true;
                    empr_helper.notify("Please enter either Credit or Debit.", 2);
                    $('#IsValidate').val('false');
                }

                if (editedRow.credit > 0) {
                    if (e.oldData.debit > 0 && editedRow.debit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }

                if (editedRow.debit > 0) {
                    if (e.oldData.credit > 0 && editedRow.credit != 0) {
                        e.cancel = true;
                        empr_helper.notify("Please enter either Credit or Debit.", 2);
                        $('#IsValidate').val('false');
                    }
                }
            },
            onRowUpdated: function (e) {

                if ($('#IsValidate').val() == 'true') {
                    if (e.data != null) {
                        if (empr_AccountOpening.editedRows.filter(r => r.acT_CODE == e.data.acT_CODE).length > 0) {
                            var rowIndex = empr_AccountOpening.editedRows.findIndex(function (row) {
                                return row.acT_CODE === e.data.acT_CODE;
                            });

                            if (rowIndex !== -1) {
                                empr_AccountOpening.editedRows.splice(rowIndex, 1);
                                empr_AccountOpening.editedRows.push(e.data);
                            }
                        }
                        else {
                            empr_AccountOpening.editedRows.push(e.data);
                        }
                    }
                }
            },
            onToolbarPreparing: function (e) {

                var toolbarItems = e.toolbarOptions.items;
                for (var i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].name === "saveButton" || toolbarItems[i].name === "revertButton" || toolbarItems[i].name === "addRowButton" || toolbarItems[i].name === "deleteRowButton") {
                        toolbarItems.splice(i, 1);
                        i--;
                    }
                }
            },
            onEditingStart: function (e) {

                if (e.data.acT_TYPE === "C") {
                    e.cancel = true;
                    empr_helper.notify("You are not allowed to edit this account because it has type 'C'.", 2);
                }

                if ($(e.element).attr('id') == 'DetailContainer' || $(e.element).attr('id') == 'deliveryFeedingDetailContainer') {
                    if (e.column.dataField == 'iteM_CODE') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                        }
                    }

                    if (e.column.dataField == 'rate') {
                        if (e.data.picK_ID != '' && e.data.picK_ID != undefined && e.data.picK_ID != '0') {
                            e.cancel = true;
                        }
                    }
                }

            },
            onKeyDown: function (e) {
                if ($(e.element).attr('id') == 'detailContainer') {

                    if (e.event.key === "Tab") {
                        var columnIndex = dataGrid.option("focusedColumnIndex");
                        if (columnIndex === 5) {
                            e.event.preventDefault();
                            $('#SETT').focus();
                        }
                    }
                    var keyCode = e.event.keyCode;
                    var focusedRowIndex = dataGrid.option("focusedRowIndex");

                    if (focusedRowIndex !== undefined) {
                        var $focusedCell = $(dataGrid.getCellElement(focusedRowIndex, "Action")); // Assuming "Action" is the dataField of your action column

                        if (keyCode === 13) {
                            console.log("Enter is pressed");
                            var $actionButton = $focusedCell.find(".Add"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                        else if (e.event.ctrlKey && keyCode === 67) {
                            console.log("Ctrl+C is pressed");
                            var $actionButton = $focusedCell.find(".Clone"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                        else if (keyCode === 46) {
                            // Ctrl+C logic
                            console.log("Delete is pressed");
                            var $actionButton = $focusedCell.find(".Delete"); // Replace "Add" with your desired class name
                            $($actionButton).click();
                            setTimeout(function () {
                                var nextElement = dataGrid.getCellElement(0, 'iteM_CODE');
                                $(nextElement).click();
                                dataGrid.focus(nextElement);
                            }, 1500);
                        }
                    }

                }
            },
            summary: {
                recalculateWhileEditing: true,
                totalItems: [
                    {
                        column: "qty",
                        summaryType: "sum",
                        displayFormat: "Total: {0}"
                    },
                    {
                        column: "amt",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "AmountTotal"
                    },
                    {
                        column: "neT_AMT",
                        summaryType: "sum",
                        displayFormat: "Total: {0}",
                        name: "NetTotal"
                    },
                ]
            }
        }).dxDataGrid('instance');
    }
}