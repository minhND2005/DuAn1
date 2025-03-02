namespace Fast_Food.Models
{
    public class HoaDonViewModels
    {
        public int MaKhachHang { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietHoaDonViewModel> ChiTietHoaDon { get; set; } = new List<ChiTietHoaDonViewModel>();

        // Danh sách hiển thị trong giao diện
        public List<MonAn> DanhSachMonAn { get; set; }
        public List<KhachHang> DanhSachKhachHang { get; set; }
    }
    public class ChiTietHoaDonViewModel
    {
        public int MaMonAn { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }
}
