using Application_Contract.DTOs.Pattern;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;
using ClosedXML.Excel;

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


    public byte[] GenerateReportExcel(
    string pattern,
    DateTime fromDate,
    DateTime toDate)
    {
      DataTable dt = GetReport(
          pattern,
          fromDate,
          toDate);

      using var workbook = new XLWorkbook();

      var ws = workbook.Worksheets.Add("Report");

      ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

      ws.RightToLeft = true;
      ws.ShowGridLines = false;

      // =========================
      // Title
      // =========================

      ws.Cell("A1").Value = "تقرير المبيعات";

      ws.Range("A1:H1").Merge();

      ws.Range("A1:H1").Style.Font.Bold = true;
      ws.Range("A1:H1").Style.Font.FontSize = 18;
      ws.Range("A1:H1").Style.Font.FontColor = XLColor.White;
      ws.Range("A1:H1").Style.Fill.BackgroundColor =
          XLColor.FromHtml("#2c3e50");

      ws.Range("A1:H1").Style.Alignment.Horizontal =
          XLAlignmentHorizontalValues.Center;

      ws.Cell("A2").Value =
          $"من {fromDate:yyyy/MM/dd} إلى {toDate:yyyy/MM/dd}";

      ws.Cell("A3").Value =
          $"تاريخ الإنشاء : {DateTime.Now:yyyy/MM/dd HH:mm}";

      int row = 5;

      string currentCustomer = "";
      decimal billTotal = 0;
      decimal customerTotal = 0;
      decimal grandTotal = 0;

      var billRanges =
          new List<(int StartRow, int EndRow)>();

      string currentBill = "";
      int billStartRow = 0;

      foreach (DataRow dr in dt.Rows)
      {
        string customer =
            dr["Cust_Name"]?.ToString() ?? "";

        string billNumber =
            dr["BillNumber"]?.ToString() ?? "";

        decimal lineTotal =
            Convert.ToDecimal(dr["Total"]);

        // =========================
        // New Customer
        // =========================

        if (currentCustomer != customer)
        {
          if (!string.IsNullOrEmpty(currentBill))
          {
            billRanges.Add((billStartRow, row - 1));

            ws.Cell(row, 1).Value =
                $"إجمالي الفاتورة : {billTotal:N2}";

            ws.Range(row, 1, row, 8).Merge();

            ws.Range(row, 1, row, 8)
                .Style.Fill.BackgroundColor =
                XLColor.FromHtml("#fdebd0");

            ws.Range(row, 1, row, 8)
                .Style.Font.Bold = true;

            row++;

            currentBill = "";
            billTotal = 0;
          }

          if (!string.IsNullOrEmpty(currentCustomer))
          {
            ws.Cell(row, 1).Value =
                $"إجمالي العميل : {customerTotal:N2}";

            ws.Range(row, 1, row, 8).Merge();

            ws.Range(row, 1, row, 8)
                .Style.Fill.BackgroundColor =
                XLColor.FromHtml("#d5f5e3");

            ws.Range(row, 1, row, 8)
                .Style.Font.Bold = true;

            row += 2;
          }

          customerTotal = 0;

          currentCustomer = customer;

          // عنوان العميل

          ws.Cell(row, 1).Value =
              $"العميل : {customer}";

          ws.Range(row, 1, row, 8).Merge();

          ws.Range(row, 1, row, 8)
              .Style.Fill.BackgroundColor =
              XLColor.FromHtml("#3498db");

          ws.Range(row, 1, row, 8)
              .Style.Font.FontColor =
              XLColor.White;

          ws.Range(row, 1, row, 8)
              .Style.Font.Bold = true;

          row++;

          string[] headers =
          {
                "رقم الفاتورة",
                "التاريخ",
                "اسم المادة",
                "ملاحظات المادة",
                "ملاحظات الفاتورة",
                "الكمية",
                "السعر",
                "الإجمالي"
            };

          for (int i = 0; i < headers.Length; i++)
          {
            var cell = ws.Cell(row, i + 1);

            cell.Value = headers[i];

            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor =
                XLColor.White;

            cell.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#34495e");

            cell.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
          }

          row++;
        }

        // =========================
        // New Bill
        // =========================

        if (currentBill != billNumber)
        {
          if (!string.IsNullOrEmpty(currentBill))
          {
            billRanges.Add(
                (billStartRow, row - 1));

            ws.Cell(row, 1).Value =
                $"إجمالي الفاتورة : {billTotal:N2}";

            ws.Range(row, 1, row, 8).Merge();

            ws.Range(row, 1, row, 8)
                .Style.Fill.BackgroundColor =
                XLColor.FromHtml("#FFF3CD");

            ws.Range(row, 1, row, 8)
                .Style.Font.Bold = true;

            row++;
          }

          currentBill = billNumber;
          billStartRow = row;
          billTotal = 0;
        }

        // =========================
        // Details
        // =========================

        ws.Cell(row, 1).Value = billNumber;

        ws.Cell(row, 2).Value =
            Convert.ToDateTime(dr["Date"]);

        ws.Cell(row, 3).Value =
            dr["Item_Name"]?.ToString();

        ws.Cell(row, 4).Value =
            dr["Item_Notes"]?.ToString();

        ws.Cell(row, 5).Value =
            dr["Bill_Note"]?.ToString();

        ws.Cell(row, 6).Value =
            Convert.ToDecimal(dr["Qty"]);

        ws.Cell(row, 7).Value =
            Convert.ToDecimal(dr["Price"]);

        ws.Cell(row, 8).Value =
            lineTotal;

        ws.Cell(row, 2)
            .Style.DateFormat.Format =
            "yyyy/MM/dd";

        ws.Cell(row, 6)
            .Style.NumberFormat.Format =
            "#,##0";

        ws.Cell(row, 7)
            .Style.NumberFormat.Format =
            "#,##0.00";

        ws.Cell(row, 8)
            .Style.NumberFormat.Format =
            "#,##0.00";

        var detailRange =
            ws.Range(row, 1, row, 8);

        detailRange.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        detailRange.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;

        if (row % 2 == 0)
        {
          detailRange.Style.Fill.BackgroundColor =
              XLColor.FromHtml("#f8f9fa");
        }

        customerTotal += lineTotal;
        grandTotal += lineTotal;
        billTotal += lineTotal;

        row++;
      }
      // =========================
      // Last Bill
      // =========================

      if (!string.IsNullOrEmpty(currentBill))
      {
        billRanges.Add(
            (billStartRow, row - 1));

        ws.Cell(row, 1).Value =
            $"إجمالي الفاتورة : {billTotal:N2}";

        ws.Range(row, 1, row, 8).Merge();

        ws.Range(row, 1, row, 8)
            .Style.Fill.BackgroundColor =
            XLColor.FromHtml("#fdebd0");

        ws.Range(row, 1, row, 8)
            .Style.Font.Bold = true;

        row++;
      }


      // =========================
      // Last Customer Total
      // =========================

      if (!string.IsNullOrEmpty(currentCustomer))
      {
        ws.Cell(row, 1).Value =
            $"إجمالي العميل : {customerTotal:N2}";

        ws.Range(row, 1, row, 8).Merge();

        ws.Range(row, 1, row, 8)
            .Style.Fill.BackgroundColor =
            XLColor.FromHtml("#d5f5e3");

        ws.Range(row, 1, row, 8)
            .Style.Font.Bold = true;

        row += 2;
      }

      // =========================
      // Merge Bills
      // =========================

      foreach (var bill in billRanges)
      {
        if (bill.EndRow <= bill.StartRow)
          continue;

        ws.Range(
            bill.StartRow,
            1,
            bill.EndRow,
            1).Merge();

        ws.Range(
            bill.StartRow,
            2,
            bill.EndRow,
            2).Merge();

        ws.Range(
            bill.StartRow,
            5,
            bill.EndRow,
            5).Merge();

        ws.Range(
            bill.StartRow,
            1,
            bill.EndRow,
            5)
            .Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        ws.Range(
            bill.StartRow,
            1,
            bill.EndRow,
            5)
            .Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
      }

      // =========================
      // Grand Total
      // =========================

      ws.Cell(row, 1).Value =
          $"الإجمالي العام : {grandTotal:N2}";

      ws.Range(row, 1, row, 8).Merge();

      ws.Range(row, 1, row, 8)
          .Style.Fill.BackgroundColor =
          XLColor.FromHtml("#27ae60");

      ws.Range(row, 1, row, 8)
          .Style.Font.FontColor =
          XLColor.White;

      ws.Range(row, 1, row, 8)
          .Style.Font.Bold = true;

      ws.Range(row, 1, row, 8)
          .Style.Font.FontSize = 14;

      ws.Columns().AdjustToContents();

      ws.Column(4).Width = 30;
      ws.Column(5).Width = 40;

      ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

      using var stream = new MemoryStream();

      workbook.SaveAs(stream);

      return stream.ToArray();
    }
  }
}