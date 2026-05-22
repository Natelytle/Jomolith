using System;

namespace Jomolith.Core.Objects;

public abstract class TowerObject
{
    public string ClassName => GetType().Name;

    protected TowerObject()
    {
        Name = ClassName;
        Id = Guid.NewGuid();
    }

    public string Name { get; set; }

    public Guid Id { get; init; }
}
