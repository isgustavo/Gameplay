using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[AlwaysSynchronizeSystem]
//[UpdateAfter(typeof(PlayableCharacterInputsComponentSystem))]
//public class PlayableCharacterMoveForwardSystem : JobComponentSystem
//{
//    EntityQuery activeCamerasQuery;

//    protected override void OnCreate()
//    {
//        base.OnCreate();
//        activeCamerasQuery = GetEntityQuery(
//            ComponentType.ReadOnly<PlayableCameraDeviceInputData>(),
//            ComponentType.ReadOnly<LocalToWorld>());
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        NativeArray<PlayableCameraDeviceInputData> activeCameras = activeCamerasQuery.ToComponentDataArray<PlayableCameraDeviceInputData>(Allocator.TempJob);
//        NativeArray<LocalToWorld> activeCamerasWorldToLocal = activeCamerasQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

//        Entities.WithoutBurst().ForEach((ref Translation translation,
//            ref MoveData moveData,
//            in PlayableCharacterInputsComponentData inputsData,
//            in PlayableCharacterDeviceInputComponentData deviceInputData) =>
//        {
//            for (int i = 0; i < activeCameras.Length; i++)
//            {
//                if (activeCameras[i].DeviceInputId == deviceInputData.DeviceInputId)
//                {
//                    float3 cameraForward = math.normalize(activeCamerasWorldToLocal[i].Forward);
//                    float3 moveFoward = inputsData.move.y * cameraForward + inputsData.move.x * activeCamerasWorldToLocal[i].Right;
//                    moveData.Turn = math.atan2(moveFoward.x, moveFoward.z);
//                    moveData.Forward = moveFoward.z;
//                }
//            }
//        }).Run();

//        activeCameras.Dispose();
//        activeCamerasWorldToLocal.Dispose();

//        return default;
//    }
//}
