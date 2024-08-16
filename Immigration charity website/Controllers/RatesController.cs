using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Immigration_charity_website.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity.Validation;
using System.IO;
using ClosedXML.Excel;

namespace Immigration_charity_website.Controllers
{
    [Authorize]
    public class RatesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Details(int id)
        {
            var rate = db.Rates.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // GET: Rates/Index
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var ratings = await db.Rates.Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                var totalRatings = ratings.Count;

                var viewModel = new AverageRatingViewModel
                {
                    Ratings = ratings,
                    RatingOptions = Enumerable.Range(1, 5).Select(i => new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString()
                    }).ToList()
                };

                ViewBag.AverageRating = totalRatings > 0 ? ratings.Average(r => r.RatingValue) : 0;
                return View("AdminViewRatings", viewModel);
            }
            else if (User.IsInRole("Customer"))
            {
                var userId = User.Identity.GetUserId();
                var ratings = await db.Rates.Include(r => r.User)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                var totalRatings = ratings.Count;

                var viewModel = new AverageRatingViewModel
                {
                    Ratings = ratings,
                    RatingOptions = Enumerable.Range(1, 5).Select(i => new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString()
                    }).ToList()
                };

                ViewBag.AverageRating = totalRatings > 0 ? ratings.Average(r => r.RatingValue) : 0;
                return View("CustomerViewRatings", viewModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // GET: Rates/Create
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Create()
        {
            var viewModel = new AverageRatingViewModel
            {
                RatingOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "1 Star" },
                new SelectListItem { Value = "2", Text = "2 Stars" },
                new SelectListItem { Value = "3", Text = "3 Stars" },
                new SelectListItem { Value = "4", Text = "4 Stars" },
                new SelectListItem { Value = "5", Text = "5 Stars" }
            }
            };


            return View(viewModel);
        }

        // POST: Rates/Create
        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AverageRatingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.RatingOptions = Enumerable.Range(1, 5).Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                }).ToList();
                return View(viewModel);
            }

            var rate = new Rates
            {
                UserId = User.Identity.GetUserId(),
                RatingValue = viewModel.RatingValue, // 确保这个属性存在并正确使用
                CreatedAt = DateTime.UtcNow // 显式设置 CreatedAt
            };


            db.Rates.Add(rate);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Rates/Edit/5
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> Edit(int id)
        {
            var rate = await db.Rates.FindAsync(id);
            if (rate == null || (rate.UserId != User.Identity.GetUserId() && !User.IsInRole("Admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var viewModel = new AverageRatingViewModel
            {
                RatingValue = rate.RatingValue, // Ensure RatingValue is set here
                RatingOptions = Enumerable.Range(1, 5).Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                }).ToList(),
                Ratings = new List<Rates> { rate } // This seems to be for displaying purposes
            };

            return View(viewModel);
        }

        // POST: Rates/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AverageRatingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var rate = await db.Rates.FindAsync(id);
                if (rate == null || (rate.UserId != User.Identity.GetUserId() && !User.IsInRole("Admin")))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                rate.RatingValue = viewModel.RatingValue;
                // Note: No need to explicitly set State to Modified, EF tracks changes
                db.Entry(rate).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // In case of validation error, repopulate RatingOptions
            viewModel.RatingOptions = Enumerable.Range(1, 5).Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            }).ToList();

            return View(viewModel);
        }

        // GET: Rates/Delete/5
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> Delete(int id)
        {
            var rate = await db.Rates.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
            if (rate == null || (rate.UserId != User.Identity.GetUserId() && !User.IsInRole("Admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(rate);
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var rate = await db.Rates.FindAsync(id);
            if (rate == null || (rate.UserId != User.Identity.GetUserId() && !User.IsInRole("Admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            db.Rates.Remove(rate);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        // 导出评分数据到 CSV 文件
        public ActionResult ExportRatesToCsv()
        {
            var rates = db.Rates.ToList(); // 获取所有评分数据

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,UserId,RatingValue,CreatedAt"); // 添加 CSV 头部

            foreach (var rate in rates)
            {
                csvBuilder.AppendLine($"{rate.Id},{rate.UserId},{rate.RatingValue},{rate.CreatedAt}");
            }

            var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var csvFileName = "Rates.csv";

            return File(csvData, "text/csv", csvFileName);
        }
        // 导出评分数据到 Excel 文件
        public ActionResult ExportRatesToExcel()
        {
            var rates = db.Rates.ToList(); // 获取所有评分数据

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Rates");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "UserId";
                worksheet.Cell(1, 3).Value = "RatingValue";
                worksheet.Cell(1, 4).Value = "CreatedAt";

                for (int i = 0; i < rates.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = rates[i].Id;
                    worksheet.Cell(i + 2, 2).Value = rates[i].UserId;
                    worksheet.Cell(i + 2, 3).Value = rates[i].RatingValue;
                    worksheet.Cell(i + 2, 4).Value = rates[i].CreatedAt;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = "Rates.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // 新增: 获取评分数据以生成柱状图
        [HttpGet]
        public ActionResult GetRatingsChartData()
        {
            var ratingCounts = db.Rates
                .GroupBy(r => r.RatingValue)
                .Select(g => new { RatingValue = g.Key, Count = g.Count() })
                .OrderBy(x => x.RatingValue)
                .ToList();

            return Json(ratingCounts, JsonRequestBehavior.AllowGet);
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
