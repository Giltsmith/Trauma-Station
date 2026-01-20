using Content.Goobstation.Common.Barks;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid;

/// <summary>
/// Trauma - barks stuff
/// </summary>
public sealed partial class HumanoidProfileSystem
{
    public static readonly ProtoId<BarkPrototype> DefaultBarkVoice = "Alto";

    public void SetBarkVoice(Entity<HumanoidProfileComponent> ent, [ForbidLiteral] ProtoId<BarkPrototype>? barkvoiceId)
    {
        var voicePrototypeId = DefaultBarkVoice;
        var species = ent.Comp.Species;
        if (barkvoiceId != null &&
            _prototype.TryIndex(barkvoiceId, out var bark) &&
            bark.SpeciesWhitelist?.Contains(species) != false)
        {
            voicePrototypeId = barkvoiceId.Value;
        }
        else
        {
            // use first valid bark as a fallback
            foreach (var o in _prototype.EnumeratePrototypes<BarkPrototype>())
            {
                if (o.RoundStart && o.SpeciesWhitelist?.Contains(species) != false)
                {
                    voicePrototypeId = o.ID;
                    break;
                }
            }
        }

        var comp = EnsureComp<SpeechSynthesisComponent>(ent);
        comp.VoicePrototypeId = voicePrototypeId;
        Dirty(ent, comp);
        ent.Comp.BarkVoice = voicePrototypeId;
        Dirty(ent);
    }
}
