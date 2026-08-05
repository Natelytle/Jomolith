using System;
using System.Numerics;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;
using Jomolith.Gameplay.Domain;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Gameplay.State;

[Meta]
public abstract partial record GameplayState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct BeginLoading;
        public readonly record struct LoadComplete;
        public readonly record struct TogglePause;
        public readonly record struct ExitGameplay;
        public readonly record struct ExitComplete;
    }

    public static class Output
    {
        public readonly record struct Load(TowerModel Tower);
        public readonly record struct Unload;
        public readonly record struct SpawnPlayer(Vector3 SpawnPosition);
        public readonly record struct SetMouseCaptureMode(bool Captured);
        public readonly record struct SetPaused(bool IsPaused);
    }

    [Meta]
    public partial record Unloaded : GameplayState, IGet<Input.BeginLoading>
    {
        public void OnTowerEntered(TowerModel tower)
        {
            Get<GameplayData>().CurrentTower = tower;
            Input(new Input.BeginLoading());
        }

        public Type On(in Input.BeginLoading input) => To<Loading>();
    }

    [Meta]
    public partial record Loading : GameplayState, IGet<Input.LoadComplete>
    {
        public Loading()
        {
            this.OnEnter(() =>
            {
                var d = Get<GameplayData>();
                Output(new Output.Load(d.CurrentTower!));
            });
        }

        public Type On(in Input.LoadComplete input)
        {
            Get<IGameplayRepo>().OnGameplayStarted();

            return To<Playing>();
        }
    }

    [Meta]
    public partial record Playing : GameplayState, IGet<Input.TogglePause>
    {
        public Type On(in Input.TogglePause input)
        {
            return To<Paused>();
        }
    }

    [Meta]
    public partial record Paused : GameplayState, IGet<Input.TogglePause>, IGet<Input.ExitGameplay>
    {
        public Paused()
        {
            this.OnEnter(() =>
            {
                Output(new Output.SetPaused(true));
            });
            this.OnExit(() =>
            {
                Output(new Output.SetPaused(false));
            });
        }

        public Type On(in Input.TogglePause input)
        {
            return To<Playing>();
        }

        public Type On(in Input.ExitGameplay input)
        {
            return To<Quit>();
        }
    }

    [Meta]
    public partial record Quit : GameplayState, IGet<Input.ExitComplete>
    {
        public Quit()
        {
            this.OnEnter(() =>
            {
                Output(new Output.Unload());

                Input(new Input.ExitComplete());
            });
        }

        public Type On(in Input.ExitComplete input)
        {
            Get<IAppRepo>().OnExitingTower();

            return To<Unloaded>();
        }
    }
}
