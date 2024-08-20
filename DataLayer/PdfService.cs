//using IronPdf;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using UPC_DropDown.Models;

//public class PdfService
//{
//    private readonly ICompositeViewEngine _viewEngine;
//    private readonly ITempDataProvider _tempDataProvider;
//    private readonly IServiceProvider _serviceProvider;


//    public PdfService(
//        ICompositeViewEngine viewEngine,
//        ITempDataProvider tempDataProvider,
//        IServiceProvider serviceProvider)
//    {
//        _viewEngine = viewEngine;
//        _tempDataProvider = tempDataProvider;
//        _serviceProvider = serviceProvider;


//        IronPdf.License.LicenseKey = "IRONPDF-MYLICENSE-KEY-1EF01";
//    }

//    public async Task<byte[]> GenerateOrderPdfAsync(List<Order> orders, string viewName, ControllerContext controllerContext)
//    {
//        // Convert the Razor view to HTML
//        var html = await ConvertRazorViewToHtml(orders, viewName, controllerContext);

//        // Create a new instance of ChromePdfRenderer
//        var renderer = new ChromePdfRenderer();

//        // Render the HTML as a PDF
//        var pdf = renderer.RenderHtmlAsPdf(html);

//        // Save the PDF to a file or return the byte array
//        var filePath = $"invoice-{orders.FirstOrDefault()?.OrderId}.pdf";
//        pdf.SaveAs(filePath);

//        return pdf.BinaryData;
//    }

//    private async Task<string> ConvertRazorViewToHtml(List<Order> orders, string viewName, ControllerContext controllerContext)
//    {
//        using (var writer = new StringWriter())
//        {
//            var viewResult = _viewEngine.FindView(controllerContext, viewName, false);
//            if (viewResult.View == null)
//            {
//                throw new FileNotFoundException($"View {viewName} cannot be found.");
//            }

//            var viewData = new ViewDataDictionary<List<Order>>(
//                metadataProvider: new EmptyModelMetadataProvider(),
//                modelState: new ModelStateDictionary())
//            {
//                Model = orders
//            };

//            var tempData = new TempDataDictionary(
//                controllerContext.HttpContext,
//                _tempDataProvider);

//            var viewContext = new ViewContext(
//                controllerContext,
//                viewResult.View,
//                viewData,
//                tempData,
//                writer,
//                new HtmlHelperOptions());

//            await viewResult.View.RenderAsync(viewContext);

//            return writer.ToString();
//        }
//    }
//}
