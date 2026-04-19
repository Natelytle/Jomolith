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

namespace Jomolith.Game.Core.Domain.Objects;

/// <summary>
///     The part class, which stores spacial and collision data.
/// </summary>
public class Part : TowerObject
{
    public Part(string name, Guid id)
        : base(name, id)
    {
    }

    public Vector3 Location { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public bool Collidable { get; set; }
    public bool HasPhysics { get; set; }

    public override string ToString()
    {
        return $"[{Name} ({Location.X}, {Location.Y}, {Location.Z})]";
    }
}