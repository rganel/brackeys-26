using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MaxUpAngle;
    [SerializeField] private float MaxDownAngle;
    [SerializeField] private float MinMouseInputThreshold;
    [SerializeField] private Vector2 RotationFactor;
    [SerializeField] private Vector2 MovementFactor;

    private float m_yaw;
    private float m_pitch;
    private InputAction m_lookAction;
    private InputAction m_moveAction;
    private Rigidbody m_rigidbody;
    private Camera m_camera;

    private void Awake()
    {
        m_lookAction = InputSystem.actions.FindAction("Look");
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_rigidbody = GetComponent<Rigidbody>();
        m_camera = Camera.main;

        Debug.Assert(MaxUpAngle > 0.0f);
        Debug.Assert(MaxDownAngle > 0.0f);
        Debug.Assert(MinMouseInputThreshold > 0.0f);
        Debug.Assert(RotationFactor.magnitude > 0.0f);
        Debug.Assert(MovementFactor.magnitude > 0.0f);
        
        Debug.Assert(m_lookAction != null);
        Debug.Assert(m_moveAction != null);
        Debug.Assert(m_rigidbody != null);
        Debug.Assert(m_camera != null);
    }

    private void OnEnable()
    {
        m_lookAction.Enable();
        m_moveAction.Enable();
    }

    private void OnDisable()
    {
        m_lookAction.Disable();
        m_moveAction.Disable();
    }

    private void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta.magnitude < MinMouseInputThreshold)
        {
            return;
        }

        m_yaw = (m_yaw + (mouseDelta.x * 2.0f * RotationFactor.x / Screen.width)) % 360.0f;
        m_pitch = Mathf.Clamp(m_pitch - mouseDelta.y * 2.0f * RotationFactor.y / Screen.height, -MaxUpAngle, MaxDownAngle);
    }

    private void FixedUpdate()
    {
        Quaternion forwardDirection = Quaternion.Euler(Vector3.up * m_yaw);
        
        Vector2 moveDelta = m_moveAction.ReadValue<Vector2>().normalized;
        Vector3 targetPosition = m_rigidbody.position + forwardDirection * new Vector3(moveDelta.x, 0.0f, moveDelta.y) * (MovementFactor.magnitude * Time.fixedDeltaTime);

        m_rigidbody.Move(targetPosition, forwardDirection);
        m_camera.transform.localRotation = Quaternion.Euler(Vector3.right * m_pitch);
    }
}