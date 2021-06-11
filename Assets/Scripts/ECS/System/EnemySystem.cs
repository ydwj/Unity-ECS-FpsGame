using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;


public class EnemySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
    //保存筛选出来的敌人的对象
    private EntityQuery query;
    private uint seed = 1;

    protected override void OnCreate()
    {
        base.OnCreate();
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed++);

        float deltaTime = Time.DeltaTime;
     
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        Entity template = FPSGameManager.instance.enemyEntity;

        Entities.
            WithStoreEntityQueryInField(ref query).
            ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref Enemy enemy) =>
        {

            if (HasComponent<LocalToWorld>(enemy.targetEntity))
            {
                //追踪主角
                LocalToWorld targetl2w = GetComponent<LocalToWorld>(enemy.targetEntity);
                float3 targetPos = targetl2w.Position;
                translation.Value = Vector3.MoveTowards(translation.Value, targetPos, enemy.speed * deltaTime);
                var targetDir = targetPos - translation.Value;
                quaternion temp1 = quaternion.LookRotation(targetDir, math.up());
                rotation.Value = temp1;
            }

        }).Run();

        //敌人数量少于6,在主角周围新生成6个敌人
        if (query.CalculateEntityCount() < 6)
        {
            Entity characterEntity = GetSingletonEntity<Character>();
            float3 characterPos=float3.zero;
            if (characterEntity!=Entity.Null)
            { 
                if (HasComponent<Translation>(characterEntity))
                {
                    Translation translation = GetComponent<Translation>(characterEntity);
                    characterPos = translation.Value;
                }
            }

            for (int i = 0; i < 6; i++)
            {
                Entity temp = ecb.Instantiate(template);

                #region 随机位置生成敌人
                float max= 20f;
                float x = random.NextFloat(characterPos.x - max, characterPos.x + max);
                float z = random.NextFloat(characterPos.z - max, characterPos.z + max);

                if (x < 0 && x > -max/2)
                {
                    x -= max / 2;
                }
                else if (x >= 0 && x < max/2)
                {
                    x += max / 2;
                }

                if (z < 0 && z > -max / 2)
                {
                    z -= max / 2;
                }
                else if (z >= 0 && z < max / 2)
                {
                    z += max / 2;
                } 
                #endregion

                Translation translation = new Translation
                {
                    Value=new float3(x,characterPos.y,z)
                };

                Enemy enemy = new Enemy
                {
                    speed=5f ,
                    targetEntity=characterEntity
                };

                ecb.SetComponent(temp, translation);
                ecb.SetComponent(temp, enemy);
                      
               
            }
        }
    }
}
