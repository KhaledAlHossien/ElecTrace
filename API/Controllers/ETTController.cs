using Infrastructure.Servicies.ETT;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ETTController : ControllerBase
  {
    private readonly ETTService _ettService;

    public ETTController(ETTService ettService)
    {
      _ettService = ettService;
    }

    public static List<Dictionary<string, object>> ToList(DataTable table)
    {
      var list = new List<Dictionary<string, object>>();

      foreach (DataRow row in table.Rows)
      {
        var dict = new Dictionary<string, object>();

        foreach (DataColumn col in table.Columns)
        {
          dict[col.ColumnName] = row[col];
        }

        list.Add(dict);
      }

      return list;
    }


    [HttpGet("report")]
    public IActionResult GetReport(
        string pattern,
        DateTime fromDate,
        DateTime toDate)
    {
      try
      {
        var result = _ettService.GetReport(
            pattern,
            fromDate,
            toDate);

        var list = ToList(result);
        return Ok(list);

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("patterns")]
    public IActionResult GetPatterns()
    {
      try
      {
        var patterns = _ettService.GetPatterns();

        return Ok(patterns);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("ExportExcel")]
    public IActionResult ExportExcel(
    string pattern,
    DateTime startDate,
    DateTime endDate)
    {
      var file = _ettService.GenerateReportExcel(
          pattern,
          startDate,
          endDate);

      return File(
          file,
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
          $"SalesReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
  }
}