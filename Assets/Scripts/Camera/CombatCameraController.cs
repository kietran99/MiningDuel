using MD.Character;
using UnityEngine;

public class CombatCameraController : MonoBehaviour
{
    [SerializeField]
    private float _maxZoomRate = 1.2f;

    [SerializeField]
    private float _zoomInSpeed = 8f;

    [SerializeField]
    private float _zoomOutSpeed = .5f;

    [SerializeField]
    private float _acceptableZoomOffset = .05f;

    [SerializeField]
    private float _zoomOutWaitDuration = .8f;

    private Camera _camera;
    private float _baseZoom, _minZoom;
    private bool _shouldZoomIn, _shouldZoomOut;
    private float _elapsedZoomOutWaitTime;

    private void Start()
    {
        _camera = Camera.main;
        _baseZoom = _camera.orthographicSize;
        _minZoom = _baseZoom / _maxZoomRate;
        _shouldZoomIn = false;
        _shouldZoomOut = false;
        _elapsedZoomOutWaitTime = 0f;
        EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<DamageGivenData>(OnDamageGiven);
    }

    private void OnDamageGiven()
    {
        _shouldZoomIn = true;   
    }

    private void Update()
    {
        if (_shouldZoomIn)
        {
            _camera.orthographicSize -= Time.deltaTime * _zoomInSpeed;

            if (_camera.orthographicSize - _acceptableZoomOffset <= _minZoom)
            {
                _camera.orthographicSize = _minZoom;
                _shouldZoomIn = false;
                _shouldZoomOut = true;
                _elapsedZoomOutWaitTime = _zoomOutWaitDuration;
            }

            return;
        }

        if (_elapsedZoomOutWaitTime >= 0f)
        {
            _elapsedZoomOutWaitTime -= Time.deltaTime;
            return;
        } 

        if (!_shouldZoomOut)
        {
            return;
        }

        _camera.orthographicSize += Time.deltaTime * _zoomOutSpeed;

        if (_camera.orthographicSize + _acceptableZoomOffset >= _baseZoom)
        {
            _camera.orthographicSize = _baseZoom;
            _shouldZoomOut = false;
        }
    }
}
