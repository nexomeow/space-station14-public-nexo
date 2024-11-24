using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Imperial.Medieval.Magic;


/// <summary>
/// DoAfter parameters that are required to cast the spell
/// </summary>
[Serializable, NetSerializable, DataDefinition]
public sealed partial class MedievalSpellDoAfterArgs
{
    /// <summary>
    ///     How long does the do_after require to complete
    /// </summary>
    [DataField(required: true)]
    public TimeSpan Delay;

    /// <summary>
    /// Whether the progress bar for this DoAfter should be hidden from other players.
    /// </summary>
    [DataField]
    public bool Hidden;

    // Break the chains
    /// <summary>
    ///     Whether or not this do after requires the user to have hands.
    /// </summary>
    [DataField]
    public bool NeedHand;

    /// <summary>
    ///     Whether we need to keep our active hand as is (i.e. can't change hand or change item). This also covers
    ///     requiring the hand to be free (if applicable). This does nothing if <see cref="NeedHand"/> is false.
    /// </summary>
    [DataField]
    public bool BreakOnHandChange = true;

    /// <summary>
    ///     Whether the do-after should get interrupted if we drop the
    ///     active item we started the do-after with
    ///     This does nothing if <see cref="NeedHand"/> is false.
    /// </summary>
    [DataField]
    public bool BreakOnDropItem = true;

    [DataField]
    public EntProtoId? SpawnedRune;

    /// <summary>
    ///     If do_after stops when the user or target moves
    /// </summary>
    [DataField]
    public bool BreakOnMove;

    /// <summary>
    ///     Whether to break on movement when the user is weightless.
    ///     This does nothing if <see cref="BreakOnMove"/> is false.
    /// </summary>
    [DataField]
    public bool BreakOnWeightlessMove = true;

    /// <summary>
    ///     Threshold for user and target movement
    /// </summary>
    [DataField]
    public float MovementThreshold = 0.3f;

    /// <summary>
    ///     Threshold for distance user from the used OR target entities.
    /// </summary>
    [DataField]
    public float? DistanceThreshold;

    /// <summary>
    ///     Whether damage will cancel the DoAfter. See also <see cref="DamageThreshold"/>.
    /// </summary>
    [DataField]
    public bool BreakOnDamage;

    /// <summary>
    ///     Threshold for user damage. This damage has to be dealt in a single event, not over time.
    /// </summary>
    [DataField]
    public FixedPoint2 DamageThreshold = 1;

    /// <summary>
    ///     If true, this DoAfter will be canceled if the user can no longer interact with the target.
    /// </summary>
    [DataField]
    public bool RequireCanInteract = true;

    /// <summary>
    /// Speed modifier when cast spell
    /// </summary>
    [DataField]
    public float SpeedModifier = 1.0f;

    public void CopyToDoAfter(ref DoAfterArgs doAfter)
    {
        doAfter.Delay = Delay;
        doAfter.Hidden = Hidden;
        doAfter.NeedHand = NeedHand;
        doAfter.BreakOnHandChange = BreakOnHandChange;
        doAfter.BreakOnMove = BreakOnMove;
        doAfter.BreakOnWeightlessMove = BreakOnWeightlessMove;
        doAfter.MovementThreshold = MovementThreshold;
        doAfter.DistanceThreshold = DistanceThreshold;
        doAfter.BreakOnDamage = BreakOnDamage;
        doAfter.DamageThreshold = DamageThreshold;
        doAfter.RequireCanInteract = RequireCanInteract;
    }
}
