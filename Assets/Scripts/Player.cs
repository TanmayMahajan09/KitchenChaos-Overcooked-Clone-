using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent{

    public event EventHandler OnSomethingPicked;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
        

    }
    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one player");
        }
        Instance = this;
    }
    private void Update() {
        HandleMovement();
        HandleInteractions();

    }
    public bool IsWalking() {
        return isWalking;
    }


    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactDistance = 2f;

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);

                }
                

            }
            else {
                SetSelectedCounter(null);
            }


        }
        else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float movementDistance = moveSpeed * Time.deltaTime;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, movementDistance);

        if (!canMove) {
            //Cant move in move Dir

            Vector3 moveDirx = new Vector3(moveDir.x, 0, 0).normalized;

            canMove = (moveDir.x <-0.5f || moveDir.x >0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirx, movementDistance);
            if (canMove) {
                //Movement possible in the x axis 
                moveDir = moveDirx;
            }
            else {
                //movement not possible in x axis 
                Vector3 moveDirz = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirz, movementDistance);
                if (canMove) {
                    //Movement possible in z axis
                    moveDir = moveDirz;
                }
                else {
                    //Movement impossible, its a corner
                }

            }

        }

        if (canMove) {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        float rotateSpeed = 15f;

        if (moveDir != Vector3.zero) {
            isWalking = true;
        }
        else {
            isWalking = false;
        }
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if(kitchenObject != null) {
            OnSomethingPicked?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
