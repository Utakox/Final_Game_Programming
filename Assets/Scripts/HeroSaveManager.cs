using UnityEngine;
using System.IO;

public static class HeroSaveManager
{
    static string SavePath()
    {
        return Application.persistentDataPath + "/hero.json";
    }

    public static void Save(HeroData h)
    {
        HeroSaveData saveData       = new HeroSaveData();
        saveData.heroName           = h.heroName;
        saveData.heroClass          = h.heroClass;
        saveData.maxHP              = h.maxHP;
        saveData.attackPower        = h.attackPower;
        saveData.skillPower         = h.skillPower;
        saveData.maxSP              = h.maxSP;
        saveData.spCost             = h.spCost;
        saveData.spGainAttack       = h.spGainAttack;
        saveData.spGainSkill        = h.spGainSkill;
        saveData.ultimatePower      = h.ultimatePower;
        saveData.ultimateCost       = h.ultimateCost;
        saveData.ultGainAttack      = h.ultGainAttack;
        saveData.ultGainSkill       = h.ultGainSkill;
        saveData.canHeal            = h.canHeal;
        saveData.healAmount         = h.healAmount;
        saveData.doubleHitChance    = h.doubleHitChance;
        saveData.execDoubleHitChance = h.execDoubleHitChance;
        saveData.execBonusUlt       = h.execBonusUlt;
        saveData.atkDebuff          = h.atkDebuff;


        string json = JsonUtility.ToJson(saveData, prettyPrint: true);
        File.WriteAllText(SavePath(), json);
        Debug.Log("Hero saved: " + SavePath());
    }

    public static HeroSaveData Load()
    {
        string path = SavePath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("ไม่พบไฟล์ save: " + path);
            return null;
        }

        string json      = File.ReadAllText(path);
        HeroSaveData d   = JsonUtility.FromJson<HeroSaveData>(json);
        Debug.Log("Hero loaded: " + d.heroName);
        return d;
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath());
    }
}