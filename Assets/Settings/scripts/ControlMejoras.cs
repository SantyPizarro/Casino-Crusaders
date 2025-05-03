using System.Collections.Generic;
using UnityEngine;

public class ControlMejoras : MonoBehaviour
{
    public List<Mejora> todasLasMejoras;

    private void Awake()
    {
        todasLasMejoras = new List<Mejora>();

        // EJEMPLO DE MEJORA
        todasLasMejoras.Add(new Mejora
        {
            //ATRIBUTOS DE LA UI
            nombre = "AGUANTE EL PAR LOCO",
            descripcion = "+2 MULT PAR.",
            icono = Resources.Load<Sprite>("mejoras/par"), //PATH DE LA IMAGEN
            aplicarMejora = () =>
            {
                if (!ModificadoresDaño.multiplicadoresExtra.ContainsKey("par"))

                ModificadoresDaño.multiplicadoresExtra["par"] += 2;
            }
        });

        // ACA IRIA OTRA

        todasLasMejoras.Add(new Mejora
        {
            nombre = "A FULLL",
            descripcion = "Los 'full' hacen +4 de multiplicador.",
            icono = Resources.Load<Sprite>("mejoras/full"),
            aplicarMejora = () =>
            {
                if (!ModificadoresDaño.multiplicadoresExtra.ContainsKey("full"))

                ModificadoresDaño.multiplicadoresExtra["full"] += 4;
            }
        });

        todasLasMejoras.Add(new Mejora
        {
            nombre = "PEPAS TRIO",
            descripcion = "Los trio hacen +3 de multiplicador.",
            icono = Resources.Load<Sprite>("mejoras/trio"),
            aplicarMejora = () =>
            {
                if (!ModificadoresDaño.multiplicadoresExtra.ContainsKey("trio"))

                    ModificadoresDaño.multiplicadoresExtra["trio"] += 3;
            }
        });
    }

    public List<Mejora> ElegirMejorasAleatorias(int cantidad)
    {
        List<Mejora> copia = new List<Mejora>(todasLasMejoras);
        List<Mejora> seleccionadas = new List<Mejora>();

        for (int i = 0; i < cantidad && copia.Count > 0; i++)
        {
            int indice = Random.Range(0, copia.Count);
            seleccionadas.Add(copia[indice]);
            copia.RemoveAt(indice); // evitar repetidas
        }

        return seleccionadas;
    }
}
