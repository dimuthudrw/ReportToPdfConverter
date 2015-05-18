using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReportToPdfConverter.Models;
using ReportToPdfConverter.Utility;

namespace ReportToPdfConverter.Controllers
{
    public class MovieController : Controller
    {
        private MovieDBContext db = new MovieDBContext();

        // GET: /Movie/
        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }

        public ActionResult PrintPDF()
        {
            const string renderType = "PDF";
            var path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["ReportPath"]), ConfigurationManager.AppSettings["MoviReportName"]);
            if (!System.IO.File.Exists(path))
                return View("Index");

            var fromDate = Convert.ToDateTime("2014-06-01");
            var toDate = Convert.ToDateTime("2015-05-01");

            var reportParameters = new List<ReportCriteriaValue>
            {
                new ReportCriteriaValue{ParameterName ="@From",ParameterValue = fromDate.ToShortDateString()},
                new ReportCriteriaValue{ParameterName ="@To",ParameterValue = toDate.ToShortDateString()}
            };

            var dataSet = RenderPdfUtility.GetDataSet(ConfigurationManager.AppSettings["MovieSpName"], reportParameters);

            var reportCriteriaValues = new List<ReportCriteriaValue>
            {
                new ReportCriteriaValue {ParameterName = "FromDate", ParameterValue = fromDate.ToShortDateString()},
                new ReportCriteriaValue {ParameterName = "ToDate", ParameterValue = toDate.ToShortDateString()}
            };

            var localReport = RenderPdfUtility.LoadReport(dataSet, path, reportCriteriaValues);

            var result = RenderPdfUtility.RenderReport(localReport, renderType);
            return File(result.Item1, result.Item2);
        }

        // GET: /Movie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: /Movie/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Movie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: /Movie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: /Movie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: /Movie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: /Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
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
