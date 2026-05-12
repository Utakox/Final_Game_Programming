using System.Collections.Generic;
using UnityEngine;

// วางบน Collider ของศัตรูแต่ละตัวใน world
// ไม่ต้องมี BattleManager ของตัวเอง — ใช้อันเดียวใน Scene ร่วมกัน
public class BattleTrigger : MonoBehaviour
{
    [Header("ศัตรูในการต่อสู้นี้")]
    public List<EnemyData>  enemies;
    public List<GameObject> enemyInGroup;  // GameObject ของศัตรูใน world

    bool _used;

    void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;

        // หา BattleManager อันเดียวใน Scene
        BattleManager bm = BattleManager.Instance;

        if (bm == null)
        {
            Debug.LogError("ไม่พบ BattleManager ใน Scene!");
            return;
        }

        bm.StartBattleFromTrigger(enemies, enemyInGroup);
    }
}