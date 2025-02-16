using UnityEngine;
using UnityEngine.Events;

public partial class Main
{
    public UnityAction<Vector2> TouchPressed;
    public UnityAction<Vector2> TouchHold;
    public UnityAction<Vector2> TouchRelease;
    public UnityAction<bool> OnInputStatusChanged;
    private Vector3 touchPosition;
    private bool isEnabled = true;
    private Camera camera;
    public Vector3 TouchPosition => touchPosition;
    public bool IsEnabled => isEnabled;
    
    private void InitializeInput()
    {
        Input.multiTouchEnabled = false;
        camera = Camera.main;
    }

    private void HandleInput()
    {
        touchPosition = GetTouchPosition(Input.mousePosition);
        
        if (!isEnabled)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            TouchPressed?.Invoke(touchPosition);
        }
            
        if (Input.GetMouseButton(0))
        {
            TouchHold?.Invoke(touchPosition);
        } 
            
        if (Input.GetMouseButtonUp(0))
        {
            TouchRelease?.Invoke(touchPosition);
        }
    }
    
    private Vector3 GetTouchPosition(Vector2 pixelCoordinates)
    {
        return camera.ScreenToWorldPoint(pixelCoordinates);
    }
    
    public void SetInputEnable(bool enabled)
    {
        if (isEnabled != enabled)
        {
            OnInputStatusChanged?.Invoke(isEnabled);
        }
        isEnabled = enabled;
    }

    public void SetEnableWithoutCallBack(bool enabled)
    {
        isEnabled = enabled;
    }
}