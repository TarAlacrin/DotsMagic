// ----- AUTO GENERATED CODE ----- //
using System.Reflection;
using System;
using System.Runtime.InteropServices;
namespace SpellWordNamespace
{
	public enum SpellWordEnum
	{
		AccellerateSpellForward,
		SpiralSpellForward,
	}
	public static unsafe class SpellWords
	{
		public delegate void SplWrdDel_Entity_Int32_ParallelWriter_Single(ref Unity.Entities.Entity spellEntity, System.Int32 entityInQueryIndex, ref Unity.Entities.EntityCommandBuffer.ParallelWriter ecb, System.Single deltaTime);
		//public static MethodInfo[] spellWordArray = {
		//	((SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.AccellerateSpellForward).GetMethodInfo(),
		//	((SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.SpiralSpellForward).GetMethodInfo(),
		//};

		//public static readonly IntPtr[] spellWordPtrArray =
		//{
		//	Marshal.GetFunctionPointerForDelegate((SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.AccellerateSpellForward),
		//	Marshal.GetFunctionPointerForDelegate((SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.SpiralSpellForward),
		//};

		public static readonly SplWrdDel_Entity_Int32_ParallelWriter_Single[] spellWordDelArray =
		{
			(SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.AccellerateSpellForward,
			(SplWrdDel_Entity_Int32_ParallelWriter_Single)SpellWordNamespace.BasicOnCreationSpells.SpiralSpellForward,
		};
	}
}
