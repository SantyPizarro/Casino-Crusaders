using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Settings.scripts;
using UnityEngine.SceneManagement;

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
    public Button botonVolverMapa;

    void Start()
    {
        botonVolverMapa.gameObject.SetActive(false);
        var controlJuego = ControlJuego.Instance;

        if (controlJuego.listaEnemigos == null || controlJuego.listaEnemigos.Count == 0)
            controlJuego.Inicializar();

        enemigoActual = controlJuego.ObtenerEnemigoActual();
        personajeJugador = controlJuego.personajeJugador;

        vidaJugador = personajeJugador.vidaActual;
        vidaEnemigo = enemigoActual.vida;

        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        barraVidaJugador.maxValue = personajeJugador.vidaMaxima;
        barraVidaJugador.value = vidaJugador;

        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        nombreEnemigoTexto.text = enemigoActual.nombre;

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
            yaLanzasteUnaVez = true;

        StartCoroutine(MostrarTextoAnimado("Lanzamientos restantes: " + lanzamientosRestantes));

        if (lanzamientosRestantes == 0)
            botonLanzarDados.interactable = false;
    }

    void Atacar()
    {
        if (!turnoJugador) return;

        ResultadoCombinacion resultado = controlDados.DetectarCombinacion();
        int daño = controlDados.CalcularDaño(resultado) + personajeJugador.danoAtaque - enemigoActual.defensa;
        daño = Mathf.Max(0, daño);
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
            //Personaje dar monedas
            botonVolverMapa.gameObject.SetActive(true);
            VariablesMapa.nivelesCompletados[VariablesMapa.nivel] = true;
            Debug.Log("Nivel marcado como completado: " + VariablesMapa.nivel);


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
        StartCoroutine(AnimarBarraVida(barraVidaJugador, vidaJugador));

        animatorEnemigo.SetTrigger(prefijoAnimacionEnemigo + "Attack");

        EnemigoInfo info = enemigoGO.GetComponent<EnemigoInfo>();
        if (info != null && info.moverDuranteAtaque)
            StartCoroutine(MoverEnemigoDuranteAtaque());

        StartCoroutine(MostrarEfecto(jugadorGO, info, 0.5f));
        animatorJugador.SetTrigger("playerDamage");

        StartCoroutine(TemblorConDelay());

        if (vidaJugador <= 0)
        {
            animatorJugador.SetTrigger("playerDeath");
            StartCoroutine(MostrarTextoAnimado("¡Perdiste!"));
            
            botonVolverMapa.gameObject.SetActive(true);
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

    void SiguienteEscena()
    {

        var controlJuego = ControlJuego.Instance;
        controlJuego.AvanzarASiguienteEscena();
    }

    IEnumerator MostrarTextoAnimado(string mensaje)
    {
        mensajeCombate.text = "";
        foreach (char c in mensaje)
        {
            mensajeCombate.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

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

    public void VolverAlMapa()
    {
        //VariablesMapa.nivelesCompletados[VariablesMapa.nivel] = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Mapa");
    }
}
