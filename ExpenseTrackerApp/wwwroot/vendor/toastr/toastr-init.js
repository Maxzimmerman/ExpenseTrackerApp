(function ($) {
    "use strict";

    /**
     * Displays a toast notification
     * @param {string} message - The message to display in the toast.
     */
    function showMessage(message) {
        toastr.success(message, "Notification", {
            timeOut: 500000,
            closeButton: true,
            debug: false,
            newestOnTop: true,
            progressBar: true,
            positionClass: "toast-top-right demo_rtl_class",
            preventDuplicates: true,
            onclick: null,
            showDuration: "300",
            hideDuration: "1000",
            extendedTimeOut: "1000",
            showEasing: "swing",
            hideEasing: "linear",
            showMethod: "fadeIn",
            hideMethod: "fadeOut",
            tapToDismiss: false,
            closeHtml: '<span class="progress-count"></span> <i class="close-toast fi fi-rr-cross-small"></i> <a href="#">Suggest</a>'
        });
    }

    // Expose the function to the global scope
    window.showMessage = showMessage;

})(jQuery);
