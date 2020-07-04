using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(PlayableCharacterInputsComponentSystem))]
public class ControllableActorMoveComponentSystem : JobComponentSystem
{
    EntityQuery activeCamerasQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        activeCamerasQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayableCameraDeviceInputData>(),
            ComponentType.ReadOnly<LocalToWorld>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<PlayableCameraDeviceInputData> activeCameras = activeCamerasQuery.ToComponentDataArray<PlayableCameraDeviceInputData>(Allocator.TempJob);
        NativeArray<LocalToWorld> activeCamerasWorldToLocal = activeCamerasQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        Entities.WithoutBurst().ForEach((ref CharacterMoveData moveData,
            in LocalToWorld localToWorld,
            in PlayableCharacterInputsComponentData inputsData,
            in PlayableCharacterDeviceInputComponentData deviceInputData) =>
        {
            for (int i = 0; i < activeCameras.Length; i++)
            {
                if (activeCameras[i].DeviceInputId == deviceInputData.DeviceInputId)
                {
                    moveData.x = inputsData.move.x;
                    moveData.y = inputsData.move.y;

                    float3 cameraForward = math.normalize(activeCamerasWorldToLocal[i].Forward * new float3(1, 0, 1));
                    moveData.InputDirection = inputsData.move.y * cameraForward + inputsData.move.x * activeCamerasWorldToLocal[i].Right;

                    float3 axisSign = math.cross(moveData.InputDirection, localToWorld.Forward);

                    float angleToMove = Angle(localToWorld.Forward, moveData.InputDirection) * (axisSign.y >= 0 ? -1f : 1f);

                    float angleRounded = math.round(angleToMove / 90f) * 90f;

                    moveData.Angle = angleRounded;
                }
            }
        }).Run();

        activeCameras.Dispose();
        activeCamerasWorldToLocal.Dispose();

        return default;
    }

    float Angle(float3 from, float3 to)
    {
        return math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1f, 1f)) * 57.29578f;
    }

}