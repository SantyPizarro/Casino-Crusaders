using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISeleccionMejoras : MonoBehaviour
{
    public ControlMejoras controlMejoras; // Asignar en el Inspector
    public GameObject prefabMejoraUI; // Prefab de la UI
    public Transform contenedorMejoras; // Un panel para instanciar las opciones

    private void Start()
    {
        MostrarMejoras();
    }

    void MostrarMejoras()
    {
        List<Mejora> mejoras = controlMejoras.ElegirMejorasAleatorias(3);


        foreach (Mejora mejora in mejoras)
        {
            GameObject nuevaUI = Instantiate(prefabMejoraUI, contenedorMejoras);

                // Asumimos que el prefab tiene estos elementos con estos nombres
                nuevaUI.transform.Find("Nombre").GetComponent<Text>().text = mejora.nombre;
            nuevaUI.transform.Find("Descripcion").GetComponent<Text>().text = mejora.descripcion;
            nuevaUI.transform.Find("Icono").GetComponent<Image>().sprite = mejora.icono;

            Button boton = nuevaUI.transform.Find("boton").GetComponent<Button>();
            boton.onClick.AddListener(() =>
            {
                mejora.aplicarMejora?.Invoke();
                Debug.Log("Aplicada mejora: " + mejora.nombre);
            });

        }
    }
}
