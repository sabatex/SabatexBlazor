using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// A universal CRUD data grid component for Blazor WebAssembly that integrates with 
/// <see cref="ISabatexRadzenBlazorDataAdapter"/> and wraps <see cref="RadzenDataGrid{TItem}"/>.
/// </summary>
/// <remarks>
/// <para>
/// This component provides a complete CRUD interface with support for:
/// <list type="bullet">
/// <item><description>Automatic data loading via <see cref="ISabatexRadzenBlazorDataAdapter"/></description></item>
/// <item><description>Inline editing or navigation to separate edit pages</description></item>
/// <item><description>Filtering, sorting, and pagination (via Radzen DataGrid)</description></item>
/// <item><description>Foreign key support for master-detail scenarios</description></item>
/// <item><description>Grid state persistence in browser localStorage</description></item>
/// <item><description>Customizable toolbar buttons and row actions</description></item>
/// <item><description>Automatic error handling and notifications</description></item>
/// </list>
/// </para>
/// <para><b>Basic Usage Example:</b></para>
/// <code>
/// &lt;SabatexRadzenGrid TItem="Person" TKey="Guid"&gt;
///     &lt;Columns&gt;
///         &lt;RadzenDataGridColumn TItem="Person" Property="Name" Title="Name" /&gt;
///         &lt;RadzenDataGridColumn TItem="Person" Property="Email" Title="Email" /&gt;
///     &lt;/Columns&gt;
/// &lt;/SabatexRadzenGrid&gt;
/// </code>
/// <para><b>Inline Edit Example:</b></para>
/// <code>
/// &lt;SabatexRadzenGrid TItem="Person" TKey="Guid" InlineEdit="true"&gt;
///     &lt;Columns&gt;
///         &lt;RadzenDataGridColumn TItem="Person" Property="Name" Title="Name"&gt;
///             &lt;EditTemplate Context="person"&gt;
///                 &lt;RadzenTextBox @@bind-Value="person.Name" /&gt;
///             &lt;/EditTemplate&gt;
///         &lt;/RadzenDataGridColumn&gt;
///     &lt;/Columns&gt;
/// &lt;/SabatexRadzenGrid&gt;
/// </code>
/// <para><b>Master-Detail Example (Foreign Key):</b></para>
/// <code>
/// var foreignKey = new ForeginKey { Name = "CustomerId", Id = customerId };
/// &lt;SabatexRadzenGrid TItem="Order" TKey="Guid" ForeginKey="@@foreignKey"&gt;
///     &lt;Columns&gt;
///         &lt;RadzenDataGridColumn TItem="Order" Property="OrderNumber" Title="Order #" /&gt;
///     &lt;/Columns&gt;
/// &lt;/SabatexRadzenGrid&gt;
/// </code>
/// </remarks>
/// <typeparam name="TKey">The type of the entity's primary key (e.g., <see cref="Guid"/>, <see cref="int"/>, <see cref="long"/>).</typeparam>
/// <typeparam name="TItem">The entity type. Must implement <see cref="IEntityBase{TKey}"/> and have a parameterless constructor.</typeparam>
public partial class SabatexRadzenGrid<TKey,TItem>: SabatexRadzenBlazorBaseDataComponent where TItem : class, IEntityBase<TKey>, new()
{
    #region Parameters
    /// <summary>
    /// Gets or sets the filter mode for the data grid.
    /// </summary>
    /// <remarks>
    /// <para>Default value is <see cref="FilterMode.Simple"/>.</para>
    /// <para><b>Available modes:</b></para>
    /// <list type="bullet">
    /// <item><description><see cref="FilterMode.Simple"/> - Single filter per column</description></item>
    /// <item><description><see cref="FilterMode.Advanced"/> - Multiple filters with AND/OR logic</description></item>
    /// </list>
    /// </remarks>
    [Parameter]
    public FilterMode FilterMode { get; set; } = FilterMode.Simple;
    /// <summary>
    /// Gets or sets the tooltip text for the Add button.
    /// </summary>
    /// <remarks>
    /// Default value is "Add new item".
    /// </remarks>
    [Parameter]
    public string TooltipAddButtonText { get; set; } = "Add new item";

