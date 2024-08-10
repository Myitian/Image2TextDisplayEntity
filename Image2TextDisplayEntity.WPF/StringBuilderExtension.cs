using System.Buffers;
using System.Text;

namespace Image2TextDisplayEntity.WPF;

public static class StringBuilderExtension
{
    public static StringBuilder AppendEscaped(this StringBuilder sb, ReadOnlySpan<char> text, SearchValues<char> charsToEscape, char escapeChar = '\\')
    {
        while (text.Length > 0)
        {
            int i = text.IndexOfAny(charsToEscape);
            if (i < 0)
            {
                sb.Append(text);
                return sb;
            }
            sb.Append(text[..i]);
            sb.Append(escapeChar);
            sb.Append(text[i]);
            text = text[(i + 1)..];
        }
        return sb;
    }
}
