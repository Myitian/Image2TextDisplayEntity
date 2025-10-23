using System.Buffers;
using System.Text;

namespace Image2TextDisplayEntity.WPF;

public static class StringBuilderExtension
{
    public static StringBuilder AppendEscaped(this StringBuilder sb, ReadOnlySpan<char> text, SearchValues<char> charsToEscape, out int writeLength, char escapeChar = '\\')
    {
        int escU8Len = new Rune(escapeChar).Utf8SequenceLength;
        writeLength = 0;
        while (text.Length > 0)
        {
            int i = text.IndexOfAny(charsToEscape);
            if (i < 0)
            {
                sb.Append(text);
                writeLength += MainWindow.UTF8.GetByteCount(text);
                return sb;
            }
            sb.Append(text[..i]);
            sb.Append(escapeChar);
            sb.Append(text[i]);
            writeLength += MainWindow.UTF8.GetByteCount(text[..(i + 1)]) + escU8Len;
            text = text[(i + 1)..];
        }
        return sb;
    }
}
