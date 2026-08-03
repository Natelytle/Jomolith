using System;
using System.Numerics;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
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
        public readonly record struct Pause;
    }

    public static class Output
    {
        public readonly record struct Load(TowerModel Tower);
        public readonly record struct Unload;
        public readonly record struct SpawnPlayer(Vector3 SpawnPosition);
        public readonly record struct SetMouseCaptureMode(bool Captured);
        public readonly record struct SetPaused(bool Paused);
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
    public partial record Playing : GameplayState
    {
        public void OnPaused()
        {

        }
    }

    [Meta]
    public partial record Paused : GameplayState
    {

    }
}
