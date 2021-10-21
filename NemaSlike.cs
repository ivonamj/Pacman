using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pmfst_GameSDK
{
    class NemaSlike:Exception
    {
        private string poruka;

        public string Poruka
        {
            get
            {
                return poruka;
            }

            set
            {
                poruka = value;
            }
        }

        public NemaSlike(string p)
        {
            Poruka = p;
        }
    }
}
