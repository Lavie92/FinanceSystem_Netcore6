
function editItem(elem) {
    // Get the transaction ID from the data-attribute
    var transactionId = $(elem).data("transactionid");

    // Send an Ajax request to get the edit form
    $.ajax({
        url: '/Transactions/Edit/' + transactionId,
        type: "GET",

        success: function (result) {
            // Display the edit form in the modal
            $('#editModal .modal-body').html(result);
            $('#editModal').modal('show');
        },
        error: function () {
            alert('Error occurred while loading data.');
        }
    });
}