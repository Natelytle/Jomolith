using System;
using System.Collections.Generic;
using System.Linq;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Settings.Models;
using Jomolith.Settings.Services;

namespace Jomolith.Menu.Screens.Settings.State;

[Meta]
public abstract partial record SettingsState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct LoadComplete;
        public readonly record struct SelectTab(SettingsTab Tab);
        public readonly record struct SetSensitivity(float Value);
        public readonly record struct BeginRebind(string Action);
        public readonly record struct KeyCaptured(Key Key);
        public readonly record struct CancelRebind;
        public readonly record struct Save;
    }

    public static class Output
    {
        public readonly record struct SettingsLoaded(SettingsTab Tab, float Sensitivity, IReadOnlyDictionary<string, Key> Bindings);
        public readonly record struct TabChanged(SettingsTab Tab);
        public readonly record struct SensitivityChanged(float Value);
        public readonly record struct BindingChanged(string Action, Key Key);
        public readonly record struct SetRebindPromptVisible(bool Visible, string? Action);
    }

    [Meta]
    public partial record Loading : SettingsState, IGet<Input.LoadComplete>
    {
        public Loading()
        {
            this.OnEnter(() =>
            {
                var settingsDto = Get<ISettingsRepository>().Load();
                var settingsData = Get<SettingsData>();

                settingsData.CameraSensitivity = settingsDto.CameraSensitivity;
                settingsData.KeyBindings = settingsDto.KeyBindings.ToDictionary(kv => kv.Key, kv => Enum.Parse<Key>(kv.Value));

                Input(new Input.LoadComplete());
            });
        }

        public Type On(in Input.LoadComplete input)
        {
            var data = Get<SettingsData>();
            Output(new Output.SettingsLoaded(data.Tab, data.CameraSensitivity, data.KeyBindings));
            return To<Editing>();
        }
    }

    [Meta]
    public partial record Editing : SettingsState,
        IGet<Input.SelectTab>,
        IGet<Input.SetSensitivity>,
        IGet<Input.BeginRebind>,
        IGet<Input.Save>
    {
        public Editing()
        {
        }

        public Type On(in Input.SelectTab input)
        {
            Get<SettingsData>().Tab = input.Tab;
            Output(new Output.TabChanged(input.Tab));
            return ToSelf();
        }

        public Type On(in Input.SetSensitivity input)
        {
            Get<SettingsData>().CameraSensitivity = input.Value;
            Output(new Output.SensitivityChanged(input.Value));
            return ToSelf();
        }

        public Type On(in Input.BeginRebind input)
        {
            Get<SettingsData>().PendingRebindAction = input.Action;
            Push();

            Output(new Output.SetRebindPromptVisible(true, input.Action));

            return To<RebindingAction>();
        }

        public Type On(in Input.Save input)
        {
            var settingsData = Get<SettingsData>();
            Get<ISettingsRepository>().Save(new SettingsDto(
                CameraSensitivity: settingsData.CameraSensitivity,
                KeyBindings: settingsData.KeyBindings.ToDictionary(kv => kv.Key, kv => kv.Value.ToString())));

            return ToSelf();
        }
    }

    [Meta]
    public partial record RebindingAction : SettingsState, IGet<Input.KeyCaptured>, IGet<Input.CancelRebind>
    {
        public RebindingAction()
        {
            this.OnExit(() => Output(new Output.SetRebindPromptVisible(false, null)));
        }

        public Type On(in Input.KeyCaptured input)
        {
            var data = Get<SettingsData>();

            // Shouldn't be null, we just set this
            string action = data.PendingRebindAction!;
            data.KeyBindings[action] = input.Key;
            data.PendingRebindAction = null;

            Output(new Output.BindingChanged(action, input.Key));

            return Pop() ?? To<Editing>();
        }

        public Type On(in Input.CancelRebind input)
        {
            Get<SettingsData>().PendingRebindAction = null;
            return To<Editing>();
        }
    }
}
