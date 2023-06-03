$('#yourform').on('submit', function (event) {
    event.preventDefault(); // Prevent the default form submission

    // Get the form data
    var formData = new FormData(this);

    // Send an AJAX request
    $.ajax({
        url: $(this).attr('action'),
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                // Success case: perform necessary actions
                // Display a success message or redirect to another page
                window.location.href = response.redirectUrl;
            } else {
                // Error case: display the error message
                var errorMessage = response.errorMessage;
                showAlert(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            // Error case: display a generic error message
            showAlert("An error occurred. Please try again later.");
        }
    });
});
function showAlert(message) {
    // Create a unique ID for the alert
    var alertId = 'alert-' + Date.now();

    // Create the HTML for the alert
    var alertHtml = '<div id="' + alertId + '" class="alert alert-danger alert-dismissible fade show" role="alert">' +
        '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
        '<span aria-hidden="true">&times;</span></button>' +
        '<span class="alert-message">' + message + '</span></div>';

    // Append the alert to the page
    $('.row').prepend(alertHtml);

    // Add click event to the close button
    $('#' + alertId + ' .close').on('click', function () {
        $('#' + alertId).alert('close');
    });
}