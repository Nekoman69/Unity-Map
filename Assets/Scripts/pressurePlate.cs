using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressurePlate : MonoBehaviour
{
    public GameObject door;
    SkinnedMeshRenderer m_skinnedMeshRenderer;
    float m_blendShapeValue = 0f;
    string m_blendShapeName = "Opend";
    float m_prev_error = 0;
    float m_doorVelocity = 0;
    float m_targetValue = 0f;
    Collider m_doorCollider;

    //The Audio
    public AudioClip myAudioClip;
    private AudioSource audioSource;



    private void Awake()
    {
        m_skinnedMeshRenderer = door.GetComponent<SkinnedMeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_doorCollider = door.GetComponent<MeshCollider>();
        audioSource = door.GetComponent<AudioSource>();
        audioSource.clip = myAudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        m_doorVelocity += PID(m_targetValue, m_blendShapeValue);
        m_doorVelocity = Mathf.Clamp(m_doorVelocity, -1f, 1f);
        m_blendShapeValue += m_doorVelocity;
        m_blendShapeValue = Mathf.Clamp(m_blendShapeValue, 0f, 100f);
        if (m_blendShapeValue == 100 || m_blendShapeValue == 0)
        {
            audioSource.Stop();
        }
        m_skinnedMeshRenderer.SetBlendShapeWeight(m_skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(m_blendShapeName), m_blendShapeValue);

        if (m_blendShapeValue > 50)
        {
            m_doorCollider.enabled = false;
        }
        else
        {
            m_doorCollider.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("WPC LEFT PRESSURE PLATE");
        if (other.CompareTag("Weighted Pressure Cube"))
        {
            m_targetValue = 0f;
            audioSource.time = 7f;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("WPC ENTERED PRESSURE PLATE");
        if (other.CompareTag("Weighted Pressure Cube"))
        {
            m_targetValue = 100f;
            audioSource.time = 7f;
            audioSource.Play();
        }
    }


    //PID VALUES
    private float P;
    private float I;
    private float D;

    public float k__P = 1;
    public float k__I = 1;
    public float k__D = 1;

    float PID(float target_Value, float current_value)
    {
        //Debug.Log("target_value: "+target_Value);
        //Debug.Log("current_value: " + current_value);

        P = target_Value - current_value;
        //Debug.Log("P: "+P);
        I += P * Time.deltaTime;
        //Debug.Log("I: " + I);
        D = (P - m_prev_error) / Time.deltaTime;
        //Debug.Log("D: " + D);
        //Debug.Log("output: " + (k__P * P + k__I * I + k__D * D));

        // Reset I if the error becomes zero
        if (P == 0)
        {
            I = 0;
        }
        if (float.IsNaN(P))
        {
            P = 0;
        }
        if (float.IsNaN(I))
        {
            I = 0;
        }
        if (float.IsNaN(D))
        {
            D = 0;
        }
        //Disables I
        //I = 0;
        float output = (k__P * P + k__I * I + k__D * D);

        m_prev_error = P;
        return output;
    }

}