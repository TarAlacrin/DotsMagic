using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
using Unity.Burst;
//public partial class MagicSpellSimSystem : SystemBase this works but IDK how sustainable it is

namespace SpellWordNamespace
{
	//TODO: https://docs.unity3d.com/Packages/com.unity.burst@1.5/manual/docs/AdvancedUsages.html#function-pointers
	//FIX THIS
	[BurstCompile]
	public static class BasicOnCreationSpells
	{
		public delegate void SplWrdDel_Entity_Int32_ParallelWriter_Single_o(ref Unity.Entities.Entity spellEntity, System.Int32 entityInQueryIndex, ref Unity.Entities.EntityCommandBuffer.ParallelWriter ecb, System.Single deltaTime);


		[BurstCompile]
		[SpellWordAttribute("AccellerateForward")]
		public static void AccellerateSpellForward(ref Entity spellEntity, int entityInQueryIndex, ref EntityCommandBuffer.ParallelWriter ecb, float deltaTime)
		{
			var localtoworld = MagicSpellSimSystem.inst.GetSpellComponent<LocalToWorld>(spellEntity);
			var physVel = MagicSpellSimSystem.inst.GetSpellComponent<PhysicsVelocity>(spellEntity);
			
			physVel.Linear += localtoworld.Forward * deltaTime*2.0f;

			//Figure out a better solution for this.
			ecb.SetComponent<PhysicsVelocity>(entityInQueryIndex, spellEntity, physVel);
		}

		[BurstCompile]
		[SpellWordAttribute("TwistySpell")]
		public static void SpiralSpellForward(ref Entity spellEntity, int entityInQueryIndex, ref EntityCommandBuffer.ParallelWriter ecb, float deltaTime)
		{
			//var translat = GetSpellComponent<Translation>(spellEntity);
			//var rotation = GetSpellComponent<Rotation>(spellEntity);
			//var physVel =  GetSpellComponent<PhysicsVelocity>(spellEntity);
		}
	}



}
