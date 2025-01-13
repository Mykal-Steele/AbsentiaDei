using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnDistance = 10f;  // Distance from the player
    public float fovAngle = 60f;  // Field of View angle
    public float detectionRange = 50f;  // How far the object can be detected
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        SpawnObject();
    }

    void Update()
    {
        // Check if the object is in the camera's field of view
        Vector3 objectDirection = objectToSpawn.transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, objectDirection);

        if (angle < fovAngle / 2f)
        {
            // Check if the object is in the camera's frustum
            if (IsObjectVisible())
            {
                objectToSpawn.SetActive(true);
            }
            else
            {
                objectToSpawn.SetActive(false);
            }
        }
        else
        {
            objectToSpawn.SetActive(false); // Hide if out of FOV
        }
    }

    void SpawnObject()
    {
        // Random angle within the field of view range but not directly in front of the player
        float spawnAngle = Random.Range(-fovAngle / 2f, fovAngle / 2f);
        // Create a direction based on the random angle around the player
        Quaternion rotation = Quaternion.Euler(0, spawnAngle, 0);
        Vector3 spawnDirection = rotation * Vector3.forward;

        // Position the object at spawnDistance in the calculated direction
        objectToSpawn.transform.position = playerCamera.transform.position + spawnDirection * spawnDistance;

        objectToSpawn.SetActive(false);  // Initially hide the object
    }

    bool IsObjectVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        return GeometryUtility.TestPlanesAABB(planes, objectToSpawn.GetComponent<Collider>().bounds);
    }
}
