namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_NguyenNhan
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string MaLoi { get; set; }

        public string PhanLoaiNN_Lon { get; set; }

        public string PhanLoaiNN_Nho { get; set; }

        public int? SoNgayClose { get; set; }

        public DateTime? DealineCloseNN { get; set; }

        public DateTime? DealineCloseDSTT { get; set; }

        public DateTime? DealineGhiNhapDSCH { get; set; }

        public DateTime? DealinePheDuyetDSCH { get; set; }

        public DateTime? DealineGhiNhapHQ { get; set; }

        public DateTime? DealinePheDuyetHQ { get; set; }

        [StringLength(50)]
        public string NguoiUpdate { get; set; }

        public string LinkFileDinhKem { get; set; }

        [StringLength(50)]
        public string SoCungSuKien { get; set; }

        public string ChiTietTV { get; set; }

        public string ChiTietTN { get; set; }

        public DateTime? TimeUpdate { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }
    }
}
