using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Pmfst_GameSDK
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                //MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(260, 240, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */

        // deklaracija objekata
        #region Likovi
        /* Initialization */
        List<Sprite> lista = new List<Sprite>();
        List<Sprite> listaTocke = new List<Sprite>();
        List<Sprite> listaTockice = new List<Sprite>();
        List<Sprite> listaNeprijatelja = new List<Sprite>();
        Pacman pacman;
        Neprijatelj neprijatelj, neprijatelj2, neprijatelj3, neprijatelj4;
        Stvar pregrada, pregrada1, pregrada2, pregrada3, pregrada4, pregrada5, pregrada6,
            pregrada7, pregrada8, pregrada9, pregrada10, pregrada11, pregrada12, pregrada13, 
            pregrada14, pregrada15, pregrada16, pregrada17, pregrada18, pregrada19, pregrada20,
            pregrada21, pregrada22, pregrada23, pregrada24, pregrada25, pregrada26, pregrada27, 
            pregrada28, pregrada29, pregrada30,pregrada31, pregrada32, pregrada33, pregrada34, 
            pregrada35, pregrada36, pregrada37, pregrada38, pregrada39, pregrada40, pregrada41, 
            pregrada42, pregrada43, pregrada44,pregrada45,pregrada46, pregrada47;
        Tocka tocka, tocka2, tocka3, tocka4;
        Tockica tockica, tockica2, tockica3, tockica4, 
            tockica5, tockica6, tockica7, tockica8, tockica9, tockica10;
        Sprite zamjena, zamjena1, zamjena2, zamjena3;
        #endregion

        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("PMF");
            setBackgroundColor(Color.Black);            
            //setBackgroundPicture("backgrounds\\back.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            #region Pacman
            pacman = new Pacman("sprites\\glavni.png",300,5);
            Game.AddSprite(pacman);
            #endregion

            #region Zamjene
            zamjena = new Sprite(0, 0);
            zamjena.Width = 20;
            zamjena.Height = 20;
            zamjena.SetVisible(false);
            Game.AddSprite(zamjena);
            zamjena1 = new Sprite(0, 0);
            zamjena1.Width = 20;
            zamjena1.Height = 20;
            zamjena1.SetVisible(false);
            Game.AddSprite(zamjena1);
            zamjena2 = new Sprite(0, 0);
            zamjena2.Width = 20;
            zamjena2.Height = 20;
            zamjena2.SetVisible(false);
            Game.AddSprite(zamjena2);
            zamjena3 = new Sprite(0, 0);
            zamjena3.Width = 20;
            zamjena3.Height = 20;
            zamjena3.SetVisible(false);
            Game.AddSprite(zamjena3);
            #endregion

            #region Neprijatelji
            neprijatelj = new Neprijatelj("sprites\\neprijatelj.jpg", 5,5);
            Game.AddSprite(neprijatelj);
           

            neprijatelj2 = new Neprijatelj("sprites\\neprijatelj.jpg", 675,5);
            Game.AddSprite(neprijatelj2);
         

            neprijatelj3 = new Neprijatelj("sprites\\neprijatelj.jpg", 5,475);
            Game.AddSprite(neprijatelj3);

            neprijatelj4 = new Neprijatelj("sprites\\neprijatelj.jpg", 675,475);
            Game.AddSprite(neprijatelj4);

            listaNeprijatelja.Add(neprijatelj);
            listaNeprijatelja.Add(neprijatelj2);
            listaNeprijatelja.Add(neprijatelj3);
            listaNeprijatelja.Add(neprijatelj4);
            #endregion

            #region Tocke
            tocka = new Tocka("sprites\\tocka.png", 158, 158);
            Game.AddSprite(tocka);

            tocka2 = new Tocka("sprites\\tocka.png", 458, 38);
            Game.AddSprite(tocka2);

            tocka3 = new Tocka("sprites\\tocka.png", 618, 238);
            Game.AddSprite(tocka3);

            tocka4 = new Tocka("sprites\\tocka.png", 428, 448);
            Game.AddSprite(tocka4);
           

            listaTocke.Add(tocka);
            listaTocke.Add(tocka2);
            listaTocke.Add(tocka3);
            listaTocke.Add(tocka4);
            #endregion

            #region Tockice
            tockica = new Tockica("sprites\\tocka.png", 41, 11);
            Game.AddSprite(tockica);

            tockica2 = new Tockica("sprites\\tocka.png", 191, 451);
            Game.AddSprite(tockica2);

            tockica3 = new Tockica("sprites\\tocka.png", 441, 181);
            Game.AddSprite(tockica3);

            tockica4 = new Tockica("sprites\\tocka.png", 311, 101);
            Game.AddSprite(tockica4);

            tockica5 = new Tockica("sprites\\tocka.png", 681, 361);
            Game.AddSprite(tockica5);

            tockica6 = new Tockica("sprites\\tocka.png", 281, 311);
            Game.AddSprite(tockica6);

            tockica7 = new Tockica("sprites\\tocka.png", 71, 331);
            Game.AddSprite(tockica7);

            tockica8 = new Tockica("sprites\\tocka.png", 11, 191);
            Game.AddSprite(tockica8);

            tockica9 = new Tockica("sprites\\tocka.png", 681, 71);
            Game.AddSprite(tockica9);

            tockica10 = new Tockica("sprites\\tocka.png", 501, 391);
            Game.AddSprite(tockica10);


            listaTockice.Add(tockica);
            listaTockice.Add(tockica2);
            listaTockice.Add(tockica3);
            listaTockice.Add(tockica4);
            listaTockice.Add(tockica5);
            listaTockice.Add(tockica6);
            listaTockice.Add(tockica7);
            listaTockice.Add(tockica8);
            listaTockice.Add(tockica9);
            listaTockice.Add(tockica10);
            #endregion

            #region Pregrade
            pregrada = new Pregrada("sprites\\pregrada.png", 30,30,60,60);
            Game.AddSprite(pregrada);

            pregrada1 = new Pregrada("sprites\\pregrada.png", 120, 30, 60, 30);
            Game.AddSprite(pregrada1);

            pregrada2 = new Pregrada("sprites\\pregrada.png", 180, 60, 30, 60);
            Game.AddSprite(pregrada2);

            pregrada3 = new Pregrada("sprites\\pregrada.png", 240, 30, 60, 30);
            Game.AddSprite(pregrada3);

            pregrada4 = new Pregrada("sprites\\pregrada.png", 300, 30, 60, 30);
            Game.AddSprite(pregrada4);

            pregrada5 = new Pregrada("sprites\\pregrada.png", 330, 60, 30, 60);
            Game.AddSprite(pregrada5);

            pregrada6 = new Pregrada("sprites\\pregrada.png", 420, 30, 60, 30);
            Game.AddSprite(pregrada6);

            pregrada7 = new Pregrada("sprites\\pregrada.png", 450, 60, 30, 60);
            Game.AddSprite(pregrada7);

            pregrada8 = new Pregrada("sprites\\pregrada.png", 540, 30, 60, 130);
            Game.AddSprite(pregrada8);

            pregrada9 = new Pregrada("sprites\\pregrada.png", 180, 0, 30, 30);
            Game.AddSprite(pregrada9);

            pregrada10 = new Pregrada("sprites\\pregrada.png", 360, 0, 30, 30);
            Game.AddSprite(pregrada10);

            pregrada11 = new Pregrada("sprites\\pregrada.png", 480, 0, 30, 30);
            Game.AddSprite(pregrada11);

            pregrada12 = new Pregrada("sprites\\pregrada.png", 30, 120, 85, 60);
            Game.AddSprite(pregrada12);

            pregrada13 = new Pregrada("sprites\\pregrada.png", 120, 120, 30, 90);
            Game.AddSprite(pregrada13);

            pregrada14 = new Pregrada("sprites\\pregrada.png", 180, 150, 55, 30);
            Game.AddSprite(pregrada14);

            pregrada15 = new Pregrada("sprites\\pregrada.png", 240, 120, 50, 250);
            Game.AddSprite(pregrada15);

            pregrada16 = new Pregrada("sprites\\pregrada.png", 520, 120, 50, 30);
            Game.AddSprite(pregrada16);

            pregrada17 = new Pregrada("sprites\\pregrada.png", 580, 120, 30, 90);
            Game.AddSprite(pregrada17);

            pregrada18 = new Pregrada("sprites\\pregrada.png", 640, 150, 20, 30);
            Game.AddSprite(pregrada18);

            pregrada19 = new Pregrada("sprites\\pregrada.png", 0, 235, 30, 30);
            Game.AddSprite(pregrada19);

            pregrada20 = new Pregrada("sprites\\pregrada.png", 240, 330, 50, 250);
            Game.AddSprite(pregrada20);

            //glavna pregrada
            pregrada21 = new Pregrada("sprites\\pregrada.png", 240, 200, 100, 220);
            Game.AddSprite(pregrada21);

            pregrada22 = new Pregrada("sprites\\pregrada.png", 60, 235, 30, 60);
            Game.AddSprite(pregrada22);

            pregrada23 = new Pregrada("sprites\\pregrada.png", 30, 295, 85, 30);
            Game.AddSprite(pregrada23);

            pregrada24 = new Pregrada("sprites\\pregrada.png", 30, 410, 60, 30);
            Game.AddSprite(pregrada24);

            pregrada25 = new Pregrada("sprites\\pregrada.png", 120, 180, 55, 30);
            Game.AddSprite(pregrada25);

            pregrada26 = new Pregrada("sprites\\pregrada.png", 90,235,85 , 60);
            Game.AddSprite(pregrada26);

            pregrada27 = new Pregrada("sprites\\pregrada.png", 150, 235, 30, 60);
            Game.AddSprite(pregrada27);

            pregrada28 = new Pregrada("sprites\\pregrada.png", 90, 350, 30, 120);
            Game.AddSprite(pregrada28);

            pregrada29 = new Pregrada("sprites\\pregrada.png", 180, 295,55, 30);
            Game.AddSprite(pregrada29);

            pregrada30 = new Pregrada("sprites\\pregrada.png", 90, 410, 60, 30);
            Game.AddSprite(pregrada30);

            pregrada31 = new Pregrada("sprites\\pregrada.png", 490, 200, 100, 30);
            Game.AddSprite(pregrada31);

            pregrada32 = new Pregrada("sprites\\pregrada.png", 580, 180,120, 30);
            Game.AddSprite(pregrada32);

            pregrada33 = new Pregrada("sprites\\pregrada.png", 550, 200, 30, 120);
            Game.AddSprite(pregrada33);

            pregrada34 = new Pregrada("sprites\\pregrada.png", 550, 230, 70,30);
            Game.AddSprite(pregrada34);

            pregrada35 = new Pregrada("sprites\\pregrada.png", 520, 330, 50, 30);
            Game.AddSprite(pregrada35);

            pregrada36= new Pregrada("sprites\\pregrada.png",580, 330, 50,90);
            Game.AddSprite(pregrada36);

            pregrada37 = new Pregrada("sprites\\pregrada.png", 640, 260, 100, 30);
            Game.AddSprite(pregrada37);

            pregrada38 = new Pregrada("sprites\\pregrada.png", 640, 410, 60, 30);
            Game.AddSprite(pregrada38);

            pregrada39 = new Pregrada("sprites\\pregrada.png", 150, 410, 30, 210);
            Game.AddSprite(pregrada39);

            pregrada40= new Pregrada("sprites\\pregrada.png", 390, 410, 30, 100);
            Game.AddSprite(pregrada40);

            pregrada41 = new Pregrada("sprites\\pregrada.png", 520, 410, 30,90);
            Game.AddSprite(pregrada41);

            pregrada42 = new Pregrada("sprites\\pregrada.png", 150, 470, 30, 30);
            Game.AddSprite(pregrada42);

            pregrada43 = new Pregrada("sprites\\pregrada.png", 210, 440, 30, 150);
            Game.AddSprite(pregrada43);

            pregrada44 = new Pregrada("sprites\\pregrada.png", 390, 440, 30, 30);
            Game.AddSprite(pregrada44);

            pregrada45 = new Pregrada("sprites\\pregrada.png", 450, 470, 30, 40);
            Game.AddSprite(pregrada45);

            pregrada46 = new Pregrada("sprites\\pregrada.png", 520, 470, 30, 30);
            Game.AddSprite(pregrada46);

            pregrada47 = new Pregrada("sprites\\pregrada.png", 580, 440, 30, 30);
            Game.AddSprite(pregrada47);



           

            lista.Add(pregrada);
            lista.Add(pregrada1);
            lista.Add(pregrada2);
            lista.Add(pregrada3);
            lista.Add(pregrada4);
            lista.Add(pregrada5);
            lista.Add(pregrada6);
            lista.Add(pregrada7);
            lista.Add(pregrada8);
            lista.Add(pregrada9);
            lista.Add(pregrada10);
            lista.Add(pregrada11);
            lista.Add(pregrada12);
            lista.Add(pregrada13);
            lista.Add(pregrada14);
            lista.Add(pregrada15);
            lista.Add(pregrada16);
            lista.Add(pregrada17);
            lista.Add(pregrada18);
            lista.Add(pregrada19);
            lista.Add(pregrada20);
            lista.Add(pregrada21);
            lista.Add(pregrada22);
            lista.Add(pregrada23);
            lista.Add(pregrada24);
            lista.Add(pregrada25);
            lista.Add(pregrada26);
            lista.Add(pregrada27);
            lista.Add(pregrada28);
            lista.Add(pregrada29);
            lista.Add(pregrada30);
            lista.Add(pregrada31);
            lista.Add(pregrada32);
            lista.Add(pregrada33);
            lista.Add(pregrada34);
            lista.Add(pregrada35);
            lista.Add(pregrada36);
            lista.Add(pregrada37);
            lista.Add(pregrada38);
            lista.Add(pregrada39);
            lista.Add(pregrada40);
            lista.Add(pregrada41);
            lista.Add(pregrada42);
            lista.Add(pregrada43);
            lista.Add(pregrada44);
            lista.Add(pregrada45);
            lista.Add(pregrada46);
            lista.Add(pregrada47);
            #endregion

            //dodati event handlers ovdje
            //napomena: prije metoda u kojima se pozivaju
            testEvent += BGL_testEvent;
            //ovaj primjer pozivamo: testEvent.Invoke(1234);
            //tada se poziva izvršavanje metode BGL_testEvent(1234)
            _gameOver += GameOver;
            _lostLife += OduzmiZivot;
            _pobjeda += Pobjeda;
            
            
            //3. scripts that start
            //Game.StartScript(Metoda);
            Game.StartScript(GlavniMove);

        }

        #region Delegati

        //možemo slati i parametre kod poziva događaja

        public delegate void testHandlerDelegat(int broj);
        public event testHandlerDelegat testEvent;

        public delegate void EventHandler(int broj);
        public event EventHandler _gameOver;

        public delegate void EventHandlerZivot();
        public event EventHandlerZivot _lostLife;

        public delegate void EventHandlerPobjeda(int broj);
        public event EventHandlerPobjeda _pobjeda;

        private void BGL_testEvent(int broj)
        {
            MessageBox.Show("Poslali ste broj: " + broj);
        }

        /* Event handlers - metode*/
        private void OduzmiZivot()
        {
            pacman.Zivot -= 1;
            ISPIS = "Bodovi " + pacman.Bodovi + " Zivoti: " + pacman.Zivot;
            _gameOver.Invoke(pacman.Zivot);
        }


        private void GameOver(int broj)
        {
            if(broj==0)
            {
                ISPIS = "Game Over!";
                Wait(0.2);
                START = false;
            }
        }

        private void Pobjeda(int broj)
        {
            if(broj==300)
            {
                ISPIS = "Pobjeda";
                Wait(0.2);
                START = false;
            }
        }
        #endregion

  
        /* Scripts */

        private int Metoda()
        {
            while(START) //ili neki drugi uvjet
            {
                
                Wait(0.1);
            }
            return 0;
        }

        bool prvi = true;
        bool zastava = false;
        int br = 0;
        private int EatEnemy()
        {
            for (int i = 0; i <= 9; i++)
            {
                Wait(1);
            }
            for (int i = 0; i < listaNeprijatelja.Count; i++)
            {
                listaNeprijatelja[i].SetVisible(true);
            }
            neprijatelj.X = 5;
            neprijatelj.Y = 5;
            neprijatelj2.X = 675;
            neprijatelj2.Y = 5;
            neprijatelj3.X = 5;
            neprijatelj3.Y = 475;
            neprijatelj4.X = 675;
            neprijatelj4.Y = 475;
            zastava = false;
            return 0;
        }

        private int GlavniMove()
        {
            while(START)
            {
                _pobjeda.Invoke(pacman.Bodovi);


                if (sensing.KeyPressed(Keys.Down))
                {
                    pacman.PacmanMoveDown(pacman, lista);
                
                    if (prvi)
                    {
                        Game.StartScript(NeprijateljMove);
                        prvi = false;
                    }
                }

                else if (sensing.KeyPressed(Keys.Up))
                {
                    pacman.PacmanMoveUp(pacman, lista);

                    if (prvi)
                    {
                        Game.StartScript(NeprijateljMove);
                        prvi = false;
                    }
                }

                else if (sensing.KeyPressed(Keys.Right))
                {
                    pacman.PacmanMoveRight(pacman, lista);

                    if (prvi)
                    {
                        Game.StartScript(NeprijateljMove);
                        prvi = false;
                    }
                }

                else if (sensing.KeyPressed(Keys.Left))
                {
                    pacman.PacmanMoveLeft(pacman, lista);

                    if (prvi)
                    {
                        Game.StartScript(NeprijateljMove);
                        prvi = false;
                    }
                }

              
                for (int i = 0; i < listaTocke.Count; i++)
                {
                    if (pacman.TouchingSprite(listaTocke[i]))
                    {
                        zastava = true;
                        
                        pacman.Bodovi += tocka.Vrijednost;
                        listaTocke[i].SetVisible(false);
                        listaTocke.Remove(listaTocke[i]);
                        ISPIS = "Bodovi " + pacman.Bodovi;

                        Game.StartScript(EatEnemy);

                    }
                }

                for (int i = 0; i < listaTockice.Count; i++)
                {
                    if (pacman.TouchingSprite(listaTockice[i]))
                    {
                        pacman.Bodovi += tockica.Vrijednost;
                        listaTockice[i].SetVisible(false);
                        listaTockice.Remove(listaTockice[i]);
                        ISPIS = "Bodovi " + pacman.Bodovi;
                    }
                   
                }

               for(int i=0;i<listaNeprijatelja.Count;i++)
                {
                   if(zastava==false)
                   {
                        if (pacman.TouchingSprite(listaNeprijatelja[i]))
                        {
                            pacman.X = 300;
                            pacman.Y = 5;
                            neprijatelj.X = 5;
                            neprijatelj.Y = 5;
                            neprijatelj2.X = 675;
                            neprijatelj2.Y = 5;
                            neprijatelj3.X = 5;
                            neprijatelj3.Y = 475;
                            neprijatelj4.X = 675;
                            neprijatelj4.Y = 475;
                            ISPIS = "Zivot " + pacman.Zivot;
                            _lostLife.Invoke();

                        }
                    }
                   else
                    {
                        if (pacman.TouchingSprite(listaNeprijatelja[i]))
                        {
                            listaNeprijatelja[i].SetVisible(false);
                        }
                    }
                }
            }
            return 0;
        }
        
        private int NeprijateljMove()
        {
            while (START)
            {
                neprijatelj.PointToSprite(pacman);
                neprijatelj2.PointToSprite(pacman);
                neprijatelj3.PointToSprite(pacman);
                neprijatelj4.PointToSprite(pacman);

                
                neprijatelj.SetDirection(neprijatelj.ClosestDirection());
                neprijatelj2.SetDirection(neprijatelj2.ClosestDirection());
                neprijatelj3.SetDirection(neprijatelj3.ClosestDirection());
                neprijatelj4.SetDirection(neprijatelj4.ClosestDirection());
                
                for (int i = 0; i < lista.Count; i++)
                {
                    br = 0;
                    if (neprijatelj.GetDirection() == 90)
                    {
                        zamjena.X = neprijatelj.X + 25;
                        zamjena.Y = neprijatelj.Y;
                        if (zamjena.TouchingSprite(lista[i]))
                        {
                            neprijatelj.X = lista[i].X - 25;
                            br = br + 90;
                            neprijatelj.SetDirection(neprijatelj.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj.GetDirection() == 270)
                    {
                        zamjena.X = neprijatelj.X - 5;
                        zamjena.Y = neprijatelj.Y;
                        if (zamjena.TouchingSprite(lista[i]))
                        {
                            neprijatelj.X = lista[i].X + 5;
                            br = br + 90;
                            neprijatelj.SetDirection(neprijatelj.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj.GetDirection() == 180)
                    {
                        zamjena.X = neprijatelj.X;
                        zamjena.Y = neprijatelj.Y + 25;
                        if (zamjena.TouchingSprite(lista[i]))
                        {
                            neprijatelj.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj.SetDirection(neprijatelj.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj.GetDirection() == 0)
                    {
                        zamjena.X = neprijatelj.X;
                        zamjena.Y = neprijatelj.Y - 5;
                        if (zamjena.TouchingSprite(lista[i]))
                        {
                            neprijatelj.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj.SetDirection(neprijatelj.GetDirection() + br);
                        }
                    }
                }

                for (int i = 0; i < lista.Count; i++)
                {
                    br = 0;
                    if (neprijatelj2.GetDirection() == 90)
                    {
                        zamjena1.X = neprijatelj2.X + 25;
                        zamjena1.Y = neprijatelj2.Y;
                        if (zamjena1.TouchingSprite(lista[i]))
                        {
                            neprijatelj2.X = lista[i].X - 25;
                            br = br + 90;
                            neprijatelj2.SetDirection(neprijatelj2.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj2.GetDirection() == 270)
                    {
                        zamjena1.X = neprijatelj2.X - 5;
                        zamjena1.Y = neprijatelj2.Y;
                        if (zamjena1.TouchingSprite(lista[i]))
                        {
                            neprijatelj2.X = lista[i].X + 5;
                            br = br + 90;
                            neprijatelj2.SetDirection(neprijatelj2.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj2.GetDirection() == 180)
                    {
                        zamjena1.X = neprijatelj2.X;
                        zamjena1.Y = neprijatelj2.Y + 25;
                        if (zamjena1.TouchingSprite(lista[i]))
                        {
                            neprijatelj2.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj2.SetDirection(neprijatelj2.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj2.GetDirection() == 0)
                    {
                        zamjena1.X = neprijatelj2.X;
                        zamjena1.Y = neprijatelj2.Y - 5;
                        if (zamjena1.TouchingSprite(lista[i]))
                        {
                            neprijatelj2.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj2.SetDirection(neprijatelj2.GetDirection() + br);
                        }
                    }
                }

                for (int i = 0; i < lista.Count; i++)
                {
                    br = 0;
                    if (neprijatelj3.GetDirection() == 90)
                    {
                        zamjena2.X = neprijatelj3.X + 25;
                        zamjena2.Y = neprijatelj3.Y;
                        if (zamjena2.TouchingSprite(lista[i]))
                        {
                            neprijatelj3.X = lista[i].X - 25;
                            br = br + 90;
                            neprijatelj3.SetDirection(neprijatelj3.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj3.GetDirection() == 270)
                    {
                        zamjena2.X = neprijatelj3.X - 5;
                        zamjena2.Y = neprijatelj3.Y;
                        if (zamjena2.TouchingSprite(lista[i]))
                        {
                            neprijatelj3.X = lista[i].X + 5;
                            br = br + 90;
                            neprijatelj3.SetDirection(neprijatelj3.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj3.GetDirection() == 180)
                    {
                        zamjena2.X = neprijatelj3.X;
                        zamjena2.Y = neprijatelj3.Y + 25;
                        if (zamjena2.TouchingSprite(lista[i]))
                        {
                            neprijatelj3.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj3.SetDirection(neprijatelj3.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj3.GetDirection() == 0)
                    {
                        zamjena2.X = neprijatelj3.X;
                        zamjena2.Y = neprijatelj3.Y - 5;
                        if (zamjena2.TouchingSprite(lista[i]))
                        {
                            neprijatelj3.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj3.SetDirection(neprijatelj3.GetDirection() + br);
                        }
                    }
                }

                for (int i = 0; i < lista.Count; i++)
                {
                    br = 0;
                    if (neprijatelj4.GetDirection() == 90)
                    {
                        zamjena3.X = neprijatelj4.X + 25;
                        zamjena3.Y = neprijatelj4.Y;
                        if (zamjena3.TouchingSprite(lista[i]))
                        {
                            neprijatelj4.X = lista[i].X - 25;
                            br = br + 90;
                            neprijatelj4.SetDirection(neprijatelj4.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj4.GetDirection() == 270)
                    {
                        zamjena3.X = neprijatelj4.X - 5;
                        zamjena3.Y = neprijatelj4.Y;
                        if (zamjena3.TouchingSprite(lista[i]))
                        {
                            neprijatelj4.X = lista[i].X + 5;
                            br = br + 90;
                            neprijatelj4.SetDirection(neprijatelj4.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj4.GetDirection() == 180)
                    {
                        zamjena3.X = neprijatelj4.X;
                        zamjena3.Y = neprijatelj4.Y + 25;
                        if (zamjena3.TouchingSprite(lista[i]))
                        {
                            neprijatelj4.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj4.SetDirection(neprijatelj4.GetDirection() + br);
                        }
                    }

                    else if (neprijatelj4.GetDirection() == 0)
                    {
                        zamjena3.X = neprijatelj4.X;
                        zamjena3.Y = neprijatelj4.Y - 5;
                        if (zamjena3.TouchingSprite(lista[i]))
                        {
                            neprijatelj4.Y = lista[i].Y - 25;
                            br = br + 90;
                            neprijatelj4.SetDirection(neprijatelj4.GetDirection() + br);
                        }
                    }
                }

                neprijatelj.MoveSteps(neprijatelj.Brzina);
                neprijatelj2.MoveSteps(neprijatelj2.Brzina);
                neprijatelj3.MoveSteps(neprijatelj3.Brzina);
                neprijatelj4.MoveSteps(neprijatelj4.Brzina);


                neprijatelj.NeprijateljMove(neprijatelj, lista);
                neprijatelj2.NeprijateljMove(neprijatelj2, lista);
                neprijatelj3.NeprijateljMove(neprijatelj3, lista);
                neprijatelj4.NeprijateljMove(neprijatelj4, lista);

                Wait(0.5);
            }
            return 0;
        }

   
        /* ------------ GAME CODE END ------------ */
    }
}
