namespace Content.Shared.Imperial.Medieval.Magic;


/// <summary>
/// Raised on action and spawned entity after last a initialize
/// </summary>
public sealed class MedievalAfterSpawnEntityBySpellEvent : EntityEventArgs
{
    public EntityUid Action;

    public EntityUid Performer;

    public Angle Rotation = Angle.FromDegrees(0);
}
