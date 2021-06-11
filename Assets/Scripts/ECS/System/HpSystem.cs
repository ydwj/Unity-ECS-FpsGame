using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class HpSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    }
    protected override void OnUpdate()
    {
        var ecb = endSimulationEcbSystem.CreateCommandBuffer();
        Entities.
         ForEach((Entity entity, ref DeleteTag deleteTag, in Hp hp) =>
         {
             if (hp.HpValue <= 0)
             {
                 deleteTag.lifeTime = 0;
             }
         }).Run();
    }
}
