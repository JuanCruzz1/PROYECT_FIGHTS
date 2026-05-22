using UnityEngine;

public enum FighterCharacter
{
    FerchoPoker,
    CruzRusher,
    KingBling
}

[CreateAssetMenu(fileName = "CharacterDatat", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public FighterCharacter fighterCharacter = FighterCharacter.FerchoPoker;
    public string characterName;
    public Sprite icon;
    public GameObject characterPrefab;

    [Header("Combat")]
    public int maxHealth = 100;
    public int punchDamage = 8;
    public int kickDamage = 12;
    public int specialDamage = 20;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpVelocity = 12f;
    public float specialCooldown = 5f;
}
