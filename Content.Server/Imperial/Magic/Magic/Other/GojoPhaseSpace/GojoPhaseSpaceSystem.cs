using Content.Shared.Imperial.Medieval.Magic;
using Content.Shared.Imperial.PhaseSpace;
using Content.Shared.Movement.Components;

namespace Content.Server.Imperial.Medieval.Magic.GojoPhaseSpace;


public sealed partial class GojoInfinitySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GojoInfinityAbilityToggleEvent>(OnInfinityToggled);
    }

    public void OnInfinityToggled(GojoInfinityAbilityToggleEvent args)
    {
        if (HasComp<PhaseSpaceComponent>(args.Performer))
        {
            RemComp<PhaseSpaceComponent>(args.Performer);
            RemComp<MovementIgnoreGravityComponent>(args.Performer);
            RemComp<PhaseSpaceShadowComponent>(args.Performer);

            return;
        }

        EnsureComp<PhaseSpaceComponent>(args.Performer);
        EnsureComp<MovementIgnoreGravityComponent>(args.Performer);
        EnsureComp<PhaseSpaceShadowComponent>(args.Performer);
    }
}
