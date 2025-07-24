using EmpyrionScripting.Interface;
using System;
using System.Linq;

namespace EmpyrionScripting.Scripts
{
    public class WIP1
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
    var allB = CsRoot.GetAllBlockPositions(iE.S);
    foreach (var cBV in allB)
    {
        var blockData = CsRoot.Block(iE.S, cBV.x, cBV.y, cBV.z);
        CsRoot.WithLockedDevice(iE.S, blockData, () =>
        {
            var sourceCon = iE.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(cBV);
            if (sourceCon == null) return;
            var cB = iE.GetCurrent().Structure.GetBlock(cBV);
            var content = sourceCon?.GetContent();
            ic += content.Count;

            foreach (var item in content)
            {
                Console.WriteLine($"  Container Looting: Item:[{item.id}] {item.count} {CsRoot.I18n(item.id)}");
                //destCon.AddItems(item.id, item.count);
                //sourceCon.Clear();
                //cB.Set(0);
            }
        }, () =>
        {
            Console.WriteLine($"  Container locking failed on block cBV:[{cBV}]");
        });
    }
}
if (ic == 0) Console.WriteLine($"  Container Looting finished");
else Console.WriteLine($"  Container looting in progress");
//========================================= 
        }
    }
}
