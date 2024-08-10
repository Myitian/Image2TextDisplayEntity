using System.Buffers;
using System.Globalization;
using System.Windows.Media;

namespace Image2TextDisplayEntity.WPF;

public static class ColorExtension
{
    public const double DegMultiplier = 1.0 / 360;
    public const double GradMultiplier = 1.0 / 400;
    public const double RadMultiplier = 0.5 / Math.PI;
    public const double TurnMultiplier = 1;

    public static readonly SearchValues<char> CSSWhitespaces = SearchValues.Create("\t\n\f\r ");
    private static readonly Dictionary<string, Color> CSSColorNameMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "aliceblue",            Color.FromRgb(240, 248, 255) },
        { "antiquewhite",         Color.FromRgb(250, 235, 215) },
        { "aqua",                 Color.FromRgb(0, 255, 255)   },
        { "aquamarine",           Color.FromRgb(127, 255, 212) },
        { "azure",                Color.FromRgb(240, 255, 255) },
        { "beige",                Color.FromRgb(245, 245, 220) },
        { "bisque",               Color.FromRgb(255, 228, 196) },
        { "black",                Color.FromRgb(0, 0, 0)       },
        { "blanchedalmond",       Color.FromRgb(255, 235, 205) },
        { "blue",                 Color.FromRgb(0, 0, 255)     },
        { "blueviolet",           Color.FromRgb(138, 43, 226)  },
        { "brown",                Color.FromRgb(165, 42, 42)   },
        { "burlywood",            Color.FromRgb(222, 184, 135) },
        { "cadetblue",            Color.FromRgb(95, 158, 160)  },
        { "chartreuse",           Color.FromRgb(127, 255, 0)   },
        { "chocolate",            Color.FromRgb(210, 105, 30)  },
        { "coral",                Color.FromRgb(255, 127, 80)  },
        { "cornflowerblue",       Color.FromRgb(100, 149, 237) },
        { "cornsilk",             Color.FromRgb(255, 248, 220) },
        { "crimson",              Color.FromRgb(220, 20, 60)   },
        { "cyan",                 Color.FromRgb(0, 255, 255)   },
        { "darkblue",             Color.FromRgb(0, 0, 139)     },
        { "darkcyan",             Color.FromRgb(0, 139, 139)   },
        { "darkgoldenrod",        Color.FromRgb(184, 134, 11)  },
        { "darkgray",             Color.FromRgb(169, 169, 169) },
        { "darkgreen",            Color.FromRgb(0, 100, 0)     },
        { "darkgrey",             Color.FromRgb(169, 169, 169) },
        { "darkkhaki",            Color.FromRgb(189, 183, 107) },
        { "darkmagenta",          Color.FromRgb(139, 0, 139)   },
        { "darkolivegreen",       Color.FromRgb(85, 107, 47)   },
        { "darkorange",           Color.FromRgb(255, 140, 0)   },
        { "darkorchid",           Color.FromRgb(153, 50, 204)  },
        { "darkred",              Color.FromRgb(139, 0, 0)     },
        { "darksalmon",           Color.FromRgb(233, 150, 122) },
        { "darkseagreen",         Color.FromRgb(143, 188, 143) },
        { "darkslateblue",        Color.FromRgb(72, 61, 139)   },
        { "darkslategray",        Color.FromRgb(47, 79, 79)    },
        { "darkslategrey",        Color.FromRgb(47, 79, 79)    },
        { "darkturquoise",        Color.FromRgb(0, 206, 209)   },
        { "darkviolet",           Color.FromRgb(148, 0, 211)   },
        { "deeppink",             Color.FromRgb(255, 20, 147)  },
        { "deepskyblue",          Color.FromRgb(0, 191, 255)   },
        { "dimgray",              Color.FromRgb(105, 105, 105) },
        { "dimgrey",              Color.FromRgb(105, 105, 105) },
        { "dodgerblue",           Color.FromRgb(30, 144, 255)  },
        { "firebrick",            Color.FromRgb(178, 34, 34)   },
        { "floralwhite",          Color.FromRgb(255, 250, 240) },
        { "forestgreen",          Color.FromRgb(34, 139, 34)   },
        { "fuchsia",              Color.FromRgb(255, 0, 255)   },
        { "gainsboro",            Color.FromRgb(220, 220, 220) },
        { "ghostwhite",           Color.FromRgb(248, 248, 255) },
        { "gold",                 Color.FromRgb(255, 215, 0)   },
        { "goldenrod",            Color.FromRgb(218, 165, 32)  },
        { "gray",                 Color.FromRgb(128, 128, 128) },
        { "green",                Color.FromRgb(0, 128, 0)     },
        { "greenyellow",          Color.FromRgb(173, 255, 47)  },
        { "grey",                 Color.FromRgb(128, 128, 128) },
        { "honeydew",             Color.FromRgb(240, 255, 240) },
        { "hotpink",              Color.FromRgb(255, 105, 180) },
        { "indianred",            Color.FromRgb(205, 92, 92)   },
        { "indigo",               Color.FromRgb(75, 0, 130)    },
        { "ivory",                Color.FromRgb(255, 255, 240) },
        { "khaki",                Color.FromRgb(240, 230, 140) },
        { "lavender",             Color.FromRgb(230, 230, 250) },
        { "lavenderblush",        Color.FromRgb(255, 240, 245) },
        { "lawngreen",            Color.FromRgb(124, 252, 0)   },
        { "lemonchiffon",         Color.FromRgb(255, 250, 205) },
        { "lightblue",            Color.FromRgb(173, 216, 230) },
        { "lightcoral",           Color.FromRgb(240, 128, 128) },
        { "lightcyan",            Color.FromRgb(224, 255, 255) },
        { "lightgoldenrodyellow", Color.FromRgb(250, 250, 210) },
        { "lightgray",            Color.FromRgb(211, 211, 211) },
        { "lightgreen",           Color.FromRgb(144, 238, 144) },
        { "lightgrey",            Color.FromRgb(211, 211, 211) },
        { "lightpink",            Color.FromRgb(255, 182, 193) },
        { "lightsalmon",          Color.FromRgb(255, 160, 122) },
        { "lightseagreen",        Color.FromRgb(32, 178, 170)  },
        { "lightskyblue",         Color.FromRgb(135, 206, 250) },
        { "lightslategray",       Color.FromRgb(119, 136, 153) },
        { "lightslategrey",       Color.FromRgb(119, 136, 153) },
        { "lightsteelblue",       Color.FromRgb(176, 196, 222) },
        { "lightyellow",          Color.FromRgb(255, 255, 224) },
        { "lime",                 Color.FromRgb(0, 255, 0)     },
        { "limegreen",            Color.FromRgb(50, 205, 50)   },
        { "linen",                Color.FromRgb(250, 240, 230) },
        { "magenta",              Color.FromRgb(255, 0, 255)   },
        { "maroon",               Color.FromRgb(128, 0, 0)     },
        { "mediumaquamarine",     Color.FromRgb(102, 205, 170) },
        { "mediumblue",           Color.FromRgb(0, 0, 205)     },
        { "mediumorchid",         Color.FromRgb(186, 85, 211)  },
        { "mediumpurple",         Color.FromRgb(147, 112, 219) },
        { "mediumseagreen",       Color.FromRgb(60, 179, 113)  },
        { "mediumslateblue",      Color.FromRgb(123, 104, 238) },
        { "mediumspringgreen",    Color.FromRgb(0, 250, 154)   },
        { "mediumturquoise",      Color.FromRgb(72, 209, 204)  },
        { "mediumvioletred",      Color.FromRgb(199, 21, 133)  },
        { "midnightblue",         Color.FromRgb(25, 25, 112)   },
        { "mintcream",            Color.FromRgb(245, 255, 250) },
        { "mistyrose",            Color.FromRgb(255, 228, 225) },
        { "moccasin",             Color.FromRgb(255, 228, 181) },
        { "navajowhite",          Color.FromRgb(255, 222, 173) },
        { "navy",                 Color.FromRgb(0, 0, 128)     },
        { "oldlace",              Color.FromRgb(253, 245, 230) },
        { "olive",                Color.FromRgb(128, 128, 0)   },
        { "olivedrab",            Color.FromRgb(107, 142, 35)  },
        { "orange",               Color.FromRgb(255, 165, 0)   },
        { "orangered",            Color.FromRgb(255, 69, 0)    },
        { "orchid",               Color.FromRgb(218, 112, 214) },
        { "palegoldenrod",        Color.FromRgb(238, 232, 170) },
        { "palegreen",            Color.FromRgb(152, 251, 152) },
        { "paleturquoise",        Color.FromRgb(175, 238, 238) },
        { "palevioletred",        Color.FromRgb(219, 112, 147) },
        { "papayawhip",           Color.FromRgb(255, 239, 213) },
        { "peachpuff",            Color.FromRgb(255, 218, 185) },
        { "peru",                 Color.FromRgb(205, 133, 63)  },
        { "pink",                 Color.FromRgb(255, 192, 203) },
        { "plum",                 Color.FromRgb(221, 160, 221) },
        { "powderblue",           Color.FromRgb(176, 224, 230) },
        { "purple",               Color.FromRgb(128, 0, 128)   },
        { "rebeccapurple",        Color.FromRgb(102, 51, 153)  },
        { "red",                  Color.FromRgb(255, 0, 0)     },
        { "rosybrown",            Color.FromRgb(188, 143, 143) },
        { "royalblue",            Color.FromRgb(65, 105, 225)  },
        { "saddlebrown",          Color.FromRgb(139, 69, 19)   },
        { "salmon",               Color.FromRgb(250, 128, 114) },
        { "sandybrown",           Color.FromRgb(244, 164, 96)  },
        { "seagreen",             Color.FromRgb(46, 139, 87)   },
        { "seashell",             Color.FromRgb(255, 245, 238) },
        { "sienna",               Color.FromRgb(160, 82, 45)   },
        { "silver",               Color.FromRgb(192, 192, 192) },
        { "skyblue",              Color.FromRgb(135, 206, 235) },
        { "slateblue",            Color.FromRgb(106, 90, 205)  },
        { "slategray",            Color.FromRgb(112, 128, 144) },
        { "slategrey",            Color.FromRgb(112, 128, 144) },
        { "snow",                 Color.FromRgb(255, 250, 250) },
        { "springgreen",          Color.FromRgb(0, 255, 127)   },
        { "steelblue",            Color.FromRgb(70, 130, 180)  },
        { "tan",                  Color.FromRgb(210, 180, 140) },
        { "teal",                 Color.FromRgb(0, 128, 128)   },
        { "thistle",              Color.FromRgb(216, 191, 216) },
        { "tomato",               Color.FromRgb(255, 99, 71)   },
        { "turquoise",            Color.FromRgb(64, 224, 208)  },
        { "violet",               Color.FromRgb(238, 130, 238) },
        { "wheat",                Color.FromRgb(245, 222, 179) },
        { "white",                Color.FromRgb(255, 255, 255) },
        { "whitesmoke",           Color.FromRgb(245, 245, 245) },
        { "yellow",               Color.FromRgb(255, 255, 0)   },
        { "yellowgreen",          Color.FromRgb(154, 205, 50)  },

        { "transparent",          Color.FromArgb(0, 0, 0, 0)   }
    };

    public static Color? FromCSSNamedColor(string name)
    {
        if (CSSColorNameMappings.TryGetValue(name, out Color color))
            return color;
        return null;
    }

    public static Color FromHsl(double h, double s, double l)
    {
        return FromHsla(h, s, l, 255);
    }
    public static Color FromHsla(double h, double s, double l, byte a)
    {
        (double rR, double rG, double rB) = HSL2RGB(h, s, l);
        return Color.FromArgb(a,
            ClampToByte((int)Math.Round(rR * 255)),
            ClampToByte((int)Math.Round(rG * 255)),
            ClampToByte((int)Math.Round(rB * 255)));
    }

    public static Color FromHwb(double h, double w, double b)
    {
        return FromHwba(h, w, b, 255);
    }
    public static Color FromHwba(double h, double w, double b, byte a)
    {
        double W = Math.Clamp(w, 0, 1);
        double B = Math.Clamp(b, 0, 1);
        if (W + B >= 1)
        {
            double G = W / (W + B) * 255;
            return Color.FromArgb(a,
                ClampToByte((int)Math.Round(G)),
                ClampToByte((int)Math.Round(G)),
                ClampToByte((int)Math.Round(G)));
        }
        (double rR, double rG, double rB) = HSL2RGB(h, 1, 0.5);
        double T = (1 - W - B);
        return Color.FromArgb(a,
            ClampToByte((int)Math.Round((rR * T + W) * 255)),
            ClampToByte((int)Math.Round((rG * T + W) * 255)),
            ClampToByte((int)Math.Round((rB * T + W) * 255)));
    }

    public static Color? FromHex(ReadOnlySpan<char> chars)
    {
        if (chars[0] == '#')
        {
            if (!uint.TryParse(chars[1..], NumberStyles.HexNumber, null, out uint result))
                return null;
            switch (chars.Length)
            {
                case 4:
                    {
                        uint ir = (result >> 8) & 0xF;
                        uint ig = (result >> 4) & 0xF;
                        uint ib = result & 0xF;
                        byte r = (byte)((ir << 4) | ir);
                        byte g = (byte)((ig << 4) | ig);
                        byte b = (byte)((ib << 4) | ib);
                        return Color.FromRgb(r, g, b);
                    }
                case 5:
                    {
                        uint ir = (result >> 12) & 0xF;
                        uint ig = (result >> 8) & 0xF;
                        uint ib = (result >> 4) & 0xF;
                        uint ia = result & 0xF;
                        byte a = (byte)((ia << 4) | ia);
                        byte r = (byte)((ir << 4) | ir);
                        byte g = (byte)((ig << 4) | ig);
                        byte b = (byte)((ib << 4) | ib);
                        return Color.FromArgb(a, r, g, b);
                    }
                case 7:
                    {
                        byte r = (byte)(result >> 16);
                        byte g = (byte)(result >> 8);
                        byte b = (byte)result;
                        return Color.FromRgb(r, g, b);
                    }
                case 9:
                    {
                        byte r = (byte)(result >> 24);
                        byte g = (byte)(result >> 16);
                        byte b = (byte)(result >> 8);
                        byte a = (byte)result;
                        return Color.FromArgb(a, r, g, b);
                    }
                default:
                    return null;
            }
        }
        return null;
    }

    public static Color? FromCSSValue(string str)
    {
        ReadOnlySpan<char> span = str.AsSpan().Trim(CSSWhitespaces);
        if (string.IsNullOrEmpty(str))
            return null;
        if (str[0] == '#') // Hex color
        {
            return FromHex(str);
        }
        else if (span.Length > 4)
        {
            if (span.StartsWith("rgb", StringComparison.OrdinalIgnoreCase)) // rgb
            {
                Color? result = ParseRGB(span);
                if (result is not null)
                    return result;
            }
            else if (span.StartsWith("hsl", StringComparison.OrdinalIgnoreCase)) // hsl
            {
                Color? result = ParseHSL(span);
                if (result is not null)
                    return result;
            }
            else if (span.StartsWith("hwb(", StringComparison.OrdinalIgnoreCase)) // hwb
            {
                Color? result = ParseHWB(span);
                if (result is not null)
                    return result;
            }
        }
        // Named color or transparent
        return FromCSSNamedColor(span.ToString());
    }

    private static (double R, double G, double B) HSL2RGB(double h, double s, double l)
    {
        double H = (double)h % 1;
        if (H < 0)
            H++;
        double S = Math.Clamp(s, 0, 1);
        double L = Math.Clamp(l, 0, 1);
        double C = (1 - Math.Abs(2 * L - 1)) * S;
        double H_1 = H * 6;
        double X = C * (1 - Math.Abs(H_1 % 2 - 1));
        double R_1 = 0, G_1 = 0, B_1 = 0;
        if (H_1 < 2)
        {
            if (H_1 < 1)
            {
                R_1 = C;
                G_1 = X;
            }
            else
            {
                R_1 = X;
                G_1 = C;
            }
        }
        else if (H_1 < 4)
        {
            if (H_1 < 3)
            {
                G_1 = C;
                B_1 = X;
            }
            else
            {
                G_1 = X;
                B_1 = C;
            }

        }
        else
        {
            if (H_1 < 5)
            {
                R_1 = X;
                B_1 = C;
            }
            else
            {
                R_1 = C;
                B_1 = X;
            }
        }
        double m = L - C * 0.5;
        return (R_1 + m, G_1 + m, B_1 + m);
    }

    private static CSSToken NextToken(ReadOnlySpan<char> chars, out int consumed)
    {
        int length = chars.Length;
        chars = chars.TrimStart(CSSWhitespaces);
        consumed = length - chars.Length;
        ReadOnlySpan<char> result = [];

        if (chars.Length == 0)
        {
            return new CSSToken { TokenType = CSSTokenType.None, Chars = chars };
        }
        char c = chars[0];
        if (c is '%')
        {
            consumed++;
            return new CSSToken { TokenType = CSSTokenType.Percentage, Chars = "%" };
        }
        if (c is '/')
        {
            consumed++;
            return new CSSToken { TokenType = CSSTokenType.Solidus, Chars = "/" };
        }
        if (c is ',')
        {
            consumed++;
            return new CSSToken { TokenType = CSSTokenType.Comma, Chars = "," };
        }
        if (c is ')')
        {
            consumed++;
            return new CSSToken { TokenType = CSSTokenType.RightBracket, Chars = "," };
        }

        if (c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'))
        {
            result = chars[..(chars.Length - chars.TrimStart('A', 'Z').TrimStart('a', 'z').Length)];
            consumed += result.Length;
            return new CSSToken { TokenType = CSSTokenType.Letter, Chars = result };
        }

        ReadOnlySpan<char> oldChars = chars;
        NumberParseStep step = NumberParseStep.Sign;
        if (c is '+' or '-')
        {
            chars = chars[1..];
            step = NumberParseStep.IntDigit;
        }
        if (chars.Length > 0 && chars[0] is >= '0' and <= '9' && step is >= NumberParseStep.Sign and <= NumberParseStep.IntDigit)
        {
            chars = chars.TrimStart('0', '9');
            step = NumberParseStep.DecDigit;

            result = oldChars[..(oldChars.Length - chars.Length)];
        }
        if (chars.Length > 0 && chars[0] is '.' && step is >= NumberParseStep.Sign and <= NumberParseStep.DecDigit)
        {
            chars = chars[1..];
            if (chars.Length > 0 && chars[0] is >= '0' and <= '9')
            {
                chars = chars.TrimStart('0', '9');
                step = NumberParseStep.Exp;

                result = oldChars[..(oldChars.Length - chars.Length)];
            }
        }
        if (step < NumberParseStep.DecDigit)
        {
            return new CSSToken { TokenType = CSSTokenType.Unknown, Chars = [] };
        }

        if (chars.Length > 0 && chars[0] is 'E' or 'e' && step is NumberParseStep.Exp)
        {
            chars = chars[1..];
            if (chars.Length > 0 && chars[0] is '+' or '-')
            {
                chars = chars[1..];
            }
            if (chars.Length > 0 && chars[0] is >= '0' and <= '9')
            {
                chars = chars.TrimStart('0', '9');

                result = oldChars[..(oldChars.Length - chars.Length)];
            }
        }

        consumed += result.Length;
        return new CSSToken { TokenType = CSSTokenType.Number, Chars = result };
    }

    private static Color? ParseRGB(ReadOnlySpan<char> chars)
    {
        // Assumes that the span starts with "rgb".
        ReadOnlySpan<char> body = chars[3..];

        if (body[0] == '(')
            body = body[1..];
        else if (body.StartsWith("a(", StringComparison.OrdinalIgnoreCase))
            body = body[2..];
        else
            return null;

        double r = 0, g = 0, b = 0, a = 255;
        LegacySyntaxParseStep legacy = LegacySyntaxParseStep.Value1;
        MordenSyntaxParseStep morden = MordenSyntaxParseStep.Value1;
        bool percentageR = false;
        bool percentageG = false;
        bool percentageB = false;
        bool percentageA = false;
        while (legacy != LegacySyntaxParseStep.AfterEnd && morden != MordenSyntaxParseStep.AfterEnd)
        {
            if (legacy == LegacySyntaxParseStep.Failed && morden == MordenSyntaxParseStep.Failed)
                return null;

            CSSToken token = NextToken(body, out int consumed);
            switch (token.TokenType)
            {
                case CSSTokenType.Letter when token.Chars.Equals("none", StringComparison.OrdinalIgnoreCase):
                    legacy = LegacySyntaxParseStep.Failed;
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Value1:
                            r = 0;
                            morden = MordenSyntaxParseStep.Value2;
                            break;
                        case MordenSyntaxParseStep.Suffix1:
                        case MordenSyntaxParseStep.Value2:
                            g = 0;
                            morden = MordenSyntaxParseStep.Value3;
                            break;
                        case MordenSyntaxParseStep.Suffix2:
                        case MordenSyntaxParseStep.Value3:
                            b = 0;
                            morden = MordenSyntaxParseStep.Solidus;
                            break;
                        case MordenSyntaxParseStep.A:
                            a = 0;
                            morden = MordenSyntaxParseStep.End;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Number:
                    double t = double.Parse(token.Chars);
                    switch (legacy)
                    {
                        case LegacySyntaxParseStep.Value1:
                            r = t;
                            legacy = LegacySyntaxParseStep.Suffix1;
                            break;
                        case LegacySyntaxParseStep.Value2:
                            g = t;
                            legacy = percentageR ?
                                LegacySyntaxParseStep.Suffix2 :
                                LegacySyntaxParseStep.Comma2;
                            break;
                        case LegacySyntaxParseStep.Value3:
                            b = t;
                            legacy = percentageR ?
                                LegacySyntaxParseStep.Suffix3 :
                                LegacySyntaxParseStep.Comma3;
                            break;
                        case LegacySyntaxParseStep.A:
                            a = t;
                            legacy = LegacySyntaxParseStep.SuffixA;
                            break;
                        default:
                            legacy = LegacySyntaxParseStep.Failed;
                            break;
                    }
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Value1:
                            r = t;
                            morden = MordenSyntaxParseStep.Suffix1;
                            break;
                        case MordenSyntaxParseStep.Suffix1:
                        case MordenSyntaxParseStep.Value2:
                            g = t;
                            morden = MordenSyntaxParseStep.Suffix2;
                            break;
                        case MordenSyntaxParseStep.Suffix2:
                        case MordenSyntaxParseStep.Value3:
                            b = t;
                            morden = MordenSyntaxParseStep.Suffix3;
                            break;
                        case MordenSyntaxParseStep.A:
                            a = t;
                            morden = MordenSyntaxParseStep.SuffixA;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Percentage:
                    switch (legacy)
                    {
                        case LegacySyntaxParseStep.Suffix1:
                            percentageR = true;
                            legacy = LegacySyntaxParseStep.Comma1;
                            break;
                        case LegacySyntaxParseStep.Suffix2:
                            percentageG = true;
                            legacy = LegacySyntaxParseStep.Comma2;
                            break;
                        case LegacySyntaxParseStep.Suffix3:
                            percentageB = true;
                            legacy = LegacySyntaxParseStep.Comma3;
                            break;
                        case LegacySyntaxParseStep.SuffixA:
                            percentageA = true;
                            legacy = LegacySyntaxParseStep.End;
                            break;
                        default:
                            legacy = LegacySyntaxParseStep.Failed;
                            break;
                    }
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Suffix1:
                            percentageR = true;
                            morden = MordenSyntaxParseStep.Value2;
                            break;
                        case MordenSyntaxParseStep.Suffix2:
                            percentageG = true;
                            morden = MordenSyntaxParseStep.Value3;
                            break;
                        case MordenSyntaxParseStep.Suffix3:
                            percentageB = true;
                            morden = MordenSyntaxParseStep.Solidus;
                            break;
                        case MordenSyntaxParseStep.SuffixA:
                            percentageA = true;
                            morden = MordenSyntaxParseStep.End;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Comma:
                    legacy = legacy switch
                    {
                        LegacySyntaxParseStep.Suffix1
                        or LegacySyntaxParseStep.Comma1
                            => LegacySyntaxParseStep.Value2,
                        LegacySyntaxParseStep.Suffix2
                        or LegacySyntaxParseStep.Comma2
                            => LegacySyntaxParseStep.Value3,
                        LegacySyntaxParseStep.Suffix3
                        or LegacySyntaxParseStep.Comma3
                            => LegacySyntaxParseStep.A,
                        _
                            => LegacySyntaxParseStep.Failed,
                    };
                    morden = MordenSyntaxParseStep.Failed;
                    break;
                case CSSTokenType.Solidus:
                    legacy = LegacySyntaxParseStep.Failed;
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                            => MordenSyntaxParseStep.A,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                case CSSTokenType.RightBracket:
                case CSSTokenType.None:
                    switch (legacy)
                    {
                        case LegacySyntaxParseStep.Suffix3 when !percentageR:
                        case LegacySyntaxParseStep.Comma3:
                        case LegacySyntaxParseStep.SuffixA:
                        case LegacySyntaxParseStep.End:
                            legacy = LegacySyntaxParseStep.AfterEnd;
                            break;
                        default:
                            legacy = LegacySyntaxParseStep.Failed;
                            break;
                    }
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                        or MordenSyntaxParseStep.SuffixA
                        or MordenSyntaxParseStep.End
                            => MordenSyntaxParseStep.AfterEnd,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                default:
                    legacy = LegacySyntaxParseStep.Failed;
                    morden = MordenSyntaxParseStep.Failed;
                    break;
            }
            body = body[consumed..];
        }

        if (NextToken(body, out _).TokenType != CSSTokenType.None)
            return null;

        if (percentageR)
            r *= 2.55;
        if (percentageG)
            g *= 2.55;
        if (percentageB)
            b *= 2.55;
        if (percentageA)
            a *= 2.55;

        return Color.FromArgb(
            ClampToByte((int)Math.Round(a)),
            ClampToByte((int)Math.Round(r)),
            ClampToByte((int)Math.Round(g)),
            ClampToByte((int)Math.Round(b)));
    }

    private static Color? ParseHSL(ReadOnlySpan<char> chars)
    {
        // Assumes that the span starts with "hsl".
        ReadOnlySpan<char> body = chars[3..];

        if (body[0] == '(')
            body = body[1..];
        else if (body.StartsWith("a(", StringComparison.OrdinalIgnoreCase))
            body = body[2..];
        else
            return null;

        double h = 0, s = 0, l = 0, a = 255;
        LegacySyntaxParseStep legacy = LegacySyntaxParseStep.Value1;
        MordenSyntaxParseStep morden = MordenSyntaxParseStep.Value1;
        double multiplierH = DegMultiplier;
        bool percentageA = false;
        while (legacy != LegacySyntaxParseStep.AfterEnd && morden != MordenSyntaxParseStep.AfterEnd)
        {
            if (legacy == LegacySyntaxParseStep.Failed && morden == MordenSyntaxParseStep.Failed)
                return null;

            CSSToken token = NextToken(body, out int consumed);
            switch (token.TokenType)
            {
                case CSSTokenType.Letter:
                    if (token.Chars.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        legacy = LegacySyntaxParseStep.Failed;
                        switch (morden)
                        {
                            case MordenSyntaxParseStep.Value1:
                                h = 0;
                                morden = MordenSyntaxParseStep.Value2;
                                break;
                            case MordenSyntaxParseStep.Suffix1:
                            case MordenSyntaxParseStep.Value2:
                                s = 0;
                                morden = MordenSyntaxParseStep.Value3;
                                break;
                            case MordenSyntaxParseStep.Suffix2:
                            case MordenSyntaxParseStep.Value3:
                                l = 0;
                                morden = MordenSyntaxParseStep.Solidus;
                                break;
                            case MordenSyntaxParseStep.A:
                                a = 0;
                                morden = MordenSyntaxParseStep.End;
                                break;
                            default:
                                morden = MordenSyntaxParseStep.Failed;
                                break;
                        }
                    }
                    else
                    {
                        if (token.Chars.Equals("deg", StringComparison.OrdinalIgnoreCase))
                            multiplierH = DegMultiplier;
                        else if (token.Chars.Equals("grad", StringComparison.OrdinalIgnoreCase))
                            multiplierH = GradMultiplier;
                        else if (token.Chars.Equals("rad", StringComparison.OrdinalIgnoreCase))
                            multiplierH = RadMultiplier;
                        else if (token.Chars.Equals("turn", StringComparison.OrdinalIgnoreCase))
                            multiplierH = TurnMultiplier;
                        else
                        {
                            legacy = LegacySyntaxParseStep.Failed;
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                        }
                        legacy = legacy switch
                        {
                            LegacySyntaxParseStep.Suffix1
                                => LegacySyntaxParseStep.Comma1,
                            _
                                => LegacySyntaxParseStep.Failed,
                        };
                        morden = morden switch
                        {
                            MordenSyntaxParseStep.Suffix1
                                => MordenSyntaxParseStep.Value2,
                            _
                                => MordenSyntaxParseStep.Failed,
                        };
                    }
                    break;
                case CSSTokenType.Number:
                    double t = double.Parse(token.Chars);
                    switch (legacy)
                    {
                        case LegacySyntaxParseStep.Value1:
                            h = t;
                            legacy = LegacySyntaxParseStep.Suffix1;
                            break;
                        case LegacySyntaxParseStep.Value2:
                            s = t;
                            legacy = LegacySyntaxParseStep.Suffix2;
                            break;
                        case LegacySyntaxParseStep.Value3:
                            l = t;
                            legacy = LegacySyntaxParseStep.Suffix3;
                            break;
                        case LegacySyntaxParseStep.A:
                            a = t;
                            legacy = LegacySyntaxParseStep.SuffixA;
                            break;
                        default:
                            legacy = LegacySyntaxParseStep.Failed;
                            break;
                    }
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Value1:
                            h = t;
                            morden = MordenSyntaxParseStep.Suffix1;
                            break;
                        case MordenSyntaxParseStep.Suffix1:
                        case MordenSyntaxParseStep.Value2:
                            s = t;
                            morden = MordenSyntaxParseStep.Suffix2;
                            break;
                        case MordenSyntaxParseStep.Suffix2:
                        case MordenSyntaxParseStep.Value3:
                            l = t;
                            morden = MordenSyntaxParseStep.Suffix3;
                            break;
                        case MordenSyntaxParseStep.A:
                            a = t;
                            morden = MordenSyntaxParseStep.SuffixA;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Percentage:
                    switch (legacy)
                    {
                        case LegacySyntaxParseStep.Suffix2:
                            legacy = LegacySyntaxParseStep.Comma2;
                            break;
                        case LegacySyntaxParseStep.Suffix3:
                            legacy = LegacySyntaxParseStep.Comma3;
                            break;
                        case LegacySyntaxParseStep.SuffixA:
                            percentageA = true;
                            legacy = LegacySyntaxParseStep.End;
                            break;
                        default:
                            legacy = LegacySyntaxParseStep.Failed;
                            break;
                    }
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Suffix2:
                            morden = MordenSyntaxParseStep.Value3;
                            break;
                        case MordenSyntaxParseStep.Suffix3:
                            morden = MordenSyntaxParseStep.Solidus;
                            break;
                        case MordenSyntaxParseStep.SuffixA:
                            percentageA = true;
                            morden = MordenSyntaxParseStep.End;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Comma:
                    legacy = legacy switch
                    {
                        LegacySyntaxParseStep.Suffix1
                        or LegacySyntaxParseStep.Comma1
                            => LegacySyntaxParseStep.Value2,
                        LegacySyntaxParseStep.Comma2
                            => LegacySyntaxParseStep.Value3,
                        LegacySyntaxParseStep.Comma3
                            => LegacySyntaxParseStep.A,
                        _
                            => LegacySyntaxParseStep.Failed,
                    };
                    morden = MordenSyntaxParseStep.Failed;
                    break;
                case CSSTokenType.Solidus:
                    legacy = LegacySyntaxParseStep.Failed;
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                            => MordenSyntaxParseStep.A,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                case CSSTokenType.RightBracket:
                case CSSTokenType.None:
                    legacy = legacy switch
                    {
                        LegacySyntaxParseStep.Comma3
                        or LegacySyntaxParseStep.SuffixA
                        or LegacySyntaxParseStep.End
                            => LegacySyntaxParseStep.AfterEnd,
                        _
                            => LegacySyntaxParseStep.Failed,
                    };
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                        or MordenSyntaxParseStep.SuffixA
                        or MordenSyntaxParseStep.End
                            => MordenSyntaxParseStep.AfterEnd,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                default:
                    legacy = LegacySyntaxParseStep.Failed;
                    morden = MordenSyntaxParseStep.Failed;
                    break;
            }
            body = body[consumed..];
        }

        if (NextToken(body, out _).TokenType != CSSTokenType.None)
            return null;

        if (percentageA)
            a *= 2.55f;

        return FromHsla(
            h * multiplierH,
            s * 0.01,
            l * 0.01,
            ClampToByte((int)Math.Round(a)));
    }

    private static Color? ParseHWB(ReadOnlySpan<char> chars)
    {
        // Assumes that the span starts with "hwb(".
        ReadOnlySpan<char> body = chars[4..];

        double h = 0, w = 0, b = 0, a = 255;
        MordenSyntaxParseStep morden = MordenSyntaxParseStep.Value1; // HWB does not have legacy format
        double multiplierH = DegMultiplier;
        bool percentageA = false;
        while (morden != MordenSyntaxParseStep.AfterEnd)
        {
            if (morden == MordenSyntaxParseStep.Failed)
                return null;

            CSSToken token = NextToken(body, out int consumed);
            switch (token.TokenType)
            {
                case CSSTokenType.Letter:
                    if (token.Chars.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (morden)
                        {
                            case MordenSyntaxParseStep.Value1:
                                h = 0;
                                morden = MordenSyntaxParseStep.Value2;
                                break;
                            case MordenSyntaxParseStep.Suffix1:
                            case MordenSyntaxParseStep.Value2:
                                w = 0;
                                morden = MordenSyntaxParseStep.Value3;
                                break;
                            case MordenSyntaxParseStep.Suffix2:
                            case MordenSyntaxParseStep.Value3:
                                b = 0;
                                morden = MordenSyntaxParseStep.Solidus;
                                break;
                            case MordenSyntaxParseStep.A:
                                a = 0;
                                morden = MordenSyntaxParseStep.End;
                                break;
                            default:
                                morden = MordenSyntaxParseStep.Failed;
                                break;
                        }
                    }
                    else
                    {
                        if (token.Chars.Equals("deg", StringComparison.OrdinalIgnoreCase))
                            multiplierH = DegMultiplier;
                        else if (token.Chars.Equals("grad", StringComparison.OrdinalIgnoreCase))
                            multiplierH = GradMultiplier;
                        else if (token.Chars.Equals("rad", StringComparison.OrdinalIgnoreCase))
                            multiplierH = RadMultiplier;
                        else if (token.Chars.Equals("turn", StringComparison.OrdinalIgnoreCase))
                            multiplierH = TurnMultiplier;
                        else
                        {
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                        }
                        morden = morden switch
                        {
                            MordenSyntaxParseStep.Suffix1
                                => MordenSyntaxParseStep.Value2,
                            _
                                => MordenSyntaxParseStep.Failed,
                        };
                    }
                    break;
                case CSSTokenType.Number:
                    double t = double.Parse(token.Chars);
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Value1:
                            h = t;
                            morden = MordenSyntaxParseStep.Suffix1;
                            break;
                        case MordenSyntaxParseStep.Suffix1:
                        case MordenSyntaxParseStep.Value2:
                            w = t;
                            morden = MordenSyntaxParseStep.Suffix2;
                            break;
                        case MordenSyntaxParseStep.Suffix2:
                        case MordenSyntaxParseStep.Value3:
                            b = t;
                            morden = MordenSyntaxParseStep.Suffix3;
                            break;
                        case MordenSyntaxParseStep.A:
                            a = t;
                            morden = MordenSyntaxParseStep.SuffixA;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Percentage:
                    switch (morden)
                    {
                        case MordenSyntaxParseStep.Suffix2:
                            morden = MordenSyntaxParseStep.Value3;
                            break;
                        case MordenSyntaxParseStep.Suffix3:
                            morden = MordenSyntaxParseStep.Solidus;
                            break;
                        case MordenSyntaxParseStep.SuffixA:
                            percentageA = true;
                            morden = MordenSyntaxParseStep.End;
                            break;
                        default:
                            morden = MordenSyntaxParseStep.Failed;
                            break;
                    }
                    break;
                case CSSTokenType.Solidus:
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                            => MordenSyntaxParseStep.A,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                case CSSTokenType.RightBracket:
                case CSSTokenType.None:
                    morden = morden switch
                    {
                        MordenSyntaxParseStep.Suffix3
                        or MordenSyntaxParseStep.Solidus
                        or MordenSyntaxParseStep.SuffixA
                        or MordenSyntaxParseStep.End
                            => MordenSyntaxParseStep.AfterEnd,
                        _
                            => MordenSyntaxParseStep.Failed,
                    };
                    break;
                default:
                    morden = MordenSyntaxParseStep.Failed;
                    break;
            }
            body = body[consumed..];
        }

        if (NextToken(body, out _).TokenType != CSSTokenType.None)
            return null;

        if (percentageA)
            a *= 2.55f;

        return FromHwba(
            h * multiplierH,
            w * 0.01,
            b * 0.01,
            ClampToByte((int)Math.Round(a)));
    }

    private static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
    {
        int i = span.IndexOfAnyExceptInRange(lowInclusive, highInclusive);
        if (i == -1)
            return [];
        else
            return span[i..];
    }

    private static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, SearchValues<char> values)
    {
        int i = span.IndexOfAnyExcept(values);
        if (i == -1)
            return [];
        else
            return span[i..];
    }

    private static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, SearchValues<char> values)
    {
        int i = span.LastIndexOfAnyExcept(values);
        if (i == -1)
            return span;
        else
            return span[..(i + 1)];
    }

    private static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, SearchValues<char> values)
    {
        return span.TrimStart(values).TrimEnd(values);
    }

    private static byte ClampToByte(int value)
    {
        if (value < byte.MinValue)
            return byte.MinValue;
        if (value > byte.MaxValue)
            return byte.MaxValue;
        return (byte)value;
    }

    private ref struct CSSToken
    {
        public CSSTokenType TokenType;
        public ReadOnlySpan<char> Chars;
    }
    /// <summary>
    /// CSS tokens. This is different from the W3C standard.
    /// </summary>
    private enum CSSTokenType
    {
        None,
        Letter,
        Number,
        Percentage,
        Comma,
        Solidus,
        RightBracket,
        Unknown
    }
    private enum NumberParseStep
    {
        Sign,
        IntDigit,
        DecDigit,
        Exp
    }
    private enum LegacySyntaxParseStep
    {
        Failed = -1,
        Value1,
        Suffix1,
        Comma1,
        Value2,
        Suffix2,
        Comma2,
        Value3,
        Suffix3,
        Comma3,
        A,
        SuffixA,
        End,
        AfterEnd
    }
    private enum MordenSyntaxParseStep
    {
        Failed = -1,
        Value1,
        Suffix1,
        Value2,
        Suffix2,
        Value3,
        Suffix3,
        Solidus,
        A,
        SuffixA,
        End,
        AfterEnd
    }
}
