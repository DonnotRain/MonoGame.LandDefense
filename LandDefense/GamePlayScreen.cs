using System;
using System.Collections.Generic;
using LandDefense.ContentModels;
using LandDefense.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LandDefense
{
    /// <summary>
    ///     这是游戏的主逻辑
    /// </summary>
    public class GamePlayScreen : DrawableGameComponent
    {
        public GamePlayScreen(Game game, MySound mySound)
            : base(game)
        {
            this.mySound = mySound;
            this.game = game;
            //LoadContent();
            inputeState = new InputState();
            graphic = game.GraphicsDevice;
            //  System.Diagnostics.Trace.WriteLine("GamePlayScreen:GamePlayScreen" + GameStateClass.ChosedLevel);
        }

        #region 初始化部分数据

        protected override void LoadContent()
        {
            StateOFgame = GameStateClass.currentGameState;
            //  if (GameStateClass.currentGameState != GameStateClass.GameState.MainGame) return;
            mapInfrolist = game.Content.Load<List<MapInfor>>("XmlData\\MapInfor");
            //   if(GameStateClass.ChosedLevel!=-1)
            MaplevelIndex = GameStateClass.ChosedLevel;

            towerSprite = game.Content.Load<SpriteFont>("TowerSpriteFont");
            foreach (var map in mapInfrolist)
            {
                if (map.MapIndex == MaplevelIndex)
                {
                    // System.Diagnostics.Trace.WriteLine("GamePlayScreen:LoadContent" + GameStateClass.ChosedLevel);
                    MapImg = game.Content.Load<Texture2D>(@"MapsImg\" + map.MapTexture);
                    currentMap = map;
                    TowerPosition = map.GetTowerPositionArray();
                }
            }

            sf = game.Content.Load<SpriteFont>("TowerInfor");
            towerInfor = game.Content.Load<List<TowerInfor>>(@"XmlData\TowerInfor");
            towers = new TowersType[TowerPosition.Length];

            // System.Diagnostics.Trace.WriteLine(towers[0].currentTowerYype.ToString());
            DrawTowerTextures[0] = game.Content.Load<Texture2D>(@"UIImage\collecting_point"); //0是集合点图片
            DrawTowerTextures[1] = game.Content.Load<Texture2D>(@"UIImage\NeededPower"); //1是需要的能量显示背景图片
            DrawTowerTextures[2] = game.Content.Load<Texture2D>(@"UIImage\TowerSelect46_44"); //2是选择塔的集合。就是所有塔德图标
            DrawTowerTextures[3] = game.Content.Load<Texture2D>(@"UIImage\TowerSelectBack"); //获取鼠标焦点的背景,有黄颜色的
            DrawTowerTextures[4] = game.Content.Load<Texture2D>(@"UIImage\SellForPower"); //出售塔的图标，把塔拿来换能量
            DrawTowerTextures[5] = game.Content.Load<Texture2D>(@"TowersImg\Mark_Tower"); //出始塔的图标
            DrawTowerTextures[6] = game.Content.Load<Texture2D>(@"TowersImg\Mark_Tower_02"); //出始塔的图标,鼠标指针在上面时。。。

            DrawTowerTextures[7] = game.Content.Load<Texture2D>(@"UIImage\JustCircle"); //光秃秃的一个圈
            //  DrawTowerTextures[8] = game.Content.Load<Texture2D>(@"TowersImg\TowerUpgrade");               //升级塔的图标


            AllTowerTexture = new Texture2D[13];
            AllTowerTexture[0] = game.Content.Load<Texture2D>(@"TowerImages\Arrow_001_80_70"); //太多了，注释没法写，还是参照着图片看吧。。
            AllTowerTexture[1] = game.Content.Load<Texture2D>(@"TowerImages\Arrow_002_90_70");
            AllTowerTexture[2] = game.Content.Load<Texture2D>(@"TowerImages\Arrow_003_18_18");
            AllTowerTexture[3] = game.Content.Load<Texture2D>(@"TowerImages\Bing_001_80_60");
            AllTowerTexture[4] = game.Content.Load<Texture2D>(@"TowerImages\Bing_002_80_70");
            AllTowerTexture[5] = game.Content.Load<Texture2D>(@"TowerImages\MagicRole_30_30");
            AllTowerTexture[6] = game.Content.Load<Texture2D>(@"TowerImages\MagicTower_001_80_70");
            AllTowerTexture[7] = game.Content.Load<Texture2D>(@"TowerImages\MagicTower_final01_90_80");
            AllTowerTexture[8] = game.Content.Load<Texture2D>(@"TowerImages\MagicTower_final02_90_80");
            AllTowerTexture[9] = game.Content.Load<Texture2D>(@"TowerImages\pao_001_80_58");
            AllTowerTexture[10] = game.Content.Load<Texture2D>(@"TowerImages\pao_002_80_56");
            AllTowerTexture[11] = game.Content.Load<Texture2D>(@"TowerImages\pao_003_80_59");
            AllTowerTexture[12] = game.Content.Load<Texture2D>(@"TowerImages\pao_004_90_68");

            //塔初始化
            for (var i = 0; i < towers.Length; i++)
            {
                towers[i] = new TowersType(DrawTowerTextures, towerSprite, towerInfor, AllTowerTexture);
                towers[i].currentTowerType = TowersType.TowerTypes.None;
                towers[i].position = TowerPosition[i];
            }


            base.LoadContent();
        }

        #endregion

        public override void Initialize()
        {
            //  System.Diagnostics.Trace.WriteLine("GamePlayScreen:Intiliaze" + GameStateClass.ChosedLevel);
            base.Initialize();
            levelIndex = GameStateClass.ChosedLevel;
            LoadContent();
            //  MySound.StopSound();
            currentState = gameState.inGame;
            //静态类中的游戏数据初始化
            GameStateClass.AllPowerNum = 0;
            GameStateClass.AllLifeNum = 20;
            IsPaused = game.Content.Load<Texture2D>(@"UIImage\IsPaused");
            success = game.Content.Load<Texture2D>(@"UIImage\success");
            gameover = game.Content.Load<Texture2D>(@"UIImage\shibai");
            CurrentLevellsit =
                game.Content.Load<List<LevelRoundInfor>>(@"XmlData\LevelAndMapInfor_00" +
                                                         (GameStateClass.ChosedLevel + 1));
            NodeList = currentMap.GetTargetArray();
            //   allRound = currentMap.AllRound;
            GameStateClass.AllPowerNum = currentMap.AllPowerNumber; //字段名写错了
            UITexture = new Texture2D[4];
            UITexture[0] = game.Content.Load<Texture2D>(@"UIImage\UI_001"); //波数，能量值显示
            UITexture[1] = game.Content.Load<Texture2D>(@"UIImage\UI_Start"); //开始战斗
            UITexture[3] = game.Content.Load<Texture2D>(@"UIImage\Paused_002");
            UITexture[2] = game.Content.Load<Texture2D>(@"UIImage\Paused_001");
            allenemiesTypeList = game.Content.Load<List<EnemyType>>("XmlData\\EnemyInfor");
            EnemyTexture = new Texture2D[allenemiesTypeList.Count + 2];
            foreach (var enemytype in allenemiesTypeList)
            {
                switch (enemytype.EnemyName)
                {
                    case "小恶魔":
                        EnemyTexture[0] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "滴水兽":
                        EnemyTexture[1] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "暗骑士":
                        EnemyTexture[2] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "暗黑屠夫":
                        EnemyTexture[3] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "亡命徒":
                        EnemyTexture[4] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                }
            }
            EnemyTexture[5] = game.Content.Load<Texture2D>(@"UIImage\blood_green");
            EnemyTexture[6] = game.Content.Load<Texture2D>(@"UIImage\blood_red");

            AttackTexture[0] = game.Content.Load<Texture2D>(@"TowerImages\arrow");
            AttackTexture[1] = game.Content.Load<Texture2D>(@"TowerImages\bomb_01_50_65");
            AttackTexture[2] = game.Content.Load<Texture2D>(@"TowerImages\magicPower");

            AttackTexture[3] = game.Content.Load<Texture2D>(@"TowerImages\paodan");

            AllPowerNum = currentMap.AllPowerNumber;


            bulletsTexture[0] = game.Content.Load<Texture2D>(@"TowerImages\arrow"); //弓箭
            bulletsTexture[1] = game.Content.Load<Texture2D>(@"TowerImages\magicPower"); //魔法

            IsStartButtonEnable = true;

            //  mySound.PlaySound("ingame");
            var allroundnum = -1;
            foreach (var level in CurrentLevellsit)
            {
                if (level.RoundIndex >= allroundnum)
                    allroundnum = level.RoundIndex;
            }
            if (allroundnum != -1)
                allRound = allroundnum + 1;
        }

        /// <summary>
        ///     绘制塔，敌人绘制，塔发射的子弹伤害计算逻辑，敌人的，子弹的移动等。
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (GameStateClass.currentGameState != GameStateClass.GameState.MainGame) return;
            sBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            graphic.Clear(ClearOptions.DepthBuffer, Color.Black, 0.5f, 4);
            sBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (currentState == gameState.pause)
                sBatch.Draw(IsPaused,
                    new Rectangle(graphic.Viewport.Width/2 - 20, graphic.Viewport.Height/2 - 20, IsPaused.Width,
                        IsPaused.Height),
                    new Rectangle(0, 0, IsPaused.Width, IsPaused.Height), Color.White, 0, Vector2.Zero,
                    SpriteEffects.None, 0.001f);

            if (currentState == gameState.success)
                sBatch.Draw(success,
                    new Rectangle(graphic.Viewport.Width/3 - 20, graphic.Viewport.Height/3 - 20, success.Width,
                        success.Height),
                    new Rectangle(0, 0, success.Width, success.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                    0.001f);
            if (currentState == gameState.gameover)
                sBatch.Draw(gameover,
                    new Rectangle(graphic.Viewport.Width/3 - 20, graphic.Viewport.Height/3 - 20, success.Width,
                        success.Height),
                    new Rectangle(0, 0, success.Width, success.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                    0.001f);

            DrawUI(gameTime);
            if (MaplevelIndex != -1)
            {
                sBatch.Draw(MapImg, new Rectangle(0, 0, MapImg.Width, MapImg.Height),
                    new Rectangle(0, 0, MapImg.Width, MapImg.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None,
                    1.0f);
            }


            //所有位置的塔的绘制
            for (var i = 0; i < TowerPosition.Length; i++)
            {
                //  towers[i].position = TowerPosition[i];
                towers[i].Draw(gameTime, sBatch, TowerPosition[i], i);
            }

            #region 炮塔的子弹绘制，子弹移动，伤害逻辑

            var turretToRemove = new List<Turret>();
            foreach (var turret in turretsList)
            {
                sBatch.Draw(AttackTexture[3],
                    new Rectangle((int) turret.Position.X, (int) turret.Position.Y, AttackTexture[3].Width,
                        AttackTexture[3].Height)
                    , new Rectangle(0, 0, AttackTexture[3].Width, AttackTexture[3].Height), Color.White, 0, Vector2.Zero,
                    SpriteEffects.None, 0.1f);
                turret.Move();
                //   Console.WriteLine(turret.Position.X + "," + turret.Position.Y);

                if (turret.IsBombing)
                {
                    foreach (var enemy in AllEnemiesList)
                    {
                        var distance =
                            (int)
                                Math.Sqrt((enemy.Position.X - turret.Position.X)*(enemy.Position.X - turret.Position.X) +
                                          (enemy.Position.Y - turret.Position.Y)*(enemy.Position.Y - turret.Position.Y));
                        if (distance <= 30)
                        {
                            //在攻击距离内，根据护甲值计算产生的伤害
                            var damage = random.Next(turret.MaxDamage - turret.MinDamage) + turret.MinDamage -
                                         (int) 0.5*enemy.armor;
                            // int damage = 80;
                            enemy.lifeNum -= damage;
                        }
                        if (enemy.lifeNum <= 0)
                        {
                            enemy.IsAlive = false;
                            GameStateClass.AllPowerNum += 20;
                        }
                    }
                    turret.IsAlive = false;
                    turret.IsBombing = false;
                }

                if (Math.Abs(turret.Position.X - turret.desPosition.X) < 4)
                {
                    turret.IsBombing = true;
                }
                if (turret.IsAlive == false)
                    turretToRemove.Add(turret);
            }

            foreach (var turret in turretToRemove)
            {
                turretsList.Remove(turret);
            }

            #endregion

            #region 魔法塔和箭塔的子弹绘制，子弹移动，伤害逻辑

            var bulletsToRemove = new List<Bullets>();
            foreach (var bullet in MagicBullets)
            {
                var MagicRec = new Rectangle((int) bullet.Position.X, (int) bullet.Position.Y, 10, 10);
                sBatch.Draw(bulletsTexture[1], MagicRec, Color.White);
                bullet.Move();
                if (Math.Abs(bullet.Position.X - bullet.desPosition.X) < 3 &&
                    Math.Abs(bullet.Position.Y - bullet.desPosition.Y) < 3)
                {
                    foreach (var enemy in AllEnemiesList)
                    {
                        var enemyRec = new Rectangle((int) enemy.Position.X, (int) enemy.Position.Y, 80, 80);

                        if (MagicRec.Intersects(enemyRec))
                        {
                            //在攻击距离内，根据护甲值计算产生的伤害
                            var damage = random.Next(bullet.MaxDamage - bullet.MinDamage) + bullet.MinDamage -
                                         (int) 0.5*enemy.AgainstSpells;
                            enemy.lifeNum -= damage;
                        }
                        if (enemy.lifeNum <= 0)
                        {
                            enemy.IsAlive = false;
                            GameStateClass.AllPowerNum += 20;
                        }
                    }

                    bullet.IsAlive = false;
                }
                if (!bullet.IsAlive)
                    bulletsToRemove.Add(bullet);
            }
            foreach (var bullet in bulletsToRemove)
            {
                MagicBullets.Remove(bullet);
            }
            bulletsToRemove.RemoveRange(0, bulletsToRemove.Count);
            foreach (var bullet in ArrowBullets)
            {
                var ArrowRec = new Rectangle((int) bullet.Position.X, (int) bullet.Position.Y, 20, 10);

                sBatch.Draw(bulletsTexture[0], ArrowRec,
                    new Rectangle(0, 0, bulletsTexture[0].Width, bulletsTexture[0].Height), Color.White
                    , bullet.rotation, Vector2.Zero, SpriteEffects.None, 0.1f);
                bullet.Move();

                if (Math.Abs(bullet.Position.X - bullet.desPosition.X) < 3 &&
                    Math.Abs(bullet.Position.Y - bullet.desPosition.Y) < 3)
                {
                    // Console.WriteLine("在魔法塔攻击范围内");
                    foreach (var enemy in AllEnemiesList)
                    {
                        var enemyRec = new Rectangle((int) enemy.Position.X, (int) enemy.Position.Y, 80, 80);

                        if (ArrowRec.Intersects(enemyRec))
                        {
                            //Console.WriteLine("撞到了");
                            //在攻击距离内，根据护甲值计算产生的伤害
                            var damage = random.Next(bullet.MaxDamage - bullet.MinDamage) + bullet.MinDamage -
                                         (int) 0.5*enemy.armor;
                            // int damage = 80;
                            enemy.lifeNum -= damage;
                            bullet.IsAlive = false;
                        }
                        if (enemy.lifeNum <= 0)
                        {
                            enemy.IsAlive = false;
                            GameStateClass.AllPowerNum += 20;
                        }
                    }
                    bullet.IsAlive = false;


                    // bullet.IsBombing = false;
                }
                if (!bullet.IsAlive)
                    bulletsToRemove.Add(bullet);
            }
            foreach (var bullet in bulletsToRemove)
            {
                ArrowBullets.Remove(bullet);
            }

            #endregion

            var enemyToRemove = new List<EnemiesInfor>();
            foreach (var enemy in AllEnemiesList)
            {
                // System.Diagnostics.Trace.WriteLine("此敌人当前生命值:" + enemy.lifeNum + "此敌人当前生命值:"+enemy.Position.X.ToString()+"\n");
                //  sBatch.DrawString(sf, enemy.Position.X.ToString() + "," + enemy.Position.Y.ToString(), enemy.Position, Color.Yellow);
                //sBatch.DrawString(sf, enemy.lifeNum.ToString()+"/"+enemy.AllLifeNum.ToString(), enemy.Position, Color.Yellow);        //炮弹总数

                // enemy.Move(gameTime);
                enemy.Draw(sBatch, gameTime);

                if (!enemy.IsAlive)
                    enemyToRemove.Add(enemy);
            }
            foreach (var enemy in enemyToRemove)
                AllEnemiesList.Remove(enemy);
            //5    sBatch.DrawString(sf, AllEnemiesList.Count.ToString(), new Vector2(120,60), Color.Yellow);  //敌人总数
            //     sBatch.DrawString(sf, turretsList.Count.ToString(), new Vector2(50, 160), Color.Yellow);        //炮弹总数
            //   sBatch.DrawString(sf, ArrowBullets.Count.ToString(), new Vector2(50, 160), Color.Yellow);

            sBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        ///     根据当前波数，将敌人添加进列表中，另外
        ///     塔的攻击逻辑也在update里实现
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (currentState == gameState.inGame)
            {
                foreach (var enemy in AllEnemiesList)
                {
                    // System.Diagnostics.Trace.WriteLine("此敌人当前生命值:" + enemy.lifeNum + "此敌人当前生命值:"+enemy.Position.X.ToString()+"\n");
                    //  sBatch.DrawString(sf, enemy.Position.X.ToString() + "," + enemy.Position.Y.ToString(), enemy.Position, Color.Yellow);
                    //sBatch.DrawString(sf, enemy.lifeNum.ToString()+"/"+enemy.AllLifeNum.ToString(), enemy.Position, Color.Yellow);        //炮弹总数

                    enemy.Move(gameTime);
                }
                coolTime += gameTime.ElapsedGameTime.Milliseconds;
                if (GameStateClass.currentGameState != GameStateClass.GameState.MainGame) return;
                if (AllEnemiesList.Count < 15)
                {
                    IsStartButtonEnable = true;
                    //     IsCurrentWaveEnable = true;
                }
                var levelToRemove = new LevelRoundInfor();

                #region 将敌人添加进列表

                if (CurrentRoundList.Count != 0)
                {
                    foreach (var level in CurrentRoundList)
                        if (coolTime >= 200)
                        {
                            foreach (var enemytype in allenemiesTypeList)
                            {
                                if (enemytype.EnemyName == level.EnemyName)
                                {
                                    var enemies = new EnemiesInfor[level.EnemiesCount];
                                    for (var i = 0; i < level.EnemiesCount; i++)
                                    {
                                        enemies[i] = new EnemiesInfor(EnemyTexture, NodeList);
                                        enemies[i].Interval = enemytype.Interval;
                                        enemies[i].IsAlive = true;
                                        enemies[i].IsFlying = enemytype.IsFlying;
                                        enemies[i].lifeNum = enemytype.LifeNum;
                                        //  System.Diagnostics.Trace.WriteLine("gameplay:" + enemytype.LifeNum);

                                        enemies[i].MaxDamage = enemytype.MaxDamage;
                                        enemies[i].MinDamage = enemytype.MinDamage;
                                        enemies[i].range = enemytype.Range;
                                        if (NodeList[0].X > NodeList[0].Y && NodeList[0].X < 600)
                                        {
                                            enemies[i].Position.X = NodeList[0].X + (int) (random.NextDouble()*4);
                                            enemies[i].Position.Y = NodeList[0].Y + (int) (random.NextDouble()*4) - i*40 -
                                                                    20;
                                        }
                                        if (NodeList[0].X > NodeList[0].Y && NodeList[0].X > 600)
                                        {
                                            enemies[i].Position.X = NodeList[0].X + (int) (random.NextDouble()*4) + i*40 +
                                                                    20;
                                            enemies[i].Position.Y = NodeList[0].Y + (int) (random.NextDouble()*4);
                                        }
                                        if (NodeList[0].Y > NodeList[0].X && NodeList[0].Y > 400)
                                        {
                                            enemies[i].Position.X = NodeList[0].X;
                                            enemies[i].Position.Y = NodeList[0].Y + (int) (random.NextDouble()*4) + i*60;
                                        }

                                        enemies[i].armor = enemytype.Armor; //护甲
                                        enemies[i].AgainstSpells = enemytype.AgainstSpells; //磨抗
                                        enemies[i].enemyName = enemytype.EnemyName;
                                        enemies[i].AllLifeNum = enemytype.LifeNum;
                                    }
                                    AllEnemiesList.AddRange(enemies);
                                    levelToRemove = level;
                                }
                            }
                            coolTime = 0;
                        }

                    CurrentRoundList.Remove(levelToRemove);
                }

                #endregion

                #region 敌人与塔的攻击逻辑

                foreach (var CurrentTower in towers)
                {
                    //if (CurrentTower.currentTowerType != TowersType.TowerTypes.None)
                    foreach (var enemy in AllEnemiesList)
                    {
                        if (CurrentTower.cooldownTime == 0 &&
                            (CurrentTower.currentTowerType == TowersType.TowerTypes.TurretTower_level1
                             || CurrentTower.currentTowerType == TowersType.TowerTypes.TurretTower_level2 ||
                             CurrentTower.currentTowerType == TowersType.TowerTypes.TurretTower_level3 ||
                             CurrentTower.currentTowerType == TowersType.TowerTypes.TurretTower_level4))
                        {
                            //  System.Diagnostics.Trace.WriteLine("正在判断是否攻击");

                            var distance =
                                (int)
                                    Math.Sqrt(((int) enemy.Position.X - CurrentTower.position.X)*
                                              ((int) enemy.Position.X - CurrentTower.position.X) +
                                              ((int) enemy.Position.Y - (int) CurrentTower.position.Y)*
                                              ((int) enemy.Position.Y - (int) CurrentTower.position.Y));
                            //System.Diagnostics.Trace.WriteLine("distance" + distance);
                            if (distance <= CurrentTower.currentTower.Range)
                            {
                                CurrentTower.HitEnemy(gameTime, sBatch, enemy.Position, this);
                                //   System.Diagnostics.Trace.WriteLine("正在攻击敌人");  
                            }
                        }
                        else if (CurrentTower.cooldownTime == 0)
                        {
                            CurrentTower.TargetEnemy = enemy;
                            var distance =
                                (int)
                                    Math.Sqrt(((int) enemy.Position.X - CurrentTower.position.X)*
                                              ((int) enemy.Position.X - CurrentTower.position.X) +
                                              ((int) enemy.Position.Y - (int) CurrentTower.position.Y)*
                                              ((int) enemy.Position.Y - (int) CurrentTower.position.Y));
                            // System.Diagnostics.Trace.WriteLine("distance" + distance);
                            if (distance <= CurrentTower.currentTower.Range)
                            {
                                CurrentTower.HitTargetEnemy(gameTime, sBatch, enemy.Position, this);
                                // System.Diagnostics.Trace.WriteLine("魔法塔正在攻击敌人");  
                            }
                        }
                        // System.Diagnostics.Trace.WriteLine("此敌人当前生命值:" + enemy.lifeNum + "此敌人当前生命值:"+enemy.Position.X.ToString()+"\n");
                        //  sBatch.DrawString(sf, enemy.Position.X.ToString() + "," + enemy.Position.Y.ToString(), enemy.Position, Color.Yellow);
                    }
                }

                #endregion
            }
            if (inputeState.IsNewKeyPress(Keys.Escape))
            {
                // GameStateClass.changeState(GameStateClass.GameState.LevelChoose, game);
                game.Exit();
            }
            inputeState.Update();

            if (AllEnemiesList.Count == 0 && currentRound + 1 == allRound && GameStateClass.AllLifeNum != 0)
                currentState = gameState.success;
            if (currentRound + 1 == allRound)
                IsStartButtonEnable = false;
            if (GameStateClass.AllLifeNum < 0)
                currentState = gameState.gameover;

            //          if (StateOFgame != GameStateClass.currentGameState)
            //          {

            //              StateOFgame = GameStateClass.currentGameState;
            //              AllEnemiesList.RemoveRange(0, AllEnemiesList.Count);
            //              currentRound = 0;
            //              Console.WriteLine("这一关结束了");
            //this.Initialize();
            //          }
            base.Update(gameTime);
        }

        /// <summary>
        ///     绘制界面，剩余敌人数量，剩余生命值
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawUI(GameTime gameTime)
        {
            AllPowerNum = GameStateClass.AllPowerNum;
            var StartRec = new Rectangle(graphic.Viewport.Width - UITexture[1].Width - 20,
                graphic.Viewport.Height - UITexture[1].Height - 20, UITexture[1].Width/2, UITexture[1].Height);
            var waveDisplayRec = new Rectangle(0, 0, UITexture[0].Width, UITexture[0].Height);
            var PausedRec = new Rectangle(graphic.Viewport.Width - UITexture[3].Width - 50, 20, UITexture[3].Width,
                UITexture[3].Height);
            sBatch.Draw(UITexture[0], waveDisplayRec, new Rectangle(0, 0, UITexture[0].Width, UITexture[0].Height),
                Color.White, 0,
                Vector2.Zero, SpriteEffects.None, 0.4f);

            //剩余的生命值
            sBatch.DrawString(sf, GameStateClass.AllLifeNum.ToString(), new Vector2(55, 18), Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            //剩余的能量值
            sBatch.DrawString(sf, GameStateClass.AllPowerNum.ToString(), new Vector2(120, 18), Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            //波数和总波数
            sBatch.DrawString(sf, (currentRound + 1) + "/" + allRound + "\n" + "Enemies Left:" + AllEnemiesList.Count,
                new Vector2(100, 55), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            #region 绘制各种按钮和及其相应

            elspseTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elspseTime > 1000)
                elspseTime = 0;
            if (IsStartButtonEnable)
            {
                if (inputeState.IsMouseOn(StartRec))
                {
                    sBatch.Draw(UITexture[1], StartRec,
                        new Rectangle(UITexture[1].Width/2, 0, UITexture[1].Width/2, UITexture[1].Height), Color.White,
                        0,
                        Vector2.Zero, SpriteEffects.None, 0.4f);
                    if (inputeState.IsLeftButtonClick())
                    {
                        IsStartButtonEnable = false;
                        currentRound++;
                        foreach (var level in CurrentLevellsit)
                        {
                            if (level.RoundIndex == currentRound)
                                CurrentRoundList.Add(level);
                        }
                        Console.WriteLine(CurrentRoundList.Count);
                    }
                }
                else if (elspseTime > 500)
                    sBatch.Draw(UITexture[1], StartRec,
                        new Rectangle(UITexture[1].Width/2, 0, UITexture[1].Width/2, UITexture[1].Height), Color.White,
                        0,
                        Vector2.Zero, SpriteEffects.None, 0.4f);
                else
                    sBatch.Draw(UITexture[1], StartRec, new Rectangle(0, 0, UITexture[1].Width/2, UITexture[1].Height),
                        Color.White, 0,
                        Vector2.Zero, SpriteEffects.None, 0.4f);
            }
            else
                sBatch.Draw(UITexture[1], StartRec, new Rectangle(0, 0, UITexture[1].Width/2, UITexture[1].Height),
                    Color.White, 0,
                    Vector2.Zero, SpriteEffects.None, 0.4f);
            if (inputeState.IsMouseOn(PausedRec))
            {
                sBatch.Draw(UITexture[3], PausedRec, new Rectangle(0, 0, UITexture[3].Width, UITexture[3].Height),
                    Color.White, 0,
                    Vector2.Zero, SpriteEffects.None, 0.4f);
                if (inputeState.IsLeftButtonClick())
                {
                    //  currentState = gameState.pause;
                    if (currentState == gameState.pause)
                        currentState = gameState.inGame;
                    else
                        currentState = gameState.pause;
                }
            }
            else
                sBatch.Draw(UITexture[2], PausedRec, new Rectangle(0, 0, UITexture[3].Width, UITexture[3].Height),
                    Color.White, 0,
                    Vector2.Zero, SpriteEffects.None, 0.4f);

            #endregion
        }

        public void addTurrets(Turret turret)
        {
            turretsList.Add(turret);
        }

        //加入子弹序列，1表示魔法塔发射的子弹，2表示弓箭塔发射的子弹
        public void addBullets(Bullets bullet, int type)
        {
            if (type == 1)
                MagicBullets.Add(bullet);
            else if (type == 2)
                ArrowBullets.Add(bullet);
        }

        private enum gameState
        {
            inGame,
            success,
            gameover,
            pause
        }

        #region field

        private readonly Game game;
        private SpriteBatch sBatch;
        private readonly GraphicsDevice graphic;
        private readonly Random random = new Random();
        private Texture2D IsPaused;
        private Texture2D success;
        private Texture2D gameover;
        //输入状态，地图图片，地图信息，塔位置信息声明
        private readonly InputState inputeState;
        private Texture2D MapImg;
        private List<MapInfor> mapInfrolist;
        private MapInfor currentMap;
        private Vector2[] TowerPosition;
        //攻击效果图片
        private readonly Texture2D[] AttackTexture = new Texture2D[4];

        //塔的图片，塔的信息字体文件声明
        private readonly Texture2D[] DrawTowerTextures = new Texture2D[8];
        private TowersType[] towers;
        private SpriteFont sf;
        //地图上塔的序列声明
        private List<TowerInfor> towerInfor;
        //拥有的总能量值
        public int AllPowerNum;


        // int WavesCount=0;         //敌人总波数
        private int MaplevelIndex;


        //所有塔的图片，传给TowersType
        private Texture2D[] AllTowerTexture;

        //关卡信息的初始化
        private List<LevelRoundInfor> CurrentLevellsit;

        //炮弹序列
        private readonly List<Turret> turretsList = new List<Turret>();
        //魔法塔子弹序列
        private readonly List<Bullets> MagicBullets = new List<Bullets>();
        //弓箭塔序列
        private readonly List<Bullets> ArrowBullets = new List<Bullets>();
        //关卡信息序列
        private readonly List<LevelRoundInfor> CurrentRoundList = new List<LevelRoundInfor>();
        private int currentRound = -1;
        private int allRound;
        private Vector2[] NodeList;
        private Texture2D[] UITexture;
        //敌人信息声明
        private List<EnemyType> allenemiesTypeList;
        private readonly List<EnemiesInfor> AllEnemiesList = new List<EnemiesInfor>();
        private int levelIndex;
        private List<Turret> MyArmies = new List<Turret>();
        private int elspseTime;
        private bool IsStartButtonEnable;

        private int coolTime;
        //敌人贴图声明
        private Texture2D[] EnemyTexture;
        //弓箭的图片，魔法子弹的图片
        private readonly Texture2D[] bulletsTexture = new Texture2D[2];

        private SpriteFont towerSprite;
        private gameState currentState;
        private GameStateClass.GameState StateOFgame;
        private MySound mySound;

        #endregion
    }
}