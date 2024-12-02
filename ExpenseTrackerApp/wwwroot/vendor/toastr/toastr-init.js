(function ($) {
    "use strict";

    /**
     * Displays a toast notification
     * @param {string} message - The message to display in the toast.
     */

    const displayedMessages = new Set(JSON.parse(localStorage.getItem('displayedMessages') || '[]'));

    // Show Message only if it hasn't been shown yet
    function showMessage(message, messageId) {
        if (!displayedMessages.has(messageId)) {
            displayedMessages.add(messageId);

            localStorage.setItem('displayedMessages', JSON.stringify(Array.from(displayedMessages)));

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
            }); // Show the message using Toastr
        }
    }

    // Expose the function to the global scope
    window.showMessage = showMessage;

})(jQuery);
