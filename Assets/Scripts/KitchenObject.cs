using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;
    public KitchenObjectSO getKitchenObjectSO() {
        return kitchenObjectSO;
    }
    public void SetKicthenObjectParent(IKitchenObjectParent kicthenObjectParent) {
        if (this.kitchenObjectParent != null) {

            this.kitchenObjectParent.ClearKitchenObject();
        } 

        this.kitchenObjectParent = kicthenObjectParent;
        kicthenObjectParent.SetKitchenObject(this);
        transform.parent = kicthenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
       
    }

    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is  PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else {
            plateKitchenObject = null;
            return false;
        }
    }


    public static KitchenObject SpawnKicthenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            
        kitchenObject.SetKicthenObjectParent(kitchenObjectParent);
        return kitchenObject;

    }
}
