using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {

        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            
        }

        return Resources.Load<T>(path);
    }

    // 부모지정 스폰
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }


        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    // 위치지정 스폰
    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        Debug.Log("instanciate position");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go = Object.Instantiate(original, position, Quaternion.identity);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        

        Object.Destroy(go);
    }
}
