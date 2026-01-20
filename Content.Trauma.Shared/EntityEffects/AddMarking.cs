// SPDX-License-Identifier: AGPL-3.0-or-later
using Content.Shared.Body;
using Content.Shared.EntityEffects;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;

namespace Content.Trauma.Shared.EntityEffects;

/// <summary>
/// Adds a marking to the target mob, on a specific organ.
/// </summary>
public sealed partial class AddMarking : EntityEffectBase<AddMarking>
{
    [DataField(required: true)]
    public ProtoId<OrganCategoryPrototype> Organ;

    [DataField(required: true)]
    public ProtoId<MarkingPrototype> Marking;

    [DataField]
    public Color? Color;

    [DataField]
    public bool Forced;

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("entity-effect-guidebook-add-marking", ("chance", Probability), ("marking", prototype.Index(Marking).Name));
}

public sealed class AddMarkingEffectSystem : EntityEffectSystem<BodyComponent, AddMarking>
{
    [Dependency] private readonly BodySystem _body = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    protected override void Effect(Entity<BodyComponent> ent, ref EntityEffectEvent<AddMarking> args)
    {
        // TODO NUBODY: make better if an actual api is made
        if (_body.GetOrgan(ent.AsNullable(), args.Effect.Organ) is not {} organ ||
            !TryComp<VisualOrganMarkingsComponent>(organ, out var comp))
            return;

        var markings = comp.Markings;
        var marking = _proto.Index(args.Effect.Marking);
        if (!markings.TryGetValue(marking.BodyPart, out var list))
            return;

        // don't add 2 of the same marking
        foreach (var data in list)
        {
            if (data.MarkingId == marking.ID)
                return;
        }

        // add it, i hope
        list.Add(new Marking(marking.ID, []));
        Dirty(organ, comp); // no fucking idea if this works :))))))
    }
}
