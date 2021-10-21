using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pmfst_GameSDK
{
    public class Neprijatelj:Sprite
    {
        private int _brzina;

        public int Brzina
        {
            get
            {
                return _brzina;
            }

            set
            {
                _brzina = value;
            }
        }

        public Neprijatelj(string path,int x,int y):base(path,x, y)
        {
            this.Width = 20;
            this.Height = 20;
            this.Brzina = 10;
        }

        public void NeprijateljMove(Neprijatelj neprijatelj,List<Sprite> lista)
        {
            if (neprijatelj.GetDirection() == 180)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if (neprijatelj.TouchingSprite(lista[i]))
                        neprijatelj.GotoXY(neprijatelj.X, lista[i].Y - neprijatelj.Height - 5);
                }
                if (neprijatelj.Y + neprijatelj.Height >= GameOptions.DownEdge)
                    neprijatelj.Y = GameOptions.DownEdge - neprijatelj.Height - 5;
            }

            if (neprijatelj.GetDirection() == 0)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if (neprijatelj.TouchingSprite(lista[i]))
                        neprijatelj.GotoXY(neprijatelj.X, lista[i].Y + lista[i].Height + 5);
                }
                if (neprijatelj.Y <= GameOptions.UpEdge)
                    neprijatelj.Y = GameOptions.UpEdge + 5;
            }

            if (neprijatelj.GetDirection() == 90)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if (neprijatelj.TouchingSprite(lista[i]))
                        neprijatelj.GotoXY(lista[i].X - neprijatelj.Width - 5, neprijatelj.Y);
                }
                if (neprijatelj.X + neprijatelj.Width >= GameOptions.RightEdge)
                    neprijatelj.X = GameOptions.RightEdge - neprijatelj.Width - 5;
            }

            if (neprijatelj.GetDirection() == 270)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if (neprijatelj.TouchingSprite(lista[i]))
                        neprijatelj.GotoXY(lista[i].X + lista[i].Width + 5, neprijatelj.Y);
                }
                if (neprijatelj.X <= GameOptions.LeftEdge)
                    neprijatelj.X = GameOptions.LeftEdge + 5;
            }
        }
    }
}
