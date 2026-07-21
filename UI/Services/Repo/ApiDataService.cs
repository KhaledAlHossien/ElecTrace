using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.DTOs.Pattern;
using Application_Contract.DTOs.Role;
using Application_Contract.DTOs.SystemInfo;
using Application_Contract.DTOs.User;
using Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using UI.Services.Interface;

namespace UI.Services.Repo;

public class ApiDataService : IApiDataService
{
  private readonly HttpClient _httpClient;

  public ApiDataService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<List<DepartmentResponseDto>> GetDepartmentsAsync()
  {
    return await GetListAsync<DepartmentResponseDto>("api/Department/GetAll");
  }

  public async Task<List<DepartmentResponseDto>> SearchDepartmentsAsync(string searchText)
  {
    if (string.IsNullOrWhiteSpace(searchText))
    {
      return (await GetDepartmentsAsync())
          .OrderBy(x => x.Name)
          .Take(25)
          .ToList();
    }

    return await GetListAsync<DepartmentResponseDto>($"api/Department/Search/{Uri.EscapeDataString(searchText.Trim())}");
  }

  public async Task<DepartmentResponseDto?> CreateDepartmentAsync(CreateDepartmentRequestDto request)
  {
    return await PostAsync<DepartmentResponseDto>("api/Department/Create", request);
  }

