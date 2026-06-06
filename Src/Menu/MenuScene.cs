using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Menu.Domain;
using Jomolith.Menu.Screens.MainMenu;
using Jomolith.Menu.Screens.SettingsMenu;
using Jomolith.Menu.Screens.TowerSelect;
using Jomolith.Menu.State;
using Jomolith.Menu.State.States;

namespace Jomolith.Menu;

public interface IMenuScene : IControl, IProvide<IMenuRepo>
{
}

[Meta(typeof(IAutoNode))]
public partial class MenuScene : Control, IMenuScene
{
    public override void _Notification(int what) => this.Notify(what);

    #region Provisions

    IMenuRepo IProvide<IMenuRepo>.Value() => MenuRepo;

    #endregion

    #region Dependencies

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

    #endregion

    #region State

    public IMenuLogic MenuLogic { get; set; } = null!;
    public IMenuRepo MenuRepo { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public IMainMenu MainMenu { get; set; } = null!;
    [Node] public ITowerSelect TowerSelect { get; set; } = null!;
    [Node] public ISettingsMenu SettingsMenu { get; set; } = null!;

    #endregion

    public void Setup()
    {
        MenuRepo = new MenuRepo();
        MenuLogic = new MenuLogic();

        MenuLogic.Set(AppRepo);
        MenuLogic.Set(MenuRepo);
    }

    public void OnResolved()
    {
        using var binding = MenuLogic.Bind();

        binding
            .OnOutput((in MenuLogic.Outputs.ShowMainMenu _) =>
            {
                hideAllMenus();
                MainMenu.Show();

                MenuRepo.SetCurrentScreen(MainMenu);
            }).OnOutput((in MenuLogic.Outputs.ShowTowerSelect _) =>
            {
                hideAllMenus();
                TowerSelect.Show();

                MenuRepo.SetCurrentScreen(TowerSelect);
            }).OnOutput((in MenuLogic.Outputs.ShowSettingsMenu _) =>
            {
                hideAllMenus();
                SettingsMenu.Show();

                MenuRepo.SetCurrentScreen(SettingsMenu);
            });

        this.Provide();

        MenuLogic.Start<MenuState.Default>();
    }

    private void hideAllMenus()
    {
        MainMenu.Hide();
        TowerSelect.Hide();
        SettingsMenu.Hide();
    }
}
