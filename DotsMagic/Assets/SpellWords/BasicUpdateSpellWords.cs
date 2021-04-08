using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

//public partial class MagicSpellSimSystem : SystemBase this works but IDK how sustainable it is

namespace SpellWordNamespace
{
	public static class BasicOnCreationSpells
	{
		[SpellWordAttribute("AccellerateForward")]
		public static void AccellerateSpellForward( Entity spellEntity, int entityInQueryIndex, in EntityCommandBuffer.ParallelWriter ecb, in MagicSpellSimSystem simsys, float deltaTime)
		{
			var localtoworld = simsys.GetSpellComponent<LocalToWorld>(spellEntity);
			var physVel = simsys.GetSpellComponent<PhysicsVelocity>(spellEntity);

			physVel.Linear += localtoworld.Forward * deltaTime*2.0f;

			//Figure out a better solution for this.
			ecb.SetComponent<PhysicsVelocity>(entityInQueryIndex, spellEntity, physVel);
			Debug.LogWarning("ACCELING");
		}

		[SpellWordAttribute("TwistySpell")]
		public static void SpiralSpellForward( Entity spellEntity, int entityInQueryIndex, in EntityCommandBuffer.ParallelWriter ecb, in MagicSpellSimSystem simsys, float deltaTime)
		{
			var translat = simsys.GetSpellComponent<Translation>(spellEntity);
			var rotation = simsys.GetSpellComponent<Rotation>(spellEntity);
			var physVel = simsys.GetSpellComponent<PhysicsVelocity>(spellEntity);


			Debug.LogWarning("SPIRALING");
		}
	}



}
