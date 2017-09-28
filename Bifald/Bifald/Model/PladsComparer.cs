using Bifald.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bifald.Model
{
    class PladsComparer : IEqualityComparer<Plads_historik>
    {
        public bool Equals(Plads_historik x, Plads_historik y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Pladsnummer == y.Pladsnummer && x.Pladsnummer == y.Pladsnummer;
        }
        public int GetHashCode(Plads_historik pladser)
        {
            if (Object.ReferenceEquals(pladser, null)) return 0;
            int hashProductName = pladser.Pladsnummer == null ? 0 : pladser.Pladsnummer.GetHashCode();
            int hashProductCode = pladser.Pladsnummer.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }
}
