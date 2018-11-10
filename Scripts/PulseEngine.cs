/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using System;
using System.Runtime.InteropServices;

// C# class that wraps the PulseC API
public class PulseEngine
{
    public enum SerializationFormat : int   // State file serialization format
    {
        BINARY = 0,
        ASCII
    }

    readonly IntPtr pulseEngineCPtr;        // Pointer to the pulse engine in C

    [DllImport("PulseC")]
    static extern IntPtr Allocate(string logfile, string data_dir);
    PulseEngine() {} // make default constructor private
    public PulseEngine(string logFilePath, string data_dir)
    {
        pulseEngineCPtr = Allocate(logFilePath, data_dir);
    }

    [DllImport("PulseC")]
    static extern void Deallocate(IntPtr pulse);
    ~PulseEngine()
    {
        Deallocate(pulseEngineCPtr);
    }

    [DllImport("PulseC")]
    static extern bool SerializeFromFile(IntPtr pulse, string filename, string data_requests, int format, double sim_time_s);
    public bool SerializeFromFile(string filename, string data_requests, SerializationFormat format, double sim_time_s)
    {
        return SerializeFromFile(pulseEngineCPtr, filename, data_requests, (int)format, sim_time_s);
    }

    [DllImport("PulseC")]
    static extern bool SerializeToFile(IntPtr pulse, string filename, int format);
    public bool SerializeToFile(string filename, SerializationFormat format)
    {
        return SerializeToFile(pulseEngineCPtr, filename, (int)format);
    }

    [DllImport("PulseC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern bool SerializeFromString(IntPtr pulse, string state, string data_requests, int format, double sim_time_s);
    public bool SerializeFromString(string state, string data_requests, SerializationFormat format, double sim_time_s)
    {
        return SerializeFromString(pulseEngineCPtr, state, data_requests, (int)format, sim_time_s);
    }

    [DllImport("PulseC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern string SerializeToString(IntPtr pulse, int format);
    public string SerializeToString(SerializationFormat format)
    {
        return SerializeToString(pulseEngineCPtr, (int)format);
    }

    [DllImport("PulseC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern void InitializeEngine(IntPtr pulse, string patient_configuration, string data_requests, int format);
    public void InitializeEngine(string patient_configuration, string data_requests, SerializationFormat format)
    {
        InitializeEngine(pulseEngineCPtr, patient_configuration, data_requests, (int)format);
    }

    [DllImport("PulseC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern bool AdvanceTime_s(IntPtr pulse, double time);
    public bool AdvanceTime_s(double timeStep)
    {
        return AdvanceTime_s(pulseEngineCPtr, timeStep);
    }

    [DllImport("PulseC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern IntPtr PullData(IntPtr pulse);
    public IntPtr PullData()
    {
        return PullData(pulseEngineCPtr);
    }
}
