using UnityEngine;
using System.Collections.Generic;
using Assets.Settings.scripts;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance { get; private set; }

    public int indiceEnemigoActual = 0;
    public Personaje personajeJugador;
    public List<Enemigo> listaEnemigos;

    void Awake()
    {
        // singleton
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
        personajeJugador = new Personaje();
        personajeJugador.idPersonaje = 1; // Asignar un ID al personaje
        personajeJugador.vidaMaxima = 100;
        personajeJugador.vidaActual = 100;
        personajeJugador.dañoAtaque = 20;
        personajeJugador.defensa = 5;

        listaEnemigos = new List<Enemigo>()
        {
            new Enemigo(1, "As 1", 60, 12, 3),
            new Enemigo(2, "As 2", 70, 14, 4),
            new Enemigo(3, "As 3", 80, 16, 5),
            new Enemigo(4, "As 4", 90, 18, 6),
            new Enemigo(5, "As 5", 100, 20, 7)
        };
    }

    public Enemigo ObtenerEnemigoActual()
    {
        if (indiceEnemigoActual < listaEnemigos.Count)
            return listaEnemigos[indiceEnemigoActual];

        return null;
    }

    public void AvanzarAlSiguienteEnemigo()
    {
        indiceEnemigoActual++;
    }

    public bool JuegoCompletado()
    {
        return indiceEnemigoActual >= listaEnemigos.Count;
    }

    public void ReiniciarJuego()
    {
        indiceEnemigoActual = 0;
        Inicializar();
    }
}