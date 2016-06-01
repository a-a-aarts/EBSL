using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSL
{
    struct Evidence
    {
        public int positive { get; private set; }
        public int negative { get; private set; }

        public Evidence(int p, int n)
        {
            positive = p;
            negative = n;
        }

        public static Evidence operator +(Evidence e, int i)
        {
            if (i > 0)
            {
                return new Evidence(e.positive + i, e.negative);
            }
            if(i < 0)
            {
                return new Evidence(e.positive, e.negative - i);
            }
            return e;
        }

        public Opinion ToOpinion(int constant)
        {
            return new Opinion(positive, negative, constant);
        }
    }
}
