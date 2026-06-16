using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.DTOs.Pattern;
using Application_Contract.DTOs.Role;
using Application_Contract.DTOs.SystemInfo;
using Application_Contract.DTOs.User;
using Domain.Enums;

namespace UI.Services.Interface;

public interface IApiDataService
{
    Task<List<DepartmentResponseDto>> GetDepartmentsAsync();
    Task<List<DepartmentResponseDto>> SearchDepartmentsAsync(string searchText);
    Task<DepartmentResponseDto?> CreateDepartmentAsync(CreateDepartmentRequestDto request);
    Task<DepartmentResponseDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequestDto request);
    Task DeleteDepartmentAsync(int id);

    Task<(byte[] Content, string FileName)> ExportMeterCodesToExcelAsync();



    Task<List<MeterReadingResponseDto>> GetMeterReadingsAsync(Months month, int year);
    Task<List<MeterReadingResponseDto>> GetMeterReadingsByDepartmentAsync(int departmentId);
    Task<MeterReadingResponseDto?> CreateMeterReadingAsync(CreateMeterReadingRequestDto request);
    Task DeleteMeterReadingAsync(int id);

    Task<List<UserResponseDto>> GetUsersAsync();
    Task<UserResponseDto?> CreateUserAsync(CreateUserRequestDto request);
    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserRequestDto request);
    Task DeleteUserAsync(int id);

    Task<List<RoleResponseDto>> GetRolesAsync();
    Task<RoleResponseDto?> CreateRoleAsync(RoleRequestDto request);
    Task<RoleResponseDto?> UpdateRoleAsync(int id, RoleRequestDto request);
    Task DeleteRoleAsync(int id);

    Task<List<SystemInfoResponseDto>> GetSystemInfoAsync();
    Task<SystemInfoResponseDto?> UpdateSystemInfoAsync(int id, UpdateSystemInfoRequestDto request);

    string GetElectricityReportUrl(Months month, int year);
    string GetInvoicesReportUrl(Months month, int year);
    Task<ReportDownloadResult> DownloadElectricityReportAsync(Months month, int year);
    Task<ReportDownloadResult> DownloadInvoicesReportAsync(Months month, int year);
    Task ImportMeterReadingsAsync(Stream fileStream, Months month, int year);
    Task<List<PatternDto>> GetEttPatternsAsync();
    Task<List<Dictionary<string, object>>> GetEttReportAsync(string pattern, DateTime fromDate, DateTime toDate);
    Task<ReportDownloadResult> DownloadEttReportExcelAsync(string pattern, DateTime startDate, DateTime endDate);
}

public sealed record ReportDownloadResult(byte[]? Content, string FileName, string ContentType, string? ErrorMessage)
{
    public bool IsSuccess => Content is { Length: > 0 };
}
