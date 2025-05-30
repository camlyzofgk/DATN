using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class DetailLoiController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: DetailLoi
        public ActionResult Index(string TienDo,string PhanCap,string Model, string LoaiMay, string HTL, string HTN, string PB, string ADID, string NguoiUpdate, string timerange, string maLoiInput)
        {
            var KQ = db.tbl_DetailLoi.AsQueryable();
            if(TienDo != null)
            {
                switch (TienDo)
                {
                    case "All":
                        break;
                    case "Đang điều tra nguyên nhân":
                        KQ = KQ.Where(x => x.TienDo == "Đang điều tra nguyên nhân");
                        break;
                    case "Hoàn thành điều tra nguyên nhân":
                        KQ = KQ.Where(x => x.TienDo == "Hoàn thành điều tra nguyên nhân");
                        break;
                    case "Hoàn thành đối sách tạm thời":
                        KQ = KQ.Where(x => x.TienDo == "Hoàn thành đối sách tạm thời");
                        break;
                    case "Chờ phê duyệt đối sách cố hữu":
                        KQ = KQ.Where(x => x.TienDo == "Chờ phê duyệt đối sách cố hữu");
                        break;
                    case "Hoàn thành đối sách cố hữu":
                        KQ = KQ.Where(x => x.TienDo == "Hoàn thành đối sách cố hữu");
                        break;
                    case "Chờ phê duyệt hiệu quả":
                        KQ = KQ.Where(x => x.TienDo == "Chờ phê duyệt hiệu quả");
                        break;
                    case "Close":
                        KQ = KQ.Where(x => x.TienDo == "Close");
                        break;
                    case "TaiPhat":
                        KQ = KQ.Where(x => x.TaiPhat == true);
                        break;
                    case "":
                        KQ = KQ.Where(x => x.TienDo == "");
                        break;
                }
            }
            var fromDate = DateTime.Now;
            switch (timerange)
            {
                case "7":
                    fromDate = DateTime.Now.AddDays(-7);
                    KQ = KQ.Where(x => x.ThoiGianPhatHanh >= fromDate);
                    break;
                case "30":
                     fromDate = DateTime.Now.AddDays(-30);
                    KQ = KQ.Where(x => x.ThoiGianPhatHanh >= fromDate);
                    break;
                case "All":
                    break;
            }
            if (maLoiInput != null)
            {
                KQ = KQ.Where(x => x.Maloi == maLoiInput);
            }
            if (PhanCap != null && PhanCap != "All")
            {
                KQ = KQ.Where(x => x.PhanCap == PhanCap);
            }
            if (Model != null && Model != "All")
            {
                KQ = KQ.Where(x => x.Model == Model);
            }
            if (LoaiMay != null && LoaiMay != "All")
            {
                KQ = KQ.Where(x => x.LoaiMay == LoaiMay);
            }
            if (HTL != null && HTL != "All")
            {
                KQ = KQ.Where(x => x.PhanLoaiHT_Lon == HTL);
            }
            if (HTN != null && HTN != "All")
            {
                KQ = KQ.Where(x => x.PhanLoaiHT_Nho == HTN);
            }
            if (PB != null && PB != "All")
            {
                // Lấy danh sách ADID thuộc phòng ban PB
                var adidsInPB = db.tbl_User.Where(u => u.MaPB == PB).Select(u => u.ADID).ToList();

                // Lọc danh sách lỗi theo những người trong phòng ban đó
                KQ = KQ.Where(x => adidsInPB.Contains(x.NguoiDamNhiemChinh));
            }
            if (ADID != null && ADID != "All")
            {
                KQ = KQ.Where(x => x.NguoiDamNhiemChinh == ADID);
            }
            if (NguoiUpdate != null && NguoiUpdate != "All")
            {
                KQ = KQ.Where(x => x.NguoiUpDateNew == NguoiUpdate);
            }
            return View(KQ.ToList());
        }

        public ActionResult BangTienDo()
        {
            return View(db.tbl_DetailLoi.ToList());
        }
        // GET: DetailLoi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DetailLoi tbl_DetailLoi = db.tbl_DetailLoi.Find(id);
            if (tbl_DetailLoi == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DetailLoi);
        }

        public ActionResult CaiDat()
        {
            return View();
        }

        public ActionResult UpdateTienDo(string maloi, string TienDo, string LyDo, string NguoiUpdateTD)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            var NN = db.tbl_NguyenNhan.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            var DSTT = db.tbl_DoiSachTamThoi.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            var DSCH = db.tbl_DoiSachCoHuu.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            var HQ = db.tbl_HieuQua.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            if(TienDo != null)
            {
                detailLoi.TienDo = TienDo;
                detailLoi.NguoiUpDateNew = NguoiUpdateTD;
                detailLoi.TimeUpdateNew = DateTime.Now;
                if(LyDo == "Tái Phát")
                {
                    detailLoi.TaiPhat = true;
                }
                else {
                    detailLoi.TaiPhat = false;
                }
                switch (TienDo)
            {
                case "Đang điều tra nguyên nhân":
                    if (NN != null) NN.TrangThai = "Ghi nhập";
                    if (DSTT != null) DSTT.TrangThai = "Ghi nhập";
                    if (DSCH != null) DSCH.TrangThai = "Ghi nhập";
                    if (HQ != null) HQ.TrangThai = "Ghi nhập";
                    break;

                case "Hoàn thành điều tra nguyên nhân":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Ghi nhập";
                    if (DSCH != null) DSCH.TrangThai = "Ghi nhập";
                    if (HQ != null) HQ.TrangThai = "Ghi nhập";
                    break;

                case "Hoàn thành đối sách tạm thời":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Hoàn thành";
                    if (DSCH != null) DSCH.TrangThai = "Ghi nhập";
                    if (HQ != null) HQ.TrangThai = "Ghi nhập";
                    break;

                case "Chờ phê duyệt đối sách cố hữu":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Hoàn thành";
                    if (DSCH != null) DSCH.TrangThai = "Chờ phê duyệt";
                    if (HQ != null) HQ.TrangThai = "Ghi nhập";
                    break;

                case "Hoàn thành đối sách cố hữu":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Hoàn thành";
                    if (DSCH != null) DSCH.TrangThai = "Hoàn thành";
                    if (HQ != null) HQ.TrangThai = "Ghi nhập";
                    break;

                case "Chờ phê duyệt hiệu quả":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Hoàn thành";
                    if (DSCH != null) DSCH.TrangThai = "Hoàn thành";
                    if (HQ != null) HQ.TrangThai = "Chờ phê duyệt";
                    break;

                case "Close":
                    if (NN != null) NN.TrangThai = "Hoàn thành";
                    if (DSTT != null) DSTT.TrangThai = "Hoàn thành";
                    if (DSCH != null) DSCH.TrangThai = "Hoàn thành";
                    if (HQ != null) HQ.TrangThai = "Hoàn thành";
                    break;
            }
            }

            var LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = NguoiUpdateTD,
                TimeUpDate = DateTime.Now,
                DetailUpdate = LyDo
            };
            db.tbl_History.Add(LSu);
            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật thành công!";
            return RedirectToAction("Details", "DetailLoi", new {id = detailLoi.ID });
        }

        public ActionResult UpdateDN(string maloi, string NguoiDN, string NguoiUpdateDN, string LyDo, string step)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            var DNC = db.tbl_User.Where(x => x.Mail == NguoiDN).FirstOrDefault();
            var nguoiDNC = "";
            switch (step)
            {
                case "Người đảm nhiệm chính":
                    nguoiDNC = detailLoi.NguoiDamNhiemChinh;
                    detailLoi.NguoiDamNhiemChinh = DNC.ADID;
                    detailLoi.NguoiDNGhiNhapNN = DNC.ADID;
                    detailLoi.NguoiDNGhiNhapDSTT = DNC.ADID;
                    detailLoi.NguoiDNGhiNhapDSCH = DNC.ADID;
                    detailLoi.NguoiDNPheDuyetDS = DNC.ADID;
                    detailLoi.NguoiDNGhiNhapHQ = DNC.ADID;
                    detailLoi.NguoiDNPheDuyetHQ = DNC.ADID;
                    break;
                case "Người đảm nhiệm ghi nhập nguyên nhân":
                    nguoiDNC = detailLoi.NguoiDNGhiNhapNN;
                    detailLoi.NguoiDNGhiNhapNN = DNC.ADID;
                    break;
                case "Người đảm nhiệm ghi nhập đối sách tạm thời":
                    nguoiDNC = detailLoi.NguoiDNGhiNhapDSTT;
                    detailLoi.NguoiDNGhiNhapDSTT = DNC.ADID;
                    break;
                case "Người đảm nhiệm ghi nhập đối sách cố hữu":
                    nguoiDNC = detailLoi.NguoiDNGhiNhapDSCH;
                    detailLoi.NguoiDNGhiNhapDSCH = DNC.ADID;
                    break;
                case "Người đảm nhiệm phê duyệt đối sách cố hữu":
                    nguoiDNC = detailLoi.NguoiDNPheDuyetDS;
                    detailLoi.NguoiDNPheDuyetDS = DNC.ADID;
                    break;
                case "Người đảm nhiệm ghi nhập hiệu quả đối sách":
                    nguoiDNC = detailLoi.NguoiDNGhiNhapHQ;
                    detailLoi.NguoiDNGhiNhapHQ = DNC.ADID;
                    break;
                case "Người đảm nhiệm phê duyệt hiệu quả đối sách":
                    nguoiDNC = detailLoi.NguoiDNPheDuyetHQ;
                    detailLoi.NguoiDNPheDuyetHQ = DNC.ADID;
                    break;
            }
            detailLoi.NguoiUpDateNew = NguoiUpdateDN;
            detailLoi.TimeUpdateNew = DateTime.Now;
            var LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = NguoiUpdateDN,
                DetailUpdate = "Thay đổi " + step + ". Lý do: " + LyDo,
                TimeUpDate = DateTime.Now
            };
            db.tbl_History.Add(LSu);
            TempData["ThongBao"] = "Cập nhật thành công!";
            db.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.To.Add(DNC.Mail);
            mail.Subject = "Thông báo thay đổi "+step;
            mail.Body = $@"
            <div style='font-family:Arial, sans-serif; font-size:14px; color:#333'>
                <h3 style='color:#00529B'>🔧 Thông báo thay đổi {step} {detailLoi.Maloi}</h3>
                <table class='table'>
                    <tr><th style='padding:4px 8px;border:1px solid black;'>Người đảm nhiệm cũ</th><th style='padding:4px 8px;border:1px solid black;'>Người đảm nhiệm mới</th></tr>
                    <tr><td style='padding:4px 8px;border:1px solid black;'><b>{nguoiDNC}</b></td><td style='padding:4px 8px;border:1px solid black;'><b>{DNC.ADID}</b></td></tr>
                </table>
                <p style='margin-top:16px;'>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
                <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
            </div>
            ";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
            return RedirectToAction("Details", "DetailLoi", new { id = detailLoi.ID });
        }

        //public ActionResult BieuDoThongKe()
        //{
        //    // Lấy danh sách ADID (người đảm nhiệm chính)
        //    var adIds = db.tbl_DetailLoi.Select(x => x.NguoiDamNhiemChinh).Distinct().ToList();

        //    // Lấy danh sách người dùng tương ứng
        //    var users = db.tbl_User.Where(u => adIds.Contains(u.ADID)).ToList();

        //    // Lấy danh sách mã phòng ban từ người dùng (Distinct)
        //    var listPB = users.Select(u => u.MaPB).Distinct().ToList();

        //    var listLoiIn = new List<int>();
        //    var listLoiOut = new List<int>();

        //    foreach (var maPB in listPB)
        //    {
        //        // Lấy danh sách ADID thuộc phòng ban này
        //        var adidsInPB = users.Where(a => a.MaPB == maPB).Select(y => y.ADID).ToList();

        //        // Đếm lỗi In (SoNgayClose = 11)
        //        var loiIn = db.tbl_DetailLoi
        //            .Where(x => x.SoNgayClose == 11 && adidsInPB.Contains(x.NguoiDamNhiemChinh))
        //            .Count();
        //        listLoiIn.Add(loiIn);

        //        // Đếm lỗi Out (SoNgayClose = 50)
        //        var loiOut = db.tbl_DetailLoi
        //            .Where(x => x.SoNgayClose == 50 && adidsInPB.Contains(x.NguoiDamNhiemChinh))
        //            .Count();
        //        listLoiOut.Add(loiOut);
        //    }

        //    // Truyền dữ liệu sang view
        //    ViewBag.LisPB = listPB;
        //    ViewBag.listLoiIn = listLoiIn;
        //    ViewBag.listLoiOut = listLoiOut;

        //    return View();
        //}
        public ActionResult BieuDoThongKe()
        {
            //var adIds = db.tbl_DetailLoi.Select(x => x.NguoiDamNhiemChinh).Distinct().ToList();
            //var listPB = db.tbl_User.Where(u => adIds.Contains(u.ADID)).Select(u => u.MaPB).Distinct().ToList();
            var lisPB = db.tbl_User.Select(x => x.MaPB).Distinct().ToList();
            var listLoiIn = new List<int>();
            var listLoiOut = new List<int>();
            var listChxCloseIn = new List<int>();
            var listChxCloseOut = new List<int>();
            var listCloseIn = new List<int>();
            var listCloseOut = new List<int>();
            ViewBag.LisPB = lisPB;
            foreach (var item in lisPB)
            {
                var loiIn = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 11 && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listLoiIn.Add(loiIn);
                var loiOut = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 50 && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listLoiOut.Add(loiOut);
                var loiChxCloseIn = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 11 && x.TienDo != "Close" && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listChxCloseIn.Add(loiChxCloseIn);
                var loiChxCloseOut = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 50 && x.TienDo != "Close" && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listChxCloseOut.Add(loiChxCloseOut);
                var loiCloseIn = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 11 && x.TienDo == "Close" && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listCloseIn.Add(loiCloseIn);
                var loiCloseOut = db.tbl_DetailLoi.Where(x => x.SoNgayClose == 50 && x.TienDo == "Close" && db.tbl_User.Where(c => c.ADID == x.NguoiDamNhiemChinh).FirstOrDefault().MaPB == item).Count();
                listCloseOut.Add(loiCloseOut);

            }
            ViewBag.listLoiIn = listLoiIn;
            ViewBag.listLoiOut = listLoiOut;
            ViewBag.listChxCloseIn = listChxCloseIn;
            ViewBag.listChxCloseOut = listChxCloseOut;
            ViewBag.listCloseIn = listCloseIn;
            ViewBag.listCloseOut = listCloseOut;
            return View();
        }

        public ActionResult BieuDoTienDo(string Model, string PBan)
        {
            var DangDTNN = new List<int>();
            var HTDTNN = new List<int>();
            var HTDSTT = new List<int>();
            var ChoPDDS = new List<int>();
            var HTDSCH = new List<int>();
            var ChoPDHQ = new List<int>();
            var HTHQ = new List<int>();
            var listLoi = db.tbl_DetailLoi.AsQueryable();
            if (Model != null && Model != "")
            {
                listLoi = listLoi.Where(x => x.Model == Model);
                ViewBag.Model = Model;
            }
            else
            {
                ViewBag.Model = "All";
            }
            if (PBan != null && PBan != "")
            {
                var lisADID = db.tbl_User.Where(x => x.MaPB == PBan).Select(u => u.ADID).ToList();
                listLoi = listLoi.Where(x => lisADID.Contains(x.NguoiDamNhiemChinh));
                ViewBag.PB = PBan;
            }
            else
            {
                ViewBag.PB = "All";
            }
            ViewBag.listLoi = listLoi.Select(x => x.Maloi).ToList();
            foreach (var item in listLoi)
            {
                if (item.TienDo == "Đang điều tra nguyên nhân")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(0);
                    HTDSTT.Add(0);
                    ChoPDDS.Add(0);
                    HTDSCH.Add(0);
                    ChoPDHQ.Add(0);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Hoàn thành điều tra nguyên nhân")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(0);
                    ChoPDDS.Add(0);
                    HTDSCH.Add(0);
                    ChoPDHQ.Add(0);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Hoàn thành đối sách tạm thời")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(1);
                    ChoPDDS.Add(0);
                    HTDSCH.Add(0);
                    ChoPDHQ.Add(0);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Chờ phê duyệt đối sách cố hữu")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(1);
                    ChoPDDS.Add(1);
                    HTDSCH.Add(0);
                    ChoPDHQ.Add(0);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Hoàn thành đối sách cố hữu")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(1);
                    ChoPDDS.Add(1);
                    HTDSCH.Add(1);
                    ChoPDHQ.Add(0);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Chờ phê duyệt hiệu quả")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(1);
                    ChoPDDS.Add(1);
                    HTDSCH.Add(1);
                    ChoPDHQ.Add(1);
                    HTHQ.Add(0);
                }
                else if (item.TienDo == "Close")
                {
                    DangDTNN.Add(1);
                    HTDTNN.Add(1);
                    HTDSTT.Add(1);
                    ChoPDDS.Add(1);
                    HTDSCH.Add(1);
                    ChoPDHQ.Add(1);
                    HTHQ.Add(1);
                }
            }
            ViewBag.DangDTNN = DangDTNN;
            ViewBag.HTDTNN = HTDTNN;
            ViewBag.HTDSTT = HTDSTT;
            ViewBag.ChoPDDS = ChoPDDS;
            ViewBag.HTDSCH = HTDSCH;
            ViewBag.ChoPDHQ = ChoPDHQ;
            ViewBag.HTHQ = HTHQ;
            return View();
        }

        // GET: DetailLoi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetailLoi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,Maloi,PhanCap,Model,LoaiMay,TieuDeTV,TieuDeTN,ThoiDiemPhatSinh,ThoiDiemBatDauLai,PhanLoaiHT_Lon,PhanLoaiHT_Nho,NguoiPhatHanh,NguoiXNHTLoi,DetailTV,DetailTN,LinkDinhKemFile,SoCungSuKien,TaiPhat,TienDo,NguoiDamNhiemChinh,ThoiGianPhatHanh,NguoiUpDateNew,TimeUpdateNew,NguoiDNGhiNhapNN,NguoiDNGhiNhapDSTT,NguoiDNGhiNhapDSCH,NguoiDNPheDuyetDS,NguoiDNGhiNhapHQ,NguoiDNPheDuyetHQ,SoNgayClose")] tbl_DetailLoi tbl_DetailLoi)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tbl_DetailLoi.Add(tbl_DetailLoi);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(tbl_DetailLoi);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "PhanCap,Model,LoaiMay,TieuDeTN,TieuDeTV,ThoiDiemPhatSinh,ThoiDiemBatDauLai,PhanLoaiHT_Lon,PhanLoaiHT_Nho,NguoiPhatHanh,NguoiXNHTLoi,DetailTV,DetailTN,SoCungSuKien")]
    tbl_DetailLoi tbl_DetailLoi,
            List<HttpPostedFileBase> files, string Email)
        {
            // BỎ QUA ModelState.IsValid TẠM THỜI
            // Gán thông tin mặc định ban đầu
            tbl_DetailLoi.TaiPhat = false;
            tbl_DetailLoi.TienDo = "Đang điều tra nguyên nhân";
            tbl_DetailLoi.ThoiGianPhatHanh = DateTime.Now;
            tbl_DetailLoi.NguoiUpDateNew = tbl_DetailLoi.NguoiPhatHanh;
            tbl_DetailLoi.TimeUpdateNew = DateTime.Now;

            var nguoiDNC = db.tbl_User.FirstOrDefault(x => x.Mail == Email);
            if (nguoiDNC != null)
            {
                tbl_DetailLoi.NguoiDamNhiemChinh = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNGhiNhapNN = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNGhiNhapDSTT = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNGhiNhapDSCH = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNPheDuyetDS = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNGhiNhapHQ = nguoiDNC.ADID;
                tbl_DetailLoi.NguoiDNPheDuyetHQ = nguoiDNC.ADID;
            }

            // Lưu để sinh ID trước
            db.tbl_DetailLoi.Add(tbl_DetailLoi);
            db.SaveChanges();

            // Gán Maloi từ ID đã sinh
            tbl_DetailLoi.Maloi = "LSPM_" + tbl_DetailLoi.ID;

            // Tạo thư mục lưu file
            string rootFolderName = tbl_DetailLoi.Maloi;
            string subFolderName = "HienTuongLoi";
            string fullPath = Path.Combine(Server.MapPath("~/Uploads"), rootFolderName, subFolderName);
            Directory.CreateDirectory(fullPath);
            db.SaveChanges();


            // Upload file nếu có
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(fullPath, fileName);
                        file.SaveAs(filePath);
                    }
                }

                tbl_DetailLoi.LinkDinhKemFile = $"~/Uploads/{rootFolderName}";
            }

            // Cập nhật lại bản ghi với các trường bổ sung
            db.Entry(tbl_DetailLoi).State = EntityState.Modified;
            tbl_HienTuong Ht = new tbl_HienTuong()
            {
                MaLoi = tbl_DetailLoi.Maloi,
                PhanCap = tbl_DetailLoi.PhanCap,
                Model = tbl_DetailLoi.Model,
                LoaiMay = tbl_DetailLoi.LoaiMay,
                TieuDeTN = tbl_DetailLoi.TieuDeTN,
                TieuDeTV = tbl_DetailLoi.TieuDeTV,
                ThoiDiemPhatSinh = tbl_DetailLoi.ThoiDiemPhatSinh,
                ThoiDiemBatDauLai = tbl_DetailLoi.ThoiDiemBatDauLai,
                PhanLoaiHT_Lon = tbl_DetailLoi.PhanLoaiHT_Lon,
                PhanLoaiHT_Nho = tbl_DetailLoi.PhanLoaiHT_Nho,
                DetailTV = tbl_DetailLoi.DetailTV,
                DetailTN = tbl_DetailLoi.DetailTV,
                SoCungSuKien = tbl_DetailLoi.SoCungSuKien,
                NguoiUpdate = tbl_DetailLoi.NguoiUpDateNew,
                TimeUpdate = DateTime.Now,
                NguoiXNHTLoi = tbl_DetailLoi.NguoiXNHTLoi,
                TrangThai = "Hoàn thành"
            };
            db.tbl_HienTuong.Add(Ht);

            tbl_History history = new tbl_History()
            {
                MaLoi = tbl_DetailLoi.Maloi,
                NguoiUpdate = tbl_DetailLoi.NguoiUpDateNew,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Cập nhật hiện tượng"
            };
            db.tbl_History.Add(history);

            db.SaveChanges();
            TempData["ThongBao"] = "Phát hành lỗi thành công!";
            var nguoiNhan = db.tbl_User.Where(x => x.ADID == tbl_DetailLoi.NguoiDamNhiemChinh).FirstOrDefault();
            var pbNPH = db.tbl_User.Where(x => x.ADID == tbl_DetailLoi.NguoiPhatHanh).FirstOrDefault();
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(nguoiNhan.Mail); // Email người nhận
                mail.Subject = "Thông báo lỗi mới từ hệ thống Quản lý dây chuyền sản xuất";
                mail.Body = $@"
            <div style='font-family:Arial, sans-serif; font-size:14px; color:#333'>
                <h3 style='color:#00529B'>🔧 Thông báo lỗi mới được tạo</h3>
                <table style='border-collapse:collapse;'>
                    <tr><td style='padding:4px 8px;'><b>Mã lỗi:</b></td><td>{tbl_DetailLoi.Maloi}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Người phát hành:</b></td><td>{pbNPH.MaPB}: {pbNPH.ADID}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Tiêu đề (TV):</b></td><td>{tbl_DetailLoi.TieuDeTV}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Tiêu đề (TN):</b></td><td>{tbl_DetailLoi.TieuDeTN}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Model:</b></td><td>{tbl_DetailLoi.Model}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Loại máy:</b></td><td>{tbl_DetailLoi.LoaiMay}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Thời điểm phát sinh:</b></td><td>{tbl_DetailLoi.ThoiDiemPhatSinh:dd/MM/yyyy HH:mm}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Thời điểm bắt đầu lại:</b></td><td>{tbl_DetailLoi.ThoiDiemBatDauLai:dd/MM/yyyy HH:mm}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Người đảm nhiệm chính:</b></td><td>{nguoiNhan.MaPB}: {tbl_DetailLoi.NguoiDamNhiemChinh}</td></tr>
                    <tr><td style='padding:4px 8px;'><b>Chi tiết hiện tượng:</b></td><td></td></tr>
                    <tr><td colspan='2' style='padding:4px 8px;'>{tbl_DetailLoi.DetailTV}</td></tr>
                </table>
                <p style='margin-top:16px;'>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
                <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
            </div>
        ";
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(); // Tự động lấy cấu hình từ Web.config
                smtp.Send(mail);
            }
            catch
            {

            }

            return RedirectToAction("Index","DetailLoi");
        }
        public ActionResult LichSu(string maloi)
        {
            var lisLS = db.tbl_History.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(lisLS);
        }
        public JsonResult Email(string MaPB)
        {
            // Truy vấn danh sách email từ cơ sở dữ liệu
            var lisEmail = db.tbl_User
                             .Where(x => x.MaPB == MaPB)
                             .Select(x => x.Mail)
                             .ToList();

            return Json(new { success = true, emails = lisEmail }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmailPD(string MaPB)
        {
            // Truy vấn danh sách email từ cơ sở dữ liệu
            var lisEmail = db.tbl_User
                             .Where(x => x.MaPB == MaPB && x.Role_PheDuyet == true)
                             .Select(x => x.Mail)
                             .ToList();

            return Json(new { success = true, emails = lisEmail }, JsonRequestBehavior.AllowGet);
        }

        // GET: DetailLoi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DetailLoi tbl_DetailLoi = db.tbl_DetailLoi.Find(id);
            if (tbl_DetailLoi == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DetailLoi);
        }

        // POST: DetailLoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Maloi,PhanCap,Model,LoaiMay,TieuDeTV,TieuDeTN,ThoiDiemPhatSinh,ThoiDiemBatDauLai,PhanLoaiHT_Lon,PhanLoaiHT_Nho,NguoiPhatHanh,NguoiXNHTLoi,DetailTV,DetailTN,LinkDinhKemFile,SoCungSuKien,TaiPhat,TienDo,NguoiDamNhiemChinh,ThoiGianPhatHanh,NguoiUpDateNew,TimeUpdateNew,NguoiDNGhiNhapNN,NguoiDNGhiNhapDSTT,NguoiDNGhiNhapDSCH,NguoiDNPheDuyetDS,NguoiDNGhiNhapHQ,NguoiDNPheDuyetHQ,SoNgayClose")] tbl_DetailLoi tbl_DetailLoi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_DetailLoi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_DetailLoi);
        }

        // GET: DetailLoi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DetailLoi tbl_DetailLoi = db.tbl_DetailLoi.Find(id);
            if (tbl_DetailLoi == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DetailLoi);
        }

        // POST: DetailLoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_DetailLoi tbl_DetailLoi = db.tbl_DetailLoi.Find(id);
            db.tbl_DetailLoi.Remove(tbl_DetailLoi);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
