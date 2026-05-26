using System;

namespace Jomolith.Core.Objects;

public abstract class TowerObjectModel
{
    public string ClassName => GetType().Name;

    protected TowerObjectModel()
    {
        Name = ClassName;
        Id = Guid.NewGuid();
    }

    public string Name { get; set; }

    public Guid Id { get; init; }
}
