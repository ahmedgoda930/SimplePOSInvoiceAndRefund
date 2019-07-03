using POSSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSSystem.Controllers
{
    public class RefundInvoiceController : Controller
    {
        // GET: RefundInvoice
        DB_POSEntities db = new DB_POSEntities();
        public ActionResult Index()
        {
            if (db.RefundInvoice.Max(z => z.RINVID) == null || db.RefundInvoice.Max(z => z.RINVID) == 0)
            {
                ViewBag.refunid = 1;
            }
            else
            {
                ViewBag.refunid = db.RefundInvoice.Max(z => z.RINVID) + 1;
            }
            ViewBag.Cust = new SelectList(db.Customers.ToList(), "id", "Name");
            ViewBag.Types = new SelectList(db.InvoiceType.ToList(), "id", "Typ");
            ViewBag.items = new SelectList(db.Items.ToList(), "id", "ItemName");
            ViewBag.Invo = new SelectList(db.Invoice.ToList(), "INVID", "INVID");
            return View();
        }


        public JsonResult GetPrice(int id)
        {
            return Json(db.Items.Where(item => item.id == id).ToList(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public JsonResult SaveOrder(int RINVID, int FK_CustomerID,int FK_InvoiceID, short FK_TypeID,
            DateTime RINDate, double Total, RefundINVDetails[] details)
        {
            if (RINVID != 0)
            {
                RefundInvoice order = new Models.RefundInvoice();
                order.RINVID = RINVID;
                order.FK_CustomerID = FK_CustomerID;
                order.FK_InvoiceID = FK_InvoiceID;
                order.FK_TypeID = FK_TypeID;
                order.RINDate = RINDate;
                order.Total = Total;
                db.RefundInvoice.Add(order);
                db.SaveChanges();
                Customers cust = db.Customers.Find(FK_CustomerID);
                cust.COUT += Total;
                db.Entry(cust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                for (int i = 0; i < details.Length; i++)
                {
                    RefundINVDetails det = new Models.RefundINVDetails();
                    det.FK_RInvoiceID = RINVID;
                    det.FK_ItemID = details[i].FK_ItemID;
                    det.Price = details[i].Price;
                    det.QTY = details[i].QTY;
                    det.Total = details[i].Total;
                    db.RefundINVDetails.Add(det);
                    db.SaveChanges();
                    Items itm = db.Items.Find(det.FK_ItemID);
                    itm.Amount += det.QTY;
                    db.Entry(itm).State = System.Data.Entity.EntityState.Modified;
                }
                return new JsonResult { Data = new { result = "Done Successfully" } };
            }
            else
            {
                return new JsonResult { Data = new { result = "Done with Error" } };
            }

        }
    }
}