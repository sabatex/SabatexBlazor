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

/// <summary>
/// HTTP-based data adapter for Blazor WebAssembly that communicates with REST API endpoints.
/// </summary>
/// <remarks>
/// <para>
/// This adapter implements <see cref="ISabatexRadzenBlazorDataAdapter"/> and provides CRUD operations 
/// via HTTP requests to a backend API (typically powered by <c>BaseController</c>).
/// </para>
/// <para><b>Supported operations:</b></para>
/// <list type="bullet">
/// <item><description>Query with filtering, sorting, pagination (POST /api/{EntityName}/query)</description></item>
/// <item><description>Get by ID with eager loading support (GET /api/{EntityName}/{id})</description></item>
/// <item><description>Create new entities (POST /api/{EntityName})</description></item>
/// <item><description>Update existing entities (PUT /api/{EntityName}/{id})</description></item>
/// <item><description>Delete entities (DELETE /api/{EntityName}/{id})</description></item>
/// </list>
/// <para><b>Error handling:</b></para>
/// <list type="bullet">
/// <item><description>Returns <see cref="SabatexValidationModel{TItem}"/> with validation errors on HTTP 400</description></item>
/// <item><description>Throws <see cref="ExceptionAccessDenied"/> on HTTP 401 (Unauthorized)</description></item>
/// <item><description>Logs errors via <see cref="ILogger"/></description></item>
/// </list>
/// <para><b>Usage example (injected in Blazor component):</b></para>
/// <code>
/// @@inject ISabatexRadzenBlazorDataAdapter DataAdapter
/// 
/// var queryParams = new QueryParams
/// {
///     Filter = "Name.Contains(\"John\")",
///     OrderBy = "CreatedDate desc",
///     Skip = 0,
///     Top = 10
/// };
/// var result = await DataAdapter.GetAsync&lt;Person, Guid&gt;(queryParams);
/// </code>
/// </remarks>
public class SabatexRadzenBlazorApiDataAdapter : ISabatexRadzenBlazorDataAdapter
{
    const string nullResponce = "The responece return null";

    /// <summary>
    /// The HTTP client used for API requests.
    /// </summary>
    protected readonly HttpClient httpClient;

    /// <summary>
    /// The base URI for API endpoints (typically {BaseAddress}/api/).
    /// </summary>
    protected readonly Uri baseUri;

    /// <summary>
    /// Logger for error and diagnostic messages.
    /// </summary>
    private readonly ILogger<SabatexRadzenBlazorApiDataAdapter> logger;

    /// <summary>
    /// Navigation manager for retrieving current URI.
    /// </summary>
    private readonly NavigationManager navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SabatexRadzenBlazorApiDataAdapter"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for API requests. Typically configured with base address in DI.</param>
    /// <param name="logger">Logger for error and diagnostic messages.</param>
    /// <param name="navigationManager">Navigation manager for retrieving current URI (used in error logging).</param>
    /// <remarks>
    /// <para>The base URI is constructed as <c>{HttpClient.BaseAddress}/api/</c> or <c>{NavigationManager.BaseUri}/api/</c> if BaseAddress is not set.</para>
    /// </remarks>
    public SabatexRadzenBlazorApiDataAdapter(HttpClient httpClient, ILogger<SabatexRadzenBlazorApiDataAdapter> logger, NavigationManager navigationManager)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        baseUri = new Uri(this.httpClient.BaseAddress ?? new Uri(navigationManager.BaseUri), "api/");
        this.logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated, filtered, and sorted collection of entities from the API.
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="queryParams">Query parameters including filters, sorting, pagination, and includes.</param>
    /// <returns>A <see cref="QueryResult{TItem}"/> containing the items and total count for pagination.</returns>
    /// <remarks>
    /// <para>Sends a POST request to <c>/api/{EntityName}/query</c> with <paramref name="queryParams"/> in the request body.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// var queryParams = new QueryParams
    /// {
    ///     Filter = "Age >= 18",
    ///     OrderBy = "Name",
    ///     Skip = 0,
    ///     Top = 10,
    ///     Include = new[] { "Orders", "Address" }
    /// };
    /// var result = await adapter.GetAsync&lt;Person, Guid&gt;(queryParams);
    /// </code>
    /// </remarks>
    /// <exception cref="ExceptionAccessDenied">Thrown when the API returns HTTP 401 (Unauthorized).</exception>
    /// <exception cref="Exception">Thrown when the API returns an error status code other than 401.</exception>
    public async Task<QueryResult<TItem>> GetAsync<TItem, TKey>(QueryParams queryParams) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/query");

