namespace Jomolith.Menu.State;

public partial class MenuLogic
{
    public static class Output
    {
        public readonly record struct ShowMainMenu;

        public readonly record struct ShowTowerSelect;

        public readonly record struct ShowSettingsMenu;

        public readonly record struct TowerSelected;
    }
}
