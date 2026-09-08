$("#ACT_NATURE").change(function () {
    var selectedText = $(this).find("option:selected").text();
    debugger
    if (selectedText.toLowerCase().startsWith("bank")) {
        $("#preifex").removeClass("hide");
        $("#accountcurrecny").removeClass("hide");
        $("#Cheqformat").removeClass("hide");
        $("#accountswift").removeClass("hide");
        $("#accounttitle").removeClass("hide");
        $("#bankaccount").removeClass("hide");
    } else {
        $("#preifex").addClass("hide");
        $("#accountcurrecny").addClass("hide");
        $("#Cheqformat").addClass("hide");
        $("#accountswift").addClass("hide");
        $("#accounttitle").addClass("hide");
        $("#bankaccount").addClass("hide");
    }
});
$("#ACT_NAME").blur(function () {
    var name = $(this).val();
    if (name !== '') {
        $.ajax({
            url: '/ChartOfAccount/CheckName',
            method: 'get',
            data: { name: name },
            success: function (response) {
                if (response.exists) {
                    $(".name-validation").text("Name already exists");
                    setTimeout(function () {
                        $(".name-validation").text("");
                    }, 1000);
                    $("#ACT_NAME").focus();
                }
            },
        });
    }
});
$(".btn-save").on("click", function () {
    debugger;
    if ($("#ACT_NAME").val() != "") {
        var selectedText = $("#ACT_NATURE").find("option:selected").text();
        if (selectedText.toLowerCase().startsWith("bank")) {
            if ($("#PREFIX").val() != "") {
                AddNewRecord();
            }
            else {
                $(".prefiex-validation").text("Please fill this field");
                setTimeout(function () {
                    $(".prefiex-validation").text("");
                }, 1000);
            }
        }
        else {
            AddNewRecord();
        }
    }
    else {
        $(".name-validation").text("Please fill this field");
        setTimeout(function () {
            $(".name-validation").text("");
        }, 1000);
    }
});
$(".btn-new").on("click", function () {
    $(".Record input").val('');
    $(".btn-delete").hide();
    $("input, select").prop("disabled", false);
});
$(".btn-delete").on("click", function () {
    debugger;
    var Id = $("#ID").val();
    Delete(Id);
});
function reloadGrid() {
    debugger;
    $.ajax({
        url: 'ChartOfAccount/Listener',
        method: 'GET',
        success: function (data) {
            const treeList = $("#treeListContainer").dxTreeList("instance");
            treeList.option("dataSource", data);
            treeList.refresh();
        },
        error: function (error) {
            console.error('Error fetching data:', error);
        }
    });
}
function GridView() {
    $(function () {
        $.ajax({
            url: 'ChartOfAccount/TreeView',
            method: 'GET',
            success: function (data) {
                console.log(data)
                $('#treeListContainer').dxTreeList({
                    dataSource: data,
                    keyExpr: 'id',
                    parentIdExpr: 'acT_PARENT_CODE',
                    headerFilter: {
                        visible: true,
                        allowSearch: true
                    },
                    searchPanel: {
                        visible: true,
                        highlightCaseSensitive: true,
                    },
                    scrolling: {
                        mode: 'virtual'
                    },
                    height: "350px",
                    columns: [
                        {
                            dataField: 'acT_NAME',
                            caption: 'Name'
                        }
                    ],
                    expandedRowKeys: [0],
                    showRowLines: true,
                    onRowClick: function (info) {
                        const clickedRowData = info.data;
                        const Id = clickedRowData.id;
                        GetRecord(Id);
                        $("input, select").prop("disabled", true);
                        $("#ACT_SNAME, #ACT_NAME, #ASTATUS, #ACT_GROUP").prop("disabled", false);


                    }
                });
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    });

}
function AddNewRecord() {
    debugger;
    onFormSubmitBegin();
    var ID = $("#ID").val();
    var ADD_USER_ID = $("#ADD_USER_ID").val();
    var ADD_DATE = $("#ADD_DATE").val();
    var ADD_COMPUTER_NAME = $("#ADD_COMPUTER_NAME").val();
    var ADD_IP_ADDRESS = $("#ADD_IP_ADDRESS").val();
    var ADD_POSTALCODE = $("#ADD_POSTALCODE").val();
    var MENU_ID = $("#MENU_ID").val();
    var ACT_GR_CODE = $("#ACT_GR_CODE").val();
    var ACT_NAME = $("#ACT_NAME").val();
    var ACT_SNAME = $("#ACT_SNAME").val();
    var ACT_TYPE = $("#ACT_TYPE").val();
    var ACT_PARENT_CODE = $("#ACT_PARENT_CODE").val();
    var ACT_GROUP = $("#ACT_GROUP").val();
    var ACT_NATURE = $("#ACT_NATURE").val();
    var ASTATUS = $("#ASTATUS").val();
    var PREFIX = $("#PREFIX").val();
    var ACCOUNT_NO = $("#ACCOUNT_NO").val();
    var TITTLE = $("#TITTLE").val();
    var SWIFT = $("#SWIFT").val();
    var CHQ_ID = $("#CHQ_ID").val();
    var CURRENCY = $("#CURRENCY").val();
    var modelRecord = {
        ID: ID,
        ADD_USER_ID: ADD_USER_ID,
        ADD_DATE: ADD_DATE,
        ADD_COMPUTER_NAME: ADD_COMPUTER_NAME,
        ADD_IP_ADDRESS: ADD_IP_ADDRESS,
        ADD_POSTALCODE: ADD_POSTALCODE,
        MENU_ID: MENU_ID,
        ACT_GR_CODE: ACT_GR_CODE,
        ACT_NAME: ACT_NAME,
        ACT_SNAME: ACT_SNAME,
        ACT_TYPE: ACT_TYPE,
        ACT_PARENT_CODE: ACT_PARENT_CODE,
        ACT_GROUP: ACT_GROUP,
        ACT_NATURE: ACT_NATURE,
        ASTATUS: ASTATUS,
        PREFIX: PREFIX,
        ACCOUNT_NO: ACCOUNT_NO,
        TITTLE: TITTLE,
        SWIFT: SWIFT,
        CHQ_ID: CHQ_ID,
        CURRENCY: CURRENCY
    }
    $.ajax({
        url: "ChartOfAccount/save",
        type: "post",
        data: JSON.stringify(modelRecord),
        success: function (response) {
            onFormSuccess(response);
            reloadGrid();
            $('.Record input').val('');
        }
    });
}
function GetRecord(Id) {
    debugger
    $.ajax({
        url: "ChartOfAccount/GetRecord",
        type: "get",
        data: { Id: Id },
        success: function (record) {
            console.log(record);
            $("#ID").val(record.id);
            $("#Code").val(record.id);
            $("#ADD_USER_ID").val(record.adD_USER_ID);
            $("#ADD_DATE").val(record.adD_DATE);
            $("#ADD_COMPUTER_NAME").val(record.adD_COMPUTER_NAME);
            $("#ADD_IP_ADDRESS").val(record.adD_IP_ADDRESS);
            $("#ADD_POSTALCODE").val(record.adD_POSTALCODE);
            $("#MENU_ID").val(record.menU_ID);
            $("#ACT_GR_CODE").val(record.acT_GR_CODE);
            $("#ACT_NAME").val(record.acT_NAME)
            $("#ACT_SNAME").val(record.acT_SNAME)
            $("#ACT_TYPE").val(record.acT_TYPE)
            $("#ACT_PARENT_CODE").val(record.acT_PARENT_CODE)
            $("#ACT_GROUP").val(record.acT_GROUP)
            $("#ACT_NATURE").val(record.acT_NATURE)
            $("#ASTATUS").val(record.astatus)
            $("#PREFIX").val(record.prefix)
            $("#ACCOUNT_NO").val(record.accounT_NO)
            $("#TITTLE").val(record.tittle)
            $("#SWIFT").val(record.swift)
            $("#CHQ_ID").val(record.chQ_ID)
            $("#CURRENCY").val(record.currency)
            $(".btn-delete").show();
        }
    });
}
function Delete(Id) {
    swal({
        title: 'Are you sure?',
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
        $.ajax({
            url: "ChartOfAccount/delete",
            type: "post",
            data: { Id: Id },
            success: function (response) {
                if (response.success) {
                    reloadGrid();
                    $(".Record Input").val('');
                    toastr.success(response.message, "Deleted Successfully", {
                        timeOut: "5000"
                    });
                    $("input, select").prop("disabled", false);
                }
                else {
                    toastr.error(response.message, "Error!");
                }
            }
        });
    });
}