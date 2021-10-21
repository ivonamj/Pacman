using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pmfst_GameSDK
{
    public class Pacman : Sprite
    {
        private int _bodovi, _zivot;

        public int Bodovi
        {
            get
            {
                return _bodovi;
            }

            set
            {
                _bodovi = value;
            }
        }

        public int Zivot
        {
            get
            {
                return _zivot;
            }

            set
            {
                _zivot = value;
            }
        }

        public Pacman(string path, int x, int y) : base(path, x, y)
        {
            this.Height = 20;
            this.Width = 20;
            this.Zivot = 3;
        }

        public void PacmanMoveDown(Sprite p, List<Sprite> listap)
        {

            p.Y += 7;

            for (int i = 0; i < listap.Count; i++)
            {
                if (p.TouchingSprite(listap[i]))
                    p.GotoXY(p.X, listap[i].Y - p.Height - 5);
            }

            if (p.Y + p.Height >= GameOptions.DownEdge)
                p.Y = GameOptions.DownEdge - p.Height - 5;

        }

        public void PacmanMoveUp(Sprite p,List<Sprite> listap)
        {
            p.Y -= 7;

            for (int i = 0; i < listap.Count; i++)
            {
                if (p.TouchingSprite(listap[i]))
                    p.GotoXY(p.X, listap[i].Y + listap[i].Height + 5);
            }

            if (p.Y <= GameOptions.UpEdge)
                p.Y = GameOptions.UpEdge + 5;

        }

        public void PacmanMoveRight(Sprite p,List<Sprite> listap)
        {
            p.X += 7;

            for (int i = 0; i < listap.Count; i++)
            {
                if (p.TouchingSprite(listap[i]))
                    p.GotoXY(listap[i].X - p.Width - 5, p.Y);
            }


            if (p.X + p.Width >= GameOptions.RightEdge)
                p.X = GameOptions.RightEdge - p.Width - 5;
        }

        public void PacmanMoveLeft(Sprite p,List<Sprite> listap)
        {

            p.X -= 7;
            for (int i = 0; i < listap.Count; i++)
            {
                if (p.TouchingSprite(listap[i]))
                    p.GotoXY(listap[i].X + listap[i].Width + 5, p.Y);
            }


            if (p.X <= GameOptions.LeftEdge)
                p.X = GameOptions.LeftEdge + 5;
        }
    }

}

