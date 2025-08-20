using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnUpForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinCollected;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public int onLandedScore;
    }

    [SerializeField] private float upwardForce = 700f;
    [SerializeField] private float leftTorque = 100f;
    [SerializeField] private float rightTorque = -100f;
    [SerializeField] private float fuelAmount = 10f;

    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if (fuelAmount <= 0)
        {
            Debug.Log("Fuel empty!");
            return;
        }

        if (Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            FuelConsumption();
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            landerRigidbody2D.AddForce(upwardForce * Time.deltaTime * transform.up);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            landerRigidbody2D.AddTorque(leftTorque * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            landerRigidbody2D.AddTorque(rightTorque * Time.deltaTime);
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

        float softLandingVelocityMagnitude = 4f;
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

        int finalScore = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log($"Landing Angle Score: {landingAngleScore}");
        Debug.Log($"Landing Speed Score: {landingSpeedScore}");
        Debug.Log($"Landing Speed Score: {finalScore}");

        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            onLandedScore = finalScore,
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuel = 10f;
            fuelAmount += addFuel;

            fuelPickup.DestroySelf();
        }

        if (other.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinCollected?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void FuelConsumption()
    {
        fuelAmount -= Time.deltaTime;
    }

}
