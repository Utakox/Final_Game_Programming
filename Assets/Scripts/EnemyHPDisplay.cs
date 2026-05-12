using UnityEngine;
using TMPro;

public class EnemyHPDisplay : MonoBehaviour
{
    public EnemyData   enemyData;
    public TextMeshPro hpText;

    int _currentHP;

    void Start()
    {
        if (enemyData == null) return;
        _currentHP = enemyData.maxHP;
        Refresh();
    }

    void LateUpdate()
    {
        if (hpText == null) return;

        Camera cam = null;

        if (BattleStage.Instance != null
            && BattleStage.Instance.battleCamera != null
            && BattleStage.Instance.battleCamera.gameObject.activeInHierarchy)
        {
            cam = BattleStage.Instance.battleCamera;
        }

        if (cam == null) cam = Camera.main;
        if (cam != null) hpText.transform.forward = cam.transform.forward;
    }

    public void UpdateHP(int current)
    {
        _currentHP = current;
        Refresh();
    }

    void Refresh()
    {
        if (hpText != null)
            hpText.text = _currentHP + " / " + enemyData.maxHP;
    }
}