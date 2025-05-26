using UnityEngine;
using System.Collections;

public class PanelDesplegableController : MonoBehaviour
{
    public RectTransform panel; // Panel debe tener un RectTransform
    public Vector2 posicionOculta;
    public Vector2 posicionVisible;
    public float velocidad = 5f;
    private bool visible = false;

    void Start()
    {
        panel.anchoredPosition = posicionOculta; // Empieza oculto
    }

    public void TogglePanel()
    {
        StopAllCoroutines(); // Evita conflictos si se presiona varias veces
        StartCoroutine(MoverPanel(visible ? posicionOculta : posicionVisible));
        visible = !visible;
    }

    IEnumerator MoverPanel(Vector2 destino)
    {
        while (Vector2.Distance(panel.anchoredPosition, destino) > 0.1f)
        {
            panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, destino, Time.deltaTime * velocidad);
            yield return null;
        }

        panel.anchoredPosition = destino;
    }
}