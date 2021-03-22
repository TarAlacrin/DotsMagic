using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;

public class SpellCastSystem : SystemBase
{
	private EntityCommandBufferSystem ecbSource;

	protected override void OnCreate()
	{
		base.OnCreate();

		ecbSource = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			var ecb = ecbSource.CreateCommandBuffer();

			Entity spellEntity = EntityManager.CreateEntity();

			SpellDataComponent splData = new SpellDataComponent();
			splData.splCastTimeStart = (float)Time.ElapsedTime;
			splData.splVelocity = new float3(0, 0, 1f);
			ecb.AddComponent<SpellDataComponent>(spellEntity, splData);
			ecb.AddComponent<SpellRootComponent>(spellEntity);
			ecb.AddComponent<LocalToWorld>(spellEntity);
			LocalToWorld ltw = new LocalToWorld();
		}
	}
}
