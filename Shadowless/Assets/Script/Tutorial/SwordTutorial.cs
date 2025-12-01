using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Enhanced tutorial with animations and visual feedback for sword attack prompt
/// Uses new Input System
/// </summary>
public class SwordTutorial : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image keyIcon; // Image showing the key to press
    [SerializeField] private Image swordIcon; // Optional sword icon
    [SerializeField] private CanvasGroup canvasGroup; // For fade animations
    
    [Header("Input Settings")]
    [SerializeField] private InputActionReference attackAction; // Assign your attack action
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite leftCtrlKeySprite; // Assign a key icon sprite
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    
    [Header("Animation Settings")]
    [SerializeField] private bool useFadeIn = true;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private bool usePulseAnimation = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.2f;
    
    [Header("Gameplay Settings")]
    [SerializeField] private string tutorialMessage = "Attack to unleash your blade!";
    [SerializeField] private bool requireMultipleAttacks = false;
    [SerializeField] private int requiredAttacks = 3;
    [SerializeField] private bool pauseGameDuringTutorial = false;
    
    private bool tutorialActive = false;
    private int attackCount = 0;
    private float fadeTimer = 0f;
    private Vector3 originalKeyScale;

    void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
            
        if (keyIcon != null)
            originalKeyScale = keyIcon.transform.localScale;
            
        // Subscribe to attack input
        if (attackAction != null && attackAction.action != null)
        {
            attackAction.action.performed += OnAttackInput;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (attackAction != null && attackAction.action != null)
        {
            attackAction.action.performed -= OnAttackInput;
        }
    }

    void Update()
    {
        if (!tutorialActive)
            return;

        // Handle fade-in animation
        if (useFadeIn && fadeTimer < fadeInDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeInDuration);
                if (fadeTimer >= fadeInDuration)
                {
                    Debug.Log("Fade-in complete! CanvasGroup alpha: " + canvasGroup.alpha);
                }
            }
        }

        // Pulse animation for key icon
        if (usePulseAnimation && keyIcon != null)
        {
            float pulse = 1f + (Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.5f + 0.5f) * (pulseScale - 1f);
            keyIcon.transform.localScale = originalKeyScale * pulse;
        }
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (!tutorialActive)
            return;

        attackCount++;
        
        // Visual feedback on press
        OnAttackPressed();
        
        if (requireMultipleAttacks && attackCount < requiredAttacks)
        {
            UpdateTutorialText($"{tutorialMessage}\n({attackCount}/{requiredAttacks} attacks)");
        }
        else
        {
            CompleteTutorial();
        }
    }

    /// <summary>
    /// Call this when the player picks up the sword
    /// </summary>
    public void StartTutorial()
    {
        tutorialActive = true;
        attackCount = 0;
        fadeTimer = 0f;
        
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
        
        // Set up canvas group for fade (or make it visible immediately)
        if (canvasGroup != null)
        {
            if (useFadeIn)
            {
                canvasGroup.alpha = 0f; // Start invisible for fade-in
            }
            else
            {
                canvasGroup.alpha = 1f; // Make it visible immediately
            }
        }
        
        // Set up key icon
        if (keyIcon != null)
        {
            if (leftCtrlKeySprite != null)
                keyIcon.sprite = leftCtrlKeySprite;
            keyIcon.color = normalColor;
        }
        
        // Optional: Pause the game
        if (pauseGameDuringTutorial)
            Time.timeScale = 0f;
        
        UpdateTutorialText(tutorialMessage);
    }

    private void UpdateTutorialText(string message)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
        }
    }

    private void OnAttackPressed()
    {
        // Flash the key icon
        if (keyIcon != null)
        {
            StartCoroutine(FlashKey());
        }
        
        // Shake or bounce effect
        if (tutorialPanel != null)
        {
            StartCoroutine(BouncePanel());
        }
    }

    private System.Collections.IEnumerator FlashKey()
    {
        if (keyIcon != null)
        {
            keyIcon.color = highlightColor;
            yield return new WaitForSecondsRealtime(0.1f);
            keyIcon.color = normalColor;
        }
    }

    private System.Collections.IEnumerator BouncePanel()
    {
        Vector3 originalScale = tutorialPanel.transform.localScale;
        float bounceTime = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < bounceTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float scale = 1f + Mathf.Sin((elapsed / bounceTime) * Mathf.PI) * 0.1f;
            tutorialPanel.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        tutorialPanel.transform.localScale = originalScale;
    }

    private void CompleteTutorial()
    {
        tutorialActive = false;
        
        // Fade out animation
        StartCoroutine(FadeOutAndClose());
        
        // Resume game if it was paused
        if (pauseGameDuringTutorial)
            Time.timeScale = 1f;
        
        Debug.Log("Sword tutorial completed!");
    }

    private System.Collections.IEnumerator FadeOutAndClose()
    {
        float fadeOutTime = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < fadeOutTime && canvasGroup != null)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            yield return null;
        }
        
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }
}
