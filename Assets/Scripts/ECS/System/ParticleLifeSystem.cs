
using Unity.Entities;
using Unity.Jobs;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class ParticleLifeSystem : SystemBase
{
    
    BeginInitializationEntityCommandBufferSystem endSimulationEcbSystem;
    protected override void OnCreate()
    {
        endSimulationEcbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();

        Entities.ForEach((Entity entity,ref ParticleLifetime particleLife) =>
        {
            particleLife.lifetime -= deltaTime;
            if (particleLife.lifetime <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }).Run() ;
    }
}
