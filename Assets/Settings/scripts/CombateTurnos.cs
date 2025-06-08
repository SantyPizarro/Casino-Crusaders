using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CombateTurnos : MonoBehaviour
{
    public ControlDados controlDados;
    public Text mensajeCombate;

    public int vidaJugador = 100;
    public int vidaEnemigo = 100;

    private bool turnoJugador = true;
    private int lanzamientosRestantes = 3;
    private bool yaLanzasteUnaVez = false;

    public Button botonLanzarDados;
    public Button botonAtacar;

    public GameObject jugadorGO;
    public GameObject enemigoGO;

    private Animator animatorJugador;
    private Animator animatorEnemigo;

    public Slider barraVidaJugador;
    public Slider barraVidaEnemigo;

    private string prefijoAnimacionEnemigo;

    public Camera camaraCombate;

    void Start()
    {
        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        barraVidaJugador.maxValue = vidaJugador;
        barraVidaJugador.value = vidaJugador;

        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        prefijoAnimacionEnemigo = info != null ? info.tipoEnemigo : "card";

        ReiniciarTurnoJugador();
    }

    void LanzarDados()
    {
        if (!turnoJugador || lanzamientosRestantes <= 0) return;

        controlDados.LanzarDados();
        lanzamientosRestantes--;

        if (!yaLanzasteUnaVez)
        {
            yaLanzasteUnaVez = true;
        }

        StartCoroutine(MostrarTextoAnimado("Lanzamientos restantes: " + lanzamientosRestantes));

        if (lanzamientosRestantes == 0)
        {
            botonLanzarDados.interactable = false;
        }
    }

    void Atacar()
    {
        if (!turnoJugador) return;

        ResultadoCombinacion resultado = controlDados.DetectarCombinacion();
        int daño = controlDados.CalcularDaño(resultado);

        vidaEnemigo -= daño;
        StartCoroutine(MostrarTextoAnimado($"Combinación: {resultado.nombre}\nDaño: {daño}\nVida enemigo: {vidaEnemigo}"));
        StartCoroutine(AnimarBarraVida(barraVidaEnemigo, vidaEnemigo));

        animatorJugador.SetTrigger("playerAttack");
        StartCoroutine(MoverJugadorDuranteAtaque());
        animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Damage");

        StartCoroutine(TemblorConDelay());

        turnoJugador = false;
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;

        if (vidaEnemigo <= 0)
        {
            animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Death");
            StartCoroutine(MostrarTextoAnimado("¡Ganaste!"));
            return;
        }

        Invoke(nameof(TurnoEnemigo), 2f);
    }

    IEnumerator MoverJugadorDuranteAtaque()
    {
        Vector3 original = jugadorGO.transform.position;
        Vector3 ataque = enemigoGO.transform.position + new Vector3(-2.0f, 0, 0);

        float duracion = 0.3f;
        float t = 0;

        while (t < duracion)
        {
            jugadorGO.transform.position = Vector3.Lerp(original, ataque, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        jugadorGO.transform.position = ataque;

        yield return new WaitForSeconds(0.2f);

        t = 0;
        while (t < duracion)
        {
            jugadorGO.transform.position = Vector3.Lerp(ataque, original, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        jugadorGO.transform.position = original;
    }

    void TurnoEnemigo()
    {
        int daño = Random.Range(10, 21);
        vidaJugador -= daño;
        StartCoroutine(AnimarBarraVida(barraVidaJugador, vidaJugador));

        animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Attack");

        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        if (info != null && info.moverDuranteAtaque)
        {
            StartCoroutine(MoverEnemigoDuranteAtaque());
        }

        StartCoroutine(MostrarEfecto(jugadorGO, info, 0.5f));
        animatorJugador.SetTrigger("playerDamage");

        StartCoroutine(TemblorConDelay());

        if (vidaJugador <= 0)
        {
            animatorJugador.SetTrigger("playerDeath");
            StartCoroutine(MostrarTextoAnimado("¡Perdiste!"));
            return;
        }

        ReiniciarTurnoJugador();
    }

    IEnumerator MostrarEfecto(GameObject jugadorGO, EnemigoInfo info, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 offset = new Vector3(-0.1f, -0.15f, -0.1f);
        Vector3 posicion = jugadorGO.transform.position + offset;

        GameObject efecto = Instantiate(info.efectoAtaque, posicion, Quaternion.identity);
        Destroy(efecto, 1f);
    }

    IEnumerator MoverEnemigoDuranteAtaque()
    {
        Vector3 original = enemigoGO.transform.position;
        Vector3 ataque = jugadorGO.transform.position + new Vector3(2.0f, 0, 0);

        float duracion = 0.3f;
        float t = 0;

        while (t < duracion)
        {
            enemigoGO.transform.position = Vector3.Lerp(original, ataque, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        enemigoGO.transform.position = ataque;

        yield return new WaitForSeconds(0.2f);

        t = 0;
        while (t < duracion)
        {
            enemigoGO.transform.position = Vector3.Lerp(ataque, original, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        enemigoGO.transform.position = original;
    }

    void ReiniciarTurnoJugador()
    {
        turnoJugador = true;
        lanzamientosRestantes = 3;
        yaLanzasteUnaVez = false;
        controlDados.ResetearDados();

        botonLanzarDados.interactable = true;
        botonAtacar.interactable = true;

        StartCoroutine(MostrarTextoAnimado("Tu turno. Lanza los dados."));
    }

    IEnumerator TemblorConDelay()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(CameraShake());
        StartCoroutine(SacudirPersonaje(jugadorGO));
        StartCoroutine(SacudirPersonaje(enemigoGO));
    }

    IEnumerator CameraShake()
    {
        if (camaraCombate == null) yield break;

        Vector3 original = camaraCombate.transform.position;
        float duration = 0.2f;
        float magnitude = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            camaraCombate.transform.position = original + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camaraCombate.transform.position = original;
    }

    IEnumerator SacudirPersonaje(GameObject obj)
    {
        Vector3 original = obj.transform.position;
        float duration = 0.2f;
        float magnitude = 0.05f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            obj.transform.position = original + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = original;
    }

    void FinCombateJugadorGana()
    {
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;
    }

    // NUEVA CORRUTINA para animar texto letra por letra
    IEnumerator MostrarTextoAnimado(string mensaje)
    {
        mensajeCombate.text = "";
        foreach (char c in mensaje)
        {
            mensajeCombate.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    // NUEVA CORRUTINA para animar la barra de vida suavemente
    IEnumerator AnimarBarraVida(Slider barra, int nuevaVida)
    {
        float duracion = 0.8f;
        float tiempo = 0f;
        float vidaInicial = barra.value;

        while (tiempo < duracion)
        {
            barra.value = Mathf.Lerp(vidaInicial, nuevaVida, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        barra.value = nuevaVida;
    }
}
