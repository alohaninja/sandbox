#r "DemoLib.dll"

using System.Net;
using System.IO;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request.");

    // parse query parameter
    string documentURI = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "documentURI", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    documentURI = documentURI ?? data?.documentURI;

    try
    {
        string baseToolPath = @"D:\home\site\wwwroot\DocConverter\bin\wkhtmltopdf.exe";
        log.Info($"Converting HTML to PDF for '{documentURI}'");
        var logger = new DemoLib.Class1.AzureFunctionLogger(log);
        byte[] pdfResult = DemoLib.Class1.ConvertFromHtmlBlobUrl(documentURI, logger, baseToolPath);
        log.Info($"FileSize: {pdfResult.Length}");
        
        System.IO.File.WriteAllBytes(@"D:\home\site\wwwroot\DocConverter\bin\temp.pdf", pdfResult);
        //PDFConverter.Convert(documentURI);
        //byte[] pdfResult = PDFConverter.ConvertFromHtmlUrl(documentURI);
        //log.Info($"FileSize: {pdfResult.Length()}");
        log.Info($"Conversion completed for '{documentURI}'");
    } 
    catch (Exception ex)
    {
        log.Info($"{ex}");
    }


    return documentURI == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a documentURI on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, $"Converting document: {documentURI}");
}