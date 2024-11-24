using Robust.Shared.Serialization;

namespace Content.Shared.Imperial.Medieval.Magic;


/// <summary>
/// An event that is called at the end of a spell cast with DoAfter
/// </summary>
[Serializable, NetSerializable]
public sealed partial class MedievalCastSpawnSpellDoAfterEvent : MedievalSpellDoAfterEvent
{
    public new MedievalSpawnSpellData SpellData;
}
