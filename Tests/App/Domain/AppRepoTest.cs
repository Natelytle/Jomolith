using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Jomolith.App.Domain;
using Shouldly;

namespace Jomolith.Tests.App.Domain;

[
    SuppressMessage(
        "Design",
        "CA1001",
        Justification = "Disposable field is disposed in last test"
    )
]
public class AppRepoTest : TestClass
{
    private AppRepo repo = null!;

    public AppRepoTest(Node testScene) : base(testScene)
    {
    }

    [Setup]
    public void Setup()
    {
        repo = new AppRepo();
    }

    [Cleanup]
    public void Cleanup()
    {
        repo.Dispose();
    }

    [Test]
    public void Initializes()
    {
        AppRepo r = new AppRepo();
        r.ShouldBeAssignableTo<IAppRepo>();
        r.Dispose();
    }

    [Test]
    public void OnMainMenuEnteredInvokesEvent()
    {
        int called = 0;

        // Invoke without handlers to cover null check.
        repo.OnMainMenuEntered();

        repo.AutoChannel.Bind().On((in IAppRepo.MainMenuEntered _) => called++);
        repo.OnMainMenuEntered();

        called.ShouldBe(1);
    }

    [Test]
    public void OnEnterTowerInvokesEvent()
    {
        int called = 0;

        // Invoke without handlers to cover null check.
        repo.OnEnterTower();

        repo.AutoChannel.Bind().On((in IAppRepo.TowerEntered _) => called++);
        repo.OnEnterTower();

        called.ShouldBe(1);
    }

    [Test]
    public void Disposes()
    {
        Should.NotThrow(repo.Dispose);
        // Redundant dispose shouldn't do anything.
        Should.NotThrow(repo.Dispose);
    }
}
