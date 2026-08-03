namespace BlogPlatform.ViewModel
{
    // Kết quả của hành động bật/tắt: thích bài và lưu bài (Issue #9, quy tắc 4.7–4.11)
    //
    // Vì sao không trả string như các hàm ghi khác: bấm Like xong View cần biết ngay
    // "đã thích chưa" và "LikeCount mới là bao nhiêu" để cập nhật nút. Trả "SUCCESS"
    // thì Controller buộc phải truy vấn lại — thừa một vòng gọi DB trên hành động
    // được bấm nhiều nhất hệ thống.
    //
    // 👥 Khu D sở hữu
    public class ToggleResultViewModel
    {
        public bool IsSuccess { get; set; }

        // Câu thông báo tiếng Việt khi thất bại, để trống khi thành công
        public string Message { get; set; } = string.Empty;

        // Trạng thái SAU khi bật/tắt: true = đang thích / đã lưu
        public bool IsActive { get; set; }

        // Giá trị bộ đếm mới (LikeCount) — bookmark không có bộ đếm nên để 0
        public int NewCount { get; set; }
    }
}
