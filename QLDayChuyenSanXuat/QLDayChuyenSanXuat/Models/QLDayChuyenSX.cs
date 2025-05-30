using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QLDayChuyenSanXuat.Models
{
    public partial class QLDayChuyenSX : DbContext
    {
        public QLDayChuyenSX()
            : base("name=QLDayChuyenSX")
        {
        }

        public virtual DbSet<LinhKien> LinhKiens { get; set; }
        public virtual DbSet<tbl_ChiTietPhieuNhap> tbl_ChiTietPhieuNhap { get; set; }
        public virtual DbSet<tbl_ChiTietPhieuXuat> tbl_ChiTietPhieuXuat { get; set; }
        public virtual DbSet<tbl_DetailLoi> tbl_DetailLoi { get; set; }
        public virtual DbSet<tbl_DoiSachCoHuu> tbl_DoiSachCoHuu { get; set; }
        public virtual DbSet<tbl_DoiSachTamThoi> tbl_DoiSachTamThoi { get; set; }
        public virtual DbSet<tbl_HienTuong> tbl_HienTuong { get; set; }
        public virtual DbSet<tbl_HieuQua> tbl_HieuQua { get; set; }
        public virtual DbSet<tbl_History> tbl_History { get; set; }
        public virtual DbSet<tbl_KiemKe> tbl_KiemKe { get; set; }
        public virtual DbSet<tbl_LoaiMay> tbl_LoaiMay { get; set; }
        public virtual DbSet<tbl_Model> tbl_Model { get; set; }
        public virtual DbSet<tbl_NguyenNhan> tbl_NguyenNhan { get; set; }
        public virtual DbSet<tbl_PhieuNhap> tbl_PhieuNhap { get; set; }
        public virtual DbSet<tbl_PhieuXuat> tbl_PhieuXuat { get; set; }
        public virtual DbSet<tbl_PhongBan> tbl_PhongBan { get; set; }
        public virtual DbSet<tbl_User> tbl_User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
