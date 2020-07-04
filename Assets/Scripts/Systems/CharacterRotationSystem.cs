using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(ControllableActorMoveComponentSystem))]
public class CharacterRotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithoutBurst().ForEach((ref Rotation rotation,
            in LocalToWorld localToWorld,
            in CharacterMoveData moveData) =>
        {
            //Debug.Log(moveData.InputDirection);
            if ((moveData.InputDirection.x + moveData.InputDirection.y + moveData.InputDirection.z) != 0)
            {
                //Debug.Log("moveData.angle" + moveData.Angle);
                quaternion lookrotation = quaternion.RotateY(math.radians(moveData.Angle));
                rotation.Value = lookrotation;
            }
        }).Run();

        return default;
    }
}

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(CharacterRotationSystem))]
public class CharacterTranslationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithoutBurst().ForEach((ref Translation translation, in LocalToWorld localToWorld,
            in CharacterMoveData moveData) =>
        {
            //translation.Value += localToWorld.Forward * moveData.Forward * deltaTime;
        }).Run();

        return default;
    }
}
