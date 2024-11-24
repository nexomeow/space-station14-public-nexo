using Robust.Shared.Prototypes;

namespace Content.Shared.Imperial.Medieval.Magic.ProjectileCursorFollower;


[RegisterComponent]
public sealed partial class RuneOnCastComponent : Component
{
    [DataField]
    public EntProtoId Rune;
}
