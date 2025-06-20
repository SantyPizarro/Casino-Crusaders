using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Settings.scripts
{
    public class Personaje
    {
        public int id_personaje;
        public int vida_maxima;
        public int vida_actual;
        public int daño_ataque;
        public int defensa;

        public Personaje(int id_personaje, int vida_maxima, int vida_actual, int daño_ataque, int defensa)
        {
            this.id_personaje = id_personaje;
            this.vida_maxima = vida_maxima;
            this.vida_actual = vida_actual;
            this.daño_ataque = daño_ataque;
            this.defensa = defensa;
        }
    }
}
