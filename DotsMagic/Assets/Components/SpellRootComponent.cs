using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct SpellRootComponent : IComponentData
{
    //this is a reference to the entity that is considered the caster of this spell.
    //will usually be the thing that cast it likely,
    //but I could see some spells or counterspells redirecting this and pointing it to another entity using some dark and mysterious magics.
    Entity castingEntity;
    bool isRoot;
}
