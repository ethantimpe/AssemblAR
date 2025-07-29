using UnityEngine;
using System.IO;
using Siccity.GLTFUtility;

public class GLB
{
    public static GameObject ToGameObject(byte[] data)
    {
        string path = Path.Combine(Application.persistentDataPath, "tempModel.glb");
        Debug.Log(path);
        File.WriteAllBytes(path, data);

        return Importer.LoadFromFile(path);
    }
}
