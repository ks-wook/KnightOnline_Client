using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance {  get { Init(); return s_instance; } }

    SoundManager _sound = new SoundManager();
    PoolManager _pool = new PoolManager();
    InventoryManager _inven = new InventoryManager();
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    NetworkManager _network = new NetworkManager();
    ObjectManager _object = new ObjectManager();
    UIManager _ui = new UIManager();
    WebManager _web = new WebManager();
    DialogueManager _dialogue = new DialogueManager();
    QuestManager _quest = new QuestManager();
    RaidGameManager _raidGame = new RaidGameManager();

    public static SoundManager Sound { get { return Instance._sound; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static InventoryManager Inven { get { return Instance._inven; } }
    public static DataManager Data { get { return Instance._data; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static ObjectManager Object { get { return Instance._object; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static WebManager Web { get { return Instance._web; } }
    public static DialogueManager Dialouge { get { return Instance._dialogue; } }
    public static QuestManager Quest { get { return Instance._quest; } }
    public static RaidGameManager RaidGame { get { return Instance._raidGame; } }


    void Start()
    {
        Init();
    }

    
    void Update()
    {
        // _input.OnUpdate();
        _network.Update(); // 네트워크 관련은 유니티 주 쓰레드에서 처리해줘야함
    }

    // 매니저 전체 초기화
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

            s_instance._sound.Init();
            s_instance._data.Init();
            s_instance._pool.Init();
        }
    }
    
    // 씬 전환 시 초기화해야하는 것들을 초기화
    public static void SceneChangeClear()
    {
        Object.Clear();
        Sound.Clear();
        UI.Clear();
        Scene.Clear();
        Pool.Clear();
    }
}
