using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Services;

namespace Jomolith.Menu.Screens.Select.State;

[Meta]
public abstract partial record TowerSelectState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct Reload;
        public readonly record struct LoadComplete;
        public readonly record struct Select(int Index);
        public readonly record struct Confirm;
    }

    public static class Output
    {
        public readonly record struct TowersLoaded(IReadOnlyList<TowerModel> Towers);
        public readonly record struct SelectionChanged(TowerModel Tower);
        public readonly record struct TowerConfirmed(TowerModel Tower);
    }

    [Meta]
    public partial record Loading : TowerSelectState, IGet<Input.LoadComplete>
    {
        public Loading()
        {
            this.OnEnter(() =>
            {
                Get<TowerSelectionData>().Towers = Get<ITowerRepository>().LoadAllTowers();
                Input(new Input.LoadComplete());
            });
        }

        public Type On(in Input.LoadComplete input) => To<Browsing>();
    }

    [Meta]
    public partial record Browsing : TowerSelectState, IGet<Input.Reload>, IGet<Input.Select>, IGet<Input.Confirm>
    {
        public Browsing()
        {
            this.OnEnter(() =>
            {
                var data = Get<TowerSelectionData>();
                Output(new Output.TowersLoaded(data.Towers));
                if (data.Towers.Count > 0) Input(new Input.Select(0));
            });
        }

        public Type On(in Input.Select input)
        {
            var data = Get<TowerSelectionData>();

            if (input.Index < 0 || input.Index >= data.Towers.Count)
                return ToSelf();

            data.SelectedIndex = input.Index;
            Output(new Output.SelectionChanged(data.Towers[data.SelectedIndex]));
            return ToSelf();
        }

        public Type On(in Input.Confirm input)
        {
            var data = Get<TowerSelectionData>();

            if (data.SelectedIndex >= 0)
                Output(new Output.TowerConfirmed(data.Towers[data.SelectedIndex]));

            return ToSelf();
        }

        public Type On(in Input.Reload input)
        {
            return To<Loading>();
        }
    }
}
