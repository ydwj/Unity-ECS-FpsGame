
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;


//[UpdateAfter(typeof(TriggerEventSystem))]
[BurstCompatible]
public class DeleteSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var  ecb =endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            ForEach((Entity entity,int entityInQueryIndex, ref  DeleteTag deleteTag) =>
            {
                if (deleteTag.delayTime > 0)
                {
                    deleteTag.delayTime -= deltaTime;
                }
                else
                {
                    ecb.DestroyEntity(entityInQueryIndex,entity);
                }
               
            }).ScheduleParallel();


        // 保证ECB system依赖当前这个Job
        endSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }
}
