using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{

    Define.CamaraMode _mode = Define.CamaraMode.TPSView;

    Vector3 _rayDirection;

    [SerializeField]
    public Transform _mainCamera;

    Transform CameraRay;

    float _camDist;
    float _maxDist;
    float _smoothness = 5.0f;

    void Start()
    {
        _camDist = _mainCamera.localPosition.magnitude;
        _maxDist = _camDist;

        CameraRay = transform.Find("CameraRay");
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        LookAround();
    }

    private void LateUpdate()
    {
        if (_mode == Define.CamaraMode.TPSView)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (CameraRay.position - transform.position).normalized, out hit, _camDist, LayerMask.GetMask("Object")))
            {
                _camDist = hit.distance;
            }
            else
            {
                _camDist = _maxDist;
            }
        }
        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition, _mainCamera.localPosition.normalized * _camDist, _smoothness * Time.deltaTime);

        Debug.DrawRay(transform.position, _mainCamera.transform.position - transform.position, Color.red);
        // Debug.DrawRay(transform.localPosition, _rayDirection * _camDist, Color.red);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = transform.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
