using System;
using System.Collections.Generic;
using LandDefense.ContentModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense.Models
{
    public class TowersType
    {
        public enum TowerTypes
        {
            None,
            MagicTower_level2, //魔法塔，按等级区分
            MagicTower_level1,
            MagicTower_level3,
            MagicTower_level4,
            ArrowTower_level1, //弓箭塔
            ArrowTower_level2,
            ArrowTower_level3,
            ArrowTower_level4,

            TurretTower_level1, //炮塔
            TurretTower_level2,
            TurretTower_level3,
            TurretTower_level4,
            BarracksTower_level1, //兵营
            BarracksTower_level2,
            BarracksTower_level3,
            BarracksTower_level4
        }

        //   Rectangle CurrentRes;                             //当前贴图来源区域 
        private readonly Texture2D[] AllTowerTexture;
        private readonly Texture2D[] DrawTowerTextures;
        private readonly int[] FirstNeededPower = new int[4];
        private readonly InputState inputeState;
        private readonly Rectangle[] ResRec = new Rectangle[16];
        private readonly SpriteFont sf;
        private readonly List<TowerInfor> towerInfor;
        //塔上面站着的角色的图片源
        private int ArrowRole_X = 3;
        public float cooldownTime; //攻击间隔时间  
        public TowerInfor currentTower = new TowerInfor();
        public TowerTypes currentTowerType;
        private Vector2 desPosition;
        private int elapseTime;
        private bool ISchoosingTower;
        //   int MagicRole_Y= 0;

        public bool IsHittingEnemy;
        //  int ArrowRole_Y=0;
        private int MagicRole_X;
        private int neededPower;
        private string NextTowerType;
        private int offset_X;
        private int offset_Y;
        public Vector2 position = new Vector2(0, 0);
        public EnemiesInfor TargetEnemy;
        //  Texture2D SingleTowerTexture;            //当前显示的塔的贴图
        private int TowerTexture_X; // 用于贴图区域中选择来源区域
        private int TowerTexture_Y;

        public TowersType(Texture2D[] DrawTowerTextures, SpriteFont sf, List<TowerInfor> towerInfor,
            Texture2D[] AllTowerTexture)
        {
            currentTowerType = TowerTypes.None;
            this.DrawTowerTextures = DrawTowerTextures;
            this.sf = sf;
            inputeState = new InputState();
            this.towerInfor = towerInfor;
            this.AllTowerTexture = AllTowerTexture;
            offset_X = DrawTowerTextures[5].Width / 2;
            offset_Y = DrawTowerTextures[5].Height / 2;

            for (var i = 0; i < 13; i++)
                ResRec[i] = new Rectangle(i * 46, 0, 46, 44); //选择塔图标的来源区域初始化
            ResRec[13] = new Rectangle(0, 0, 35, 34); //集合点图标的来源区域
            ResRec[14] = new Rectangle(0, 0, 30, 20); //显示升级所需能量值图标的来源区域
            ResRec[15] = new Rectangle(0, 0, 32, 32); //出售塔图标的来源区域

            //初始化升最初等级的塔需要的能量值
            foreach (var tower in towerInfor)
            {
                if (tower.TowerTypeName.Equals(TowerTypes.ArrowTower_level1.ToString()))
                    FirstNeededPower[0] = tower.NeededPower; //升级第一级箭塔所需要的能量值
                if (tower.TowerTypeName.Equals(TowerTypes.BarracksTower_level1.ToString()))
                    FirstNeededPower[1] = tower.NeededPower; //升级第一级兵营所需要的能量值     
                if (tower.TowerTypeName.Equals(TowerTypes.MagicTower_level1.ToString())) //升级第一级魔法塔所需要的能量值
                    FirstNeededPower[2] = tower.NeededPower;
                if (tower.TowerTypeName.Equals(TowerTypes.TurretTower_level1.ToString())) //升级第一级炮塔所需要的能量值
                    FirstNeededPower[3] = tower.NeededPower;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch sBatch, Vector2 Position, int Index)
        {
            //  sBatch.DrawString(sf, cooldownTime.ToString(), position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            if (cooldownTime != 0)
            {
                cooldownTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
            if (cooldownTime < 0)
            {
                cooldownTime = 0;
            }
            position = Position;


            DrawTower(gameTime, sBatch, Position, Index); //绘制出塔

            #region 初始塔显示

            if (currentTowerType == TowerTypes.None)
            {
                if (inputeState.IsMouseOn(new Rectangle((int)(Position.X - (DrawTowerTextures[5].Width / 2)),
                    (int)(Position.Y - (DrawTowerTextures[5].Height / 2)), DrawTowerTextures[5].Width,
                    DrawTowerTextures[5].Height)) || ISchoosingTower)
                //鼠标指针在塔上的时候，画出塔的底座
                {
                    sBatch.Draw(DrawTowerTextures[6], new Rectangle((int)(Position.X - (DrawTowerTextures[5].Width / 2)),
                        (int)(Position.Y - (DrawTowerTextures[5].Height / 2)), DrawTowerTextures[5].Width,
                        DrawTowerTextures[5].Height),
                        new Rectangle(0, 0, DrawTowerTextures[5].Width, DrawTowerTextures[5].Height),
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
                    if (inputeState.IsLeftButtonClick() && GameStateClass.LayerDepth == -1)
                    {
                        ISchoosingTower = true; //设置塔的正在选择升级状态为true，激活升级信息图标的显示
                        GameStateClass.LayerDepth = Index; //设置图层的深度为当前图层序号，激活当前图层的操作
                        //  System.Diagnostics.Trace.WriteLine(GameStateClass.LayerDepth + "," + Index);
                    }
                }
                else
                {
                    //鼠标指针不在塔上的时候，画出塔的底座
                    sBatch.Draw(DrawTowerTextures[5], new Rectangle((int)(Position.X - (DrawTowerTextures[5].Width / 2)),
                        (int)(Position.Y - (DrawTowerTextures[5].Height / 2)), DrawTowerTextures[6].Width,
                        DrawTowerTextures[5].Height),
                        new Rectangle(0, 0, DrawTowerTextures[5].Width, DrawTowerTextures[5].Height),
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
                }
            }

            #endregion

            //塔等级不为0时，根据塔的状态及类型显示相关状态。

            if (currentTowerType != TowerTypes.None)
            {
                foreach (var tower in towerInfor)
                {
                    if (tower.TowerTypeName.Equals(currentTowerType.ToString()))
                    {
                        NextTowerType = tower.NextTowerLevel;
                        currentTower = tower;
                    }
                }

                foreach (var tower in towerInfor)
                {
                    if (tower.TowerTypeName.Equals(NextTowerType))
                    {
                        neededPower = tower.NeededPower;
                    }
                }
            }
            //正在选择塔时显示相关信息
            if (ISchoosingTower && GameStateClass.LayerDepth == Index)
            {
                if (currentTowerType != TowerTypes.None)
                {
                    sBatch.Draw(DrawTowerTextures[1],
                        new Rectangle((int)Position.X - 20, (int)Position.Y - DrawTowerTextures[5].Height + 20,
                            DrawTowerTextures[1].Width, DrawTowerTextures[1].Height),
                        new Rectangle(0, 0, DrawTowerTextures[1].Width,
                            DrawTowerTextures[1].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.01f);
                    sBatch.DrawString(sf, neededPower.ToString(), new Vector2(Position.X - 15, position.Y - 34),
                        Color.Yellow);
                }
                //正在选择塔的图标显示
                sBatch.Draw(DrawTowerTextures[7],
                    new Rectangle((int)Position.X - 60, (int)Position.Y - DrawTowerTextures[5].Height - 5,
                        DrawTowerTextures[7].Width,
                        DrawTowerTextures[7].Height),
                    new Rectangle(0, 0, DrawTowerTextures[7].Width, DrawTowerTextures[7].Height), Color.White
                    , 0, Vector2.Zero, SpriteEffects.None, 0.5f);

                var DesRec = new Rectangle((int)Position.X - DrawTowerTextures[7].Width / 4,
                    (int)Position.Y - DrawTowerTextures[7].Height / 2 - 10, ResRec[1].Width, ResRec[1].Height);
                //根据当前塔的等级及类型，判断并显示塔的升级信息
                switch (currentTowerType)
                {
                    #region 塔类型为空时

                    case TowerTypes.None:
                        //塔等级为0时，在四角绘制出四个供选择的塔的图标
                        //绘制选择箭塔的图标

                        var ArrowRec = new Rectangle((int)Position.X - DrawTowerTextures[7].Width / 3 + 10,
                            (int)Position.Y - DrawTowerTextures[7].Height / 2 - 10, ResRec[1].Width, ResRec[1].Height);
                        sBatch.Draw(DrawTowerTextures[2], ArrowRec, ResRec[0], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        //绘制所需能量值  
                        sBatch.Draw(DrawTowerTextures[1],
                            new Rectangle((int)Position.X - 20, (int)Position.Y - DrawTowerTextures[5].Height + 20,
                                DrawTowerTextures[1].Width, DrawTowerTextures[1].Height),
                            new Rectangle(0, 0, DrawTowerTextures[1].Width,
                                DrawTowerTextures[1].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.01f);
                        sBatch.DrawString(sf, FirstNeededPower[0].ToString(),
                            new Vector2(Position.X - 15, position.Y - 34), Color.Yellow);
                        if (GameStateClass.AllPowerNum >= FirstNeededPower[0])
                            if (inputeState.IsMouseOn(ArrowRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], ArrowRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);
                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.ArrowTower_level1;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= FirstNeededPower[0];
                                }
                            }

                        //将兵营的选择功能关闭
                        //绘制选择兵营的图标
                        //Rectangle BarracksRec = new Rectangle((int)Position.X + DrawTowerTextures[7].Width / 6, (int)Position.Y - DrawTowerTextures[7].Height / 2, ResRec[1].Width, ResRec[1].Height);
                        //sBatch.Draw(DrawTowerTextures[2], BarracksRec, ResRec[1], Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                        //if (inputeState.IsMouseOn(BarracksRec))
                        //{
                        //    sBatch.Draw(DrawTowerTextures[3], BarracksRec, new Rectangle(0, 0, DrawTowerTextures[3].Width,
                        //        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);

                        //    if (inputeState.IsLeftButtonClick())
                        //    {
                        //        currentTowerType = TowerTypes.BarracksTower_level1;
                        //        ISchoosingTower = false;
                        //    }
                        //}
                        //绘制选择魔法塔的图标
                        var MagicArrow = new Rectangle((int)Position.X - DrawTowerTextures[7].Width / 2 - 10,
                            (int)Position.Y + DrawTowerTextures[7].Height / 6, ResRec[1].Width, ResRec[1].Height);
                        sBatch.Draw(DrawTowerTextures[2], MagicArrow,
                            ResRec[2], Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                        //绘制所需能量值
                        sBatch.Draw(DrawTowerTextures[1], new Rectangle((int)Position.X - 60, (int)Position.Y + 55,
                            DrawTowerTextures[1].Width, DrawTowerTextures[1].Height),
                            new Rectangle(0, 0, DrawTowerTextures[1].Width,
                                DrawTowerTextures[1].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.01f);
                        sBatch.DrawString(sf, FirstNeededPower[2].ToString(),
                            new Vector2(Position.X - 55, position.Y + 57), Color.Yellow);
                        if (GameStateClass.AllPowerNum >= FirstNeededPower[2])
                            if (inputeState.IsMouseOn(MagicArrow))
                            {
                                sBatch.Draw(DrawTowerTextures[3], MagicArrow,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.MagicTower_level1;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= FirstNeededPower[2];
                                }
                            }
                        //绘制选择炮塔的图标
                        var TurretRec = new Rectangle((int)Position.X + DrawTowerTextures[7].Width / 6,
                            (int)Position.Y + DrawTowerTextures[7].Height / 6, ResRec[1].Width, ResRec[1].Height);
                        sBatch.Draw(DrawTowerTextures[2], TurretRec,
                            ResRec[3], Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                        //绘制所需能量值
                        sBatch.Draw(DrawTowerTextures[1], new Rectangle((int)Position.X + 30, (int)Position.Y + 55,
                            DrawTowerTextures[1].Width, DrawTowerTextures[1].Height),
                            new Rectangle(0, 0, DrawTowerTextures[1].Width,
                                DrawTowerTextures[1].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.01f);
                        sBatch.DrawString(sf, FirstNeededPower[3].ToString(),
                            new Vector2(Position.X + 34, position.Y + 57), Color.Yellow);

                        if (GameStateClass.AllPowerNum >= FirstNeededPower[3])
                            if (inputeState.IsMouseOn(TurretRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], TurretRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);
                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.TurretTower_level1;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= FirstNeededPower[3];
                                }
                            }
                        break;

                    #endregion

                    #region 为其他可选等级的情况时

                    case TowerTypes.ArrowTower_level1:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)
                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.2f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.ArrowTower_level2;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.ArrowTower_level2:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)
                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.2f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.ArrowTower_level3;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;

                    case TowerTypes.ArrowTower_level3:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.2f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.ArrowTower_level4;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.BarracksTower_level1:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.BarracksTower_level2;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.BarracksTower_level2:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (inputeState.IsMouseOn(DesRec))
                        {
                            sBatch.Draw(DrawTowerTextures[3], DesRec, new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);

                            if (inputeState.IsLeftButtonClick())
                            {
                                currentTowerType = TowerTypes.BarracksTower_level3;
                                ISchoosingTower = false;
                                GameStateClass.AllPowerNum -= neededPower;
                            }
                        }
                        break;
                    case TowerTypes.BarracksTower_level3:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (inputeState.IsMouseOn(DesRec))
                        {
                            sBatch.Draw(DrawTowerTextures[3], DesRec, new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);

                            if (inputeState.IsLeftButtonClick())
                            {
                                currentTowerType = TowerTypes.BarracksTower_level4;
                                ISchoosingTower = false;
                            }
                        }
                        break;
                    case TowerTypes.MagicTower_level1:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.MagicTower_level2;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.MagicTower_level2:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)
                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.MagicTower_level3;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.MagicTower_level3:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)
                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.MagicTower_level4;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.TurretTower_level1:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.TurretTower_level2;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.TurretTower_level2:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.TurretTower_level3;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    case TowerTypes.TurretTower_level3:

                        sBatch.Draw(DrawTowerTextures[2], DesRec, ResRec[4], Color.White, 0, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                        if (GameStateClass.AllPowerNum >= neededPower)

                            if (inputeState.IsMouseOn(DesRec))
                            {
                                sBatch.Draw(DrawTowerTextures[3], DesRec,
                                    new Rectangle(0, 0, DrawTowerTextures[3].Width,
                                        DrawTowerTextures[3].Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                                    0.6f);

                                if (inputeState.IsLeftButtonClick())
                                {
                                    currentTowerType = TowerTypes.TurretTower_level4;
                                    ISchoosingTower = false;
                                    GameStateClass.AllPowerNum -= neededPower;
                                }
                            }
                        break;
                    default:
                        break;

                        #endregion
                }
            }

            #region   //鼠标在塔位置外的区域点击，设置ISchoosingTower为false，不再绘制选择的图层

            if (inputeState.IsMouseOn(new Rectangle(0, 0, 700, 600)) &&
                inputeState.IsMouseOn(new Rectangle((int)(Position.X - (DrawTowerTextures[5].Width / 2)),
                    (int)(Position.Y - (DrawTowerTextures[5].Height / 2)), 100, 100)) == false)
            {
                if (inputeState.IsLeftButtonClick() && GameStateClass.LayerDepth == Index)
                {
                    ISchoosingTower = false;
                    GameStateClass.LayerDepth = -1; //设置图层深度为公共的-1，激活其他塔的选择
                    //  System.Diagnostics.Trace.WriteLine("out" + GameStateClass.LayerDepth + "," + Index);
                }
            }

            #endregion

            inputeState.Update();
        }

        private void DrawTower(GameTime gameTime, SpriteBatch sBatch, Vector2 Position, int Index)
        {
            elapseTime += gameTime.ElapsedGameTime.Milliseconds;
            if (IsHittingEnemy)
            {
                if (elapseTime >= 200)
                {
                    TowerTexture_X++;
                    ArrowRole_X++;
                    MagicRole_X++;
                    elapseTime = 0;
                }
            }
            Texture2D towerTexture;
            Rectangle DesRec;
            Rectangle SouRec;
            var height = 0;
            var Width = 0;
            var ArrowRoleDesRec1 = new Rectangle((int)Position.X - 80 / 3 + 28, (int)Position.Y - 70 / 3, 18, 18);
            var ArrowRoleResRec = new Rectangle(ArrowRole_X * 18, 0, 18, 18);
            var ArrowRoleDesRec2 = new Rectangle((int)Position.X - 80 / 3 + 11, (int)Position.Y - 70 / 3, 18, 18);

            var MagicRoleDesRec = new Rectangle((int)Position.X - 80 / 5, (int)Position.Y - 70 / 2 + 5, 30, 30);
            var MagicRoleResRec = new Rectangle(MagicRole_X * 30, 0, 30, 30);

            #region 根据塔的类型绘制塔

            switch (currentTowerType)
            {
                case TowerTypes.ArrowTower_level1:
                    //   sBatch();
                    Width = 80;
                    height = 70;
                    if (ArrowRole_X >= 5)
                    {
                        ArrowRole_X = 3;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[0];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(0, 0, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec1, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec2, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);

                    break;
                case TowerTypes.ArrowTower_level2:
                    Width = 80;
                    height = 70;
                    if (ArrowRole_X >= 5)
                    {
                        ArrowRole_X = 3;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[0];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(80, 0, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec1, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec2, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);

                    break;
                case TowerTypes.ArrowTower_level3:
                    Width = 80;
                    height = 70;
                    if (ArrowRole_X >= 5)
                    {
                        ArrowRole_X = 3;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[0];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(160, 0, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec1, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec2, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);

                    break;
                case TowerTypes.ArrowTower_level4:
                    Width = 90;
                    height = 70;
                    if (ArrowRole_X >= 5)
                    {
                        ArrowRole_X = 3;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[1];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(90, 0, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec1, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    sBatch.Draw(AllTowerTexture[2], ArrowRoleDesRec2, ArrowRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);

                    break;
                case TowerTypes.MagicTower_level1:
                    Width = 80;
                    height = 70;
                    if (TowerTexture_X >= 5)
                    {
                        TowerTexture_X = 0;
                        //  IsHittingEnemy = false;
                    }
                    if (MagicRole_X >= 9)
                    {
                        MagicRole_X = 0;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[6];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[5], MagicRoleDesRec, MagicRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    break;
                case TowerTypes.MagicTower_level2:
                    Width = 80;
                    height = 70;
                    if (TowerTexture_X >= 5)
                    {
                        TowerTexture_X = 0;
                        //   IsHittingEnemy = false;
                    }
                    if (MagicRole_X >= 9)
                    {
                        MagicRole_X = 0;
                        IsHittingEnemy = false;
                    }
                    TowerTexture_Y = 1;
                    towerTexture = AllTowerTexture[6];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[5], MagicRoleDesRec, MagicRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    break;
                case TowerTypes.MagicTower_level3:
                    Width = 80;
                    height = 70;
                    if (TowerTexture_X >= 5)
                    {
                        TowerTexture_X = 0;
                        // IsHittingEnemy = false;
                    }
                    if (MagicRole_X >= 9)
                    {
                        MagicRole_X = 0;
                        IsHittingEnemy = false;
                    }
                    TowerTexture_Y = 2;
                    towerTexture = AllTowerTexture[6];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[5], MagicRoleDesRec, MagicRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    break;
                case TowerTypes.MagicTower_level4:
                    Width = 90;
                    height = 80;
                    if (TowerTexture_X >= 8)
                    {
                        TowerTexture_X = 0;
                        // IsHittingEnemy = false;
                    }
                    if (MagicRole_X >= 9)
                    {
                        MagicRole_X = 0;
                        IsHittingEnemy = false;
                    }
                    TowerTexture_Y = 0;
                    towerTexture = AllTowerTexture[7];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    sBatch.Draw(AllTowerTexture[5], MagicRoleDesRec, MagicRoleResRec, Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.4f);
                    break;
                case TowerTypes.BarracksTower_level1:
                    Width = 80;
                    height = 60;
                    TowerTexture_Y = 0;
                    towerTexture = AllTowerTexture[3];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.BarracksTower_level2:
                    Width = 80;
                    height = 60;
                    TowerTexture_Y = 1;
                    towerTexture = AllTowerTexture[3];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.BarracksTower_level3:
                    Width = 80;
                    height = 70;
                    TowerTexture_Y = 0;
                    towerTexture = AllTowerTexture[4];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.BarracksTower_level4:
                    Width = 80;
                    height = 70;
                    TowerTexture_Y = 2;
                    towerTexture = AllTowerTexture[4];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.TurretTower_level1:
                    Width = 80;
                    height = 56;
                    //   TowerTexture_Y = 2;
                    if (TowerTexture_X >= 8)
                    {
                        TowerTexture_X = 0;
                        IsHittingEnemy = false;
                    }
                    towerTexture = AllTowerTexture[10];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.TurretTower_level2:
                    Width = 80;
                    height = 58;
                    if (TowerTexture_X >= 9)
                    {
                        TowerTexture_X = 0;
                        IsHittingEnemy = false;
                    } //   TowerTexture_Y = 2;
                    towerTexture = AllTowerTexture[9];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.TurretTower_level3:
                    Width = 80;
                    height = 59;
                    if (TowerTexture_X >= 7)
                    {
                        TowerTexture_X = 0;
                        IsHittingEnemy = false;
                    } //   TowerTexture_Y = 2;
                    towerTexture = AllTowerTexture[11];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
                case TowerTypes.TurretTower_level4:
                    Width = 90;
                    height = 68;
                    if (TowerTexture_X >= 12)
                    {
                        TowerTexture_X = 0;
                        IsHittingEnemy = false;
                    } //   TowerTexture_Y = 2;
                    towerTexture = AllTowerTexture[12];
                    DesRec = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width, height);
                    SouRec = new Rectangle(TowerTexture_X * Width, TowerTexture_Y * height, Width, height);
                    sBatch.Draw(towerTexture, DesRec, SouRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    break;
            }

            #endregion

            if (
                inputeState.IsMouseOn(new Rectangle((int)Position.X - Width / 2, (int)Position.Y - height / 2, Width,
                    height)) || ISchoosingTower)
            {
                //   sBatch.DrawString(sf,"LayeRDEPth"+
                if (inputeState.IsLeftButtonClick() && GameStateClass.LayerDepth == -1 &&
                    currentTowerType != TowerTypes.ArrowTower_level4 &&
                    currentTowerType != TowerTypes.BarracksTower_level4 &&
                    currentTowerType != TowerTypes.MagicTower_level4 &&
                    currentTowerType != TowerTypes.TurretTower_level4)
                {
                    ISchoosingTower = true; //设置塔的正在选择升级状态为true，激活升级信息图标的显示
                    GameStateClass.LayerDepth = Index; //设置图层的深度为当前图层序号，激活当前图层的操作
                    //  System.Diagnostics.Trace.WriteLine(GameStateClass.LayerDepth + "," + Index);
                }
            }
        }

        public void HitEnemy(GameTime gameTime, SpriteBatch sBatch, Vector2 EnemyPosition, GamePlayScreen gamePlay)
        {
            IsHittingEnemy = true;
            desPosition = EnemyPosition;
            if (currentTowerType == TowerTypes.TurretTower_level1 || currentTowerType == TowerTypes.TurretTower_level2 ||
                currentTowerType == TowerTypes.TurretTower_level3 || currentTowerType == TowerTypes.TurretTower_level4)
            {
                var acceleration = 0.5f;
                var newTurret = new Turret();
                newTurret.Position.X = position.X - 10;
                newTurret.Position.Y = position.Y - 20;
                //炮弹的抛物线轨迹计算
                // newTurret.Velocity.X =(desPosition.X - position.X) / 50;
                // Console.WriteLine(desPosition.X + "," + position.X);
                newTurret.Velocity.Y = -6;
                var vy = newTurret.Velocity.Y;
                newTurret.Velocity.X =
                    -(float)
                        ((2 * acceleration * (desPosition.X - position.X)) /
                         (5 * vy + Math.Sqrt(vy * vy + 2 + acceleration * (desPosition.Y - position.Y))));
                newTurret.IsAlive = true;
                newTurret.MaxDamage = currentTower.MaxDamage;
                newTurret.MinDamage = currentTower.MinDamage;
                newTurret.desPosition = EnemyPosition;
                //  newTurret.Velocity
                gamePlay.addTurrets(newTurret);
            }

            cooldownTime = currentTower.Interval;
        }

        //有目标的攻击
        public void HitTargetEnemy(GameTime gameTime, SpriteBatch sBatch, Vector2 EnemyPosition, GamePlayScreen gamePlay)
        {
            desPosition = EnemyPosition;
            IsHittingEnemy = true;

            //弓箭塔攻击逻辑,将子弹加入序列
            if (currentTowerType == TowerTypes.ArrowTower_level1 || currentTowerType == TowerTypes.ArrowTower_level2 ||
                currentTowerType == TowerTypes.ArrowTower_level3 || currentTowerType == TowerTypes.ArrowTower_level4)
            {
                var newBullet = new Bullets();
                newBullet.Position.X = position.X;
                newBullet.Position.Y = position.Y - 20;
                var distance =
                    (float)Math.Sqrt((desPosition.Y - newBullet.Position.Y) * (desPosition.Y - newBullet.Position.Y) +
                                      (desPosition.X - newBullet.Position.X) * (desPosition.X - newBullet.Position.X));
                var rotation = (float)Math.Asin((desPosition.Y - newBullet.Position.Y) / distance);
                if (desPosition.X - newBullet.Position.X < 0)
                    rotation = (float)Math.PI - rotation;
                newBullet.rotation = rotation;
                newBullet.desPosition = desPosition;
                newBullet.MaxDamage = currentTower.MaxDamage;
                // newBullet.k = Convert.ToInt16((desPosition.Y - newBullet.Position.Y) / (desPosition.X - newBullet.Position.X));
                newBullet.MinDamage = currentTower.MinDamage;
                newBullet.IsAlive = true;
                gamePlay.addBullets(newBullet, 2);
            }
            if (currentTowerType == TowerTypes.MagicTower_level1 || currentTowerType == TowerTypes.MagicTower_level2 ||
                currentTowerType == TowerTypes.MagicTower_level3 || currentTowerType == TowerTypes.MagicTower_level4)
            {
                var newBullet = new Bullets();
                newBullet.Position.X = position.X;
                newBullet.Position.Y = position.Y - 20;
                newBullet.desPosition = desPosition;
                // newBullet.k = Convert.ToInt16((desPosition.Y - newBullet.Position.Y) / (desPosition.X - newBullet.Position.X));

                newBullet.MaxDamage = currentTower.MaxDamage;
                newBullet.MinDamage = currentTower.MinDamage;
                newBullet.IsAlive = true;
                gamePlay.addBullets(newBullet, 1);
            }
            cooldownTime = currentTower.Interval;
        }
    }
}