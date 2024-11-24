using Robust.Shared.Network;

namespace Content.Shared.Imperial.Medieval.Magic.ProjectileCursorFollower;



public sealed partial class RuneFollowerSystem : EntitySystem
{
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RuneOnCastComponent, MedievalBeforeCastSpellEvent>(OnCast);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var enumerator = EntityQueryEnumerator<RuneFollowerComponent, TransformComponent>();

        while (enumerator.MoveNext(out var uid, out var component, out var _))
        {
            var grid = Transform(component.Parent).GridUid;
            var gridRotation = grid.HasValue ? _transformSystem.GetWorldRotation(grid.Value) : Angle.Zero;

            _transformSystem.SetWorldPosition(uid, _transformSystem.GetWorldPosition(component.Parent));
        }
    }


    private void OnCast(EntityUid uid, RuneOnCastComponent component, MedievalBeforeCastSpellEvent args)
    {
        if (_netManager.IsClient) return;

        var effect = Spawn(component.Rune, _transformSystem.GetMoverCoordinates(args.Performer));
        var followerComp = EnsureComp<RuneFollowerComponent>(effect);

        followerComp.Parent = args.Performer;
    }
}
