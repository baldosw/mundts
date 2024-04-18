var tableTitle = document.getElementById("tableTitle");

var dataTable;

// tinymce.init({
//     selector: 'textarea',
//     plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount',
//     toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
// });

var qrCodeDiv = document.getElementById('qrcode');

var qrCode;

if(qrCodeDiv !== null  && qrCodeDiv !== 'undefined'){
    qrCode = new QRCode(qrCodeDiv, {
        text: '',
        width: 128,
        height: 128
    });
}
 
function updateQRCode(content) {
    qrCode.clear();  
    qrCode.makeCode(content);  
}
 
var idioma =
{
    "sProcessing": "Processing...",
    "sLengthMenu": "Length _MENU_ rows",
    "sZeroRecords": "No record found",
    "sEmptyTable": "Empty records",
    "sInfo": "Show row from _START_ to _END_ Total _TOTAL_ Rows",
    "sInfoEmpty": "No rows to display 0 ",
    "sInfoFiltered": "(Filter total _MAX_ rows)",
    "sInfoPostFix": "",
    "sSearch": "Search:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Loading...",
    "oPaginate": {
        "sFirst": "First",
        "sLast": "Last",
        "sNext": "Next",
        "sPrevious": "Previous"
    },
    "oAria": {
        "sSortAscending": ": Sort Ascending",
        "sSortDescending": ": Sort Descending"
    },
    "buttons": {
        "copyTitle": 'Copy Title',
        "copyKeys": 'Use your keyboard or menu to select the copy command',
        "copySuccess": {
            "_": '%d files has been copied',
            "1": '1 file has been copied'
        },

        "pageLength": {
            "_": "Show %d rows",
            "-1": "Display All Rows"
        }
    }
};

function displayValidationError(errorField, errorMessage){
    if (validateValueIfNullOrUndefined(errorMessage)){
        $(errorField).text(errorMessage)
       
        clientErrors.push(errorMessage)
    }
}

function checkIfPropertyExistsThenGetValue(arrayObj, propertyName, callback) {
    arrayObj.forEach(function(obj) {
        if (obj.hasOwnProperty(propertyName)) {
            callback(obj[propertyName]);
            return;
        }
    });
    callback(null);
}

function convertDateTimeToTimeStamp(dateTime){
    let newDateTime = new Date(dateTime);
    return newDateTime.getTime() / 1000;  
}

// 
// function checkIfPropertyExistsThenGetValue(arrayObj, propertyName, callback) {
//     arrayObj.forEach(function(obj) {
//         if (obj.hasOwnProperty(propertyName)) {
//             callback(obj[propertyName]);
//             return;
//         }
//     });
// 
//     callback(null);
// }
function validateInput(field, value, minimumLength, maximumLength) {
 
    if (!value) {
        return `${field} is required.`;
    }

    if (value.length < minimumLength || value.length > maximumLength) {
        return `${field} value cannot exceed ${maximumLength} characters and should not be less than ${minimumLength} characters.`;
    }
    
    return null;
}

function validateSelect(field, value) {
    if (!value) {
        return `${field} is required.`;
    }
    return null;
}

function validateValueIfNullOrUndefined(value) {
    if (value === null || value === undefined) {
        return false;  
    } else {
        return true;  
    }
}
//
function clearErrorMessages(){
    $('.errorFieldText').text('')
}
 
function pushNotify(changesTitle, changesText, changesStatus) {
    new Notify({
        status:  changesStatus,
        title: changesTitle,
        text: changesText,
        effect: 'fade',
        speed: 300,
        customClass: null,
        customIcon: null,
        showIcon: true,
        showCloseButton: true,
        autoclose: true,
        autotimeout: 3000,
        gap: 20,
        distance: 20,
        type: 'outline',
        position: 'right top'
    })
}
function generateTrackingCode(length = 12) {
    const alphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const numericChars = "0123456789";
    let trackingCode = "";

    for (let i = 0; i < length; i++) {
        if (i < 6) {
            trackingCode += alphabetChars.charAt(Math.floor(Math.random() * alphabetChars.length));
        } else {
            trackingCode += numericChars.charAt(Math.floor(Math.random() * numericChars.length));
        }
    }

    return trackingCode.toUpperCase();
}
  
