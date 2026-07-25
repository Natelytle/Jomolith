using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Menu.State;

[Meta]
public abstract partial record MenuState : LogicBlockState
{
    public static class Input {
        public readonly record struct ToPlay(TowerModel Tower);
        public readonly record struct ToTowerSelect;
        public readonly record struct ToSettings;
        public readonly record struct Back;
        public readonly record struct RequestExit;
        public readonly record struct ExitCancelled;
        public readonly record struct ExitConfirmed;
    }

    public static class Output {
        public readonly record struct ScreenChanged(Type NewScreen, bool CanGoBack);
        public readonly record struct ExitPromptVisible(bool Visible);
        public readonly record struct QuitGame;
    }

    [Meta]
    public partial record Screen : MenuState, IGet<Input.RequestExit> {
        public Type On(in Input.RequestExit input) {
            Push();
            return To<ExitPromptOpen>();
        }
    }

    [Meta]
    public partial record MainMenu : Screen, IGet<Input.ToTowerSelect>, IGet<Input.ToSettings> {
        public MainMenu() {
            this.OnEnter(() => Output(new Output.ScreenChanged(typeof(MainMenu), CanGoBack: false)));
        }
        public Type On(in Input.ToTowerSelect input) { Push(); return To<TowerSelect>(); }
        public Type On(in Input.ToSettings input) { Push(); return To<Settings>(); }
    }

    [Meta]
    public partial record Play : Screen, IGet<Input.Back> {
        public Play() {
            this.OnEnter(() => Output(new Output.ScreenChanged(typeof(Play), CanGoBack: true)));
        }
        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();
    }

    [Meta]
    public partial record TowerSelect : Screen, IGet<Input.ToPlay>, IGet<Input.Back> {
        public TowerSelect() {
            this.OnEnter(() => Output(new Output.ScreenChanged(typeof(TowerSelect), CanGoBack: true)));
        }
        public Type On(in Input.ToPlay input) { Push(); return To<Play>(); }
        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();
    }

    [Meta]
    public partial record Settings : Screen, IGet<Input.Back> {
        public Settings() {
            this.OnEnter(() => Output(new Output.ScreenChanged(typeof(Settings), CanGoBack: true)));
        }
        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();
    }

    [Meta]
    public partial record ExitPromptOpen : MenuState,
        IGet<Input.ExitCancelled>,
        IGet<Input.ExitConfirmed>
    {
        public ExitPromptOpen() {
            this.OnEnter(() => Output(new Output.ExitPromptVisible(true)));
            this.OnExit(() => Output(new Output.ExitPromptVisible(false)));
        }

        public Type On(in Input.ExitCancelled input) => Pop() ?? To<MainMenu>();

        public Type On(in Input.ExitConfirmed input)
        {
            Output(new Output.QuitGame());
            return Pop() ?? To<MainMenu>();
        }
    }
}
