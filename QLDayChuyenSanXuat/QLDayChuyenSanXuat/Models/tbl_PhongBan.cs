namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PhongBan
    {
        [Key]
        [StringLength(50)]
        public string MaPB { get; set; }

        [StringLength(50)]
        public string TenPB { get; set; }
    }
}
