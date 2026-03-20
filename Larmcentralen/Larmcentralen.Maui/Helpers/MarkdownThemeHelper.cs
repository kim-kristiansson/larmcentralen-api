using Indiko.Maui.Controls.Markdown;
using Indiko.Maui.Controls.Markdown.Theming;

namespace Larmcentralen.Maui.Helpers;

public static class MarkdownThemeHelper
{
    public static MarkdownTheme Create()
    {
        return new MarkdownTheme
        {
            Palette =
            {
                TextPrimary = Color.FromArgb("#1E293B"),
                H1Color = Color.FromArgb("#1E293B"),
                H2Color = Color.FromArgb("#334155"),
                H3Color = Color.FromArgb("#475569"),
                HyperlinkColor = Color.FromArgb("#C41E2A"),
                CodeBlockBackground = Color.FromArgb("#F1F5F9"),
                CodeBlockText = Color.FromArgb("#334155"),
                CodeBlockBorder = Color.FromArgb("#E2E8F0"),
                BlockQuoteBackground = Color.FromArgb("#FEF3C7"),
                BlockQuoteBorder = Color.FromArgb("#D97706"),
                BlockQuoteText = Color.FromArgb("#92400E"),
                TableHeaderBackground = Color.FromArgb("#F1F5F9"),
                TableHeaderText = Color.FromArgb("#1E293B"),
                TableBorder = Color.FromArgb("#E2E8F0"),
                DividerColor = Color.FromArgb("#E2E8F0")
            },
            Typography =
            {
                H1FontSize = 26,
                H2FontSize = 22,
                H3FontSize = 18,
                BodyFontSize = 15,
                CodeFontSize = 13,
                LineHeight = 1.6,
                ParagraphSpacing = 1.2,
                ListItemSpacing = 6,
                ListIndent = 24
            }
        };
    }
}