using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Settings.scripts;

public class JuegoDeVasos : MonoBehaviour
{
    public GameObject[] vasos;
    public Transform[] posiciones;
    public GameObject recompensa;
    public Text mensaje;
    public Button botonComenzar;
    public Sprite spriteVida;
    public Sprite spriteAtaque;
    public Sprite spriteDefensa;
    public Image imagenRecompensa;

    private int indiceCorrecto;
    private bool puedeElegir = false;

    private int statElegida = -1;

    public Vector3 posicionCentro;
    public float duracionMovimiento = 1f;

    void Start()
    {
        botonComenzar.onClick.AddListener(ComenzarJuego);
        mensaje.text = "Pulsa comenzar para mezclar los vasos.";

        imagenRecompensa.rectTransform.position = vasos[1].transform.position;
        imagenRecompensa.enabled = false; // ocultar al inicio
        recompensa.SetActive(true);
    }

    void ComenzarJuego()
    {
        botonComenzar.interactable = false;
        mensaje.text = "Observa bien...";

        recompensa.SetActive(false);
        imagenRecompensa.enabled = true;

        indiceCorrecto = Random.Range(0, vasos.Length);

        statElegida = Random.Range(0, 3);
        switch (statElegida)
        {
            case 0: imagenRecompensa.sprite = spriteVida; break;
            case 1: imagenRecompensa.sprite = spriteAtaque; break;
            case 2: imagenRecompensa.sprite = spriteDefensa; break;
        }

        foreach (var vaso in vasos)
        {
            vaso.transform.position += new Vector3(0, 50, 0);
        }

        StartCoroutine(MezclarVasos());
    }

    IEnumerator MezclarVasos()
    {
        int mezclas = 5;
        for (int i = 0; i < mezclas; i++)
        {
            int a = Random.Range(0, vasos.Length);
            int b;
            do
            {
                b = Random.Range(0, vasos.Length);
            } while (a == b);

            Vector3 posA = vasos[a].transform.position;
            vasos[a].transform.position = vasos[b].transform.position;
            vasos[b].transform.position = posA;

            yield return new WaitForSeconds(0.6f);
        }

        mensaje.text = "¡Elige un vaso!";
        puedeElegir = true;
    }

    public void ElegirVaso(int indiceElegido)
    {
        if (!puedeElegir) return;

        puedeElegir = false;
        recompensa.SetActive(true);
        botonComenzar.gameObject.SetActive(false);

        var personaje = ControlJuego.Instance.personajeJugador;

        if (indiceElegido == indiceCorrecto)
        {
            string mensajeRecompensa = statElegida == 0 ? "vida" : statElegida == 1 ? "ataque" : "defensa";
            mensaje.text = "¡Correcto! tu recompensa es..." + mensajeRecompensa;

            switch (statElegida)
            {
                case 0:
                    personaje.vidaMaxima += 5;
                    personaje.vidaActual += 5;
                    break;
                case 1:
                    personaje.danoAtaque += 5;
                    break;
                case 2:
                    personaje.defensa += 5;
                    break;
            }

            imagenRecompensa.enabled = true;

            StartCoroutine(terminarJuego(true));
        }


        botonComenzar.interactable = true;
    }

    IEnumerator MoverRecompensaAlVaso(Vector3 destino)
    {
        Vector3 inicio = imagenRecompensa.rectTransform.position;
        float tiempo = 0;

        while (tiempo < duracionMovimiento)
        {
            imagenRecompensa.rectTransform.position = Vector3.Lerp(inicio, destino, tiempo / duracionMovimiento);
            tiempo += Time.deltaTime;
            yield return null;
        }

        imagenRecompensa.rectTransform.position = destino;

        Vector3 posOriginal = vasos[indiceCorrecto].transform.position;
        Vector3 posLevantada = posOriginal + new Vector3(0, 30, 0);

        float duracionLevantada = 0.3f;
        tiempo = 0;

        while (tiempo < duracionLevantada)
        {
            vasos[indiceCorrecto].transform.position = Vector3.Lerp(posOriginal, posLevantada, tiempo / duracionLevantada);
            tiempo += Time.deltaTime;
            yield return null;
        }
        vasos[indiceCorrecto].transform.position = posLevantada;

        yield return new WaitForSeconds(1.2f);
        imagenRecompensa.enabled = false;
    }


    IEnumerator terminarJuego(bool acerto)
    {
        if (acerto)
        {
            yield return StartCoroutine(MoverRecompensaAlVaso(vasos[indiceCorrecto].transform.position));
        }

        yield return new WaitForSeconds(1.5f);

        ControlJuego.Instance.AvanzarASiguienteEscena();
    }
}
