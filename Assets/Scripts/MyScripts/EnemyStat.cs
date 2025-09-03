using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat", menuName = "Stats/EnemyStat")]
public class EnemyStat : ScriptableObject
{
    public EnemyType enemyType; // e.g. "Goblin", "Orc", "Demon"
    public int health;
    public int damage;
    // Add other stats as needed
}

public enum EnemyType
{
    Goblin,
    Orc,
    Demon,
    Troll
}