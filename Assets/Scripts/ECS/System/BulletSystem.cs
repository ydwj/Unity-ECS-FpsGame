using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class BulletSystem : SystemBase
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
        var ecb = endSimulationEcbSystem.CreateCommandBuffer();
        Entities.
        ForEach(( ref Translation translation, ref DeleteTag deleteTag, in Rotation rot, in Bullet bullet) =>
        {
            translation.Value += bullet.flySpeed * deltaTime * math.forward(rot.Value);
            deleteTag.lifeTime-= deltaTime;
         

        }).Run();

    }
}
