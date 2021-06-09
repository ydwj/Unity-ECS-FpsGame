using Unity.Entities;


[GenerateAuthoringComponent]
public struct Enemy : IComponentData
{
    public float speed;
    public Entity targetEntity;
}
