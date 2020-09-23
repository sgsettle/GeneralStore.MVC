using GeneralStore.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Controllers
{
    public class TransactionController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index()
        {
            List<Transaction> transactionList = _db.Transactions.ToList();
            List<Transaction> orderedList = transactionList.OrderBy(date => date.DateOfTransaction).ToList();
            return View(orderedList);
        }

        // GET: Transaction
        public ActionResult Create()
        {
            var products = new SelectList(_db.Products.ToList(), "ProductID", "Name");
            ViewBag.Products = products;
            var customers = new SelectList(_db.Customers.ToList(), "CustomerID", "FullName");
            ViewBag.Customers = customers;
            return View();
        }

        // POST: Transaction
        [HttpPost]
        public ActionResult Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Product product = _db.Products.Find(transaction.ProductID);
                if (product == null)
                    return HttpNotFound();

                Customer customer = _db.Customers.Find(transaction.CustomerID);
                if (customer == null)
                    return HttpNotFound();

                if (transaction.ProductCount > product.InventoryCount)
                    return HttpNotFound();

                _db.Transactions.Add(transaction);
                product.InventoryCount -= transaction.ProductCount;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transaction);
        }

        // GET : Delete
        // Transaction/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Transaction transaction = _db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST : Delete
        // Transaction/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Transaction transaction = _db.Transactions.Find(id);
            Product product = _db.Products.Find(transaction.ProductID);
            _db.Transactions.Remove(transaction);
            product.InventoryCount += transaction.ProductCount;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}