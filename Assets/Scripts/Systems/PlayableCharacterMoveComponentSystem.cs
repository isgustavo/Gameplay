using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class ControllableActorMoveComponentSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithoutBurst().ForEach((ref Translation translation, in PlayableCharacterInputsComponentData inputsData) =>
        {
            translation.Value.x = translation.Value.x + inputsData.move.x * 2 * deltaTime;
            translation.Value.y = 1;
            translation.Value.z = translation.Value.z + inputsData.move.y * 2 * deltaTime;

        }).Run();

        return default;
    }
}
