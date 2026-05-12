// ไฟล์นี้ไม่ต้องแขวนบน GameObject ไหนเลย
// เป็นแค่กล่องเก็บข้อมูลว่าตอนนี้เลือก Hero ตัวไหน
// HeroSelectManager เขียนลงมา → BattleManager อ่านออกไป
public static class SelectedHero
{
    public static HeroData data;  // Hero ที่ผู้เล่นเลือก
}