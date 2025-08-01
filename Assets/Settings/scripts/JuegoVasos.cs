﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class JuegoDeVasos : MonoBehaviour
{
    public GameObject[] vasos;
    public Transform[] posiciones;
    public GameObject recompensa;
    public Text mensaje;
    public UnityEngine.UI.Button botonComenzar;
    public Sprite spriteVida;
    public Sprite spriteAtaque;
    public Sprite spriteDefensa;
    public UnityEngine.UI.Image imagenRecompensa;
    public UnityEngine.UI.Button botonVolverMapa;


    private int indiceCorrecto;
    private bool puedeElegir = false;

    private int statElegida = -1;

    public Vector3 posicionCentro;
    public float duracionMovimiento = 1f;

    void Start()
    {
        ControlJuego.Instance.ResetVolverAlMapa();
        botonVolverMapa.gameObject.SetActive(false);

        // Agregar listener manualmente para evitar conflictos desde el Inspector
        botonVolverMapa.onClick.RemoveAllListeners();
        botonVolverMapa.onClick.AddListener(() =>
        {
            Debug.Log("Botón Volver al Mapa presionado");
            ControlJuego.Instance.VolverDesdeEvento2();
        });
        botonComenzar.onClick.AddListener(ComenzarJuego);
        mensaje.text = "Pulsa el botón sin vacilar,\n¡los vasos van a bailar!";

        recompensa.SetActive(true);
    }

    void ComenzarJuego()
    {
        botonComenzar.interactable = false;
        mensaje.text = "¡Ojo atento, que va el movimiento!";

        recompensa.SetActive(false);

        indiceCorrecto = Random.Range(0, vasos.Length);

        statElegida = Random.Range(0, 3);
        switch (statElegida)
        {
            case 0: imagenRecompensa.sprite = spriteVida; break;
            case 1: imagenRecompensa.sprite = spriteAtaque; break;
            case 2: imagenRecompensa.sprite = spriteDefensa; break;
        }

        StartCoroutine(DestacarVasoCorrecto());
        StartCoroutine(EjecutarMezclaConDelay());
    }

    IEnumerator MezclarVasos()
    {
        int mezclas = 10;
        float duracionMovimiento = 0.3f;

        for (int i = 0; i < mezclas; i++)
        {
            int a = Random.Range(0, vasos.Length);
            int b;
            do
            {
                b = Random.Range(0, vasos.Length);
            } while (a == b);

            yield return StartCoroutine(IntercambiarPosiciones(vasos[a], vasos[b], duracionMovimiento));
        }

        mensaje.text = "¿Dónde está? ¡Adiviná ya!";
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
            string mensajeRecompensa = statElegida == 0 ? "vida ganada" : statElegida == 1 ? "fuerza aumentada" : "defensa mejorada";
            mensaje.text = $"¡Acierto total, mi buen rival!\nTu premio: {mensajeRecompensa}.";

            switch (statElegida)
            {
                case 0:
                    ControlJuego.Instance.personajeJugador.vidaMaxima += 10;
                    ControlJuego.Instance.personajeJugador.vidaActual += 10;
                    ControlJuego.Instance.GuardarPersonaje(this);
                    break;
                case 1:
                    ControlJuego.Instance.personajeJugador.danoAtaque += 2;
                    ControlJuego.Instance.GuardarPersonaje(this);
                    break;
                case 2:
                    ControlJuego.Instance.personajeJugador.defensa += 2;
                    ControlJuego.Instance.GuardarPersonaje(this);
                    break;
            }

            imagenRecompensa.enabled = true;
            StartCoroutine(terminarJuego(true));
        }
        else
        {
            mensaje.text = "¡Ups! No fue la opción ideal,\nesta ronda no va al historial.";
            imagenRecompensa.enabled = false;
            StartCoroutine(terminarJuego(true));
        }

        botonComenzar.interactable = true;
    }

    IEnumerator MoverRecompensaAlVaso(Vector3 destino)
    {
        imagenRecompensa.rectTransform.position = destino;
        imagenRecompensa.enabled = true;

        Vector3 posOriginal = destino;
        Vector3 posLevantada = posOriginal + new Vector3(0, 30, 0);

        float duracionLevantada = 0.3f;
        float tiempo = 0;

        while (tiempo < duracionLevantada)
        {
            vasos[indiceCorrecto].transform.position = Vector3.Lerp(posOriginal, posLevantada, tiempo / duracionLevantada);
            tiempo += Time.deltaTime;
            yield return null;
        }

        vasos[indiceCorrecto].transform.position = posLevantada;

        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator terminarJuego(bool acerto)
    {
        botonVolverMapa.gameObject.SetActive(true);
        if (acerto)
        {
            yield return StartCoroutine(MoverRecompensaAlVaso(vasos[indiceCorrecto].transform.position));
        }

        yield return new WaitForSeconds(1.5f);
        //ControlJuego.Instance.AvanzarASiguienteEscena();
    }

    IEnumerator IntercambiarPosiciones(GameObject vasoA, GameObject vasoB, float duracion)
    {
        Vector3 inicioA = vasoA.transform.position;
        Vector3 inicioB = vasoB.transform.position;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            vasoA.transform.position = Vector3.Lerp(inicioA, inicioB, t);
            vasoB.transform.position = Vector3.Lerp(inicioB, inicioA, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        vasoA.transform.position = inicioB;
        vasoB.transform.position = inicioA;
    }

    IEnumerator EjecutarMezclaConDelay(float delay = 1.2f)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(MezclarVasos());
    }

    IEnumerator DestacarVasoCorrecto()
    {
        UnityEngine.UI.Image imagenVaso = vasos[indiceCorrecto].GetComponent<UnityEngine.UI.Image>();
        if (imagenVaso == null) yield break;

        int parpadeos = 4;
        float intervalo = 0.2f;

        Color colorOriginal = imagenVaso.color;
        Color colorDestacado = Color.red;

        for (int i = 0; i < parpadeos; i++)
        {
            imagenVaso.color = colorDestacado;
            yield return new WaitForSeconds(intervalo);
            imagenVaso.color = colorOriginal;
            yield return new WaitForSeconds(intervalo);
        }
    }
}