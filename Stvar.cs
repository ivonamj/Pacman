using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pmfst_GameSDK
{
    public abstract class Stvar:Sprite
    {
        private int _vrijednost;
        

        public int Vrijednost
        {
            get
            {
                return _vrijednost;
            }

            set
            {
                _vrijednost = value;
            }
        }
        public Stvar(string path, int x, int y) : base(path, x, y)
        {

        }
    }

    public class Tocka:Stvar
    {
        public Tocka(string path,int x,int y):base(path,x, y)
        {
            this.Vrijednost = 50;
            this.Width = 14;
            this.Height = 14;
        }
    }

    public class Tockica : Stvar
    {
        public Tockica(string path, int x, int y) : base(path, x, y)
        {
            this.Vrijednost = 10;
            this.Width = 8;
            this.Height = 8;
        }
    }

    public class Pregrada : Stvar
    {
        public Pregrada(string path, int x, int y,int height,int width) : base(path, x, y)
        {
            this.Width = width;

            this.Height = height;
        }
    }

}