function converDate(strDate) {
    var date = new Date(strDate);

    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    return { month, day, year }

}

function convertDateToLabel(strDate) {
    let date = converDate(strDate)

    return date.month + "-" + date.day + "-" + date.year;
}

function converDateToInputBox(strDate) {

    let date = converDate(strDate)

    return date.year + "-" + date.month + "-" + date.day;
}

// Delete 

function OnClickDelete(url, entity, returlUrl = "") {

    Swal.fire({
        title: `Are you sure you want to delete this ${entity}?`,
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            try {
             
                $.ajax({
                    url: url,
                    type: 'DELETE',
                    success: function (data) {
                        console.log(data)
                        if (data.success) {
                             
                            console.log("RETURNURL", returlUrl)
                            
                            if(returlUrl !== ""){
                                window.location.href = returlUrl;
                            }
                            
                            if(dataTable != null){
                                dataTable.ajax.reload();
                            }
                             
                            toastr.success(data.message);
                            Swal.fire(
                                'Deleted!',
                                `The ${entity} has been deleted.`,
                                'success'
                            )
                        } else {
                            
                            toastr.error(data.message)
                           
                        }
                    },
                    error: function (err) {
                        console.log('ERROR:', err)
                    }

                }
                )

            } catch (e) {
               

                Swal.fire(
                    'Ooops!',
                    'Something went wrong',
                    'failed'
                )
            }

        }
    })
}

function loadDocumentFromDatabase(urlFromClient){
 
    $(document).ready(function() {
        $('.errorFieldText').text('')
        
        $.ajax({
            url: urlFromClient,
            type: 'GET',
            dataType: 'json',
            success: function(response) {
                if (response && response.data && response.data.title) {
                    // Populate the input box within the modal with the title
                    console.log(response.data)
                    $('#updateTitle').val(response.data.title);
                    $('#updateContent').val(response.data.content);
                    $('#updateRemarks').val(response.data.remarks);
                    $('#updateDepartmentId').val(response.data.departmentId);
                    $('#updateRequestTypeId').val(response.data.requestTypeId);
                    $('#updateTrackingCode').text(response.data.trackingCode);
                    $('#updateId').val(response.data.id);
                    $('#updateRouteDepartmentId').val(response.data.routeDepartmentId)
                    $('#modalUpdateDocument').modal('show');
                } else {
                    console.error('Title not found in the response.');
                }
            },
            error: function(xhr, status, error) {

                console.error(xhr.responseText);
            }
        });
    });
}

 
function loadIncomingData(urlFromClient){
    $.ajax({
        url: urlFromClient,
        type: 'GET',
        dataType: 'json',
        success: function(response) {
            if (response && response.data && response.data.title) {
                $('#documentId').val(response.data.id);
            }
        },
        error: function(xhr, status, error) {

             
        }
    });
}

function loadPrintDocument(urlFromClient){
    
    $.ajax({
        url: urlFromClient,
        type: 'GET',
        dataType: 'json',
        success: function(response) {
            if (response && response.data && response.data.title) {
                $('#printDocumentTrackingCode').text(response.data.trackingCode);
                $('#printDocumentDepartmentName').text(response.data.departmentName);
                $('#printDocumentTitle').text(response.data.title);
                $('#printDocumentContent').text(response.data.content);
                $('#printDocumentRequestTypeTitle').text(response.data.requestTypeTitle);
                $('#printDocumentRemarks').text(response.data.remarks);
                
                updateQRCode(response.data.trackingCode)
                 
                $('#modalPrintDocument').modal('show');
            } else {
                console.error('Title not found in the response.');
            }
        },
        error: function(xhr, status, error) {

            console.error(xhr.responseText);
        }
    });
    
   
}
  
//------------------- Get All Documents -------------------------------------
 
