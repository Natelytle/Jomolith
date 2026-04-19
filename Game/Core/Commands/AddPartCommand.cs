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
using System.Numerics;
using Jomolith.Game.Core.Domain;
using Jomolith.Game.Core.Domain.Objects;
using Jomolith.Game.Core.Events;
using static Jomolith.Game.Core.Events.Events;

namespace Jomolith.Game.Core.Commands;

public class AddPartCommand : ICommand
{
    private readonly bool collidable;
    private readonly bool hasPhysics;
    private readonly Vector3 location;
    private readonly string name;
    private readonly Guid objectId;
    private readonly Guid? parentId;
    private readonly Quaternion rotation;
    private readonly Vector3 scale;

    private Part? addedPart;

    public AddPartCommand(String name, Guid objectId, Vector3 location, Quaternion rotation, Vector3 scale,
        bool collidable, bool hasPhysics, Guid? parentId = null)
    {
        this.name = name;
        this.objectId = objectId;
        this.location = location;
        this.rotation = rotation;
        this.scale = scale;
        this.collidable = collidable;
        this.hasPhysics = hasPhysics;
        this.parentId = parentId;
    }

    public void Execute(Tower tower, EventBus eventBus)
    {
        addedPart = new Part(name, objectId)
        {
            Collidable = collidable,
            HasPhysics = hasPhysics,
            Location = location,
            Rotation = rotation,
            Scale = scale
        };

        tower.AddObject(addedPart, parentId);
        eventBus.Publish(new ObjectAddedEvent(objectId));
    }

    public void Undo(Tower tower, EventBus eventBus)
    {
        if (addedPart is null)
            throw new InvalidOperationException("AddPart: Cannot undo before execute");

        tower.RemoveObject(addedPart.Id);
        eventBus.Publish(new ObjectRemovedEvent(objectId));
    }

    public static AddPartCommand FromPart(Part part, Guid? parentId = null)
    {
        return new AddPartCommand(part.Name, part.Id, part.Location, part.Rotation, part.Scale, part.Collidable,
            part.HasPhysics, parentId);
    }
}