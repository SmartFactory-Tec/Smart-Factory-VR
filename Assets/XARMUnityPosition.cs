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
public class XARMUnityPosition : MonoBehaviour
{
    public string ip = "10.22.240.51";
    public int port = 2022;
    
    const ushort startAddress = 0;
    const ushort numRegisters = 43;
    // Start is called before the first frame update
    ModbusIpMaster modbusClient; 
    void Start()
    {
                // Get the initial position of the object
        Vector3 objectPosition = transform.localPosition;
        
        // Display the position in the console
        Debug.Log("Object position: " + objectPosition);
        TcpClient client = new TcpClient(ip, port);
        modbusClient = ModbusIpMaster.CreateIp(client);
    }

    // Update is called once per frame
    void Update()
    {
         float objectPositionX = transform.localPosition.x;
         float objectPositionZ = transform.localPosition.z;
         fixData(objectPositionX, objectPositionZ); 
         // Display the position in the console
        //Debug.Log("Object position in x: " + objectPositionX);
        //Debug.Log("Object position in z: " + objectPositionZ);
    }

    private const float theta = 0.0523599f;
    private const float offsetx = 7.9115512f;
    private const float offsety = 2.50132282f;
    private const float rheight = 11.29203556f;
    private const float rwidth = 17.69995916f;
    private const int screen_x = 500; 
    private const int screen_y = 500; 

    void fixData(float X, float Z){
        
        // float firstx=(X % 1000);
        // float firstz=(Z % 1000); 
        // if (firstx == 10){
        //     firstx=(X-firstx*1000);
        // }   
        // else{
        //     firstx=-(X-firstx*1000);
        // }

        // if (firstz == 10){
        //     firstz=(Z-firstz*1000);
        // }   
        // else{
        //     firstz=-(Z-firstz*1000);
        // }
        
        // firstz=-(Z-firstz*1000); 
        // float goalx = firstx/100.0f;  //#Goalx
        // float goalz = firstz/100.0f;  //#Goaly
        // ushort localx=(ushort)(mapping(goalx +offsetx-0.5f,screen_x,rwidth));
        // ushort localz=(ushort)(mapping(goalz +offsety-0.5f,screen_y,rheight));
        ushort localx = (ushort)UnityToModbus(X);
        ushort localz = (ushort)UnityToModbus(Z);
        
        Debug.Log("Object position in x: " + localx);
        Debug.Log("Object position in z: " + localz);

        modbusClient.WriteMultipleRegisters(1, new ushort[] { localx, localz });
    }

    private float UnityToModbus(float data)
    {
        if (data > 0){
            return 10000 + data * 1000f + offsetx; 
        }
        else {
            return 11000 + data * 1000f + offsetx; 
        }
    }  
}
