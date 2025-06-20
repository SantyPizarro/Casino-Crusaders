using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Settings.scripts;

public class EventoDados : MonoBehaviour
{
    public ControlDados controlDados;
    public Text mensajeEvento;
    public Button botonLanzarDados;
    public Button botonAceptar;
    public GameObject panelResultado;

    private bool dadoLanzado = false;
    private int lanzamientosRestantes = 3;
    private ResultadoCombinacion combinacionFinal;
    private Personaje personaje;

    void Start()
    {
        personaje = ControlJuego.Instance.personajeJugador;
        botonLanzarDados.onClick.AddListener(LanzarDados);
        botonAceptar.onClick.AddListener(AceptarResultado);
        panelResultado.SetActive(true);

        mensajeEvento.text = "Tira los dados sin temor,\n�la suerte dicta tu honor!";
        botonAceptar.interactable = false;
    }

    void LanzarDados()
    {
        if (lanzamientosRestantes <= 0) return;

        controlDados.LanzarDados();
        lanzamientosRestantes--;

        combinacionFinal = controlDados.DetectarCombinacion();

        if (lanzamientosRestantes > 0)
        {
            StartCoroutine(MostrarTextoAnimado($"Te sali� un {combinacionFinal.nombre},\n�ser� que pod�s a�n mejor?"));
        }
        else
        {
            StartCoroutine(MostrarTextoAnimado($"Un {combinacionFinal.nombre} final qued�,\ntu premio ahora se revel�."));
            botonLanzarDados.interactable = false;
            botonAceptar.interactable = true;
        }
    }

    void AceptarResultado()
    {
        dadoLanzado = true;

        switch (combinacionFinal.nombre)
        {
            case "par":
                // personaje.armadura += 5;
                mensajeEvento.text = "Con un par no est�s tan mal,\nun poco de escudo... nada banal.";
                break;
            case "trio":
                // personaje.oro += 10;
                mensajeEvento.text = "Tr�o dorado, gran ocasi�n,\nllueven monedas sin perd�n.";
                break;
            case "full":
                // personaje.vidaActual += 10;
                mensajeEvento.text = "Full de fortuna, �qu� bendici�n!\nRecobr�s vida sin condici�n.";
                break;
            case "poker":
                // personaje.armadura += 10;
                mensajeEvento.text = "Un p�ker al fin apareci�,\ntu armadura se fortaleci�.";
                break;
            case "generala":
                // personaje.vidaActual = personaje.vidaMaxima;
                mensajeEvento.text = "Generala, jugada ideal,\ntu vida vuelve a su nivel total.";
                break;
            case "escalera":
                // personaje.da�oAtaque += 2;
                mensajeEvento.text = "Escalera que no patin�,\ntu ataque se multiplic�.";
                break;
            default:
                mensajeEvento.text = "Ni sombra de suerte, ni chispa, ni luz,\nte vas sin premio, �qu� gran cruz!";
                break;
        }

        botonAceptar.interactable = false;
    }

    IEnumerator MostrarTextoAnimado(string mensaje)
    {
        mensajeEvento.text = "";
        foreach (char c in mensaje)
        {
            mensajeEvento.text += c;
            yield return new WaitForSeconds(0.015f);
        }
    }
}