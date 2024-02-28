using System.Net;
using HtmlAgilityPack;

namespace CasaRutterCards.Utils;

public static class HtmlDocumentUtils
{
    public static async Task<HtmlDocument> GetHtmlDoc(string url)
    {
        var client = new HttpClient();

        var response = await client.GetAsync(url);

        var html = await response.Content.ReadAsStringAsync();

        var htmlDoc = new HtmlDocument();

        htmlDoc.LoadHtml(WebUtility.HtmlDecode(html));
        return htmlDoc;
    }
}