var documentColumns = {
    "processing": true,
    "serverSide": true,
    "deferLoading": 10, // Load 10 records initially
    "paging": true, // Enable paging
    "pagingType": "full_numbers",
    filter: true,
    ajax: {
        url: '/user/document/getdocuments',
    },
    col: [
        { data: 'id' },
        {data: 'department', width: '5%'},
        { data: 'trackingCode', width: '5%' },
        { data: 'title', width: '20%' },         
        { data: 'content', width: '30%'  },
        {data: 'requestType', width: '10%'},
        {data: 'remarks', width: '30%'},
        {data:'createdTimestamp', visible: false},
        {
            data: 'id',
            "render": function (data, type, row) {
                
                if(row.statusId === 2 || row.statusId === 4)
                {
                    return `
                        <div class="d-flex justify-content-center">
                            <a class="btn btn-info btn-hover text-end text-white d-block d-flex justify-content-center align-items-center dropdown-toggle pl-2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="width: 30px; height: 30px">                                                
                            </a>
                             <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"   >                                 
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" onclick='loadPrintDocument("/user/document/getdocument/${data}")'>
                                   <i class="bi bi-printer"></i>
                                    Print
                                </a>                          
                            </div>                             
                        </div>
                    `;
                }else{
                    return `
                        <div class="d-flex justify-content-center">
                            <a class="btn btn-info btn-hover text-end text-white d-block d-flex justify-content-center align-items-center dropdown-toggle pl-2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="width: 30px; height: 30px">                                                
                            </a>
                             <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"   >
                                <a class="dropdown-item" href="#" onclick='loadDocumentFromDatabase("/user/document/getdocument/${data}")' style = "font-size: 12px !important;"   id = "btnUpdateDocumentModal" >
                                <i class="bi bi-pencil-square" ></i>
                                Update</a>
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" onclick='loadPrintDocument("/user/document/getdocument/${data}")'>
                                   <i class="bi bi-printer"></i>
                                    Print
                                </a>                          
                            </div>                             
                        </div>
                    `;
                }
                
                
            }, width: "5%"
        }
    ],
    colDefs: [
        {
            targets: [0], // index of the column you want to hide
            visible: false, // hide the column
            searchable: true // allow searching on this column
        }
        ],
    'select': {
        'style': 'multi'
    },
    
    'order': [[0, 'desc']]
}


//--------------------- Outgoing Documents-----------------------------
 
var outgoingDocumentColumns = {
    "processing": true,
    "serverSide": true,
    "deferLoading": 10, // Load 10 records initially
    "paging": true, // Enable paging
    "pagingType": "full_numbers",
    filter: true,
    ajax: {
        url: '/user/documentstatus/getoutgoingdocuments',
    },
    col: [
        { data: 'id' },
        {data: 'department', width: '5%'},
        { data: 'trackingCode', width: '5%' },
        { data: 'title', width: '20%' },
        { data: 'content', width: '30%'  },
        {data: 'requestType', width: '10%'},
        {data: 'remarks', width: '30%'},
        {data:'createdTimestamp', visible: false},
        {
            data: 'id',
            "render": function (data) {
                return `
                        <div class="d-flex justify-content-center">
                            <a class="btn btn-info btn-hover text-end text-white d-block d-flex justify-content-center align-items-center dropdown-toggle pl-2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="width: 30px; height: 30px">                                                
                            </a>
                             <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"   >                                
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" data-toggle="modal" data-target="#cancelModal" onclick='loadIncomingData("/user/document/getdocument/${data}")'  >
                                   <i class="bi bi-check"></i>
                                   Cancel
                                </a>     
                                 <a class="dropdown-item" href="#" style = "font-size: 12px !important;" onclick='loadPrintDocument("/user/document/getdocument/${data}")'>
                                   <i class="bi bi-eye"></i>
                                    Details
                                </a>                                                           
                            </div>                             
                        </div>
                    `;
            }, width: "5%"
        }
    ],
    colDefs: [
        {
            targets: [0], // index of the column you want to hide
            visible: false, // hide the column
            searchable: true // allow searching on this column
        }
    ],
    'select': {
        'style': 'multi'
    },

    'order': [[0, 'desc']]
}


