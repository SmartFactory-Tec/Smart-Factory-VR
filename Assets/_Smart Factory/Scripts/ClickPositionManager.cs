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
    public string ipAddress = "10.22.240.51"; // Dirección IP del dispositivo Modbus
    public int port = 2022;
    ModbusIpMaster modbusMaster;
    void Start(){
        GameObject objeto = GameObject.Find("ModBusSocket"); 
        script = objeto.GetComponent<ModBusSocket>();
        TcpClient tcpClient = new TcpClient(ipAddress, port);
        modbusMaster = ModbusIpMaster.CreateIp(tcpClient);

            // Escribir el valor en el registro Modbus
        

    }
    
    void Update()
    {

        if (Input.GetMouseButtonDown(0)){
            Vector3 clickPosition = -Vector3.one; 

            //clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3 (0,0,5f)); 

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            RaycastHit hit; 

            if (Physics.Raycast(ray, out hit)){
                clickPosition = hit.point; 
            }

            ushort x = (ushort)script.UnityFloatToModbus(clickPosition[0]); 
            ushort z = (ushort)script.UnityFloatToModbus(clickPosition[2]); 

            Debug.Log(x + "," + z); 
            modbusMaster.WriteMultipleRegisters(1, new ushort[] { x, z });
        }
    }

}


