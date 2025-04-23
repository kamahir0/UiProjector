using UnityEngine;

namespace UiProjector.Sandbox
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private bool _reverseMove;
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private bool _reverseRotation;

        private float MoveSpeed => _reverseMove ? -_moveSpeed : _moveSpeed;
        private float RotationSpeed => _reverseRotation ? -_rotationSpeed : _rotationSpeed;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleReset();
        }

        private void HandleMovement()
        {
            Vector3 move = Vector3.zero;

            // 水平方向
            if (Input.GetKey(KeyCode.W)) move += transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= transform.right;
            if (Input.GetKey(KeyCode.D)) move += transform.right;

            // 上下方向
            if (Input.GetKey(KeyCode.Q)) move -= transform.up;
            if (Input.GetKey(KeyCode.E)) move += transform.up;

            transform.position += move * MoveSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            float yRot = 0f;
            float xRot = 0f;

            if (Input.GetKey(KeyCode.LeftArrow)) yRot -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) yRot += 1f;
            if (Input.GetKey(KeyCode.UpArrow)) xRot -= 1f;
            if (Input.GetKey(KeyCode.DownArrow)) xRot += 1f;

            transform.Rotate(Vector3.up, yRot * RotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, xRot * RotationSpeed * Time.deltaTime, Space.Self);
        }

        private void HandleReset()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
            }
        }
    }
}
