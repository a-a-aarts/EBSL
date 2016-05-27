using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSL
{
    struct Opinion
    {
        public static Func<Opinion, float> g { get; set; }

        public static Opinion B { get; private set; } = new Opinion() { b = 1, d = 0, u = 0 };
        public static Opinion D { get; private set; } = new Opinion() { b = 0, d = 1, u = 0 };
        public static Opinion U { get; private set; } = new Opinion() { b = 0, d = 0, u = 1 };

        public float b { get; private set; }
        public float d { get; private set; }
        public float u { get; private set; }

        public static Opinion from_bdu(float b, float d, float u)
        {
            return new Opinion()
            {
                b = b,
                d = d,
                u = u
            };
        }

        public Opinion(float support, float opposition, float uncertainty)
        {
            //TODO: figure out uncertainty constant
            float div = support + opposition + uncertainty;
            b = support / div;
            d = opposition / div;
            u = uncertainty / div;
        }

        public static Opinion Consensus(Opinion x, Opinion y)
        {
            float div = x.u + y.u - x.u * y.u;
            return new Opinion()
            {
                b = (x.u * y.b + y.u * x.b) / div,
                d = (x.u * y.d + y.u * x.d) / div,
                u = (x.u * y.u) / div
            };
        }

        public static Opinion Scalar(float a, Opinion x)
        {
            if (a == 0)
            {
                return Opinion.U;
            }
            float div = a * (x.b + x.d) + x.u;
            return new Opinion()
            {
                b = (x.b * a) / div,
                d = (x.d * a) / div,
                u = x.u / div
            };
        }

        public static Opinion Discount(Opinion x, Opinion y)
        {
            return Scalar(g(x), y);
        }


        #region comparison stuff
        public override int GetHashCode()
        {
            return b.GetHashCode() ^ d.GetHashCode() ^ u.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                Opinion o = (Opinion)obj;
                return o.b == this.b && o.d == this.d && o.u == this.u;
            }
            return false;
        }

        public static bool operator ==(Opinion a, Opinion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Opinion a, Opinion b)
        {
            return !a.Equals(b);
        }
        #endregion
    }
}
