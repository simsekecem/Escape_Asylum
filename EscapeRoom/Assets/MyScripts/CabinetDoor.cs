using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(Rigidbody))]
public class Cupboard : MonoBehaviour
{
    [Header("Door Settings")]
    public float openVelocity = -220f;
    public float closeVelocity = 180f;
    public float motorForce = 5000f;

    private HingeJoint hinge;
    private bool isOpen = false;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();

        
        JointMotor motor = hinge.motor;
        motor.force = motorForce;
        motor.freeSpin = false;
        motor.targetVelocity = 0;
        hinge.motor = motor;
        hinge.useMotor = true;

        
        JointLimits limits = hinge.limits;
        limits.min = -90f;
        limits.max = 0f;
        limits.bounciness = 0f;
        hinge.limits = limits;
        hinge.useLimits = true;

        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Debug.Log("[Cupboard] Initialized");
    }

    void Update()
    {
        JointMotor motor = hinge.motor;

        
        if (isOpen)
        {
            if (hinge.angle > -85f)
            {
                motor.targetVelocity = openVelocity;
            }
            else
            {
                
                motor.targetVelocity = 0f;
            }
        }

        
        else
        {
            if (hinge.angle < -5f)
            {
                motor.targetVelocity = closeVelocity;
            }
            else
            {
                
                motor.targetVelocity = 0f;
            }
        }

        hinge.motor = motor;

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log(
                $"Angle:{hinge.angle:F1} | " +
                $"RotY:{transform.localEulerAngles.y:F1} | " +
                $"Open:{isOpen}"
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"Trigger: {other.name} | " +
            $"Tag:{other.tag}"
        );

        if (
            other.CompareTag("PlayerHand") ||
            other.transform.root.CompareTag("PlayerHand")
        )
        {
            Debug.Log("✅ EL ALGILANDI → Kapı Açılıyor");

            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            Debug.Log("→ OPEN");
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            Debug.Log("→ CLOSE");
        }
    }
}