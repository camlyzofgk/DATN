namespace QLDayChuyenSanXuat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ChiTietPhieuXuat
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string SoPhieu { get; set; }

        [StringLength(50)]
        public string MaLinhKien { get; set; }

        public string TenLinhKien { get; set; }

        public int? SoLuong { get; set; }

        public string ViTriLuuTru { get; set; }

        public string NhaCungCap { get; set; }
    }
}
