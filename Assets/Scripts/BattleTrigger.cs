using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public GameObject battleUI;    // Canvas ของ battle
    public GameObject worldEnemy; // ตัวศัตรูใน world

    bool _used;

    void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;
        if (battleUI   != null) battleUI.SetActive(true);
        if (worldEnemy != null) worldEnemy.SetActive(false);
        gameObject.SetActive(false);
    }
}