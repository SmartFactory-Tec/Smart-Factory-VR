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

public class ModBusSocket : MonoBehaviour
{
    public string ip = "10.22.240.51";
    public int port = 2022;

    private ModbusIpMaster master;
    
    const ushort startAddress = 0;
    const ushort numRegisters = 43;
    
    public void Awake()
    {
        //Using NModBus4 library connect to ModBus TCP
        TcpClient client = new TcpClient(ip, port);
        master = ModbusIpMaster.CreateIp(client);
    }

    private ushort[] lastData = new ushort[numRegisters];
    
    private void Update()
    {  
        ushort[] newData = master.ReadHoldingRegisters(startAddress, numRegisters);

        if (lastData.SequenceEqual(newData) == false)
        {
            UpdateRobot(ParseDataToMissionData(newData));
            PrintArrayInLine(newData);
            lastData = newData;
        }
    }

    [SerializeField] private Transform robot;
    
    private void UpdateRobot(MissionData missionData)
    {
        robot.localPosition = new Vector3(missionData.currentPosition.x, 0, missionData.currentPosition.y);
        robot.eulerAngles = new Vector3(0, missionData.angle,0);
    }
    
    private void PrintArrayInLine(ushort[] array)
    {
        string line = "[";
        foreach (ushort item in array)
        {
            line += item + ", ";
        }
        line += "]";
        Debug.Log(line);
    }
    
    private const float theta = 0.0523599f;
    private const float offsetx = 7.9115512f;
    private const float offsety = 2.50132282f;
    private const float rheight = 11.29203556f;
    private const float rwidth = 17.69995916f;
    
    private MissionData ParseDataToMissionData(ushort[] data)
    {
        float c1 = Mathf.Cos(theta);
        float s = Mathf.Sin(theta);
        
        MissionData missionData = new MissionData();

        missionData.status = (MissionStatus) data[0];
        
        missionData.goalPosition = new Vector2(data[1], data[2]);
        missionData.angle = data[3];
        
        missionData.battery = data[4];
        missionData.robotStatus = (RobotStatus) data[5];
        missionData.distance = data[6];

        //Get two first digits of data[7] and data[8]
        bool positiveX = (data[7] / 100) == 10;
        bool positiveY = (data[8] / 100) == 10;

        //Get 3rd digit of data[7] and data[8]
        int x = data[7] % 1000 / 100;
        int y = data[8] % 1000 / 100;
        
        Debug.Log("X: " + x + " Y: " + y);

        //Get 4th and 5th digit of data[7] and data[8]
        float xDigits = data[7] % 100 / 100f;
        float yDigits = data[8] % 100 / 100f;
        
        Debug.Log("XDig: " + xDigits + " YDig: " + yDigits);

        float finalPosX = positiveX ? 1 : -1 * (x + xDigits) - offsetx;
        float finalPosY = positiveY ? 1 : -1 * (y + yDigits) - offsety;

        missionData.currentPosition = new Vector2(finalPosX, finalPosY);
        
        Debug.Log(missionData.currentPosition);
        
        missionData.goalPositionPi = new Vector2(data[9], data[10]);
        missionData.currentPositionPi = new Vector2(data[11], data[12]);
        missionData.plcCommunication = data[13];
        
        missionData.raw_data = data;
        
        return missionData;
    }
    
}
