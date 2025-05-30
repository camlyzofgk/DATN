namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_DetailLoi
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Maloi { get; set; }

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

        [StringLength(50)]
        public string NguoiPhatHanh { get; set; }

        public string NguoiXNHTLoi { get; set; }

        public string DetailTV { get; set; }

        public string DetailTN { get; set; }

        public string LinkDinhKemFile { get; set; }

        [StringLength(50)]
        public string SoCungSuKien { get; set; }

        public bool? TaiPhat { get; set; }

        [StringLength(50)]
        public string TienDo { get; set; }

        [StringLength(50)]
        public string NguoiDamNhiemChinh { get; set; }

        public DateTime? ThoiGianPhatHanh { get; set; }

        [StringLength(50)]
        public string NguoiUpDateNew { get; set; }

        public DateTime? TimeUpdateNew { get; set; }

        [StringLength(50)]
        public string NguoiDNGhiNhapNN { get; set; }

        [StringLength(50)]
        public string NguoiDNGhiNhapDSTT { get; set; }

        [StringLength(50)]
        public string NguoiDNGhiNhapDSCH { get; set; }

        [StringLength(50)]
        public string NguoiDNPheDuyetDS { get; set; }

        [StringLength(50)]
        public string NguoiDNGhiNhapHQ { get; set; }

        [StringLength(50)]
        public string NguoiDNPheDuyetHQ { get; set; }

        public int? SoNgayClose { get; set; }
    }
}
