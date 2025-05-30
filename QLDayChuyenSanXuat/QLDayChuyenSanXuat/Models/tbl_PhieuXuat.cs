namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PhieuXuat
    {
        [Key]
        [StringLength(50)]
        public string SoPhieu { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayLap { get; set; }

        [StringLength(50)]
        public string NguoiTaoPhieu { get; set; }

        [StringLength(50)]
        public string NguoiYeuCau { get; set; }

        [StringLength(50)]
        public string NguoiPheDuyet { get; set; }

        [StringLength(50)]
        public string NguoiCapNhat { get; set; }

        public DateTime? TimeUpDate { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }
    }
}
