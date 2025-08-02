using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace EcfParser
{
    public static class Merge
    {
        public static IReadOnlyDictionary<T, EcfBlock> EcfBlocksToDictionary<T>(this IEnumerable<EcfBlock> blocks, Func<EcfBlock, bool> blockSelector, Func<EcfBlock, T> keySelector)
            => blocks
                .Where(B => blockSelector(B))
                .Aggregate(new Dictionary<T, EcfBlock>(), (result, b) => {
                    var key = keySelector(b);
                    if (!result.ContainsKey(key)) result.Add(key, b); return result; });

        public static EcfFile Ecf(params EcfFile[] files)
        {
            EcfFile result = null;
            foreach (var ecf in files)
            {
                if (result == null) result = ecf;
                else                result.MergeWith(ecf);
            }

            return result;
        }

        public static void MergeWith(this EcfFile ecf, EcfFile add)
        {
            if(ecf.Blocks == null) ecf.Blocks = new List<EcfBlock>();

            add?.Blocks?.ForEach(B => {
                // Search in master block list ecf for a block with same Name, Id
                var found = ecf.Blocks
                    .Where(b => b.Name == B.Name)
                    .Where(b => Equals(b.Values?.FirstOrDefault(a => a.Key == "Id").Value  , B.Values?.FirstOrDefault(a => a.Key == "Id").Value))
                    .Where(b => Equals(b.Values?.FirstOrDefault(a => a.Key == "Name").Value, B.Values?.FirstOrDefault(a => a.Key == "Name").Value))
                    .FirstOrDefault();

                // If not found and block name is "Container", search by LootList == B.Id
                if (found == null && B.Name == "Container" && B.Values != null && B.Values.ContainsKey("Id"))
                {
                    var lootListId = B.Values["Id"];

                    //if (lootListId.Equals(38))
                    //{
                    //    // Example +Container Id: 38 AlienContainer: Common - Grey

                    //    var pause = 1;
                    //}

                    var blockMatchingLootList = ecf.Blocks
                        .Where(b => b.Values != null && b.Values.ContainsKey("LootList") && Equals(b.Values["LootList"], lootListId))
                        .FirstOrDefault();
                    
                    if (blockMatchingLootList == null)
                    {
                        return;
                    } else
                    {
                        // Before merging we need to give the child a Name attribute with value of Container_<lootListId>
                        var nameValue = $"ContainerChild_{lootListId}";
                        //var nameAttr = new EcfAttribute { Name = "ContainerChild", Value = nameValue };
                        if (B.Childs.ContainsKey("Child Items"))
                        {
                            var childBlock = B.Childs["Child Items"];
                            B.Childs.Remove("Child Items");
                            B.Childs.Add(nameValue, childBlock);
                        }
                        blockMatchingLootList.MergeChildren(B);
                        //blockMatchingLootList.MergeWith(B);// B has Attr[0] here with Id=38 but the child does not have any attributes
                        return;
                    }
                }

                if (found == null && B.Name == "LootGroup" && B.Values != null && B.Values.ContainsKey("Name"))
                {
                    // LootGroups are used in the children of Block.Childs.Container children they have a syntax like Group_0: ComponentsBasic, param1: 0.80
                    var LootGroupName = B.Values != null && B.Values.ContainsKey("Name") ? B.Values["Name"] : null; // Working e.g. EscapePodEasy
                    var blocksWithContainersWithLootGroupMatching = new List<EcfBlock>();

                    if (LootGroupName.Equals("ZiraxComputer"))
                    {
                        // Example +LootGroup Name: ZiraxComputer belongs to Container Id: 251
                        var pause = 1;
                    }

                    //foreach (var block in ecf.Blocks)
                    //{
                    //    var lootGroupChildAdded = false;

                    //    if (block?.EcfValues != null && block.EcfValues.ContainsKey("Id") && block.EcfValues["Id"]?.Value?.ToString() == "542")
                    //    {
                    //        var pause3 = 1;
                    //    }
                    //    if (block.Childs != null)
                    //    {
                    //        foreach (var child in block.Childs)
                    //        {
                    //            if (child.Key.Contains("ContainerChild_"))
                    //            {
                    //                foreach (var ecfValue in child.Value.EcfValues)
                    //                {
                    //                    if (ecfValue.Key.Contains("Group_"))
                    //                    {
                    //                        if (ecfValue.Value.Value.Equals(LootGroupName))
                    //                        {
                    //                            lootGroupChildAdded = true;

                    //                            // So we need to take the B block lootgroup and add it as a child to ecf block
                    //                            if (block.Childs == null)
                    //                                block.Childs = new Dictionary<string, EcfBlock>();

                    //                            var childKey = $"LootGroupChild_{LootGroupName}";

                    //                            if (!block.Childs.ContainsKey(childKey))
                    //                            {
                    //                                block.Childs.Add(childKey, B);
                    //                            }
                    //                            else
                    //                            {
                    //                                // Optionally handle duplicate key, e.g. overwrite or use a different key
                    //                                block.Childs[childKey] = B;
                    //                            }
                    //                            break;
                    //                        }
                    //                    }
                    //                }

                    //            }
                    //            if (lootGroupChildAdded) break;
                    //        }
                    //    }
                    //}

                    var matchingParentBlocks = ecf.Blocks
                        .Where(block => block.Childs != null &&
                            block.Childs.Any(child =>
                                child.Key.Contains("ContainerChild_") &&
                                child.Value.EcfValues != null &&
                                child.Value.EcfValues.Any(ev =>
                                    ev.Key.Contains("Group_") &&
                                    Equals(ev.Value.Value, LootGroupName)
                                )
                            )
                        )
                        .ToList();

                    foreach (var block in matchingParentBlocks)
                    {
                        if (block.Childs == null)
                            block.Childs = new Dictionary<string, EcfBlock>();

                        var childKey = $"LootGroupChild_{LootGroupName}";
                        block.Childs[childKey] = B; // Add or overwrite
                    }
                    return; // Return so we dont fall into normal block process adding and merging
                }

                if (found == null)
                {
                    ecf.Blocks.Add(B);
                }
                else
                {
                    found.MergeWith(B);
                }
            });
        }

        public static void MergeChildren(this EcfBlock destination, EcfBlock source)
        {
            if (source == null) return;

            if (destination.Childs == null && source.Childs != null) destination.Childs = source.Childs
                     .ToDictionary(B => B.Key, B => { var block = new EcfBlock(); block.MergeWith(B.Value); return block; });
            else source.Childs?
                    .ToList()
                    .ForEach(B =>
                    {
                        if (destination.Childs.TryGetValue(B.Key, out var block)) block.MergeWith(B.Value);
                        else
                        {
                            var newBlock = new EcfBlock(); newBlock.MergeWith(B.Value);
                            destination.Childs.Add(B.Key, newBlock);
                        }
                    });

            destination.Childs?.Values
                .ToList()
                .ForEach(B => {
                    B.EcfValues?.Where(A => !destination.EcfValues.ContainsKey(A.Key))
                    .ToList()
                    .ForEach(A =>
                    {
                        if (destination.EcfValues == null) destination.EcfValues = new Dictionary<string, EcfAttribute>();
                        if (destination.Values == null) destination.Values = new Dictionary<string, object>();

                        destination.EcfValues.Add(A.Key, A.Value);
                        destination.Values.Add(A.Key, A.Value.Value);
                    });
                });
        }

        public static void MergeWith(this EcfBlock destination, EcfBlock source)
        {
            if (source == null) return;

            destination.Name = source.Name;

            source.Attr?.ForEach(A =>
            {
                if (destination.Attr      == null) destination.Attr      = new List<EcfAttribute>();
                if (destination.Values    == null) destination.Values    = new Dictionary<string, object>();
                if (destination.EcfValues == null) destination.EcfValues = new Dictionary<string, EcfAttribute>();

                var foundAttr = destination.Attr.FirstOrDefault(a => a.Name == A.Name);
                if (foundAttr == null)
                {
                    destination.Attr.Add(foundAttr = new EcfAttribute()
                    {
                        Name   = A.Name,
                        Value  = A.Value,
                        AddOns = A.AddOns == null ? null : new Dictionary<string, object>(A.AddOns)
                    });

                    if (A.Name != null && !destination.EcfValues.ContainsKey(A.Name)) destination.EcfValues.Add(A.Name, foundAttr);
                    if (A.Name != null && !destination.Values   .ContainsKey(A.Name)) destination.Values   .Add(A.Name, foundAttr.Value);
                }
                else
                {
                    MergeEcfAttribute(foundAttr, A);

                    if (A.Name != null && !destination.EcfValues.ContainsKey(A.Name)) destination.EcfValues.Add(A.Name, foundAttr);
                    if (A.Name != null && !destination.Values.ContainsKey(A.Name)) destination.Values.Add(A.Name, foundAttr.Value);
                }
            });

            if (destination.Childs == null && source.Childs != null) destination.Childs = source.Childs
                     .ToDictionary(B => B.Key, B => { var block = new EcfBlock(); block.MergeWith(B.Value); return block; });
            else source.Childs?
                    .ToList()
                    .ForEach(B =>
                    {
                        if (destination.Childs.TryGetValue(B.Key, out var block)) block.MergeWith(B.Value);
                        else
                        {
                            var newBlock = new EcfBlock(); newBlock.MergeWith(B.Value);
                            destination.Childs.Add(B.Key, newBlock);
                        }
                    });

            destination.Childs?.Values
                .ToList()
                .ForEach(B => { 
                    B.EcfValues?.Where(A => !destination.EcfValues.ContainsKey(A.Key))
                    .ToList()
                    .ForEach(A =>
                    {
                        if (destination.EcfValues == null) destination.EcfValues = new Dictionary<string, EcfAttribute>();
                        if (destination.Values    == null) destination.Values    = new Dictionary<string, object>();

                        destination.EcfValues.Add(A.Key, A.Value);
                        destination.Values   .Add(A.Key, A.Value.Value);
                    });
                });

            if (source.EcfValues != null && destination.EcfValues == null) destination.EcfValues = new Dictionary<string, EcfAttribute>(source.EcfValues);
            else if (source.EcfValues != null) foreach (var item in source.EcfValues)
                {
                    if (destination.EcfValues.TryGetValue(item.Key, out var attr)) MergeEcfAttribute(attr, item.Value);
                    else                                                           destination.EcfValues.Add(item.Key, item.Value);
                }

            if (source.Values != null && destination.Values == null) destination.Values = new Dictionary<string, object>(source.Values);
            else if (source.Values != null) foreach (var item in source.Values)
                {
                    if (destination.Values.ContainsKey(item.Key)) destination.Values[item.Key] = item.Value;
                    else                                          destination.Values.Add(item.Key, item.Value);
                }

        }

        private static void MergeEcfAttribute(EcfAttribute dest, EcfAttribute source)
        {
            dest.Value = source.Value;
            if (dest.AddOns == null && source.AddOns != null) dest.AddOns = new Dictionary<string, object>(source.AddOns);
            else source.AddOns?.ToList().ForEach(P =>
            {
                if (dest.AddOns.ContainsKey(P.Key)) dest.AddOns[P.Key] = P.Value;
                else dest.AddOns.Add(P.Key, P.Value);
            });
        }
    }
}
