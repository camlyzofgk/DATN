namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_LoaiMay
    {
        [Key]
        [StringLength(50)]
        public string MaLoaiMay { get; set; }

        [StringLength(50)]
        public string TenLoaiMay { get; set; }
    }
}
