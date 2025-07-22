using EmpyrionScripting.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionScripting.Scripts
{
    public class ModMain
    {
        public static void Main(IScriptModData r)
        {

            var CsRoot = r.CsRoot;

            //=========================================

            Console.WriteLine(DateTime.Now.ToString() + " ");

            var entityName = "test";
            var allEntities = CsRoot.Root.GetEntities();

            if (allEntities == null)
            {
                Console.WriteLine($"  {DateTime.Now.ToString()} Entity '{entityName}' not found.");
                return;
            } else if (allEntities.Count() > 0)
            {
                
                // loop through all entities
                foreach (var entity in allEntities)
                {
                    Console.WriteLine($"  {DateTime.Now.ToString()} Entity: {entity.Name} - Id: {entity.Id}");
                }
                return;
            }




            // ========================================
            var fuelItem = r.CsRoot.Items(r.E.S, "FuelBox")
                .FirstOrDefault(I => I.Id == 4314);

            r.Console.Write(DateTime.Now.ToString() + " ");
            r.Console.Write("Sync:" +
            $"{r.ScriptWithinMainThread}/" +
            $"{r.ScriptNeedsMainThread} ");

            if (fuelItem == null)
            {
                r.Console.WriteLine("no fuel cell found");
                return;
            }

            r.Console.Write("#");
            var miiArray = r.CsRoot.Fill(fuelItem, r.E.S,
            StructureTankType.Fuel, 100);
            r.Console.Write("#");

            if (miiArray == null)
            {
                r.Console.WriteLine("no fuel transferd");
                return;
            }

            var mii = miiArray.FirstOrDefault();

            if (mii == null)
            {
                r.Console.WriteLine("no fuel transfer");
                return;
            }

            r.Console.WriteLine("transfer");

            r.Console.WriteLine(
            $"#{miiArray.Count()} \n" +
            $"Count: {mii.Count} \n" +
            $"Destination: {mii.Destination} \n" +
            $"DestinationE: {mii.DestinationE} \n" +
            $"Id: {mii.Id} \n" +
            $"Source: {mii.Source} \n" +
            $"SourceE: {mii.SourceE} \n" +
            $"Error: {mii.Error} \n" +
            "");
        }
    }
}
