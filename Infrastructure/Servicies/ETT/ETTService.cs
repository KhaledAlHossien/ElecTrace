using Application_Contract.DTOs.Pattern;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace Infrastructure.Servicies.ETT
{
    public class ETTService
    {
        private readonly string _connectionString;
        private readonly string _reportQuery;

        public ETTService(IConfiguration configuration)
        {
            _connectionString = GetConnectionString();

            string sqlPath = Path.Combine(
                AppContext.BaseDirectory,
                "Queries",
                "PrcImportInvoice.sql");

            if (!File.Exists(sqlPath))
                throw new FileNotFoundException(
                    $"Query file not found: {sqlPath}");

            _reportQuery = File.ReadAllText(sqlPath);
        }

        private class DatabaseConfig
        {
            public string ConnectionString { get; set; } = "";
        }

        private static string GetConnectionString()
        {
            string path = Path.Combine(
                AppContext.BaseDirectory,
                "Config",
                "AmeenDatabase.config");

            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"Connection file not found: {path}");

            string json = File.ReadAllText(path);

            DatabaseConfig config =
                JsonSerializer.Deserialize<DatabaseConfig>(json)!;

            return config.ConnectionString;
        }

        public DataTable GetReport(
            string pattern,
            DateTime fromDate,
            DateTime toDate)
        {
            DataTable dt = new DataTable();

            using SqlConnection conn =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(_reportQuery, conn);

            cmd.Parameters.AddWithValue("@Type", pattern);
            cmd.Parameters.AddWithValue("@StartDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@EndDate", toDate.Date);

            conn.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            dt.Load(reader);

            return dt;
        }

        public List<PatternDto> GetPatterns()
        {
            var result = new List<PatternDto>();

            using SqlConnection conn =
                new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Guid, Name
                FROM bt000";

            using SqlCommand cmd =
                new SqlCommand(sql, conn);

            conn.Open();

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new PatternDto
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Guid")),
                    Name = reader["Name"].ToString()
                });
            }

            return result;
        }


        public byte[] GenerateReportExcel(string pattern, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = GetReport(pattern, fromDate, toDate);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("الحركة اليومية");

            // إعدادات عامة للورقة
            ws.RightToLeft = true;
            ws.ShowGridLines = false;

            // جعل الخط الافتراضي لكامل التقرير 16
            ws.Style.Font.FontSize = 16;

            int row = 1;

            string currentCustomer = "";
            string currentBill = "";
            double billTotal = 0;
            double customerTotal = 0;
            int billStartRow = 0;
            int customerStartRow = 1;

            var billRanges = new List<(int StartRow, int EndRow)>();

            foreach (DataRow dr in dt.Rows)
            {
                string customer = dr["Cust_Name"]?.ToString() ?? "";
                string billNumber = dr["BillNumber"]?.ToString() ?? "";
                double lineTotal = Convert.ToDouble(dr["Total"] ?? 0);

                // =========================
                // تغيير الزبون
                // =========================
                if (currentCustomer != customer)
                {
                    if (!string.IsNullOrEmpty(currentCustomer))
                    {
                        // 1. إغلاق آخر فاتورة للزبون السابق
                        if (!string.IsNullOrEmpty(currentBill))
                        {
                            billRanges.Add((billStartRow, row - 1));

                            ws.Range(row, 1, row, 6).Merge().Value = "";

                            ws.Range(row, 7, row, 9).Merge().Value = Convert.ToDouble(billTotal);
                            ws.Range(row, 7, row, 9).Style.Font.Bold = true;
                            ws.Range(row, 7, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#F4D03F");
                            ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";
                            row++;
                        }

                        // 2. طباعة المجموع الإجمالي للزبون
                        ws.Row(row).Height = 35;

                        ws.Range(row, 1, row, 6).Merge().Value = $"المجموع الإجمالي للزبون: {currentCustomer}";
                        ws.Range(row, 1, row, 6).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 6).Style.Font.FontSize = 18;
                        ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#D5F5E3");

                        // التقريب لأقرب 10
                        double roundedTotal = Math.Round(customerTotal / 10.0, MidpointRounding.AwayFromZero) * 10;

                        ws.Range(row, 7, row, 9).Merge().Value = roundedTotal;
                        ws.Range(row, 7, row, 9).Style.Font.Bold = true;
                        ws.Range(row, 7, row, 9).Style.Font.FontSize = 18;
                        ws.Range(row, 7, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#D5F5E3");
                        ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";
                        row++;

                        // 3. رسم الحدود للعميل الحالي
                        var custRange = ws.Range(customerStartRow, 1, row - 1, 9);
                        custRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        custRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        custRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        custRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        // 4. سطرين فارغين للفصل بين الزبائن
                        row += 2;
                        currentBill = "";
                        billTotal = 0;
                    }

                    currentCustomer = customer;
                    customerTotal = 0;
                    customerStartRow = row;

                    // =========================
                    // ترويسة الزبون مع صندوق التوقيع
                    // =========================
                    var sigRange = ws.Range(row, 7, row + 2, 9);
                    sigRange.Merge().Value = "التوقيع";
                    sigRange.Style.Font.FontColor = XLColor.Gray;
                    sigRange.Style.Fill.BackgroundColor = XLColor.White;
                    sigRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sigRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(row, 1).Value = customer;
                    ws.Range(row, 1, row, 6).Merge().Style.Font.Bold = true;
                    ws.Range(row, 1, row, 6).Style.Font.FontColor = XLColor.DarkRed;
                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F3F4");
                    row++;

                    ws.Cell(row, 1).Value = "الحركة اليومية - تفصيلي";
                    ws.Range(row, 1, row, 6).Merge().Style.Font.Bold = true;
                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F3F4");
                    row++;

                    ws.Cell(row, 1).Value = $"اعتباراً من {fromDate:dd-MM-yyyy} ولغاية {toDate:dd-MM-yyyy}";
                    ws.Range(row, 1, row, 6).Merge().Style.Font.Bold = true;
                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F3F4");
                    row++;

                    // رؤوس الأعمدة
                    string[] headers = { "الفاتورة", "التاريخ", "اسم الزبون", "البيان", "بيان القلم", "اسم المادة", "كمية", "الإفرادي", "السعر الإجمالي" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(row, i + 1).Value = headers[i];
                        ws.Cell(row, i + 1).Style.Font.Bold = true;
                        ws.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E8E8");
                    }
                    row++;
                }
                else if (currentBill != billNumber)
                {
                    if (!string.IsNullOrEmpty(currentBill))
                    {
                        billRanges.Add((billStartRow, row - 1));

                        ws.Range(row, 1, row, 6).Merge().Value = "";

                        ws.Range(row, 7, row, 9).Merge().Value = Convert.ToDouble(billTotal);
                        ws.Range(row, 7, row, 9).Style.Font.Bold = true;
                        ws.Range(row, 7, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#F4D03F");
                        ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";
                        row++;
                    }
                }

                if (currentBill != billNumber)
                {
                    currentBill = billNumber;
                    billStartRow = row;
                    billTotal = 0;
                }

                // =========================
                // تعبئة البيانات
                // =========================
                ws.Cell(row, 1).Value = billNumber;
                ws.Cell(row, 2).Value = dr["Date"] != DBNull.Value ? Convert.ToDateTime(dr["Date"]).ToString("yyyy-M-d") : "";
                ws.Cell(row, 3).Value = customer;
                ws.Cell(row, 4).Value = dr["Bill_Note"]?.ToString() ?? "";
                ws.Cell(row, 5).Value = dr["Item_Notes"]?.ToString() ?? "";
                ws.Cell(row, 6).Value = dr["Item_Name"]?.ToString() ?? "";

                ws.Cell(row, 7).Value = Convert.ToDouble(dr["Qty"] ?? 0);
                ws.Cell(row, 8).Value = Convert.ToDouble(dr["Price"] ?? 0);
                ws.Cell(row, 9).Value = Convert.ToDouble(lineTotal);

                ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";

                billTotal += lineTotal;
                customerTotal += lineTotal;
                row++;
            }

            // =========================
            // إغلاق آخر فاتورة وآخر زبون
            // =========================
            if (!string.IsNullOrEmpty(currentCustomer))
            {
                if (!string.IsNullOrEmpty(currentBill))
                {
                    billRanges.Add((billStartRow, row - 1));

                    ws.Range(row, 1, row, 6).Merge().Value = "";

                    ws.Range(row, 7, row, 9).Merge().Value = Convert.ToDouble(billTotal);
                    ws.Range(row, 7, row, 9).Style.Font.Bold = true;
                    ws.Range(row, 7, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#F4D03F");
                    ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";
                    row++;
                }

                ws.Row(row).Height = 35;

                ws.Range(row, 1, row, 6).Merge().Value = $"المجموع الإجمالي للزبون: {currentCustomer}";
                ws.Range(row, 1, row, 6).Style.Font.Bold = true;
                ws.Range(row, 1, row, 6).Style.Font.FontSize = 18;
                ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#D5F5E3");

                // التقريب لأقرب 10
                double roundedTotal = Math.Round(customerTotal / 10.0, MidpointRounding.AwayFromZero) * 10;

                ws.Range(row, 7, row, 9).Merge().Value = roundedTotal;
                ws.Range(row, 7, row, 9).Style.Font.Bold = true;
                ws.Range(row, 7, row, 9).Style.Font.FontSize = 18;
                ws.Range(row, 7, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#D5F5E3");
                ws.Range(row, 7, row, 9).Style.NumberFormat.Format = "#,##0";
                row++;

                var custRange = ws.Range(customerStartRow, 1, row - 1, 9);
                custRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                custRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                custRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                custRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // =========================
            // تنفيذ الدمج الرأسي للفواتير
            // =========================
            foreach (var bill in billRanges)
            {
                if (bill.StartRow < bill.EndRow)
                {
                    ws.Range(bill.StartRow, 1, bill.EndRow, 1).Merge();
                    ws.Range(bill.StartRow, 2, bill.EndRow, 2).Merge();
                    ws.Range(bill.StartRow, 3, bill.EndRow, 3).Merge();
                    ws.Range(bill.StartRow, 4, bill.EndRow, 4).Merge();
                }
            }

            // =========================
            // التنسيق النهائي والأبعاد
            // =========================
            ws.Columns().AdjustToContents();

            ws.Column(6).Width = 35;
            ws.Column(4).Width = 30;
            ws.Column(3).Width = 25;
            ws.Column(9).Width = 15;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}