using System;
using System.Collections.Generic;
using System.Linq;

namespace Incapsulation.Failures
{
    public class ReportMaker
    {
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes,
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var date = new DateTime(year, month, day);
            var failures = new List<Failure>();
            for (var i = 0; i < devices.Count; i++)
            {
                var device = new Device((int)devices[i]["DeviceId"], devices[i]["Name"] as string);
                for (var j = 0; j < failureTypes.Length; j++)
                {
                    if (device.Id == deviceId[j])
                    {
                        var failDate = new DateTime(
                            (int)times[i][2],
                            (int)times[i][1],
                            (int)times[i][0]);
                        failures.Add(new Failure((FailureType)failureTypes[i], failDate, device));
                    }
                }
            }

            return FindDevicesFailedBeforeDate(date, failures);
        }

        public static List<string> FindDevicesFailedBeforeDate(DateTime date, List<Failure> failures)
        {
            var result = new List<string>();
            foreach (var failure in failures)
            {
                if (failure.IsSerious() && failure.Date < date)
                {
                    result.Add(failure.Device.Name);
                }
            }

            return result;
        }
    }

    public class Device
    {
        public readonly int Id;
        public readonly string Name;

        public Device(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Failure
    {
        public readonly FailureType Type;
        public readonly DateTime Date;
        public readonly Device Device;

        public Failure(FailureType type, DateTime date, Device device)
        {
            Type = type;
            Date = date;
            Device = device;
        }

        public bool IsSerious()
        {
            return Type == FailureType.UnexpectedShutdown
                || Type == FailureType.HardwareFailures;
        }
    }

    public enum FailureType
    {
        UnexpectedShutdown,
        ShortNonResponding,
        HardwareFailures,
        ConnectionProblems,
    }
}
