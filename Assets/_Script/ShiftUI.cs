using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftUI : MonoBehaviour
{
    private bool _forward = false;
    private Coroutine _coroutine;
    private float _t;
    [SerializeField]
    private float _speed = 1;
    private Vector3 _origin, _target;
    [SerializeField]
    private Vector3 _difference;
    private RectTransform _rect;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _origin = _rect.position;
        _target = _origin + _difference;
    }

    public void Shift()
    {
        if (!_forward && _coroutine == null)
            _coroutine = StartCoroutine(Forward());
        else if (_forward && _coroutine == null)
            _coroutine = StartCoroutine(Backward());
    }

    IEnumerator Forward()
    {
        _t = 0;
        while (_t <= 1)
        {
            _rect.position = Vector3.Lerp(_origin, _target, _t);
            _t += Time.deltaTime * _speed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _rect.position = _target;
        _forward = true;
        _coroutine = null;
        yield return null;
    }

    IEnumerator Backward()
    {
        _t = 0;
        while (_t <= 1)
        {
            _rect.position = Vector3.Lerp(_target, _origin, _t);
            _t += Time.deltaTime * _speed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _rect.position = _origin;
        _forward = false;
        _coroutine = null;
        yield return null;
    }
}
