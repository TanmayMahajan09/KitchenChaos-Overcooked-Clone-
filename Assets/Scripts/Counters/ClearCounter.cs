using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    

    
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //There is no KitchenObject
            if (player.HasKitchenObject()) {
                //Player Has Kitchen Object
                player.GetKitchenObject().SetKicthenObjectParent(this);
            }
        }
        else {
            //There is Kitchen Object
            if(player.HasKitchenObject()) {
                //Player is Carrying Something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //The Player is Carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().getKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else {
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().getKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf() ;
                        }
                    }
                }
            }
            else {
                //Player not Carrying Anything
                GetKitchenObject().SetKicthenObjectParent(player);
            }
        }

    }

    

}
