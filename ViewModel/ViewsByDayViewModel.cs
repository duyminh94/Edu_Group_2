namespace BlogPlatform.ViewModel
{
    // Một điểm dữ liệu trên biểu đồ "lượt xem theo ngày" (Issue #12, UC23)
    //
    // Dùng class này thay cho tuple List<(DateTime, int)>: tuple trong interface public
    // khó đọc (phải nhớ Item1 là gì) và khó bind trong Razor.
    //
    // 👥 Khu C sở hữu
    public class ViewsByDayViewModel
    {
        // Ngày thống kê — chỉ lấy phần ngày, bỏ giờ phút
        public DateTime Date { get; set; }

        // Số lượt xem ghi nhận trong ngày đó
        public int ViewCount { get; set; }
    }
}
