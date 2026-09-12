using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Settings;

namespace Jomolith.Menu.Screens.Settings.State;

[Meta]
public abstract partial record SettingsState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct SettingDisplayComplete;
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
    public partial record Loading : SettingsState, IGet<Input.SettingDisplayComplete>
    {
        public Loading()
        {
            this.OnEnter(() =>
            {
                var settings = Get<ISettingsService>().Load();
                var data = Get<SettingsMenuData>();

                data.SettingsCache = settings;

                Output(new Output.SettingsLoaded(data.Tab, settings.CameraSensitivity, settings.KeyBindings));
            });
        }

        public Type On(in Input.SettingDisplayComplete input)
        {
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
        public Type On(in Input.SelectTab input)
        {
            Get<SettingsMenuData>().Tab = input.Tab;
            Output(new Output.TabChanged(input.Tab));
            return ToSelf();
        }

        public Type On(in Input.SetSensitivity input)
        {
            Get<SettingsMenuData>().SettingsCache.CameraSensitivity = input.Value;
            Output(new Output.SensitivityChanged(input.Value));
            return ToSelf();
        }

        public Type On(in Input.BeginRebind input)
        {
            Get<SettingsMenuData>().PendingRebindAction = input.Action;
            Push();

            Output(new Output.SetRebindPromptVisible(true, input.Action));

            return To<RebindingAction>();
        }

        public Type On(in Input.Save input)
        {
            var settingsData = Get<SettingsMenuData>();

            Get<ISettingsService>().Save(settingsData.SettingsCache);

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
            var data = Get<SettingsMenuData>();

            // Shouldn't be null, we just set this
            string action = data.PendingRebindAction!;
            data.SettingsCache.KeyBindings[action] = input.Key;
            data.PendingRebindAction = null;

            Output(new Output.BindingChanged(action, input.Key));

            return Pop() ?? To<Editing>();
        }

        public Type On(in Input.CancelRebind input)
        {
            Get<SettingsMenuData>().PendingRebindAction = null;
            return To<Editing>();
        }
    }
}
