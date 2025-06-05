using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Settings.scripts;

public class CombateTurnos : MonoBehaviour
{
    public ControlDados controlDados;
    public Text mensajeCombate;
    public Text nombreEnemigoTexto;

    private Enemigo enemigoActual;
    private Personaje personajeJugador;

    public int vidaJugador = 100;
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
        var controlJuego = ControlJuego.Instance;

        if (controlJuego.listaEnemigos == null || controlJuego.listaEnemigos.Count == 0)
        {
            controlJuego.Inicializar();
        }

        enemigoActual = controlJuego.ObtenerEnemigoActual();
        personajeJugador = controlJuego.personajeJugador;

        vidaJugador = personajeJugador.vida_actual;
        vidaEnemigo = enemigoActual.vida;

        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        // Inicializar barras de vida
        barraVidaJugador.maxValue = personajeJugador.vida_maxima;
        barraVidaJugador.value = vidaJugador;

        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        nombreEnemigoTexto.text = enemigoActual.nombre;

        // Obtener prefijo de animación del enemigo
        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        prefijoAnimacionEnemigo = info != null ? info.tipoEnemigo : "card";

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
            Invoke(nameof(SiguienteEscena), 2f);
            return;
        }

        Invoke(nameof(TurnoEnemigo), 2f);
    }

    IEnumerator MoverJugadorDuranteAtaque()
    {
        Vector3 original = jugadorGO.transform.position;
        Vector3 ataque = enemigoGO.transform.position + new Vector3(-2.0f, 0, 0);

        yield return MoverDeA_A(jugadorGO, original, ataque, 0.3f);
        yield return new WaitForSeconds(0.2f);
        yield return MoverDeA_A(jugadorGO, ataque, original, 0.3f);
    }

    IEnumerator MoverEnemigoDuranteAtaque()
    {
        Vector3 original = enemigoGO.transform.position;
        Vector3 ataque = jugadorGO.transform.position + new Vector3(2.0f, 0, 0);

        yield return MoverDeA_A(enemigoGO, original, ataque, 0.3f);
        yield return new WaitForSeconds(0.2f);
        yield return MoverDeA_A(enemigoGO, ataque, original, 0.3f);
    }

    IEnumerator MoverDeA_A(GameObject obj, Vector3 desde, Vector3 hasta, float duracion)
    {
        float t = 0;
        while (t < duracion)
        {
            obj.transform.position = Vector3.Lerp(desde, hasta, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = hasta;
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

    IEnumerator MostrarEfecto(GameObject objetivo, EnemigoInfo info, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 offset = new Vector3(-0.1f, -0.15f, -0.1f);
        Vector3 pos = objetivo.transform.position + offset;

        GameObject efecto = Instantiate(info.efectoAtaque, pos, Quaternion.identity);
        Destroy(efecto, 1f);
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

    void SiguienteEscena()
    {
        var controlJuego = ControlJuego.Instance;
        controlJuego.AvanzarAlSiguienteEnemigo();

        if (controlJuego.JuegoCompletado())
        {
            mensajeCombate.text = "¡Has ganado!";
            botonAtacar.interactable = false;
            botonLanzarDados.interactable = false;

            controlJuego.ReiniciarJuego();
            return;
        }

        enemigoActual = controlJuego.ObtenerEnemigoActual();
        vidaEnemigo = enemigoActual.vida;
        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        nombreEnemigoTexto.text = enemigoActual.nombre;
        mensajeCombate.text = $"Enfrentas a: {enemigoActual.nombre}";

        ReiniciarTurnoJugador();
    }
}
