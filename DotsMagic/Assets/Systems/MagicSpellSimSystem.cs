using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Reflection;
using System;
using System.Runtime.InteropServices;

unsafe public class MagicSpellSimSystem : SystemBase
{
    public static MagicSpellSimSystem inst;
    private EntityCommandBufferSystem ecbSource;


	protected override void OnCreate()
	{
		base.OnCreate();
        MagicSpellSimSystem.inst = this;
        ecbSource = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    public static void ProcessUpdate(Entity entity, int entityInQueryIndex, SpellWordNamespace.SpellWordEnum instructionElement, EntityCommandBuffer.ParallelWriter parallelWriterECB, in float deltatime)
    {
        //object[] invokeParams = new object[] { entity, entityInQueryIndex, parallelWriterECB, null, deltatime };
        //SpellWordNamespace.SpellWords.spellWordArray[(int)instructionElement].Invoke(null, invokeParams);
    } 



    public static void ProcessUpdateEntity(Entity entity, int entityInQueryIndex, in SpellDataComponent spellComponent, EntityCommandBuffer.ParallelWriter parallelWriterECB, in DynamicBuffer<SpellUpdateInstructionBuffElem> updateInstructions, in float deltatime)
	{
        //SpellDataComponent newSpellComponent = spellComponent;
        //newSpellComponent.splPosition += deltatime * newSpellComponent.splVelocity;
        //parallelWriterECB.SetComponent<SpellDataComponent>(entityInQueryIndex, entity, newSpellComponent);

    }


    protected override void OnUpdate()
    {
        //Systems execute buffers in 3 phases, initialization, simulation, and presentation, and have buffers that run at the start and end of each of these
        //This can be used to modify other components.
        EntityCommandBuffer.ParallelWriter parallelWriterECB = ecbSource.CreateCommandBuffer().AsParallelWriter();

        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        float deltatime = Time.DeltaTime;

        Entities
            .WithAll<SpellRootComponent>()
            .ForEach(
            (Entity entity, int entityInQueryIndex, in SpellDataComponent spellComponent, in DynamicBuffer<SpellUpdateInstructionBuffElem> updateInstructions) =>
                {
                    //object[] invokeParams = new object[] { entity, entityInQueryIndex, parallelWriterECB, deltatime };

                    foreach (var instruction in updateInstructions)
                    {
                        SpellWordNamespace.SpellWords.spellWordDelArray[(int)instruction.spellWordLink](ref entity, entityInQueryIndex, ref parallelWriterECB, deltatime);
                    }
                }
            ).ScheduleParallel();

        ecbSource.AddJobHandleForProducer(this.Dependency);
    }


    public T GetSpellComponent<T>(Entity spellEntity) where T : struct, IComponentData
    {
        return GetComponent<T>(spellEntity);
	}
}
