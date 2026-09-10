using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Menu.State;

[Meta]
public abstract partial record MenuState : LogicBlockState
{
    public static class Input {
        public readonly record struct TowerSelected(TowerModel Tower);
        public readonly record struct ToTowerSelect;
        public readonly record struct ToSettings;
        public readonly record struct Back;
        public readonly record struct RequestExit;
    }

    public static class Output {
        public readonly record struct ShowMainMenu;
        public readonly record struct ShowTowerSelect;
        public readonly record struct ShowSettings;
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
    public partial record MainMenu : Screen, IGet<Input.ToTowerSelect>, IGet<Input.ToSettings>, IGet<Input.Back> {
        public MainMenu() {
            this.OnEnter(() => Output(new Output.ShowMainMenu()));
        }

        public Type On(in Input.ToTowerSelect input)
        {
            Push();
            return To<TowerSelect>();
        }

        public Type On(in Input.ToSettings input)
        {
            Push();
            return To<Settings>();
        }

        public Type On(in Input.Back input)
        {
            Push();
            return To<ExitPromptOpen>();
        }
    }

    [Meta]
    public partial record TowerSelect : Screen, IGet<Input.TowerSelected>, IGet<Input.Back> {
        public TowerSelect() {
            this.OnEnter(() => Output(new Output.ShowTowerSelect()));
        }

        public Type On(in Input.TowerSelected input)
        {
            Get<IAppRepo>().OnEnteringTower(input.Tower);

            return ToSelf();
        }

        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();
    }

    [Meta]
    public partial record Settings : Screen, IGet<Input.Back> {
        public Settings() {
            this.OnEnter(() => Output(new Output.ShowSettings()));
        }

        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();
    }

    [Meta]
    public partial record ExitPromptOpen : MenuState,
        IGet<Input.Back>,
        IGet<Input.RequestExit>
    {
        public ExitPromptOpen() {
            this.OnEnter(() => Output(new Output.ExitPromptVisible(true)));
            this.OnExit(() => Output(new Output.ExitPromptVisible(false)));
        }

        public Type On(in Input.Back input) => Pop() ?? To<MainMenu>();

        public Type On(in Input.RequestExit input)
        {
            Output(new Output.QuitGame());
            return Pop() ?? To<MainMenu>();
        }
    }
}
