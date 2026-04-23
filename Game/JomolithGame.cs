using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Game.State;
using Jomolith.Game.Menu;

namespace Jomolith.Game;

public interface IJomolithGame : IControl, IProvide<IGameRepo>;

[Meta(typeof(IAutoNode))]
public partial class JomolithGame : Control, IJomolithGame
{
    public override void _Notification(int what) => this.Notify(what);

    #region Provisions

    IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

    #endregion

    public IGameRepo GameRepo { get; set; } = null!;
    public IGameLogic GameLogic { get; set; } = null!;

    public GameLogic.IBinding GameBinding { get; set; } = null!;

    [Node] public IMainMenu MainMenu { get; set; } = null!;
    [Node] public ISubViewport TowerPreview { get; set; } = null!;

    public void Setup()
    {
        GameRepo = new GameRepo();
        GameLogic = new GameLogic();
        
        GameLogic.Set(GameRepo);
    }

    public void OnResolved()
    {
        MainMenu.PlayTower += OnPlayTower;
        MainMenu.EditTower += OnEditTower;

        GameBinding = GameLogic.Bind();

        GameBinding.Handle((in GameLogic.Output.ShowMainMenu _) =>
        {
            MainMenu.Show();
        }).Handle((in GameLogic.Output.StartLoadingTower _) =>
        {
            GameLogic.Input(new GameLogic.Input.TowerLoaded());
        }).Handle((in GameLogic.Output.EnterTower _) =>
        {
            MainMenu.Hide();
            GameRepo.OnEnterTower();
        }).Handle((in GameLogic.Output.UnloadCurrentTower _) =>
        {
            
        });
        
        this.Provide();
        
        GameLogic.Start();
    }

    public void OnReady()
    {
        
    }

    public void OnPlayTower() => GameLogic.Input(new GameLogic.Input.PlayTower());
    public void OnEditTower() { }
}