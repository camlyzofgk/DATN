namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_History
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string MaLoi { get; set; }

        public DateTime? TimeUpDate { get; set; }

        [StringLength(50)]
        public string NguoiUpdate { get; set; }

        public string DetailUpdate { get; set; }
    }
}
