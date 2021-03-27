using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Unity.Rendering;

public class SpellCastSystem : SystemBase
{

	private EntityCommandBufferSystem ecbSource;

	private SpellVisualUpdateSystem spellVisualSystem;
	protected override void OnCreate()
	{
		base.OnCreate();
		spellVisualSystem = World.GetExistingSystem<SpellVisualUpdateSystem>();
		ecbSource = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

	}

	protected override void OnUpdate()
    {
		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			var ecb = ecbSource.CreateCommandBuffer();

			Entity spellEntity = EntityManager.CreateEntity();

			SpellDataComponent splData = new SpellDataComponent();
			splData.splCastTimeStart = (float)Time.ElapsedTime;
			splData.splVelocity = new float3(0, 0, 10f);
			ecb.AddComponent<SpellDataComponent>(spellEntity, splData);
			ecb.AddComponent<SpellRootComponent>(spellEntity);
			ecb.AddComponent<LocalToWorld>(spellEntity, new LocalToWorld { Value = float4x4.TRS(float3.zero, quaternion.identity, new float3(1)) });
			ecb.AddComponent<Translation>(spellEntity);
			ecb.AddComponent<Rotation>(spellEntity);
			ecb.AddComponent<Scale>(spellEntity);
			ecb.AddSharedComponent<RenderMesh>(spellEntity, new RenderMesh
			{
				mesh = spellVisualSystem.GetReferenceToSharedComponent(),
				material = spellVisualSystem.GetReferenceToSharedMaterial()
			}); 

			ecb.AddComponent<RenderBounds>(spellEntity, new RenderBounds() { 
				Value = new AABB { Center= float3.zero, Extents = new float3(1,1,1)}
			});
		}
	}
}
