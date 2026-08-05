using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.Screens.Select.State;
using Jomolith.Menu.State;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Factory;
using Jomolith.Towers.Services;

namespace Jomolith.Menu.Screens.Select;

[Meta(typeof(IAutoNode))]
public partial class TowerSelect : Control, IScreen
{
    public override void _Notification(int what) => this.Notify(what);

    private ITowerRepository towerRepository = null!;

    public event TowerSelectedEventHandler? TowerSelected;
    public delegate void TowerSelectedEventHandler(TowerModel tower);

    [Node("%TowerList")]
    private IItemList towerList { get; set; } = null!;

    [Node("%TitleLabel")]
    private ILabel titleLabel { get; set; } = null!;

    [Node("%CreatorLabel")]
    private ILabel creatorLabel { get; set; } = null!;

    [Node("%DifficultyLabel")]
    private ILabel difficultyLabel { get; set; } = null!;

    [Node("%PreviewNode")]
    private INode3D previewNode { get; set; } = null!;

    [Node("%PlayButton")]
    private IButton playButton { get; set; } = null!;

    public bool ShowFooter => true;

    private TowerSelectLogic selectLogic = null!;
    private readonly TowerBuilder towerBuilder = new();
    private Node3D? currentPreview;

    public void Setup()
    {
        selectLogic = new TowerSelectLogic();
        towerRepository = new LocalTowerRepository();
    }

    public void OnResolved()
    {
        selectLogic.Set(towerRepository);

        selectLogic.Bind()
            .OnOutput<TowerSelectState.Output.TowersLoaded>((in o) => populateList(o.Towers))
            .OnOutput<TowerSelectState.Output.SelectionChanged>((in o) => showPreview(o.Tower))
            .OnOutput<TowerSelectState.Output.TowerConfirmed>((in o) => TowerSelected?.Invoke(o.Tower));

        towerList.ItemSelected += index => selectLogic.Input(new TowerSelectState.Input.Select((int)index));
        playButton.Pressed += () => selectLogic.Input(new TowerSelectState.Input.Confirm());

        selectLogic.Start<TowerSelectState.Loading>();
    }

    public void OnEnter()
    {
        selectLogic.Input(new TowerSelectState.Input.Reload());
    }

    public void OnExit() { }

    private void populateList(System.Collections.Generic.IReadOnlyList<TowerModel> towers)
    {
        towerList.Clear();

        foreach (var tower in towers)
            towerList.AddItem(tower.Name);
    }

    private void showPreview(TowerModel tower)
    {
        currentPreview?.QueueFree();
        currentPreview = towerBuilder.BuildTower(tower, isPreview: true);
        previewNode.AddChildEx(currentPreview);

        titleLabel.Text = tower.Name;
        creatorLabel.Text = $"By: {tower.Creator}";
        difficultyLabel.Text = $"Difficulty: {tower.Difficulty, 2}";
    }
}
