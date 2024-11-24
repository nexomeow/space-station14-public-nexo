using System.Linq;
using Content.Server.Chat.Systems;
using Content.Server.Hands.Systems;
using Content.Server.Imperial.MouseInput;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Imperial.Medieval.Magic;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server.Imperial.Medieval.Magic;


/// <summary>
/// Server part of the <see cref="SharedMedievalMagicSystem" />
/// <para>
/// Responsible for the words spoken when casting spells and for the spawn of the projectile
/// </para>
/// </summary>
public sealed partial class MedievalMagicSystem : SharedMedievalMagicSystem
{
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly HandsSystem _handsSystem = default!;
    [Dependency] private readonly MouseInputSystem _mouseInputSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;


    public override void Initialize()
    {
        base.Initialize();


        InitializeTargetSpells();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var enumerator = EntityQueryEnumerator<MedievalSpellCasterComponent>();

        while (enumerator.MoveNext(out var uid, out var component))
        {
            foreach (var speakPoint in component.SpellWordsStack.ToList())
            {
                if (speakPoint.Item1 > _timing.CurTime) continue;

                var spellSpeech = speakPoint.Item2;

                var ev = new MedievalSpeakSpellEvent(uid, spellSpeech.Speech);
                RaiseLocalEvent(ref ev);

                component.SpellWordsStack.Remove(speakPoint);
                Dirty(uid, component);

                if (ev.Cancelled) continue;

                _chatSystem.TrySendInGameICMessage(
                    uid,
                    Loc.GetString(spellSpeech.Speech),
                    TransformToChatEnum(spellSpeech.SpeechType),
                    spellSpeech.HideChat
                );
            }
        }
    }

    #region Helpers

    protected override void AddToStack(EntityUid uid, Dictionary<TimeSpan, MedievalSpellSpeech>? el)
    {
        if (el == null) return;

        var casterComponent = EnsureComp<MedievalSpellCasterComponent>(uid);

        foreach (var speakPoint in el)
        {
            var speakPointTime = _timing.CurTime + speakPoint.Key;

            casterComponent.SpellWordsStack.Add((speakPointTime, speakPoint.Value));
        }

        Dirty(uid, casterComponent);
    }

    private InGameICChatType TransformToChatEnum(SpellSpeechType? speechType)
    {
        return speechType switch
        {
            SpellSpeechType.Speak => InGameICChatType.Speak,
            SpellSpeechType.Emote => InGameICChatType.Emote,
            SpellSpeechType.Whisper => InGameICChatType.Whisper,
            _ => InGameICChatType.Speak
        };
    }

    #endregion
}
