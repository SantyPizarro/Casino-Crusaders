using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ControlDados : MonoBehaviour
{
    private int[] valoresDados;
    public Image[] imagenesDados;
    public Sprite[] carasDados;
    public Text resultadoTexto;
    public Sprite caraVacia;

    private bool[] dadosGuardados;
    private Vector2[] posicionesOriginales;

    public int cantidadDadosActivos = 5;

    private void Start()
    {
        resultadoTexto.text = "Juego iniciado";
        valoresDados = new int[imagenesDados.Length];
        dadosGuardados = new bool[imagenesDados.Length];

        posicionesOriginales = new Vector2[imagenesDados.Length];
        for (int i = 0; i < imagenesDados.Length; i++)
        {
            if (imagenesDados[i] == null)
            {
                Debug.LogError($"imagenesDados[{i}] no está asignado en Inspector.");
                continue;
            }
            RectTransform rt = imagenesDados[i].GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError($"imagenesDados[{i}] no tiene RectTransform.");
                continue;
            }
            posicionesOriginales[i] = rt.anchoredPosition;
            imagenesDados[i].sprite = caraVacia;
            imagenesDados[i].gameObject.SetActive(true);
        }
    }

    public void LanzarDados()
    {
        for (int i = 0; i < cantidadDadosActivos; i++)
        {
            if (!dadosGuardados[i])
            {
                int valorDado = Random.Range(1, 7);
                imagenesDados[i].sprite = carasDados[valorDado - 1];
                valoresDados[i] = valorDado;
            }

            imagenesDados[i].gameObject.SetActive(true);
        }

        for (int i = cantidadDadosActivos; i < imagenesDados.Length; i++)
        {
            imagenesDados[i].gameObject.SetActive(false);
        }

        var resultado = DetectarCombinacion();
        ActualizarResultado(resultado.nombre);
    }

    public void AlternarDado(int indice)
    {
        if (imagenesDados[indice].sprite == caraVacia)
        {
            
            return;
        }
        dadosGuardados[indice] = !dadosGuardados[indice];

        RectTransform rectTransform = imagenesDados[indice].GetComponent<RectTransform>();

        if (dadosGuardados[indice])
        {
            rectTransform.anchoredPosition += new Vector2(0, 10);
        }
        else
        {

            rectTransform.anchoredPosition -= new Vector2(0, 10);
        }
    }

    public ResultadoCombinacion DetectarCombinacion()
    {
        Dictionary<int, int> contador = new Dictionary<int, int>();

        foreach (int valor in valoresDados)
        {
            if (!contador.ContainsKey(valor))
                contador[valor] = 0;
            contador[valor]++;
        }

        var pares = contador.Where(kvp => kvp.Value >= 2).ToList();
        var trios = contador.Where(kvp => kvp.Value == 3).ToList();

        bool escalera1 = Enumerable.Range(1, 5).All(n => valoresDados.Contains(n));
        bool escalera2 = Enumerable.Range(2, 5).Concat(new[] { 6 }).All(n => valoresDados.Contains(n));

        foreach (var par in contador.OrderByDescending(kvp => kvp.Value))
        {
            int valor = par.Key;
            int cantidad = par.Value;

            if (cantidad == 5)
                return new ResultadoCombinacion("generala", valor * 5);
            if (cantidad == 4)
                return new ResultadoCombinacion("poker", valor * 4);
            if (cantidad == 3 && contador.Values.Contains(2))
                return new ResultadoCombinacion("full", valor * 3 + contador.First(kvp => kvp.Value == 2).Key * 2);
            if (cantidad == 3)
                return new ResultadoCombinacion("trio", valor * 3);
            if (cantidad == 2)
                return new ResultadoCombinacion("par", valor * 2);
        }

        if (escalera1 || escalera2)
            return new ResultadoCombinacion("escalera", valoresDados.Sum());

        return new ResultadoCombinacion("nada", 0);
    }

    public string GetCombinacionActual()
    {
        return DetectarCombinacion().nombre;
    }

    public void ResetearDados()
    {
        if (posicionesOriginales == null || posicionesOriginales.Length == 0)
        {
            Debug.LogWarning("ResetearDados llamado antes de inicializar posicionesOriginales");
            return;
        }

        for (int i = 0; i < imagenesDados.Length; i++)
        {
            bool activo = i < cantidadDadosActivos;

            if (imagenesDados[i] == null) continue;

            imagenesDados[i].gameObject.SetActive(activo);
            valoresDados[i] = 0;
            dadosGuardados[i] = false;

            if (activo)
            {
                imagenesDados[i].sprite = caraVacia;

                RectTransform rectTransform = imagenesDados[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                    rectTransform.anchoredPosition = posicionesOriginales[i];
            }
        }

        if (resultadoTexto != null)
            resultadoTexto.text = "";
    }

    //TOCAR ACÁ PARA AÑADIR MEJORAS DE DAÑO
    public int CalcularDaño(ResultadoCombinacion resultado)
    {
        int multiplicadorBase = 1;

        switch (resultado.nombre)
        {
            case "par":
                multiplicadorBase = 2;
                break;
            case "trio":
                multiplicadorBase = 3;
                break;
            case "full":
                multiplicadorBase = 4;
                break;
            case "poker":
                multiplicadorBase = 5;
                break;
            case "generala":
                multiplicadorBase = 6;
                break;
            case "escalera":
                multiplicadorBase = 4;
                break;
            default:
                multiplicadorBase = 1;
                break;
        }

        int extra = 0;
        ModificadoresDaño.multiplicadoresExtra.TryGetValue(resultado.nombre, out extra);
        int multiplicadorFinal = multiplicadorBase + extra;

        return resultado.sumaDados * multiplicadorFinal;
    }


    public void ActualizarResultado(string combinacion)
    {
        resultadoTexto.SetAllDirty();
        resultadoTexto.text = $"Combinación: {combinacion}";
    }

    public int[] ObtenerValoresDados()
    {
        return valoresDados;
    }
}
