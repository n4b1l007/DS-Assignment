$(document).ajaxStop($.unblockUI); 

toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
function showSuccess(msg, title) {
    toastr.success(msg, title)
}
function showError(msg, title) {
    toastr.error(msg, title)
}
function showWarning(msg, title) {
    toastr.warning(msg, title)
}