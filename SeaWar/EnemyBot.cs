using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace SeaWar
{
    public class EnemyBot
    {
        

        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize]; // It's Bot Map !!
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize]; // It's My Map !!
        
        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        public EnemyBot(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.enemyButtons = enemyButtons;
            this.myButtons = myButtons;
        }
         public static bool IsInsideMap(int i, int j) => (i< 1 || j<1 || i >= Form1.mapSize || j >= Form1.mapSize) ? false : true;


        public bool IsEmpty(int x, int y, int length, bool horizont)
        {
            bool IsEmpty = true;

            if (horizont)
            {
                for (int k = y - 1; k < y + length + 1; k++)
                {
                    if (IsInsideMap(x, k))
                    {
                        if (myMap[x, k] != 0)
                        {
                            IsEmpty = false;
                            break;
                        }
                    }
                }
            }
            else {
                for (int k = x - 1; k < x + length + 1; k++)
                {
                    if (IsInsideMap(k, y))
                    {
                        if (myMap[k, y] != 0)
                        {
                            IsEmpty = false;
                            break;
                        }
                    }
                }
            }
            return IsEmpty;
        }

        public bool IsEmptyButton(int i, int j) {
            bool IsEmpty = false;
            if (IsInsideMap( i, j)) {
                if (myMap[i, j] == 0)
                {
                    return true;
                } 
            }   
            return IsEmpty;
        }

        public int[,] ConfigureShip()
        {
            bool horizont = true;  
            int lengthShip = 4;
            int cycleValue = 4;
            int countShips = 10;
            Random rnd = new Random();
          

            int posX = 0;
            int posY = 0;

            

            while (countShips > 0) 
            {
                for (int i = 0; i < cycleValue / 4; i++)
                {
                    posX = rnd.Next(1, Form1.mapSize);
                    posY = rnd.Next(1, Form1.mapSize);      //|| myMap[posX, posY] != 0

                    bool horizontShip = (!IsInsideMap(posX, posY + lengthShip - 1) || !IsEmpty(posX, posY, lengthShip, horizont));
                    bool verticalShip = (!IsInsideMap(posX + +lengthShip - 1, posY) || !IsEmpty(posX, posY, lengthShip, !horizont));

                    while (verticalShip && horizontShip)
                    {
                        posX = rnd.Next(1, Form1.mapSize);
                        posY = rnd.Next(1, Form1.mapSize);

                        horizontShip = (!IsInsideMap(posX, posY + lengthShip - 1) || !IsEmpty(posX, posY, lengthShip, horizont));
                        verticalShip = (!IsInsideMap(posX  + lengthShip - 1, posY) || !IsEmpty(posX, posY, lengthShip, !horizont));
                    }

                    if (!horizontShip)
                    {
                        for (int k = posY; k < posY + lengthShip; k++)
                        {
                            myMap[posX, k] = 1;
                            myButtons[posX, k].Text = "";
                            myButtons[posX, k].BackColor = Color.LightYellow;
                        }
                        myButtons[posX, posY].Text = "";
                        // -- раставляем границы корабля
                        for (int x = posX - 1; x < posX + 2; x++)
                        { // Прозодим по координатам Х (выше корабля ниже и сам кораболь)
                            for (int y = posY - 1; y < posY + lengthShip + 1; y++)// Проходим по координатам У (выше корабля ниже и сам кораболь
                            {
                                if (IsEmptyButton(x, y))
                                {
                                    myButtons[x, y].Text = "";
                                    myMap[x, y] = -1;
                                }
                            }
                        }
                    }
                    else// horizontShip
                    {
                        for (int k = posX; k < posX + lengthShip ; k++)
                        {
                            myMap[k, posY] = 1;
                            myButtons[k, posY].Text = "";
                            myButtons[k, posY].BackColor = Color.LightYellow;
                        }
                        myButtons[posX, posY].Text = "";
                        // -- ПЫТАЕМСЯ раставить границы корабля
                        for (int y = posY - 1; y < posY + 2; y++)
                        { // Прозодим по координатам Х (выше корабля ниже и сам кораболь)
                            for (int x = posX - 1; x < posX + lengthShip + 1; x++)// Проходим по координатам У (выше корабля ниже и сам кораболь)
                            {
                                if (IsEmptyButton(x, y))
                                {
                                    myButtons[x, y].Text = "";
                                    myMap[x, y] = -1;
                                }
                            }
                        }
                    }
                    countShips--;
                    if (countShips <= 0) break;                 
                }
                lengthShip--;
                cycleValue += 4;
            }
            return myMap;
        }

        public bool CheckMap() {
            bool IsEmpty1 = true;
            bool IsEmpty2 = true;
            for (int i = 0; i < Form1.mapSize; i++)
            {
                for (int j = 0; j < Form1.mapSize; j++)
                {
                    if (myMap[i, j] != 0) IsEmpty1 = false;
                    if (enemyMap[i, j] != 0) IsEmpty2 = false;
                }
            }
            return (IsEmpty1 || IsEmpty2) ? false : true;
        }

        public bool Shoot()
        {

            bool hit = false;
            Random rnd = new Random();

            int posX = rnd.Next(1, Form1.mapSize);
            int posY = rnd.Next(1, Form1.mapSize);

            while (enemyButtons[posX, posY].BackColor == Color.DarkOrchid || enemyButtons[posX, posY].BackColor == Color.Gray)
            {
                 posX = rnd.Next(1, Form1.mapSize);
                 posY = rnd.Next(1, Form1.mapSize);
            }
            if (enemyMap[posX, posY] != 0)
            {
                hit = true;
                enemyMap[posX, posY] = 0;
                enemyButtons[posX, posY].BackColor = Color.DarkOrchid;
                enemyButtons[posX, posY].Text = "X";
            }
            else 
            {
                enemyButtons[posX, posY].BackColor = Color.Gray;
                enemyButtons[posX, posY].Text = "";
            }

            return hit;
        }

    }
}
