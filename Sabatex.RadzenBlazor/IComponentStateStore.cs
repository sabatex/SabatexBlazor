using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public interface IComponentStateStore
{
    Task SaveStateAsync<TState>(string key, TState state);
    Task<TState?> LoadStateAsync<TState>(string key);
}
