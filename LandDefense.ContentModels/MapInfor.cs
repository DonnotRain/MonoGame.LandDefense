using Microsoft.Xna.Framework;

namespace LandDefense.ContentModels
{
    public class MapInfor
    {
        public Vector2 EndPosition { get; set; } //终点所在位置
        public string PositionArray { get; set; } //地图线路序列
        public string TowerPositionArray { get; set; } //所有塔所在的位置序列
        public int MapIndex { get; set; } //地图序号，即关卡序号
        public string MapTexture { get; set; } //这一关的地图文件所在位置
        public int AllPowerNumber { get; set; } //每一关初始的能量值
        public int AllRound { set; get; }

        public Vector2[] GetTowerPositionArray()
        {
            var len = TowerPositionArray.Length;
            var arrayindex = len/6;
            var positions = new Vector2[arrayindex];
            for (var i = 0; i < arrayindex; i++)
            {
                positions[i].X = int.Parse(TowerPositionArray.Substring(i*6, 3));
                positions[i].Y = int.Parse(TowerPositionArray.Substring(i*6 + 3, 3));
            }
            return positions;
            // PositionArray.Substring();
        }

        public Vector2[] GetTargetArray()
        {
            var len = PositionArray.Length;
            var arrayindex = len/6;
            var positions = new Vector2[arrayindex];
            for (var i = 0; i < arrayindex; i++)
            {
                positions[i].X = int.Parse(PositionArray.Substring(i*6, 3));
                positions[i].Y = int.Parse(PositionArray.Substring(i*6 + 3, 3));
            }
            return positions;
        }
    }
}