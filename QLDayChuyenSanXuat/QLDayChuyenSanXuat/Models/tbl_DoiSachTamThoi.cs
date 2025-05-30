namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_DoiSachTamThoi
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string MaLoi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TimeStart { get; set; }

        public string PhanLoaiDSTT_Lon { get; set; }

        public string PhanLoaiDSTT_Nho { get; set; }

        public string NguoiTH { get; set; }

        [StringLength(50)]
        public string NguoiUpdate { get; set; }

        public DateTime? TimeUpdate { get; set; }

        public string LinkFileDinhKem { get; set; }

        [StringLength(50)]
        public string SoCungSuKien { get; set; }

        public string ChiTietTV { get; set; }

        public string ChiTietTN { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }
    }
}
