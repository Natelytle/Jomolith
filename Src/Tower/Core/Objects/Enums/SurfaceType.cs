using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SurfaceType
{
    Smooth = 0,
    Glue = 1,
    Weld = 2,
    Studs = 3,
    Inlet = 4,
    Universal = 5,
    Hinge = 6,
    Motor = 7,
    SteppingMotor = 8,
    SmoothNoOutlines = 10 // For legacy reasons, this is 10 and not 9
}
