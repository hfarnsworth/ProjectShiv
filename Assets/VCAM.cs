using System;
using Cinemachine;
using UnityEngine;

public class VCam : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxPlayerSpeed = 12f;
    [SerializeField] private float maxLookAhead = 7.2f;
    [SerializeField] private float smoothTime = 0.25f;

    private CinemachineFramingTransposer _transposer;
    private Vector3 _currentOffset;
    private Vector3 _velocity = Vector3.zero;
    private float _previousDir;

    // Start is called before the first frame update
    void Start()
    {
        var vCam = GetComponent<CinemachineVirtualCamera>();
        _transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        _currentOffset = _transposer.m_TrackedObjectOffset;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    private void LateUpdate() {
        
        float speed = Mathf.Abs(rb.velocity.x);
        float direction = MathF.Sign(rb.velocity.x);

        if (direction != 0 && direction != _previousDir) {
            _velocity = Vector3.zero;
        }
        _previousDir = direction;
        
        float t = Mathf.InverseLerp(0f, maxPlayerSpeed, speed);
        float targetX;
        float adjustedSmoothTime = smoothTime;
        if (speed < 10f) {
            targetX = 0f;
            adjustedSmoothTime = 1;
        } else {
            targetX = direction * maxLookAhead * t;
        }
        
        Vector3 targetOffset = new Vector3(
            targetX,
            _currentOffset.y,
            _currentOffset.z
        );

        _currentOffset = Vector3.SmoothDamp(
            _currentOffset,
            targetOffset,
            ref _velocity,
            adjustedSmoothTime
        );
        
        _transposer.m_TrackedObjectOffset = _currentOffset;
        Debug.Log(speed);
    }
}
