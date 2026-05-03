using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName   = "Enemy";
    public int    maxHP       = 80;
    public int    attackPower = 10;
    public int    skillPower  = 20;
    [Range(0f, 1f)]
    public float  normalAttackChance = 0.6f;
}