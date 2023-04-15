using UnityEngine;
using UnityEngine.Events;

public class TriggerButton : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent pressEvent;
    [SerializeField] private UnityEvent unpressEvent;
 
    [Header("Hover Events")]
    [SerializeField] private UnityEvent hoverEvent;
    [SerializeField] private UnityEvent unhoverEvent;
    
    private Renderer _renderer;

    [Header("Materials")] 
    [SerializeField] private Material pressedMaterial;
    [SerializeField] private Material regularMaterial;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (hoverEvent != null && unhoverEvent != null)
        {
            GameObject hover = new GameObject("Hover");
            hover.transform.SetParent(transform);
            hover.transform.localPosition = Vector3.zero;
            hover.transform.localRotation = Quaternion.identity;

            BoxCollider hoverCollider = hover.AddComponent<BoxCollider>();
            hoverCollider.isTrigger = true;
            hoverCollider.size = new Vector3(4f/100f, 2f/100f, 3.58f/100f);
            hoverCollider.center = new Vector3(-0.01f, 0, 0);

            //hover.AddComponent<HoverButton>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name != "C1") return;
        
        //OVRInput.SetControllerVibration(1,1,OVRInput.Controller.RTouch);
        
        pressEvent?.Invoke();

        _renderer.material = pressedMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name != "C1") return;

        //OVRInput.SetControllerVibration(1,0,OVRInput.Controller.RTouch);
        
        unpressEvent?.Invoke();
        
        _renderer.material = regularMaterial;
    }

    public void SetActive(bool active)
    {
        _renderer.material = active ? pressedMaterial : regularMaterial;
    }

    public void SetHover(bool newState)
    {
        if (newState)
        {
            hoverEvent?.Invoke();
            return;
        }

        unhoverEvent?.Invoke();
    }
}
