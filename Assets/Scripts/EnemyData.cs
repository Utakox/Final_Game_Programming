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

    [Header("Heal")]
    public bool canHeal      = false;   // เปิด/ปิดการฮิล
    public int  healAmount   = 15;      // ฮิลเท่าไหร่ต่อครั้ง
    [Range(0f, 1f)]
    public float healChance  = 0.3f;    // โอกาสฮิล (0 = ไม่ฮิลเลย, 1 = ฮิลทุกเทิร์น)
}