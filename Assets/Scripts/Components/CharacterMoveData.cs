using Unity.Entities;
using Unity.Mathematics;

public struct CharacterMoveData : IComponentData
{
	public float x, y;

	public float Angle;
	public float3 InputDirection;
}
