using System.ComponentModel.DataAnnotations;

namespace RadzenBlazorDemo.Client.Models;

public enum DaysOfWeek
{
    [Display(Description = "Sunday")]
    Sunday = 0,

    [Display(Description = "Monday")]
    Monday = 1,

    [Display(Description = "Tuesday")]
    Tuesday = 2,

    [Display(Description = "Wednesday")]
    Wednesday = 3,

    [Display(Description = "Thursday")]
    Thursday = 4,

    [Display(Description = "Friday")]
    Friday = 5,

    [Display(Description = "Saturday")]
    Saturday = 6
}
