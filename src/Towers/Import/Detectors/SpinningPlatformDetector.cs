using RobloxFiles;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Import.Detectors;

public class SpinningPlatformDetector : IClientObjectDetector
{
    public bool TryExtract(Instance instance, out ClientObjectDto? dto)
    {
        dto = null;

        return false;
    }
}
