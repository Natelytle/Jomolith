using Jomolith.Core.Objects;

namespace Jomolith.Core;

public interface ITower
{
    TowerMetadata Metadata { get; }
    TowerData TowerData { get; }

    TowerDto BuildTowerDto();
    static abstract ITower FromDto(TowerDto dto);
}

public partial class Tower : ITower
{
    public TowerMetadata Metadata { get; init; }
    public TowerData TowerData { get; init; }

    public Tower()
    {
        TowerData = new TowerData();
    }

    public TowerDto BuildTowerDto()
    {
        return TowerData.BuildTowerDataDto(Metadata);
    }

    public static ITower FromDto(TowerDto dto)
    {
        Tower tower = new Tower { Metadata = dto.Metadata };

        tower.TowerData.PopulateFromDto(dto);

        return tower;
    }
}
