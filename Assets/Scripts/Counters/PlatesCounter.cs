using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlatesCounter : BaseCounter {

    public event EventHandler OnPlateSpwaned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] KitchenObjectSO plateKitchenObjectSO;
    private float plateTimer;
    private float plateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedMax = 4;

    private void Update() {
        plateTimer += Time.deltaTime;
        if (plateTimer > plateTimerMax) {
            plateTimer = 0f;
            if (platesSpawnedAmount < platesSpawnedMax) {
                platesSpawnedAmount += 1; 
                OnPlateSpwaned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            //player has no object
            if(platesSpawnedAmount> 0) {
                //Theres atleast one plate there
                platesSpawnedAmount--;
                KitchenObject.SpawnKicthenObject(plateKitchenObjectSO, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }



}