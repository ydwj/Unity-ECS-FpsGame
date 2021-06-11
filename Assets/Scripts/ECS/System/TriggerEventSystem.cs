using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Burst;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

public class TriggerEventSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = endSimulationEcbSystem.CreateCommandBuffer();
        //传入两个bool值，用来判断是否播放被击中或者被击杀的音效
        NativeArray<bool> isbehit = new NativeArray<bool>(2, Allocator.TempJob);
       
        TriggerJob triggerJob = new TriggerJob
        {
            #region 填入各类组件的Group
            PhysicVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            EnemyGroup = GetComponentDataFromEntity<Enemy>(),
            BeatBackGroup = GetComponentDataFromEntity<BeatBack>(),
            RotationGroup = GetComponentDataFromEntity<Rotation>(),
            HpGroup = GetComponentDataFromEntity<Hp>(),
            BulletGroup = GetComponentDataFromEntity<Bullet>(),
            DeleteGroup = GetComponentDataFromEntity<DeleteTag>(),
            TranslationGroup = GetComponentDataFromEntity<Translation>(),
            ecb = ecb,
            PhysicsColliderGroup = GetComponentDataFromEntity<PhysicsCollider>(),
            CharacterGroup = GetComponentDataFromEntity<Character>(),
            boom = FPSGameManager.instance.boomEntity,
            isbehit = isbehit,
            #endregion
        };
        Dependency = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld,this.Dependency );
        Dependency.Complete();

        if (isbehit[0])
        {
            isbehit[0] = false;
            FPSGameManager.instance.PlayBehit();
        }
        
        if (isbehit[1])
        {
            isbehit[1] = false;
            FPSGameManager.instance.PlayBoom();
        }
        isbehit.Dispose();
    }

    [BurstCompile]
    private struct TriggerJob :ITriggerEventsJob
    {
        #region 各类group

        public ComponentDataFromEntity<PhysicsVelocity> PhysicVelocityGroup;

        public ComponentDataFromEntity<Enemy> EnemyGroup;
        public ComponentDataFromEntity<BeatBack> BeatBackGroup;
        public ComponentDataFromEntity<Rotation> RotationGroup;
        public ComponentDataFromEntity<Hp> HpGroup;

        public ComponentDataFromEntity<Bullet> BulletGroup;
        public ComponentDataFromEntity<DeleteTag> DeleteGroup;
        public ComponentDataFromEntity<Translation> TranslationGroup;
        public ComponentDataFromEntity<Character> CharacterGroup;

        public EntityCommandBuffer ecb;

        public ComponentDataFromEntity<PhysicsCollider> PhysicsColliderGroup;

        public Entity boom;

        public NativeArray<bool> isbehit;
        #endregion

        public void Execute(TriggerEvent triggerEvent)
        {

            if (EnemyGroup.HasComponent(triggerEvent.EntityA))
            {
                //敌人与主角碰撞效果
                if (!BulletGroup.HasComponent(triggerEvent.EntityB) && BeatBackGroup.HasComponent(triggerEvent.EntityB))
                {

                    #region 击退

                    BeatBack beatBack1 = BeatBackGroup[triggerEvent.EntityB];

                    if (beatBack1.curVelocity > 0.1f)
                    {
                        beatBack1.velocity += (5f - beatBack1.curVelocity) * 0.1f;

                    }
                    else
                    {
                        beatBack1.velocity = 5f;
                    }
                    if (RotationGroup.HasComponent(triggerEvent.EntityB))
                    {
                        Rotation rotation = RotationGroup[triggerEvent.EntityB];
                        beatBack1.rotation = rotation;
                    }

                    BeatBackGroup[triggerEvent.EntityB] = beatBack1;
                    #endregion
                    return;
                }
                isbehit[0] = true;

                #region 删除子弹

                float3 boomPos = float3.zero;
                if (TranslationGroup.HasComponent(triggerEvent.EntityA))
                {
                    Translation temp = TranslationGroup[triggerEvent.EntityB];
                    boomPos = temp.Value;
                    temp.Value = new float3(0, 100, 0);
                    TranslationGroup[triggerEvent.EntityB] = temp;
                    if (DeleteGroup.HasComponent(triggerEvent.EntityB))
                    {
                       DeleteTag temp1 = DeleteGroup[triggerEvent.EntityB];
                       temp1.lifeTime = 0f;
                        DeleteGroup[triggerEvent.EntityB] = temp1;
                    }
                   
                }

                #endregion

                #region 子弹击退敌人效果
                if (BeatBackGroup.HasComponent(triggerEvent.EntityA))
                {
                    BeatBack beatBack = BeatBackGroup[triggerEvent.EntityA];

                    if (beatBack.curVelocity > 0.1f)
                    {
                        beatBack.velocity += (5f - beatBack.curVelocity) * 0.1f;

                    }
                    else
                    {
                        beatBack.velocity = 5f;
                    }
                    if (RotationGroup.HasComponent(triggerEvent.EntityB))
                    {
                        Rotation rotation = RotationGroup[triggerEvent.EntityB];
                        beatBack.rotation = rotation;
                    }

                    BeatBackGroup[triggerEvent.EntityA] = beatBack;
                }

                #endregion

                #region 扣血并生成爆炸粒子实体
                if (HpGroup.HasComponent(triggerEvent.EntityA))
                {
                    Hp hp = HpGroup[triggerEvent.EntityA];
                    hp.HpValue--;
                    HpGroup[triggerEvent.EntityA] = hp;
                    if (hp.HpValue == 0)
                    {
                        //播放死亡音效
                        isbehit[1] = true;
                        Entity boomEntity = ecb.Instantiate(boom);
                        Translation translation = new Translation
                        {
                            Value = boomPos
                        };
                        ecb.SetComponent(boomEntity, translation);
                    }
                }
            

                #endregion
            }

            if (EnemyGroup.HasComponent(triggerEvent.EntityB))
            {

                if (!BulletGroup.HasComponent(triggerEvent.EntityA) && BeatBackGroup.HasComponent(triggerEvent.EntityA))
                {

                    #region 击退
                    BeatBack beatBack1 = BeatBackGroup[triggerEvent.EntityA];

                    if (beatBack1.curVelocity > 0.1f)
                    {
                        beatBack1.velocity += (6f - beatBack1.curVelocity) * 0.1f;

                    }
                    else
                    {
                        beatBack1.velocity = 6f;
                    }
                    if (RotationGroup.HasComponent(triggerEvent.EntityA))
                    {
                        Rotation rotation = RotationGroup[triggerEvent.EntityA];
                        beatBack1.rotation = rotation;
                    }

                    BeatBackGroup[triggerEvent.EntityA] = beatBack1;
                    #endregion
                    return;
                }
                //播放被击中音效
                isbehit[0] = true;

                #region 删除子弹
                float3 boomPos = float3.zero;
                if (TranslationGroup.HasComponent(triggerEvent.EntityA))
                {
                    Translation temp = TranslationGroup[triggerEvent.EntityA];
                    boomPos = temp.Value;
                    temp.Value = new float3(0, 100, 0);
                    TranslationGroup[triggerEvent.EntityA] = temp;
                    if (DeleteGroup.HasComponent(triggerEvent.EntityA))
                    {
                        DeleteTag temp1 = DeleteGroup[triggerEvent.EntityA];
                        temp1.lifeTime = 0f;
                        DeleteGroup[triggerEvent.EntityA] = temp1;
                    }
                }


                #endregion

                #region 击退
                if (BeatBackGroup.HasComponent(triggerEvent.EntityB))
                {
                    BeatBack beatBack = BeatBackGroup[triggerEvent.EntityB];
                    if (beatBack.curVelocity > 0.1f)
                    {
                        beatBack.velocity = (6f - beatBack.curVelocity) * 0.1f;

                    }
                    else
                    {
                        beatBack.velocity = 6f;
                    }
                    if (RotationGroup.HasComponent(triggerEvent.EntityA))
                    {
                        Rotation rotation = RotationGroup[triggerEvent.EntityA];
                        beatBack.rotation = rotation;
                    }
                    BeatBackGroup[triggerEvent.EntityB] = beatBack;
                }


                #endregion

                #region 扣血并生成爆炸粒子实体
                if (HpGroup.HasComponent(triggerEvent.EntityB))
                {
                    Hp hp = HpGroup[triggerEvent.EntityB];
                    hp.HpValue--;
                    HpGroup[triggerEvent.EntityB] = hp;

                    if (hp.HpValue == 0)
                    {
                        //播放死亡音效
                        isbehit[1] = true;
                        Entity boomEntity = ecb.Instantiate(boom);
                        Translation translation = new Translation
                        {
                            Value = boomPos
                        };
                        ecb.SetComponent(boomEntity, translation);
                    }
                }

                #endregion

            }
        }
    }

}
