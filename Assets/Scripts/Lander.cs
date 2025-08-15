using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{

    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Keyboard.current.upArrowKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(force * Time.deltaTime * transform.up);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            float turn = +100f;
            landerRigidbody2D.AddTorque(turn * Time.deltaTime);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            float turn = -100f;
            landerRigidbody2D.AddTorque(turn * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float softLandingVelocityMagnitude = 3f;
        if (collision.relativeVelocity.magnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Landing too hard!");
            return; //No need to run the below
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up); // ((0, 1), (Where the object facing))
        float minDotVector = .90f;

        if (dotVector < minDotVector)
        {
            Debug.Log("Landing on too steep angle!");
            return;
        }

        Debug.Log("Successfully Landing!");

    }

}
