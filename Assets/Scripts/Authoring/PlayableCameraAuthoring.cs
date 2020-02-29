using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlayableCameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    int deviceInputId;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //entityReference = conversionSystem.GetPrimaryEntity(this.transform);

        dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
        dstManager.AddComponent(entity, typeof(PlayableCameraHybridData));

        dstManager.AddComponentData(entity, new PlayableCameraDeviceInputData
        {
            DeviceInputId = deviceInputId
        });

        dstManager.AddComponentData(entity, new NewPlayableCameraSpawnedTag { });
    }

    public void SetupPlayableCameraDeviceInputValue(int value)
    {
        deviceInputId = value;
    }
}
