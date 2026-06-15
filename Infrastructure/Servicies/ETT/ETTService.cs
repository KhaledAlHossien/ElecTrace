using Application_Contract.DTOs.Pattern;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Servicies.ETT
{


  public class ETTService
  {
    private readonly IConfiguration _configuration;

    public ETTService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public DataTable GetReport(string pattern,
                               DateTime fromDate,
                               DateTime toDate)
    {
      string connectionString =
          _configuration.GetConnectionString("AmeenDb");

      using SqlConnection conn =
          new SqlConnection(connectionString);

      using SqlCommand cmd =
          new SqlCommand("PrcImportInvoice", conn);

      cmd.CommandType = CommandType.StoredProcedure;

      cmd.Parameters.AddWithValue("@Type", pattern);
      cmd.Parameters.AddWithValue("@StartDate", fromDate);
      cmd.Parameters.AddWithValue("@EndDate", toDate);

      DataTable dt = new DataTable();

      conn.Open();

      using SqlDataReader reader =
          cmd.ExecuteReader();

      dt.Load(reader);

      return dt;
    }

    public List<PatternDto> GetPatterns()
    {
      var result = new List<PatternDto>();

      string connectionString =
          _configuration.GetConnectionString("AmeenDb");

      using SqlConnection conn =
          new SqlConnection(connectionString);

      string sql = @"
        SELECT Guid, Name
        FROM bt000";

      using SqlCommand cmd =
          new SqlCommand(sql, conn);

      conn.Open();

      using SqlDataReader reader = cmd.ExecuteReader();

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
  }
}