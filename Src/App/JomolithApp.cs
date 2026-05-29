using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.UMLGenerator;
using Godot;
using Jomolith.App.Domain;
using Jomolith.App.State;
using Jomolith.Menu;
using Jomolith.Play.Gameplay;

namespace Jomolith.App;

public interface IJomolithApp : IControl, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode)), ClassDiagram]
public partial class JomolithApp : Control, IJomolithApp
{
    public override void _Notification(int what) => this.Notify(what);

    #region Provisions

    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

    #endregion

    #region State

    public IAppRepo AppRepo { get; set; } = null!;
    public IAppLogic AppLogic { get; set; } = null!;

    public AppLogic.IBinding AppBinding { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public IMenuScene MenuScene { get; set; } = null!;
    [Node] public ISubViewport GameplayPreview { get; set; } = null!;
    [Node] public IGameplay Gameplay { get; set; } = null!;

    #endregion

    public void Setup()
    {
        AppRepo = new AppRepo();
        AppLogic = new AppLogic();

        AppLogic.Set(AppRepo);
    }

    public void OnResolved()
    {
        AppBinding = AppLogic.Bind();

        this.Provide();

        AppLogic.Start();
    }

    public void OnReady()
    {
    }
}
