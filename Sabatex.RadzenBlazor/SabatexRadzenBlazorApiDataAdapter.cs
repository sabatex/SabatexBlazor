using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor.Rendering;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using static Sabatex.Core.LocalizerHelper;


namespace Sabatex.RadzenBlazor;

public class SabatexRadzenBlazorApiDataAdapter<TKey> : ISabatexRadzenBlazorDataAdapter<TKey>
{
    const string nullResponce = "The responece return null";

    protected readonly HttpClient httpClient;
    protected readonly Uri baseUri;
    private readonly ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger;
    private readonly NavigationManager navigationManager;
    

    public SabatexRadzenBlazorApiDataAdapter(HttpClient httpClient, ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger, NavigationManager navigationManager)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        baseUri = new Uri(this.httpClient.BaseAddress ?? new Uri(navigationManager.BaseUri), "api/");
        this.logger = logger;
    }
    public async Task<QueryResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null,string? apply = null) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}");
        //uri = GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count,apply);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception($"Помилка запиту {response.StatusCode}");
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<QueryResult<TItem>>(response);

    }
    public async Task<QueryResult<TItem>> GetAsync<TItem>(QueryParams queryParams) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}?json={System.Text.Json.JsonSerializer.Serialize(queryParams)}");
        var response = await httpClient.GetAsync(uri);
        if (response.IsSuccessStatusCode)
            return await ReadAsync<QueryResult<TItem>>(response);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new Exception(Localize<SabatexRadzenBlazorApiDataAdapter<TKey>>("Access denied!"));

        throw new Exception($"Помилка запиту {response.StatusCode}");
            
    }
    public async Task<SabatexValidationModel<TItem>> PostAsync<TItem>(TItem? item) where TItem : class, IEntityBase<TKey>
    {
        if (item == null) throw new ArgumentNullException("item");

        var uri = new Uri(baseUri, typeof(TItem).Name);
        var response = await httpClient.PostAsJsonAsync<TItem>(uri, item);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TItem>();
            if (result == null)
                throw new DeserializeException();
            return new SabatexValidationModel<TItem>(result);
        }
                    
        var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>() ?? new Dictionary<string, List<string>>();
        
        if (response.StatusCode == HttpStatusCode.BadRequest &&   errors.Any())
        {
            return new SabatexValidationModel<TItem>(null,errors);
        }
        throw new Exception($"Error Post with status code: {response.StatusCode}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task DeleteAsync<TItem>(TKey id) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{id}"); ;
        var responce = await httpClient.DeleteAsync(uri); 
        if (responce == null)
            throw new Exception(nullResponce);

        if (responce.StatusCode != System.Net.HttpStatusCode.NoContent)
            throw new Exception($"Delete error with responce code = {responce.StatusCode}");
    }
    public async Task<SabatexValidationModel<TItem>> UpdateAsync<TItem>(TItem item) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{item.Id}");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsJsonAsync(uri,item);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TItem>();
            if (result == null)
                throw new DeserializeException();
            return new SabatexValidationModel<TItem>(result);
        }

        var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>() ?? new Dictionary<string, List<string>>();

        if (response.StatusCode == HttpStatusCode.BadRequest && errors.Any())
        {
            return new SabatexValidationModel<TItem>(null, errors);
        }
        throw new Exception($"Error Post with status code: {response.StatusCode}");
    }

    public async Task<TItem?> GetByIdAsync<TItem>(TKey id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        if (id is not null)
            return await GetByIdAsync<TItem>(id.ToString(),expand);
        else
            throw new ArgumentNullException(nameof(id));
    }

    public async Task<TItem?> GetByIdAsync<TItem>(string? id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{id}");
        //uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);

    }

    async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
            using Stream stream = await response.Content.ReadAsStreamAsync();
            return (stream.Length <= 0) ? default(T) : (await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            }));
        }
        catch
        {
            string text = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(text))
            {
                if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    JsonDocument jsonDocument;
                    try
                    {
                        jsonDocument = JsonDocument.Parse(text);
                    }
                    catch
                    {
                        throw new Exception("Unable to parse the response.");
                    }

                    if (jsonDocument.RootElement.TryGetProperty("error", out var value) && value.TryGetProperty("message", out var value2))
                    {
                        throw new Exception(value2.GetString());
                    }
                }
                else
                {
                    XElement xElement2;
                    try
                    {
                        XDocument xDocument = XDocument.Parse(text);
                        XElement xElement = xDocument.Descendants().SingleOrDefault((XElement p) => p.Name.LocalName == "internalexception");
                        xElement2 = ((xElement == null) ? xDocument.Descendants().SingleOrDefault((XElement p) => p.Name.LocalName == "error") : xElement);
                    }
                    catch
                    {
                        throw new Exception("Unable to parse the response.");
                    }

                    if (xElement2 != null)
                    {
                        throw new Exception(xElement2.Value);
                    }
                }
            }

            throw;
        }
    }


}