        var response = await httpClient.PostAsJsonAsync(uri, queryParams);
        if (response.IsSuccessStatusCode)
        {
            var result = await ReadAsync<QueryResult<TItem>>(response);
            return result ?? throw new DeserializeException();
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogWarning("Access denied to resource {Resource} for user {User}", typeof(TItem).Name, navigationManager.Uri);
            throw new ExceptionAccessDenied();
        }

        var error = $"Error retrieving resource {navigationManager.Uri} for user {typeof(TItem).Name}. Status code: {response.StatusCode}";
        logger.LogError(error);
        throw new Exception(error);
    }

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="item">The entity to create. Cannot be null.</param>
    /// <returns>A <see cref="SabatexValidationModel{TItem}"/> containing the created entity or validation errors.</returns>
    /// <remarks>
    /// <para>Sends a POST request to <c>/api/{EntityName}</c> with <paramref name="item"/> in the request body.</para>
    /// <para><b>Success (HTTP 200-299):</b> Returns <see cref="SabatexValidationModel{TItem}"/> with the created entity (including server-generated ID).</para>
    /// <para><b>Validation errors (HTTP 400):</b> Returns <see cref="SabatexValidationModel{TItem}"/> with validation errors dictionary.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// var person = new Person { Name = "John Doe", Email = "john@example.com" };
    /// var result = await adapter.PostAsync&lt;Person, Guid&gt;(person);
    /// if (result.IsValid)
    /// {
    ///     var createdPerson = result.Value; // Contains server-generated ID
    /// }
    /// else
    /// {
    ///     // Display validation errors: result.Errors
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="DeserializeException">Thrown when the API response cannot be deserialized.</exception>
    /// <exception cref="Exception">Thrown when the API returns an error status code other than 400.</exception>
    public async Task<SabatexValidationModel<TItem>> PostAsync<TItem, TKey>(TItem? item) where TItem : class, IEntityBase<TKey>
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

        if (response.StatusCode == HttpStatusCode.BadRequest && errors.Any())
        {
            return new SabatexValidationModel<TItem>(null, errors);
        }
        throw new Exception($"Error Post with status code: {response.StatusCode}");
    }

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>Sends a DELETE request to <c>/api/{EntityName}/{id}</c>.</para>
    /// <para>Expects HTTP 204 (No Content) on success.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// await adapter.DeleteAsync&lt;Person, Guid&gt;(personId);
    /// </code>
    /// </remarks>
    /// <exception cref="Exception">Thrown when the API response is null or does not return HTTP 204.</exception>
    public async Task DeleteAsync<TItem, TKey>(TKey id) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{id}");
        var responce = await httpClient.DeleteAsync(uri);
        if (responce == null)
            throw new Exception(nullResponce);

        if (responce.StatusCode != System.Net.HttpStatusCode.NoContent)
            throw new Exception($"Delete error with responce code = {responce.StatusCode}");
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="item">The entity with updated values. The <see cref="IEntityBase{TKey}.Id"/> property must match the entity to update.</param>
    /// <returns>A <see cref="SabatexValidationModel{TItem}"/> containing the updated entity or validation errors.</returns>
    /// <remarks>
    /// <para>Sends a PUT request to <c>/api/{EntityName}/{id}</c> with <paramref name="item"/> in the request body.</para>
    /// <para><b>Success (HTTP 200-299):</b> Returns <see cref="SabatexValidationModel{TItem}"/> with the updated entity.</para>
    /// <para><b>Validation errors (HTTP 400):</b> Returns <see cref="SabatexValidationModel{TItem}"/> with validation errors dictionary.</para>
    /// <para><b>Concurrency conflict (HTTP 409):</b> Thrown as Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException by the server (if <c>IVersionedEntity</c> is used).</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// person.Name = "Jane Doe";
    /// var result = await adapter.UpdateAsync&lt;Person, Guid&gt;(person);
    /// if (result.IsValid)
    /// {
    ///     var updatedPerson = result.Value;
    /// }
    /// else
    /// {
    ///     // Display validation errors: result.Errors
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="DeserializeException">Thrown when the API response cannot be deserialized.</exception>
    /// <exception cref="Exception">Thrown when the API returns an error status code other than 400.</exception>
    public async Task<SabatexValidationModel<TItem>> UpdateAsync<TItem, TKey>(TItem item) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{item.Id}");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsJsonAsync(uri, item);
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

    /// <summary>
    /// Retrieves a single entity by its ID.
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="id">The ID of the entity to retrieve. Cannot be null.</param>
    /// <param name="expand">Optional. Comma-separated list of navigation properties to eagerly load (e.g., "Orders,Address").</param>
    /// <returns>The entity if found, or <c>null</c> if not found.</returns>
    /// <remarks>
    /// <para>Sends a GET request to <c>/api/{EntityName}/{id}</c>.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// var person = await adapter.GetByIdAsync&lt;Person, Guid&gt;(personId, expand: "Orders,Address");
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null.</exception>
    public async Task<TItem?> GetByIdAsync<TItem, TKey>(TKey id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        if (id is not null)
            return await GetByIdAsync<TItem, TKey>(id.ToString(), expand);
        else
            throw new ArgumentNullException(nameof(id));
    }

    /// <summary>
    /// Retrieves a single entity by its ID (string representation).
    /// </summary>
    /// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    /// <param name="id">The string representation of the entity's ID. Cannot be null.</param>
    /// <param name="expand">Optional. Comma-separated list of navigation properties to eagerly load (e.g., "Orders,Address").</param>
    /// <returns>The entity if found, or <c>null</c> if not found.</returns>
    /// <remarks>
    /// <para>Sends a GET request to <c>/api/{EntityName}/{id}</c>.</para>
    /// <para>This overload accepts a string ID for scenarios where the ID is already in string format.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null.</exception>
    public async Task<TItem?> GetByIdAsync<TItem, TKey>(string? id, string? expand = null) where TItem : class, IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}/{id}");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
    }

    /// <summary>
    /// Reads and deserializes the HTTP response content.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The deserialized object, or <c>default(T)</c> if the response body is empty.</returns>
    /// <remarks>
    /// <para>This method handles error responses by parsing error messages from JSON or XML.</para>
    /// <para>Supports error formats: <c>{"error": {"message": "..."}}</c> (JSON) or <c>&lt;error&gt;...&lt;/error&gt;</c> (XML).</para>
    /// </remarks>
    /// <exception cref="Exception">Thrown when the response indicates an error or cannot be deserialized.</exception>
    async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                if (stream == null)
                    return default(T);

                return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch
        {
            string text = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(text))
            {
                if (response.Content.Headers.ContentType?.MediaType == "application/json")
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
                    XElement? xElement2;
                    try
                    {
                        XDocument xDocument = XDocument.Parse(text);
                        XElement? xElement = xDocument.Descendants().SingleOrDefault((XElement p) => p.Name.LocalName == "internalexception");
                        xElement2 = (xElement == null) ? xDocument.Descendants().SingleOrDefault((XElement p) => p.Name.LocalName == "error") : xElement;
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
