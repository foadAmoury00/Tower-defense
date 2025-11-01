using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Mouse Movement Settings")]
    public float mouseSensitivity = 20f;
    public float smoothSpeed = 2f;
    
    [Header("Auto Zoom Settings")]
    public float zoomAmount = 0.1f;
    public float zoomSpeed = 1f;
    public bool enableAutoZoom = true;
    
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3 targetPosition;
    private Vector3 targetScale;
    private bool isMouseOver = false;
    private bool isAutoZooming = false;
    private float zoomDirection = 1f;
    
    void Start()
    {
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
        targetPosition = initialPosition;
        targetScale = initialScale;
    }

    void Update()
    {
        HandleMouseMovement();
        HandleAutoZoom();
        ApplyMovement();
    }
    
    private void HandleMouseMovement()
    {
        if (isMouseOver)
        {
            // Get mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;
            
            // Convert to normalized coordinates (-1 to 1)
            float normalizedX = (mousePosition.x / Screen.width) * 2f - 1f;
            float normalizedY = (mousePosition.y / Screen.height) * 2f - 1f;
            
            // Calculate target position (move opposite to mouse for parallax effect)
            Vector3 mouseOffset = new Vector3(-normalizedX, -normalizedY, 0) * mouseSensitivity;
            targetPosition = initialPosition + mouseOffset;
        }
        else
        {
            // Return to initial position when mouse is not over
            targetPosition = initialPosition;
        }
    }
    
    private void HandleAutoZoom()
    {
        if (!enableAutoZoom || isMouseOver) return;
        
        if (!isAutoZooming)
        {
            isAutoZooming = true;
            zoomDirection = 1f;
        }
        
        // Calculate zoom scale
        float zoomScale = initialScale.x + (Mathf.Sin(Time.time * zoomSpeed) * zoomAmount * zoomDirection);
        targetScale = new Vector3(zoomScale, zoomScale, initialScale.z);
    }
    
    private void ApplyMovement()
    {
        // Smoothly move to target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothSpeed * Time.deltaTime);
        
        // Smoothly scale to target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, smoothSpeed * Time.deltaTime);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        isAutoZooming = false;
        targetScale = initialScale; // Reset scale when mouse enters
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        // Auto zoom will resume in HandleAutoZoom()
    }
}
