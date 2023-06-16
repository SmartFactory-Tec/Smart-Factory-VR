﻿

using UnityEngine;

namespace _Smart_Factory.Scripts
{
    public struct MissionData
    {
        public MissionStatus status;
        public MissionStatus status2;
        public Vector2Int goalPosition;
        public float angle;
        public float battery;
        public RobotStatus robotStatus;
        public float distance;
        public Vector2 currentPosition;
        public Vector2 currentPosition2;
        public float angle2;
        
        public Vector2 goalPositionPi;
        public Vector2 currentPositionPi;

        public float plcCommunication;

        public ushort[] raw_data;
    }

    public enum MissionStatus
    {
        Charge,
        Move,
        Free,
    }

    public enum RobotStatus
    {
        Charging,
        Moving,
        Free,
        Success,
        Failure
    }
}