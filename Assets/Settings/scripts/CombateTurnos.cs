using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Assets.Settings.scripts;
using System.Collections.Generic;

public class CombateTurnos : MonoBehaviour
{
    public ControlDados controlDados;
    public Text mensajeCombate;

    private List<Enemigo> listaEnemigos;
    private int indiceEnemigoActual = 0;

    private Enemigo enemigoActual;
    private Personaje personajeJugador;


    public int vidaJugador = 100; //datos hardcodeados
    public int vidaEnemigo = 100;

    private bool turnoJugador = true;
    private int lanzamientosRestantes = 3;

    public Button botonLanzarDados;
    public Button botonAtacar;

    public GameObject jugadorGO;
    public GameObject enemigoGO;

    private Animator animatorJugador;
    private Animator animatorEnemigo;

    public Slider barraVidaJugador;
    public Slider barraVidaEnemigo;

    private string prefijoAnimacionEnemigo;

    void Start()
    {
        InicializarEnemigos();
        InicializarPersonaje();

        enemigoActual = listaEnemigos[indiceEnemigoActual];

        vidaJugador = personajeJugador.vida_actual;
        vidaEnemigo = enemigoActual.vida;

        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        // Obtener prefijo de animación del enemigo
        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        if (info != null)
        {
            prefijoAnimacionEnemigo = info.tipoEnemigo;
        }
        else
        {
            prefijoAnimacionEnemigo = "card"; // Valor por defecto si no se encuentra
        }

        ReiniciarTurnoJugador();
    }

    void LanzarDados()
    {
        if (!turnoJugador || lanzamientosRestantes <= 0) return;

        controlDados.LanzarDados();
        lanzamientosRestantes--;

        mensajeCombate.text = "Lanzamientos restantes: " + lanzamientosRestantes;

        if (lanzamientosRestantes == 0)
        {
            botonLanzarDados.interactable = false;
        }
    }

    void Atacar()
    {
        if (!turnoJugador) return;

        ResultadoCombinacion resultado = controlDados.DetectarCombinacion();

        int daño = controlDados.CalcularDaño(resultado) + personajeJugador.daño_ataque - enemigoActual.defensa;
        daño = Mathf.Max(0, daño);
        vidaEnemigo -= daño;

        mensajeCombate.text = $"Combinación: {resultado.nombre}\nDaño: {daño}\nVida enemigo: {vidaEnemigo}";
        barraVidaEnemigo.value = vidaEnemigo;

        animatorJugador.SetTrigger("playerAttack");
        StartCoroutine(MoverJugadorDuranteAtaque());

        animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Damage");

        turnoJugador = false;
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;

        if (vidaEnemigo <= 0)
        {
            animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Death");
            mensajeCombate.text += "\n¡Ganaste!";

            // espera 2s
            Invoke(nameof(SiguienteEscena), 2f);
            return;
        }
    }

    IEnumerator MoverJugadorDuranteAtaque()
    {
        Vector3 posicionOriginal = jugadorGO.transform.position;
        Vector3 posicionAtaque = enemigoGO.transform.position + new Vector3(-2.0f, 0, 0);

        float duracion = 0.3f;
        float t = 0;

        while (t < duracion)
        {
            jugadorGO.transform.position = Vector3.Lerp(posicionOriginal, posicionAtaque, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        jugadorGO.transform.position = posicionAtaque;

        yield return new WaitForSeconds(0.2f);

        t = 0;
        while (t < duracion)
        {
            jugadorGO.transform.position = Vector3.Lerp(posicionAtaque, posicionOriginal, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        jugadorGO.transform.position = posicionOriginal;
    }

    void TurnoEnemigo()
    {
        int daño = enemigoActual.ataque - personajeJugador.defensa;
        daño = Mathf.Max(0, daño);
        vidaJugador -= daño;
        barraVidaJugador.value = vidaJugador;

        animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Attack");
        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        if (info != null && info.moverDuranteAtaque)
        {
            StartCoroutine(MoverEnemigoDuranteAtaque());
        }





        StartCoroutine(MostrarEfecto(jugadorGO, info, 0.5f));
        animatorJugador.SetTrigger("playerDamage");


        if (vidaJugador <= 0)
        {
            animatorJugador.SetTrigger("playerDeath");
            mensajeCombate.text = "¡Perdiste!";
            return;
        }

        ReiniciarTurnoJugador();
    }

    private IEnumerator MostrarEfecto(GameObject jugadorGO, EnemigoInfo info, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 offset = new Vector3(-0.1f, -0.15f, -0.1f);
        Vector3 posicion = jugadorGO.transform.position + offset;

        GameObject efecto = Instantiate(info.efectoAtaque, posicion, Quaternion.identity);
        Destroy(efecto, 1f);
    }

    IEnumerator MoverEnemigoDuranteAtaque()
    {
        Vector3 posicionOriginal = enemigoGO.transform.position;
        Vector3 posicionAtaque = jugadorGO.transform.position + new Vector3(2.0f, 0, 0);

        float duracion = 0.3f;
        float t = 0;

        while (t < duracion)
        {
            enemigoGO.transform.position = Vector3.Lerp(posicionOriginal, posicionAtaque, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        enemigoGO.transform.position = posicionAtaque;

        yield return new WaitForSeconds(0.2f);

        t = 0;
        while (t < duracion)
        {
            enemigoGO.transform.position = Vector3.Lerp(posicionAtaque, posicionOriginal, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        enemigoGO.transform.position = posicionOriginal;
    }

    void ReiniciarTurnoJugador()
    {
        turnoJugador = true;
        lanzamientosRestantes = 3;
        controlDados.ResetearDados();

        botonLanzarDados.interactable = true;
        botonAtacar.interactable = true;

        mensajeCombate.text = "Tu turno. Lanza los dados.";
    }

    void FinCombateJugadorGana()
    {
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;
    }

    //datos hardcodeados hasta linkear base de datos)

    void InicializarEnemigos()
    {
        listaEnemigos = new List<Enemigo>()
    {
        new Enemigo(1, "As 1", 60, 12, 3),
        new Enemigo(2, "As 2", 70, 14, 4),
        new Enemigo(3, "As 3", 80, 16, 5),
        new Enemigo(4, "As 4", 90, 18, 6),
        new Enemigo(5, "As 5", 100, 20, 7)
    };
    }

    void InicializarPersonaje()
    {
        personajeJugador = new Personaje(1, 100, 100, 15, 5);
    }

    void SiguienteEscena()
    {
        indiceEnemigoActual++;

        if (indiceEnemigoActual >= listaEnemigos.Count)
        {
            mensajeCombate.text = "¡Has ganado!";
            botonAtacar.interactable = false;
            botonLanzarDados.interactable = false;
            //ACA TERMINA EL JUEGO, PODRIA PASAR A UNA ESCENA DE VICTORIA
            return;
        }

        else
        {
            // SceneManager.LoadScene("Mejoras");
            // SceneManager.LoadScene("Mapa");

            enemigoActual = listaEnemigos[indiceEnemigoActual];
            vidaEnemigo = enemigoActual.vida;
            barraVidaEnemigo.maxValue = enemigoActual.vida;
            barraVidaEnemigo.value = vidaEnemigo;

            mensajeCombate.text = $"Enfrentas a: {enemigoActual.nombre}";
            ReiniciarTurnoJugador();
        }
    }
}