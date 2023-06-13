using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using _Smart_Factory.Scripts;
using Modbus.Device;
using Modbus.Utility;

public class ClickPositionManager : MonoBehaviour
{
    //ModBusSocket coordinates; 
    // Update is called once per frame
    ModBusSocket script; 
    public string ipAddress = "10.22.240.51"; // Direccion IP del dispositivo Modbus
    public int port = 2022;
    ModbusIpMaster modbusMaster;
    private const float offsetx = 7.9115512f;
    private const float offsety = 2.50132282f;
    public Vector2 myVector; 
    public Camera mainCamera; 
    void Start(){
        GameObject objeto = GameObject.Find("ModBusSocket"); 
        script = objeto.GetComponent<ModBusSocket>();
        TcpClient tcpClient = new TcpClient(ipAddress, port);
        modbusMaster = ModbusIpMaster.CreateIp(tcpClient);
        mainCamera = Camera.main; 
            // Escribir el valor en el registro Modbus
        

    }
    
    void Update()
    {

        if (Input.GetMouseButtonDown(0)){
            //Vector3 clickPosition = -Vector3.one; 

            //clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3 (0,0,5f)); 
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position); 
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z); 
            Vector3 localPosition = transform.parent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(mousePosition)); 
            Debug.Log(localPosition); 
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            // RaycastHit hit; 

            // if (Physics.Raycast(ray, out hit)){
            //     if (hit.collider.gameObject == gameObject){
            //         Vector3 localPosition = transform.InverseTransformPoint(hit.point); 
            //         Debug.Log(localPosition); 
            //     }
            // }
            //     Vector3 clickPosition = hit.point; 
            //     Vector3 worldClickPosition = hit.transform.InverseTransformPoint(clickPosition); 
            //     Debug.Log(worldClickPosition.x +  ", " + worldClickPosition.y +  ", " + worldClickPosition.z); 
            // }
            // Vector3 mousePosition = Input.mousePosition; 
            // mousePosition.z = transform.position.z - mainCamera.transform.position.z; 
            // Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); 

            // Vector3 localPosition = transform.InverseTransformPoint(worldPosition); 

            // Debug.Log(localPosition); 
            //myVector = new Vector2(clickPosition[0], clickPosition[2]);
            //ushort x = (ushort)(script.UnityFloatToModbus(clickPosition[0]) + offsetx); 
            //ushort z = (ushort)(script.UnityFloatToModbus(clickPosition[2]) + offsety); 


            //Debug.Log(clickPosition.x +  ", " + clickPosition.y +  ", " + clickPosition.z); 
            //modbusMaster.WriteMultipleRegisters(1, new ushort[] { x, z });
        }
    }

}


