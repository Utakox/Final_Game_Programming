using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HeroSelectManager : MonoBehaviour
{
    // ── Start Screen ─────────────────────────
    [Header("Start Screen")]
    [Tooltip("ปุ่ม Start และของที่อยู่หน้าแรก")]
    public GameObject[] startOnlyObjects;   // StartButton, TitleText ฯลฯ

    // ── Hero Select Objects ───────────────────
    [Header("Hero Select (ซ่อนก่อน กด Start แล้วค่อยโชว์)")]
    public GameObject[] heroSelectObjects;  // HeroButtons, ConfirmButton, NameText, StatsText

    // ── Hero Data ────────────────────────────
    [Header("Hero List")]
    public HeroData[] heroList;

    // ── Hero Select UI ───────────────────────
    [Header("Hero Select UI")]
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroDescriptionText;
    public TextMeshProUGUI heroStatsText;
    public Button          confirmButton;

    [Header("ชื่อ Scene ที่จะไป")]
    public string worldSceneName = "Desert";

    HeroData _selected;

    // ═════════════════════════════════════════
    void Start()
    {
        SetActive(startOnlyObjects, true);
        SetActive(heroSelectObjects, false);

        if (confirmButton != null)
            confirmButton.interactable = false;
    }

    // ═════════════════════════════════════════
    //  ปุ่ม START
    // ═════════════════════════════════════════
    public void OnStartClicked()
    {
        SetActive(startOnlyObjects, false);
        SetActive(heroSelectObjects, true);
        UpdateDisplay();
    }

    // ═════════════════════════════════════════
    //  เลือก Hero
    // ═════════════════════════════════════════
    public void SelectHero(int index)
    {
        if (index < 0 || index >= heroList.Length) return;
        _selected = heroList[index];
        if (confirmButton != null) confirmButton.interactable = true;
        UpdateDisplay();
    }

    // ═════════════════════════════════════════
    //  ปุ่ม CONFIRM
    // ═════════════════════════════════════════
    public void OnConfirm()
    {
        if (_selected == null) return;
        HeroSaveManager.Save(_selected);
        SceneManager.LoadScene(worldSceneName);
    }

    // ═════════════════════════════════════════
    //  Helpers
    // ═════════════════════════════════════════
    void SetActive(GameObject[] objects, bool active)
    {
        if (objects == null) return;
        foreach (var obj in objects)
            if (obj != null) obj.SetActive(active);
    }

    void UpdateDisplay()
    {
        if (_selected == null)
        {
            if (heroNameText  != null) heroNameText.text  = "Choose a Hero";
            if (heroStatsText != null) heroStatsText.text = "";
            return;
        }

        if (heroNameText  != null) heroNameText.text  = _selected.heroName;
        if (heroStatsText != null)
            heroStatsText.text =
                "HP:  " + _selected.maxHP        + "\n" +
                "ATK: " + _selected.attackPower   + "\n" +
                "SKL: " + _selected.skillPower    + "\n" +
                "ULT: " + _selected.ultimatePower;
        if (heroDescriptionText != null)
            heroDescriptionText.text = _selected.heroDescription;
    }
}