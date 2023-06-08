
$(document).ready(function () {
    // Thêm sự kiện click cho các thẻ li có class 'btndelete'
    $('body').on('click', '.btndelete', function () {
        // Lấy ID sản phẩm từ data-attribute
        var TransactionId = $(this).data("transactionid")

        // Lưu đối tượng HTML ban đầu vào biến `item`
        var item = $(this);

        if (confirm("Bạn có muốn xóa mục này?")) {
            // Gửi yêu cầu xóa sản phẩm đến máy chủ
            $.ajax({
                url: '/Transactions/Delete/' + TransactionId,
                type: "POST",
                data: { id: TransactionId },
                success: function (response) {
                    if (response.success) {
                        // Xóa sản phẩm khỏi danh sách hiển thị
                        item.parents(".products-row").remove();
                    }
                    else {
                        alert(response.message);
                    }
                },
                error: function () {
                    alert("Error occurred while deleting the product.");
                }
            });
        }
    });
});