    /// <summary>
    /// Defines the columns to display in the grid.
    /// </summary>
    /// <remarks>
    /// <para>Use <see cref="RadzenDataGridColumn{TItem}"/> components inside this fragment.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// &lt;Columns&gt;
    ///     &lt;RadzenDataGridColumn TItem="Person" Property="Name" Title="Name" /&gt;
    ///     &lt;RadzenDataGridColumn TItem="Person" Property="Email" Title="Email" /&gt;
    ///     &lt;RadzenDataGridColumn TItem="Person" Property="CreatedDate" Title="Created" FormatString="{0:d}" /&gt;
    /// &lt;/Columns&gt;
    /// </code>
    /// </remarks>
    [Parameter]
    public RenderFragment Columns { get; set; } = default!;

    /// <summary>
    /// Defines custom toolbar buttons that receive the currently selected item as a parameter.
    /// </summary>
    /// <remarks>
    /// <para>Use this to add custom actions to the toolbar. The selected item (or <c>null</c> if no selection) is passed as context.</para>
    /// <para><b>Example:</b></para>
    /// <code>
    /// &lt;SabatexRadzenGrid TItem="Person" TKey="Guid"&gt;
    ///     &lt;Buttons Context="person"&gt;
    ///         &lt;RadzenButton Icon="email" Text="Send Email"
    ///                       Click="@(() => SendEmail(person))"
    ///                       Disabled="@(person == null)" /&gt;
    ///     &lt;/Buttons&gt;
    ///     &lt;Columns&gt;...&lt;/Columns&gt;
    /// &lt;/SabatexRadzenGrid&gt;
    /// </code>
    /// </remarks>
    [Parameter]
    public RenderFragment<TItem?> Buttons { get; set; } = default!;

    /// <summary>
    /// Gets or sets the foreign key for master-detail scenarios.
    /// </summary>
    /// <remarks>
    /// <para>When set, the grid automatically filters data by this foreign key.</para>
    /// <para><b>Example (Orders grid for a specific Customer):</b></para>
    /// <code>
    /// var foreignKey = new ForeginKey { Name = "CustomerId", Id = customerId };
    /// &lt;SabatexRadzenGrid TItem="Order" TKey="Guid" ForeginKey="@foreignKey"&gt;
    ///     &lt;Columns&gt;
    ///         &lt;RadzenDataGridColumn TItem="Order" Property="OrderNumber" Title="Order #" /&gt;
    ///     &lt;/Columns&gt;
    /// &lt;/SabatexRadzenGrid&gt;
    /// </code>
    /// <para>The foreign key is also passed to the edit page via query parameters.</para>
    /// </remarks>
    [Parameter]
    public ForeginKey? ForeginKey { get; set; }

    /// <summary>
    /// Gets or sets the edit mode for the grid.
    /// </summary>
    /// <remarks>
    /// <para>Default value is <see cref="DataGridEditMode.Single"/>.</para>
    /// <para><b>Available modes:</b></para>
    /// <list type="bullet">
    /// <item><description><see cref="DataGridEditMode.Single"/> - Edit one row at a time</description></item>
    /// <item><description><see cref="DataGridEditMode.Multiple"/> - Edit multiple rows simultaneously</description></item>
    /// </list>
    /// </remarks>
    [Parameter]
    public DataGridEditMode? EditMode { get; set; }

    /// <summary>
    /// Gets or sets whether inline editing is enabled.
    /// </summary>
    /// <remarks>
    /// <para>When <c>true</c>, rows are edited directly in the grid. When <c>false</c> (default), clicking Edit navigates to a separate edit page.</para>
    /// <para><b>Inline editing requires defining <c>EditTemplate</c> for each editable column.</b></para>
    /// <para><b>Example with Inline Edit:</b></para>
    /// <code>
    /// &lt;SabatexRadzenGrid TItem="Person" TKey="Guid" InlineEdit="true"&gt;
    ///     &lt;Columns&gt;
    ///         &lt;RadzenDataGridColumn TItem="Person" Property="Name" Title="Name"&gt;
    ///             &lt;EditTemplate Context="person"&gt;
    ///                 &lt;RadzenTextBox @bind-Value="person.Name" /&gt;
    ///             &lt;/EditTemplate&gt;
    ///         &lt;/RadzenDataGridColumn&gt;
    ///     &lt;/Columns&gt;
    /// &lt;/SabatexRadzenGrid&gt;
    /// </code>
    /// </remarks>
    [Parameter]
    public bool? InlineEdit { get; set; }

