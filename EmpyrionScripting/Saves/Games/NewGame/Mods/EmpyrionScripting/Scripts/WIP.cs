using EmpyrionScripting.Interface;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

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





            var destBlock = CsRoot.Block(E.S, -1, 128, 0);
            var targetBlock = CsRoot.Block(E.S, -1, 129, 0);
            Console.WriteLine($"Block:{destBlock.BlockType} ");
            destBlock.BlockType = 542;
            var destCon = E.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(destBlock.Position);

            destCon.SetPropertyDefaultValues();
            int blockType = destBlock.BlockType;
            if (CsRoot.Root.ConfigEcfAccess.ConfigBlockById.TryGetValue(blockType, out EcfParser.EcfBlock blockConfig))
            {
                if (blockConfig.EcfValues.TryGetValue("LootList", out var lootListAttr))
                {
                    // Try to get the loot container config by its Container Id
                    //if (CsRoot.Root.ConfigEcfAccess.ConfigContainerById.TryGetValue((int)lootListAttr.Value, out var lootContainerConfig))
                    //{
                    //    // lootContainerConfig.EcfValues contains the loot container's properties
                    //    foreach (var kvp in lootContainerConfig.Childs)
                    //    {
                    //        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine($"No loot container config found for Container Id {lootListAttr.Value}");
                    //}
                }
                else
                {
                    Console.WriteLine("No LootList property found for this block.");
                }
            }




            //// lootListAttr.Value is a string, e.g., "39"
            //if (int.TryParse(lootListAttr.Value, out int lootListId))
            //{
            //    // Try to get the loot container config by its Container Id
            //    if (CsRoot.Root.ConfigEcfAccess.FlatConfigBlockById.TryGetValue(lootListId, out var lootContainerConfig))
            //    {
            //        // lootContainerConfig.EcfValues contains the loot container's properties
            //        foreach (var kvp in lootContainerConfig.EcfValues)
            //        {
            //            Console.WriteLine($"{kvp.Key}: {kvp.Value.Value}");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"No loot container config found for Container Id {lootListId}");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine($"Invalid LootList number: {lootListAttr.Value}");
            //}



            //var destBlock = CsRoot.Block(E.S, -1, 128, 0);
            //var targetBlock = CsRoot.Block(E.S, -1, 129, 0);


            //Console.WriteLine($"Block:{destBlock.BlockType} ");
            //destBlock.BlockType = 542;
            ////destBlock.ChangeBlockType(543);
            //var destCon = E.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(destBlock.Position);

            //destCon.SetPropertyDefaultValues();


            //// Given a block type or block ID, look it up in your config mapping
            //int blockType = destBlock.BlockType;
            //if (CsRoot.Root.ConfigEcfAccess.ConfigBlockById.TryGetValue(blockType, out EcfParser.EcfBlock blockConfig))
            //{
            //    if (blockConfig.EcfValues.TryGetValue("LootList", out var lootListAttr))
            //    {
            //        // lootListAttr.Value contains the loot list number as a string
            //        Console.WriteLine($"LootList: {lootListAttr.Value}"); // this works shows 39

            //        // now that we know the loot list number, we can use it to get the loot items from the config
            //        //CsRoot.Root.ConfigEcfAccess.ConfigBlockById.TryGetValue((int)lootListAttr.Value, out EcfParser.EcfBlock lootContainerConfig); // didnt work, showed SandOrange, sand shape mesh, texture,group
            //        //CsRoot.Root.ConfigEcfAccess.FlatConfigBlockById.TryGetValue((int)lootListAttr.Value, out EcfParser.EcfBlock lootContainerConfig); // didnt work, showed SandOrange, sand shape mesh, texture,group
            //        CsRoot.Root.ConfigEcfAccess.ResourcesForBlockById.TryGetValue((int)lootListAttr.Value, out var lootContainerConfig);

            //        // Example: print all EcfValues for this loot container
            //        foreach (var kvp in lootContainerConfig.EcfValues)
            //        {
            //            Console.WriteLine($"{kvp.Key}: {kvp.Value.Value}");
            //        }


            //    }
            //    else
            //    {
            //        Console.WriteLine("No LootList property found for this block.");
            //    }
            //}




            // Does the elon modding API have a way to get the container loot list number?





            //Console.WriteLine($"{DateTime.Now} Looter ScriptId[{CsRoot.Root.ScriptId}] Running");
            //var eF = "Destruct-*";
            //var destF = "1-Dump1";
            //var destPos = CsRoot.Devices(E.S, destF)?.FirstOrDefault()?.Position;
            //if (destPos == null) return;
            //var destCon = E.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(destPos.Value);
            //var allE = CsRoot.EntitiesByName(eF);
            //if (allE == null || allE.Count() == 0) return;
            //int ic = 0;
            //foreach (var iE in allE)
            //{
            //    var allB = CsRoot.GetAllBlockPositions(iE.S);
            //    foreach (var cBV in allB)
            //    {
            //        var blockData = CsRoot.Block(iE.S, cBV.x, cBV.y, cBV.z);
            //        CsRoot.WithLockedDevice(iE.S, blockData, () =>
            //        {
            //            var sourceCon = iE.GetCurrent().Structure.GetDevice<Eleon.Modding.IContainer>(cBV);
            //            if (sourceCon == null) return;
            //            var cB = iE.GetCurrent().Structure.GetBlock(cBV);
            //            var content = sourceCon?.GetContent();
            //            ic += content.Count;

            //            foreach (var item in content)
            //            {
            //                Console.WriteLine($"  Container Looting: Item:[{item.id}] {item.count} {CsRoot.I18n(item.id)}");
            //                //destCon.AddItems(item.id, item.count);
            //                //sourceCon.Clear();
            //                //cB.Set(0);
            //            }


            //        }, () =>
            //        {
            //            Console.WriteLine($"  Container locking failed on block cBV:[{cBV}]");
            //        });
            //    }
            //}
            //if (ic == 0) Console.WriteLine($"  Container Looting finished");
            //else Console.WriteLine($"  Container looting in progress");
            //========================================= 
        }
    }
}



