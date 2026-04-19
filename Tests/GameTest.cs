using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Drivers;
using Godot;
using Jomolith.Game;
using Shouldly;

namespace Jomolith.Tests;

public class GameTest : TestClass
{
    private Fixture _fixture = default!;
    private JomolithGame _game = default!;

    public GameTest(Node testScene) : base(testScene)
    {
    }

    [SetupAll]
    public async Task Setup()
    {
        _fixture = new Fixture(TestScene.GetTree());
        _game = await _fixture.LoadAndAddScene<JomolithGame>();
    }

    [CleanupAll]
    public void Cleanup()
    {
        _fixture.Cleanup();
    }

    [Test]
    public void TestButtonUpdatesCounter()
    {
    }
}