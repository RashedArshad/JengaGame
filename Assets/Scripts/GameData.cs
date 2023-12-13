using UnityEngine;
[CreateAssetMenu(fileName = "GameData", menuName = "Scriptables/Game Data")]
public class GameData : ScriptableObject
{
    public BlockGameObject BlockPrefab;

    [Space(5)]
    [Header("Block Materials ")]
    public Material GlassMaterial;
    public Material WoodMaterial;
    public Material StoneMaterial;
}
