using Unity.Entities;

//手枪，霰弹枪，自动模式
public enum WeaponType
{
    gun,
    shotgun,
    gunAutoshot
}
[GenerateAuthoringComponent]
public struct  Weapon : IComponentData
{
    //枪口位置
    public Entity gunPoint;
    //武器类型
    public WeaponType weaponType;
    //是否允许切换武器
    public bool canSwitch;

    //开枪间隔
    public float firingInterval;
    //用来记录每次开枪的时间
    public float shotTime;
}
