using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR;

public class VRButton : XRBaseInteractable
{
    [Header("Renderer")]
    [SerializeField] private Renderer buttonRenderer;

    [Space]
    [Header("Materials")]
    [SerializeField] private Material regularMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material pressedMaterial;
    [SerializeField] private Material Rojomaterial;
    ModBusSocket socket;
    private void Start()
    {

        GameObject objeto = GameObject.Find("ModBusSocket");
        socket = objeto.GetComponent<ModBusSocket>();
    }
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
        Debug.Log(socket);
        buttonRenderer.material = Rojomaterial;

        switch (gameObject.name)
        {
            case "LeftArrow (0)": //MODULA
                socket.missions(10754, 10502, 0);
                break;
            case "LeftArrow (6)": //CARGA
                socket.missions(11071, 11138, 0);
                break;
            case "LeftArrow (2)": //Entregar paquete
                socket.missions(10385, 10360, 0);
                break;
            case "LeftArrow (3)": //Sacar paquete
                socket.missions(10342, 10208, 0);
                break;
            case "LeftArrow (4)": //Entregar yummy
                socket.missions(10466, 10595, 0);
                break;
            case "RightArrow (0)": //MODULA ROBOT 2
                socket.missions(10754, 10502, 15);
                break;
            case "RightArrow (1)": //CARGA
                socket.missions(11071, 11138, 15);
                break;
            case "RightArrow (2)": //Entregar paquete
                socket.missions(10385, 10360, 15);
                break;
            case "RightArrow (3)": //Sacar paquete
                socket.missions(10342, 10208, 15);
                break;
            case "RightArrow (4)": //Entregar yummy
                socket.missions(10466, 10595, 15);
                break;
            //case "LeftArrow (5)": //Pick es del XARM y aún no está implementado
            //    socket.missions(10466, 10595, 0);
            //    break;
            default:
                break;

        }



    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (interactorsSelecting.Count != 0) return;

        buttonRenderer.material = regularMaterial;
    }
}
