using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

public partial record MenuState
{
    [Meta]
    public partial record InMainMenu : MenuState,
        IGet<MenuLogic.Inputs.MainMenu.PlayButtonPressed>,
        IGet<MenuLogic.Inputs.MainMenu.EditButtonPressed>,
        IGet<MenuLogic.Inputs.MainMenu.SettingsButtonPressed>
    {
        public InMainMenu()
        {
            this.OnEnter(() =>
            {
                Push();
                Output(new MenuLogic.Outputs.ShowMainMenu());
            });
        }

        public Type On(in MenuLogic.Inputs.MainMenu.PlayButtonPressed input)
        {
            return To<InTowerSelect>();
        }

        public Type On(in MenuLogic.Inputs.MainMenu.EditButtonPressed input)
        {
            return To<InEditor>();
        }

        public Type On(in MenuLogic.Inputs.MainMenu.SettingsButtonPressed input)
        {
            return To<InSettings>();
        }
    }
}
