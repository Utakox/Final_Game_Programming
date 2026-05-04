using UnityEngine;
using TMPro;

// แขวนบน GameObject ของตัวละคร
// ต้องมี TextMeshPro (World Space) เป็น child
// text จะหันหน้าตาม Camera ตลอด
public class HPLabel : MonoBehaviour
{
    public TextMeshPro hpText;   // ลาก TMP (World Space) มาใส่

    void LateUpdate()
    {
        // หันหน้าตาม main camera เสมอ
        if (Camera.main != null)
            hpText.transform.forward = Camera.main.transform.forward;
    }

    public void SetHP(int current, int max)
    {

        hpText.text = $"{current} / {max}";
    }

    public void SetLabel(string label)
    {
        hpText.text = label;
    }
}