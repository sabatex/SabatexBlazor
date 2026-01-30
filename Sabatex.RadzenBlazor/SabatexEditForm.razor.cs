using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Represents a generic edit form component for creating or updating entities in a Blazor application. Provides
/// customizable templates, validation, and event callbacks for handling form submission and user interactions.
/// </summary>
/// <remarks>Use this component to build data entry forms that support both creation and editing scenarios for
/// entities. The form supports customizable content rendering, pre-submit validation, and user notifications for
/// success or error conditions. The component is designed to be integrated into Blazor applications that utilize data
/// adapters and notification services. It is typically used as a base for more specialized edit forms.</remarks>
/// <typeparam name="TKey">The type of the primary key for the entity being edited.</typeparam>
/// <typeparam name="TItem">The type of the entity being edited. Must implement IEntityBase{TKey} and have a parameterless constructor.</typeparam>
public partial class SabatexEditForm<TKey,TItem>: SabatexRadzenBlazorBaseDataComponent where TItem : class, IEntityBase<TKey>,new()
{
    /// <summary>
    /// Gets or sets a value indicating whether the item is considered new.
    /// </summary>
    [Parameter]
    public bool? IsNew { get; set; }
    /// <summary>
    /// Gets or sets the URL to return to after the form is submitted.
    /// </summary>
    [Parameter]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the template used to render the content for each item.
    /// </summary>
    /// <remarks>The template receives the current item as a parameter. Use this property to customize how
    /// each item is displayed within the component.</remarks>
    [Parameter]
    public RenderFragment<TItem?> Content { get; set; } = default!;

    /// <summary>
    /// Gets or sets the data item associated with the component.
    /// </summary>
    [Parameter]
    public TItem? Item { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked before the form is submitted.
    /// </summary>
    /// <remarks>Use this callback to perform validation, modify the item, or cancel the submission process
    /// before the form data is sent. The callback receives the item being submitted as its argument.</remarks>
    [Parameter]
    public EventCallback<TItem> OnBeforeSubmit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the component is in a read-only state.
    /// </summary>
    [Parameter]
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the content to render as custom buttons within the component.
    /// </summary>
    [Parameter]
    public RenderFragment Buttons { get; set; } = default!;

    /// <summary>
    /// Gets or sets the text for the save button.
    /// </summary>
    [Parameter]
    public string TextButtonSave { get; set; } = "Save";

    /// <summary>
    /// Gets or sets the text displayed on the Cancel button.
    /// </summary>
    [Parameter]
    public string TextButtonCancel { get; set; } = "Cancel";

    /// <summary>
    /// Gets or sets the error message text to display when a write operation fails.
    /// </summary>
    [Parameter]
    public string WriteErrorText { get; set; } = "";
    
    /// <summary>
    /// 
    /// </summary>
    protected ValidationSummary? ValidationSummary;

    void ReturnToList()
    {
        if (ReturnUrl == null)
            NavigationManager.NavigateTo("/");
        else
        {
            NavigationManager.NavigateTo(ReturnUrl);
        }

    }
    /// <summary>
    /// Cancels the current operation and returns to the previous list or view.
    /// </summary>
    /// <remarks>Call this method to abort the current workflow and navigate back to the main list or overview
    /// screen. This method is typically used in response to a user-initiated cancel action.</remarks>
    protected void Cancel()
    {
        ReturnToList();
    }

    /// <summary>
    /// Handles the submission of the specified item, performing either a create or update operation as appropriate.
    /// </summary>
    /// <remarks>This method invokes pre-submit logic, attempts to persist the item using the data adapter,
    /// and provides user notifications for success or error conditions. The operation performed (create or update)
    /// depends on the state of the item. If errors occur during submission, appropriate notifications are displayed to
    /// the user.</remarks>
    /// <param name="item">The item to be submitted for creation or update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task OnSubmit(TItem item)
    {
        await OnBeforeSubmit.InvokeAsync(item);

        SabatexValidationModel<TItem>? result;

        try
        {
            if (IsNew ?? false)
                result = await DataAdapter.PostAsync<TItem, TKey>(item);
            else
                result = await DataAdapter.UpdateAsync<TItem, TKey>(item);
            if (result == null)
            {
                NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = "Server response null" });
                return;
            }

            if (result.Result != null)
            {
                ReturnToList();
                return;
            }

            if (result.Errors != null)
            {
                var error = result.Errors[""];
                if (error != null)
                {
                    string s = string.Empty;
                    foreach (var item1 in error) s = s + item1 + "\r\n";
                    NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = s });
                }

                return;
            }
            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = "Uknown Error" });
        }
        catch (Exception ex)
        {
            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = ex.Message });
        }
    }

}
