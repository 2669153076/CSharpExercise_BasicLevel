using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp基础实践
{
    internal class Program
    {
        #region 枚举
        /// <summary>
        /// 游戏场景类型枚举
        /// </summary>
        enum E_SceneType
        {
            /// <summary>
            /// 开始场景
            /// </summary>
            Begin,
            /// <summary>
            /// 游戏场景
            /// </summary>
            Game,
            /// <summary>
            /// 结束场景
            /// </summary>
            End
        }

        /// <summary>
        /// 格子类型枚举
        /// </summary>
        enum E_GridType
        {
            /// <summary>
            /// 普通格子
            /// </summary>
            Normal,
            /// <summary>
            /// 炸弹
            /// </summary>
            Boom,
            /// <summary>
            /// 暂停
            /// </summary>
            Pause,
            /// <summary>
            /// 时空隧道 随机倒退、暂停、换位置
            /// </summary>
            Tunnel
        }

        /// <summary>
        /// 玩家类型枚举
        /// </summary>
        enum E_PlayerType
        {
            Player,
            Computer
        }
        #endregion

        #region 结构体
        /// <summary>
        /// 位置结构体
        /// </summary>
        struct Vector2
        {
            private int x;
            private int y;

            public Vector2(int x,int y)
            {
                this.x = x;
                this.y = y;
            }

            public void setX(int x) { this.x = x; }
            public int getX() { return x; }
            public void setY(int y) { this.y = y; }
            public int getY() { return y; }
        }

        /// <summary>
        /// 格子结构体
        /// </summary>
        struct Grid
        {
            private E_GridType type;
            private Vector2 pos;

            public Grid(E_GridType type,int x,int y)
            {
                this.type = type;
                pos = new Vector2(x, y);
            }

            public void setType(E_GridType type) { this.type = type; }
            public E_GridType getType() { return type; }
            public void setPos(Vector2 pos) { this.pos = pos; }
            public Vector2 getPos() { return pos; }

            public void Draw()
            {
                Console.SetCursorPosition(pos.getX(), pos.getY());
                switch (type)
                {
                    case E_GridType.Normal:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("□");
                        break;
                    case E_GridType.Boom:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("●");
                        break;
                    case E_GridType.Pause:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("∥");
                        break;
                    case E_GridType.Tunnel:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("¤");
                        break;
                }
            }
        }

        /// <summary>
        /// 地图结构体
        /// </summary>
        struct Map
        {
            private Grid[] grids;

            public Map(int x,int y,int num)
            {
                grids = new Grid[num];
                int xCounter = 0;
                int yCounter = 0;

                Random random = new Random();
                int randomNum;
                int stepXNum = 2;
                for(int i = 0; i < grids.Length; ++i)
                {
                    randomNum = random.Next(0, 101);

                    //普通格子概率为85%，且首尾格子为普通格子
                    //炸弹、暂停、时空隧道 均为5%
                    if (randomNum < 85 || i == 0 || i == num - 1)
                    {
                        grids[i].setType(E_GridType.Normal);
                    }
                    else if (randomNum >= 85 && randomNum < 90)
                    {
                        grids[i].setType(E_GridType.Boom);
                    }
                    else if(randomNum>=90&& randomNum < 95)
                    {
                        grids[i].setType(E_GridType.Pause);
                    }
                    else
                    {
                        grids[i].setType(E_GridType.Tunnel);
                    }

                    grids[i].setPos(new Vector2(x,y));

                    if (xCounter == 14)
                    {
                        ++y;
                        ++yCounter;
                        if (yCounter == 4)
                        {
                            xCounter = 0;
                            yCounter = 0;
                            stepXNum = -stepXNum;
                        }
                    }
                    else
                    {
                        x += stepXNum;
                        ++xCounter;

                    }
                }
            }

            public void Draw()
            {
                for(int i = 0; i < grids.Length; i++)
                {
                    grids[i].Draw();
                }
            }

            public Grid getGrid(int index) { return grids[index]; }
            public Grid[] getGrids() { return grids; }
        }

        /// <summary>
        /// 玩家结构体
        /// </summary>
        struct Player
        {
            public E_PlayerType type;
            public int currentIndex;    //当前位置在地图哪一个索引的格子
            public bool isPause;

            public Player(int index,E_PlayerType type)
            {
                currentIndex = index;
                this.type = type;
                isPause = false;
            }

            public void Draw(Map mapInfo)
            {
                Grid grid = mapInfo.getGrid(currentIndex);

                Console.SetCursorPosition(grid.getPos().getX(), grid.getPos().getY());

                switch (type)
                {
                    case E_PlayerType.Player:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("★");
                        break;
                    case E_PlayerType.Computer:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("▲");
                        break;
                }
            }

            public int getCurrentIndex() { return currentIndex; }
            public void setCurrentIndex(int index) { this.currentIndex = index; }
            public bool getIsPause() { return isPause; }
            public void setIsPause(bool isPause) { this.isPause=isPause; }
            public E_PlayerType getType() { return type; }
        }
        #endregion

        #region 窗口设置

        /// <summary>
        /// 窗口设置
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        static void ConsoleInit(int weight, int height)
        {
            Console.SetWindowSize(weight, height);
            Console.SetBufferSize(weight, height);
            Console.CursorVisible = false;
        }

        #endregion

        #region 场景切换

        /// <summary>
        /// 开始、结束场景设置
        /// </summary>
        /// <param name="Weight"></param>
        /// <param name="height"></param>
        /// <param name="currentSceneType"></param>
        static void ChangeScene(int Weight, int height, ref E_SceneType currentSceneType)
        {
            Console.SetCursorPosition(currentSceneType == E_SceneType.Begin ? Weight / 2 - 3: Weight / 2 - 4, height / 2 - 8);
            Console.ForegroundColor = currentSceneType==E_SceneType.Begin?ConsoleColor.White:ConsoleColor.Magenta;
            Console.Write(currentSceneType==E_SceneType.Begin?"飞行棋":"游戏结束");

            //当前选项编号
            int currentSelect = 0;
            //是否退出场景
            bool isExitScene = false;
            while (true)
            {
                Console.SetCursorPosition(currentSceneType == E_SceneType.Begin ? Weight / 2 - 4:Weight/2-5, height / 2 + 4);
                Console.ForegroundColor = currentSelect == 0 ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write(currentSceneType == E_SceneType.Begin ? "开始游戏":"回到主菜单");
                Console.SetCursorPosition(Weight / 2 - 4, height / 2 + 8);
                Console.ForegroundColor = currentSelect == 1 ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write("退出游戏");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        currentSelect = 0;
                        break;
                    case ConsoleKey.S:
                        currentSelect = 1;
                        break;
                    case ConsoleKey.J:
                        if (currentSelect == 0)
                        {
                            currentSceneType = currentSceneType == E_SceneType.Begin ? E_SceneType.Game:E_SceneType.Begin;
                            isExitScene = true;
                            break;
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
                if (isExitScene)
                {
                    break;
                }
            }

        }

        #endregion

        #region 游戏场景
        /// <summary>
        /// 固定信息 不变的红墙与游戏说明
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        static void DrawRedWall(int weight,int height)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            //上
            for (int i = 0; i < weight; i += 2)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
            }
            //下
            for (int i = 0; i < weight; i += 2)
            {
                Console.SetCursorPosition(i, height - 2);
                Console.Write("■");
            }
            //左
            for (int i = 1; i < height - 2; ++i)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
            }
            //右
            for (int i = 1; i < height - 2; ++i)
            {
                Console.SetCursorPosition(weight - 2, i);
                Console.Write("■");
            }
            //下->上 1
            for (int i = 2; i < weight - 2; i += 2)
            {
                Console.SetCursorPosition(i, height - 7);
                Console.Write("■");
            }
            //下->上 2
            for (int i = 2; i < weight - 2; i += 2)
            {
                Console.SetCursorPosition(i, height - 12);
                Console.Write("■");
            }

            Console.SetCursorPosition(2, height - 11);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("□:普通格子");

            Console.SetCursorPosition(2, height - 10);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("∥:暂停，一回合不动\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("●:炸弹，倒退5格");

            Console.SetCursorPosition(2, height - 9);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("¤:时空隧道，随机倒退、暂停、换位置");

            Console.SetCursorPosition(2, height - 8);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("★:玩家\t ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("▲:电脑\t ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("◎:玩家电脑重合");

            Console.SetCursorPosition(2, height - 6);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("请按任意键开始扔骰子");
        }

        /// <summary>
        /// 绘制玩家
        /// </summary>
        /// <param name="player"></param>
        /// <param name="computer"></param>
        /// <param name="map"></param>
        static void DrawPlayer(Player player,Player computer,Map map)
        {
            if (player.getCurrentIndex() == computer.getCurrentIndex())
            {
                Grid grid = map.getGrid(player.getCurrentIndex());
                Console.SetCursorPosition(grid.getPos().getX(), grid.getPos().getY());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("◎");

            }
            else
            {
                player.Draw(map);
                computer.Draw(map);
            }
        }
        
        static void ClearCow(int windowWeight, int windowHeight)
        {
            for(int i = 2; i < windowWeight-2; ++i)
            {
                Console.Write(" ");
            }
        }
        static void ClearInfo(int windowWeight, int windowHeight)
        {
            Console.SetCursorPosition(2, windowHeight - 6);
            ClearCow(windowWeight, windowHeight);
            Console.SetCursorPosition(2, windowHeight - 5);
            ClearCow(windowWeight, windowHeight);
            Console.SetCursorPosition(2, windowHeight - 4);
            ClearCow(windowWeight, windowHeight);
            Console.SetCursorPosition(2, windowHeight - 3);
            ClearCow(windowWeight, windowHeight);
        }

        /// <summary>
        /// 扔骰子
        /// </summary>
        /// <param name="windowWeight">窗口的宽</param>
        /// <param name="windowHeight">窗口的高</param>
        /// <param name="p">扔骰子的对象</param>
        /// <param name="otherPlayer"></param>
        /// <param name="map">地图信息</param>
        /// <returns></returns>
        static bool RandomMove(int windowWeight,int windowHeight,ref Player player,ref Player otherPlayer,Map map)
        {
            
            //提示信息
            ClearInfo(windowWeight, windowHeight);
            Console.ForegroundColor = player.type == E_PlayerType.Player ? ConsoleColor.Blue : ConsoleColor.Magenta;

            if (player.getIsPause())
            {
                Console.SetCursorPosition(2, windowHeight - 6);
                Console.Write("处于暂停点，{0}需要暂停一回合", player.getType() == E_PlayerType.Player ? "你" : "电脑");

                player.setIsPause(false);
                return false;
            }

            Random random = new Random();
            int randomNum = random.Next(1, 7);
            player.setCurrentIndex(player.getCurrentIndex()+randomNum);

            Console.SetCursorPosition(2, windowHeight - 6);
            Console.Write("{0}扔出的点数为{1}", player.getType() == E_PlayerType.Player ? "你" : "电脑",randomNum);

            //是否到终点
            if (player.getCurrentIndex() >= map.getGrids().Length)
            {
                player.setCurrentIndex(map.getGrids().Length - 1);
                Console.SetCursorPosition(2, windowHeight - 5);
                if (player.getType() == E_PlayerType.Player)
                {
                    Console.Write("恭喜，你率先到达终点");
                }
                else
                {
                    Console.Write("很遗憾，电脑率先到达终点");
                }
                Console.SetCursorPosition(2, windowHeight - 4);
                Console.Write("请按任意键结束游戏");
                return true;
            }
            else
            {
                Grid grid = map.getGrid(player.getCurrentIndex());
                switch (grid.getType())
                {
                    case E_GridType.Normal:
                        Console.SetCursorPosition(2, windowHeight - 5);
                        Console.Write("{0}处于安全位置", player.getType() == E_PlayerType.Player ? "你" : "电脑");
                        Console.SetCursorPosition(2, windowHeight - 4);
                        Console.Write("请按任意键继续，{0}", player.getType() == E_PlayerType.Player ? "你" : "电脑");
                        break;
                    case E_GridType.Boom:
                        player.setCurrentIndex(player.getCurrentIndex() - 5);
                        if (player.getCurrentIndex() < 0)
                        {
                            player.setCurrentIndex(0);
                        }
                        Console.SetCursorPosition(2, windowHeight - 5);
                        Console.Write("{0}猜到了炸弹，退后5格", player.getType() == E_PlayerType.Player ? "你" : "电脑");
                        Console.SetCursorPosition(2, windowHeight - 4);
                        Console.Write("请按任意键继续");
                        break;
                    case E_GridType.Pause:
                        
                        player.setIsPause(true);
                        break;
                    case E_GridType.Tunnel:
                        Console.SetCursorPosition(2, windowHeight - 5);
                        Console.Write("{0}触发了时空隧道", player.getType() == E_PlayerType.Player ? "你" : "电脑");
                        randomNum = random.Next(1, 91);
                        if (randomNum <= 30)
                        {
                            player.setCurrentIndex(player.getCurrentIndex() - 5);
                            if (player.getCurrentIndex() < 0)
                            {
                                player.setCurrentIndex(0);
                            }
                            Console.SetCursorPosition(2, windowHeight - 4);
                            Console.Write("触发倒退5格");

                        }
                        else if (randomNum <= 60)
                        {
                            player.setIsPause(true);
                            Console.SetCursorPosition(2, windowHeight - 4);
                            Console.Write("触发暂停一回合");
                        }
                        else
                        {
                            int temp = player.getCurrentIndex();
                            player.setCurrentIndex(otherPlayer.getCurrentIndex());
                            otherPlayer.setCurrentIndex(temp);
                            Console.SetCursorPosition(2, windowHeight - 4);
                            Console.Write("交换位置");
                        }
                        Console.SetCursorPosition(2, windowHeight - 3);
                        Console.Write("请按任意键继续");

                        break;
                }
            }
            return false;
        }
        static bool PlayerRandomMove(int windowWeight, int windowHeight, ref Player player, ref Player otherPlayer, Map map,ref E_SceneType currentSceneType)
        {
            //玩家扔骰子
            //检测输入
            Console.ReadKey(true);
            //扔骰子逻辑
            bool isGameOver = RandomMove(windowWeight, windowHeight, ref player, ref otherPlayer, map);
            //绘制地图
            map.Draw();
            //绘制玩家
            DrawPlayer(player, otherPlayer, map);
            //判断是否结束游戏
            if (isGameOver)
            {
                Console.ReadKey(true);
                currentSceneType = E_SceneType.End;
            }
            return isGameOver;
        }

        static void GameScene(int weight, int height,ref E_SceneType currentSceneType)
        {
            #region 不变的红墙
            DrawRedWall(weight, height);
            #endregion
            Map map = new Map(16, 4, 100);
            map.Draw();

            Player player = new Player(0, E_PlayerType.Player);
            Player computer = new Player(0, E_PlayerType.Computer);
            DrawPlayer(player, computer, map);

            while (true)
            {
                //玩家扔骰子
                if(PlayerRandomMove(weight, height, ref player, ref computer, map, ref currentSceneType)){
                    break;
                }
                //电脑扔骰子
                if(PlayerRandomMove(weight, height, ref computer, ref player, map, ref currentSceneType)){
                    break;
                }
            }

        }
        #endregion

        static void Main(string[] args)
        {
            //窗口设置
            int windowsWidth = 60, windowsHeight = 40;
            ConsoleInit(windowsWidth, windowsHeight);
            Console.Clear();

            E_SceneType currentSceneType = E_SceneType.Begin;
            while (true)
            {

                switch (currentSceneType)
                {
                    //开始场景
                    case E_SceneType.Begin:
                        Console.Clear();
                        ChangeScene(windowsWidth, windowsHeight, ref currentSceneType);                       
                        break;
                    //游戏场景
                    case E_SceneType.Game:
                        Console.Clear();
                        GameScene(windowsWidth, windowsHeight,ref currentSceneType);
                        Console.ReadLine();
                        break;
                    //结束场景
                    case E_SceneType.End:
                        Console.Clear();
                        ChangeScene(windowsWidth, windowsHeight,ref currentSceneType);
                        break;
                }
            }

        }
    }
}
