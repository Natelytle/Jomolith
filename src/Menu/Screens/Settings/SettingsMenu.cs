using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.Screens.Settings.State;
using Jomolith.Menu.State;
using Jomolith.Settings.Domain.Models;
using Jomolith.Settings.Services;

namespace Jomolith.Menu.Screens.Settings;

[Meta(typeof(IAutoNode))]
public partial class SettingsMenu : Control, IScreen
{
    public override void _Notification(int what) => this.Notify(what);

    private ISettingsRepository settingsRepository = null!;

    [Dependency]
    private GameplaySettings gameplaySettings => this.DependOn<GameplaySettings>();

    [Node("%Tabs")]
    private ITabContainer tabs { get; set; } = null!;

    [Node("%SensitivitySlider")]
    private IHSlider sensitivitySlider { get; set; } = null!;

    [Node("%SensitivityBox")]
    private ISpinBox sensitivityBox { get; set; } = null!;

    [Node("%BindingsList")]
    private IVBoxContainer bindingsList { get; set; } = null!;

    [Node("%RebindPrompt")]
    private IControl rebindPrompt { get; set; } = null!;

    public bool ShowFooter => true;

    private SettingsLogic settingsLogic = null!;

    public void Setup()
    {
        settingsRepository = new LocalSettingsRepository();
        settingsLogic = new SettingsLogic();
    }

    public void OnResolved()
    {
        settingsLogic.Set(settingsRepository);
        settingsLogic.Set(gameplaySettings);

        settingsLogic.Bind()
            .OnOutput<SettingsState.Output.SettingsLoaded>((in o) =>
            {
                tabs.SetCurrentTab((int)o.Tab);
                setSensitivityDisplay(o.Sensitivity);
                rebuildBindingsList(o.Bindings);
            })
            .OnOutput<SettingsState.Output.BindingChanged>((in o) =>
            {
                updateBindingRow(o.Action, o.Key);
                applyToInputMap(o.Action, o.Key);
            })
            .OnOutput<SettingsState.Output.SensitivityChanged>((in o) =>
            {
                setSensitivityDisplay(o.Value);
            })
            .OnOutput<SettingsState.Output.TabChanged>((in o) => tabs.SetCurrentTab((int)o.Tab))
            .OnOutput<SettingsState.Output.SetRebindPromptVisible>((in o) => rebindPrompt.Visible = o.Visible);

        tabs.TabChanged += index => settingsLogic.Input(new SettingsState.Input.SelectTab((SettingsTab)index));
        sensitivitySlider.ValueChanged += value => onSensitivityChanged((float)value);
        sensitivityBox.ValueChanged += value => onSensitivityChanged((float)value);

        settingsLogic.Start<SettingsState.Loading>();
    }

    public void OnEnter() { }

    public void OnExit()
    {
        settingsLogic.Input(new SettingsState.Input.Save());
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyPress || !rebindPrompt.Visible)
            return;

        if (keyPress.Keycode == Key.Escape)
            settingsLogic.Input(new SettingsState.Input.CancelRebind());
        else
            settingsLogic.Input(new SettingsState.Input.KeyCaptured(keyPress.Keycode));

        GetViewport().SetInputAsHandled();
    }

    private void rebuildBindingsList(IReadOnlyDictionary<string, Key> bindingsDict)
    {
        foreach (var child in bindingsList.GetChildren())
            child.QueueFree();

        foreach (var (action, key) in bindingsDict)
        {
            HBoxContainer row = new HBoxContainer { Name = action };
            Label label = new Label { Text = action };
            label.AddThemeFontSizeOverride("font_size", 24);
            row.AddChild(label);
            Button keyButton = new Button { Text = key.ToString(), Name = "KeyButton" };
            keyButton.Pressed += () => settingsLogic.Input(new SettingsState.Input.BeginRebind(action));
            keyButton.AddThemeFontSizeOverride("font_size", 24);
            row.AddChild(keyButton);
            bindingsList.AddChildEx(row);
        }
    }

    private void updateBindingRow(string action, Key key)
    {
        if (bindingsList.GetNodeOrNull<HBoxContainer>(action).GetNodeOrNull<Button>("KeyButton") is {} button)
        {
            button.Text = key.ToString();
        }
    }

    private static void applyToInputMap(string action, Key key)
    {
        InputMap.ActionEraseEvents(action);
        InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = key });
    }

    private void setSensitivityDisplay(float value)
    {
        sensitivitySlider.SetValueNoSignal(value);
        sensitivityBox.SetValueNoSignal(value);
    }

    private void onSensitivityChanged(float value) => settingsLogic.Input(new SettingsState.Input.SetSensitivity(value));
}
