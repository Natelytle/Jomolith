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

using System.Collections.Generic;
using Jomolith.Game.Core.Domain;
using Jomolith.Game.Core.Events;

namespace Jomolith.Game.Core.Commands;

public class CommandExecutor
{
    private readonly EventBus eventBus;
    private readonly Stack<ICommand> redoStack;
    private readonly Tower tower;
    private readonly Stack<ICommand> undoStack;

    public CommandExecutor(Tower tower, EventBus eventBus)
    {
        this.tower = tower;
        this.eventBus = eventBus;
        undoStack = new Stack<ICommand>();
        redoStack = new Stack<ICommand>();
    }

    public void Execute(ICommand command)
    {
        undoStack.Push(command);
        redoStack.Clear();

        command.Execute(tower, eventBus);
    }

    public void Undo()
    {
        if (undoStack.TryPeek(out ICommand? undoCommand))
        {
            undoCommand.Undo(tower, eventBus);
            redoStack.Push(undoCommand);
        }
    }

    public void Redo()
    {
        if (redoStack.TryPeek(out ICommand? redoCommand))
        {
            redoCommand.Execute(tower, eventBus);
            undoStack.Push(redoCommand);
        }
    }
}