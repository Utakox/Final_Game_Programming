using UnityEngine;
using UnityEngine.Video;

public enum HeroClass
{
    Swordmaster,
    Executioner,
    Archer,
    Archmage,
    Ambassador
}

[CreateAssetMenu(menuName = "Battle/Hero Data")]
public class HeroData : ScriptableObject
{
    [Header("ข้อมูลทั่วไป")]
    public string heroName        = "Hero";
    public string heroDescription = "A hero description";
    public HeroClass heroClass    = HeroClass.Swordmaster;

    [Header("Stats")]
    public int maxHP         = 120;
    public int attackPower   = 15;
    public int skillPower    = 30;

    [Header("Skill Point")]
    public int maxSP        = 5;
    public int spCost       = 2;
    public int spGainAttack = 1;
    public int spGainSkill  = 0;

    [Header("Ultimate")]
    public int ultimatePower = 80;
    public int ultimateCost  = 100;
    public int ultGainAttack = 10;
    public int ultGainSkill  = 20;

    [Header("Archmage")] // ฮิลตัวเองตอนใช้ Skill
    public bool canHeal   = false;
    public int  healAmount = 0;

    [Header("Swordmaster")] // โอกาสตี 2 ครั้งตอนใช้ Skill
    [Range(0f, 1f)]
    public float doubleHitChance = 0.5f;

    [Header("Executioner")] // โอกาสตี 2 ครั้งตอนตีปกติ + โบนัสเพิ่ม Ultimate
    [Range(0f, 1f)]
    public float execDoubleHitChance = 0.4f;
    public int   execBonusUlt        = 20;

    [Header("Ambassador")] // ลด ATK ตอนใช้สกิล 
    public int atkDebuff = 1;

    [Header("Animation Videos")]
    [Tooltip("Video ที่เล่นตอนใช้ Skill (เฉพาะ Hero นี้)")]
    public VideoClip skillVideo;

    [Tooltip("Video ที่เล่นตอนใช้ Ultimate (เฉพาะ Hero นี้)")]
    public VideoClip ultimateVideo;

}