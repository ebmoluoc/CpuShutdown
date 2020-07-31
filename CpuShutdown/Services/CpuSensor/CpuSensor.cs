using OpenHardwareMonitorLib.Hardware;
using System;

namespace CpuShutdown.Services.CpuSensor
{

    public sealed class CpuSensor : ICpuSensor, IDisposable
    {

        private const string _Name = "CPU Package";

        private readonly Computer _computer = new Computer { CPUEnabled = true };
        private readonly IHardware _cpu;


        public CpuSensor()
        {
            _computer.Open();
            _cpu = _computer.Hardware[0];
        }


        public void Dispose()
        {
            _computer.Close();
        }


        public int ReadTemperature()
        {
            _cpu.Update();

            foreach (var sensor in _cpu.Sensors)
                if (sensor.SensorType == SensorType.Temperature)
                    if (string.Equals(sensor.Name, _Name))
                        return (int)sensor.Value;

            throw new InvalidOperationException($"No sensor containing the temperature of \"{_Name}\" was found");
        }

    }

}
