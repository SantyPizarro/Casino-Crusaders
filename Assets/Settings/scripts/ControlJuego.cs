using UnityEngine;
using System.Collections.Generic;
using Assets.Settings.scripts;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance { get; private set; }

    public int indiceEnemigoActual = 0;
    public Personaje personajeJugador;
    public List<Enemigo> listaEnemigos;
    private List<string> flujoEscenas = new List<string>()
    {
        "Combate1", //pelea-evento-pelea-tienda-pelea-evento-pelea(jefe) //acá iria el mapa de por medio
        "Evento1",
        "Combate2",

    };

    private int indiceEscenaActual = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Inicializar()
    {
        personajeJugador = new Personaje(1, 100, 100, 15, 5);

        listaEnemigos = new List<Enemigo>()
        {
            new Enemigo(1, "As 1", 60, 12, 3),
            new Enemigo(2, "As 2", 70, 14, 4),
            new Enemigo(3, "As 3", 80, 16, 5),
            new Enemigo(4, "As 4", 90, 18, 6),
            new Enemigo(5, "As 5", 100, 20, 7)
        };
    }

    public void AvanzarASiguienteEscena()
    {
        indiceEscenaActual++;
        if (indiceEscenaActual >= flujoEscenas.Count)
        {
            Debug.Log("no hay más escenas");
            return;
        }

        string siguienteEscena = flujoEscenas[indiceEscenaActual];
        Debug.Log("Cargando escena: " + siguienteEscena);
        UnityEngine.SceneManagement.SceneManager.LoadScene(siguienteEscena);
    }

    public Enemigo ObtenerEnemigoActual()
    {
        int indiceEnemigo = indiceEscenaActual / 2;
        if (indiceEnemigo < listaEnemigos.Count)
            return listaEnemigos[indiceEnemigo];
        else
            return null;
    }

    public void ReiniciarJuego()
    {
        indiceEscenaActual = 0;
        Inicializar();
        UnityEngine.SceneManagement.SceneManager.LoadScene(flujoEscenas[0]);
    }
}