using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
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
using System.Threading.Tasks;
using System.Web;


namespace Sabatex.RadzenBlazor;

public class SabatexRadzenBlazorODataAdapter<TKey> : ISabatexRadzenBlazorDataAdapter<TKey>
{
    const string nullResponce = "The responece return null";

    private readonly HttpClient httpClient;
    private readonly Uri baseUri;
    private readonly ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger;
    private readonly NavigationManager navigationManager;

    public static Uri GetODataUri2(Uri uri, string? filter = null, int? top = null, int? skip = null, string? orderby = null, string? expand = null, string? select = null, bool? count = null,string? apply=null)
    {
        UriBuilder uriBuilder = new UriBuilder(uri);
        NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (!string.IsNullOrEmpty(filter))
        {
            nameValueCollection["$filter"] = filter.Replace("\"", "'") ?? "";
        }

        if (top.HasValue)
        {
            nameValueCollection["$top"] = $"{top}";
        }

        if (skip.HasValue)
        {
            nameValueCollection["$skip"] = $"{skip}";
        }

        if (!string.IsNullOrEmpty(orderby))
        {
            nameValueCollection["$orderby"] = orderby ?? "";
        }

        if (!string.IsNullOrEmpty(expand))
        {
            nameValueCollection["$expand"] = expand ?? "";
        }

        if (!string.IsNullOrEmpty(select))
        {
            nameValueCollection["$select"] = select ?? "";
        }
        if (!string.IsNullOrEmpty(apply))
        {
            nameValueCollection["$apply"] = apply ?? "";
        }

        if (count.HasValue)
        {
            nameValueCollection["$count"] = $"{count}".ToLower();
        }

        uriBuilder.Query = nameValueCollection.ToString();
        return uriBuilder.Uri;
    }


public SabatexRadzenBlazorODataAdapter(HttpClient httpClient, ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger, NavigationManager navigationManager)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        baseUri = new Uri(this.httpClient.BaseAddress ?? new Uri(navigationManager.BaseUri), "odata/");
        this.logger = logger;
    }
    public async Task<Core.RadzenBlazor.ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null,string? apply = null) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}");
        uri = GetODataUri2(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count,apply);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception($"Помилка запиту {response.StatusCode}");
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<Core.RadzenBlazor.ODataServiceResult<TItem>>(response);

    }
    public async Task<Core.RadzenBlazor.ODataServiceResult<TItem>> GetAsync<TItem>(QueryParams queryParams) where TItem : class, IEntityBase<TKey>
    {
        var args = queryParams.Args;
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}");
        string expand = string.Empty;
        uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, expand: expand, count: args.Top != null && args.Skip != null);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception($"Помилка запиту {response.StatusCode}");
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<Core.RadzenBlazor.ODataServiceResult<TItem>>(response);

    }

    public async Task<SabatexValidationModel<TItem>> PostAsync<TItem>(TItem? item) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, typeof(TItem).Name);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            var result = await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
            if (result == null)
                throw new DeserializeException();
            return new SabatexValidationModel<TItem>(result);
        }

        var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>() ?? new Dictionary<string, List<string>>();

        if (response.StatusCode == HttpStatusCode.BadRequest && errors.Any())
        {
            return new SabatexValidationModel<TItem>(null,errors);
        }
        throw new Exception($"Error Post with status code: {response.StatusCode}");
     }
 
    public async Task<SabatexValidationModel<TItem>> UpdateAsync<TItem>(TItem item) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({item.Id})");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            var result = await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
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

        //if (responce == null)
        //        throw new Exception(nullResponce);
        //if (responce.StatusCode == System.Net.HttpStatusCode.NotFound)
        //        throw new Exception($"Відсутній запис для Entity<{typeof(TItem).Name}> з Id = {item.Id}");
        //if (responce.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //        throw new Exception($"Код відповіді сервера - BadRequest");
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
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})"); ;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
        var responce = await httpClient.SendAsync(httpRequestMessage);
        if (responce == null)
            throw new Exception(nullResponce);

        if (responce.StatusCode != System.Net.HttpStatusCode.NoContent)
            throw new Exception($"Delete error with responce code = {responce.StatusCode}");
    }

    public async Task<TItem?> GetByIdAsync<TItem>(TKey id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        if (id is not null)
            return await GetByIdAsync<TItem>(id.ToString(),expand);

        throw new ArgumentNullException(nameof(id));
    }

    public async Task<TItem?> GetByIdAsync<TItem>(string? id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})");
        uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);

    }

}

internal struct ODataSearchFilterBuilder
{
    StringBuilder sb;
    bool first;
    public ODataSearchFilterBuilder()
    {
        sb = new StringBuilder();
        first = true;

    }

    void AddStringSearch(string FieldName, string value)
    {
        AddOperation();
        sb.Append($"contains(tolower({FieldName}),'{value}')");
    }
    void AddIntSearch(string FieldName, string value)
    {
        if (int.TryParse(value, out int _))
        {
            AddOperation();
            sb.Append($"{FieldName} eq {value}");
        }
    }
    void AddOperation()
    {
        if (!first) sb.Append(" or ");
        first = false;
    }
    public void AddField(FieldDescriptor fieldDescriptor, string value)
    {
        if (fieldDescriptor.FieldType == typeof(string))
        {
            AddStringSearch(fieldDescriptor.Name, value);
        }
        else if (fieldDescriptor.FieldType == typeof(int))
        {
            AddIntSearch(fieldDescriptor.Name, value);
        }


    }
    public override string ToString()
    {
        return sb.ToString();
    }
}