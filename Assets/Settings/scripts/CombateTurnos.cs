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
        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        barraVidaJugador.maxValue = vidaJugador;
        barraVidaJugador.value = vidaJugador;

        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        // Obtener prefijo de animaci�n del enemigo
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
        int da�o = controlDados.CalcularDa�o(resultado);

        vidaEnemigo -= da�o;
        mensajeCombate.text = $"Combinaci�n: {resultado.nombre}\nDa�o: {da�o}\nVida enemigo: {vidaEnemigo}";
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
            mensajeCombate.text += "\n�Ganaste!";
            return;
        }

        Invoke(nameof(TurnoEnemigo), 2f);
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
        int da�o = Random.Range(10, 21);
        vidaJugador -= da�o;
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
            mensajeCombate.text = "�Perdiste!";
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
}