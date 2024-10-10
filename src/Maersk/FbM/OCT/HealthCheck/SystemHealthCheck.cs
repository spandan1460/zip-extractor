using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace Maersk.FbM.OCT.HealthCheck;

/// <summary>
/// Health checks for a micro-service, SHOULD:
/// 1. Check the internal system health - memory, and disk for example to ensure neither are running low
/// 2. Verify that the service has not been sending too many HTTP 5xx errors as a reply.
///
/// Health checks should NOT:
/// 1. Check the health of their dependencies, that is what alarms are for, not for what a health check intends to provide.
///    If a database resource is unhealthy, an alarm should exist for the database to tell you that.
///
///    If a micro-service health check checks the health of its dependencies, then when the dependencies go down, so
///    too does the microservice.
/// 
/// 2. Verify its own metrics from a client perspective, again this is what SLO's and alarms should indicate.
///
/// The purpose of a health check is to enable the ability of a control-plane that maintains such services to take out
/// of rotation one of the instances that becomes unhealthy and replace it with an instance that is stable/healthy.
///
/// If your application had a memory leak for example, it would be ideal if that control-plane could remove nodes that
/// started to run out of memory in advance of when the instance terminated.
///
/// A failing health check might be the result of an infrastructure problem, or an internal coding issue - it should not
/// be diagnostics on the health of the system but instead on the health of instance. 
/// </summary>
public class SystemHealthCheck : IHealthCheck
{

    private readonly Logger _logger = NLog.LogManager.Setup().GetCurrentClassLogger();
    private static float MIN_FREE_DISKSPACE = 0.10f;
    private static float MAX_MEMORY = 0.98f;
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.Info($"CheckHealthAsync Start: context={context.Registration.Name}");

        try
        {
            HealthStatus status = HealthStatus.Healthy;

            // Check to ensure the memory pressure is lower than 98%
            Process currentProcess = Process.GetCurrentProcess();
            long used = currentProcess.PrivateMemorySize64;
            var gcInfo = GC.GetGCMemoryInfo();
            var available = gcInfo.TotalAvailableMemoryBytes;
            var ratio = used / available;
            bool atMemoryPressureThreshold = (ratio > MAX_MEMORY);
            
            _logger.Info($"Memory ratio={ratio}, available={available}, used={used}, at threshold={atMemoryPressureThreshold}");
            if (atMemoryPressureThreshold)
            {
                _logger.Error("Setting health to unhealthy due to memory pressure");
                status = HealthStatus.Unhealthy;
            }
            Dictionary<string, object> volumeData = new Dictionary<string, object>();
            status = worstHealth(status, localDiskSpaceHealth(volumeData));
            
            // Supplies the data used to make the decision around the health of the service.
            IReadOnlyDictionary<string, object> data = new Dictionary<string, object>(volumeData)
            {
                { "memoryRatio", ratio.ToString() },
                { "memoryUsed", used.ToString() },
                { "memoryAvailable", available.ToString() }
            };
            
            return Task.FromResult(new HealthCheckResult(status, status.ToString(), null, data));
        }
        catch (Exception e)
        {
            _logger.Error("Failed to capture system health due to exception: ", e);
            return Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, 
                "Degraded - exception throw" + e.Message, e));
        }
    }

    /// <summary>
    /// Provide a reply with the worst case health.  If the two health a = Unhealthy, b = Degraded the return is Unhealthy.
    /// If a = Degraded, b = Healthy, return = Degraded, etc. 
    /// </summary>
    /// <param name="a">First health status</param>
    /// <param name="b">Second health status</param>
    /// <returns>Worst case health status either a or b.</returns>
    private HealthStatus worstHealth(HealthStatus a, HealthStatus b)
    {
        if (a == HealthStatus.Unhealthy || b == HealthStatus.Unhealthy)
            return HealthStatus.Unhealthy;
        if (a == HealthStatus.Degraded || b == HealthStatus.Degraded)
            return HealthStatus.Degraded;
        return HealthStatus.Healthy;
    }

    /// <summary>
    /// Determines if the health check status for disk space should return healthy
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public HealthStatus localDiskSpaceHealth(Dictionary<string, object> data)
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        foreach (DriveInfo d in allDrives)
        {
            if (d.IsReady)
            {
                string volumeLabel = d.VolumeLabel;
                // Ignore labels that contain filesystems that do not have data
                if (volumeLabel.StartsWith("/System/") || volumeLabel.StartsWith("/proc")) continue;
                
                if (d.TotalFreeSpace > 0) // avoid filesystems with no attached space and don't divide by zero
                {
                    float availableFreeSpace = d.AvailableFreeSpace / d.TotalFreeSpace;
                    data.Add(d.VolumeLabel, availableFreeSpace.ToString());
                    _logger.Debug($"Filesystem check, volume={volumeLabel}, freeSpace={availableFreeSpace}");
                    if (availableFreeSpace < MIN_FREE_DISKSPACE)
                    {
                        _logger.Error($"Available free space of volume={volumeLabel} is less than expected minimum={MIN_FREE_DISKSPACE} current={availableFreeSpace}");
                        return HealthStatus.Unhealthy;
                    }
                }
                else
                {
                    // filesystem had no space to begin with, debug log these
                    _logger.Debug($"volume={volumeLabel} freeSpace={d.TotalFreeSpace}, available={d.AvailableFreeSpace}");
                }
            }
        }

        return HealthStatus.Healthy;
    }
}