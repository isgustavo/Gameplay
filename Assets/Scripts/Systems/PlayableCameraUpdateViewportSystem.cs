using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
[UpdateBefore(typeof(SpawnPlayableCharacterSystem))]
public class PlayableCameraUpdateViewportSystem : JobComponentSystem
{
    EntityQuery activeCamerasQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        activeCamerasQuery = GetEntityQuery(ComponentType.ReadOnly<PlayableCameraDeviceInputData>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        bool wasCameraSpawnedThisFrame = false;
        Entities
            .WithoutBurst()
            .ForEach((Entity e, NewPlayableCameraSpawnedTag camera) =>
            {
                wasCameraSpawnedThisFrame = true;
                ecb.RemoveComponent<NewPlayableCameraSpawnedTag>(e);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        if (wasCameraSpawnedThisFrame)
        {
            NativeArray<PlayableCameraDeviceInputData> activeCameras = activeCamerasQuery.ToComponentDataArray<PlayableCameraDeviceInputData>(Allocator.TempJob);

            int index = 0;
            Entities
                .WithoutBurst() // PlayableCameraHybridData until there inst a scene manager to select game level
                .ForEach((PlayableCameraHybridData i, Camera camera) =>
                {

                    switch (activeCameras.Length)
                    {
                        case 2:
                            UpdateViewportForTwo(index, camera);
                            break;
                        case 3:
                            UpdateViewportForThree(index, camera);
                            break;
                        case 4:
                            UpdateViewportForFour(index, camera);
                            break;
                    }
                    index += 1;
                }).Run();

            activeCameras.Dispose();
        }

        return default;
    }

    private void UpdateViewportForTwo(int index, Camera camera)
    {
        switch (index)
        {
            case 0:
                camera.rect = new Rect(0f, 0f, .5f, 1f);
                break;
            case 1:
                camera.rect = new Rect(.5f, 0f, .5f, 1f);
                break;
        }
    }

    private void UpdateViewportForThree(int index, Camera camera)
    {
        switch (index)
        {
            case 0:
                camera.rect = new Rect(0f, 0f, .333f, 1f);
                break;
            case 1:
                camera.rect = new Rect(.333f, 0f, .333f, 1f);
                break;
            case 2:
                camera.rect = new Rect(.666f, 0f, .333f, 1f);
                break;
        }
    }


    private void UpdateViewportForFour(int index, Camera camera)
    {
        switch (index)
        {
            case 0:
                camera.rect = new Rect(0f, .5f, .5f, .5f);
                break;
            case 1:
                camera.rect = new Rect(.5f, .5f, .5f, 1f);
                break;
            case 2:
                camera.rect = new Rect(.0f, 0f, .5f, .5f);
                break;
            case 4:
                camera.rect = new Rect(.5f, 0f, .5f, .5f);
                break;
        }
    }
}