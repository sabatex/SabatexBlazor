using System;
using System.Collections.Generic;
using System.Text;

namespace Sabatex.RadzenBlazor;

public class ExceptionAccessDenied : Exception
{
    public ExceptionAccessDenied() : base("Access denied.")
    {
    }
}
