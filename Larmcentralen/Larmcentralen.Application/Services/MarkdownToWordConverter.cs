using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Larmcentralen.Application.Services;

public class MarkdownToWordConverter
{
    public byte[] Convert(string markdown, string title)
    {
        using var stream = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();

            // Add title
            body.Append(CreateHeading(title, 1));

            // Parse markdown
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var document = Markdown.Parse(markdown, pipeline);

            foreach (var block in document)
            {
                ProcessBlock(block, body);
            }

            mainPart.Document.Append(body);
        }

        return stream.ToArray();
    }

    private void ProcessBlock(Block block, Body body)
    {
        switch (block)
        {
            case HeadingBlock heading:
                var headingText = GetInlineText(heading.Inline);
                body.Append(CreateHeading(headingText, heading.Level));
                break;

            case ParagraphBlock paragraph:
                var para = new Paragraph();
                ProcessInlines(paragraph.Inline, para);
                body.Append(para);
                break;

            case ListBlock list:
                ProcessList(list, body);
                break;

            case ThematicBreakBlock:
                body.Append(CreateHorizontalRule());
                break;

            case QuoteBlock quote:
                foreach (var child in quote)
                    ProcessBlock(child, body);
                break;

            case FencedCodeBlock code:
                var codeText = code.Lines.ToString().TrimEnd();
                body.Append(CreateCodeBlock(codeText));
                break;

            case Markdig.Extensions.Tables.Table table:
                ProcessTable(table, body);
                break;
        }
    }

    private void ProcessInlines(ContainerInline? inlines, Paragraph para)
    {
        if (inlines is null) return;

        foreach (var inline in inlines)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    para.Append(CreateRun(literal.Content.ToString(), false, false));
                    break;

                case EmphasisInline emphasis:
                    var isBold = emphasis.DelimiterCount == 2;
                    var isItalic = emphasis.DelimiterCount == 1;
                    var emphText = GetInlineText(emphasis);
                    para.Append(CreateRun(emphText, isBold, isItalic));
                    break;

                case CodeInline code:
                    para.Append(CreateCodeRun(code.Content));
                    break;

                case LineBreakInline:
                    para.Append(new Run(new Break()));
                    break;

                case LinkInline link:
                    var linkText = GetInlineText(link);
                    para.Append(CreateRun(linkText, false, false));
                    break;
            }
        }
    }

    private void ProcessList(ListBlock list, Body body)
    {
        int itemNumber = 1;
        foreach (var item in list)
        {
            if (item is ListItemBlock listItem)
            {
                foreach (var child in listItem)
                {
                    if (child is ParagraphBlock paragraph)
                    {
                        var para = new Paragraph();
                        var prefix = list.IsOrdered ? $"{itemNumber}. " : "• ";
                        para.Append(CreateRun(prefix, false, false));
                        ProcessInlines(paragraph.Inline, para);

                        // Indent
                        para.ParagraphProperties = new ParagraphProperties(
                            new Indentation { Left = "720" });

                        body.Append(para);
                    }
                }
                itemNumber++;
            }
        }
    }

    private void ProcessTable(Markdig.Extensions.Tables.Table table, Body body)
    {
        var wordTable = new Table();

        var tableProperties = new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            ),
            new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }
        );
        wordTable.Append(tableProperties);

        bool isHeader = true;
        foreach (var row in table)
        {
            if (row is Markdig.Extensions.Tables.TableRow tableRow)
            {
                var wordRow = new TableRow();

                foreach (var cell in tableRow)
                {
                    if (cell is Markdig.Extensions.Tables.TableCell tableCell)
                    {
                        var cellText = "";
                        foreach (var cellBlock in tableCell)
                        {
                            if (cellBlock is ParagraphBlock p)
                                cellText = GetInlineText(p.Inline);
                        }

                        var wordCell = new TableCell();
                        var para = new Paragraph();

                        if (isHeader)
                            para.Append(CreateRun(cellText, true, false));
                        else
                            para.Append(CreateRun(cellText, false, false));

                        wordCell.Append(para);
                        wordRow.Append(wordCell);
                    }
                }

                wordTable.Append(wordRow);
                if (isHeader) isHeader = false;
            }
        }

        body.Append(wordTable);
        body.Append(new Paragraph()); // spacing after table
    }

    private static string GetInlineText(ContainerInline? inlines)
    {
        if (inlines is null) return "";
        var text = "";
        foreach (var inline in inlines)
        {
            text += inline switch
            {
                LiteralInline literal => literal.Content.ToString(),
                EmphasisInline emphasis => GetInlineText(emphasis),
                CodeInline code => code.Content,
                _ => ""
            };
        }
        return text;
    }

    private static Paragraph CreateHeading(string text, int level)
    {
        var fontSize = level switch
        {
            1 => "48",
            2 => "36",
            3 => "28",
            _ => "24"
        };

        var run = new Run();
        run.RunProperties = new RunProperties(
            new Bold(),
            new FontSize { Val = fontSize }
        );
        run.Append(new Text(text));

        var para = new Paragraph();
        para.ParagraphProperties = new ParagraphProperties(
            new SpacingBetweenLines { Before = "240", After = "120" }
        );
        para.Append(run);
        return para;
    }

    private static Run CreateRun(string text, bool bold, bool italic)
    {
        var run = new Run();
        var props = new RunProperties();
        if (bold) props.Append(new Bold());
        if (italic) props.Append(new Italic());
        run.RunProperties = props;
        run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        return run;
    }

    private static Run CreateCodeRun(string text)
    {
        var run = new Run();
        run.RunProperties = new RunProperties(
            new RunFonts { Ascii = "Consolas" },
            new FontSize { Val = "20" },
            new Shading { Fill = "F1F5F9", Val = ShadingPatternValues.Clear }
        );
        run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        return run;
    }

    private static Paragraph CreateCodeBlock(string text)
    {
        var para = new Paragraph();
        para.ParagraphProperties = new ParagraphProperties(
            new Shading { Fill = "F1F5F9", Val = ShadingPatternValues.Clear },
            new Indentation { Left = "360" }
        );

        foreach (var line in text.Split('\n'))
        {
            var run = new Run();
            run.RunProperties = new RunProperties(
                new RunFonts { Ascii = "Consolas" },
                new FontSize { Val = "20" }
            );
            run.Append(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
            para.Append(run);
            para.Append(new Run(new Break()));
        }

        return para;
    }

    private static Paragraph CreateHorizontalRule()
    {
        var para = new Paragraph();
        para.ParagraphProperties = new ParagraphProperties(
            new ParagraphBorders(
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "E2E8F0" }
            ),
            new SpacingBetweenLines { Before = "240", After = "240" }
        );
        return para;
    }
}