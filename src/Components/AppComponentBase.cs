using Microsoft.AspNetCore.Components;

namespace MudBlazorRokPaperScissors.Components;

/// <summary>
/// Base class for components that need to be disposable.
/// </summary>
public class AppComponenDisposableBase : ComponentBase, IDisposable
{
    /// <summary>
    /// override this method to perform cleanup operations before the component is disposed.
    /// </summary>
    public virtual void Dispose()
    {        
    }
}

