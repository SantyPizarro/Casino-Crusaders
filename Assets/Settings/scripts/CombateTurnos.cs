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

    public GameObject jugadorGO;
    public GameObject enemigoGO;

    private Animator animatorJugador;
    private Animator animatorEnemigo;

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

        animatorJugador = jugadorGO.GetComponent<Animator>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

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
        int daño = controlDados.CalcularDaño(resultado);

        vidaEnemigo -= daño;
        mensajeCombate.text = $"Combinación: {resultado.nombre}\nDaño: {daño}\nVida enemigo: {vidaEnemigo}";
        barraVidaEnemigo.value = vidaEnemigo;

        animatorJugador.SetTrigger("playerAttack");
        animatorEnemigo.SetTrigger("cardDamage");
        

        turnoJugador = false;
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;

        if (vidaEnemigo <= 0)
        {
            animatorEnemigo.SetTrigger("cardDeath");
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

        animatorEnemigo.SetTrigger("cardAttack");
        animatorJugador.SetTrigger("playerDamage");
         

        if (vidaJugador <= 0)
        {
            animatorJugador.SetTrigger("playerDeath");
            mensajeCombate.text = "¡Perdiste!";
        ;
            Invoke(nameof(FinCombateJugadorGana), 1.5f);
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
    void FinCombateJugadorGana()
    {
        
        botonLanzarDados.interactable = false;
        botonAtacar.interactable = false;

     
    }
}
