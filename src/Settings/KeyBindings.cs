using System.Collections.Generic;
using Godot;

namespace Jomolith.Settings;

public class KeyBindings : Dictionary<string, Key>
{
    public static readonly KeyBindings DEFAULT = new()
    {
        ["move_forward"] = Key.W,
        ["move_back"] = Key.S,
        ["move_left"] = Key.A,
        ["move_right"] = Key.D,
        ["jump"] = Key.Space,
        ["shift_lock"] = Key.Shift,
        ["toggle_noclip"] = Key.F,
        ["move_up"] = Key.Space,
        ["move_down"] = Key.Ctrl
    };
}
