using UnityEngine;
using Siccity.GLTFUtility;

public class GLB
{
    public static GameObject ToGameObject(byte[] data)
    {
        return Importer.LoadFromBytes(data);
    }
}
