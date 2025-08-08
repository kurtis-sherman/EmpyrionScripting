using Eleon.Modding;
using EmpyrionScripting;
using EmpyrionScripting.CustomHelpers;
using EmpyrionScripting.DataWrapper;
using EmpyrionScripting.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class ModMain
{
    public static void Main(IScriptModData rootObject)
    {
        Script1(rootObject);

        Script2(rootObject);


    }
    private static void Script2(IScriptModData rootObject)
    {
        if (!(rootObject is IScriptSaveGameRootData root)) return;
        if (root.E.Faction.Id == 0) return;

        var CsRoot = rootObject.CsRoot;
        var P = rootObject.P;
        var E = rootObject.E;

        // Find all CoreScan LCD devices
        var coreScanLcds = CsRoot.GetDevices<ILcd>(CsRoot.Devices(E.S, "CoreScan"));

        ClearLcds(coreScanLcds);
        WriteTo(coreScanLcds, $"{DateTime.Now:HH:mm:ss} Core Scan Results:");

        if (coreScanLcds == null || coreScanLcds.Length == 0)
        {
            //Console.WriteLine("No CoreScan LCD devices found");
            return;
        }

        var allEntities = CsRoot.Root.GetEntities();
        if (allEntities == null || allEntities.Count() == 0)
        {
            WriteTo(coreScanLcds, "No entities found");
            return;
        }

        //WriteTo(coreScanLcds, $"Scanning found [{allEntities.Count()}] entities...");

        var maxDistance = 2000f;
        var nearbyEntities = allEntities
            .Where(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position) <= maxDistance)
            .OrderBy(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position));


        var coreName = "*ore*";
        var coresFound = 0;
        var processedEntities = 0;

        foreach (var entity in nearbyEntities)
        {
            processedEntities++;
            var distance = UnityEngine.Vector3.Distance(E.Pos, entity.Position);

            WriteTo(coreScanLcds, $"[{processedEntities}] {entity.Name} [{entity.Type}] - Δ{distance:F0}m");

            try
            {
                // Get the structure's min and max bounds
                var structure = entity.Structure;
                var minPos = structure.MinPos;
                var maxPos = structure.MaxPos;

                // Apply Empyrion's strange Y-offset of 128 for block coordinates
                var adjustedMinPos = new VectorInt3(minPos.x, 128 + minPos.y, minPos.z);
                var adjustedMaxPos = new VectorInt3(maxPos.x, 128 + maxPos.y, maxPos.z);

                //WriteTo(coreScanLcds, $"  Scanning blocks from {minPos} to {maxPos}");

                var blocksChecked = 0;
                var coreBlocksFound = 0;

                // Iterate through all block positions (be careful with large structures)
                var maxBlocksToCheck = 50000; // Limit to prevent timeouts

                for (int x = adjustedMinPos.x; x <= adjustedMaxPos.x && blocksChecked < maxBlocksToCheck; x++)
                {
                    for (int y = adjustedMinPos.y; y <= adjustedMaxPos.y && blocksChecked < maxBlocksToCheck; y++)
                    {
                        for (int z = adjustedMinPos.z; z <= adjustedMaxPos.z && blocksChecked < maxBlocksToCheck; z++)
                        {
                            blocksChecked++;
                            var blockPos = new VectorInt3(x, y, z);

                            try
                            {
                                var block = structure.GetBlock(blockPos);
                                if (block != null)
                                {
                                    block.Get(out int blockType, out int blockShape, out int blockRotation, out bool isActive);

                                    // Check if this is a core block
                                    if (IsCoreBlock(blockType))
                                    {
                                        coreBlocksFound++;
                                        coresFound++;

                                        var result = PlaceLcdProjector(CsRoot, entity, structure, blockPos, coreScanLcds);
                                        WriteTo(coreScanLcds, result);
                                    }
                                    // now we have the core block, add +1 to the Y value of the block and change the block type to a lcd projector block type 1400
                                    // and we want to set the height of projector to maximum height so it can project far
                                    // and display a red line of pipes going up from the core block to the sky so we can see it


                                    //var customName = string.IsNullOrEmpty(block.CustomName) ? "Unnamed" : block.CustomName;
                                    //WriteTo(coreScanLcds, $"  CORE: {customName} Type:{blockType} Pos:{blockPos}");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Skip invalid block positions
                                continue;
                            }
                        }
                    }
                }

                if (coreBlocksFound > 0)
                {
                    WriteTo(coreScanLcds, $"  → Found {coreBlocksFound} core(s) in {blocksChecked} blocks");
                }
                else
                {
                    //WriteTo(coreScanLcds, $"  → No cores found in {blocksChecked} blocks checked");
                }
            }
            catch (Exception ex)
            {
                WriteTo(coreScanLcds, $"  Error scanning entity: {ex.Message}");
            }

            WriteTo(coreScanLcds, ""); // Empty line between entities
        }

        WriteTo(coreScanLcds, $"Scan complete: {coresFound} cores found in {processedEntities} entities");
        WriteTo(coreScanLcds, $"Time: {DateTime.Now:HH:mm:ss}");

        //foreach (var entity in nearbyEntities)
        //{
        //    var distance = UnityEngine.Vector3.Distance(E.Pos, entity.Position);
        //    WriteTo(coreScanLcds, $" {entity.Name} [{entity.Type}]-Δ{distance:F0}m");

        //    var corePosList = entity.Structure.GetDevicePositions(coreName);
        //    if (corePosList != null || corePosList.Count() > 0)
        //    {
        //        WriteTo(coreScanLcds, $"   corePosList Count[{corePosList.Count()}]");
        //    }


        //    var allDevices = entity.Structure.GetDevices(DeviceTypeName.All_NOT_USED_YET);
        //    if (allDevices != null || allDevices.Count > 0)
        //    {
        //        WriteTo(coreScanLcds, $"   allDevices Count[{allDevices.Count}]");
        //    }

        //    var allDevices1 = entity.Structure.GetDevicePositions("*");
        //    if (allDevices1 != null || allDevices1.Count() > 0)
        //    {
        //        WriteTo(coreScanLcds, $"   allDevices1 Count[{allDevices1.Count()}]");
        //    }


        //    // I need to get all blocks for the entity and find their block type

        //    //if (corePosList == null || corePosList.Count == 0)
        //    //{
        //    //    // Try alternative core names
        //    //    var allDevices = entity.Structure.GetDevices(DeviceTypeName.All_NOT_USED_YET);
        //    //    WriteTo(coreScanLcds, $"   allDevices Count[{allDevices.Count}]");

        //    //}

        //    if (corePosList != null && corePosList.Count > 0)
        //    {
        //        coresFound++;
        //        WriteTo(coreScanLcds, $" CORE FOUND: ");

        //        //foreach (var corePos in corePosList)
        //        //{
        //        //    var core = entity.Structure.GetBlock(corePos);
        //        //    if (core != null)
        //        //    {
        //        //        core.Get(out int coreBlockType, out _, out _, out _);
        //        //        var coreName_display = string.IsNullOrEmpty(core.CustomName) ? "Unnamed Core" : core.CustomName;
        //        //        WriteTo(coreScanLcds, $"  Core: {coreName_display} (Type: {coreBlockType})");
        //        //    }
        //        //}
        //    }
        //}
        //WriteTo(coreScanLcds, $"{DateTime.Now:HH:mm:ss} Core Scan Results:");
    }

    private static void Script1(IScriptModData rootObject)
    {
        if (!(rootObject is IScriptSaveGameRootData root)) return;
        if (root.E.Faction.Id == 0) return;

        var infoOutLcds = root.CsRoot.GetDevices<ILcd>(root.CsRoot.Devices(root.E.S, "CargoOutInfo*"));

        root.E.S
            .AllCustomDeviceNames
            .GetUniqueNames("CargoOut@*")
            .ForEach(cargoOutContainerName =>
            {
                var container = root.CsRoot.Devices(root.E.S, cargoOutContainerName).FirstOrDefault();
                if (container == null) return;
                if (!int.TryParse(cargoOutContainerName.Substring("CargoOut@".Length), out var targetEntityId))
                {
                    WriteTo(infoOutLcds, $"CargoOut@[ID] id is not a number");
                    return;
                }

                var cargoTargetFileName = Path.Combine(root.MainScriptPath, "..", "CargoTeleport", root.E.Faction.Id.ToString(), $"Cargo-{targetEntityId}.json");

                if (!File.Exists(cargoTargetFileName))
                {
                    WriteTo(infoOutLcds, $"CargoIn in [ID] not ready");
                    return;
                }

                root.CsRoot.WithLockedDevice(root.E.S, container, () =>
                {
                    var nativeContainer = ((ContainerData)container.Device).GetContainer() as IContainer;

                    var items = nativeContainer.GetContent();
                    var failedItems = new List<ItemStack>();
                    items.ForEach(i =>
                    {
                        try
                        {
                            File.AppendAllText(cargoTargetFileName, JsonConvert.SerializeObject(i) + "\n");
                            WriteTo(infoOutLcds, $"Transfer: Item:[{i.id}] {i.count} {root.CsRoot.I18n(i.id)}");
                        }
                        catch
                        {
                            failedItems.Add(i);
                            WriteTo(infoOutLcds, $"Transfer failed: Item:[{i.id}] {i.count} {root.CsRoot.I18n(i.id)}");
                        }
                    });
                    nativeContainer.SetContent(failedItems.UniqueSlots());
                });
            });

        var infoInLcds = root.CsRoot.GetDevices<ILcd>(root.CsRoot.Devices(root.E.S, "CargoInInfo*"));

        root.E.S
            .AllCustomDeviceNames
            .GetUniqueNames("CargoIn")
            .ForEach(cargoInContainerName =>
            {
                var container = root.CsRoot.Devices(root.E.S, cargoInContainerName).FirstOrDefault();
                if (container == null) return;

                var nativeContainer = ((ContainerData)container.Device).GetContainer() as IContainer;
                if (nativeContainer == null) return;

                var cargoTargetFileName = Path.Combine(root.MainScriptPath, "..", "CargoTeleport", root.E.Faction.Id.ToString(), $"Cargo-{root.E.Id}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(cargoTargetFileName));

                root.CsRoot.WithLockedDevice(root.E.S, container, () =>
                {
                    var items = nativeContainer.GetContent() ?? new List<ItemStack>();
                    bool itemsAdded = false;
                    using (var lockFile = File.Open(cargoTargetFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        using (var file = new StreamReader(lockFile))
                        {
                            var remainLines = new List<string>();

                            while (true)
                            {
                                var itemLine = file.ReadLine();
                                if (string.IsNullOrEmpty(itemLine)) break;
                                else if (items.Count < 64)
                                {
                                    var item = JsonConvert.DeserializeObject<ItemStack>(itemLine);
                                    items.Add(item);
                                    itemsAdded = true;
                                    WriteTo(infoInLcds, $"Transfer: Item:[{item.id}] {item.count} {root.CsRoot.I18n(item.id)}");
                                }
                                else remainLines.Add(itemLine);
                            }

                            lockFile.Seek(0, SeekOrigin.Begin);
                            lockFile.SetLength(0);

                            using (var fileWrite = new StreamWriter(lockFile))
                            {
                                remainLines.ForEach(l => fileWrite.WriteLine(l));
                            }
                        }
                    }
                    if (items.Count > 0 && itemsAdded) nativeContainer.SetContent(items.UniqueSlots());
                });
            });
    }

    // Helper method to determine if a block type is a core
    private static bool IsCoreBlock(int blockType)
    {
        // Common core block type IDs in Empyrion
        // You may need to adjust these based on your game version
        var coreBlockTypes = new int[] {
        560,  // NPC Core
        //558,  // player core
        1360,
        1361,
        1401, // 1400 is 
        1402,
        2768,
        2769,
        2770,
        2771,
        3148,
        3299,
        3300,
        3302,
        // Add other core type IDs as needed
    };

        return coreBlockTypes.Contains(blockType);
    }

    private static void WriteTo(ILcd[] lcds, string text)
    {
        lcds.ForEach(L => L.SetText($"{text}\n{L.GetText()}"));
    }

    private static void ClearLcds(ILcd[] lcds)
    {
        lcds.ForEach(L => L.SetText($""));
    }


    // Add this method at the end of the class
    private static string CreateVerticalRedLinePattern()
    {
        // Create a pattern of red vertical lines using LCD text formatting
        var lines = new List<string>();

        // Add multiple lines of red pipe symbols to create a vertical line effect
        for (int i = 0; i < 20; i++) // Adjust line count as needed
        {
            lines.Add("<color=red>||||||||||||||||||||||||||||||||||||||||</color>");
        }

        // Add core identification text
        lines.Insert(0, "<color=red>CORE DETECTED</color>");
        lines.Insert(1, "<color=red>↑ BEACON ↑</color>");
        lines.Add("<color=red>↓ CORE BELOW ↓</color>");

        return string.Join("\n", lines);
    }

    // Replace the PlaceLcdProjector method with this updated version:
    private static string PlaceLcdProjector(ICsScriptFunctions CsRoot, IEntity entity, IStructure structure, VectorInt3 corePos, ILcd[] debugLcds)
    {
        var results = new List<string>();

        try
        {
            // Find and place up to 5 projectors above the core
            var abovePositions = FindEmptySpaces(structure, corePos, 1, 50, 5); // Search 50, place max 5
            if (abovePositions.Count > 0)
            {
                foreach (var pos in abovePositions)
                {
                    var aboveResult = CreateProjectorAtPosition(CsRoot, entity, structure, pos, "ABOVE");
                    results.Add(aboveResult);
                }
            }
            else
            {
                results.Add($"    → No empty spaces found above core at {corePos}");
            }

            // Find and place up to 5 projectors below the core
            var belowPositions = FindEmptySpaces(structure, corePos, -1, 50, 5); // Search 50, place max 5
            if (belowPositions.Count > 0)
            {
                foreach (var pos in belowPositions)
                {
                    var belowResult = CreateProjectorAtPosition(CsRoot, entity, structure, pos, "BELOW");
                    results.Add(belowResult);
                }
            }
            else
            {
                results.Add($"    → No empty spaces found below core at {corePos}");
            }

            return string.Join("\n", results);
        }
        catch (Exception ex)
        {
            return $"    → Error placing projectors: {ex.Message}";
        }
    }

    // Replace the FindEmptySpaces method with this corrected version:
    private static List<VectorInt3> FindEmptySpaces(IStructure structure, VectorInt3 startPos, int direction, int maxSearchDistance, int maxProjectors)
    {
        var emptySpaces = new List<VectorInt3>();
        var projectorCount = 0; // Track how many projectors we've found/placed in this direction

        // direction: 1 for up (+Y), -1 for down (-Y)
        for (int i = 1; i <= maxSearchDistance && projectorCount < maxProjectors; i++)
        {
            var checkPos = new VectorInt3(startPos.x, startPos.y + (i * direction), startPos.z);

            try
            {
                var block = structure.GetBlock(checkPos);
                if (block != null)
                {
                    block.Get(out int blockType, out _, out _, out _);

                    if (blockType == 0) // Empty space (air)
                    {
                        emptySpaces.Add(checkPos);
                        projectorCount++; // Count this as a potential projector placement
                    }
                    else if (blockType == 1400) // Already an LCD projector
                    {
                        projectorCount++; // Count existing projectors toward our limit
                                          // Don't add to emptySpaces since it's already occupied by a projector
                    }
                    // For any other block type, just continue searching without incrementing counters
                }
                else
                {
                    // GetBlock returned null - this might indicate empty space or out of bounds
                    // In some implementations, null means empty space
                    emptySpaces.Add(checkPos);
                    projectorCount++; // Count this as a potential projector placement
                }
            }
            catch
            {
                // If we can't access the block (out of bounds), stop searching in this direction
                break;
            }
        }

        return emptySpaces;
    }

    private static string CreateProjectorAtPosition(ICsScriptFunctions CsRoot, IEntity entity, IStructure structure, VectorInt3 projectorPos, string direction)
    {
        try
        {
            // Double-check that the position is still empty
            var block = structure.GetBlock(projectorPos);
            if (block != null)
            {
                block.Get(out int blockType, out _, out _, out _);
                if (blockType == 1400) // Already an LCD projector
                {
                    UpdateExistingProjector(structure, projectorPos);
                    return $"    → Updated existing {direction} projector at {projectorPos}";
                }
                else if (blockType != 0) // Not empty
                {
                    return $"    → {direction} position {projectorPos} occupied by BlockType {blockType}";
                }
            }

            // Create EntityData wrapper to get IStructureData
            var entityData = new EntityData(CsRoot.Root.GetCurrentPlayfield(), entity);
            var structureData = entityData.S;

            if (structureData == null)
            {
                return $"    → Failed to get structure data for {direction} projector placement";
            }

            // Create a new LCD projector block at the specified position
            var newProjectorBlock = CsRoot.Block(structureData, projectorPos.x, projectorPos.y, projectorPos.z);

            // Configure the block
            newProjectorBlock.BlockType = 1400; // LCD projector type
            newProjectorBlock.Active = true;

            // Configure the LCD device
            var projectorLcd = structure.GetDevice<ILcd>(projectorPos);
            if (projectorLcd != null)
            {
                ConfigureProjectorDisplay(projectorLcd, direction);
            }

            return $"    → Placed {direction} projector at {projectorPos}";
        }
        catch (Exception ex)
        {
            return $"    → Error creating {direction} projector: {ex.Message}";
        }
    }

    private static void UpdateExistingProjector(IStructure structure, VectorInt3 projectorPos)
    {
        var existingLcd = structure.GetDevice<ILcd>(projectorPos);
        if (existingLcd != null)
        {
            ConfigureProjectorDisplay(existingLcd, "EXISTING");
        }
    }

    // Also update the ConfigureProjectorDisplay method to set background colors based on direction:
    private static void ConfigureProjectorDisplay(ILcd projectorLcd, string direction)
    {
        try
        {
            // Set projector to display vertical line pattern with direction indicator
            var redLinePattern = CreateVerticalRedLinePattern(direction);
            projectorLcd.SetText(redLinePattern);

            // Set background color based on direction
            if (direction == "ABOVE")
            {
                // Dark blue background for upward projectors
                projectorLcd.SetBackgroundColor(new UnityEngine.Color(0.0f, 0.0f, 0.2f, 1.0f)); // Dark blue
            }
            else if (direction == "BELOW")
            {
                // Dark red background for downward projectors
                projectorLcd.SetBackgroundColor(new UnityEngine.Color(0.2f, 0.0f, 0.0f, 1.0f)); // Dark red
            }
            else
            {
                // Default black background
                projectorLcd.SetBackgroundColor(new UnityEngine.Color(0.0f, 0.0f, 0.0f, 1.0f)); // Black
            }
        }
        catch (Exception ex)
        {
            // Silently handle LCD configuration errors
        }
    }

    // Update the CreateVerticalRedLinePattern method to include direction-based colors:
    private static string CreateVerticalRedLinePattern(string direction = "")
    {
        var lines = new List<string>();

        // Determine color based on direction
        string color = "red"; // default color
        if (direction == "ABOVE")
        {
            color = "blue"; // Blue for upward/above (+Y direction)
        }
        else if (direction == "BELOW")
        {
            color = "red"; // Red for downward/below (-Y direction)
        }

        lines.Add($"<color={color}>CORE DETECTED</color>");

        // Add directional indicators with appropriate colors
        if (direction == "ABOVE")
        {
            lines.Add($"<color={color}>↓ CORE BELOW ↓</color>");
        }
        else if (direction == "BELOW")
        {
            lines.Add($"<color={color}>↑ CORE ABOVE ↑</color>");
        }
        else
        {
            lines.Add($"<color={color}>↑ BEACON ↑</color>");
        }

        // Add multiple lines of colored pipe symbols to create a vertical line effect
        for (int i = 0; i < 18; i++) // Reduced to make room for direction text
        {
            lines.Add($"<color={color}>||||||||||||||||||||||||||||||||||||||||</color>");
        }

        return string.Join("\n", lines);
    }

}