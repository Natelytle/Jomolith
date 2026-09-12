using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Settings;

namespace Jomolith.App.State;

[Meta]
public abstract partial record AppState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct ToMenus;
        public readonly record struct ToGameplay;
    }

    public static class Output
    {
        public readonly record struct SetGameVisibility(bool Visible);
        public readonly record struct SetMenuVisibility(bool Visible);
    }

    [Meta]
    public partial record InMenus : AppState, IGet<Input.ToGameplay>
    {
        public InMenus()
        {
            this.OnEnter(() =>
            {
                Output(new Output.SetMenuVisibility(true));
            });

            this.OnExit(() =>
            {
                Output(new Output.SetMenuVisibility(false));
            });
        }

        public void OnEnteringTower() => Input(new Input.ToGameplay());

        public Type On(in Input.ToGameplay input) => To<InGameplay>();
    }

    [Meta]
    public partial record InGameplay : AppState, IGet<Input.ToMenus>
    {
        public InGameplay()
        {
            this.OnEnter(() =>
            {
                Output(new Output.SetGameVisibility(true));
            });

            this.OnExit(() =>
            {
                Output(new Output.SetGameVisibility(false));
            });
        }

        public void OnExitingTower() => Input(new Input.ToMenus());

        public Type On(in Input.ToMenus input) => To<InMenus>();
    }
}
