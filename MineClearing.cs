using System;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace MineClearing
{
    class Program
    {
        static void Main(string[] args)
        {
            Layout();
            string[,] map;
            int[,] interMap;
            int row = 0;
            int column = 0;
            bool isMine = false;
            int clearNum = 0;
            Initialize(out map, out interMap);
            while (true)
            {
                bool legalIn;
                do
                {
                    legalIn = Input(ref row, ref column, ref isMine);
                } while (!legalIn);
                clearNum = Mark(row, column, isMine, map, interMap, ref clearNum);

                // 胜利结束
                if (clearNum == -1)
                    return;
            }
        }
        /*
         判断输入符是否正确：
        第一个：数字，第二个：数字，第三个：'t' or 'f'
        数字的判断：必须是数字；在0~9之间
        字符的判断：f或t
         */
        private static bool Input(ref int row, ref int column, ref bool isMine)
        {
            bool norm = true;
            Console.WriteLine("请输入坐标：");
            string pos = Console.ReadLine();
            
            if(pos.Length != 3)
            {
                Console.Write("请按标准输入！");
                return false;
            }

            try
            {
                // 问题：char转int，会按ascii值转
                // 解决：先将char转为string，再转为int
                row = int.Parse(pos[0].ToString());
                column = int.Parse(pos[2].ToString());
            }
            catch
            {
                Console.WriteLine("输入必须是数字坐标");
                norm = false;
            }
            if (!norm || row < 0 || row > 9 || column < 0 || column > 9)
            {
                Console.WriteLine("输入坐标不合法");
                return false;
            }

            string jude = "";
            Console.WriteLine("请输入判断结果：");
            jude = Console.ReadLine();

            if (jude == "t")
                isMine = true;
            else if (jude == "f")
                isMine = false;
            else
            {
                Console.WriteLine("第三个参数只能是\"f\"或\"t\"");
                return false;
            }
            
            return true;
        
        }

        static void Layout()
        {
            Console.WriteLine("----------------扫雷--------------\n\n");
            Console.WriteLine("游戏玩法：");
            Console.WriteLine("输入x的坐标，回车后再输入y的坐标\n0表示判断该处无雷，1表示判断该处有雷");
            Console.WriteLine("坐标输入标准：a,b 或 a b\n\n\n");
            Console.WriteLine("----------------------------------\n\n");
        }

        static void Initialize(out string[,] map, out int[,] interMap)
        {
            //两个数组，一个显示地图，一个标明雷
            int arrayRow = 10;
            int arrayColumn = 10;
            map = new string[arrayRow, arrayColumn];
            interMap = new int[arrayRow, arrayColumn];
            //界面数据结构存储的数据：【o】【1】【2】【3】【4】【5】【6】【7】【8】【?】
            for (int row = 0; row < arrayRow; row++)
            {
                for (int column = 0; column < arrayColumn; column++)
                {
                    map[row, column] = "o";
                    Console.Write(map[row, column] + "\t");
                }
                Console.WriteLine();
            }
            Console.Write("\n\n\n");
            //inter数组的赋值:0表示无雷，9表示有雷，其他表示附近的雷数
            //默认各元素为0
            //先随机出雷的位置
            //再修改10个雷附近的元素值
            int mineNum = 10;
            int[,] temp = new int[arrayRow, 2];
            for (int num = 0; num < mineNum; )
            {
                int row = RandPos()[0];
                int column = RandPos()[1];
                if (interMap[row, column] < 9)
                {
                    UpdateNum(interMap, row, column);
                    temp[num, 0] = row;
                    temp[num, 1] = column;
                    interMap[row, column] = 9;
                    num++;
                }
            }
            //解决更新值的附加问题
            for (int i = 0; i < arrayRow; i++)
                interMap[temp[i, 0], temp[i, 1]] = 9;

        }
        static void UpdateNum(int[,] interMap, int row, int column)
        {
            int rowLen = interMap.GetLength(0) - 1;
            int columnLen = interMap.GetLength(1) - 1;
            if (row == 0)
            {
                if (column == 0)
                {
                    interMap[row, column + 1]++;
                    interMap[row + 1, column]++;
                    interMap[row + 1, column + 1]++;
                }
                else if (column == columnLen)
                {
                    interMap[row, column - 1]++;
                    interMap[row + 1, column - 1]++;
                    interMap[row + 1, column]++;
                }
                else
                {
                    interMap[row, column + 1]++;
                    interMap[row, column - 1]++;
                    interMap[row + 1, column - 1]++;
                    interMap[row + 1, column]++;
                    interMap[row + 1, column + 1]++;
                }
            }
            else if (row == rowLen)
            {
                if (column == 0)
                {
                    interMap[row, column + 1]++;
                    interMap[row - 1, column]++;
                    interMap[row - 1, column + 1]++;
                }
                else if (column == columnLen)
                {
                    interMap[row, column - 1]++;
                    interMap[row - 1, column - 1]++;
                    interMap[row - 1, column]++;
                }
                else
                {
                    interMap[row, column + 1]++;
                    interMap[row, column - 1]++;
                    interMap[row - 1, column - 1]++;
                    interMap[row - 1, column]++;
                    interMap[row - 1, column + 1]++;
                }
            }
            else
            {
                if (column == 0 && row != 0 && row != rowLen)
                {
                    interMap[row - 1, column]++;
                    interMap[row - 1, column + 1]++;
                    interMap[row, column + 1]++;
                    interMap[row + 1, column]++;
                    interMap[row + 1, column + 1]++;
                }
                else if (column == 9 && row != 0 && row != rowLen)
                {
                    interMap[row - 1, column]++;
                    interMap[row - 1, column - 1]++;
                    interMap[row, column - 1]++;
                    interMap[row + 1, column]++;
                    interMap[row + 1, column - 1]++;
                }
                //一般情况
                else
                {
                    interMap[row - 1, column]++;
                    interMap[row - 1, column + 1]++;
                    interMap[row - 1, column - 1]++;
                    interMap[row, column - 1]++;
                    interMap[row, column + 1]++;
                    interMap[row + 1, column]++;
                    interMap[row + 1, column - 1]++;
                    interMap[row + 1, column + 1]++;
                }
            }
        }
        static int[] RandPos()
        {
            int[] arr = new int[2];
            arr[0] = Math.Abs(Guid.NewGuid().GetHashCode() % 10);
            arr[1] = Math.Abs(Guid.NewGuid().GetHashCode() % 10);
            return arr;
        }

        static int Mark(int row, int column, bool isMine, string[,] map, int[,] interMap, ref int clearNum)
        {
            // 胜利条件
            if (clearNum == map.GetLength(0)*(map.GetLength(1) - 1))
            {
                Victory(true);
                return -1;
            }

            if (isMine)
                map[row, column] = "?";
            else
            {
                if (interMap[row, column] == 9)
                {
                    Victory(false);
                    return -1;
                }
                else
                {
                    if (interMap[row, column] == 0)
                        ClearZero(row, column, map, interMap);

                    else
                        map[row, column] = interMap[row, column].ToString();
                    
                }
                clearNum++;
            }
            RefreshPage(map, interMap);
            return clearNum;
            
        }

        static void ClearZero(int posRow, int posColumn, string[,] map, int[,] interMap)
        {
            //（消除同类项）算是学习算法的动力，学到了补上
            map[posRow, posColumn] = " ";
        }
        static void RefreshPage(string[,] map, int[,] interMap)
        {
            Console.Clear();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n\n");

            // 用于调试
            for (int i = 0; i < interMap.GetLength(0); i++)
            {
                for (int j = 0; j < interMap.GetLength(1); j++)
                {
                    Console.Write(interMap[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        static void Victory(bool result)
        {
            Console.Clear();
            Console.WriteLine("游戏结束");
            if (result)
                Console.WriteLine("你赢了\nbiu~biu~biu~");
            else
                Console.WriteLine("很遗憾，再接再厉");
        }
    }
}
