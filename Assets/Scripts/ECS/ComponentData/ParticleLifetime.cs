using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleLifetime : IComponentData
{
    public float lifetime;
}
