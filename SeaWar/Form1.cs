using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Net;

namespace SeaWar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();          
            this.Text = "sea battle";
            
        }

        public const int mapSize = 10;
        public int ceilSize = 30;
        public string alphabet = "АБВГДЕЖЗИК";
        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];
        public bool isPlaying = false;
        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];
        public EnemyBot bot;
        

        public string txt = "";

        public void Init() {
            CreateMap();
            isPlaying = false;
            bot = new EnemyBot(enemyMap, myMap, enemyButtons, myButtons);
            enemyMap = bot.ConfigureShip();                 
        }

        public void CreateMap()
        {
            this.Width = mapSize * 2 * ceilSize + 100;
            this.Height = mapSize * ceilSize + 150;
            // MY MAP
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;
                    Button button = new Button();
                    button.Location = new Point(j * ceilSize, i * ceilSize);
                    button.Size = new Size(ceilSize, ceilSize);
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.AliceBlue;
                        if (j == 0 & i > 0) button.Text = i.ToString();
                        if (i == 0 & j > 0) button.Text = alphabet[j - 1].ToString();
                    }
                    else 
                    {
                        button.BackColor = Color.AntiqueWhite;
                        button.Text = "";
                        button.Click += new EventHandler(ConfigureShips);
                    }
                    myButtons[i, j] = button;
                    this.Controls.Add(button);   
                }
            }

            //ENEMY MAP
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    
                    enemyMap[i, j] = 0;
                   
                    Button button = new Button();
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.AliceBlue;
                        if (j == 0 & i > 0) button.Text = i.ToString();
                        else if (i == 0 & j > 0) button.Text = alphabet[j - 1].ToString();
                       
                    }
                    else 
                    {
                        button.BackColor = Color.LightYellow;
                        button.Text = "";
                        button.Click += new EventHandler(PlayerShoot);
                    }
                    button.Location = new Point( 350 + j * ceilSize , i * ceilSize);
                    button.Size = new Size(ceilSize, ceilSize);
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
                Label map1 = new Label();
                map1.Text = "Player map";
                map1.Location = new Point(mapSize * ceilSize / 2 - 20,mapSize * ceilSize + 20);
                this.Controls.Add(map1);

                Label map2 = new Label();
                map2.Text = "Enemy map";
                map2.Location = new Point(mapSize * ceilSize / 2 + 320, mapSize * ceilSize + 20);
                this.Controls.Add(map2);

                Button StartButton = new Button();
                StartButton.Click += new EventHandler(Start);
                StartButton.Text = "Let's go!";
                StartButton.Location = new Point(285, mapSize * ceilSize + 20);
                this.Controls.Add(StartButton);
            }
        }

        public void Start(object sender, EventArgs E)
        {          
            Button pressedButton = sender as Button;


            if (CheckShips(myMap, out this.txt) && !isPlaying)  
            {
                isPlaying = true;
                pressedButton.BackColor = Color.YellowGreen;
                pressedButton.Text = "Играем!";
            }

            else
            {
                isPlaying = false;
                pressedButton.BackColor = Color.Red;
                pressedButton.Text = ":-(";
            }
        }


            public bool CheckMap()
        {
            bool IsEmpty1 = true;
            bool IsEmpty2 = true;
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] == 1) IsEmpty1 = false;
                    if (enemyMap[i, j]  == 1) IsEmpty2 = false;
                }
            }
            return (IsEmpty1 || IsEmpty2) ? false : true;
        }

        public int CountDeck(int x, int y, bool horizontShip) 
        {
            int count = 0;
            if (horizontShip)
            {             
                while (myMap[x, y] == 1)
                {
                        for (int i = -1; i <= 1; i++)
                        {   
                                if ((EnemyBot.IsInsideMap(x + 1, y + i) && myMap[x + 1, y + i] == 1) ||
                                    (EnemyBot.IsInsideMap(x - 1, y + i) && myMap[x - 1, y + i] == 1))
                                {
                                    return -1;
                                } 
                        }
                    count++;
                        y++;
                    if (EnemyBot.IsInsideMap(x, y)) continue; else break;
                }             
            }
            else 
            {

                while (myMap[x, y] == 1)
                {
    
                        for (int i = -1; i <= 1; i++)
                        {
                            if (i != 0)
                            {
                                if ((EnemyBot.IsInsideMap(x + i, y + 1) && myMap[x + i, y + 1] == 1) ||
                                    (EnemyBot.IsInsideMap(x + i, y - 1) && myMap[x + i, y - 1] == 1))                             
                                     return -2;
                            } 
                        }
                    count++;
                    x++;
                    if (EnemyBot.IsInsideMap(x, y)) continue; else break;    
                }
            }
            return count;
        }

        public bool CheckShips(int[,] myMap,out string txt)
        {
            txt = "";
            List<int> result = new List<int>();
            bool horizontShip = true;
            int[] RightArr_ShipsCount = {1, 1, 1, 1, 2, 2, 2, 3, 3, 4};
            for (int x = 1; x < mapSize; x++)
            {
                for (int y = 1; y < mapSize; y++) 
                {
                    if (myMap[x, y] == 1) 
                    {
                        if ((!EnemyBot.IsInsideMap(x - 1, y) || myMap[x - 1, y] != 1) && // vertical ship
                            (!EnemyBot.IsInsideMap(x, y + 1) || myMap[x, y + 1] != 1) &&
                            (!EnemyBot.IsInsideMap(x, y - 1) || myMap[x, y - 1] != 1))
                        {
                            if (CountDeck(x, y, !horizontShip) < 0) return false;
                            else result.Add(CountDeck(x, y, !horizontShip)); 
                        }
                        else if ((!EnemyBot.IsInsideMap(x - 1, y) || myMap[x - 1, y] != 1) && // horizont ship
                                 (!EnemyBot.IsInsideMap(x + 1, y) || myMap[x + 1, y] != 1) &&
                                 (!EnemyBot.IsInsideMap(x, y - 1) || myMap[x, y - 1] != 1))
                        {
                            if (CountDeck(x, y, horizontShip) < 0) return false;
                            else result.Add(CountDeck(x, y, horizontShip));
                        }
                    }
                }
            }
            result.Sort();         
            return (result.ToArray().Length == 10) ? Enumerable.SequenceEqual(RightArr_ShipsCount, result) : false ;
        }

        public void ConfigureShips(object sender, EventArgs E)
        {
            Button pressedButton = sender as Button;
            if (!isPlaying)
            {
                if (myMap[pressedButton.Location.Y / ceilSize, pressedButton.Location.X / ceilSize] == 0)
                {
                    pressedButton.BackColor = Color.DarkSeaGreen;
                    myMap[pressedButton.Location.Y / ceilSize, pressedButton.Location.X / ceilSize] = 1;
                    pressedButton.Text = "-";
                }
                else 
                {
                    pressedButton.BackColor = Color.AntiqueWhite;
                    myMap[pressedButton.Location.Y / ceilSize, pressedButton.Location.X / ceilSize] = 0;
                    pressedButton.Text = "";
                }
            }
        }


        public void PlayerShoot(object sender,EventArgs E)
        {
            Button pressButton = sender as Button;
            int playerTurn = Shoot(enemyMap, pressButton);
            if (playerTurn == 1 & isPlaying) { CheckWreckedShip(enemyMap, enemyButtons); bot.Shoot(); }
            if (playerTurn == 0 & isPlaying)  bot.Shoot(); 
            if (!CheckMap() & isPlaying) {
                this.Controls.Clear();
                Init();
            }
        }


        public int Shoot(int[,] map, Button pressButton) {
            int hit = 2;
            if (isPlaying)
            {
                    int delta = 0;
                    if (pressButton.Location.X > 350)
                        delta = 350;
               
                    if (map[pressButton.Location.Y / ceilSize, (pressButton.Location.X - delta) / ceilSize] == 1)
                    {
                        hit = 1;
                        map[pressButton.Location.Y / ceilSize, (pressButton.Location.X - delta) / ceilSize] = 3;// Попал в корабль
                        pressButton.BackColor = Color.DarkOrange;
                        pressButton.Text = "";
                        
                    }
             
                    else if ((map[pressButton.Location.Y / ceilSize, (pressButton.Location.X - delta) / ceilSize] != 2) &&
                              map[pressButton.Location.Y / ceilSize, (pressButton.Location.X - delta) / ceilSize] != 3)
                     {
                        hit = 0;
                        map[pressButton.Location.Y / ceilSize, (pressButton.Location.X - delta) / ceilSize] = 2; // Стрельнул мимо
                        pressButton.BackColor = Color.White;
                        pressButton.Text = "0";
                     }
            }
            
            return hit;
        }

        public void CheckWreckedShip(int[,] map, Button[,] button)
        {
            if (isPlaying)
            {

               
                for (int x = 1; x < mapSize; x++)
                {
                    for (int y = 1; y < mapSize; y++)
                    {
                        if (map[x, y] == 3)
                        {
                           // button[x, y].BackColor = Color.Red;
                            if ((!EnemyBot.IsInsideMap(x - 1, y) || map[x - 1, y] != 3) && // vertical ship
                                (!EnemyBot.IsInsideMap(x, y + 1) || map[x, y + 1] != 3) &&
                                (!EnemyBot.IsInsideMap(x, y - 1) || map[x, y - 1] != 3) &&
                                (!EnemyBot.IsInsideMap(x, y + 1) || map[x, y + 1] != 1) &&
                                (!EnemyBot.IsInsideMap(x - 1, y) || map[x - 1, y] != 1) &&
                                (!EnemyBot.IsInsideMap(x , y - 1) || map[x , y - 1] != 1))
                            {
                                int t = x;
                                while  (EnemyBot.IsInsideMap(t, y)  &&  map[t, y] == 3)
                                {
                                    t++;
                                    if (EnemyBot.IsInsideMap(t, y) && map[t, y] == 1) break;
                                    else if (EnemyBot.IsInsideMap(t, y) && map[t, y] == 3 && EnemyBot.IsInsideMap(t + 1, y)) continue;
                                    else
                                    {
                                       // if (!EnemyBot.IsInsideMap(t, y)) t--;
                                        for (int i = x; i < t  ; i++)
                                        {
                                            button[i, y].BackColor = Color.DarkRed;
                                            button[i, y].Text = "X";
                                        }
                                    }
                                }
                            }
                            else if ((!EnemyBot.IsInsideMap(x - 1, y) || map[x - 1, y] != 3) && // horizont ship
                                     (!EnemyBot.IsInsideMap(x + 1, y) || map[x + 1, y] != 3) &&
                                     (!EnemyBot.IsInsideMap(x, y - 1) || map[x, y - 1] != 3) &&
                                     (!EnemyBot.IsInsideMap(x - 1, y) || map[x - 1, y] != 1) &&
                                     (!EnemyBot.IsInsideMap(x , y - 1) || map[x , y - 1] != 1))
                                    {                                      
                                        int t = y;
                                        while (EnemyBot.IsInsideMap(x, t) && map[x, t] == 3 )
                                        {
                                            t++;
                                            if (EnemyBot.IsInsideMap(x, t) && map[x, t] == 1) break;
                                            else if (EnemyBot.IsInsideMap(x, t) && map[x, t] == 3 && EnemyBot.IsInsideMap(x, t + 1)) continue;
                                            else
                                            {
                                                for (int i = y; i < t  ; i++)
                                                {
                                                    button[x, i].BackColor = Color.DarkRed;
                                                    button[x, i].Text = "X";
                                                }
                                            }
                                        }
                                    }
                        }
                    }
                }
            }
        }

        // ============================
    }
}
