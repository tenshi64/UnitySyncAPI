using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float Acceleration;

    [SerializeField]
    private float BreakForce;

    [SerializeField]
    private float MaxSpeed;

    [SerializeField]
    private float RotationSpeed;

    private float Velocity;
    private float Rotation;

    private Transform CarModel;
    private Transform FrontWheel1, FrontWheel2;
    private Rigidbody RigidBody;

    [SerializeField]
    private Cam CamScript;
    private AudioSource AudioSource;

    private NetworkObject NetworkObject;

    void Start()
    {
        NetworkObject = transform.parent.GetComponent<NetworkObject>();
        RigidBody = GetComponent<Rigidbody>();
        CarModel = transform.Find("Car Model");
        FrontWheel1 = transform.Find("Wheels").Find("Front Wheel1");
        FrontWheel2 = transform.Find("Wheels").Find("Front Wheel2");

        AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!NetworkObject.IsLocalPlayer)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(transform.Find("Map Camera").gameObject);
            enabled = false;
        }

        if(!transform.Find("Map Camera").gameObject.activeSelf && NetworkObject.IsLocalPlayer)
        {
            transform.Find("Map Camera").gameObject.SetActive(true);
        }

        if((transform.eulerAngles.z <= 50 && transform.eulerAngles.z >= -50) && (transform.eulerAngles.x <= 50 && transform.eulerAngles.x >= -50))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    Velocity += Acceleration * Time.deltaTime;
                    CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z-5), 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    Velocity -= BreakForce * Time.deltaTime;
                    CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z+5), 2 * Time.deltaTime);
                }
            }
            else
            {
                Velocity = Mathf.Lerp(Velocity, 0, 1 * Time.deltaTime);
                CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z), 5 * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    Rotation += -RotationSpeed * Time.deltaTime;

                    FrontWheel1.rotation = Quaternion.Lerp(FrontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 - 20, transform.eulerAngles.z), 5 * Time.deltaTime);
                    FrontWheel2.rotation = Quaternion.Lerp(FrontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 - 20, transform.eulerAngles.z), 5 * Time.deltaTime);

                    CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(CarModel.eulerAngles.x + (20 * (Velocity/MaxSpeed)), transform.eulerAngles.y + 90, (transform.eulerAngles.z) * (Velocity / MaxSpeed)), 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    Rotation += RotationSpeed * Time.deltaTime;

                    FrontWheel1.rotation = Quaternion.Lerp(FrontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 + 20, transform.eulerAngles.z), 5 * Time.deltaTime);
                    FrontWheel2.rotation = Quaternion.Lerp(FrontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 + 20, transform.eulerAngles.z), 5 * Time.deltaTime);

                    CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(CarModel.eulerAngles.x - (20 * (Velocity / MaxSpeed)), transform.eulerAngles.y + 90, (transform.eulerAngles.z) * (Velocity / MaxSpeed)), 2 * Time.deltaTime);
                }
            }
            else
            {
                Rotation = Mathf.Lerp(Rotation, 0, 2 * Time.deltaTime);

                FrontWheel1.rotation = Quaternion.Lerp(FrontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z), 5 * Time.deltaTime);
                FrontWheel2.rotation = Quaternion.Lerp(FrontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z), 5 * Time.deltaTime);
            }

            /*if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(new Vector3(0, -rotationSpeed * (velocity / maxSpeed) * Time.deltaTime, 0));
                    frontWheel1.rotation = Quaternion.Lerp(frontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 70, transform.eulerAngles.z), 5 * Time.deltaTime);
                    frontWheel2.rotation = Quaternion.Lerp(frontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 70, transform.eulerAngles.z), 5 * Time.deltaTime);

                    carModel.rotation = Quaternion.Lerp(carModel.rotation, Quaternion.Euler(carModel.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z + 20) *(velocity/maxSpeed)), 2 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(new Vector3(0, rotationSpeed * (velocity / maxSpeed) * Time.deltaTime, 0));
                    frontWheel1.rotation = Quaternion.Lerp(frontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 70, transform.eulerAngles.z), 5 * Time.deltaTime);
                    frontWheel2.rotation = Quaternion.Lerp(frontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 70, transform.eulerAngles.z), 5 * Time.deltaTime);

                    carModel.rotation = Quaternion.Lerp(carModel.rotation, Quaternion.Euler(carModel.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z - 20) * (velocity / maxSpeed)), 2 * Time.deltaTime);
                }
            }
            else
            {
                frontWheel1.rotation = Quaternion.Lerp(frontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), 5 * Time.deltaTime);
                frontWheel2.rotation = Quaternion.Lerp(frontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), 5 * Time.deltaTime);
            }*/

            if(Input.GetKey(KeyCode.Space))
            {
                Velocity = Mathf.Lerp(Velocity, 0, (BreakForce / 3) * Time.deltaTime);

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    /*if (Input.GetKey(KeyCode.A))
                    {
                        transform.Rotate(new Vector3(0, -rotationSpeed/2 * (velocity / maxSpeed) * Time.deltaTime, 0));

                        rigidBody.AddForce(-transform.right * 600 * Time.deltaTime, ForceMode.Impulse);

                        frontWheel1.rotation = Quaternion.Lerp(frontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 - 70, transform.eulerAngles.z), 5 * Time.deltaTime);
                        frontWheel2.rotation = Quaternion.Lerp(frontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 - 70, transform.eulerAngles.z), 5 * Time.deltaTime);

                        carModel.rotation = Quaternion.Lerp(carModel.rotation, Quaternion.Euler(carModel.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z + 20) * (velocity / maxSpeed)), 2 * Time.deltaTime);
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.Rotate(new Vector3(0, rotationSpeed/2 * (velocity / maxSpeed) * Time.deltaTime, 0));

                        rigidBody.AddForce(transform.right * 600 * Time.deltaTime, ForceMode.Impulse);

                        frontWheel1.rotation = Quaternion.Lerp(frontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 + 70, transform.eulerAngles.z), 5 * Time.deltaTime);
                        frontWheel2.rotation = Quaternion.Lerp(frontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90 + 70, transform.eulerAngles.z), 5 * Time.deltaTime);

                        carModel.rotation = Quaternion.Lerp(carModel.rotation, Quaternion.Euler(carModel.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z - 20) * (velocity / maxSpeed)), 2 * Time.deltaTime);
                    }*/
                }
                else
                {
                    FrontWheel1.rotation = Quaternion.Lerp(FrontWheel1.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z), 5 * Time.deltaTime);
                    FrontWheel2.rotation = Quaternion.Lerp(FrontWheel2.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z), 5 * Time.deltaTime);

                    CarModel.rotation = Quaternion.Lerp(CarModel.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z + 5), 2 * Time.deltaTime);
                }
            }
        }
        else
        {
            Velocity = Mathf.Lerp(Velocity, 0, 2 * Time.deltaTime);

            if(Input.GetKey(KeyCode.G))
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 90, 0);
            }
        }

        CamScript.distanceY = Mathf.Lerp(CamScript.distanceY, Mathf.Clamp((Velocity / MaxSpeed) * 3f, 0, 3), 2 * Time.deltaTime);

        Velocity = Mathf.Clamp(Velocity, -MaxSpeed/2, MaxSpeed);


        AudioSource.pitch = (Mathf.Abs(Velocity) / MaxSpeed) + 1.3f;

        Rotation = Mathf.Clamp(Rotation, -60, 60);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Rotation * (Velocity/MaxSpeed) * 2 * Time.deltaTime, transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        if (!NetworkObject.IsLocalPlayer) return;

        Vector3 _position = transform.position;
        _position += transform.forward * Velocity * Time.fixedDeltaTime;
        RigidBody.MovePosition(_position);
    }
}
