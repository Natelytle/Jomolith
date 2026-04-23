// Copyright (c) 2026 Natelytle
// 
// This file is part of Jomolith.
// 
// Jomolith is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Jomolith is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
// License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Jomolith. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Jomolith.Core.Domain.Objects;

namespace Jomolith.Core.Domain;

public class Tower
{
    // Hierarchical information
    private readonly Dictionary<Guid, HashSet<Guid>> childrenMap = new Dictionary<Guid, HashSet<Guid>>();
    private readonly Dictionary<Guid, Guid> parentMap = new Dictionary<Guid, Guid>();
    private readonly Dictionary<Guid, TowerObject> towerObjects = new Dictionary<Guid, TowerObject>();

    public Tower(string name)
    {
        Name = name;

        // Every tower has a root object
        TowerObject root = new TowerObject("Root", Guid.NewGuid());
        towerObjects[root.Id] = root;
        RootId = root.Id;
    }

    public string Name { get; set; }
    public Guid RootId { get; }

    public IReadOnlyDictionary<Guid, TowerObject> Objects => towerObjects;

    public TowerObject FindObject(Guid id)
    {
        if (!towerObjects.TryGetValue(id, out TowerObject? towerObj))
            throw new InvalidOperationException($"Tried to access object {id}, which does not exist.");

        return towerObj;
    }

    public void AddObject(TowerObject towerObj, Guid? parentId = null)
    {
        if (towerObjects.ContainsKey(towerObj.Id))
            throw new InvalidOperationException($"Object {towerObj.Id} already exists");

        towerObjects.Add(towerObj.Id, towerObj);

        // Set the parent to the root if not specified.
        SetParent(towerObj.Id, parentId ?? RootId);
    }

    public void RemoveObject(Guid id)
    {
        if (id == RootId)
            throw new InvalidOperationException("Cannot remove root object");

        // Recursively remove all descendants first
        List<Guid> descendants = GetDescendants(id).ToList();

        foreach (var descendantId in descendants.OrderByDescending(GetDepth))
        {
            clearParent(descendantId);
            towerObjects.Remove(descendantId);
        }

        // Remove the object itself
        clearParent(id);
        towerObjects.Remove(id);
    }

    /// <summary>
    ///     Gets the parent Guid of this object
    /// </summary>
    /// <param name="id">The Guid of the child object</param>
    /// <returns>The Guid of the parent to this object</returns>
    public Guid GetParent(Guid id)
    {
        // Every object should have a parent so no need to check for null
        parentMap.TryGetValue(id, out Guid parentId);

        return parentId;
    }

    /// <summary>
    ///     Sets the parent of this object to the new object Guid
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newParentId"></param>
    /// <returns>True if the reparenting was successful, false otherwise.</returns>
    public void SetParent(Guid id, Guid newParentId)
    {
        validateReparent(id, newParentId);

        // Remove this ID from its old parent, if it exists.
        if (parentMap.TryGetValue(id, out Guid oldParentId)) childrenMap[oldParentId].Remove(id);

        parentMap[id] = newParentId;

        // Ensure the new parent has a map of children
        if (!childrenMap.ContainsKey(newParentId))
            childrenMap[newParentId] = new HashSet<Guid>();

        childrenMap[newParentId].Add(id);
    }

    /// <summary>
    ///     Clears the parent of this Guid
    /// </summary>
    /// <param name="childId">The Guid to clear the parent of.</param>
    private void clearParent(Guid childId)
    {
        if (parentMap.TryGetValue(childId, out var parentId))
        {
            childrenMap[parentId].Remove(childId);
            parentMap.Remove(childId);
        }
    }

    /// <summary>
    ///     Returns the first level children of this Guid
    /// </summary>
    /// <param name="id">The parent ID.</param>
    /// <returns>The first children of this ID.</returns>
    public IEnumerable<Guid> GetChildren(Guid id)
    {
        return childrenMap.TryGetValue(id, out HashSet<Guid>? children) ? children : Enumerable.Empty<Guid>();
    }

    /// <summary>
    ///     Returns all descendents of this Guid
    /// </summary>
    /// <param name="id">The parent ID.</param>
    /// <returns>All descendants of this ID.</returns>
    public IEnumerable<Guid> GetDescendants(Guid id)
    {
        Stack<Guid> stack = new Stack<Guid>(GetChildren(id));

        while (stack.Count > 0)
        {
            Guid current = stack.Pop();
            yield return current;

            foreach (var childId in GetChildren(current))
                stack.Push(childId);
        }
    }

    public IEnumerable<Guid> GetAncestors(Guid id)
    {
        Guid currentId = id;

        while (parentMap.TryGetValue(currentId, out Guid parentId))
        {
            currentId = parentId;

            yield return parentId;
        }
    }

    public int GetDepth(Guid id)
    {
        return GetAncestors(id).Count();
    }

    // Ensures that this reparenting creates no cyclical parents
    private void validateReparent(Guid id, Guid newParentId)
    {
        if (id == RootId)
            throw new InvalidOperationException("Tried to reparent root object.");

        // Can't parent to yourself
        if (id == newParentId)
            throw new InvalidOperationException($"Tried parenting object {id} to itself.");

        // Make sure we aren't parenting this to a child
        if (GetAncestors(newParentId).Contains(id))
            throw new InvalidOperationException($"Tried parenting object {id} to child object {newParentId}.");
    }
}