using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Canvas canvas;
    EventSystem evs;

    private GameObject _targetTr;

    private Vector2 _beginPoint;
    private Vector2 _moveBegin;
    private Vector2 _resetPoint = new Vector2(0, 0);

    private GraphicRaycaster _gr;
    private PointerEventData _ped;

    List<RaycastResult> _results = new List<RaycastResult>();
    List<Transform> slots = new List<Transform>();

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        canvas = FindObjectOfType<Canvas>();
        _ped = new PointerEventData(FindObjectOfType<EventSystem>());

        _gr = canvas.GetComponent<GraphicRaycaster>();
        _ped = new PointerEventData(evs);

        for(int i = 0; i < transform.childCount; i++)
            slots.Add(transform.GetChild(i));
        
    }

    public GameObject getFirstTartgetItem(PointerEventData eventData)
    {
        _results.Clear();

        _ped.position = Input.mousePosition;
        _gr.Raycast(_ped, _results);

        if (_results[0].gameObject.tag == "UI_ITEM")
            return _results[0].gameObject;

        return null;
    }

    // 드래그 시작 위치 지정
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _targetTr = getFirstTartgetItem(eventData);

        if (_targetTr == null)
            return;

        _targetTr.transform.SetParent(transform.GetChild(transform.childCount - 1));

        _beginPoint = _targetTr.transform.position;
        _moveBegin = eventData.position;
    }

    // 드래그 : 마우스 커서 위치로 이동
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (_targetTr == null)
            return;

        _targetTr.transform.position = _beginPoint + (eventData.position - _moveBegin);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_targetTr == null)
            return;

        _targetTr.transform.localPosition = _resetPoint;

        _results.Clear();

        _ped.position = Input.mousePosition;
        _gr.Raycast(_ped, _results);

        Debug.Log(_results[0]);

        _targetTr.transform.SetParent(_results[0].gameObject.transform);
        _targetTr.transform.localPosition = _resetPoint;


    }

    
}
