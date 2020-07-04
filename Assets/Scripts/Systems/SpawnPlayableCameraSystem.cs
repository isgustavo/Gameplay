using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(SpawnPlayableCharacterSystem))]
public class SpawnPlayableCameraSystem : JobComponentSystem
{
    EntityQuery activeCamerasQuery;

    GameObject PlayableCameraPrefab;

    protected override void OnCreate()
    {
        base.OnCreate();

        PlayableCameraPrefab = Resources.Load<GameObject>("Camera");
        activeCamerasQuery = GetEntityQuery(ComponentType.ReadOnly<PlayableCameraDeviceInputData>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<PlayableCameraDeviceInputData> activeCameras = activeCamerasQuery.ToComponentDataArray<PlayableCameraDeviceInputData>(Allocator.TempJob);
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithoutBurst()
            .ForEach((in PlayableCharacterDeviceInputComponentData playableCharacterDeviceInput) =>
            {
                bool hasPlayableCharacterCamera = false;
                foreach (PlayableCameraDeviceInputData camera in activeCameras)
                {
                    if (camera.DeviceInputId == playableCharacterDeviceInput.DeviceInputId)
                    {
                        hasPlayableCharacterCamera = true;
                        break;
                    }
                }

                if (hasPlayableCharacterCamera == false)
                {
                    SpawnPlayableCamera(ecb, playableCharacterDeviceInput.DeviceInputId);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        activeCameras.Dispose();

        return default;
    }

    private void SpawnPlayableCamera(EntityCommandBuffer ecb, int deviceInputId)
    {
        GameObject obj = GameObject.Instantiate(PlayableCameraPrefab, new Vector3(0f, 2f, -5f), Quaternion.identity);
        obj.GetComponent<PlayableCameraAuthoring>().SetupPlayableCameraDeviceInputValue(deviceInputId);

        //GameObject camera = new GameObject();
        //camera.AddComponent<Camera>();
        //camera.transform.position = new Vector3(0f, 5f, -10f);
        //camera.transform.rotation = new Quaternion(.15f, 0.0f, 0f, 0.85f);
        //PlayableCameraHybridData playableCameraHybridData = camera.AddComponent<PlayableCameraHybridData>();
        //playableCameraHybridData.DeviceInputId = deviceInputId;

        //Entity playableCamera = ecb.CreateEntity();
        //ecb.AddComponent(playableCamera, new PlayableCameraDeviceInputData { DeviceInputId = deviceInputId });
        //ecb.AddComponent(playableCamera, new NewPlayableCameraSpawnedTag { });

    }
}
