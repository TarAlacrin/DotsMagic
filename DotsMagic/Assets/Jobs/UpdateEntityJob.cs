using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics;

[Serializable]
public struct UpdateEntityJob : IJobParallelFor
{
	public NativeArray<Entity> entitiesToOperateUpon;
	
	ComponentDataFromEntity<Rotation> rotationFromEntity;
	ComponentDataFromEntity<Translation> translationFromEntity;
	ComponentDataFromEntity<PhysicsVelocity> velocityFromEntity;

	void IJobParallelFor.Execute(int index)
	{
		Entity relavent = entitiesToOperateUpon[index];

		var job = new UpdateEntityJob();

		JobHandle jobhand = job.Schedule(1024, 64);


	}
}
