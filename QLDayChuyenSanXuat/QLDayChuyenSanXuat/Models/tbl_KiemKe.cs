namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KiemKe
    {
        [Required]
        [StringLength(50)]
        public string SoPhieu { get; set; }

        [StringLength(50)]
        public string NguoiKiemKe { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayKiemKe { get; set; }

        public string GhiChu { get; set; }

        [Key]
        [StringLength(50)]
        public string MaLinhKien { get; set; }

        public int? SoLuongThucTe { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }
    }
}
