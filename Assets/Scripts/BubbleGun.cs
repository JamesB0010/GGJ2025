using System;
using System.Collections;
using System.Collections.Generic;
using EvolveGames;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleGun : MonoBehaviour
{
    [Header("Player Stuff")]
    [SerializeField] ItemChange playerLoadout; // remember to set this

    [Header("Projectiles")]
    [SerializeField] Transform firePoint; // end of the gun, this is childed to the object in the hierarchy (hopefully)
    [SerializeField] GameObject bubbleProjectilePrefab; // generic prefab reference for instantiation purposes
    SoapBubble currentBubble; // This is the one that gets instantiated and fired during a charge shot
    [SerializeField] float sizeIncreaseRate; // how much does the bubble increase in size while being charged. In Scale Units per Second
    [SerializeField] GameObject lightAttackPrefab;

    [Header("Ammo")]
    [SerializeField] float maxFuel;
    [SerializeField] float currentFuel;
    [SerializeField] float refuelRate;

    [Header("Input Stuff")]

    [SerializeField] float minChargeTime; // the time window between the mouse button being considered "pressed" vs "held"
    [SerializeField] float maxChargeTime; // after the mouse button has been pressed for this long, the bubble will stop getting bigger
    [SerializeField] float chargeTimer;


    // Start is called before the first frame update
    void Start()
    {
        if(playerLoadout == null){
            Debug.Log("Hey! No reference to player loadout in the bubble gun!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // this fucking sucks but change this item ID to whatever the bubble gun is assigned to.
        if(playerLoadout.ItemIdInt == 0){
            
            if(Input.GetMouseButton(0)){
                // Shitty hack time
                // On Button Down, start incrementing a timer
                // Then if the timer goes above a certain threshold, start charging a bubble up
                chargeTimer += Time.deltaTime;
                if(chargeTimer > minChargeTime){
                    if(currentBubble != null){
                        GameObject newBubble = Instantiate(bubbleProjectilePrefab, firePoint.position, Quaternion.identity);
                        newBubble.TryGetComponent<SoapBubble>(out currentBubble);
                    }
                    else if(chargeTimer < maxChargeTime){
                        // increase bubble size
                        currentBubble.transform.localScale += new Vector3(1,1,1)*sizeIncreaseRate*Time.deltaTime;
                        // move bubble away from gun barrel accordingly to prevent clipping or other weird visuals
                        // it would be so cool if i could easily move an object's pivot point
                    }
                }
            }
            if(Input.GetMouseButtonUp(0)){
                Debug.Log($"Shot Fired after {chargeTimer} seconds"); // oooh look at me i can use string interpolation
                if(chargeTimer > minChargeTime){
                    FireChargeShot(currentBubble.gameObject);
                }
                else{
                    FireLightShot();
                }
                chargeTimer = 0; // reset this after shooting
            }
        }
    }

    private void FireLightShot()
    {
        throw new NotImplementedException();
    }

    private void BeginChargeShot()
    {
        throw new NotImplementedException();
    }

    private void FireChargeShot(GameObject bubble)
    {
        // do this eventually
        throw new NotImplementedException();
    }
}
