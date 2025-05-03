using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Image imagenJugador;   // ESTOS SE CAMBIAN POR SPRITERENDERER
    public Image imagenEnemigo;   // SI SE USA UN SPRITE EN EL FUTURO

    public Sprite spriteJugadorNormal;
    public Sprite spriteJugadorGolpe;

    public Sprite spriteEnemigoNormal;
    public Sprite spriteEnemigoGolpe;

    public Slider barraVidaJugador;
    public Slider barraVidaEnemigo;

    void Start()
    {
        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAtacar.onClick.AddListener(Atacar);

        barraVidaJugador.maxValue = vidaJugador;
        barraVidaJugador.value = vidaJugador;

        barraVidaEnemigo.maxValue = vidaEnemigo;
        barraVidaEnemigo.value = vidaEnemigo;

        ReiniciarTurnoJugador();
    }

    void LanzarDados()
    {
        if (!turnoJugador || lanzamientosRestantes <= 0) return;

        controlDados.LanzarDados();
        lanzamientosRestantes--;

        mensajeCombate.text = "Lanzamientos restantes: " + lanzamientosRestantes.ToString();

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
        mensajeCombate.text = $"Combinación: {resultado.nombre}\nDaño: {daño}\nVida enemigo: {vidaEnemigo}"; //debug
        barraVidaEnemigo.value = vidaEnemigo;


        StartCoroutine(MostrarGolpe(imagenEnemigo, spriteEnemigoGolpe, spriteEnemigoNormal)); //piña jugador, cambia sprite enemigo

        turnoJugador = false;
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;

        if (vidaEnemigo <= 0)
        {
            mensajeCombate.text += "\n¡Ganaste!";
            return;
        }

        Invoke(nameof(TurnoEnemigo), 2f);
    }


    void TurnoEnemigo()
    {
        int daño = Random.Range(10, 21);
        vidaJugador -= daño;
        barraVidaJugador.value = vidaJugador;

        Debug.Log(vidaEnemigo);
        StartCoroutine(MostrarGolpe(imagenJugador, spriteJugadorGolpe, spriteJugadorNormal)); // piña del enemigo, le cambia el sprite al jugador

        if (vidaJugador <= 0)
        {
            mensajeCombate.text = $"¡Perdiste!";
            return;
        }


        ReiniciarTurnoJugador();
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

    System.Collections.IEnumerator MostrarGolpe(Image imagen, Sprite spriteGolpe, Sprite spriteNormal)
    {
        Color colorOriginal;
        colorOriginal = imagen.color;

        imagen.sprite = spriteGolpe;
        imagen.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        imagen.color = colorOriginal;
        imagen.sprite = spriteNormal;
    }
}
