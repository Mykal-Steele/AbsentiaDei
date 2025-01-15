using UnityEngine;
using System.Collections;

public class PlayerChopTree : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public Transform mainCamera; // Camera for attaching the axe
    public Animator playerAnimator; // Player's Animator
    public float teleportOffset = 1.5f; // Distance to place the player near the tree
    public GameObject logPrefab; // Prefab for the log object
    public Transform hand; // Player's hand position for holding the axe
    public Transform objectToPickUp; // Axe object
    public float pickUpRadius = 2f; // Radius within which the player can pick up the axe
    private bool hasAxe = false; // Whether the player has the axe
    private bool isChopping = false;

    void Update()
    {
        // Check if the player presses the E key to pick up the axe
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(hand.position, objectToPickUp.position) <= pickUpRadius && !hasAxe)
            {
                objectToPickUp.parent = mainCamera; // Attach to the camera

                // Position and rotation adjustments
                objectToPickUp.position = hand.position;
                objectToPickUp.localRotation = Quaternion.Euler(-181.6f, 89.9f, -70.366f);

                Debug.Log("Axe picked up!");
                hasAxe = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player has the axe and is interacting with a tree
        if (other.CompareTag("Tree") && hasAxe && !isChopping)
        {
            isChopping = true;
            Transform tree = other.transform;

            // Teleport player near the tree
            Vector3 chopPosition = tree.position - (tree.forward * teleportOffset);
            player.position = chopPosition;

            // Face the tree
            player.LookAt(tree);

            // Play chopping animation
            playerAnimator.SetTrigger("Chop");

            // Destroy the tree after the animation duration
            StartCoroutine(ChopTree(tree));
        }
    }

    private IEnumerator ChopTree(Transform tree)
    {
        //chopping animation
        // Change the seconds according to the animation length.
        yield return new WaitForSeconds(2f);

        // Destroy the tree
        Destroy(tree.gameObject);

        // Spawn a log at the tree's position
        Instantiate(logPrefab, tree.position, Quaternion.identity);

        isChopping = false;
    }
}
