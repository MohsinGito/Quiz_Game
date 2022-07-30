using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    private void Awake()
    {
        T CreateInstance = Instance;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];

                if (objs.Length > 0)
                        _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    for(int i = 1; i < objs.Length; i++)
                    {
                        Destroy(objs[i].gameObject);
                    }
                }
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = string.Format("_{0}", typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
               // DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
 }