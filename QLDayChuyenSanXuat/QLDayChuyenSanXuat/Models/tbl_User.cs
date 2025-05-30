namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_User
    {
        [Key]
        [StringLength(50)]
        public string ADID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Mail { get; set; }

        [StringLength(50)]
        public string MaPB { get; set; }

        public bool Role_PhatHanhLoi { get; set; }

        public bool Role_PheDuyet { get; set; }

        public bool Role_QuanLyUser { get; set; }

        public bool Role_XuatKho { get; set; }

        public bool Role_NhapKho { get; set; }

        public bool Role_KiemKe { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }
}
