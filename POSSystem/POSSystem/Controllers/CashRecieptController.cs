using POSSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSSystem.Controllers
{
    public class CashRecieptController : Controller
    {
        // GET: CashReciept
        DB_POSEntities db = new DB_POSEntities();
        public ActionResult Index()
        {
            ViewBag.Cust = new SelectList(db.Customers.ToList(), "id", "Name");
            ViewBag.Invo = new SelectList(db.Invoice.ToList(), "INVID", "INVID");
            return View();
        }
        [HttpPost]
        public JsonResult SaveOrder(int FK_InvoiceID, int FK_CustomerID, DateTime CRDate, double Total)
        {
            if (Total != 0)
            {
                CashReciept order = new Models.CashReciept();
                order.FK_InvoiceID = FK_InvoiceID;
                order.FK_CustomerID = FK_CustomerID;
                order.CRDate = CRDate;
                order.Total = Total;
                db.CashReciept.Add(order);
                db.SaveChanges();
                Customers cust = db.Customers.Find(FK_CustomerID);
                cust.CIN += Total;
                db.Entry(cust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges(); 
                return new JsonResult { Data = new { result = "Done Successfully" } };
            }
            else
            {
                return new JsonResult { Data = new { result = "Done with Error" } };
            }

        }

        //public JsonResult GetInvoices(int id)
        //{
        //    return Json(db.Invoice.Where(item => item.FK_CustomerID == id && item.IsDeleted == false).ToList(), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetInvoicesRemaind(int InvoiceID)
        //{
        //    return Json(db.Invoice.Where(item => item.INVID == InvoiceID).ToList(), JsonRequestBehavior.AllowGet);
        //}
    }
}