using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class BulletSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float deltaTime = Time.DeltaTime;
        //这里只是为了展示自定义EntityCommandBuffer的用法，为了性能考虑一般还是用系统提供的ecb
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities.
            ForEach((Entity entity, ref Bullet bullet, ref Translation translation,in Rotation rot) =>
        {
            translation.Value += bullet.flySpeed * deltaTime * math.forward(rot.Value);
            bullet.lifetime-=deltaTime;
            if (bullet.lifetime <=0)
            {
                translation.Value = new float3(0, 100, 0);
                DeleteTag deleteTag = new DeleteTag
                {
                    delayTime=1f
                };
                ecb.AddComponent(entity, deleteTag);
             
            }
           
        }).Run();

        ecb.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
        ecb.Dispose();
    }
}
