using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{

    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnUpForce;
    public event EventHandler OnBeforeForce;

    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (Keyboard.current.upArrowKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(force * Time.deltaTime * transform.up);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            float turn = +100f;
            landerRigidbody2D.AddTorque(turn * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            float turn = -100f;
            landerRigidbody2D.AddTorque(turn * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on terrain!");
            return;
        }

        float softLandingVelocityMagnitude = 3f;
        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
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

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        int finalScore = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.getScoreMultiplier());

        Debug.Log($"Landing Angle Score: {landingAngleScore}");
        Debug.Log($"Landing Speed Score: {landingSpeedScore}");
        Debug.Log($"Landing Speed Score: {finalScore}");

    }

}
