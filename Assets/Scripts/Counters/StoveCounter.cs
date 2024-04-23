using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs: EventArgs {
        public State state;
    } 
    public enum State {
        idle,
        frying,
        fried,
        burned,
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private State state;

    private void Start() {
        state = State.idle;
    }

    private void Update() {
        if (HasKitchenObject()) {

            switch (state) {
                case State.idle:
                    break;
                case State.frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });
                    if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
                        
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKicthenObject(fryingRecipeSO.output, this);
                        state = State.fried;
                        fryingTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().getKitchenObjectSO());
                        burningTimer = 0f;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
                    
                    break;
                case State.fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    }) ;
                    if (burningTimer > burningRecipeSO.burningTimerMax) {
                       
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKicthenObject(burningRecipeSO.output, this);
                        state = State.burned;
                       
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized =  0f
                        });
                    }
                    break;
                case State.burned:
                    break;
            }
            
        }
        
    }
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //There is no KitchenObject
            if (player.HasKitchenObject()) {
                //Player Has Kitchen Object
                if (HasRecipeWithInput(player.GetKitchenObject().getKitchenObjectSO())) {
                    //Player Carrying Somethig That Can Be Fried
                    player.GetKitchenObject().SetKicthenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().getKitchenObjectSO());
                    state = State.frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                }
            }
        }
        else {
            //There is Kitchen Object
            if (player.HasKitchenObject()) {
                //Player is Carrying Something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //The Player is Carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().getKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        state = State.idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else {
                //Player not Carrying Anything
                GetKitchenObject().SetKicthenObjectParent(player);
                state = State.idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }

    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null) {
            return fryingRecipeSO.input;
        }
        else {
            return null;
        }

    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
            if (fryingRecipeSO.input == inputKitchenObjectSO) {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
            if (burningRecipeSO.input == inputKitchenObjectSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }





}