  public async Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequestDto request)
  {
    return await PutAsync<DepartmentResponseDto>($"api/Department/Update/{id}", request);
  }

  public async Task DeleteDepartmentAsync(int id)
  {
    await DeleteAsync($"api/Department/Delete/{id}");
  }

  //public async Task<(byte[] Content, string FileName)> ExportMeterCodesToExcelAsync()
  //{
  //    var response = await _httpClient.GetAsync("api/Department/GetAllMeterCodeToExcel");
  //    response.EnsureSuccessStatusCode();
  //    var content = await response.Content.ReadAsByteArrayAsync();

  //    // محاولة استخراج اسم الملف من Content-Disposition
  //    var fileName = "MeterCodes.xlsx"; // القيمة الافتراضية
  //    if (response.Content.Headers.ContentDisposition != null)
  //    {
  //        var cd = response.Content.Headers.ContentDisposition;
  //        fileName = cd.FileNameStar ?? cd.FileName ?? fileName;
  //    }
  //    return (content, fileName);
  //}

  public async Task<(byte[] Content, string FileName)> ExportMeterCodesToExcelAsync()
  {
    var response = await _httpClient.GetAsync("api/Department/GetAllMeterCodeToExcel");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsByteArrayAsync();

    // إنشاء اسم الملف مع التاريخ الحالي
    var fileName = $"MeterCodes_{DateTime.Now:yyyyMMdd}.xlsx";

    return (content, fileName);
  }


  public async Task<List<MeterReadingResponseDto>> GetMeterReadingsAsync(Months month, int year)
  {
    return await GetListAsync<MeterReadingResponseDto>($"api/MeterReading/GetByMonthAndYear?month={month}&year={year}");
  }

  public async Task<List<MeterReadingResponseDto>> GetMeterReadingsByDepartmentAsync(int departmentId)
  {
    return await GetListAsync<MeterReadingResponseDto>($"api/MeterReading/GetByDepartment/{departmentId}");
  }

  public async Task<MeterReadingResponseDto?> CreateMeterReadingAsync(CreateMeterReadingRequestDto request)
  {
    return await PostAsync<MeterReadingResponseDto>("api/MeterReading/Create", request);
  }

  public async Task DeleteMeterReadingAsync(int id)
  {
    await DeleteAsync($"api/MeterReading/Delete/{id}");
  }

  public async Task<List<UserResponseDto>> GetUsersAsync()
  {
    return await GetListAsync<UserResponseDto>("api/User/GetAll");
  }

  public async Task<UserResponseDto?> CreateUserAsync(CreateUserRequestDto request)
  {
    return await PostAsync<UserResponseDto>("api/User/Create", request);
  }

  public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserRequestDto request)
  {
    return await PutAsync<UserResponseDto>($"api/User/Update/{id}", request);
  }

  public async Task DeleteUserAsync(int id)
  {
    await DeleteAsync($"api/User/Delete/{id}");
  }

  public async Task<List<RoleResponseDto>> GetRolesAsync()
  {
    return await GetListAsync<RoleResponseDto>("api/Role/GetAll");
  }

  public async Task<RoleResponseDto?> CreateRoleAsync(RoleRequestDto request)
  {
    using var content = BuildRoleForm(request);
    using var response = await _httpClient.PostAsync("api/Role/Create", content);
    return await ReadAsync<RoleResponseDto>(response);
  }

  public async Task<RoleResponseDto?> UpdateRoleAsync(int id, RoleRequestDto request)
  {
    using var content = BuildRoleForm(request);
    using var response = await _httpClient.PutAsync($"api/Role/Update/{id}", content);
    return await ReadAsync<RoleResponseDto>(response);
  }

  public async Task DeleteRoleAsync(int id)
  {
    await DeleteAsync($"api/Role/Delete/{id}");
  }

  public async Task<List<SystemInfoResponseDto>> GetSystemInfoAsync()
  {
    return await GetListAsync<SystemInfoResponseDto>("api/SystemInfo/GetAll");
  }

  public async Task<SystemInfoResponseDto?> UpdateSystemInfoAsync(int id, UpdateSystemInfoRequestDto request)
  {
    return await PutAsync<SystemInfoResponseDto>($"api/SystemInfo/Update/{id}", request);
  }

  public string GetElectricityReportUrl(Months month, int year)
  {
    return new Uri(_httpClient.BaseAddress!, $"api/MeterReading/DownloadElectricityExcelReport?month={month}&year={year}").ToString();
  }

  public string GetInvoicesReportUrl(Months month, int year)
  {
    return new Uri(_httpClient.BaseAddress!, $"api/MeterReading/DownloadAllInvoices?month={month}&year={year}").ToString();
  }

  public async Task<ReportDownloadResult> DownloadElectricityReportAsync(Months month, int year)
  {
    var fileName = $"Electricity_Report_{year}_{(int)month}.xlsx";
    return await DownloadReportAsync($"api/MeterReading/DownloadElectricityExcelReport?month={month}&year={year}", fileName);
  }

  public async Task<ReportDownloadResult> DownloadInvoicesReportAsync(Months month, int year)
  {
    var fileName = $"All_Invoices_{year}_{(int)month}.xlsx";
    return await DownloadReportAsync($"api/MeterReading/DownloadAllInvoices?month={month}&year={year}", fileName);
  }

  private async Task<List<T>> GetListAsync<T>(string url)
  {
    using var response = await _httpClient.GetAsync(url);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return [];
    }

    return await ReadAsync<List<T>>(response) ?? [];
  }

  private async Task<T?> PostAsync<T>(string url, object request)
  {
    using var response = await _httpClient.PostAsJsonAsync(url, request);
    return await ReadAsync<T>(response);
  }

  private async Task<T?> PutAsync<T>(string url, object request)
  {
    using var response = await _httpClient.PutAsJsonAsync(url, request);
    return await ReadAsync<T>(response);
  }

  private async Task DeleteAsync(string url)
  {
    using var response = await _httpClient.DeleteAsync(url);
    await EnsureSuccessAsync(response);
  }

  private static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
  {
    await EnsureSuccessAsync(response);
    return await response.Content.ReadFromJsonAsync<T>();
  }

  private static async Task EnsureSuccessAsync(HttpResponseMessage response)
  {
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    var error = await response.Content.ReadAsStringAsync();
    throw new InvalidOperationException(string.IsNullOrWhiteSpace(error) ? response.ReasonPhrase : error);
  }

  private static MultipartFormDataContent BuildRoleForm(RoleRequestDto request)
  {
    return new MultipartFormDataContent
        {
            { new StringContent(request.Name), nameof(RoleRequestDto.Name) }
        };
  }

  private async Task<ReportDownloadResult> DownloadReportAsync(string url, string fileName)
  {
    using var response = await _httpClient.GetAsync(url);
    var contentType = response.Content.Headers.ContentType?.MediaType
        ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    if (response.IsSuccessStatusCode)
    {
      var bytes = await response.Content.ReadAsByteArrayAsync();
      return new ReportDownloadResult(bytes, fileName, contentType, null);
    }

    var error = await ReadErrorMessageAsync(response);
    return new ReportDownloadResult(null, fileName, contentType, error);
  }

  private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
  {
    var body = await response.Content.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(body))
    {
      return response.ReasonPhrase ?? "تعذر تنفيذ العملية.";
    }

    try
    {
      using var document = JsonDocument.Parse(body);
      if (document.RootElement.TryGetProperty("message", out var message))
      {
        return message.GetString() ?? body;
      }
    }
    catch (JsonException)
    {
      return body;
    }

    return body;
  }

  public async Task ImportMeterReadingsAsync(Stream fileStream, Months month, int year)
  {
    using var content = new MultipartFormDataContent();

    // تحويل الـ Stream إلى StreamContent
    var fileContent = new StreamContent(fileStream);

    // إضافة الملف إلى الـ Form
    content.Add(fileContent, "file", "readings.xlsx");

    // إرسال الطلب (تأكد من مطابقة المسار في الـ API)
    using var response = await _httpClient.PostAsync($"api/MeterReading/Import?month={month}&year={year}", content);

    await EnsureSuccessAsync(response);
  }

  public async Task ImportCurrentReadingsAsync(Stream fileStream, Months month, int year)
  {
    using var content = new MultipartFormDataContent();
    var fileContent = new StreamContent(fileStream);
    content.Add(fileContent, "file", "readings.xlsx");
    using var response = await _httpClient.PostAsync($"api/MeterReading/importCurrentReadings?month={month}&year={year}", content);
    await EnsureSuccessAsync(response);
  }

  public async Task<List<PatternDto>> GetEttPatternsAsync()
  {
    return await GetListAsync<PatternDto>("api/ETT/patterns");
  }

  public async Task<List<Dictionary<string, object>>> GetEttReportAsync(string pattern, DateTime fromDate, DateTime toDate)
  {
    var url = $"api/ETT/report?pattern={Uri.EscapeDataString(pattern)}&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
    return await GetListAsync<Dictionary<string, object>>(url);
  }

  public async Task<ReportDownloadResult> DownloadEttReportExcelAsync(string pattern, DateTime startDate, DateTime endDate)
  {
    var url = $"api/ETT/ExportExcel?pattern={Uri.EscapeDataString(pattern)}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
    var fileName = $"SalesReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
    return await DownloadReportAsync(url, fileName);
  }

  public async Task ImportDepartmentsAsync(Stream fileStream)
  {
    using var content = new MultipartFormDataContent();

    var fileContent = new StreamContent(fileStream);

    content.Add(fileContent, "file", "departments.xlsx");

    using var response = await _httpClient.PostAsync($"api/Department/Import", content);

    await EnsureSuccessAsync(response);
  }
}
