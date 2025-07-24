using EmpyrionScripting.Interface;
using System;
using System.Linq;

namespace EmpyrionScripting.Scripts
{
public class WIP
{
public static void Main(IScriptModData r)
{
var CsRoot = r.CsRoot;
var P = r.P;
var E = r.E;
// Note script length should not exceed 1000 characters
//POI Container Looter =========================================
Console.WriteLine($"{DateTime.Now} Looter ScriptId[{CsRoot.Root.ScriptId}] Running");
var eF = "Destruct-*";
var destF = "1-Dump1";
var destPos = CsRoot.Devices(E.S, destF)?.FirstOrDefault()?.Position;
if (destPos == null) return;
var destCon = E.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(destPos.Value);
var allE = CsRoot.EntitiesByName(eF);
if (allE == null || allE.Count() == 0) return;
int ic = 0;
foreach (var iE in allE)
{
    var allC = iE.GetCurrent().Structure.GetDevices(DeviceTypeName.Container);
    if (allC == null || allC.Count == 0) continue;

    for (int i = 0; i < allC.Count; i++)
    {
        var sourceVec = allC.GetAt(i);
        var sourceCon = iE.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(sourceVec);
        var sourceBlock = iE.GetCurrent().Structure.GetBlock(sourceVec);
        var content = sourceCon.GetContent();
        ic += content.Count;
        foreach (var item in content)
        {
            destCon.AddItems(item.id, item.count);
            sourceCon.Clear();
            sourceBlock.Set(0);
        }
    }
}
if (ic == 0) Console.WriteLine($"  Container Looting finished");
else Console.WriteLine($"  Container looting in progress");
//========================================= 
        }
    }
}



