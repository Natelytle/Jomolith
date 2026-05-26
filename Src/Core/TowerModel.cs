using Jomolith.Core.Objects;

namespace Jomolith.Core;

public interface ITowerModel
{
    TowerMetadata Metadata { get; }
    TowerSceneModel TowerSceneModel { get; }

    TowerDto BuildTowerDto();
    static abstract ITowerModel FromDto(TowerDto dto);
}

public partial class TowerModel : ITowerModel
{
    public TowerMetadata Metadata { get; init; }
    public TowerSceneModel TowerSceneModel { get; init; } = new TowerSceneModel();

    public TowerDto BuildTowerDto()
    {
        return TowerSceneModel.BuildTowerDataDto(Metadata);
    }

    public static ITowerModel FromDto(TowerDto dto)
    {
        TowerModel towerModel = new TowerModel { Metadata = dto.Metadata };

        towerModel.TowerSceneModel.PopulateFromDto(dto);

        return towerModel;
    }
}
