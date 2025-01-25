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
    [SerializeField] Camera playerCam;

    [Header("Projectiles")]
    [SerializeField] Transform firePoint; // end of the gun, this is childed to the object in the hierarchy (hopefully)
    [SerializeField] Transform furthestPoint; // used in position lerping to move a big bubble
    [SerializeField] GameObject bubbleProjectilePrefab; // generic prefab reference for instantiation purposes
    [SerializeField] SoapBubble currentBubble; // This is the one that gets instantiated and fired during a charge shot
    [SerializeField] float sizeIncreaseRate; // how much does the bubble increase in size while being charged. In Scale Units per Second
    [SerializeField] GameObject lightAttackPrefab;

    [Header("Ammo")]
    [SerializeField] float maxFuel;
    [SerializeField] float currentFuel;
    [SerializeField] float refuelRate;

    [Header("Input Stuff")]

    [SerializeField] float minChargeTime = 0.45f; // the time window between the mouse button being considered "pressed" vs "held"
    [SerializeField] float maxChargeTime = 5.45f; // after the mouse button has been pressed for this long, the bubble will stop getting bigger
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
            if(Input.GetMouseButtonDown(1)){
                Instantiate(bubbleProjectilePrefab, new Vector3(0,0,0), Quaternion.identity);
            }

            if(Input.GetMouseButton(0)){
                // Shitty hack time
                // On Button Down, start incrementing a timer
                // Then if the timer goes above a certain threshold, start charging a bubble up
                chargeTimer += Time.deltaTime;

                // Once sufficient time has passed, then a charge shot will begin
                if(chargeTimer > minChargeTime){

                    if(currentBubble == null){
                        GameObject newBubble = Instantiate(bubbleProjectilePrefab, firePoint.position, firePoint.transform.rotation);
                        currentBubble = newBubble.GetComponent<SoapBubble>();
                    }

                    else{
                        if(chargeTimer < maxChargeTime){
                            // increase bubble size
                            currentBubble.transform.localScale += new Vector3(1,1,1)*sizeIncreaseRate*Time.deltaTime;
                            // move bubble away from gun barrel accordingly to prevent clipping or other weird visual issues
                        } 
                        // Need to do this outside of the timer check otherwise we might leave the bubble behind after a max charge
                        float lerpval = (currentBubble.transform.localScale.x - 0.5f)/2.5f; // ooweee i love arbitrary numbers
                        currentBubble.transform.position = Vector3.Lerp(firePoint.position, furthestPoint.position, lerpval);
                    } 
                    
                }
            }
            else if(Input.GetMouseButtonUp(0)){
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
        Debug.Log("TODO Fired a Light Shot");
    }

    private void FireChargeShot(GameObject bubble)
    {
        // TODO Fire the bubble in such a direction that it goes to the center of the screen.
        Ray aimRay = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if(Physics.Raycast(aimRay, out RaycastHit hit, Mathf.Infinity)){
            Vector3 aimPoint = hit.point;
            Debug.DrawLine(currentBubble.transform.position, hit.point, Color.red, 5f);
            currentBubble.transform.LookAt(aimPoint);
        }
        currentBubble.OnGunRelease();
        currentBubble = null;
    }
}
