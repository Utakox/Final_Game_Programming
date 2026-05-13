using UnityEngine;
using System.Collections.Generic;

public static class HeroSkills
{
    public class SkillData
    {
        public HeroSaveData  Hero;
        public int           Target;
        public List<int>     EnemyHP;
        public List<int>     EnemyAttackPower;
        public List<int>     EnemyHeavyAttackPower;
        public int           PlayerHP;
        public int           Ultimate;
        public string        Message;
    }

    // =========================================
    // BattleManager เรียกตรงนี้แค่จุดเดียว
    // =========================================
    public static void UseSkill(SkillData data)
    {
        switch (data.Hero.heroClass)
        {
            case HeroClass.Swordmaster: Swordmaster(data); break;
            case HeroClass.Executioner: Executioner(data); break;
            case HeroClass.Archmage:    Archmage(data);    break;
            case HeroClass.Ambassador:  Ambassador(data);  break;
            case HeroClass.Archer:      Archer(data);      break;
            default:                    Default(data);     break;
        }
    }

    // =========================================
    // Skill แต่ละ Class
    // =========================================

    // Swordmaster: มีโอกาสได้ชาจอัลติ 2 ครั้ง
    static void Swordmaster(SkillData data)
    {
        int dmg = data.Hero.skillPower;
        bool doubleHit = Random.value < data.Hero.doubleHitChance;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        data.Message = data.Hero.heroName + " strikes! took " + dmg + "!";

        if (doubleHit)
        {
            data.Ultimate += data.Hero.ultGainSkill;
            if (data.Ultimate > data.Hero.ultimateCost)
                data.Ultimate = data.Hero.ultimateCost;
            data.Message = data.Hero.heroName + " Gains Double Ult Charge!" + "\n" + data.Hero.ultGainSkill + " Ult gained!";
        }
    }

    // Executioner: มีโอกาสตี 2 ครั้ง + ได้ Ult โบนัส
    static void Executioner(SkillData data)
    {
        int  dmg       = data.Hero.skillPower;
        bool doubleHit = Random.value < data.Hero.execDoubleHitChance;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        if (doubleHit)
        {
            data.EnemyHP[data.Target] -= dmg;

            if (data.EnemyHP[data.Target] < 0)
                data.EnemyHP[data.Target] = 0;

            data.Ultimate += data.Hero.execBonusUlt;

            if (data.Ultimate > data.Hero.ultimateCost)
                data.Ultimate = data.Hero.ultimateCost;

            data.Message = data.Hero.heroName + " executes TWICE! took " + dmg + " + " + dmg + "! (Ult +" + data.Hero.execBonusUlt + ")";
        }
        else
        {
            data.Message = data.Hero.heroName + " executes! took " + dmg + "!";
        }
    }

    // Archmage: ตีปกติ + ฮิลตัวเอง
    static void Archmage(SkillData data)
    {
        int dmg = data.Hero.skillPower;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        if (data.Hero.canHeal)
        {
            data.PlayerHP += data.Hero.healAmount;

            if (data.PlayerHP > data.Hero.maxHP)
                data.PlayerHP = data.Hero.maxHP;

            data.Message = data.Hero.heroName + " casts Skill! took " + dmg + "! Healed " + data.Hero.healAmount + " HP!";
        }
        else
        {
            data.Message = data.Hero.heroName + " casts Skill! took " + dmg + "!";
        }
    }

    // Ambassador: ลด Attack ศัตรูถาวร
    static void Ambassador(SkillData data)
    {
        data.EnemyAttackPower[data.Target] -= data.Hero.atkDebuff;

        int dmg = data.Hero.skillPower;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        // แก้ bug: เดิมเขียน -= 1 ทำให้ลดต่อไปเรื่อยๆ ต้องเป็น = 1
        if (data.EnemyAttackPower[data.Target] < 1)
            data.EnemyAttackPower[data.Target] = 1;

        if (data.EnemyHeavyAttackPower[data.Target] < 1)
            data.EnemyHeavyAttackPower[data.Target] = 1;

        data.Message = data.Hero.heroName + " weakens enemy! Attack reduced to " + data.EnemyAttackPower[data.Target] + "!";
    }

    // Archer: ยิงธนู
    static void Archer(SkillData data)
    {
        int dmg = data.Hero.skillPower;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        data.Message = data.Hero.heroName + " shoots an arrow! took " + dmg + "!";
    }

    static void Default(SkillData data)
    {
        int dmg = data.Hero.skillPower;

        data.EnemyHP[data.Target] -= dmg;

        if (data.EnemyHP[data.Target] < 0)
            data.EnemyHP[data.Target] = 0;

        data.Message = data.Hero.heroName + " uses Skill! took " + dmg + "!";
    }
}