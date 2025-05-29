using UnityEngine;
using UnityEngine.UI;

public class TiendaController : MonoBehaviour
{
    public int monedas = 100;
    public int vidaActual = 50;
    public int vidaMaxima = 100;
    public int armadura = 0;
    public int dano = 0;

    public Text TextoMonedas;
    public Text TextoVida;
    public Text TextoVidaMax;
    public Text TextoArmadura;
    public Text TextoDano;

    private const int COSTO = 5;

    void Start()
    {
        Debug.Log("TextoMonedas es null? " + (TextoMonedas == null));
        Debug.Log("TextoVida es null? " + (TextoVida == null));
        ActualizarUI();
    }

    public void ComprarArmadura()
    {
        if (monedas >= COSTO)
        {
            armadura += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarVida()
    {
        if (monedas >= COSTO)
        {
            vidaActual += 25;
            if (vidaActual > vidaMaxima)
                vidaActual = vidaMaxima;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarVidaMax()
    {
        if (monedas >= COSTO)
        {
            vidaMaxima += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    public void ComprarDano()
    {
        if (monedas >= COSTO)
        {
            dano += 5;
            monedas -= COSTO;
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        TextoMonedas.text = "Monedas: " + monedas;
        TextoVida.text = "Vida: " + vidaActual;
        TextoVidaMax.text = "Vida Máx: " + vidaMaxima;
        TextoArmadura.text = "Armadura: " + armadura;
        TextoDano.text = "Daño: " + dano;
    }
}
