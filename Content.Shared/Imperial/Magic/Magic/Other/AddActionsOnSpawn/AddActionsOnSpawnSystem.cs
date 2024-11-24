using Content.Shared.Actions;

namespace Content.Shared.Imperial.Medieval.Magic.AddActionsOnSpawn;



public sealed partial class AddActionsOnSpawnSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AddActionsOnSpawnComponent, MapInitEvent>(OnMapInit);
    }


    public void OnMapInit(EntityUid uid, AddActionsOnSpawnComponent component, MapInitEvent args)
    {
        foreach (var actionId in component.Actions)
            _actions.AddAction(uid, actionId);
    }
}
