using System.Text;

namespace UI.Services;

// توحيد النص العربي قبل المقارنة حتى يلاقي البحث النتيجة رغم اختلاف طريقة الكتابة:
// أحمد = احمد، مدرسة = مدرسه، على = علي، والتشكيل والتطويل ما بيأثروا، والأرقام الشرقية (٣) = العادية (3)
public static class SearchNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var builder = new StringBuilder(value.Length);

        foreach (var ch in value.Trim())
        {
            switch (ch)
            {
                case 'أ' or 'إ' or 'آ' or 'ٱ':
                    builder.Append('ا');
                    break;
                case 'ى' or 'ئ':
                    builder.Append('ي');
                    break;
                case 'ة':
                    builder.Append('ه');
                    break;
                case 'ؤ':
                    builder.Append('و');
                    break;
                case 'ـ':
                    break;
                case >= 'ً' and <= 'ٟ':
                case 'ٰ':
                    break;
                case >= '٠' and <= '٩':
                    builder.Append((char)('0' + (ch - '٠')));
                    break;
                case >= '۰' and <= '۹':
                    builder.Append((char)('0' + (ch - '۰')));
                    break;
                default:
                    builder.Append(char.ToLowerInvariant(ch));
                    break;
            }
        }

        return builder.ToString();
    }

    public static string[] Tokens(string? query) =>
        Normalize(query).Split(' ', StringSplitOptions.RemoveEmptyEntries);
}
