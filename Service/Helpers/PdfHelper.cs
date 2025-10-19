using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

namespace Service.Helpers
{
    public static class PdfHelper
    {
        private static readonly SynchronizedConverter Converter = new(new PdfTools());

        public static byte[] RenderHtmlToPdf(string html, string? headerText = null)
        {

            string libFile;
            if (OperatingSystem.IsWindows())
                libFile = "libwkhtmltox.dll";
            else if (OperatingSystem.IsLinux())
                libFile = "libwkhtmltox.so";
            else
                libFile = "libwkhtmltox.dylib";

            var libPath = Path.Combine(AppContext.BaseDirectory, "Libs", libFile);

            Console.WriteLine($"[PdfHelper] OS: {(OperatingSystem.IsWindows() ? "Windows" : OperatingSystem.IsLinux() ? "Linux" : "macOS")}");
            Console.WriteLine($"[PdfHelper] BaseDirectory: {AppContext.BaseDirectory}");
            Console.WriteLine($"[PdfHelper] Looking for native lib: {libPath}");

            if (File.Exists(libPath))
            {
                try
                {
                    Console.WriteLine($"[PdfHelper] ✅ Loading native library from: {libPath}");
                    var context = new CustomAssemblyLoadContext();
                    context.LoadUnmanagedLibrary(libPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PdfHelper] ❌ Error loading libwkhtmltox: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine($"[PdfHelper] ❌ Native lib not found: {libPath}");
                Console.WriteLine($"[PdfHelper] ⚠️ Please make sure 'Service/Libs/{libFile}' is copied to output folder or Docker image.");
            }

            string style = @"
<style>
  body {
    font-family: 'DejaVu Sans', Arial, sans-serif;
    font-size: 12px;
    line-height: 1.5;
  }
  h2, h3, h4 {
    text-align: center;
    margin-bottom: 8px;
  }
  table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 10px;
    page-break-inside: avoid;
  }
  th, td {
    border: 1px solid #000;
    padding: 6px;
    vertical-align: top;
    word-wrap: break-word;
  }
  thead {
    display: table-header-group;
  }
  tfoot {
    display: table-row-group;
  }
  em {
    font-style: italic;
  }
  small {
    font-size: 11px;
  }
</style>";
            string fullHtml = $"<!DOCTYPE html><html><head>{style}</head><body>{html}</body></html>";

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 15, Right = 15 },
                DocumentTitle = "E-Contract",
                Out = null
            };

            var objectSettings = new ObjectSettings
            {
                HtmlContent = fullHtml,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8",
                    EnableIntelligentShrinking = false, 
                    PrintMediaType = true,
                    LoadImages = true,
                    MinimumFontSize = 10
                },
                HeaderSettings = new HeaderSettings
                {
                    FontSize = 9,
                    FontName = "Arial",
                    Line = true,
                    Right = "Trang [page] / [toPage]",
                    Left = headerText ?? "EV Co-ownership Contract",
                    Spacing = 3
                },
                FooterSettings = new FooterSettings
                {
                    FontSize = 8,
                    Line = true,
                    Center = "© EV Co-ownership Platform",
                    Spacing = 2
                }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return Converter.Convert(pdf);
        }

    }
}
