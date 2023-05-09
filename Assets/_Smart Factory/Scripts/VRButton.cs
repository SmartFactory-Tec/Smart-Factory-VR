using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButton : XRBaseInteractable
{
    [Header("Renderer")]
    [SerializeField] private Renderer buttonRenderer;
    
    [Space]
    [Header("Materials")]
    [SerializeField] private Material regularMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material pressedMaterial;


    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        
        if (interactorsHovering.Count != 1) return;
        
        buttonRenderer.material = hoverMaterial;
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        
        if (interactorsHovering.Count != 0) return;
        
        buttonRenderer.material = regularMaterial;
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        if (interactorsSelecting.Count != 1) return;
        
        buttonRenderer.material = pressedMaterial;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        if (interactorsSelecting.Count != 0) return;
        
        buttonRenderer.material = regularMaterial;
    }
}
