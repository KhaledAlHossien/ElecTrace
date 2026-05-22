using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.Report;
using Application_Contract.Interfaces;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Infrastructure.Servicies
{
    public class ElectricityReportService : IElectricityReportService
    {
        public byte[] GenerateElectricityExcelReport(IEnumerable<ElectricityReportResponseDto> reportData, string title)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("تقرير الاستهلاك");
                worksheet.RightToLeft = true;

                worksheet.Cell("A2").Value = title;
                var titleRange = worksheet.Range("A2:H2");

                var titleStyle = titleRange.Merge().Style;
                titleStyle.Font.Bold = true;
                titleStyle.Font.FontSize = 16;
                titleStyle.Font.FontColor = XLColor.FromHtml("#2c3e50");
                titleStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var firstRow = System.Linq.Enumerable.FirstOrDefault(reportData);
                string prevMonthLabel = firstRow?.PreviousMonthLabel ?? "تأشيرة نهاية الشهر السابق";
                string currentMonthLabel = firstRow?.CurrentMonthLabel ?? "تأشيرة نهاية الشهر الحالي";

                var headers = new string[]
                {
                    "الرقم", "الفعالية", prevMonthLabel, currentMonthLabel,
                    "كمية الاستهلاك", "عامل الضرب", "سعر الكيلو", "قيمة الاستهلاك"
                };

                int headerRowIndex = 4;
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(headerRowIndex, i + 1);
                    cell.Value = headers[i];

                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#34495e"))
                               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                               .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                }

                int startRowIndex = 5;
                int currentRowIndex = startRowIndex;

                foreach (var item in reportData)
                {
                    worksheet.Cell(currentRowIndex, 1).Value = item.Number;
                    worksheet.Cell(currentRowIndex, 2).Value = item.DepartmentName;
                    worksheet.Cell(currentRowIndex, 3).Value = item.PreviousReading;
                    worksheet.Cell(currentRowIndex, 4).Value = item.CurrentReading;
                    worksheet.Cell(currentRowIndex, 5).Value = item.ActualConsumption;
                    worksheet.Cell(currentRowIndex, 6).Value = item.ConversionFactor;
                    worksheet.Cell(currentRowIndex, 7).Value = item.PricePerKilo;

                    // ⚠️ تم التحديث هنا: أخذ التوتل كوست الصافي والمحسوب من قاعدة البيانات مباشرة
                    worksheet.Cell(currentRowIndex, 8).Value = item.TotalCost;

                    var dataRange = worksheet.Range(currentRowIndex, 1, currentRowIndex, 8);
                    dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                   .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    worksheet.Cell(currentRowIndex, 3).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(currentRowIndex, 4).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(currentRowIndex, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(currentRowIndex, 8).Style.NumberFormat.Format = "#,##0.00";

                    worksheet.Cell(currentRowIndex, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    if (currentRowIndex % 2 == 0)
                    {
                        dataRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#f8f9fa"));
                    }

                    currentRowIndex++;
                }

                int lastDataRowIndex = currentRowIndex - 1;
                worksheet.Cell(currentRowIndex, 2).Value = "المجموع الكلي";

                worksheet.Cell(currentRowIndex, 5).FormulaA1 = $"=SUM(E{startRowIndex}:E{lastDataRowIndex})";
                worksheet.Cell(currentRowIndex, 8).FormulaA1 = $"=SUM(H{startRowIndex}:H{lastDataRowIndex})";

                var totalRange = worksheet.Range(currentRowIndex, 1, currentRowIndex, 8);
                totalRange.Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#eaeded"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(currentRowIndex, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Cell(currentRowIndex, 5).Style.NumberFormat.Format = "#,##0";
                worksheet.Cell(currentRowIndex, 8).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateAllInvoicesExcel(IEnumerable<ElectricityReportResponseDto> allInvoicesData, int year, int month)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("كشف الفواتير الفردية");
                worksheet.RightToLeft = true;

                worksheet.ShowGridLines = false;

                int lastDay = DateTime.DaysInMonth(year, month);
                var invoiceDate = new DateTime(year, month, lastDay);

                int currentRow = 3;

                var darkBlue = XLColor.FromHtml("#1f4e78");
                var lightBlueBg = XLColor.FromHtml("#ebf5fb");
                var textDark = XLColor.FromHtml("#2c3e50");
                var textGray = XLColor.FromHtml("#7f8c8d");
                var borderColor = XLColor.FromHtml("#d6dbdf");

                foreach (var invoiceData in allInvoicesData)
                {
                    var cardRange = worksheet.Range(currentRow, 2, currentRow + 5, 5);
                    cardRange.Style.Fill.SetBackgroundColor(XLColor.White);

                    worksheet.Cell(currentRow, 2).Value = invoiceData.DepartmentName;
                    var headerRange = worksheet.Range(currentRow, 2, currentRow, 5);
                    var headerStyle = headerRange.Merge().Style;
                    headerStyle.Font.Bold = true;
                    headerStyle.Font.FontSize = 14;
                    headerStyle.Font.FontColor = XLColor.White;
                    headerStyle.Fill.SetBackgroundColor(darkBlue);
                    headerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    headerStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    worksheet.Row(currentRow).Height = 30;

                    int labelRow = currentRow + 2;
                    worksheet.Cell(labelRow, 2).Value = "كمية الاستهلاك";
                    worksheet.Cell(labelRow, 3).Value = "تاريخ الاستحقاق";
                    worksheet.Cell(labelRow, 4).Value = "نوع الخدمة";

                    var labelRange = worksheet.Range(labelRow, 2, labelRow, 4);
                    labelRange.Style.Font.Bold = true;
                    labelRange.Style.Font.FontSize = 11;
                    labelRange.Style.Font.FontColor = textGray;
                    labelRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    labelRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    worksheet.Row(labelRow).Height = 22;

                    int valueRow = labelRow + 1;
                    worksheet.Cell(valueRow, 2).Value = invoiceData.ActualConsumption;
                    worksheet.Cell(valueRow, 3).Value = invoiceDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(valueRow, 4).Value = "كهرباء";

                    var valueRange = worksheet.Range(valueRow, 2, valueRow, 4);
                    valueRange.Style.Font.FontSize = 11;
                    valueRange.Style.Font.FontColor = textDark;
                    valueRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    valueRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    worksheet.Cell(valueRow, 2).Style.NumberFormat.Format = "#,##0";
                    worksheet.Row(valueRow).Height = 24;

                    var gridRange = worksheet.Range(labelRow, 2, valueRow, 4);
                    gridRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                    gridRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    gridRange.Style.Border.InsideBorderColor = borderColor;
                    gridRange.Style.Border.OutsideBorderColor = borderColor;

                    var totalBoxRange = worksheet.Range(labelRow, 5, valueRow, 5);
                    var totalBoxStyle = totalBoxRange.Merge().Style;
                    totalBoxStyle.Fill.SetBackgroundColor(lightBlueBg);
                    totalBoxStyle.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    totalBoxStyle.Border.OutsideBorderColor = XLColor.FromHtml("#a9cce3");
                    totalBoxStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    totalBoxStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    // ⚠️ تم التأكيد هنا: قراءة الحقل المالي الإجمالي الفردي بشكل منسق ومطابق
                    worksheet.Cell(labelRow, 5).Value = $"إجمالي الفاتورة\n{invoiceData.TotalCost:N0} ل.س.";
                    worksheet.Cell(labelRow, 5).Style.Font.Bold = true;
                    worksheet.Cell(labelRow, 5).Style.Font.FontSize = 12;
                    worksheet.Cell(labelRow, 5).Style.Font.FontColor = XLColor.FromHtml("#1b4f72");
                    worksheet.Cell(labelRow, 5).Style.Alignment.SetWrapText(true);

                    int footerRow = valueRow + 1;
                    worksheet.Cell(footerRow, 2).Value = "⚠️ مهلة التسديد 7 أيام من تاريخ استلام الفاتورة";
                    var footerRange = worksheet.Range(footerRow, 2, footerRow, 5);
                    var footerStyle = footerRange.Merge().Style;
                    footerStyle.Font.Italic = true;
                    footerStyle.Font.FontSize = 10;
                    footerStyle.Font.FontColor = textGray;
                    footerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    footerStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    worksheet.Row(footerRow).Height = 22;

                    var invoiceFullBox = worksheet.Range(currentRow, 2, footerRow, 5);
                    invoiceFullBox.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    invoiceFullBox.Style.Border.OutsideBorderColor = XLColor.FromHtml("#b2babb");

                    var bottomLineRange = worksheet.Range(footerRow, 2, footerRow, 5);
                    bottomLineRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Medium);
                    bottomLineRange.Style.Border.BottomBorderColor = darkBlue;

                    currentRow = footerRow + 5;
                }

                worksheet.Column("A").Width = 5;
                worksheet.Column("B").Width = 22;
                worksheet.Column("C").Width = 22;
                worksheet.Column("D").Width = 20;
                worksheet.Column("E").Width = 26;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateDepartmentsExcel(IEnumerable<MeterCodeNameDto> departments, string sheetName = "الأقسام")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);
            worksheet.RightToLeft = true;

            worksheet.Cell(1, 1).Value = $"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}";
            worksheet.Cell(1, 1).Style.Font.Italic = true;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#7f8c8d");

            worksheet.Row(2).Height = 5;

            worksheet.Cell(3, 1).Value = "Meter Code";
            worksheet.Cell(3, 2).Value = "اسم القسم";
            worksheet.Cell(3, 3).Value = "QR Code";

            var headerRange = worksheet.Range(3, 1, 3, 3);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#34495e"));
            headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headerRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Row(3).Height = 25;

            int row = 4;
            foreach (var dept in departments)
            {
                worksheet.Cell(row, 1).Value = dept.MeterCode;
                worksheet.Cell(row, 2).Value = dept.Name;

                worksheet.Cell(row, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(row, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(row, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(row, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                var qrCodeText = dept.MeterCode;
                var qrImage = GenerateQRCodeBytes(qrCodeText, 90, 90);

                if (qrImage != null)
                {
                    using var ms = new MemoryStream(qrImage);
                    var picture = worksheet.AddPicture(ms);
                    picture.MoveTo(worksheet.Cell(row, 3), 18, 8);
                    picture.Width = 90;
                    picture.Height = 90;
                }

                var dataRange = worksheet.Range(row, 1, row, 3);
                dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                dataRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                dataRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Row(row).Height = 85;

                if (row % 2 == 0)
                    dataRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#f8f9fa"));

                row++;
            }

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 30;
            worksheet.Column(3).Width = 15;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] GenerateQRCodeBytes(string text, int width, int height)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new BitmapByteQRCode(qrCodeData);
            var qrCodeImageBytes = qrCode.GetGraphic(20);

            using var ms = new MemoryStream(qrCodeImageBytes);
            using var bitmap = new Bitmap(ms);
            using var resized = new Bitmap(bitmap, new Size(width, height));
            using var resultStream = new MemoryStream();
            resized.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
            return resultStream.ToArray();
        }
    }
}