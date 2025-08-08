using EmpyrionScripting.Interface;
using EmpyrionScripting.Internal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Eleon.Modding;

public class CoreFinder
{
    public static void Main(IScriptModData r)
    {
        var CsRoot = r.CsRoot;
        var P = r.P;
        var E = r.E;

        // Find all CoreScan LCD devices
        var coreScanLcds = CsRoot.GetDevices<ILcd>(CsRoot.Devices(E.S, "CoreScan=*"));

        if (coreScanLcds == null || coreScanLcds.Length == 0)
        {
            //Console.WriteLine("No CoreScan LCD devices found");
            return;
        }

        WriteTo(coreScanLcds, $"{DateTime.Now:HH:mm:ss} Core Scan Results:");


        // ===================================================
        var root = CsRoot.Root.P as IScriptRootData; // in game I get IScriptRootData could not be found
        var allEntities = CsRoot.Root.GetEntities();
        WriteTo(coreScanLcds, $"Scanning {allEntities.Count()} entities...");

        var coreDeviceName = "Core";
        var maxDistance = 2000f;
        var targetEntityTypes = new[] {
            EntityType.BA,
            EntityType.CV,
            EntityType.SV,
            EntityType.HV,
            EntityType.EnemyDrone,
            EntityType.TroopTransport,
            EntityType.Proxy
        };

        if (allEntities == null || allEntities.Count() == 0)
        {
            WriteTo(coreScanLcds, "No entities found");
            return;
        }

        var nearbyEntities = allEntities
            .Where(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position) <= maxDistance)
            .OrderBy(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position));

        var coreName = "NPC Core";
        var coresFound = 0;

        foreach (var entity in nearbyEntities)
        {
            var distance = UnityEngine.Vector3.Distance(E.Pos, entity.Position);
            var corePosList = entity.Structure.GetDevicePositions(coreName);

            if (corePosList == null || corePosList.Count == 0)
            {
                // Try alternative core names
                corePosList = entity.Structure.GetDevicePositions("Core");
            }

            if (corePosList != null && corePosList.Count > 0)
            {
                coresFound++;
                WriteTo(coreScanLcds, $" {entity.Name} ({entity.Type}) - Δ{distance:F0}m");

                foreach (var corePos in corePosList)
                {
                    var core = entity.Structure.GetBlock(corePos);
                    if (core != null)
                    {
                        core.Get(out int coreBlockType, out _, out _, out _);
                        var coreName_display = string.IsNullOrEmpty(core.CustomName) ? "Unnamed Core" : core.CustomName;
                        WriteTo(coreScanLcds, $"  Core: {coreName_display} (Type: {coreBlockType})");
                    }
                }
            }
        }

        if (coresFound == 0)
        {
            WriteTo(coreScanLcds, "No cores detected in range");
        }
        else
        {
            WriteTo(coreScanLcds, $"Found {coresFound} entities with cores\n\n");
        }

        WriteTo(coreScanLcds, $"Scan completed at {DateTime.Now:HH:mm:ss}");

        // Also log to console for debugging
        Console.WriteLine("CoreFinder script completed - output sent to LCD devices");
        // ===================================================
    }

    private static void WriteTo(ILcd[] lcds, string text)
    {
        lcds.ForEach(L => L.SetText($"{text}\n{L.GetText()}"));
    }
}