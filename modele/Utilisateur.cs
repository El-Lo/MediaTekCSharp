using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.modele.utilisateur
{
    public static class Utilisateur
    {
        public static Role? Service { get; set; }
    }

    public enum Role { admin, pres, culture }
}
