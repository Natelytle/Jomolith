using Chickensoft.GameTools.Displays;
using Godot;

namespace Jomolith;

#if RUN_TESTS
using System.Reflection;
using Chickensoft.GoDotTest;
using Chickensoft.GodotNodeInterfaces;
#endif

// This entry-point file is responsible for determining if we should run tests.
//
// If you want to edit your game's main entry-point, please see Game.tscn and
// Game.cs instead.

public partial class Main : Node2D
{
#if RUN_TESTS
    public TestEnvironment Environment = default!;
#endif
    public Vector2I DesignResolution => Display.UHD4k;

    public override void _Ready()
    {
        // Correct any erroneous scaling and guess sensible defaults.
        GetWindow().LookGood(WindowScaleBehavior.UIFixed, DesignResolution);

#if RUN_TESTS
        // If this is a debug build, use GoDotTest to examine the
        // command line arguments and determine if we should run tests.
        Environment = TestEnvironment.From(OS.GetCmdlineArgs());
        if (Environment.ShouldRunTests)
        {
            RuntimeContext.IsTesting = true;
            CallDeferred("RunTests");
            return;
        }
#endif

        // If we don't need to run tests, we can just switch to the game scene.
        CallDeferred("RunScene");
    }

#if RUN_TESTS
    // ReSharper disable once InconsistentNaming
    private void RunTests()
    {
        _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);
    }
#endif

    // ReSharper disable once InconsistentNaming
    private void RunScene()
    {
        GetTree().ChangeSceneToFile("res://Src/Game/JomolithGame.tscn");
    }
}