using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public struct GameplayDeviceInputEvent<T> where T : struct
{
    public int deviceId;
    public T inputValue;
}

[AlwaysSynchronizeSystem]
public class PlayableCharacterInputsComponentSystem : JobComponentSystem, Input.IGameplayActions
{
    Input input;

    NativeList<GameplayDeviceInputEvent<float2>> MoveInputs;
    NativeList<GameplayDeviceInputEvent<float2>> CameraInputs;

    protected override void OnCreate()
    {
        base.OnCreate();
        input = new Input();
        input.Gameplay.SetCallbacks(this);

        MoveInputs = new NativeList<GameplayDeviceInputEvent<float2>>(Allocator.Persistent);
        CameraInputs = new NativeList<GameplayDeviceInputEvent<float2>>(Allocator.Persistent);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        input.Enable();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeList<GameplayDeviceInputEvent<float2>> moveInputs = MoveInputs;
        NativeList<GameplayDeviceInputEvent<float2>> lookInputs = CameraInputs;

        Entities.WithoutBurst()
            .ForEach((ref PlayableCharacterInputsComponentData playeableCharacterInputsData, in PlayableCharacterDeviceInputComponentData playableCharacterDeviceInputData) =>
        {
            foreach (GameplayDeviceInputEvent<float2> inputEvent in moveInputs)
            {
                if (inputEvent.deviceId == playableCharacterDeviceInputData.DeviceInputId)
                {
                    playeableCharacterInputsData.move = inputEvent.inputValue;
                    break;
                }
            }

            foreach (GameplayDeviceInputEvent<float2> inputEvent in lookInputs)
            {
                int deviceId = playableCharacterDeviceInputData.AdditionalDeviceInputId != -1 ? playableCharacterDeviceInputData.AdditionalDeviceInputId : playableCharacterDeviceInputData.DeviceInputId;
                if (inputEvent.deviceId == deviceId)
                {
                    playeableCharacterInputsData.look = inputEvent.inputValue;
                    break;
                }
            }

        }).Run();

        MoveInputs.Clear();
        CameraInputs.Clear();

        return default;
    }

    protected override void OnStopRunning()
    {
        base.OnStartRunning();

        input.Disable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (MoveInputs.IsCreated)
        {
            MoveInputs.Dispose();
        }


        if (CameraInputs.IsCreated)
        {
            CameraInputs.Dispose();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        int deviceId = context.control.device.deviceId;

        GameplayDeviceInputEvent<float2> e = new GameplayDeviceInputEvent<float2>
        {
            deviceId = deviceId,
            inputValue = context.ReadValue<Vector2>()
        };
        
        MoveInputs.Add(e);
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        int deviceId = context.control.device.deviceId;

        GameplayDeviceInputEvent<float2> e = new GameplayDeviceInputEvent<float2>
        {
            deviceId = deviceId,
            inputValue = context.ReadValue<Vector2>()
        };

        CameraInputs.Add(e);
    }

}
