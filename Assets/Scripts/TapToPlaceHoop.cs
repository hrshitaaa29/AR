using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneOldInputSystem : MonoBehaviour
{
    [SerializeField]
    GameObject hoopPrefab; // Prefab of the basketball hoop

    [SerializeField]
    GameObject basketballPrefab; // Prefab of the basketball

    GameObject placedHoop; // The instantiated hoop object
    GameObject spawnedBasketball; // The instantiated basketball object

    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        if (arRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (placedHoop == null)
            {
                // Instantiate hoop prefab
                placedHoop = Instantiate(hoopPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                // Move the hoop to touch position
                placedHoop.transform.position = hitPose.position;
                placedHoop.transform.rotation = hitPose.rotation;
            }

            // Check if hoop is placed and basketball is not spawned yet
            if (placedHoop != null && spawnedBasketball == null)
            {
                // Spawn basketball prefab slightly above the hoop
                spawnedBasketball = Instantiate(basketballPrefab, placedHoop.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            }

            // Make the hoop always look at the camera
            Vector3 lookPos = Camera.main.transform.position - placedHoop.transform.position;
            lookPos.y = 0;
            placedHoop.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}
