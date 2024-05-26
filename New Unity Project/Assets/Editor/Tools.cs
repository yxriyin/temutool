using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Tools
{
    [MenuItem("Tools/CreateMesh")]
    public static void justdoit()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
    }
}
