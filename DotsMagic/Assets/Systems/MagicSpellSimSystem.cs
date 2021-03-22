using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MagicSpellSimSystem : SystemBase
{

    private EntityCommandBufferSystem ecbSource;


	protected override void OnCreate()
	{
		base.OnCreate();

        ecbSource = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
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
            (Entity entity, int entityInQueryIndex, in SpellDataComponent spellComponent) =>
                {
                    SpellDataComponent newSpellComponent = spellComponent;
                    newSpellComponent.splPosition += deltatime * newSpellComponent.splVelocity;
                    parallelWriterECB.SetComponent<SpellDataComponent>(entityInQueryIndex, entity, newSpellComponent);
                }
            ).ScheduleParallel();

        ecbSource.AddJobHandleForProducer(this.Dependency);
    }
}
