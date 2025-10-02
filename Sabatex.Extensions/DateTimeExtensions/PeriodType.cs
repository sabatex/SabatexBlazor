using Sabatex.Extensions.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace sabatex.Extensions.DateTimeExtensions;

[DisplayEnum(Name ="Select period")]
public enum PeriodType
{
    [DisplayEnum(Name = "Довільний період")]
    None = 0,
    [DisplayEnum(Name = "Рік")]
    Year = 1,
    [DisplayEnum(Name = "Квартал")]
    Quarter = 2,
    [DisplayEnum(Name = "Місяць")]
    Month = 3,
    [DisplayEnum(Name = "Неділя")]
    Week = 4,
    [DisplayEnum(Name = "День")]
    Day = 5

}
