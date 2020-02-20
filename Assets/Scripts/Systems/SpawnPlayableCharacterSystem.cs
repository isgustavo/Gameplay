using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;

[AlwaysSynchronizeSystem]
public class SpawnPlayableCharacterSystem : JobComponentSystem
{
    EntityQuery activeDeviceInputsQuery;
    Entity playableCharacterEntity;

    protected override void OnCreate()
    {
        base.OnCreate();

        GameObject prefab = Resources.Load<GameObject>("Sphere");
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        playableCharacterEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);

        activeDeviceInputsQuery = GetEntityQuery(ComponentType.ReadOnly<PlayableCharacterDeviceInputComponentData>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<PlayableCharacterDeviceInputComponentData> activePlayableCharactersInputs = activeDeviceInputsQuery.ToComponentDataArray<PlayableCharacterDeviceInputComponentData>(Allocator.TempJob);
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithoutBurst()
            .ForEach((ref LevelInfosComponentData levelInfosData) =>
        {
            if (levelInfosData.currentPlayableCharacters < levelInfosData.playableCharacterAmount)
            {
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    bool isKeyboardInGame = false;
                    foreach (PlayableCharacterDeviceInputComponentData deviceInput in activePlayableCharactersInputs)
                    {
                        if (deviceInput.DeviceInputId == Keyboard.current.deviceId)
                        {
                            isKeyboardInGame = true;
                            break;
                        }
                    }

                    if (isKeyboardInGame == false)
                    {
                        levelInfosData.currentPlayableCharacters += 1;
                        SpawnPlayableCharacter(ecb, Keyboard.current.deviceId, Mouse.current.deviceId);
                        return;
                    }
                }

                foreach (var device in Gamepad.all)
                {
                    if (device.crossButton.wasPressedThisFrame)
                    {
                        bool isGamepadInGame = false;
                        foreach (PlayableCharacterDeviceInputComponentData deviceInput in activePlayableCharactersInputs)
                        {
                            if (deviceInput.DeviceInputId == device.deviceId)
                            {
                                isGamepadInGame = true;
                                break;
                            }
                        }

                        if (isGamepadInGame == false)
                        {
                            levelInfosData.currentPlayableCharacters += 1;
                            SpawnPlayableCharacter(ecb, device.deviceId);
                            return;
                        }
                    }
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        activePlayableCharactersInputs.Dispose();

        return default;
    }

    private void SpawnPlayableCharacter(EntityCommandBuffer ecb, int deviceInputId, int mouseInputId = -1)
    {
        Entity playableCharacter = ecb.Instantiate(playableCharacterEntity);
        ecb.AddComponent(playableCharacter, new PlayableCharacterDeviceInputComponentData {
            DeviceInputId = deviceInputId,
            AdditionalDeviceInputId = mouseInputId });
        ecb.AddComponent(playableCharacter, new PlayableCharacterInputsComponentData { });
    }
}
