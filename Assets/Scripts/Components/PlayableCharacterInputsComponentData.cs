using Unity.Entities;
using Unity.Mathematics;

public struct PlayableCharacterInputsComponentData : IComponentData
{
    public float2 move;
    public float2 look;
}
