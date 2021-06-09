using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct BeatBack : IComponentData
{
    public float velocity;
    public float curVelocity;
    public Rotation rotation;
    public float timer;
}
