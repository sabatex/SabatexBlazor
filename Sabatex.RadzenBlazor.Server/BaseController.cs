using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sabatex.Core;
using Sabatex.RadzenBlazor;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sabatex.Core.RadzenBlazor;
using System.Globalization;



namespace Sabatex.RadzenBlazor.Server;
/// <summary>
/// Serves as the base class for API controllers that manage entities of type <typeparamref name="TItem"/>.
/// </summary>
/// <remarks>The <see cref="BaseController{TItem}"/> class provides common functionality for handling CRUD
/// operations, including querying, adding, updating, and deleting entities. It is designed to be inherited by specific
/// controllers that manage particular entity types. This class includes extension points for customizing query behavior
/// and access control, as well as built-in support for OData-style querying.  Key features include: <list
/// type="bullet"> <item><description>Support for OData-style querying, including filtering, sorting, and
/// pagination.</description></item> <item><description>Customizable query modification through virtual methods such as
/// <see cref="OnAfterIncludeInGet"/> and <see cref="OnAfterWhereInGet"/>.</description></item>
/// <item><description>Access control checks via the abstract <see cref="CheckAccess"/> method.</description></item>
/// <item><description>Pre- and post-operation hooks for CRUD actions, such as <see cref="OnBeforeAddItemToDatabase"/>
/// and <see cref="OnAfterGetById"/>.</description></item> </list> Derived classes must implement the <see
/// cref="CheckAccess"/> method to define access control logic.</remarks>
/// <typeparam name="TItem">The type of entity managed by the controller. Must implement <see cref="IEntityBase{Guid}"/> and have a
/// parameterless constructor.</typeparam>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public abstract class BaseController<TItem,TKey> : ControllerBase where TItem : class, IEntityBase<TKey>, new()
{
    /// <summary>
    /// Represents the database context used for interacting with the underlying data store.
    /// </summary>
    /// <remarks>This field is intended for use within derived classes to perform database operations. It
    /// provides access to the entity framework's DbContext, enabling CRUD operations and  querying of the data
    /// store.</remarks>
    protected readonly DbContext context;
    /// <summary>
    /// A logger instance used for logging messages and events within the class.
    /// </summary>
    /// <remarks>This field is intended for internal use by the class to facilitate logging operations. It is
    /// initialized by the class and should not be modified directly.</remarks>
    protected readonly ILogger logger;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class with the specified database context and
    /// logger.
    /// </summary>
    /// <remarks>This constructor is intended to be used by derived controller classes to provide access to
    /// database operations  and logging functionality. Ensure that valid instances of <paramref name="context"/> and
    /// <paramref name="logger"/>  are provided to avoid runtime errors.</remarks>
    /// <param name="context">The <see cref="DbContext"/> instance used to interact with the database. Cannot be null.</param>
    /// <param name="logger">The <see cref="ILogger"/> instance used for logging operations. Cannot be null.</param>
    protected BaseController(DbContext context, ILogger logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    /// <remarks>The identifier is retrieved from the user's claims using the <see
    /// cref="ClaimTypes.NameIdentifier"/> claim type. Ensure that the user is authenticated and the claim is present to
    /// avoid unexpected behavior.</remarks>
    public Guid UserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
    /// <summary>
    /// Provides an extension point for modifying the query after the default include operations  have been applied
    /// during a GET request.
    /// </summary>
    /// <remarks>Override this method to apply additional filtering, sorting, or other query modifications 
    /// after the default include operations have been performed. This method is called during  the query execution
    /// pipeline for GET requests.</remarks>
    /// <param name="query">The <see cref="IQueryable{T}"/> representing the current query.</param>
    /// <param name="queryParams">The parameters associated with the current query operation.</param>
    /// <returns>The modified <see cref="IQueryable{T}"/> to be used for further processing. By default, returns the original
    /// query unchanged.</returns>
    protected virtual IQueryable<TItem> OnAfterIncludeInGet(IQueryable<TItem> query, QueryParams queryParams)
    {
        return query;
    }
    /// <summary>
    /// Provides an extension point for modifying the query after applying a "Where In" filter.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to customize the behavior of the
    /// query  after the "Where In" filter is applied. By default, it returns the input query unchanged.</remarks>
    /// <param name="query">The <see cref="IQueryable{T}"/> representing the current query.</param>
    /// <param name="queryParams">The parameters used to configure the query.</param>
    /// <returns>The modified <see cref="IQueryable{T}"/> after applying additional logic.</returns>
    protected virtual IQueryable<TItem> OnAfterWhereInGet(IQueryable<TItem> query, QueryParams queryParams)
    {
        return query;
    }


    /// <summary>
    /// Retrieves a collection of items based on the specified query parameters.
    /// </summary>
    /// <remarks>This method supports OData-style querying, allowing clients to filter, sort, include related
    /// entities, and paginate the results. The query parameters are provided as a JSON string and must be properly
    /// formatted.</remarks>
    /// <param name="json">A JSON-encoded string representing the query parameters. The string must include valid OData query options such
    /// as filtering, ordering, inclusion of related entities, and pagination settings.</param>
    /// <returns>An <see cref="ODataServiceResult{TItem}"/> containing the queried items and, if applicable, the total count of
    /// items matching the query.</returns>
    /// <exception cref="Exception">Thrown if the provided JSON string cannot be deserialized into valid query parameters.</exception>
    [HttpGet]
    public virtual async Task<QueryResult<TItem>> Get([FromQuery] string json)
    {
        QueryParams? queryParams = JsonSerializer.Deserialize<QueryParams>(Uri.UnescapeDataString(json));

        if (queryParams == null)
            throw new Exception("Deserialize error");

        var query = context.Set<TItem>().AsQueryable<TItem>();
        if (queryParams.Include != null)
        {
            foreach (var item in queryParams.Include)
           {
                query = query.Include(item);
           }
        }
        query = OnAfterIncludeInGet(query, queryParams);

        if (queryParams.ForeginKey != null)
        {
            query = query.Where($"it => it.{queryParams.ForeginKey.Name}.ToString() == \"{queryParams.ForeginKey.Id}\"");
        }

        if (!String.IsNullOrEmpty(queryParams.Filter))
            query = query.Where(queryParams.Filter); 

        query = OnAfterWhereInGet(query,queryParams);

        if (!String.IsNullOrEmpty(queryParams.OrderBy))
        {
            query = query.OrderBy(queryParams.OrderBy);
        }
       
        if (queryParams.Skip != null)
            query = query.Skip(queryParams.Skip.Value); 
        if (queryParams.Top != null)
            query = query.Take(queryParams.Top.Value);

        var result = new QueryResult<TItem>();
        result.Value = await query.ToArrayAsync();
        if ((queryParams.Skip != null) || (queryParams.Top != null))
            result.Count = await query.CountAsync();
        return result;
    }
 
    /// <summary>
    /// Allows customization of the query used to retrieve an item by its identifier.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to apply custom logic to the
    /// query before retrieving an item by its identifier. By default, it returns the unmodified query.</remarks>
    /// <param name="query">The initial query representing the data source. This query can be modified to apply additional filters or
    /// transformations.</param>
    /// <param name="id">The unique identifier of the item to be retrieved. This parameter can be used to refine the query.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> representing the modified query. The returned query will be used to retrieve
    /// the item by its identifier.</returns>
    protected virtual IQueryable<TItem> OnBeforeGetById(IQueryable<TItem> query,TKey id)
    {
        return query;
    }
    /// <summary>
    /// Executes additional processing after retrieving an item by its identifier.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to implement custom
    /// post-retrieval logic. The default implementation performs no operation.</remarks>
    /// <param name="item">The item retrieved by its identifier. This parameter may be <see langword="null"/> if no item was found.</param>
    /// <param name="id">The unique identifier used to retrieve the item.</param>
    /// <returns></returns>
    protected virtual async Task OnAfterGetById(TItem item, TKey id)
    {
        await Task.Yield();
    }
    /// <summary>
    /// Determines whether access is granted for the specified item and its optional updated state.
    /// </summary>
    /// <param name="item">The item to evaluate for access permissions. Cannot be <see langword="null"/>.</param>
    /// <param name="updated">The optional updated state of the item. May be <see langword="null"/> if no updates are provided.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if access is
    /// granted; otherwise, <see langword="false"/>.</returns>
    protected abstract Task<bool> CheckAccess(TItem item,TItem? updated);

    /// <summary>
    /// Retrieves an item by its unique identifier.
    /// </summary>
    /// <remarks>This method performs an access check on the retrieved item. If access is granted,  the item
    /// is returned in the response. Otherwise, an unauthorized response is returned.</remarks>
    /// <param name="id">The unique identifier of the item to retrieve.</param>
    /// <returns>An <see cref="IActionResult"/> containing the requested item if access is granted,  or an <see
    /// cref="UnauthorizedResult"/> if access is denied.</returns>
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById([FromRoute]TKey id)
    {
        var query = context.Set<TItem>().AsQueryable<TItem>();
        query = OnBeforeGetById(query,id);
        var result  = await query.Where(s=>EqualityComparer<TKey>.Default.Equals(s.Id,id)).SingleAsync();
        if (await CheckAccess(result,null))
        {
            await OnAfterGetById(result, id);
            return Ok(result);
        }
        return Unauthorized(); 
    }

    /// <summary>
    /// Performs actions or validations before adding an item to the database.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to implement custom pre-add
    /// logic,  such as validation or preprocessing of the item. The base implementation does not perform any
    /// actions.</remarks>
    /// <param name="item">The item to be added to the database. Cannot be null.</param>
    /// <returns></returns>
    protected virtual async Task OnBeforeAddItemToDatabase(TItem item) => await Task.Yield();
    /// <summary>
    /// Invoked after an object of type <typeparamref name="TItem"/> has been saved.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to perform additional actions    
    /// after the save operation completes. The default implementation does not perform any actions.</remarks>
    /// <param name="item">The object of type <typeparamref name="TItem"/> that was saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task OnAfterSaveObject(TItem item)=> await Task.Yield();

    /// <summary>
    /// Handles HTTP POST requests to add a new item to the database.
    /// </summary>
    /// <remarks>This method validates the incoming item and checks user access before adding the item to the
    /// database. If the model state is invalid or the item is null, the method returns a bad request response. If an
    /// exception occurs during processing, the exception message is added to the model state and returned as part of
    /// the bad request response.</remarks>
    /// <param name="value">The item to be added to the database. This parameter is bound from the request body and must not be null.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet"> <item><see
    /// cref="OkObjectResult"/> containing the added item if the operation succeeds.</item> <item><see
    /// cref="BadRequestObjectResult"/> if the model state is invalid or an error occurs during processing.</item>
    /// <item><see cref="UnauthorizedResult"/> if the user does not have access to perform the operation.</item> </list></returns>
   [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TItem value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (value == null)
        {
            ModelState.AddModelError(string.Empty, "The post null value");
            return BadRequest(ModelState);
        }
        try
        {
            await this.OnBeforeAddItemToDatabase(value);
            if (await CheckAccess(value, null))
            {
                await context.Set<TItem>().AddAsync(value);
                await context.SaveChangesAsync();
                await OnAfterSaveObject(value);
                return Ok(value);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }
    /// <summary>
    /// Invoked before an update operation is performed on an item.
    /// </summary>
    /// <remarks>Override this method to implement custom logic that should execute prior to updating an item.
    /// This method is asynchronous and can be used to perform pre-update validations, logging, or other
    /// operations.</remarks>
    /// <param name="item">The original item before the update.</param>
    /// <param name="update">The updated item containing the new values.</param>
    /// <returns></returns>
    protected virtual async Task OnBeforeUpdateAsync(TItem item,TItem update)
    {
       await Task.Yield();
    }
    
    /// <summary>
    /// Updates an existing item in the database with the specified values.
    /// </summary>
    /// <remarks>This method performs several checks before updating the item: <list type="bullet">
    /// <item><description>Validates the model state.</description></item> <item><description>Ensures the <paramref
    /// name="id"/> matches the <see cref="TItem.Id"/> property of <paramref name="update"/>.</description></item>
    /// <item><description>Checks if the item exists in the database.</description></item> <item><description>Verifies
    /// access permissions using the <c>CheckAccess</c> method.</description></item> </list> If a concurrency conflict
    /// occurs during the update, the method will throw an exception unless the item no longer exists, in which case it
    /// returns <see cref="NotFoundResult"/>.</remarks>
    /// <param name="id">The unique identifier of the item to update.</param>
    /// <param name="update">The updated values for the item. The <see cref="TItem.Id"/> property must match the <paramref name="id"/>
    /// parameter.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation: <list type="bullet">
    /// <item><description><see cref="BadRequestResult"/> if the model state is invalid or the <paramref name="id"/>
    /// does not match the <see cref="TItem.Id"/> property of <paramref name="update"/>.</description></item>
    /// <item><description><see cref="NotFoundResult"/> if the item with the specified <paramref name="id"/> does not
    /// exist.</description></item> <item><description><see cref="UnauthorizedResult"/> if the caller does not have
    /// access to update the item.</description></item> <item><description><see cref="OkObjectResult"/> containing the
    /// updated item if the operation succeeds.</description></item> </list></returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put([FromRoute] TKey id, TItem update)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (! EqualityComparer<TKey>.Default.Equals(id,update.Id))
        {
            return BadRequest("Mismatched entity ID.");
        }

        var item = await context.Set<TItem>().FindAsync(id);
        if (item == null)
            return NotFound();

        if (!await CheckAccess(item,update))
            return Unauthorized(ModelState);

        if (item is IVersionedEntity versionedItem &&  update is IVersionedEntity versionedUpdate &&  versionedItem.DateStamp != versionedUpdate.DateStamp)
        {
            return Conflict("The item was modified by another user.");
        }

        //  update the DateStamp if the item implements IVersionedEntity
        if (item is IVersionedEntity versioned)
            versioned.DateStamp = DateTimeOffset.UtcNow;

        await OnBeforeUpdateAsync(item,update);
        context.Entry(item).CurrentValues.SetValues(update);
        try
        {
            await context.SaveChangesAsync();
            await OnAfterSaveObject(update);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ValueExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Ok(update);
    }


    /// <summary>
    /// Deletes the specified item by its unique identifier.
    /// </summary>
    /// <remarks>This method performs a deletion operation on an item identified by <paramref name="id"/>. 
    /// Before the item is deleted, the <c>OnBeforeDeleteAsync</c> method is invoked to determine  whether the deletion
    /// is authorized. If authorization fails, the method returns  <see cref="UnauthorizedResult"/>.</remarks>
    /// <param name="id">The unique identifier of the item to delete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="NotFoundResult"/> if
    /// the item does not exist,  <see cref="NoContentResult"/> if the item is successfully deleted,  or <see
    /// cref="UnauthorizedResult"/> if the deletion is not authorized.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var item = await context.Set<TItem>().FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        if (await OnBeforeDeleteAsync(item))
        {
            context.Set<TItem>().Remove(item);
            await context.SaveChangesAsync();
            return new NoContentResult();
        }
        else
            return Unauthorized();
    }
    /// <summary>
    /// Executes custom logic before an item is deleted.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement custom pre-deletion behavior. The
    /// default implementation always returns <see langword="true"/>.</remarks>
    /// <param name="item">The item that is about to be deleted. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the deletion
    /// should proceed; otherwise, <see langword="false"/>.</returns>
    protected virtual async Task<bool> OnBeforeDeleteAsync(TItem item)
    {
        await Task.Yield();
        return true;
    }


    private bool ValueExists(TKey key)
    {
        return context.Set<TItem>().Any(p => EqualityComparer<TKey>.Default.Equals(p.Id,key));
    }



}


/// <summary>
/// Serves as the base class for API controllers that manage entities of type <typeparamref name="TItem"/>.
/// </summary>
/// <remarks>The <see cref="BaseController{TItem}"/> class provides common functionality for handling CRUD
/// operations, including querying, adding, updating, and deleting entities. It is designed to be inherited by specific
/// controllers that manage particular entity types. This class includes extension points for customizing query behavior
/// and access control, as well as built-in support for OData-style querying.  Key features include: <list
/// type="bullet"> <item><description>Support for OData-style querying, including filtering, sorting, and
/// pagination.</description></item> <item><description>Customizable query modification through virtual methods such as
/// <see cref="OnAfterIncludeInGet"/> and <see cref="OnAfterWhereInGet"/>.</description></item>
/// <item><description>Access control checks via the abstract <see cref="CheckAccess"/> method.</description></item>
/// <item><description>Pre- and post-operation hooks for CRUD actions, such as <see cref="OnBeforeAddItemToDatabase"/>
/// and <see cref="OnAfterGetById"/>.</description></item> </list> Derived classes must implement the <see
/// cref="CheckAccess"/> method to define access control logic.</remarks>
/// <typeparam name="TItem">The type of entity managed by the controller. Must implement <see cref="IEntityBase{Guid}"/> and have a
/// parameterless constructor.</typeparam>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public abstract class BaseController<TItem> : BaseController<TItem,Guid> where TItem : class, IEntityBase<Guid>, new()
{
    protected BaseController(DbContext context, ILogger logger): base(context, logger)
    {
        
    }
}