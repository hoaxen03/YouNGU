// Admin JavaScript

// Confirm delete
function confirmDelete(message, formId) {
    if (confirm(message || 'Bạn có chắc chắn muốn xóa mục này không?')) {
        document.getElementById(formId).submit();
    }
    return false;
}

// Initialize tooltips
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// Format file size
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// Format duration
function formatDuration(seconds) {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);

    return [
        h > 0 ? h.toString().padStart(2, '0') : null,
        m.toString().padStart(2, '0'),
        s.toString().padStart(2, '0')
    ].filter(Boolean).join(':');
}

// Quick approve video
function quickApproveVideo(videoId, button) {
    if (!confirm('Bạn có chắc chắn muốn phê duyệt video này không?')) {
        return;
    }

    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

    fetch('/Admin/VideoManagement/QuickApprove/' + videoId, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update UI
                const statusCell = button.closest('tr').querySelector('.video-status');
                statusCell.innerHTML = '<span class="badge bg-primary">Public</span>';
                button.closest('td').innerHTML = '<span class="text-success"><i class="fas fa-check"></i> Đã phê duyệt</span>';
            } else {
                alert('Lỗi: ' + data.message);
                button.disabled = false;
                button.innerHTML = '<i class="fas fa-check"></i> Phê duyệt';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Đã xảy ra lỗi khi phê duyệt video');
            button.disabled = false;
            button.innerHTML = '<i class="fas fa-check"></i> Phê duyệt';
        });
}
// Sidebar toggle state
document.addEventListener("DOMContentLoaded", () => {
    // Kiểm tra trạng thái sidebar từ localStorage
    var sidebarToggled = localStorage.getItem("sb|sidebar-toggle") === "true"

    if (sidebarToggled) {
        document.body.classList.add("sb-sidenav-toggled")
    } else {
        document.body.classList.remove("sb-sidenav-toggled")
    }

    // Đảm bảo các liên kết trong sidebar được highlight khi active
    var currentUrl = window.location.pathname
    var navLinks = document.querySelectorAll(".sb-sidenav .nav-link")

    navLinks.forEach((link) => {
        var href = link.getAttribute("href")
        if (href && currentUrl.includes(href) && href !== "/Admin/Dashboard") {
            link.classList.add("active")
        } else if (href === "/Admin/Dashboard" && (currentUrl === "/Admin" || currentUrl === "/Admin/Dashboard")) {
            link.classList.add("active")
        }
    })
})
