
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private Transform[] tireTransforms;
    [SerializeField] private Transform[] frontTireTransforms;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Rigidbody carRigidbody;

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CarSpecs carSpecs;

    [SerializeField] private AnimationCurve steerAnimCurve;
    [SerializeField] private AnimationCurve accelAnimCurve;

    [Category("Input Attributes")]
    private float steeringInput;
    private float accelInput;

    [Category("Global-Script-Attributes")]
    private int tiresCount = 0;
    private float remTime = 0f;
    private float tiresInGround = 0f;

    [Category("Script-Object-Refs.")]
    private CarSystem carSystem;
    private TimerSystem timerSystem;
    private LevelManager levelManager;

    [Category("Tags.")]
    private const string checkpointTag = "Checkpoint";
    private const string finishCheckpointTag = "Finish";

    [Category("Level-End-Panel-Texts")]
    private const string GameOver_Text = "Game Over!";
    private const string OutOfTime_Text = "Out Of Time!";
    private const string LevelComplete_Text = "Level Complete";

    private RaycastHit hitInfo;
    private bool isGameOver = false;
    private Coroutine GroundCheckCoroutine = null;

    [Category("Action Delegates")]
    public static Action<float> OnCheckpointReached;
    public Action<float, Transform, float> OnApplyForce;
    public Action<float> OnCarRotate;
    public Action<string> OnGameOver;

    public static GameplayController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isGameOver = false;
        tiresCount = tireTransforms.Length;
        gameOverPanel.SetActive(false);

        timerSystem = new TimerSystem();
        remTime = timerSystem.Timer;
        carSystem = new CarSystem(carRigidbody, carSpecs, frontTireTransforms, steeringAnimCurve: steerAnimCurve, accelAnimCurve: accelAnimCurve);
        levelManager = new LevelManager(gameOverText, gameOverPanel);

        PlayerInputAction playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();
        playerInputAction.Player.Jump.performed += Jump;
        playerInputAction.Player.Move.performed += Move;
    }

    public void Jump(InputAction.CallbackContext context)
    {
    }

    private void Move(InputAction.CallbackContext context)
    {
        Debug.Log($":: Move");
        Vector2 inputVec = context.ReadValue<Vector2>();
        steeringInput = inputVec.x;
        accelInput = inputVec.y;
    }

    /* Apply force to Car's rigidbody for Suspension(Y-Axis), 
       steering(X-Axis), and acceleration(Z-Axis) for each tire. */
    private void FixedUpdate()
    {
        tiresInGround = 0;
        for (int tireIndex = 0; tireIndex < tiresCount; tireIndex++)
        {
            if (Physics.Raycast(tireTransforms[tireIndex].position, -tireTransforms[tireIndex].up, out hitInfo, carSpecs.hitDist))
            {
                tiresInGround++;
                OnApplyForce?.Invoke(hitInfo.distance, tireTransforms[tireIndex], accelInput);
            }
        }

        CheckGameOverStates();
    }

    // Updates the timer for UI and Rotation of the wheels (transforms) according to input
    private void Update()
    {
        if (isGameOver || remTime < 1)
            return;

#if UNITY_EDITOR
        ProcessInputs();
#endif
        DisplayUITimer();
        OnCarRotate?.Invoke(steeringInput);
    }

    // Add additional time when a checkpoint's reached
    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
            return;

        string otherGOTag = other.gameObject.tag;
        if (otherGOTag == checkpointTag)
        {
            OnCheckpointReached?.Invoke(remTime);
            remTime = timerSystem.Timer;
        }
        else if (otherGOTag == finishCheckpointTag && remTime > 0)
        {
            EnableGameOverState(LevelComplete_Text);
        }
    }

    private void EnableGameOverState(string panelText)
    {
        OnGameOver?.Invoke(panelText);
        isGameOver = true;
    }

    private void DisplayUITimer()
    {
        if (remTime < 0) return;

        //remTime -= Time.deltaTime;
        timerText.text = ((int)remTime).ToString();
    }

    /* Editor movement inputs */
    private void ProcessInputs()
    {
        //accelInput = Input.GetAxis("Vertical");
        //steeringInput = Input.GetAxis("Horizontal");
    }

    private void CheckGameOverStates()
    {
        if (isGameOver)
            return;

        /* Check for Time run out */
        if (remTime < 1)
        {
            EnableGameOverState(OutOfTime_Text);
            return;
        }

        /* Check for Tires out of bounds (ground/platform) */
        if (tiresInGround < 3 && GroundCheckCoroutine == null)
            GroundCheckCoroutine = StartCoroutine(CheckTiresInGround());
    }

    /* Check if car/rigidbody is out of bounds/ground/platform */
    private IEnumerator CheckTiresInGround()
    {
        yield return new WaitForSeconds(3f);
        if (tiresInGround >= 3)
        {
            StopCoroutine();
        }

        /* Check for dealing with edge cases where 
           car would be moving through the edge of the platforms */
        yield return new WaitForSeconds(2f);

        if (tiresInGround < 3 && !isGameOver)
        {
            EnableGameOverState(GameOver_Text);
            StopCoroutine();
        }
    }

    private void StopCoroutine()
    {
        if (GroundCheckCoroutine != null)
        {
            StopCoroutine(GroundCheckCoroutine);
            GroundCheckCoroutine = null;
        }
    }

    /* Functions mapped to event triggers of associated buttons */
    public void MoveInput(float input) => accelInput = input;
    public void SteerInput(float input) => steeringInput = input;

    public void RestartLevel()
    {
        StopCoroutine();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
