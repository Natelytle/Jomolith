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
using Jomolith.Core.Domain;
using Jomolith.Core.Domain.Objects;
using Jomolith.Core.Events;
using static Jomolith.Core.Events.Events;

namespace Jomolith.Core.Commands;

public class TransformPartCommand : ICommand
{
    private readonly Vector3 newPosition;
    private readonly Quaternion newRotation;
    private readonly Vector3 newScale;
    private readonly Guid objectId;

    private Vector3 oldPosition;
    private Quaternion oldRotation;
    private Vector3 oldScale;

    public TransformPartCommand(Guid objectId, Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        this.objectId = objectId;
        this.newPosition = newPosition;
        this.newRotation = newRotation;
        this.newScale = newScale;
    }

    public void Execute(Tower tower, EventBus eventBus)
    {
        TowerObject obj = tower.FindObject(objectId);

        if (obj is Part part)
        {
            oldPosition = part.Location;
            part.Location = newPosition;

            oldRotation = part.Rotation;
            part.Rotation = newRotation;

            oldScale = part.Scale;
            part.Scale = newScale;

            eventBus.Publish(new ObjectTransformedEvent(objectId));
        }
    }

    public void Undo(Tower tower, EventBus eventBus)
    {
        TowerObject obj = tower.FindObject(objectId);

        if (obj is Part part)
        {
            part.Location = oldPosition;
            part.Rotation = oldRotation;
            part.Scale = oldScale;

            eventBus.Publish(new ObjectTransformedEvent(objectId));
        }
    }
}