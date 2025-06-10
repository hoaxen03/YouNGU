// Thêm vào site.js hoặc tạo file notification.js mới
$(document).ready(function () {
    // Kết nối đến SignalR hub
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    // Xử lý khi nhận thông báo mới
    connection.on("ReceiveNotification", function (message) {
        // Cập nhật số lượng thông báo chưa đọc
        updateUnreadCount();

        // Hiển thị toast thông báo
        showNotificationToast(message);
    });

    // Bắt đầu kết nối
    connection.start().catch(function (err) {
        console.error(err.toString());
    });

    // Lấy số lượng thông báo chưa đọc khi trang được tải
    updateUnreadCount();

    // Hàm cập nhật số lượng thông báo chưa đọc
    function updateUnreadCount() {
        $.ajax({
            url: '/Notification/GetUnreadCount',
            type: 'GET',
            success: function (data) {
                var badge = $('#notification-badge');
                if (data.count > 0) {
                    badge.text(data.count);
                    badge.show();
                } else {
                    badge.hide();
                }
            }
        });
    }

    // Hàm hiển thị toast thông báo
    function showNotificationToast(message) {
        var toast = $('<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">' +
            '<div class="toast-header">' +
            '<strong class="mr-auto">YouNGU</strong>' +
            '<small>Vừa xong</small>' +
            '<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>' +
            '<div class="toast-body">' + message + '</div>' +
            '</div>');

        $('.toast-container').append(toast);
        toast.toast('show');
    }
});
