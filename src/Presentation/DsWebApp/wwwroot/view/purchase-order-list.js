$(document).ready(function () {
    var purchaseOrdersTable = '#purchaseOrdersTable';
    var createButtonHtml = '<button id="createButton" class="btn btn-sm btn-success"><i class="fas fa-plus"></i> Create</button>';

    var table = $(purchaseOrdersTable).DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": baseUrl + "/api/purchaseorders",
            "type": "GET",
            "data": function (d) {
                return {
                    start: d.start,
                    length: d.length,
                    search: d.search.value,
                    orderColumn: d.columns[d.order[0].column].data,
                    orderDir: d.order[0].dir
                };
            }
        },
        "columns": [
            { "data": "id", "visible": false },
            { "data": "referenceId" },
            { "data": "orderNumber" },
            { "data": "supplierId" },
            {
                "data": "orderDate",
                "render": function (data, type, row) {
                    return new Date(data).toLocaleDateString();
                }
            },
            {
                "data": "expectedDate",
                "render": function (data, type, row) {
                    return new Date(data).toLocaleDateString();
                }
            },
            {
                "data": null,
                "render": function (data, type, row) {
                    return `
                                                <button class="btn btn-sm btn-primary edit-btn" data-id="${row.id}">
                                                    <i class="fas fa-edit"></i>
                                                </button>
                                                <button class="btn btn-sm btn-danger delete-btn" data-id="${row.id}">
                                                    <i class="fas fa-trash-alt"></i>
                                                </button>
                                                <button class="btn btn-sm btn-info report-btn" data-id="${row.id}">
                                                    <i class="fas fa-file-alt"></i>
                                                </button>
                                            `;
                }
            }
        ],
        "dom": '<"toolbar">frtip',
        "initComplete": function () {
            $("div.toolbar").html(createButtonHtml);
        }
    });

    $(document).on('click', '#createButton', function () {
        window.location.href = '/PurchaseOrderCreateUpdate';
    });

    $(purchaseOrdersTable).on('click', '.edit-btn', function () {
        var id = $(this).data('id');
        window.location.href = '/PurchaseOrderCreateUpdate/' + id;

    });

    $(purchaseOrdersTable).on('click', '.delete-btn', function () {
        var button = $(this);
        var row = button.closest('tr');
        var id = $(this).data('id');
        $.ajax({
            url: baseUrl + '/api/purchaseorders/' + id,
            type: 'DELETE',
            success: function (result) {
                window.location.reload();
            },
            error: function (xhr, status, error) {
                alert('Error: ' + xhr.responseText);
            }
        });
    });

    $(purchaseOrdersTable).on('click', '.report-btn', function () {
        var id = $(this).data('id');
        var url = baseUrl + '/api/reports/' + id + '/pdf';
        fetch(url)
            .then(response => response.json())
            .then(data => {
                const pdfViewer = document.getElementById('pdfViewer');
                pdfViewer.src = 'data:application/pdf;base64,' + data.pdfContent;
                $('#pdfPopup').modal('show');
            })
            .catch(error => console.error('Error:', error));
    });


});