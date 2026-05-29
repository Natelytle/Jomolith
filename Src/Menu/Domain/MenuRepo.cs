using System;
using Chickensoft.Sync.Primitives;
using Jomolith.Menu.Screens;

namespace Jomolith.Menu.Domain;

public interface IMenuRepo
{
    public IAutoValue<IScreen?> CurrentScreen { get; }

    public void SetCurrentScreen(IScreen screen);
}

public class MenuRepo : IMenuRepo
{
    public IAutoValue<IScreen?> CurrentScreen => currentScreen;

    private AutoValue<IScreen?> currentScreen;

    private bool disposedValue;

    public MenuRepo()
    {
        currentScreen = new AutoValue<IScreen?>(null);
    }

    public void SetCurrentScreen(IScreen screen) => currentScreen.Value = screen;

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                currentScreen.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
