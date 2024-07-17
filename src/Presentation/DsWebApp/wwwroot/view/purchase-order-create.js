var itemGridName = "ProductionOrderItems";
$(function () {
    let orderDetails = [];
    $('#SupplierId').select2({
        ajax: {
            url: baseUrl + '/api/supplier/search',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    term: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.result
                };
            },
            cache: true
        },
        theme: 'bootstrap-5',
        placeholder: 'Select an Supplier',
        minimumInputLength: 0,
        dropdownAutoWidth: true,
        width: '100%'
    });

    var dateconfig = {
        singleDatePicker: true,
        showDropdowns: true,
        autoApply: true, // Close the picker when a date is selected
        allowInputToggle: false, // Disable typing in the input field
        startDate: moment(),
        locale: {
            format: 'DD-MMM-YYYY'
        }
    };
    var myGrid = new AppendGrid({
        element: itemGridName,
        uiFramework: "bootstrap5",
        iconFramework: "bootstrapicons",
        iconParams: {
            baseUrl: "https://cdn.jsdelivr.net/npm/bootstrap-icons/icons/"
        },
        hideButtons: {
            moveUp: true,
            moveDown: true,
            insert: true
        },
        columns: [
            {
                name: "itemId",
                display: "Item Name",
                type: 'custom',
                ctrlAttr: {
                    required: "required"
                },
                customBuilder: function (parent, idPrefix, name, uniqueIndex) {
                    var selectControl = document.createElement("select");
                    selectControl.id = idPrefix + "_" + name + "_" + uniqueIndex;
                    selectControl.name = selectControl.id;
                    selectControl.classList.add("form-control");
                    parent.appendChild(selectControl);
                },
                customGetter: function (idPrefix, name, uniqueIndex) {
                    var controlId = idPrefix + "_" + name + "_" + uniqueIndex;
                    return parseFloat(document.getElementById(controlId).value);
                },
                customSetter: function (idPrefix, name, uniqueIndex, value) {
                    var controlId = idPrefix + "_" + name + "_" + uniqueIndex;
                    var orderDetail = orderDetails[uniqueIndex - 1];
                    var option = new Option(orderDetail.itemName, orderDetail.itemId, true, true);
                    $('#' + controlId).append(option).trigger('change');
                },
                cellCss: { 'width': '30%' }
            },
            {
                name: "quantity",
                display: "Qty",
                type: "number",
                ctrlAttr: {
                    required: "required",
                    min: "1"
                },
                cellCss: { 'width': '30%' }
            },
            {
                name: "rate",
                display: "Rate ($)",
                type: "number",
                ctrlAttr: {
                    required: "required",
                    min: "1"
                },
                cellCss: { 'width': '30%' }
            },
            {
                name: "id",
                type: "hidden",
                value: "0"
            }
        ],
        sectionClasses: {
            table: "table-sm",
            control: "form-control-sm",
            buttonGroup: "btn-group-sm"
        },
        afterRowAppended: function (caller, parentRowIndex, addedRowIndex) {
            newRowAdded(caller, parentRowIndex, addedRowIndex);
        },
        afterRowInserted(caller, parentRowIndex, addedRowIndex) {
            newRowAdded(caller, parentRowIndex, addedRowIndex);
        }
    });
    $('#OrderDate').daterangepicker(dateconfig);
    $('#ExpectedDate').daterangepicker(dateconfig);
    $("#data-form").parsley({
        errorClass: "is-invalid",
        errorsWrapper: '<span class="invalid-feedback"></span>',
        errorTemplate: "<div></div>"
    });
    $('#saveButton').click(function () { 
        var isValid = $('#data-form').parsley().validate();
        if (!isValid) {
            return;
        } 
        save();
    });
    $("#cancelButton").click(function () {
        goback();
    });
    function save() {
        var formData = {};
        if ($('#Id').val()) {
            formData.Id = $('#Id').val();
        }
        formData.ReferenceId = $('#ReferenceId').val();
        formData.OrderDate = $('#OrderDate').val();
        formData.ExpectedDate = $('#ExpectedDate').val();
        formData.OrderNumber = $('#OrderNumber').val();
        formData.SupplierId = $('#SupplierId').val();
        formData.Remark = $('#Remark').val();
        var items = myGrid.getAllValue();
        formData.OrderDetails = items;

        const hasDuplicateItems = checkDuplicatesItems(items);
        if (formData.OrderDetails.length == 0) {
            showWarning("Please enter minimum 1 item in order detail");
        }
        else if (hasDuplicateItems) {
            showWarning("Thete Is Duplicate Items In Order Details Grid");
        }
        else {
            $.blockUI({ message: '<h4><img src="/images/Iphone-spinner-2.gif" /> Just a moment...</h4>' });
            var jsonData = JSON.stringify(formData);

            $.ajax({
                url: baseUrl + '/api/purchaseorders',
                type: 'POST',
                contentType: 'application/json',
                data: jsonData,
                success: function (response) {
                    if (response.isSuccess) {
                        showSuccess("Data Saved", "Data Saved");
                        clean();
                        $('#data-form').parsley().reset();
                    }
                    else {
                        showError(response.Message, "ERROR");
                    }
                },
                error: function (error) {
                    showError(error, "ERROR");
                }
            });
            if ($('#Id').val()) {
                goback();
            }
        }
    }
    function clean() {
        $('#Id').val('');
        $('#ReferenceId').val('');
        var orderDate = moment().startOf('day');
        $('#OrderDate').data('daterangepicker').setStartDate(orderDate);
        $('#OrderDate').data('daterangepicker').setEndDate(orderDate);
        var expectedDate = moment().startOf('day');
        $('#ExpectedDate').data('daterangepicker').setStartDate(expectedDate);
        $('#ExpectedDate').data('daterangepicker').setEndDate(expectedDate);
        $('#OrderNumber').val('');
        $('#SupplierId').val('').trigger('change');
        $('#Remark').val('');

        cleanGrid();
    }
    function cleanGrid() {
        var rowOrder = myGrid.getRowOrder();
        for (let i = 0; i < rowOrder.length; i++) {
            myGrid.removeRow(0);
        }
        myGrid.appendRow(3);
    }
    function newRowAdded(caller, parentRowIndex, addedRowIndex) {
        for (let i = 0; i < addedRowIndex.length; i++) {
            var thisRowIndex = addedRowIndex[i];
            var thisRowItem = itemGridName + "_itemId_" + thisRowIndex;
            $('#' + thisRowItem).select2({
                ajax: {
                    url: baseUrl + '/api/items/search',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            term: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.result
                        };
                    },
                    cache: true
                },
                theme: 'bootstrap-5',
                containerCssClass: 'form-control-sm',
                placeholder: 'Select an Item',
                minimumInputLength: 0,
                dropdownAutoWidth: true,
                width: '100%'
            });
        }
    };
    function goback() {
        window.location = "/PurchaseOrders";
    }
    if ($('#Id').val()) {
        $.ajax({
            url: baseUrl + '/api/purchaseorders/' + $('#Id').val(),
            method: 'GET',
            success: function (data) {
                var orderDate = moment(data.orderDate, 'DD-MMM-YYYY');
                $('#OrderDate').data('daterangepicker').setStartDate(orderDate);
                $('#OrderDate').data('daterangepicker').setEndDate(orderDate);
                var expectedDate = moment(data.expectedDate, 'DD-MMM-YYYY');
                $('#ExpectedDate').data('daterangepicker').setStartDate(expectedDate);
                $('#ExpectedDate').data('daterangepicker').setEndDate(expectedDate);

                $('#ReferenceId').val(data.referenceId);
                $('#OrderNumber').val(data.orderNumber);
                $('#Remark').val(data.remark);

                var option = new Option(data.supplierName, data.supplierId, true, true);
                $('#SupplierId').append(option).trigger('change');
                orderDetails = data.orderDetails;
                myGrid.load(data.orderDetails);
            },
            error: function (xhr, status, error) {
            }
        });
    }

    function checkDuplicatesItems (arr) {
        const itemIdCount = arr.reduce((acc, item) => {
            acc[item.itemId] = (acc[item.itemId] || 0) + 1;
            return acc;
        }, {});

        return Object.values(itemIdCount).some(count => count > 1);
    };
});

