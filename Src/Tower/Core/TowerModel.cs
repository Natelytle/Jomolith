using Jomolith.Tower.Core.Dto;
using Jomolith.Tower.Core.Serialization;

namespace Jomolith.Tower.Core;

public interface ITowerModel
{
    TowerMetadata Metadata { get; }
    TowerSceneModel Scene { get; }

    TowerDto BuildTowerDto();
}

public partial class TowerModel : ITowerModel
{
    public TowerSceneModel Scene { get; }
    public TowerMetadata Metadata { get; }

    public TowerModel(TowerSceneModel scene, TowerMetadata metadata)
    {
        Scene = scene;
        Metadata = metadata;
    }

    public TowerDto BuildTowerDto()
    {
        return TowerSerializer.BuildTowerDto(Scene, Metadata);
    }
}
