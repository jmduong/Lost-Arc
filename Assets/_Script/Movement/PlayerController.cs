using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float rotationSpeed = 4.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField]
    private GraphicRaycaster canvasRaycaster;
    private PointerEventData eventData;
    private List<RaycastResult> results;

    private InputManager inputManager;
    private CharacterController controller;
    private Vector2 initScreenPosition;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;
    private Coroutine movRoutine;

    public Finger finger;

    private void OnEnable()
    {
        inputManager.OnStartTouch += OnTouchPress;
        inputManager.OnEndTouch += OnTouchRelease;
    }
    private void OnDisable()
    {
        inputManager.OnStartTouch -= OnTouchPress;
        inputManager.OnEndTouch -= OnTouchRelease;
    }

    private void Awake()
    {
        inputManager = InputManager.Instance;
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
        eventData = new PointerEventData(EventSystem.current);
        results = new List<RaycastResult>();
        //mousePress.action.started += ctx => OnMousePress(ctx);
        //mousePress.action.canceled += ctx => OnMouseRelease(ctx);
    }
    /*
    private void OnMousePress(InputAction.CallbackContext context)
    {
        if(mouseScreen.action.ReadValue<Vector2>().x <= Screen.width / 2)
        {
            initScreenPosition = mouseScreen.action.ReadValue<Vector2>();
            if (movRoutine != null)
                StopCoroutine(movRoutine);
            movRoutine = StartCoroutine(Move());
        }
    }
    private void OnMouseRelease(InputAction.CallbackContext context)
    {
        if (movRoutine != null)
            StopCoroutine(movRoutine);
    }*/
    
    private void OnTouchPress(Vector2 screenPosition)
    {
        if (screenPosition.x <= Screen.width / 2 && vcam.Priority == 0)
        {
            finger = inputManager.finger;
            initScreenPosition = screenPosition;
            if (movRoutine != null)
                StopCoroutine(movRoutine);
            movRoutine = StartCoroutine(TouchMove());
        }
    }

    private void OnTouchRelease(Vector2 screenPosition)
    {
        if (movRoutine != null)
            StopCoroutine(movRoutine);
        finger = null;
    }
    /*
    IEnumerator Move()
    {
        while(mouseScreen.action.ReadValue<Vector2>().x <= Screen.width / 2)
        {
            Debug.Log("Moving");

            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector2 movement = mouseScreen.action.ReadValue<Vector2>() - initScreenPosition;
            movement.x = movement.x >= 0 ? Mathf.Min(movement.x / 100, 1) : Mathf.Max(movement.x / 100, -1);
            movement.y = movement.y >= 0 ? Mathf.Min(movement.y / 100, 1) : Mathf.Max(movement.y / 100, -1);
            Vector3 move = new Vector3(movement.x, 0, movement.y);
            move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
            move.y = 0;
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player..
            if (jumpControl.action.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (movement != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    */
    IEnumerator TouchMove()
    {
        while (finger.screenPosition.x <= Screen.width / 2)
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector2 movement = finger.screenPosition - initScreenPosition;
            movement.x = movement.x >= 0 ? Mathf.Min(movement.x / 100, 1) : Mathf.Max(movement.x / 100, -1);
            movement.y = movement.y >= 0 ? Mathf.Min(movement.y / 100, 1) : Mathf.Max(movement.y / 100, -1);
            Vector3 move = new Vector3(movement.x, 0, movement.y);
            move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
            move.y = 0;
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player..
            /*if (jumpControl.action.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }*/

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (movement != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private bool IsPointerOverUIObject()
    {
        eventData.position = finger.screenPosition;
        results.Clear();
        canvasRaycaster.Raycast(eventData, results);
        return results.Count > 0 ? true : false;
    }
}
