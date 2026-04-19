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
using Jomolith.Game.Core.Commands;
using Jomolith.Game.Core.Domain;
using Jomolith.Game.Core.Domain.Objects;
using Jomolith.Game.Core.Events;
using static Jomolith.Game.Core.Events.Events;

namespace Jomolith.Game.Core;

internal class Program
{
    public static void Main(string[] args)
    {
        Tower tower = new Tower("Test");
        EventBus eventBus = new();

        CommandExecutor executor = new(tower, eventBus);

        Part part = new("Awesome part", Guid.NewGuid());

        ICommand towerAddCommand = AddPartCommand.FromPart(part);

        eventBus.Subscribe<ObjectAddedEvent>(onObjectAdded);
        eventBus.Subscribe<ObjectRemovedEvent>(onObjectRemoved);

        executor.Execute(towerAddCommand);
        executor.Undo();
        executor.Redo();

        void onObjectAdded(ObjectAddedEvent addedEvent)
        {
            Console.WriteLine($"Woo hoo! Added {tower.FindObject(addedEvent.ObjectID)} at {addedEvent.ObjectID}!!!");
        }

        void onObjectRemoved(ObjectRemovedEvent removedEvent)
        {
            Console.WriteLine($"Fuck... we removed {removedEvent.ObjectID}!!!");
        }
    }
}