namespace LandDefense.ContentModels
{
    public class TowerInfor
    {
        public int DamageType { set; get; } //伤害类型，1代表是物理伤害，2代表是魔法伤害，物理伤害受护甲减免，魔法伤害受魔抗减免
        public int MinDamage { set; get; } //塔能发射出的最小伤害
        public int MaxDamage { set; get; } //塔能发射出的最大伤害，产生的实际值随即产生在最小最大范围内
        public float Interval { set; get; } //攻击的时间间隔,毫秒为单位
        public int Range { set; get; } //攻击范围，以像素点为单位
        public int NeededPower { set; get; } //升级到这个等级所需要的能量数
        public string TowerTypeName { set; get; } //塔类型的名称
        public string NextTowerLevel { set; get; }
    }
}