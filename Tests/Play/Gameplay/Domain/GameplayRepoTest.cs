using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Jomolith.Play.Gameplay.Domain;
using Shouldly;

namespace Jomolith.Tests.Play.Gameplay.Domain;

[
    SuppressMessage(
        "Design",
        "CA1001",
        Justification = "Disposable field is disposed in last test"
    )
]
public class GameplayRepoTest : TestClass
{
    private GameplayRepo repo = null!;

    public GameplayRepoTest(Node testScene) : base(testScene)
    {
    }

    [Setup]
    public void Setup()
    {
        repo = new GameplayRepo();
    }

    [Cleanup]
    public void Cleanup()
    {
        repo.Dispose();
    }

    [Test]
    public void Initializes()
    {
        GameplayRepo r = new GameplayRepo();
        r.ShouldBeAssignableTo<IGameplayRepo>();
        r.IsMouseCaptured.Value.ShouldBe(false);
        r.IsPaused.Value.ShouldBe(false);
        r.Dispose();
    }

    [Test]
    public void SetIsMouseCapturedUpdatesValue()
    {
        repo.SetIsMouseCaptured(true);
        repo.IsMouseCaptured.Value.ShouldBe(true);

        repo.SetIsMouseCaptured(false);
        repo.IsMouseCaptured.Value.ShouldBe(false);
    }

    [Test]
    public void PauseSetsIsPausedTrue()
    {
        repo.IsPaused.Value.ShouldBe(false);
        repo.Pause();
        repo.IsPaused.Value.ShouldBe(true);
    }

    [Test]
    public void ResumeSetIsPausedFalse()
    {
        repo.Pause();
        repo.IsPaused.Value.ShouldBe(true);

        repo.Resume();
        repo.IsPaused.Value.ShouldBe(false);
    }

    [Test]
    public void PauseAndResumeApplyConsistently()
    {
        repo.Pause();
        repo.Pause();
        repo.IsPaused.Value.ShouldBe(true);

        repo.Resume();
        repo.Resume();
        repo.IsPaused.Value.ShouldBe(false);
    }

    [Test]
    public void Disposes()
    {
        Should.NotThrow(repo.Dispose);
        // Redundant dispose shouldn't do anything.
        Should.NotThrow(repo.Dispose);
    }
}
