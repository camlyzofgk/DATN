namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HienTuong
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string MaLoi { get; set; }

        [StringLength(50)]
        public string PhanCap { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string LoaiMay { get; set; }

        public string TieuDeTV { get; set; }

        public string TieuDeTN { get; set; }

        public DateTime? ThoiDiemPhatSinh { get; set; }

        public DateTime? ThoiDiemBatDauLai { get; set; }

        public string PhanLoaiHT_Lon { get; set; }

        public string PhanLoaiHT_Nho { get; set; }

        public string NguoiXNHTLoi { get; set; }

        public string DetailTV { get; set; }

        public string DetailTN { get; set; }

        public string LinkDinhKemFile { get; set; }

        [StringLength(50)]
        public string SoCungSuKien { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }

        [StringLength(50)]
        public string NguoiUpdate { get; set; }

        public DateTime? TimeUpdate { get; set; }
    }
}
