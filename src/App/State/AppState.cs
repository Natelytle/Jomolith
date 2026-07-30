using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.App.State;

[Meta]
public abstract partial record AppState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct ToGameplay(TowerModel Tower);
        public readonly record struct ToMenus;
    }

    public static class Output
    {
        public readonly record struct ShowGame;
        public readonly record struct HideGame;
    }

    [Meta]
    public partial record InMenus : AppState, IGet<Input.ToGameplay>
    {
        public void OnTowerEntered(TowerModel tower) => Input(new Input.ToGameplay(tower));

        public Type On(in Input.ToGameplay input) => To<InGameplay>();
    }

    public partial record InGameplay : AppState, IGet<Input.ToMenus>
    {
        public InGameplay()
        {
            this.OnEnter(() =>
            {
                Output(new Output.ShowGame());
            });

            this.OnExit(() =>
            {
                Output(new Output.HideGame());
            });
        }

        public void OnTowerExited() => Input(new Input.ToMenus());

        public Type On(in Input.ToMenus input) => To<InMenus>();
    }
}
