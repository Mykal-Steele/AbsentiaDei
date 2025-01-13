using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public Transform player;
    public Transform hand;
    public Transform mainCamera;
    public Transform objectToPickUp;
    // 0.77 -0.47 1.177557
    public float pickUpRadius = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(hand.position, objectToPickUp.position) <= pickUpRadius)
            {
                objectToPickUp.parent = mainCamera;

                // Set the position to the hand's position
                objectToPickUp.position = hand.position;

                // Set the local rotation to avoid inheriting the camera's rotation
                objectToPickUp.localRotation = Quaternion.Euler(-181.6f, 89.9f, -70.366f);

                Debug.Log("Object picked up!");
            }
        }
    }
}