    /// <summary>
    /// Gets or sets whether double-clicking a row navigates to the edit page.
    /// </summary>
    /// <remarks>
    /// <para>Default value is <c>true</c>.</para>
    /// <para>When <c>true</c>, double-clicking a row navigates to the edit page. When <c>false</c>, double-clicking invokes <see cref="OnRowClick"/> instead.</para>
    /// </remarks>
    [Parameter]
    public bool DoubleClickRowEdit { get; set; } = true;

    /// <summary>
    /// Gets or sets a custom cell render callback.
    /// </summary>
    /// <remarks>
    /// <para>Use this to customize cell rendering (e.g., applying conditional styles).</para>
    /// <para><b>Example (highlight negative values in red):</b></para>
    /// <code>
    /// CellRender="@(args =>
    /// {
    ///     if (args.Column.Property == "Balance" &amp;&amp; args.Data.Balance &lt; 0)
    ///         args.Attributes["style"] = "color: red; font-weight: bold;";
    /// })"
    /// </code>
    /// </remarks>
    [Parameter]
    public Action<DataGridCellRenderEventArgs<TItem>>? CellRender { get; set; }

    /// <summary>
    /// Gets or sets the list of nested properties to include in queries.
    /// </summary>
    /// <remarks>
    /// <para>Use this to eagerly load related entities (similar to Entity Framework's <c>Include</c>).</para>
    /// <para><b>Example (include Customer and Orders for each Person):</b></para>
    /// <code>
    /// IncludeNestedProperty="@(new[] { "Customer", "Orders" })"
    /// </code>
    /// <para>This translates to <c>.Include("Customer").Include("Orders")</c> in the backend query.</para>
    /// </remarks>
    [Parameter]
    public IEnumerable<string>? IncludeNestedProperty { get; set; }

