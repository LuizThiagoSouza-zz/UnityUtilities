using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Properties:")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private bool turnToDirection = true;
    [SerializeField] private float turnSmoothSpeed = 0.1f;
    [Header("Callbacks:")]
    [SerializeField] private BoolEvent onMovement;
    private CharacterController controller;
    private Vector3 direction;
    private Transform myTransform;
    private float targetAngle, turnSmoothVelocity;

    public float Speed => movementSpeed;
    public Vector3 Direction => direction;

    private void Start()
    {
        myTransform = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CalculateInputDirection();
        FaceToDirection();
        Move();
    }

    #region <--- PRIVATE METHODS --->

    private void CalculateInputDirection()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.z = Input.GetAxisRaw("Vertical");
    }

    private void FaceToDirection()
    {
        if (turnToDirection && direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            myTransform.rotation =
                Quaternion.Euler(0,
                Mathf.SmoothDampAngle(myTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothSpeed),
                0);
        }
    }

    private void Move()
    {
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * movementSpeed * Time.deltaTime);
            onMovement?.Invoke(true);
            return;
        }

        onMovement?.Invoke(false);
    }

    #endregion <--- PRIVATE METHODS --->
}

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }