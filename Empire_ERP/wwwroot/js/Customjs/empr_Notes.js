var empr_Notes = {
    editorInstance: '',
    companyName: '',
    companyAddress: '',
    initialDate: '',
    initEvents: function () {

        $(document).ready(function () {

            $('#saveAttempt').click(function () {
                if (Permissions != "Admin") {
                    if (!$("#Code").val() && !Permissions.r_ADD) {
                        empr_helper.notify("You are not allowed to add new record !", 2);
                    }
                    else if (($("#Code").val() > 0) && !Permissions.r_EDIT) {
                        empr_helper.notify("You are not allowed to edit records !", 2);
                    } else {
                        if (empr_Notes.validateForm()) {
                            empr_Notes.saveAttempt();
                        }
                    }
                } else {
                    if (empr_Notes.validateForm()) {
                        empr_Notes.saveAttempt();
                    }
                }
            })

            $('body').on('click', '#quicksearch', function () {
                empr_Notes.InintQuickSearch();
            })

            $('body').on('click', '.elm_edit', function () {
                var rportid = $(this).attr("rportid")
                empr_Notes.GetNotesByID(rportid);
            })

            $('body').on('click', '#resetall', function () {
                $('.btn-delete').hide();
                $('#printButton').hide();
                $('#resetall').hide();
                empr_Notes.resetForm();

            })

            $('.btn-delete').click(function () {

                empr_Notes.DeleteRecord();

            })

            if (Permissions != "Admin") {
                !Permissions.r_ADD && $('#resetall').hide();
                !Permissions.r_VIEW && $('#quicksearch').hide();
                (!Permissions.r_ADD && !Permissions.r_EDIT) && $('#saveAttempt').hide();
            }

            ClassicEditor.create(document.querySelector('#editor')).then(editor => {
                // Store the editor instance for later use
                empr_Notes.editorInstance = editor;
            }).catch(error => console.error(error));

            $("#printButton").on("click", function () {
                empr_Notes.Print();
            });
            //ClassicEditor.create(document.querySelector('#editor'), {
            //    image: {
            //        toolbar: ['imageTextAlternative', 'imageStyle:inline', 'imageStyle:side', 'imageResize'],
            //    },
            //    simpleUpload: {
            //        uploadUrl: '/your-upload-url', // The server URL where files will be uploaded
            //        headers: {
            //            'X-CSRF-TOKEN': 'your-csrf-token', // Add CSRF token if required
            //        }
            //    }
            //}).then(editor => {
            //        console.log('Editor is ready.');
            //    })
            //    .catch(error => {
            //        console.error(error);
            //    });


            //tinymce.init({
            //    selector: '#editorTiny'
            //});
            //tinymce.init({
            //    selector: '#editorTiny',
            //    height: 300,
            //    plugins: 'lists link image code table save',
            //    menubar: true, // Disable the menu bar
            //    toolbar: 'undo redo | bold italic underline | fontsizeselect forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | link image table code | save',
            //    toolbar_mode: 'wrap', // Ensure everything fits even if it's wide
            //    setup: (editor) => {
            //        editor.ui.registry.addMenuButton('fileMenu', {
            //            text: 'File',
            //            fetch: (callback) => {
            //                callback([
            //                    { type: 'menuitem', text: 'Save', onAction: () => console.log('Save clicked') },
            //                ]);
            //            },
            //        })
            //    },
            //})

            //tinymce.init({
            //    selector: '#editorTiny',
            //    height: 300,
            //    plugins: 'lists link image code table save textcolor colorpicker advlist fontsize fontfamily',
            //    menubar: false, // Disable the menu bar
            //    toolbar: 'undo redo | formatselect | fontselect fontsizeselect | bold italic underline strikethrough | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | link image table code | save',
            //    toolbar_mode: 'wrap', // Ensure everything fits if the toolbar is wide
            //    branding: false, // Optional: Removes the TinyMCE branding
            //});

            //tinymce.init({
            //    selector: '#editorTiny',
            //    height: 300,
            //    plugins: 'lists link image code table save textcolor colorpicker advlist fontsize fontfamily',
            //    menubar: false, // Disable the menu bar
            //    toolbar: 'undo redo | formatselect | fontselect | fontsizeselect | bold italic underline strikethrough | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | link image table code | save',
            //    fontsize_formats: '8pt 10pt 12pt 14pt 18pt 24pt 36pt', // Define font size options
            //    toolbar_mode: 'wrap', // Ensure toolbar fits in small widths
            //    branding: false, // Optional: Removes TinyMCE branding
            //});

            //tinymce.init({
            //    selector: 'textarea',
            //    plugins: [
            //        // Core editing features
            //        'anchor', 'autolink', 'charmap', 'codesample', 'emoticons', 'image', 'link', 'lists', 'media', 'searchreplace', 'table', 'visualblocks', 'wordcount',
            //        // Your account includes a free trial of TinyMCE premium features
            //        // Try the most popular premium features until Dec 20, 2024:
            //        'checklist', 'mediaembed', 'casechange', 'export', 'formatpainter', 'pageembed', 'a11ychecker', 'tinymcespellchecker', 'permanentpen', 'powerpaste', 'advtable', 'advcode', 'editimage', 'advtemplate', 'ai', 'mentions', 'tinycomments', 'tableofcontents', 'footnotes', 'mergetags', 'autocorrect', 'typography', 'inlinecss', 'markdown',
            //        // Early access to document converters
            //        'importword', 'exportword', 'exportpdf'
            //    ],
            //    toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | addcomment showcomments | spellcheckdialog a11ycheck typography | align lineheight | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
            //    tinycomments_mode: 'embedded',
            //    tinycomments_author: 'Author name',
            //    mergetags_list: [
            //        { value: 'First.Name', title: 'First Name' },
            //        { value: 'Email', title: 'Email' },
            //    ],
            //    ai_request: (request, respondWith) => respondWith.string(() => Promise.reject('See docs to implement AI Assistant')),
            //});

            //const markup = `
            //    <h2>
            //        <img src="https://js.devexpress.com/jQuery/Demos/WidgetsGallery/JSDemos/images/widgets/HtmlEditor.svg" alt="HtmlEditor">
            //        Formatted Text Editor (HTML Editor)
            //    </h2>
            //    <br>
            //    <p>DevExtreme JavaScript HTML Editor is a client-side WYSIWYG text editor that allows its users to format textual and visual content and store it as HTML or Markdown.</p>
            //    <p>Supported features:</p>
            //    <ul>
            //        <li>Inline formats:
            //            <ul>
            //                <li><strong>Bold</strong>, <em>italic</em>, <s>strikethrough</s> text formatting</li>
            //                <li>Font, size, color changes (HTML only)</li>
            //            </ul>
            //        </li>
            //        <li>Block formats:
            //            <ul>
            //                <li>Headers</li>
            //                <li>Text alignment</li>
            //                <li>Lists (ordered and unordered)</li>
            //                <li>Code blocks</li>
            //                <li>Quotes</li>
            //            </ul>
            //        </li>
            //        <li>Custom formats</li>
            //        <li>HTML and Markdown support</li>
            //        <li>Mail-merge placeholders (for example, %username%)</li>
            //        <li>Adaptive toolbar for working images, links, and color formats</li>
            //        <li>Image upload: drag-and-drop images onto the form, select files from the file system, or specify a URL.</li>
            //        <li>Copy-paste rich content (unsupported formats are removed)</li>
            //        <li>Tables support</li>
            //    </ul>
            //    <br>
            //    <p>Supported frameworks and libraries</p>
            //    <table>
            //        <tr>
            //            <td><strong>jQuery</strong></td>
            //            <td style="text-align: right;">v2.1 - v2.2 and v3.x</td>
            //        </tr>
            //        <tr>
            //            <td><strong>Angular</strong></td>
            //            <td style="text-align: right;">v7.0+</td>
            //        </tr>
            //        <tr>
            //            <td><strong>React</strong></td>
            //            <td style="text-align: right;">v16.2+</td>
            //        </tr>
            //        <tr>
            //            <td><strong>Vue</strong></td>
            //            <td style="text-align: right;">v2.6.3+</td>
            //        </tr>
            //    </table>
            //`;
            //$('.html-editor').dxHtmlEditor({
            //    height: 725,
            //    value: markup,
            //    imageUpload: {
            //        tabs: ['file', 'url'],
            //        fileUploadMode: 'base64',
            //    },
            //    toolbar: {
            //        items: [
            //            'undo', 'redo', 'separator',
            //            {
            //                name: 'size',
            //                acceptedValues: ['8pt', '10pt', '12pt', '14pt', '18pt', '24pt', '36pt'],
            //                options: { inputAttr: { 'aria-label': 'Font size' } },
            //            },
            //            {
            //                name: 'font',
            //                acceptedValues: ['Arial', 'Courier New', 'Georgia', 'Impact', 'Lucida Console', 'Tahoma', 'Times New Roman', 'Verdana'],
            //                options: { inputAttr: { 'aria-label': 'Font family' } },
            //            },
            //            'separator', 'bold', 'italic', 'strike', 'underline', 'separator',
            //            'alignLeft', 'alignCenter', 'alignRight', 'alignJustify', 'separator',
            //            'orderedList', 'bulletList', 'separator',
            //            {
            //                name: 'header',
            //                acceptedValues: [false, 1, 2, 3, 4, 5],
            //                options: { inputAttr: { 'aria-label': 'Header' } },
            //            }, 'separator',
            //            'color', 'background', 'separator',
            //            'link', 'image', 'separator',
            //            'clear', 'codeBlock', 'blockquote', 'separator',
            //            'insertTable', 'deleteTable',
            //            'insertRowAbove', 'insertRowBelow', 'deleteRow',
            //            'insertColumnLeft', 'insertColumnRight', 'deleteColumn',
            //        ],
            //    },
            //    mediaResizing: {
            //        enabled: true,
            //    },
            //}).dxHtmlEditor('instance');


        });


    },
    DeleteRecord: function () {

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
            ajaxHelper.ajaxPostJsonData({ id: $('#Code').val() }, "/Notes/Delete", function (data) {
                empr_helper.notify(data.msg, data.msgType);
                if (data.msgType == 1) {
                    empr_Notes.resetForm();
                    $('#optmodal').modal('hide');
                    $('.btn-delete').hide();
                    $('#printButton').hide();
                    $('#resetall').hide();
                }
            }, false, true);

        });

    },
    resetForm: function () {
        empr_Notes.initialDate = "";
        $("#Code").val('')
        //$("#GROUP_NAME").val('')
        empr_Notes.editorInstance.setData('');
        $("#hdnDOC").val('')
        $("input[type='radio']").prop("disabled", false);
        if (Permissions != "Admin") {
            if (Permissions.r_ADD) {
                $('#saveAttempt').show();
            } else {
                $('#saveAttempt').hide();
            }
        }
    },
    validateForm: function () {
        var valid = true;
        var GROUP_NAME = '';
        if (empr_Notes.editorInstance) {
            GROUP_NAME = empr_Notes.editorInstance.getData();
        }
        if ($("#V_DATE").val() == '') {
            empr_helper.notify("Date is required.", 2);
            valid = false;
            return valid;
        }
        //var GROUP_NAME = $("#GROUP_NAME").val().trim();
        if (GROUP_NAME.trim() == '') {
            valid = false;
            empr_helper.notify("Please enter notes.", 2);
        }

        var Code = $("#Code").val();
        var V_DATE = empr_Notes.initialDate;
        var currentDate = new Date();
        var inputDate = new Date(V_DATE);
        var timeDifference = currentDate.getTime() - inputDate.getTime();
        var dayDifference = timeDifference / (1000 * 3600 * 24);

        if (Code > 0 && dayDifference > 3) {
            valid = false;
            empr_helper.notify("You are not allowed to update notes after three days.", 2);
        }

        return valid;
    },
    getDataToSave: function () {
        var Code = $("#Code").val()
        var V_DATE = $("#V_DATE").val();
        var GROUP_NAME = empr_Notes.editorInstance.getData()
        //var GROUP_NAME = $("#GROUP_NAME").val()
        var DOC = $("#hdnDOC").val();
        var modelRecord = {
            GROUP_NAME: GROUP_NAME,
            GPIC: DOC,
            Code: Code,
            V_DATE: V_DATE
        }
        return modelRecord;
    },
    saveAttempt: function () {
        var obj = empr_Notes.getDataToSave();
        console.log(obj)
        var xhr = ajaxHelper.ajaxPostJsonData(obj, "/Notes/save", function (data) {
            empr_helper.notify(data.msg, data.msgType);
            if (data.msgType == 1) {
                empr_Notes.resetForm();
                $('.btn-delete').hide();
                $('#printButton').hide();
                $('#resetall').hide();
            }
        }, false, true);
    },
    InintQuickSearch: function () {
        empr_Notes.GetAllNotess();
    },
    GetAllNotess: function () {
        var xhr = ajaxHelper.ajaxGetJson('/Notes/QuickSearch', function (data) {
            empr_Notes.CreateGrid(data.data);
        }, false, true);
    },
    GetNotesByID: function (id) {
        var xhr = ajaxHelper.ajaxGetJson('/Notes/NotesByid?id=' + id, function (data) {
            if (Permissions != "Admin") {
                if (Permissions.r_DLT) {
                    $('.btn-delete').show();
                }
                if (Permissions.r_EDIT) {
                    $('#saveAttempt').show();
                }
                else {
                    $('#saveAttempt').hide();
                }
            } else {
                $('#saveAttempt').show();
                $('.btn-delete').show();
            }
            $('#printButton').show();
            $('#resetall').show();
            $('.modal').modal('hide')
            $('#Code').val(data.data.grouP_CODE);
            $('#V_DATE').val(data.data.v_DATE);
            empr_Notes.initialDate = data.data.v_DATE;
            empr_Notes.editorInstance.setData(data.data.grouP_NAME);
            //$('#GROUP_NAME').val(data.data.grouP_NAME);
            $('#hdnDOC').val(data.data.gpic);
        }, false, true);
    },
    CreateGrid: function (dataSrc) {
        var col = [{
            dataField: "Action",
            width: 100,
            alignment: 'center',
            fixed: true,
            fixedPosition: "left",
            allowExporting: false,
            cellTemplate: function (container, options) {
                debugger

                var data = JSON.stringify(options.data);
                var data_ = encodeURI(data);

                $(`<div class="btn-group btn-group-sm">
                                <a href="javascript:;"  class="grid-action-icon elm_edit" rportid=${options.data.id} title="Edit"><i class="fa fa-edit"></i></a>
                                </div>`).appendTo(container);


            }
        },
        { dataField: 'id', caption: 'Code', width: 80, alignment: "center" },
        { dataField: 'adD_DATE', caption: 'Date', dataType: 'date', format: 'dd-MM-yyy' },
            /*{ dataField: 'grouP_NAME', caption: 'Note' },*/
        ];
        empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Notes");
    },
    UploadDoc: function () {
        $('#saveAttempt').prop('disabled', true);
        var files = document.getElementById('DOC').files;
        var formData = new FormData();
        for (var i = 0; i !== files.length; i++) {
            formData.append("model", files[i]);
        }
        $.ajax(
            {
                url: "/Common/UploadVoucherDocs",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (data) {
                    if (data.msgType == '1') {
                        $("#hdnDOC").val(data.data);
                    }
                    else {
                        empr_helper.notify("Something went wrong while saving the file. please re-upload the file.", data.msgType);
                    }
                    $('#saveAttempt').prop('disabled', false);

                }
            }
        );
    },
    OpenDoc: function () {
        var hdnUrl = $('#hdnDOC').val();
        if (hdnUrl == "" || hdnUrl == null) {
            empr_helper.notify("Please upload a file to view.", 2);
        }
        else {
            const fileURL = window.location.origin + hdnUrl;
            window.open(fileURL, '_blank');
        }
    },
    //Print: function () {
    //    const editorContent = empr_Notes.editorInstance.getData();
    //    const currentDate = $("#V_DATE").val();
    //    const printContent = `
    //                <div>
    //                    <p style="color: #055a87; font-size: 18px; font-weight: bold;">
    //                        ${empr_Notes.companyName}
    //                    </p>
    //                    <p style="color: #055a87; font-size: 14px;">
    //                        ${empr_Notes.companyAddress}
    //                    </p>
    //                    <hr>
    //                    <p><strong>Date:</strong> ${currentDate}</p>
    //                    </br>
    //                    <p><strong>Note:</strong></p>
    //                    ${editorContent}
    //                </div>
    //            `;
    //    const printWindow = window.open("", "_blank");
    //    printWindow.document.open();
    //    printWindow.document.write(`
    //                <!DOCTYPE html>
    //                <html>
    //                <head>
    //                    <title>Note - ${currentDate}</title>
    //                    <style>
    //                        body { font-family: Arial, sans-serif; margin: 20px; }
    //                        p { margin: 5px 0; }
    //                    </style>
    //                </head>
    //                <body>
    //                    ${printContent}
    //                </body>
    //                </html>
    //            `);
    //    printWindow.document.close();
    //    printWindow.print();
    //},

    Print: function () {
        const editorContent = empr_Notes.editorInstance.getData();
        console.log(editorContent)
        const currentDate = $("#V_DATE").val();
        const printContent = `
        <div>
            <p style="color: #055a87; font-size: 18px; font-weight: bold;">
                ${empr_Notes.companyName}
            </p>
            <p style="color: #055a87; font-size: 14px;">
                ${empr_Notes.companyAddress}
            </p>
            <hr>
            <p><strong>Date:</strong> ${currentDate}</p>
            <br>
            <p><strong>Note:</strong></p>
            ${editorContent}
        </div>
    `;
        const iframe = document.createElement("iframe");
        iframe.style.position = "absolute";
        iframe.style.width = "0";
        iframe.style.height = "0";
        iframe.style.border = "none";
        document.body.appendChild(iframe);
        const doc = iframe.contentDocument || iframe.contentWindow.document;
        doc.open();
        doc.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Note - ${currentDate}</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                p { margin: 5px 0; }
                table { 
                    width: 100%; 
                    border-collapse: collapse;
                    border: 1px solid #000;
                }
                th, td { 
                    border: 1px solid #000;
                    padding: 8px; 
                    text-align: left; 
                }
                th { 
                    font-weight: bold;
                    background-color: #f2f2f2;
                }
                @media print {
                    @page { margin: 0; }
                    body { margin: 1cm; }
                }
            </style>
        </head>
        <body>
            ${printContent}
        </body>
        </html>
    `);
        doc.close();
        iframe.contentWindow.focus();
        iframe.contentWindow.print();
        setTimeout(() => {
            document.body.removeChild(iframe);
        }, 1000);
    }
}