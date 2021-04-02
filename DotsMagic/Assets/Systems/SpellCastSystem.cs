using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using Unity.Rendering;
using Havok.Physics;
using Unity.Physics;

unsafe public class SpellCastSystem : SystemBase
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
			
			Entity spellEntity = SpellCreationNamespace.SpellWrapperPrefabSpawningSystem.InstantiateNewSpellWrapperEntity(EntityManager, SpellCreationNamespace.SpellWrapperBasePrefabTypeEnum.SPHERE);
			PlayerCameraTrackerComponent cameraTracker = GetSingleton<PlayerCameraTrackerComponent>();
			float3 camPos = GetComponent<Translation>(cameraTracker.CameraEntity).Value;
			quaternion camRot = GetComponent<Rotation>(cameraTracker.CameraEntity).Value;


			SpellDataComponent splData = new SpellDataComponent();



			splData.splCastTimeStart = (float)Time.ElapsedTime;
			splData.splVelocity = new float3(0, 0, 10f);
			ecb.AddComponent<SpellDataComponent>(spellEntity, splData);
			ecb.AddComponent<SpellRootComponent>(spellEntity);


			EntityManager.SetComponentData(spellEntity, new Translation { Value = camPos });
			EntityManager.SetComponentData(spellEntity, new Rotation { Value = camRot });
		}
	}
}
