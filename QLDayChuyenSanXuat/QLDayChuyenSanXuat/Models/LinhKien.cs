namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LinhKien")]
    public partial class LinhKien
    {
        [Key]
        [StringLength(50)]
        public string MaLinhKien { get; set; }

        public string TênLinhKien { get; set; }

        public int? SoLuong { get; set; }

        public string ViTriLuuTru { get; set; }

        public string NhaCungCap { get; set; }
    }
}
