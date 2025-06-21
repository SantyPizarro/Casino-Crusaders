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

        mensajeEvento.text = "Tira los dados sin temor,\n¡la suerte dicta tu honor!";
        botonAceptar.interactable = true;
    }

    void LanzarDados()
    {
        if (lanzamientosRestantes <= 0) return;

        controlDados.LanzarDados();
        lanzamientosRestantes--;

        combinacionFinal = controlDados.DetectarCombinacion();

        if (lanzamientosRestantes > 0)
        {
            StartCoroutine(MostrarTextoAnimado($"Te salió {combinacionFinal.nombre},\n¿será que podés aún mejor?"));
            
        }
        else
        {
            StartCoroutine(MostrarTextoAnimado($"Un {combinacionFinal.nombre} al final quedó,\ntu premio ahora se reveló."));
            botonLanzarDados.interactable = false;
           
        }
    }

    void AceptarResultado()
    {
        dadoLanzado = true;

        switch (combinacionFinal.nombre)
        {
            case "par":
                // personaje.armadura += 5;
                mensajeEvento.text = "Con un par no estás tan mal,\nun poco de escudo... nada banal. (Defensa +10)";
                ControlJuego.Instance.personajeJugador.defensa += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            case "trio":
                // personaje.oro += 10;
                mensajeEvento.text = "Trío dorado, gran ocasión,\nllueven monedas sin perdón. (Oro +10)";
                ControlJuego.Instance.personajeJugador.monedas += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            case "full":
                // personaje.vidaActual += 10;
                mensajeEvento.text = "Full de fortuna, ¡qué bendición!\nRecobrás vida sin condición. (Vida +10)";
                ControlJuego.Instance.personajeJugador.vidaActual += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            case "poker":
                // personaje.armadura += 10;
                mensajeEvento.text = "Un póker al fin apareció,\ntu ataque se fortaleció. (Ataque +10)";
               ControlJuego.Instance.personajeJugador.danoAtaque += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            case "generala":
                // personaje.vidaActual = personaje.vidaMaxima;
                mensajeEvento.text = "Generala, jugada ideal,\ntu subida de estadisticas es total. (Todo +10)";
                ControlJuego.Instance.personajeJugador.vidaMaxima += 10;
                ControlJuego.Instance.personajeJugador.vidaActual += 10;
                ControlJuego.Instance.personajeJugador.defensa += 10;
                ControlJuego.Instance.personajeJugador.danoAtaque += 10;
                ControlJuego.Instance.personajeJugador.monedas += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            case "escalera":
                // personaje.dañoAtaque += 2;
                mensajeEvento.text = "Escalera que no patinó,\ntu ataque se multiplicó. (Ataque y Defensa +10)";
                ControlJuego.Instance.personajeJugador.danoAtaque += 10;
                ControlJuego.Instance.personajeJugador.defensa += 10;
                ControlJuego.Instance.GuardarPersonaje(this);
                break;
            default:
                mensajeEvento.text = "Ni sombra de suerte, ni chispa, ni luz,\nte vas sin premio, ¡qué gran cruz!";
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

