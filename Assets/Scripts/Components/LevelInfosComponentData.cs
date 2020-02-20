using Unity.Entities;

[GenerateAuthoringComponent]
public struct LevelInfosComponentData : IComponentData
{
    public int playableCharacterAmount;
    public int currentPlayableCharacters;
}
