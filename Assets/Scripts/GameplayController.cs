
using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using TMPro;
using UnityEngine;
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
    public Action<float> OnCheckpointReached;
    public Action<float, Transform, float> OnApplyForce;
    public Action<float> OnCarRotate;

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
        carSystem = new CarSystem(carRigidbody, carSpecs, frontTireTransforms);
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
        string otherGOTag = other.gameObject.tag;
        if (otherGOTag == checkpointTag)
        {
            OnCheckpointReached?.Invoke(remTime);
            remTime = timerSystem.Timer;
        }
        else if (otherGOTag == finishCheckpointTag && remTime > 0)
            LevelEndPanel(LevelComplete_Text);
    }

    private void DisplayUITimer()
    {
        if (remTime < 0) return;

        remTime -= Time.deltaTime;
        timerText.text = ((int)remTime).ToString();
    }

    /* Editor movement inputs */
    private void ProcessInputs()
    {
        accelInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
    }

    /*
     * Apply force to Car's rigidbody for Suspension(Y-Axis), 
     * steering(X-Axis), and acceleration(Z-Axis) for each tire.
     */
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

    private void CheckGameOverStates()
    {
        /* Check for Time run out */
        if (remTime < 1)
        {
            LevelEndPanel(OutOfTime_Text);
            return;
        }

        /* Check for Tires out of bounds (ground/platform) */
        if (tiresInGround < 3 && !isGameOver && GroundCheckCoroutine == null)
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

        /* Second delay is for dealing with edge cases where 
           car would be moving through the edge of the platforms */
        yield return new WaitForSeconds(2f);

        if (tiresInGround < 3)
        {
            LevelEndPanel(GameOver_Text);
            StopCoroutine();
        }
    }

    /*
     * Activate GameOver Panel when car has:
     * fallen off, 
     * or when level's complete 
     * or when time has run out
     */
    private void LevelEndPanel(string text)
    {
        if (isGameOver) return;

        gameOverText.text = text;
        gameOverPanel.SetActive(true);
        isGameOver = true;
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
