using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;



unsafe struct PlayerCameraTrackerComponent : IComponentData
{
    public Entity CameraEntity;
}

[DisallowMultipleComponent]
public class CameraTrackerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Camera cameraToTrack;

    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
        //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });

        var cameraTrackerComponent = new PlayerCameraTrackerComponent
        {
            CameraEntity = entity
        };

        PlayerCameraSystem.currentPlayerCamera = cameraToTrack.transform;

        dstManager.AddComponentData(entity, cameraTrackerComponent);
    }
}


[UpdateBefore(typeof(TRSToLocalToWorldSystem))]
class PlayerCameraSystem : SystemBase
{
    public static Transform currentPlayerCamera;
    PlayerCameraTrackerComponent cameraComp;
    
    protected void MoveDataToSingleton()
    {
        using (var entities = GetEntityQuery(new ComponentType[] { typeof(PlayerCameraTrackerComponent) }).ToEntityArray(Allocator.TempJob))
        {
            if(entities.Length == 0)
			{
                Debug.LogError("TRYING TO MOVE DATA BUT NO ENTITIES FOUND WITH TRACKING COMPONENT");
                return;
			}


            cameraComp = EntityManager.GetComponentData<PlayerCameraTrackerComponent>(entities[0]);
            SetSingleton<PlayerCameraTrackerComponent>(cameraComp);

            EntityManager.AddComponent<LocalToWorld>(entities[0]);
            EntityManager.AddComponent<Translation>(entities[0]);
            EntityManager.AddComponent<Rotation>(entities[0]);
            EntityManager.AddComponent<Scale>(entities[0]);
        }
    }
    protected override void OnCreate()
    {
        base.OnCreate();
        MoveDataToSingleton();
    }

	protected override void OnUpdate()
	{

        if(cameraComp.CameraEntity == Entity.Null)
		{
            MoveDataToSingleton();

            if (cameraComp.CameraEntity == Entity.Null)
			{
                Debug.LogError("ERROR FINDING CAMERA TRACKER COMPONENT");
                return; 
			}
        }
        EntityManager.SetComponentData<Translation>(cameraComp.CameraEntity, new Translation() { Value = currentPlayerCamera.position });
        EntityManager.SetComponentData<Rotation>(cameraComp.CameraEntity, new Rotation() { Value = currentPlayerCamera.rotation });
        EntityManager.SetComponentData<Scale>(cameraComp.CameraEntity, new Scale() { Value = currentPlayerCamera.lossyScale.y });
    }
}