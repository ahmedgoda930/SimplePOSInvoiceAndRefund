using POSSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POSSystem.Controllers
{
    public class InvoiceController : Controller
    {
        DB_POSEntities db = new DB_POSEntities();
        // GET: Invoice
        public ActionResult Index()
        {
            if(db.Invoice.Max(z => z.INVID) == null || db.Invoice.Max(z => z.INVID) == 0)
            {
                ViewBag.ordid = 1;
            }
            else
            {
                ViewBag.ordid = db.Invoice.Max(z => z.INVID) + 1;
            }
            
            ViewBag.Cust = new SelectList(db.Customers.ToList(), "id", "Name");
            ViewBag.Types = new SelectList(db.InvoiceType.ToList(), "id", "Typ");
            ViewBag.items = new SelectList(db.Items.ToList(), "id", "ItemName");
            return View();
        }


        //public double GetPrice(int id)
        //{
        //    return double.Parse(db.Items.Where(item => item.id == id).Select(i => i.Price).ToString());
        //}
        public JsonResult GetPrice(int? id)
        {
            if(id != null)
            {
                return Json(db.Items.Where(item => item.id == id).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return new JsonResult { Data = new { result = "Done with Error" } };
            }
            
        }



        [HttpPost]
        
        public JsonResult SaveOrder(int INVID, int FK_CustomerID, short FK_TypeID, DateTime INDate,
            double Total, double Paid, double Remaind, InvoiceDetails[] details)
        {
            if(INVID != 0)
            {
                Invoice order = new Models.Invoice();
                order.INVID = INVID;
                order.FK_CustomerID = FK_CustomerID;
                order.FK_TypeID = FK_TypeID;
                order.INDate = INDate;
                order.Total = Total;
                order.Paid = Paid;
                order.Remaind = Remaind;
                db.Invoice.Add(order);
                db.SaveChanges();
                Customers cust = db.Customers.Find(FK_CustomerID);
                cust.CIN += Paid;
                db.Entry(cust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                for (int i = 0; i < details.Length; i++)
                {
                    InvoiceDetails det = new Models.InvoiceDetails();
                    det.FK_InvoiceID = INVID;
                    det.FK_ItemID = details[i].FK_ItemID;
                    det.Price = details[i].Price;
                    det.QTY = details[i].QTY;
                    det.Total = details[i].Total;
                    db.InvoiceDetails.Add(det);
                    db.SaveChanges();
                    Items item = db.Items.Find(details[i].FK_ItemID);
                    item.Amount -= details[i].QTY;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
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