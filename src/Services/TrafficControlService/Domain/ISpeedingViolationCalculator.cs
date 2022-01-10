using System;

namespace TrafficControlService.Domain
{
    public interface ISpeedingViolationCalculator
    {
        int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
        string GetRoadId();
    }
}
