using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            HelicopterSound.pitch = Mathf.Clamp(value / 0.2f, 0, 30.0f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;

    public InputAction dragonflyControls;
    public Vector3 movementValue = Vector3.zero; 

    private void OnEnable()
    {
        dragonflyControls.Enable(); 
    }

    private void OnDisable()
    {
        dragonflyControls.Disable();
    }

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
       // HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void Update()
    {
        movementValue = dragonflyControls.ReadValue<Vector3>();
        Movement(); 
    }

    void Movement() {
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = -Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
            tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
            tempX = Time.fixedDeltaTime;

        if (movementValue.y > .65f)
        {
            EngineForce += 0.8f;
        }

        if (movementValue.y < .65f)
        {
            EngineForce -= 0.8f;
        }

        if (movementValue.x < .06f) //Forward
        {
            if (IsOnGround) return;
            tempY = Time.fixedDeltaTime;
        }

        if (movementValue.x > .06f) //Forward
        {
            if (IsOnGround) return;
            tempY = -Time.fixedDeltaTime;
        }

        if (movementValue.z < 0)
        {
            if (IsOnGround) return;
            tempX = -Time.fixedDeltaTime;
            var force = (turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
            HelicopterModel.AddRelativeTorque(0f, force, 0);
        }

        if (movementValue.z > 0)
        {
            if (IsOnGround) return;
            tempX = Time.fixedDeltaTime;
            var force = -(turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
            HelicopterModel.AddRelativeTorque(0f, force, 0);
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
} 