//## AlienContainer: Rare - Yellow
//{
//    +Container Id: 39
//  Count: "4,5"
//  Size: "8,8" # "8,1"
//  SfxOpen: UseActions / BarrelOpen
//  SfxClose: UseActions / BarrelClose

//  {
//        Child Items
//    Name_0: ArmorLightT2, param1: 0.05, param2: 1
//    Name_1: ArmorMediumT2, param1: 0.05, param2: 1
//    Name_2: LargeUpgradeKit, param1: 0.08, param2: "1"
//    Name_3: GoldIngot, param1: 0.10, param2: "5,10"
//    Name_4: SathiumIngot, param1: 0.12, param2: "50,100"
//    Name_5: AdvancedUpgradeKit, param1: 0.125, param2: "1"
//    Name_6: SmallUpgradeKit, param1: 0.14, param2: "1"
//    Name_7: PlasmaConverter, param1: 0.15, param2: "1,3"
//    Name_8: NeodymiumIngot, param1: 0.15, param2: "35,70"
//    Name_9: Medikit04, param1: 0.15, param2: "1,5"
//    Name_10: AluminiumOre, param1: 0.2, param2: "35,70"
//    Name_11: PulseLaserChargeRifle, param1: 0.2, param2: "10,15"
//    Name_12: MagnesiumPowder, param1: 0.2, param2: "80,180"
//    Name_13: PlatinBar, param1: 0.2, param2: "10,20"
//    Name_14: Oscillator, param1: 0.25, param2: "15,30"
//    Name_15: PromethiumPellets, param1: 0.25, param2: "250,400"
//    Name_16: FluxCoil, param1: 0.3, param2: "8,20"
//    Name_17: MoneyCard, param1: 0.4, param2: "2500,5000"
//    Name_18: PentaxidCrystal, param1: 0.4, param2: "20,40"
//    Name_19: TitanRods, param1: 0.6, param2: "80,150"
//    Name_20: CobaltAlloy, param1: 0.7, param2: "40,80"
//    Name_21: EnergyCellLarge, param1: 1, param2: "10,20"

//    Group_0: Hacking, param1: 0.05
//    Group_1: WeaponUpgradesRare, param1: 0.075
//    Group_2: WeaponUpgradesCommon, param1: 0.10
//    Group_3: SuitBooster, param1: 0.20
//    Group_4: Medikits, param1: 0.5
//    Group_5: Eden_Rare, param1: 0.1
//  }
//}