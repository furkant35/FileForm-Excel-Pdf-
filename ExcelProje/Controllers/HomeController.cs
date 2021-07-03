using ExcelProje.Models;
using FastMember;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ExcelGetir()
        {
            ExcelPackage excelPackage = new ExcelPackage();
            var excelBlank = excelPackage.Workbook.Worksheets.Add("Calisma1");

            //excelBlank.Cells[1, 1].Value = "Ad";
            //excelBlank.Cells[1, 2].Value = "Soyad";
            //excelBlank.Cells[2, 1].Value = "Furkan";
            //excelBlank.Cells[2, 2].Value = "TOPAL";

            excelBlank.Cells["A1"].LoadFromCollection(new List<Musteri>
            {
                new Musteri{Id=1,Ad="Furkan"},
                new Musteri{Id=2,Ad="Ramazan"}
            }, true, OfficeOpenXml.Table.TableStyles.Light15);

            var bytes = excelPackage.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Guid.NewGuid() + "" + ".xlsx");
        }

        public IActionResult PdfGetir()
        {
            DataTable dataTable = new DataTable();
            dataTable.Load(ObjectReader.Create(new List<Musteri>
            {
                new Musteri{Id=1,Ad="Furkan"},
                new Musteri{Id=2,Ad="Ramazan"}
            }));
            string fileName = Guid.NewGuid() + ".pdf";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents/" + fileName);
            var stream = new FileStream(path, FileMode.Create);
            Document document = new Document(PageSize.A4, 25f, 25f, 25f, 25f);
            PdfWriter.GetInstance(document, stream);
            document.Open();
            //Paragraph paragraph = new Paragraph("Furkan TOPAL");
            PdfPTable pdfPTable = new PdfPTable(dataTable.Columns.Count);   // -Buraya sayıda yazılabilir
            //pdfPTable.AddCell("Ad");
            //pdfPTable.AddCell("Soyad");
            //pdfPTable.AddCell("Furkan");
            //pdfPTable.AddCell("TOPAL");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                pdfPTable.AddCell(dataTable.Columns[i].ColumnName);
            }
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    pdfPTable.AddCell(dataTable.Rows[i][j].ToString());
                }
            }
            document.Add(pdfPTable);
            document.Close();
            return File("/documents/" + fileName, "application/pdf", fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        class Musteri
        {
            public int Id { get; set; }
            public string Ad { get; set; }
        }
    }
}
