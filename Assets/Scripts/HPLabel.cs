using UnityEngine;
using TMPro;

public class HPLabel : MonoBehaviour
{
    public TextMeshPro hpText;

    void LateUpdate()
    {
        // หันตาม BattleCamera ก่อน ถ้าไม่มีค่อยหันตาม Main Camera
        Camera cam = null;

        if (BattleStage.Instance != null && BattleStage.Instance.battleCamera != null
            && BattleStage.Instance.battleCamera.gameObject.activeInHierarchy)
        {
            cam = BattleStage.Instance.battleCamera;
        }

        if (cam == null)
            cam = Camera.main;

        if (cam != null)
            transform.forward = cam.transform.forward;
    }

    public void SetHP(int current, int max)
    {
        if (hpText == null) return;
        hpText.text = current + " / " + max;
    }

    public void SetLabel(string label)
    {
        if (hpText == null) return;
        hpText.text = label;
    }
}