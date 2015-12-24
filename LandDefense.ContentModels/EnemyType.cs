using Microsoft.Xna.Framework;

namespace LandDefense.ContentModels
{
    public class EnemyType
    {
        public Vector2 Velocity { set; get; }
        public string EnemyName { set; get; } //敌人名称
        public string TextureName { set; get; } //贴图所在位置
        public int MinDamage { set; get; } //塔能发射出的最小伤害
        public int MaxDamage { set; get; } //塔能发射出的最大伤害，产生的实际伤害值随机产生在最小最大范围内
        public float Interval { set; get; } //攻击的时间间隔,毫秒为单位
        public int Range { set; get; } //攻击范围，以像素点为单位
        public bool IsFlying { set; get; } //是否为飞行生物,1是，0否
        public int LifeNum { set; get; } //生命值
        public int Armor { set; get; } //护甲值。
        public int AgainstSpells { set; get; } //魔法抗性      
    }
}