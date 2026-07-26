using Jomolith.Towers.Models;
using RobloxFiles;

namespace Jomolith.Towers.Import.Detectors;

public interface IClientObjectDetector
{
    bool TryExtract(Instance instance, out ClientObjectDto? dto);
}
