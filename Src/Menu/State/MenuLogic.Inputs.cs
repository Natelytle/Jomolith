namespace Jomolith.Menu.State;

public partial class MenuLogic
{
    public static class Inputs
    {
        public static class MainMenu
        {
            public readonly record struct PlayButtonPressed;
            public readonly record struct EditButtonPressed;
            public readonly record struct SettingsButtonPressed;
        }

        public static class Global
        {
            public readonly record struct BackButtonPressed;
        }
    }
}
