
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
public class WeaponSystem : SystemBase
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
        float time = UnityEngine.Time.time;
        EntityCommandBuffer ecb = endSimulationEcbSystem.CreateCommandBuffer();
        Entities.
            WithoutBurst().
            ForEach((ref Weapon weapon, in Rotation rotation) =>
        {

            if (weapon.weaponType == WeaponType.gunAutoshot)
            {

                if (weapon.shotTime == -1f)
                {
                    weapon.shotTime = time;
                }

                //  Debug.Log("当前时间" + time);
                if (time - weapon.shotTime >= weapon.firingInterval)
                {

                    weapon.shotTime = time;
                    float3 pos = new float3();
                    LocalToWorld gunPointL2w = new LocalToWorld();

                    if (HasComponent<LocalToWorld>(weapon.gunPoint))
                    {
                        gunPointL2w = GetComponent<LocalToWorld>(weapon.gunPoint);

                        Entity tempBullet = ecb.Instantiate(FPSGameManager.instance.bulletEntity);

                        Translation translation = new Translation
                        {
                            Value = gunPointL2w.Position
                        };
                        Rotation rot = new Rotation
                        {
                            Value = rotation.Value
                        };

                        Bullet bullet = new Bullet
                        {

                            flySpeed = 20,
                        };
                        DeleteTag deleteTag = new DeleteTag
                        {
                            lifeTime = 1f
                        };

                        ecb.SetComponent(tempBullet, translation);
                        ecb.SetComponent(tempBullet, rot);
                        ecb.SetComponent(tempBullet, bullet);
                        ecb.SetComponent(tempBullet, deleteTag);
                        FPSGameManager.instance.PlayShoot();

                    }
                }
                return;
            }

            if (weapon.canSwitch)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (weapon.weaponType == WeaponType.gun)
                    {
                        weapon.weaponType = WeaponType.shotgun;
                    }
                    else
                    {
                        weapon.weaponType = WeaponType.gun;
                    }
                }
            }


            #region 开枪
            if (Input.GetKeyDown(KeyCode.J))
            {
                float3 pos = new float3();
                LocalToWorld gunPointL2w = new LocalToWorld();

                if (HasComponent<LocalToWorld>(weapon.gunPoint))
                {
                    gunPointL2w = GetComponent<LocalToWorld>(weapon.gunPoint);
                    pos = gunPointL2w.Position;
                }

                switch (weapon.weaponType)
                {
                    case WeaponType.gun:
                        #region 手枪
                        Entity tempBullet = ecb.Instantiate(FPSGameManager.instance.bulletEntity);

                        Translation translation = new Translation
                        {
                            Value = pos
                        };
                        Rotation rot = new Rotation
                        {
                            Value = rotation.Value
                        };

                        Bullet bullet = new Bullet
                        {

                            flySpeed = 20,

                        };
                        DeleteTag deleteTag = new DeleteTag
                        {
                            lifeTime = 1f
                        };

                        ecb.SetComponent(tempBullet, translation);
                        ecb.SetComponent(tempBullet, rot);
                        ecb.SetComponent(tempBullet, bullet);
                        ecb.SetComponent(tempBullet, deleteTag);

                        FPSGameManager.instance.PlayShoot();
                        #endregion
                        break;
                    case WeaponType.shotgun:
                        #region  霰弹枪
                        Translation translation2 = new Translation
                        {
                            Value = pos
                        };

                        Bullet bullet1 = new Bullet
                        {
                            flySpeed = 25,
                        };
                        DeleteTag deleteTag2= new DeleteTag
                        {
                           lifeTime= 0.5f
                        };

                        for (int i = -5; i < 5; i++)
                        {
                            Entity tempBullet2 = ecb.Instantiate(FPSGameManager.instance.bulletEntity);

                            #region 传统写法
                            //Quaternion q = rotation.Value;
                            //Quaternion tempRot = Quaternion.Euler(0, q.eulerAngles.y + i * 7, 0);
                            #endregion

                            //这里默认按照弧度旋转
                            quaternion temp = math.mul( quaternion.EulerXYZ(0, i *0.1f, 0), rotation.Value) ;
                            Rotation rotation2 = new Rotation
                            {
                                Value = temp 
                            };
                            ecb.SetComponent(tempBullet2, translation2);
                            ecb.SetComponent(tempBullet2, rotation2);
                            ecb.SetComponent(tempBullet2, bullet1);
                            ecb.SetComponent(tempBullet2, deleteTag2);
                        }

                        FPSGameManager.instance.PlayShoot();
                        #endregion
                        break;
                    default:
                        break;
                }

            }
            #endregion

        }).Run();
    }
}
