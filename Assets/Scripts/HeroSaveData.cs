[System.Serializable]
public class HeroSaveData
{
    public string    heroName;
    public HeroClass heroClass;
    public int       maxHP;
    public int       attackPower;
    public int       skillPower;
    public int       maxSP;
    public int       spCost;
    public int       spGainAttack;
    public int       spGainSkill;
    public int       ultimatePower;
    public int       ultimateCost;
    public int       ultGainAttack;
    public int       ultGainSkill;
 
    // Archmage
    public bool canHeal;
    public int  healAmount;
 
    // Swordmaster
    public float doubleHitChance;
 
    // Executioner
    public float execDoubleHitChance;
    public int   execBonusUlt;
 
    // Ambassador
    public int atkDebuff;
}