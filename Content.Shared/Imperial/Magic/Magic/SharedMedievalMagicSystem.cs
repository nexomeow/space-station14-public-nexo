using System.Linq;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Imperial.MouseInput.Events;
using Content.Shared.Mind;
using Content.Shared.Movement.Systems;
using Robust.Shared.Map;

namespace Content.Shared.Imperial.Medieval.Magic;


public abstract partial class SharedMedievalMagicSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _speedModifierSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;


    public override void Initialize()
    {
        base.Initialize();

        #region Do Afters

        SubscribeLocalEvent<MedievalSpellCasterComponent, MedievalCastSpawnSpellDoAfterEvent>(OnSpellDoAfterCast);
        SubscribeLocalEvent<MedievalSpellCasterComponent, MedievalCastProjectileSpellDoAfterEvent>(OnSpellDoAfterCast);

        #endregion

        #region Relay

        SubscribeLocalEvent<MedievalSpellCasterComponent, MousePositionRefreshEvent>(OnMousePositionResponse);

        #endregion

        SubscribeLocalEvent<MedievalSpellCasterComponent, RefreshMovementSpeedModifiersEvent>(OnRefresh);

        InitializeTargetSpells();
    }

    private void OnSpellDoAfterCast(EntityUid uid, MedievalSpellCasterComponent component, MedievalSpellDoAfterEvent args)
    {
        if (args.Handled) return;

        var casterComponent = EnsureComp<MedievalSpellCasterComponent>(uid);
        casterComponent.SpeedModifiers.Remove(GetSpellData(args).CastSpeedModifier);

        Dirty(uid, casterComponent);

        _speedModifierSystem.RefreshMovementSpeedModifiers(uid);

        if (args.Cancelled) return;

        CastSpell(args);
    }

    private void OnRefresh(EntityUid uid, MedievalSpellCasterComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        var speedModifier = component.SpeedModifiers.Aggregate(1.0f, (acc, next) => next < acc ? next : acc);

        args.ModifySpeed(speedModifier, speedModifier);
    }

    private void OnMousePositionResponse(EntityUid uid, MedievalSpellCasterComponent component, MousePositionRefreshEvent args)
    {
        if (!_mindSystem.TryGetMind(uid, out var mind, out var _)) return;
        if (!TryComp<ActionsContainerComponent>(mind, out var actionsContainerComponent)) return;

        var ev = new MedievalSpellRelayEvent<MousePositionRefreshEvent>(args);

        foreach (var action in actionsContainerComponent.Container.ContainedEntities)
            RaiseLocalEvent(action, ev);
    }

    #region Helpers

    protected bool PassesSpellPrerequisites(EntityUid spell, EntityUid performer, EntityCoordinates target)
    {
        var ev = new MedievalBeforeCastSpellEvent(performer, target);
        RaiseLocalEvent(spell, ref ev);

        return !ev.Cancelled;
    }

    protected MedievalSpellData GetSpellData(MedievalSpellDoAfterEvent ev)
    {
        return ev switch
        {
            MedievalCastProjectileSpellDoAfterEvent projectileSpellDoAfterEvent => projectileSpellDoAfterEvent.SpellData,
            MedievalCastSpawnSpellDoAfterEvent spawnSpellDoAfterEvent => spawnSpellDoAfterEvent.SpellData,
            _ => throw new ArgumentOutOfRangeException("Cannot find upcast method")
        };
    }

    #region Cast Spell

    protected void CastSpell(MedievalSpellDoAfterEvent args)
    {
        switch (args)
        {
            case MedievalCastProjectileSpellDoAfterEvent projectileSpell:
                CastProjectileSpell(projectileSpell.SpellData);
                break;
            case MedievalCastSpawnSpellDoAfterEvent spawnSpell:
                CastSpawnSpell(spawnSpell.SpellData);
                break;
        }
    }

    protected void CastSpell(MedievalSpellData args)
    {
        switch (args)
        {
            case MedievalSpawnSpellData spawnSpellData:
                CastSpawnSpell(spawnSpellData);
                break;
            case MedievalProjectileSpellData projectileSpellData:
                CastProjectileSpell(projectileSpellData);
                break;
        }
    }

    #endregion

    #region Server/Client implementation

    protected virtual void AddToStack(EntityUid uid, Dictionary<TimeSpan, MedievalSpellSpeech>? el)
    {
    }

    #endregion

    #endregion
}
