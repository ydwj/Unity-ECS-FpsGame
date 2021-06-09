using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Hp : IComponentData
{
    public float  HpValue;
}
