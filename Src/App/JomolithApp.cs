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

    [Node] public IMainMenu MainMenu { get; set; } = null!;
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
        MainMenu.PlayTower += OnPlayTower;
        MainMenu.EditTower += OnEditTower;

        AppBinding = AppLogic.Bind();

        AppBinding.Handle((in AppLogic.Output.ShowMainMenu _) =>
        {
            MainMenu.Show();
        }).Handle((in AppLogic.Output.StartLoadingTower _) =>
        {
            AppLogic.Input(new AppLogic.Input.TowerLoaded());
        }).Handle((in AppLogic.Output.EnterTower _) =>
        {
            MainMenu.Hide();
            AppRepo.OnEnterTower();
        }).Handle((in AppLogic.Output.UnloadCurrentTower _) =>
        {
        });

        this.Provide();

        AppLogic.Start();
    }

    public void OnReady()
    {
    }

    public void OnPlayTower() => AppLogic.Input(new AppLogic.Input.PlayTower());
    public void OnEditTower() { }
}
