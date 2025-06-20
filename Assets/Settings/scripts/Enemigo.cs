using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Settings.scripts
{
    public class Enemigo
    {
        public int id_enemigo;
        public string nombre;
        public int vida;
        public int ataque;
        public int defensa;

        public Enemigo(int id_enemigo, String nombre, int vida, int ataque, int defensa)
        {
            this.id_enemigo = id_enemigo;
            this.nombre = nombre;
            this.vida = vida;
            this.ataque = ataque;
            this.defensa = defensa;
        }
    }
}
