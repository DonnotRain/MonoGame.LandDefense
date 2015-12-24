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
    ///     ������Ϸ�����߼�
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

        #region ��ʼ����������

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
            DrawTowerTextures[0] = game.Content.Load<Texture2D>(@"UIImage\collecting_point"); //0�Ǽ��ϵ�ͼƬ
            DrawTowerTextures[1] = game.Content.Load<Texture2D>(@"UIImage\NeededPower"); //1����Ҫ��������ʾ����ͼƬ
            DrawTowerTextures[2] = game.Content.Load<Texture2D>(@"UIImage\TowerSelect46_44"); //2��ѡ�����ļ��ϡ�������������ͼ��
            DrawTowerTextures[3] = game.Content.Load<Texture2D>(@"UIImage\TowerSelectBack"); //��ȡ��꽹��ı���,�л���ɫ��
            DrawTowerTextures[4] = game.Content.Load<Texture2D>(@"UIImage\SellForPower"); //��������ͼ�꣬��������������
            DrawTowerTextures[5] = game.Content.Load<Texture2D>(@"TowersImg\Mark_Tower"); //��ʼ����ͼ��
            DrawTowerTextures[6] = game.Content.Load<Texture2D>(@"TowersImg\Mark_Tower_02"); //��ʼ����ͼ��,���ָ��������ʱ������

            DrawTowerTextures[7] = game.Content.Load<Texture2D>(@"UIImage\JustCircle"); //��ͺͺ��һ��Ȧ
            //  DrawTowerTextures[8] = game.Content.Load<Texture2D>(@"TowersImg\TowerUpgrade");               //��������ͼ��


            AllTowerTexture = new Texture2D[13];
            AllTowerTexture[0] = game.Content.Load<Texture2D>(@"TowerImages\Arrow_001_80_70"); //̫���ˣ�ע��û��д�����ǲ�����ͼƬ���ɡ���
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

            //����ʼ��
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
            //��̬���е���Ϸ���ݳ�ʼ��
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
            GameStateClass.AllPowerNum = currentMap.AllPowerNumber; //�ֶ���д����
            UITexture = new Texture2D[4];
            UITexture[0] = game.Content.Load<Texture2D>(@"UIImage\UI_001"); //����������ֵ��ʾ
            UITexture[1] = game.Content.Load<Texture2D>(@"UIImage\UI_Start"); //��ʼս��
            UITexture[3] = game.Content.Load<Texture2D>(@"UIImage\Paused_002");
            UITexture[2] = game.Content.Load<Texture2D>(@"UIImage\Paused_001");
            allenemiesTypeList = game.Content.Load<List<EnemyType>>("XmlData\\EnemyInfor");
            EnemyTexture = new Texture2D[allenemiesTypeList.Count + 2];
            foreach (var enemytype in allenemiesTypeList)
            {
                switch (enemytype.EnemyName)
                {
                    case "С��ħ":
                        EnemyTexture[0] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "��ˮ��":
                        EnemyTexture[1] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "����ʿ":
                        EnemyTexture[2] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "��������":
                        EnemyTexture[3] = game.Content.Load<Texture2D>(@enemytype.TextureName);
                        continue;
                    case "����ͽ":
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


            bulletsTexture[0] = game.Content.Load<Texture2D>(@"TowerImages\arrow"); //����
            bulletsTexture[1] = game.Content.Load<Texture2D>(@"TowerImages\magicPower"); //ħ��

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
        ///     �����������˻��ƣ���������ӵ��˺������߼������˵ģ��ӵ����ƶ��ȡ�
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


            //����λ�õ����Ļ���
            for (var i = 0; i < TowerPosition.Length; i++)
            {
                //  towers[i].position = TowerPosition[i];
                towers[i].Draw(gameTime, sBatch, TowerPosition[i], i);
            }

            #region �������ӵ����ƣ��ӵ��ƶ����˺��߼�

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
                            //�ڹ��������ڣ����ݻ���ֵ����������˺�
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

            #region ħ�����ͼ������ӵ����ƣ��ӵ��ƶ����˺��߼�

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
                            //�ڹ��������ڣ����ݻ���ֵ����������˺�
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
                    // Console.WriteLine("��ħ����������Χ��");
                    foreach (var enemy in AllEnemiesList)
                    {
                        var enemyRec = new Rectangle((int) enemy.Position.X, (int) enemy.Position.Y, 80, 80);

                        if (ArrowRec.Intersects(enemyRec))
                        {
                            //Console.WriteLine("ײ����");
                            //�ڹ��������ڣ����ݻ���ֵ����������˺�
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
                // System.Diagnostics.Trace.WriteLine("�˵��˵�ǰ����ֵ:" + enemy.lifeNum + "�˵��˵�ǰ����ֵ:"+enemy.Position.X.ToString()+"\n");
                //  sBatch.DrawString(sf, enemy.Position.X.ToString() + "," + enemy.Position.Y.ToString(), enemy.Position, Color.Yellow);
                //sBatch.DrawString(sf, enemy.lifeNum.ToString()+"/"+enemy.AllLifeNum.ToString(), enemy.Position, Color.Yellow);        //�ڵ�����

                // enemy.Move(gameTime);
                enemy.Draw(sBatch, gameTime);

                if (!enemy.IsAlive)
                    enemyToRemove.Add(enemy);
            }
            foreach (var enemy in enemyToRemove)
                AllEnemiesList.Remove(enemy);
            //5    sBatch.DrawString(sf, AllEnemiesList.Count.ToString(), new Vector2(120,60), Color.Yellow);  //��������
            //     sBatch.DrawString(sf, turretsList.Count.ToString(), new Vector2(50, 160), Color.Yellow);        //�ڵ�����
            //   sBatch.DrawString(sf, ArrowBullets.Count.ToString(), new Vector2(50, 160), Color.Yellow);

            sBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        ///     ���ݵ�ǰ��������������ӽ��б��У�����
        ///     ���Ĺ����߼�Ҳ��update��ʵ��
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (currentState == gameState.inGame)
            {
                foreach (var enemy in AllEnemiesList)
                {
                    // System.Diagnostics.Trace.WriteLine("�˵��˵�ǰ����ֵ:" + enemy.lifeNum + "�˵��˵�ǰ����ֵ:"+enemy.Position.X.ToString()+"\n");
                    //  sBatch.DrawString(sf, enemy.Position.X.ToString() + "," + enemy.Position.Y.ToString(), enemy.Position, Color.Yellow);
                    //sBatch.DrawString(sf, enemy.lifeNum.ToString()+"/"+enemy.AllLifeNum.ToString(), enemy.Position, Color.Yellow);        //�ڵ�����

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

                #region ��������ӽ��б�

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

                                        enemies[i].armor = enemytype.Armor; //����
                                        enemies[i].AgainstSpells = enemytype.AgainstSpells; //ĥ��
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

                #region ���������Ĺ����߼�

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
                            //  System.Diagnostics.Trace.WriteLine("�����ж��Ƿ񹥻�");

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
                                //   System.Diagnostics.Trace.WriteLine("���ڹ�������");  
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
                                // System.Diagnostics.Trace.WriteLine("ħ�������ڹ�������");  
                            }
                        }
                        // System.Diagnostics.Trace.WriteLine("�˵��˵�ǰ����ֵ:" + enemy.lifeNum + "�˵��˵�ǰ����ֵ:"+enemy.Position.X.ToString()+"\n");
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
            //              Console.WriteLine("��һ�ؽ�����");
            //this.Initialize();
            //          }
            base.Update(gameTime);
        }

        /// <summary>
        ///     ���ƽ��棬ʣ�����������ʣ������ֵ
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

            //ʣ�������ֵ
            sBatch.DrawString(sf, GameStateClass.AllLifeNum.ToString(), new Vector2(55, 18), Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            //ʣ�������ֵ
            sBatch.DrawString(sf, GameStateClass.AllPowerNum.ToString(), new Vector2(120, 18), Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            //�������ܲ���
            sBatch.DrawString(sf, (currentRound + 1) + "/" + allRound + "\n" + "Enemies Left:" + AllEnemiesList.Count,
                new Vector2(100, 55), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            #region ���Ƹ��ְ�ť�ͼ�����Ӧ

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

        //�����ӵ����У�1��ʾħ����������ӵ���2��ʾ������������ӵ�
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
        //����״̬����ͼͼƬ����ͼ��Ϣ����λ����Ϣ����
        private readonly InputState inputeState;
        private Texture2D MapImg;
        private List<MapInfor> mapInfrolist;
        private MapInfor currentMap;
        private Vector2[] TowerPosition;
        //����Ч��ͼƬ
        private readonly Texture2D[] AttackTexture = new Texture2D[4];

        //����ͼƬ��������Ϣ�����ļ�����
        private readonly Texture2D[] DrawTowerTextures = new Texture2D[8];
        private TowersType[] towers;
        private SpriteFont sf;
        //��ͼ��������������
        private List<TowerInfor> towerInfor;
        //ӵ�е�������ֵ
        public int AllPowerNum;


        // int WavesCount=0;         //�����ܲ���
        private int MaplevelIndex;


        //��������ͼƬ������TowersType
        private Texture2D[] AllTowerTexture;

        //�ؿ���Ϣ�ĳ�ʼ��
        private List<LevelRoundInfor> CurrentLevellsit;

        //�ڵ�����
        private readonly List<Turret> turretsList = new List<Turret>();
        //ħ�����ӵ�����
        private readonly List<Bullets> MagicBullets = new List<Bullets>();
        //����������
        private readonly List<Bullets> ArrowBullets = new List<Bullets>();
        //�ؿ���Ϣ����
        private readonly List<LevelRoundInfor> CurrentRoundList = new List<LevelRoundInfor>();
        private int currentRound = -1;
        private int allRound;
        private Vector2[] NodeList;
        private Texture2D[] UITexture;
        //������Ϣ����
        private List<EnemyType> allenemiesTypeList;
        private readonly List<EnemiesInfor> AllEnemiesList = new List<EnemiesInfor>();
        private int levelIndex;
        private List<Turret> MyArmies = new List<Turret>();
        private int elspseTime;
        private bool IsStartButtonEnable;

        private int coolTime;
        //������ͼ����
        private Texture2D[] EnemyTexture;
        //������ͼƬ��ħ���ӵ���ͼƬ
        private readonly Texture2D[] bulletsTexture = new Texture2D[2];

        private SpriteFont towerSprite;
        private gameState currentState;
        private GameStateClass.GameState StateOFgame;
        private MySound mySound;

        #endregion
    }
}