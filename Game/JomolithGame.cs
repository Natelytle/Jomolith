using Godot;
using Jomolith.Game.Domain;
using Jomolith.Game.State;
using Jomolith.Game.Menu;
using Jomolith.Game.Play.Tower;

namespace Jomolith.Game;

public partial class JomolithGame : Control
{
    public IGameRepo GameRepo { get; set; } = null!;
    public IGameLogic GameLogic { get; set; } = null!;

    public GameLogic.IBinding GameBinding { get; set; } = null!;

    [Export] public MainMenu MainMenu { get; set; } = null!;
    [Export] public Tower Tower { get; set; } = null!;

    public override void _Ready()
    {
        GameRepo = new GameRepo();
        GameLogic = new GameLogic();
        GameLogic.Set(GameRepo);

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
        }).Handle((in GameLogic.Output.UnloadCurrentTower _) =>
        {
            
        });
        
        GameLogic.Start();
    }

    public void OnPlayTower() => GameLogic.Input(new GameLogic.Input.PlayTower());
    public void OnEditTower() { }
}