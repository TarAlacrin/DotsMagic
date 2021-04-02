using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class SpellVisualUpdateSystem : SystemBase
{
    public static int test;
    private EntityCommandBufferSystem ecbSource;

    Mesh rendermesh;
    Material rendermaterial;
    protected override void OnCreate()
    {
        base.OnCreate();
        ecbSource = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        rendermesh = Resources.Load<Mesh>("trifbx");
        rendermaterial = Resources.Load<Material>("Materials/DefaultSpellBlu");
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter parallelWriterECB = ecbSource.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .ForEach(
            (Entity entity, int entityInQueryIndex, in SpellDataComponent spellComponent, in Translation translation, in Rotation rotation, in Scale scale) =>
            {
                UpdateComponentData(entity, entityInQueryIndex, spellComponent, translation, rotation, scale, ref parallelWriterECB);
            }).ScheduleParallel();
        
        
        ecbSource.AddJobHandleForProducer(this.Dependency);
    }

    public Mesh GetReferenceToSharedComponent()
	{
        return rendermesh;
    }


    public Material GetReferenceToSharedMaterial()
	{
        return rendermaterial;
	}


    public static void UpdateComponentData(in Entity entity, int entityInQueryIndex, in SpellDataComponent spellData, in Translation translation, in Rotation rotation , in Scale scale, ref EntityCommandBuffer.ParallelWriter parallelWriter)
    {
        parallelWriter.SetComponent<Translation>(entityInQueryIndex, entity, new Translation { Value = spellData.splPosition });
        parallelWriter.SetComponent<Rotation>(entityInQueryIndex, entity, new Rotation { Value = spellData.splRotaiton });
        parallelWriter.SetComponent<Scale>(entityInQueryIndex, entity, new Scale { Value = 1 });
    }
}