//--------------------- Incoming Documents-----------------------------

var incomingDocumentColumns = {
    "processing": true,
    "serverSide": true,
    "deferLoading": 10, // Load 10 records initially
    "paging": true, // Enable paging
    "pagingType": "full_numbers",
    filter: true,
    ajax: {
        url: '/user/documentstatus/getincomingdocuments',
    },
    col: [
        { data: 'id' },
        {data: 'department', width: '5%'},
        { data: 'trackingCode', width: '5%' },
        { data: 'title', width: '20%' },
        { data: 'content', width: '30%'  },
        {data: 'requestType', width: '10%'},
        {data: 'remarks', width: '30%'},
        {data:'createdTimestamp', visible: false},
        {
            data: 'id',
            "render": function (data) {
                return `
                        <div class="d-flex justify-content-center">
                            <a class="btn btn-info btn-hover text-end text-white d-block d-flex justify-content-center align-items-center dropdown-toggle pl-2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="width: 30px; height: 30px">                                                
                            </a>
                             <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"   >                                
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" data-toggle="modal" data-target="#receiveModal" onclick='loadIncomingData("/user/document/getdocument/${data}")'  >
                                   <i class="bi bi-check"></i>
                                   Receive
                                </a>    
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" onclick='loadPrintDocument("/user/document/getdocument/${data}")'>
                                   <i class="bi bi-eye"></i>
                                    Details
                                </a>                      
                            </div>                             
                        </div>
                    `;
            }, width: "5%"
        }
    ],
    colDefs: [
        {
            targets: [0], // index of the column you want to hide
            visible: false, // hide the column
            searchable: true // allow searching on this column
        }
    ],
    'select': {
        'style': 'multi'
    },

    'order': [[0, 'desc']]
}

 // --------------------- Get Documents By Page ---------------------------------
 
var documentPageColumns = {
    "processing": true,
    "serverSide": true,
    filter: true,
    ajax: function ( data, callback, settings ) {

        $.ajax({
            url: 'http://localhost:64506/api/values',
            // dataType: 'text',
            type: 'post',
            contentType: 'application/x-www-form-urlencoded',
            data: {
                RecordsStart: data.start,
                PageSize: data.length
            },
            success: function( data, textStatus, jQxhr ){
                callback({
                    // draw: data.draw,
                    data: data.Data,
                    recordsTotal:  data.TotalRecords,
                    recordsFiltered:  data.RecordsFiltered
                });
            },
            error: function( jqXhr, textStatus, errorThrown ){
            }
        });
    },     
    col: [
        { data: 'id' },
        {data: 'department'},
        { data: 'trackingCode' },
        { data: 'title' },
        { data: 'content', className: 'truncate'  },
        {data: 'requestType'},
        {data: 'remarks'},
        {data:'createdTimestamp', visible: false},
        {
            data: 'id',
            "render": function (data) {
                return `
                        <div class="d-flex justify-content-center">
                            <a class="btn btn-info btn-hover text-end text-white d-block d-flex justify-content-center align-items-center dropdown-toggle pl-2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="width: 30px; height: 30px">                                                
                            </a>
                             <div class="dropdown-menu" aria-labelledby="dropdownMenuButton"   >
                                <a class="dropdown-item" href="#" onclick='loadDocumentFromDatabase("/user/document/getdocument/${data}")' style = "font-size: 12px !important;"   id = "btnUpdateDocumentModal" >
                                <i class="bi bi-pencil-square" ></i>
                                Update</a>
                                <a class="dropdown-item" href="#" style = "font-size: 12px !important;" onclick='loadPrintDocument("/user/document/getdocument/${data}")'>
                                   <i class="bi bi-printer"></i>
                                    Print
                                </a>                          
                            </div>                             
                        </div>
                    `;
            }, width: "5%"
        }
    ],
    colDefs: [
        {
            targets: [0], // index of the column you want to hide
            visible: false, // hide the column
            searchable: true // allow searching on this column
        }
    ],
    'select': {
        'style': 'multi'
    },

    'order': [[0, 'desc']],
    
}


 // Load Data Tables
 
 function loadDataTable(ajaxColumns) {
    dataTable = $('#dataTable').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": true,
        "language": idioma,
        // "lengthMenu": [[5, 10, 20, -1], [5, 10, 50, "Display All"]],
        // lengthMenu: [],
        dom: 'Bfrt<"col-md-6 inline"i> <"col-md-6 inline"p>',
        ajax: ajaxColumns.ajax.url,
        columns: ajaxColumns.col,
        columnDefs: ajaxColumns.colDefs,
        order: ajaxColumns.order,
        select: ajaxColumns.select,
        buttons: {
            dom: {
                container: {
                    tag: 'div',
                    className: 'flexcontent'
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [

                {
                    extend: 'excelHtml5',
                    text: '<i class="fa fa-file-excel-o"></i> Excel',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export excel btn-success',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'csvHtml5',
                    text: '<i class="fa fa-file-text-o"></i> CSV',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export csv btn-info',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'print',
                    text: '<i class="fa fa-print"></i> Print',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export imprimir btn-dark',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'pageLength',
                    titleAttr: 'Registros a mostrar',
                    className: 'selectTable'
                }
            ]
        },
        "createdRow": function(row, data, index) {
            $(row).find('.paginate_button').css('font-size', '12px'); // Adjust font size
        }
    });


    return dataTable;
}
 
