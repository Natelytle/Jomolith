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

namespace Jomolith.Game.Core.Domain.Objects;

public class TowerObject
{
    public TowerObject(string name, Guid id)
    {
        Name = name;
        Id = id;
    }

    public string Name { get; }
    public Guid Id { get; private set; }

    public override string ToString()
    {
        return $"TowerObject: {Name}";
    }
}