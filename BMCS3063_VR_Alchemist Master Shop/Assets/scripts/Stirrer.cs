using UnityEngine;

public class Stirrer : MonoBehaviour
{
    public float stirSpeedThreshold = 0.05f;
    private Vector3 lastPosition;

    public Cauldron currentCauldron;


    public float maxDistance = 2.0f;       
    private Vector3 homePosition;         
    private Quaternion homeRotation;        

    private bool isInsideLiquid = false;
    private Rigidbody rb;                    

    void Start()
    {
        homePosition = transform.position;
        homeRotation = transform.rotation;

        lastPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (currentCauldron == null)
        {
            currentCauldron = Object.FindFirstObjectByType<Cauldron>();
        }
    }

    void Update()
    {
        CheckBoundary();

        if (currentCauldron == null || !isInsideLiquid) return;

        float distance = Vector3.Distance(transform.position, lastPosition);

        if (distance > stirSpeedThreshold)
        {
            currentCauldron.AddStirProgress(distance);
        }

        lastPosition = transform.position;
    }

    private void CheckBoundary()
    {
        float distanceFromHome = Vector3.Distance(transform.position, homePosition);

        if (distanceFromHome > maxDistance)
        {
            ResetToHome();
        }
    }

    public void ResetToHome()
    {
 
        transform.position = homePosition;
        transform.rotation = homeRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        lastPosition = homePosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CauldronLiquid"))
        {
            isInsideLiquid = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CauldronLiquid"))
        {
            isInsideLiquid = false;
            
        }
    }

    // �����ڱ༭���ﻭ��һ��Ȧ������ JunYang �� JeffLim ���Է�Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? homePosition : transform.position;
        Gizmos.DrawWireSphere(center, maxDistance);
    }
}