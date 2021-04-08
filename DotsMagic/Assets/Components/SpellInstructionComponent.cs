using System;
using Unity.Entities;
using SpellWordNamespace;


public interface ISpellInstructionBufferElement : IBufferElementData
{
	SpellWordNamespace.SpellWordEnum linkedInstruction { get; set; }
}




[Serializable]
public struct SpellUpdateInstructionBuffElem : ISpellInstructionBufferElement
{
	public SpellWordNamespace.SpellWordEnum spellWordLink;

	SpellWordEnum ISpellInstructionBufferElement.linkedInstruction { get => spellWordLink; set => spellWordLink = value; }
}
