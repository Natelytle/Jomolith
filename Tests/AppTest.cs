using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Drivers;
using Godot;
using Jomolith.App;
using Shouldly;

namespace Jomolith.Tests;

public class GameTest : TestClass
{
    private Fixture fixture = default!;
    private JomolithApp app = default!;

    public GameTest(Node testScene) : base(testScene)
    {
    }

    [SetupAll]
    public async Task Setup()
    {
        fixture = new Fixture(TestScene.GetTree());
        app = await fixture.LoadAndAddScene<JomolithApp>();
    }

    [CleanupAll]
    public void Cleanup()
    {
        fixture.Cleanup();
    }

    [Test]
    public void TestButtonUpdatesCounter()
    {
    }
}
