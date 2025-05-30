namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Model
    {
        [Key]
        [StringLength(50)]
        public string MaModel { get; set; }

        [StringLength(50)]
        public string TenModel { get; set; }
    }
}
