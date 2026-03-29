// Assets/Scripts/Data/CardData.cs
using UnityEngine;

public enum CardType { Warrior, Archer, Cavalry, Spell, Building, Hero }
public enum Rarity { Common, Rare, Epic, Legendary }
public enum TargetType { Ground, Air, Both, Building }

[CreateAssetMenu(fileName = "NewCard", menuName = "Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public int id;
    public string cardName;
    public string description;
    public CardType cardType;
    public Rarity rarity;

    [Header("Costs")]
    public int elixirCost;

    [Header("Unit Stats (for unit cards)")]
    public int hp;
    public int damage;
    public float attackSpeed = 1f;
    public float moveSpeed = 1f;
    public float attackRange = 1f;
    public TargetType targetType = TargetType.Ground;

    [Header("Spell Stats")]
    public bool isSpell = false;
    public float spellRadius = 0f;
    public int spellDamage = 0;
    public float spellDuration = 0f; // For freeze etc

    [Header("Visuals")]
    public Sprite cardArt;
    public GameObject unitPrefab;
}