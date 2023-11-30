using UnityEngine;

public class gravityPath : MonoBehaviour
{
    public Transform startPoint; // Assign the starting point in the Inspector
    public Transform endPoint; // Assign the ending point in the Inspector
    public Vector3 rotationOffset = new Vector3(0,0,0);

    private float distance; // Variable to store the distance between the points

    private float newLerpPos = 0.5f;
    private float velocity = 0f;
    private GravityManager gm;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    //End of path audio
    private GameObject AudioSourceContainer;
    private AudioSource audioSource;

    private void Awake()
    {
        if (gameObject.GetComponent<AudioSource>())
        { 
        AudioSourceContainer = GameObject.Find("Gate Audio Container");
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = AudioSourceContainer.GetComponent<AudioSource>().clip;
        audioSource.Stop();
        }

        gm = GameObject.Find("Gravity Manager").GetComponent<GravityManager>();
    }

    // Function to rotate the object around its look vector by specified degrees
    void RotateAroundLookVector(Vector3 rotationDegrees)
    {
        // Get the current forward direction of the object
        Vector3 lookDirection = transform.forward;

        // Calculate the desired rotation based on the input degrees
        Quaternion additionalRotation = Quaternion.Euler(rotationDegrees);

        // Get the rotation based on the look direction and apply the additional rotation
        Quaternion newRotation = Quaternion.LookRotation(lookDirection) * additionalRotation;

        // Set the object's rotation to the new rotation
        transform.rotation = newRotation;
    }

    void Update()
    {
        gravity = gm.globalGravity;
        // Calculate the distance between the start and end points
        distance = Vector3.Distance(startPoint.position, endPoint.position);

        // Calculate the direction between the start and end points
        Vector3 direction = endPoint.position - startPoint.position;

        // Calculate the dot product between the gravity vector and the direction vector
        float gravityComponent = Vector3.Dot(gravity.normalized, direction.normalized);
        velocity += gravityComponent * gravity.magnitude * Time.deltaTime;

        newLerpPos += moveLerp(velocity * Time.deltaTime);

        // Clamp the lerp position between 0 and 1
        newLerpPos = Mathf.Clamp(newLerpPos, 0f, 1f);

        Debug.Log(velocity);
        // Reset velocity when reaching the ends
        if (newLerpPos == 0 || newLerpPos == 1)
        {
            if (audioSource && Mathf.Abs(velocity) > 1)
            {
                audioSource.time = 0.15f;
                audioSource.Play();
            }
            velocity = 0;
        }


        // Set the cube's position to a point on the line between the start and end points
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, newLerpPos);

        // Rotate the cube to align with the line between the two points
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        RotateAroundLookVector(rotationOffset);
    }

    public float moveLerp(float meters)
    {
        return (meters / distance);
    }
}