function loadSearchTable(ajaxColumns){

    dataTable = $('#dataTable').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "processing": false,
        "serverSide": true,
        "info": true,
        "autoWidth": true,
        "language": idioma,
        "lengthMenu": [[5, 10, 20, -1], [5, 10, 50, "Display All"]],
        dom: 'Bfrt<"col-md-6 inline"i> <"col-md-6 inline"p>',
        ajax: ajaxColumns.ajax.url,
        columns: ajaxColumns.col,
        columnDefs: ajaxColumns.colDefs,
        order: ajaxColumns.order,
        select: ajaxColumns.select,
        buttons: {
            dom: {
                container: {
                    tag: 'div',
                    className: 'flexcontent'
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [

                {
                    extend: 'excelHtml5',
                    text: '<i class="fa fa-file-excel-o"></i> Excel',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export excel btn-success',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'csvHtml5',
                    text: '<i class="fa fa-file-text-o"></i> CSV',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export csv btn-info',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'print',
                    text: '<i class="fa fa-print"></i> Print',
                    title: tableTitle.textContent,
                    className: 'btn btn-app export imprimir btn-dark',
                    exportOptions: {
                        columns: ':not(.not-export-column)'
                    }
                },
                {
                    extend: 'pageLength',
                    titleAttr: 'Registros a mostrar',
                    className: 'selectTable'
                }
            ]


        },
        "initComplete": function (settings, json) {
            // Access the JSON response data here
            console.log(json);
        }

    });


    return dataTable;
}


function loadSelectionTable(ajaxColumns) {
    dataTable = $('.selectionTable').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": true,
        "language": idioma,
        "lengthMenu": [[5, 10, 20, -1], [5, 10, 50, "Display All"]],
        dom: 'Bfrt<"col-md-6 inline"i> <"col-md-6 inline"p>',
        ajax: ajaxColumns.ajax.url,
        columns: ajaxColumns.col,
        'columnDefs': [
            {
                'targets': 0,
                'checkboxes': {
                    'selectRow': true
                }
            }
        ],
        'select': {
            'style': 'multi'
        },
        'order': [[1, 'asc']],
        buttons: {
            dom: {
                container: {
                    tag: 'div',
                    className: 'flexcontent'
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [

                {
                    extend: 'pageLength',
                    titleAttr: 'Registros a mostrar',
                    className: 'selectTable'
                }
            ]


        },


    });

    return dataTable;
}


