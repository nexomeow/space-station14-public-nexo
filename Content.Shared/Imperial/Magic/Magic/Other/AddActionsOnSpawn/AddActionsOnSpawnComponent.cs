using Robust.Shared.Prototypes;

namespace Content.Shared.Imperial.Medieval.Magic.AddActionsOnSpawn;


[RegisterComponent]
public sealed partial class AddActionsOnSpawnComponent : Component
{
    [DataField]
    public HashSet<EntProtoId> Actions;
}
