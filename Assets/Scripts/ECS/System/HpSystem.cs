using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;

//[UpdateAfter(typeof(TriggerEventSystem))]
public class HpSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
    private EntityQuery query;
    protected override void OnCreate()
    {
        base.OnCreate();
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();

        Entities.
            WithoutBurst().
            ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in Hp hp) =>
            {
                if (hp.HpValue <= 0)
                {
                    ecb.DestroyEntity(entity);
                    FPSGameManager.instance.PlayBoom();
                   
                }

            }).Run();
        this.CompleteDependency();
    }
}
