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

    [SerializeField] ViveControllerScript freeMotion;

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
            PrintArrayInLine(newData);
            UpdateRobot(ParseDataToMissionData(newData));
            lastData = newData;
        }
        
        //Lerp robot to target
        robot.localPosition = Vector3.Lerp(robot.localPosition, targetPos, Time.deltaTime * 5);
        robot.localRotation = Quaternion.Lerp(robot.localRotation, targetRot, Time.deltaTime * 5);
    }

    [SerializeField] private Transform robot;
    [SerializeField] private Transform robot2;
    [SerializeField] private Transform targetDebug;

    [SerializeField] private Vector3 targetPos;
    [SerializeField] private Quaternion targetRot;
    
    private void UpdateRobot(MissionData missionData)
    {
        targetPos = new Vector3(missionData.currentPosition.x, 0, missionData.currentPosition.y);
        targetRot = Quaternion.Euler(new Vector3(0, missionData.angle, 0));
        robot2.localPosition = new Vector3(missionData.currentPosition2.x, 0, missionData.currentPosition2.y);
        robot2.eulerAngles = new Vector3(0, missionData.angle2, 0);
        
        Vector2 pos = ModbusPosToUnityVector2((ushort)missionData.goalPosition.x, (ushort)missionData.goalPosition.y);
        targetDebug.localPosition = new Vector3(pos.x, 0, pos.y);

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

        missionData.status = (MissionStatus)data[0];
        missionData.status2 = (MissionStatus)data[15];
        missionData.goalPosition = new Vector2Int(data[1], data[2]);
        missionData.angle = data[3];
        missionData.angle2 = data[19];

        missionData.battery = data[4];
        missionData.robotStatus = (RobotStatus)data[5];
        missionData.distance = data[6];

        missionData.currentPosition = ModbusPosToUnityVector2(data[7], data[8]);
        missionData.currentPosition2 = ModbusPosToUnityVector2(data[23], data[24]);
        Debug.Log($"Input: {data[7]}, {data[8]} | Output: {missionData.currentPosition.x}, {missionData.currentPosition.y}");

        missionData.goalPositionPi = new Vector2(data[9], data[10]);
        missionData.currentPositionPi = new Vector2(data[11], data[12]);
        missionData.plcCommunication = data[13];

        missionData.raw_data = data;

        return missionData;
    }

    private Vector2 ModbusPosToUnityVector2(ushort dataX, ushort dataY)
    {
        Vector2 pos = new Vector2(ModbusToUnityFloat(dataX), ModbusToUnityFloat(dataY));

        return pos - new Vector2(offsetx, offsety);
    }

    private float ModbusToUnityFloat(ushort data)
    {
        bool positive = ((int)(data / 100f)) == 10;
        int x = data % 1000 / 100;

        float digits = data % 100 / 100f;

        return positive ? 1 : -1 * (x + digits);
    }

    public Vector2Int UnityVector2ToModbus(Vector2 position)
    {
        float fx = position.x + offsetx;
        float fy = position.y + offsety;

        short x = UnityFloatToModbus(fx);
        short y = UnityFloatToModbus(fy);

        return new Vector2Int(x, y);
    }

    private short UnityFloatToModbus(float data)
    {
        Debug.Log("--------------------");
        Debug.Log("Input " + data);
        
        //Opposite of ModbusToUnityFloat
        string output = data >= 0 ? "10" : "11";
        Debug.Log(output);

        int integer = Mathf.Abs((int)Math.Truncate(data));
        
        output += integer.ToString("0");
        Debug.Log(output);
        
        float digits = Mathf.Abs(data) - integer;
        Debug.Log("Digits " + digits);
        Debug.Log(digits.ToString("0.00"));

        output += digits.ToString("0.00")[2..];
        
        Debug.Log(output);
        
        return short.Parse(output);
    }

    public void SetRobotTarget(Vector2 target)
    {
        ushort[] newData = lastData;

        Vector2Int targetModbus = UnityVector2ToModbus(target);

        newData[0] = (ushort)MissionStatus.Move;
        newData[1] = (ushort)targetModbus.x;
        newData[2] = (ushort)targetModbus.y;

        Debug.Log("Writing " + newData[1] + ", " + newData[2]);
        
        if (master == null)
        {
            Debug.LogError("Master is null");
            return;
        }
        master.WriteMultipleRegisters(startAddress, newData);
    }

    public void missions(ushort x, ushort y, ushort address)
    {
        ushort[] newData = lastData;

        newData[0 + address] = (ushort)MissionStatus.Move;
        newData[1 + address] = x;
        newData[2 + address] = y;


        if(master != null)
            master.WriteMultipleRegisters(startAddress, newData);
    }

    public void SetPredeterminedMission(int mission)
    {
        PredeterminedMission predMission = (PredeterminedMission)mission;

        switch (predMission)
        {
            case PredeterminedMission.Modula: //MODULA
                missions(10754, 10502, 0);
                break;
            case PredeterminedMission.ChargeStation: //CARGA
                missions(11071, 11138, 0);
                break;
            case PredeterminedMission.DeliverPackage: //Entregar paquete
                missions(10385, 10360, 0);
                break;
            case PredeterminedMission.PickPackage: //Sacar paquete
                missions(10342, 10208, 0);
                break;
            case PredeterminedMission.DeliverYummy: //Entregar yummy
                missions(10466, 10595, 0);
                break;
            case PredeterminedMission.FreeMovement: //Free Motion
                Debug.Log("Free Motion activated");
                freeMotion.useFreemovement = !freeMotion.useFreemovement;
                //freeMotion.Update();
                break;
            //case "RightArrow (0)": //MODULA ROBOT 2
            //    socket.missions(10754, 10502, 15);
            //    break;
            //case "RightArrow (1)": //CARGA
            //    socket.missions(11071, 11138, 15);
            //    break;
            //case "RightArrow (2)": //Entregar paquete
            //    socket.missions(10385, 10360, 15);
            //    break;
            //case "RightArrow (3)": //Sacar paquete
            //    socket.missions(10342, 10208, 15);
            //    break;
            //case "RightArrow (4)": //Entregar yummy
            //    socket.missions(10466, 10595, 15);
            //    break;
            //case "LeftArrow (5)": //Pick es del XARM y aún no está implementado
            //    socket.missions(10466, 10595, 0);
            //    break;
            default:
                break;

        }

        Debug.Log("Setting mission to " + predMission);
    }

    public enum PredeterminedMission
    {
        Modula,
        DeliverPackage,
        PickPackage,
        DeliverYummy,
        ChargeStation,
        FreeMovement
    }
}