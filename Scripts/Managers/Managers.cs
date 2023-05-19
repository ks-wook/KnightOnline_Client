using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance {  get { Init(); return s_instance; } }

    InventoryManager _inven = new InventoryManager();
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    NetworkManager _network = new NetworkManager();
    ObjectManager _object = new ObjectManager();
    UIManager _ui = new UIManager();
    WebManager _web = new WebManager();
    DialogueManager _dialogue = new DialogueManager();
    QuestManager _quest = new QuestManager();


    public static InventoryManager Inven { get { return Instance._inven; } }
    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static ObjectManager Object { get { return Instance._object; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static WebManager Web { get { return Instance._web; } }
    public static DialogueManager Dialouge { get { return Instance._dialogue; } }
    public static QuestManager Quest { get { return Instance._quest; } }

    void Start()
    {
        Init();
    }

    
    void Update()
    {
        // _input.OnUpdate();
        _network.Update(); // 네트워크 관련은 유니티 주 쓰레드에서 처리해줘야함
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null) // 없는 경우 Instanciate 후 반환
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
        }
    }

    public static void Clear()
    {
        // TODO
    }
}
