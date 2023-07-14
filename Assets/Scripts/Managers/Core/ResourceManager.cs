using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 리소스 관련 매니저 
 * 
 * 리소스 Load, Instanciate를 담당
 */

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

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
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

        // instanciate 하려는 오브젝트가 오브젝트 풀링이 되어있는 경우
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;


        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        return go;
    }

    // 위치지정 스폰
    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        Debug.Log("instanciate position");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // instanciate 하려는 오브젝트가 오브젝트 풀링이 되어있는 경우
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;


        GameObject go = Object.Instantiate(original, position, Quaternion.identity);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null) // 풀링해야 하는 객체라면 지우지 않고 Pool에 Push
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
