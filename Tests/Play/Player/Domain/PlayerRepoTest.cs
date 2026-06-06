using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Jomolith.Play.Player.Domain;
using Shouldly;

namespace Jomolith.Tests.Play.Player.Domain;

[
    SuppressMessage(
        "Design",
        "CA1001",
        Justification = "Disposable field is disposed in last test"
    )
]
public class PlayerRepoTest : TestClass
{
    private PlayerRepo repo = null!;

    public PlayerRepoTest(Node testScene) : base(testScene)
    {
    }

    [Setup]
    public void Setup()
    {
        repo = new PlayerRepo();
    }

    [Cleanup]
    public void Cleanup()
    {
        repo.Dispose();
    }

    [Test]
    public void Initializes()
    {
        PlayerRepo r = new PlayerRepo();
        r.ShouldBeAssignableTo<IPlayerRepo>();
        r.IsPlayerRotationLocked.Value.ShouldBe(false);
        r.PlayerGlobalPosition.Value.ShouldBe(Vector3.Zero);
        r.PlayerBasis.Value.ShouldBe(Basis.Identity);
        r.CameraBasis.Value.ShouldBe(Basis.Identity);
        r.AvatarOpacity.Value.ShouldBe(1.0f);
        r.Dispose();
    }

    [Test]
    public void SetIsPlayerRotationLockedUpdatesValue()
    {
        repo.SetIsPlayerRotationLocked(true);
        repo.IsPlayerRotationLocked.Value.ShouldBe(true);

        repo.SetIsPlayerRotationLocked(false);
        repo.IsPlayerRotationLocked.Value.ShouldBe(false);
    }

    [Test]
    public void SetPlayerGlobalPositionUpdatesValue()
    {
        Vector3 position = new Vector3(1f, 2f, 3f);
        repo.SetPlayerGlobalPosition(position);
        repo.PlayerGlobalPosition.Value.ShouldBe(position);
    }

    [Test]
    public void SetPlayerBasisUpdatesValue()
    {
        Basis basis = new Basis(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f));
        repo.SetPlayerBasis(basis);
        repo.PlayerBasis.Value.ShouldBe(basis);
    }

    [Test]
    public void SetCameraBasisUpdatesValue()
    {
        Basis basis = new Basis(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f));
        repo.SetCameraBasis(basis);
        repo.CameraBasis.Value.ShouldBe(basis);
    }

    [Test]
    public void SetAvatarOpacityUpdatesValue()
    {
        repo.SetAvatarOpacity(0.5f);
        repo.AvatarOpacity.Value.ShouldBe(0.5f);

        repo.SetAvatarOpacity(0.0f);
        repo.AvatarOpacity.Value.ShouldBe(0.0f);
    }

    [Test]
    public void Disposes()
    {
        Should.NotThrow(repo.Dispose);
        // Redundant dispose shouldn't do anything.
        Should.NotThrow(repo.Dispose);
    }
}
