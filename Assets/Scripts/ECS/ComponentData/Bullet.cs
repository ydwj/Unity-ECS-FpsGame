using Unity.Entities;

[GenerateAuthoringComponent]
public struct Bullet: IComponentData
{
    public float lifetime;
    public float flySpeed;
}
