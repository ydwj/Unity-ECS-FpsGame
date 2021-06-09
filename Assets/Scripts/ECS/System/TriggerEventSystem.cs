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
    BeginInitializationEntityCommandBufferSystem endSimulationEcbSystem;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endSimulationEcbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        NativeArray<bool> isbehit = new NativeArray<bool>(1, Allocator.TempJob);

        TriggerJob triggerJob = new TriggerJob
        {
            #region ÌîÈë¸÷Àà×é¼þµÄGroup
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
        Dependency = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
        Dependency.Complete();

        if (isbehit[0])
        {
            isbehit[0] = false;
            FPSGameManager.instance.PlayBehit();
        }
        isbehit.Dispose();
    }

    [BurstCompile]
    private struct TriggerJob : ITriggerEventsJob
    {
        #region ¸÷Ààgroup

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
                if (!BulletGroup.HasComponent(triggerEvent.EntityB) && BeatBackGroup.HasComponent(triggerEvent.EntityB))
                {

                    #region »÷ÍË

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

                #region É¾³ý×Óµ¯
                Translation temp = TranslationGroup[triggerEvent.EntityB];
                float3 boomPos = temp.Value;
                temp.Value = new float3(0, 100, 0);
                TranslationGroup[triggerEvent.EntityB] = temp;
                DeleteTag deleteTag = new DeleteTag
                {
                    delayTime = 1f
                };
                ecb.AddComponent(triggerEvent.EntityB, deleteTag);
                #endregion

                #region »÷ÍË
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
                #endregion

                #region ¿ÛÑª

                Hp hp = HpGroup[triggerEvent.EntityA];
                hp.HpValue--;
                HpGroup[triggerEvent.EntityA] = hp;

                #endregion

                if (hp.HpValue == 0)
                {
                    Entity boomEntity = ecb.Instantiate(boom);
                    Translation translation = new Translation
                    {
                        Value = boomPos
                    };
                    ecb.SetComponent(boomEntity, translation);
                }

            }

            if (EnemyGroup.HasComponent(triggerEvent.EntityB))
            {

                if (!BulletGroup.HasComponent(triggerEvent.EntityA) && BeatBackGroup.HasComponent(triggerEvent.EntityA))
                {

                    #region »÷ÍË
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
                //²¥·Å±»»÷ÖÐÒôÐ§
                isbehit[0] = true;

                #region É¾³ý×Óµ¯
                Translation temp = TranslationGroup[triggerEvent.EntityA];
                float3 boomPos = temp.Value;
                temp.Value = new float3(0, 100, 0);
                TranslationGroup[triggerEvent.EntityA] = temp;
                DeleteTag deleteTag = new DeleteTag
                {
                    delayTime = 1f
                };
                ecb.AddComponent(triggerEvent.EntityA, deleteTag);
                #endregion

                #region »÷ÍË
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
                #endregion

                #region ¿ÛÑª

                Hp hp = HpGroup[triggerEvent.EntityB];
                hp.HpValue--;
                HpGroup[triggerEvent.EntityB] = hp;

                if (hp.HpValue == 0)
                {
                    Entity boomEntity = ecb.Instantiate(boom);
                    Translation translation = new Translation
                    {
                        Value = boomPos
                    };
                    ecb.SetComponent(boomEntity, translation);
                }

                #endregion

            }

        }
    }

}
