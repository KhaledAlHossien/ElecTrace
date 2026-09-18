using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Infrastructure.Servicies
{
    public class AmeenConnectionService : IAmeenConnectionService
    {
        // مهلة الاتصال: لازم تتحمّل الاتصال البارد الأول بالنسخة المسمّاة (القياس الفعلي ~23 ثانية)
        private const int ConnectTimeoutSeconds = 45;

        private readonly DataContext _context;

        public AmeenConnectionService(DataContext context)
        {
            _context = context;
        }

        public async Task<AmeenConnectionDto> GetAsync()
        {
            var stored = await _context.SystemInfos
                .AsNoTracking()
                .Select(s => s.AmeenConnectionString)
                .FirstOrDefaultAsync();

            return ToDto(stored);
        }

        public async Task<AmeenConnectionDto> UpdateAsync(UpdateAmeenConnectionRequestDto request)
        {
            var systemInfo = await _context.SystemInfos.FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("لم يتم العثور على إعدادات النظام.");

            systemInfo.AmeenConnectionString = BuildConnectionString(request, systemInfo.AmeenConnectionString);
            await _context.SaveChangesAsync();

            return ToDto(systemInfo.AmeenConnectionString);
        }

        public async Task<AmeenConnectionTestResultDto> TestAsync(UpdateAmeenConnectionRequestDto request)
        {
            var stored = await _context.SystemInfos
                .AsNoTracking()
                .Select(s => s.AmeenConnectionString)
                .FirstOrDefaultAsync();

            var builder = new SqlConnectionStringBuilder(BuildConnectionString(request, stored))
            {
                // أول اتصال بنسخة مسمّاة (SRV1\MSSQL2014) بياخد وقت طويل بسبب تحويل الاسم عبر SQL Browser
                // (مقيس: ~23 ثانية أول مرة وبعدها فوري)، فمهلة قصيرة بتفشل الاختبار رغم إنو الاتصال سليم
                ConnectTimeout = ConnectTimeoutSeconds
            };

            try
            {
                await using var connection = new SqlConnection(builder.ConnectionString);
                await connection.OpenAsync();

                // bt000 جدول الأنماط بنظام الأمين - نجاح الاستعلام بيأكد إنها فعلاً قاعدة الأمين الصحيحة
                await using var command = new SqlCommand("SELECT COUNT(*) FROM bt000", connection);
                var patterns = Convert.ToInt32(await command.ExecuteScalarAsync());

                return new AmeenConnectionTestResultDto(true, $"تم الاتصال بنجاح — {patterns} نمط متاح.", patterns);
            }
            catch (SqlException ex)
            {
                // أي رقم خطأ غير معروف بيكون عملياً مشكلة وصول للسيرفر، فمنعطي رسالة مفهومة
                // بدل نص SQL الإنكليزي الطويل
                var message = ex.Number switch
                {
                    18456 => "اسم المستخدم أو كلمة المرور غير صحيحة.",
                    4060 => "قاعدة البيانات غير موجودة أو لا توجد صلاحية للوصول إليها.",
                    208 => "تم الاتصال، لكن قاعدة البيانات ليست قاعدة نظام الأمين (جدول الأنماط bt000 غير موجود).",
                    _ => "تعذّر الوصول إلى السيرفر. تحقق من اسم السيرفر واسم النسخة (Instance) والشبكة، وأن SQL Server يسمح بالاتصالات البعيدة."
                };

                return new AmeenConnectionTestResultDto(false, message, null);
            }
            catch (Exception ex)
            {
                return new AmeenConnectionTestResultDto(false, "فشل الاتصال: " + ex.Message, null);
            }
        }

        public string GetConnectionString()
        {
            var stored = _context.SystemInfos
                .AsNoTracking()
                .Select(s => s.AmeenConnectionString)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(stored))
                throw new InvalidOperationException("اتصال نظام الأمين غير مُعدّ. اضبطه من صفحة الإعدادات.");

            var builder = TryParse(stored)
                ?? throw new InvalidOperationException("سلسلة اتصال نظام الأمين المحفوظة غير صالحة. أعد ضبطها من صفحة الإعدادات.");

            // منضمن المهلة حتى للسلاسل المحفوظة من قبل، حتى ما يفشل أول اتصال بارد على المهلة الافتراضية (15 ثانية)
            if (builder.ConnectTimeout < ConnectTimeoutSeconds)
                builder.ConnectTimeout = ConnectTimeoutSeconds;

            return builder.ConnectionString;
        }

        // أي سلسلة محفوظة معطوبة (مثلاً ناقصها Server=) ما لازم تكسّر الشاشة، منرجّع null
        // حتى تقدر تعيد إدخالها من الإعدادات
        private static SqlConnectionStringBuilder? TryParse(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            try
            {
                return new SqlConnectionStringBuilder(connectionString);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        // بيبني على السلسلة المحفوظة حتى ما تضيع خيارات إضافية (مثل TrustServerCertificate)،
        // وكلمة المرور الفارغة معناها الإبقاء على المحفوظة
        private static string BuildConnectionString(UpdateAmeenConnectionRequestDto request, string? existing)
        {
            var builder = TryParse(existing) ?? new SqlConnectionStringBuilder { TrustServerCertificate = true };

            builder.DataSource = request.Server.Trim();
            builder.InitialCatalog = request.Database.Trim();
            builder.IntegratedSecurity = false;
            builder.UserID = request.UserId.Trim();

            if (!string.IsNullOrEmpty(request.Password))
                builder.Password = request.Password;

            if (builder.ConnectTimeout < ConnectTimeoutSeconds)
                builder.ConnectTimeout = ConnectTimeoutSeconds;

            return builder.ConnectionString;
        }

        private static AmeenConnectionDto ToDto(string? connectionString)
        {
            var builder = TryParse(connectionString);

            if (builder is null)
                return new AmeenConnectionDto("", "", "", false, false);

            return new AmeenConnectionDto(
                builder.DataSource,
                builder.InitialCatalog,
                builder.UserID,
                !string.IsNullOrEmpty(builder.Password),
                true);
        }
    }
}