    /// <summary>
    /// Gets or sets whether the Add button is visible.
    /// </summary>
    /// <remarks>
    /// <para>Default value is <c>true</c>.</para>
    /// <para>Set to <c>false</c> to hide the Add button and disable creating new items.</para>
    /// </remarks>
    [Parameter]
    public bool IsInserted { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the Delete button is visible.
    /// </summary>
    /// <remarks>
    /// <para>Default value is <c>true</c>.</para>
    /// <para>Set to <c>false</c> to hide the Delete button and disable deleting items.</para>
    /// </remarks>
    [Parameter]
    public bool IsDeleted { get; set; } = true;

    /// <summary>
    /// Callback invoked after a new row is created (before saving to the database).
    /// </summary>
    /// <remarks>
    /// <para>Use this to set default values for new entities.</para>
    /// <para><b>Example (set default CreatedDate):</b></para>
    /// <code>
    /// &lt;SabatexRadzenGrid TItem="Order" TKey="Guid" 
    ///                     OnCreatedRow="@(order => order.CreatedDate = DateTime.UtcNow)"&gt;
    ///     &lt;Columns&gt;...&lt;/Columns&gt;
    /// &lt;/SabatexRadzenGrid&gt;
    /// </code>
    /// </remarks>
    [Parameter]
    public EventCallback<TItem> OnCreatedRow { get; set; }

    /// <summary>
    /// Callback invoked before saving an item (create or update).
    /// </summary>
    /// <remarks>
    /// <para>Use this to perform custom validation or modification before saving.</para>
    /// <para><b>Example (set UpdatedDate before saving):</b></para>
    /// <code>
    /// OnBeforeSave="@(item => item.UpdatedDate = DateTime.UtcNow)"
    /// </code>
    /// </remarks>
    [Parameter]
    public EventCallback<TItem> OnBeforeSave { get; set; }

    /// <summary>
    /// Callback invoked when a row is clicked (when <see cref="DoubleClickRowEdit"/> is <c>false</c>).
    /// </summary>
    /// <remarks>
    /// <para>Use this for custom row click actions instead of navigating to the edit page.</para>
    /// <para><b>Example (show details dialog):</b></para>
    /// <code>
    /// OnRowClick="@(person => ShowDetailsDialog(person))"
    /// DoubleClickRowEdit="false"
    /// </code>
    /// </remarks>
    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    /// <summary>
    /// Gets or sets the URL of the form to use for editing the associated item.
    /// </summary>
    [Parameter]
    public string? EditFormUrl { get; set; }
    #endregion Parameters



    /// <summary>
    /// Gets whether the grid is currently in edit row state (inserting a new row).
    /// </summary>
    public bool IsEditRowState => itemToInsert != null;
    
    /// <summary>
    /// Gets the collection of items currently loaded in the grid.
    /// </summary>
    public IEnumerable<TItem> Items => dataCollection.Value ?? Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets or sets whether the grid is read-only (all editing actions disabled).
    /// </summary>
    /// <remarks>
    /// <para>Default value is <c>false</c>.</para>
    /// <para>When <c>true</c>, Add, Edit, Delete buttons are disabled.</para>
    /// </remarks>
    public bool IsGridRO { get; set; } = false;

    /// <summary>
    /// Reference to the underlying <see cref="RadzenDataGrid{TItem}"/> component.
    /// </summary>
    protected RadzenDataGrid<TItem> grid = default!;

    /// <summary>
    /// The current data collection displayed in the grid (items + total count for pagination).
    /// </summary>
    protected QueryResult<TItem> dataCollection = new QueryResult<TItem>();

    /// <summary>
    /// Gets whether the grid is currently loading data.
    /// </summary>
    protected bool IsGridDataLoading = false;

    /// <summary>
    /// The list of currently selected items (single selection mode).
    /// </summary>
    protected IList<TItem>? SelectedItems;

    /// <summary>
    /// Gets the currently selected item (or <c>null</c> if no selection).
    /// </summary>
    private TItem? CurrentItem => SelectedItems?.First();

    string gridStyle = "height:100%";

    /// <summary>
    /// Gets or sets the name of the edit page route.
    /// </summary>
    /// <remarks>
    /// <para>Default value is "edit".</para>
    /// <para>This is appended to the current route when navigating to the edit page.</para>
    /// <para><b>Example:</b> If current route is <c>/people</c>, clicking Edit navigates to <c>/people/edit/{id}</c>.</para>
    /// </remarks>
    public virtual string EditPageName { get; set; } = "edit";

    /// <summary>
    /// Gets whether any item is currently selected.
    /// </summary>
    bool IsItemSelected => SelectedItems?.First() != null;

    /// <summary>
    /// Loads data from the API based on grid state (filtering, sorting, pagination).
    /// </summary>
    /// <param name="args">The load data arguments from Radzen DataGrid.</param>
    protected async Task GridLoadData(Radzen.LoadDataArgs args)
    {
        IsGridDataLoading = true;
        try
        {
            if (dataCollection == null)
                throw new ArgumentNullException(nameof(dataCollection));

            var queryParams = new QueryParams
            {
                Filter = args.Filter,
                OrderBy = args.OrderBy,
                Skip = args.Skip,
                Top = args.Top,
                ForeginKey = ForeginKey,
                Include = IncludeNestedProperty ?? new string[] { },
            };

            var result = await DataAdapter.GetAsync<TItem, TKey>(queryParams);
            dataCollection.Value = result.Value;
            dataCollection.Count = result.Count;
        }
        catch (Exception e)
        {
            string error = $"Помилка отримання даних {typeof(TItem).Name}  {e.Message}";
            NotificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Помилка",
                Detail = error
            });
        }
        IsGridDataLoading = false;
    }

    TItem? itemToInsert;

    /// <summary>
    /// Handles the Add button click event.
    /// </summary>
    /// <remarks>
    /// <para>If <see cref="InlineEdit"/> is <c>true</c>, inserts a new row inline. Otherwise, navigates to the edit page.</para>
    /// </remarks>
    public virtual async Task AddButtonClick(MouseEventArgs args)
    {
        if (InlineEdit ?? false)
        {
            itemToInsert = new TItem();
            await OnCreatedRow.InvokeAsync(itemToInsert);
            if (dataCollection.Count == 0) dataCollection.Count++;
            await grid.InsertRow(itemToInsert);

        }
        else
        {
            NavigateToEditPage(null);
        }
    }

    /// <summary>
    /// Gets the URI for the edit page.
    /// </summary>
    /// <remarks>
    /// <para>Constructed as: <c>{currentBasePath}/{<see cref="EditPageName"/>}</c></para>
    /// <para><b>Example:</b> If current route is <c>/people</c> and <see cref="EditPageName"/> is "edit", returns <c>/people/edit</c>.</para>
    /// </remarks>
    protected virtual string EditPageUri
    {
        get
        {
            var url = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();
            var index = url.LastIndexOf("/");
            string baseRoute = string.Empty;
            if (index != -1)
                baseRoute = url.Substring(0, index + 1);
            return $"{baseRoute}{EditPageName}";
        }
    }

    /// <summary>
    /// Navigates to the edit page with optional item ID.
    /// </summary>
    /// <param name="id">The ID of the item to edit, or <c>null</c> for creating a new item.</param>
    /// <remarks>
    /// <para>Query parameters include:</para>
    /// <list type="bullet">
    /// <item><description><c>returnUrl</c> - The current page URL (for navigation back)</description></item>
    /// <item><description>Foreign key (if <see cref="ForeginKey"/> is set)</description></item>
    /// </list>
    /// </remarks>
    private void NavigateToEditPage(string? id = null)
    {
        string? idRoute = string.Empty;
        if (id != null)
            idRoute = $"/{id}";

        var queryParams = new Dictionary<string, object?>()
        {
            ["returnUrl"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
        };
        if (ForeginKey != null)
            queryParams.Add(ForeginKey.Name, ForeginKey.Id);

        var uri = NavigationManager.GetUriWithQueryParameters($"{EditPageUri}{idRoute}", queryParams);
        NavigationManager.NavigateTo(uri);
    }

    /// <summary>
    /// Handles row double-click event.
    /// </summary>
    /// <param name="args">The row mouse event arguments.</param>
    /// <remarks>
    /// <para>If <see cref="DoubleClickRowEdit"/> is <c>true</c>, navigates to the edit page. Otherwise, invokes <see cref="OnRowClick"/>.</para>
    /// </remarks>
    async Task RowDoubleClick(DataGridRowMouseEventArgs<TItem> args)
    {
        if (args.Data != null)
        {
            if (DoubleClickRowEdit)
                NavigateToEditPage(args.Data?.Id?.ToString());
            else
                await OnRowClick.InvokeAsync(args.Data);
        }
        else
            throw new NullReferenceException("The args.Data is null");

        await Task.Yield();
    }

    TItem? itemToUpdate;

    /// <summary>
    /// Handles the Edit button click event.
    /// </summary>
    /// <param name="data">The item to edit.</param>
    /// <remarks>
    /// <para>If <see cref="InlineEdit"/> is <c>true</c>, starts inline editing. Otherwise, navigates to the edit page.</para>
    /// </remarks>
    async Task EditButtonClick(TItem data)
    {
        if (InlineEdit ?? false)
        {
            await grid.EditRow(data);
            itemToUpdate = data;
        }
        else
        {
            if (data is not null)
                NavigateToEditPage(data.Id?.ToString());
        }
    }

    /// <summary>
    /// Handles the Delete button click event.
    /// </summary>
    /// <param name="data">The item to delete.</param>
    /// <remarks>
    /// <para>Shows a confirmation dialog before deleting. If confirmed, calls <see cref="ISabatexRadzenBlazorDataAdapter.DeleteAsync{TItem, TKey}(TKey)"/>.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is <c>null</c>.</exception>
    async Task DeleteButtonClick(TItem? data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        try
        {
            if (DialogService == null)
            {
                throw new Exception("The DialogService=null sabatex.RadzenBlazor->InlineEditGridPage->GridDeleteButtonClick()");
            }
            if (await DialogService.Confirm("Ви впевнені?", "Видалення запису", new ConfirmOptions() { OkButtonText = "Так", CancelButtonText = "Ні" }) == true)
            {
                try
                {
                    await DataAdapter.DeleteAsync<TItem, TKey>(data.Id);
                    await grid.Reload();
                }
                catch (Exception e)
                {
                    NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to delete {data} with Error: {e.Message}" });
                }
            }
        }
        catch (System.Exception e)
        {
            NotificationService?.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = $"Помилка",
                Detail = $"Не можливо видалити Error:{e.Message}"
            });
        }
    }

    /// <summary>
    /// Handles the Save button click event (for inline editing).
    /// </summary>
    /// <param name="data">The item to save.</param>
    async Task SaveButtonClick(TItem data)
    {
        await OnBeforeSave.InvokeAsync(data);
        await grid.UpdateRow(data);
    }

    /// <summary>
    /// Handles the Cancel button click event (for inline editing).
    /// </summary>
    /// <param name="data">The item being edited.</param>
    async Task CancelButtonClick(TItem data)
    {
        grid.CancelEditRow(data);
        await grid.Reload();
        itemToInsert = null;
        itemToUpdate = null;
    }

    /// <summary>
    /// Handles the grid row create event (after inline row creation).
    /// </summary>
    /// <param name="item">The newly created item.</param>
    /// <remarks>
    /// <para>Calls <see cref="ISabatexRadzenBlazorDataAdapter.PostAsync{TItem, TKey}(TItem)"/> to save the item to the database.</para>
    /// </remarks>
    protected async Task gridRowCreate(TItem item)
    {
        var result = await DataAdapter.PostAsync<TItem, TKey>(item);
        if (grid != null)
            await grid.Reload();
        itemToInsert = null;
        await InvokeAsync(() => { StateHasChanged(); });
    }

    /// <summary>
    /// Handles the grid row update event (after inline row editing).
    /// </summary>
    /// <param name="item">The updated item.</param>
    /// <remarks>
    /// <para>Calls <see cref="ISabatexRadzenBlazorDataAdapter.UpdateAsync{TItem, TKey}(TItem)"/> to save changes to the database.</para>
    /// </remarks>
    protected async Task gridRowUpdate(TItem item)
    {
        await DataAdapter.UpdateAsync(item);
        itemToUpdate = null;
    }

    /// <summary>
    /// Shows an error notification if the grid reference is not initialized.
    /// </summary>
    private void GridRefNull()
    {
        NotificationService?.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = "Error",
            Detail = "The grid reference not initialize!"
        });
    }

    /// <summary>
    /// Gets whether the grid is currently busy (inserting or updating a row).
    /// </summary>
    protected bool IsGridBusy => itemToInsert != null || itemToUpdate != null;

    /// <summary>
    /// Reloads the component (forces re-render).
    /// </summary>
    public async Task Reload()
    {
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Refreshes the grid data from the API.
    /// </summary>
    /// <remarks>
    /// <para>Calls <see cref="RadzenDataGrid{TItem}.Reload"/> to reload data from the data adapter.</para>
    /// </remarks>
    public async Task RefreshAsync()
    {
        await grid.Reload();
    }

    DataGridSettings? _settings;

    /// <summary>
    /// Gets or sets the grid settings (column visibility, order, filters, etc.).
    /// </summary>
    /// <remarks>
    /// <para>Grid settings are automatically persisted to browser <c>localStorage</c> with the key <c>GridSettings{EntityName}</c>.</para>
    /// <para>This allows users to preserve their grid customizations across sessions.</para>
    /// </remarks>
    public DataGridSettings Settings
    {
        get => _settings ??= new DataGridSettings();
        set
        {
            var safeValue = value ?? new DataGridSettings();
            if (_settings != safeValue)
            {
                _settings = safeValue;
                InvokeAsync(SaveStateAsync);
            }
        }
    }

    /// <summary>
    /// Loads grid settings from browser localStorage.
    /// </summary>
    private async Task LoadStateAsync()
    {
        await Task.CompletedTask;

        var result = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", $"GridSettings{typeof(TItem).Name}");
        if (!string.IsNullOrEmpty(result))
        {
            _settings = System.Text.Json.JsonSerializer.Deserialize<DataGridSettings>(result) ?? new DataGridSettings();
        }
    }

    /// <summary>
    /// Saves grid settings to browser localStorage.
    /// </summary>
    private async Task SaveStateAsync()
    {
        await Task.CompletedTask;

        await JSRuntime.InvokeVoidAsync("window.localStorage.setItem", $"GridSettings{typeof(TItem).Name}", System.Text.Json.JsonSerializer.Serialize<DataGridSettings>(Settings));
    }

    /// <summary>
    /// Called after the component has finished rendering.
    /// </summary>
    /// <param name="firstRender">True if this is the first render, false otherwise.</param>
    /// <remarks>
    /// <para>On first render, loads grid settings from localStorage.</para>
    /// </remarks>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadStateAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles the Add button click event (navigates to edit page for creating a new item).
    /// </summary>
    protected virtual void AddButtonClickHandler()
    {
        NavigateToEditPage();
    }

    /// <summary>
    /// Edits the currently selected item (navigates to edit page).
    /// </summary>
    private void EditSelectedItem()
    {
        NavigateToEditPage(SelectedItems?.First()?.Id?.ToString());
    }

}
