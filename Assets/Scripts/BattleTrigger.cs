using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public GameObject    battleUI;
    public BattleManager battleManager;
    public List<EnemyData>   enemies;
    public List<GameObject>  enemyInGroup;

    bool _used;

    void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;

        // ส่งข้อมูลก่อน แล้วค่อย init
        battleManager.enemies      = enemies;
        battleManager.enemyInGroup = enemyInGroup;

        if (battleUI != null) battleUI.SetActive(true);

        // เรียก init หลังจาก set enemies เสร็จแล้ว
        battleManager.InitBattle();
    }
}