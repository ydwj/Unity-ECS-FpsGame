using Unity.Entities;
using Unity.Jobs;


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
         // 请求一个ECS并且转换成可并行的
        var ecb = endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
           .ForEach((Entity entity, int entityInQueryIndex, in DeleteTag deleteTag) =>
      {
          if (deleteTag.lifeTime <=0f)
          {
              ecb.DestroyEntity(entityInQueryIndex, entity);
            
          }
      }).ScheduleParallel();

        // 保证ECB system依赖当前这个Job
        endSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
