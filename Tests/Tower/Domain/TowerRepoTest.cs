using System;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
using Jomolith.Tower.Core;
using Jomolith.Tower.Core.Objects;
using Jomolith.Tower.Domain;
using LightMock.Generator;
using LightMoq;
using Shouldly;

namespace Jomolith.Tests.Tower.Domain;

[
    SuppressMessage(
        "Design",
        "CA1001",
        Justification = "Disposable field is disposed in last test"
    )
]
public class TowerRepoTest : TestClass
{
    private TowerRepo repo = null!;
    private TowerSceneModel scene = null!;
    private Mock<ITowerModel> towerMock = null!;

    public TowerRepoTest(Node testScene)
        : base(testScene)
    {
    }

    [Setup]
    public void Setup()
    {
        repo = new TowerRepo();
        scene = new TowerSceneModel();
        towerMock = new Mock<ITowerModel>();
        towerMock.Setup(t => t.Scene).Returns(scene);
    }

    [Cleanup]
    public void Cleanup()
    {
        repo.Dispose();
    }

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    [Test]
    public void Initializes()
    {
        TowerRepo r = new TowerRepo();
        r.ShouldBeAssignableTo<IEditableTowerRepo>();
        r.CurrentTower.Value.ShouldBeNull();
        r.Dispose();
    }

    // -------------------------------------------------------------------------
    // LoadTower / UnloadTower
    // -------------------------------------------------------------------------

    [Test]
    public void LoadTowerSetsTower()
    {
        repo.LoadTower(towerMock.Object);
        repo.CurrentTower.Value.ShouldBe(towerMock.Object);
    }

    [Test]
    public void UnloadTowerClearsTower()
    {
        repo.LoadTower(towerMock.Object);
        repo.UnloadTower();
        repo.CurrentTower.Value.ShouldBeNull();
    }

    // -------------------------------------------------------------------------
    // AddObject
    // -------------------------------------------------------------------------

    [Test]
    public void AddObjectDoesNothingWhenNoTowerLoaded()
    {
        PartModel model = new PartModel();
        bool called = false;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectAdded _) => called = true);
        repo.AddObject(model);

        called.ShouldBe(false);
    }

    [Test]
    public void AddObjectAddsToSceneAndSendsEvent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel model = new PartModel();
        bool called = false;
        ITowerRepo.ObjectAdded received = default;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectAdded e) =>
        {
            called = true;
            received = e;
        });

        repo.AddObject(model);

        called.ShouldBe(true);
        received.Model.ShouldBe(model);
        scene.TowerObjects.ContainsKey(model.Id).ShouldBeTrue();
    }

    [Test]
    public void AddObjectWithParentIdUsesParent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel parent = new PartModel();
        PartModel child = new PartModel();

        repo.AddObject(parent);
        repo.AddObject(child, parent.Id);

        scene.GetParent(child.Id).ShouldBe(parent.Id);
    }

    [Test]
    public void AddObjectWithNoParentDefaultsToRoot()
    {
        repo.LoadTower(towerMock.Object);

        PartModel model = new PartModel();
        repo.AddObject(model);

        scene.GetParent(model.Id).ShouldBe(scene.RootId);
    }

    // -------------------------------------------------------------------------
    // RemoveObject
    // -------------------------------------------------------------------------

    [Test]
    public void RemoveObjectDoesNothingWhenNoTowerLoaded()
    {
        bool called = false;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectRemoved _) => called = true);
        repo.RemoveObject(Guid.NewGuid());

        called.ShouldBe(false);
    }

    [Test]
    public void RemoveObjectRemovesFromSceneAndSendsEvent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel model = new PartModel();
        repo.AddObject(model);

        bool called = false;
        ITowerRepo.ObjectRemoved received = default;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectRemoved e) =>
        {
            called = true;
            received = e;
        });

        repo.RemoveObject(model.Id);

        called.ShouldBe(true);
        received.Id.ShouldBe(model.Id);
        scene.TowerObjects.ContainsKey(model.Id).ShouldBeFalse();
    }

    [Test]
    public void RemoveObjectAlsoRemovesDescendants()
    {
        repo.LoadTower(towerMock.Object);

        PartModel parent = new PartModel();
        PartModel child = new PartModel();
        PartModel grandchild = new PartModel();

        repo.AddObject(parent);
        repo.AddObject(child, parent.Id);
        repo.AddObject(grandchild, child.Id);

        repo.RemoveObject(parent.Id);

        scene.TowerObjects.ContainsKey(parent.Id).ShouldBeFalse();
        scene.TowerObjects.ContainsKey(child.Id).ShouldBeFalse();
        scene.TowerObjects.ContainsKey(grandchild.Id).ShouldBeFalse();
    }

    // -------------------------------------------------------------------------
    // UpdateObject
    // -------------------------------------------------------------------------

    [Test]
    public void UpdateObjectDoesNothingWhenNoTowerLoaded()
    {
        bool called = false;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectChanged _) => called = true);
        repo.UpdateObject(new PartModel());

        called.ShouldBe(false);
    }

    [Test]
    public void UpdateObjectReplacesObjectAndSendsEvent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel model = new PartModel();
        repo.AddObject(model);

        PartModel updated = new PartModel { Id = model.Id, Name = "Updated" };

        bool called = false;
        ITowerRepo.ObjectChanged received = default;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectChanged e) =>
        {
            called = true;
            received = e;
        });

        repo.UpdateObject(updated);

        called.ShouldBe(true);
        received.Model.ShouldBe(updated);
        scene.TowerObjects[model.Id].Name.ShouldBe("Updated");
    }

    [Test]
    public void UpdateObjectPreservesParent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel parent = new PartModel();
        PartModel child = new PartModel();

        repo.AddObject(parent);
        repo.AddObject(child, parent.Id);

        PartModel updated = new PartModel { Id = child.Id };
        repo.UpdateObject(updated);

        scene.GetParent(child.Id).ShouldBe(parent.Id);
    }

    // -------------------------------------------------------------------------
    // ReparentObject
    // -------------------------------------------------------------------------

    [Test]
    public void ReparentObjectDoesNothingWhenNoTowerLoaded()
    {
        bool called = false;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectReparented _) => called = true);
        repo.ReparentObject(Guid.NewGuid(), Guid.NewGuid());

        called.ShouldBe(false);
    }

    [Test]
    public void ReparentObjectChangesParentAndSendsEvent()
    {
        repo.LoadTower(towerMock.Object);

        PartModel oldParent = new PartModel();
        PartModel newParent = new PartModel();
        PartModel child = new PartModel();

        repo.AddObject(oldParent);
        repo.AddObject(newParent);
        repo.AddObject(child, oldParent.Id);

        bool called = false;
        ITowerRepo.ObjectReparented received = default;

        repo.AutoChannel.Bind().On((in ITowerRepo.ObjectReparented e) =>
        {
            called = true;
            received = e;
        });

        repo.ReparentObject(child.Id, newParent.Id);

        called.ShouldBe(true);
        received.Id.ShouldBe(child.Id);
        received.NewParent.ShouldBe(newParent.Id);
        scene.GetParent(child.Id).ShouldBe(newParent.Id);
    }

    // -------------------------------------------------------------------------
    // Dispose
    // -------------------------------------------------------------------------

    [Test]
    public void Disposes()
    {
        Should.NotThrow(repo.Dispose);
        // Redundant dispose shouldn't do anything.
        Should.NotThrow(repo.Dispose);
    }
}
