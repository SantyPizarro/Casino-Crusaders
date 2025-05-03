using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mejora
{
    public string nombre;
    public string descripcion;
    public Sprite icono; //puede ser image
    public System.Action aplicarMejora;
}

public static class ModificadoresDaño
{
    public static Dictionary<string, int> multiplicadoresExtra = new Dictionary<string, int>();
}