using Eleon.Modding;
using EmpyrionScripting.CustomHelpers;
using EmpyrionScripting.DataWrapper;
using EmpyrionScripting.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ModMain
{
    public static void Main(IScriptModData rootObject)
    {
        Script1(rootObject);

        Script2(rootObject);


    }

    // Helper method to show scan progress during waiting periods
    private static void ShowScanProgress(ICsScriptFunctions CsRoot, ILcd[] coreScanLcds)
    {
        var persistentData = CsRoot.Root.GetPersistendData();
        var activeScans = 0;

        foreach (var kvp in persistentData)
        {
            if (kvp.Key.StartsWith("CoreScan_Progress_") && kvp.Value is EntityScanProgress progress)
            {
                if (!progress.IsComplete)
                {
                    activeScans++;
                    WriteTo(coreScanLcds, $"📊 Entity {kvp.Key.Split('_').Last()}: {progress.TotalBlocksProcessed} blocks scanned");
                }
            }
        }

        if (activeScans == 0)
        {
            WriteTo(coreScanLcds, "✅ All entities fully scanned");
        }
        else
        {
            WriteTo(coreScanLcds, $"⏳ {activeScans} entities still scanning...");
        }
    }

    // Update the Script2 method to include periodic cleanup:
    private static void Script2(IScriptModData rootObject)
    {
        if (!(rootObject is IScriptSaveGameRootData root)) return;
        if (root.E.Faction.Id == 0) return;

        var CsRoot = rootObject.CsRoot;
        var P = rootObject.P;
        var E = rootObject.E;

        // Find all CoreScan LCD devices
        var coreScanLcds = CsRoot.GetDevices<ILcd>(CsRoot.Devices(E.S, "CoreScan"));

        if (coreScanLcds == null || coreScanLcds.Length == 0)
        {
            return;
        }

        // Perform periodic cleanup of stale entity data every 5 minutes
        var cleanupKey = "CoreScan_LastCleanup";
        var persistentData = CsRoot.Root.GetPersistendData();
        var now = DateTime.Now;

        if (persistentData.TryGetValue(cleanupKey, out var lastCleanupObj) && lastCleanupObj is DateTime lastCleanup)
        {
            var timeSinceLastCleanup = now - lastCleanup;
            if (timeSinceLastCleanup.TotalMinutes >= 5) // Cleanup every 5 minutes
            {
                CleanupStaleEntityData(CsRoot);
                persistentData.AddOrUpdate(cleanupKey, now, (k, v) => now);
                WriteTo(coreScanLcds, "🧹 Cleaned up stale entity data");
            }
        }
        else
        {
            // First run - perform cleanup and set timestamp
            CleanupStaleEntityData(CsRoot);
            persistentData.AddOrUpdate(cleanupKey, now, (k, v) => now);
        }

        // Check if we should perform a core scan based on timing
        var cacheKey = $"CoreScan_LastScan_{E.Id}";

        // Check when last scan was performed
        var shouldScan = true;
        if (persistentData.TryGetValue(cacheKey, out var lastScanObj) && lastScanObj is DateTime lastScan)
        {
            var timeSinceLastScan = now - lastScan;
            shouldScan = timeSinceLastScan.TotalSeconds >= 20; // 20 second interval

            if (!shouldScan)
            {
                // Update LCD with time remaining until next scan
                var timeRemaining = 20 - (int)timeSinceLastScan.TotalSeconds;
                ClearLcds(coreScanLcds);
                WriteTo(coreScanLcds, $"{DateTime.Now:HH:mm:ss} Core Scan Results:");
                WriteTo(coreScanLcds, $"Next scan cycle in {timeRemaining} seconds...");

                // Show any ongoing scan progress
                ShowScanProgress(CsRoot, coreScanLcds);

                // Still update existing projectors without scanning for new cores
                UpdateExistingProjectors(CsRoot, coreScanLcds);
                return;
            }
        }

        // Perform the actual core scan cycle
        if (shouldScan)
        {
            // Update last scan time
            persistentData.AddOrUpdate(cacheKey, now, (k, v) => now);

            // Clear LCD and show scan start
            ClearLcds(coreScanLcds);
            WriteTo(coreScanLcds, $"{DateTime.Now:HH:mm:ss} Core Scan Cycle:");
            WriteTo(coreScanLcds, "🔍 Processing 5000 blocks...");

            var allEntities = CsRoot.Root.GetEntities();
            if (allEntities == null || allEntities.Count() == 0)
            {
                WriteTo(coreScanLcds, "No entities found");
                return;
            }

            var maxDistance = 2000f;
            var nearbyEntities = allEntities
                .Where(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position) <= maxDistance)
                .OrderBy(entity => UnityEngine.Vector3.Distance(E.Pos, entity.Position));

            var coresFound = 0;
            var processedEntities = 0;

            foreach (var entity in nearbyEntities)
            {
                processedEntities++;
                var distance = UnityEngine.Vector3.Distance(E.Pos, entity.Position);

                WriteTo(coreScanLcds, $"[{processedEntities}] {entity.Name} [{entity.Type}] - Δ{distance:F0}m");

                try
                {
                    var coreBlocksFound = ScanEntityForCores(CsRoot, entity, coreScanLcds);
                    coresFound += coreBlocksFound;

                    if (coreBlocksFound > 0)
                    {
                        WriteTo(coreScanLcds, $"  → Found {coreBlocksFound} core(s) this cycle");
                    }
                }
                catch (Exception ex)
                {
                    WriteTo(coreScanLcds, $"  Error scanning entity: {ex.Message}");
                }

                WriteTo(coreScanLcds, ""); // Empty line between entities
            }

            WriteTo(coreScanLcds, $"Cycle complete: {coresFound} cores found");
            WriteTo(coreScanLcds, $"Time: {DateTime.Now:HH:mm:ss}");
            WriteTo(coreScanLcds, "Next cycle in 20 seconds...");
        }
    }

    // Update the ScanEntityForCores method to track entities with cores:
    private static int ScanEntityForCores(ICsScriptFunctions CsRoot, IEntity entity, ILcd[] coreScanLcds)
    {
        var structure = entity.Structure;
        var minPos = structure.MinPos;
        var maxPos = structure.MaxPos;

        // Apply Empyrion's strange Y-offset of 128 for block coordinates
        var adjustedMinPos = new VectorInt3(minPos.x, 128 + minPos.y, minPos.z);
        var adjustedMaxPos = new VectorInt3(maxPos.x, 128 + maxPos.y, maxPos.z);

        // Get or create scan progress data for this entity
        var scanProgressKey = $"CoreScan_Progress_{entity.Id}";
        var persistentData = CsRoot.Root.GetPersistendData();

        var scanProgress = GetOrCreateScanProgress(persistentData, scanProgressKey, adjustedMinPos, adjustedMaxPos);

        var blocksToProcess = 8000; // Process 8000 blocks per cycle
        var blocksProcessed = 0;
        var coreBlocksFound = 0;

        // Continue scanning from where we left off
        for (int x = scanProgress.CurrentX; x <= adjustedMaxPos.x && blocksProcessed < blocksToProcess; x++)
        {
            var startY = (x == scanProgress.CurrentX) ? scanProgress.CurrentY : adjustedMinPos.y;
            for (int y = startY; y <= adjustedMaxPos.y && blocksProcessed < blocksToProcess; y++)
            {
                var startZ = (x == scanProgress.CurrentX && y == scanProgress.CurrentY) ? scanProgress.CurrentZ : adjustedMinPos.z;
                for (int z = startZ; z <= adjustedMaxPos.z && blocksProcessed < blocksToProcess; z++)
                {
                    blocksProcessed++;
                    scanProgress.TotalBlocksProcessed++;

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

                                // Track that this entity has cores
                                TrackEntityWithCores(persistentData, entity.Id);

                                var result = PlaceLcdProjector(CsRoot, entity, structure, blockPos, coreScanLcds);
                                WriteTo(coreScanLcds, result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip invalid block positions
                        continue;
                    }

                    // Update current position
                    scanProgress.CurrentX = x;
                    scanProgress.CurrentY = y;
                    scanProgress.CurrentZ = z;
                }
                // Reset Z for next Y iteration
                scanProgress.CurrentZ = adjustedMinPos.z;
            }
            // Reset Y for next X iteration  
            scanProgress.CurrentY = adjustedMinPos.y;
        }

        // Check if scanning is complete
        if (scanProgress.CurrentX > adjustedMaxPos.x)
        {
            // Scanning complete - reset for next full scan
            scanProgress.IsComplete = true;
            scanProgress.CompletedAt = DateTime.Now;
            WriteTo(coreScanLcds, $"  ✓ Scan complete: {scanProgress.TotalBlocksProcessed} blocks processed");

            // If no cores found in this entity, remove it from the cores list
            if (coreBlocksFound == 0)
            {
                RemoveEntityFromCoresList(persistentData, entity.Id);
            }

            // Remove progress data to start fresh next time
            persistentData.TryRemove(scanProgressKey, out _);
        }
        else
        {
            // Scanning in progress
            var totalBlocks = (adjustedMaxPos.x - adjustedMinPos.x + 1) *
                             (adjustedMaxPos.y - adjustedMinPos.y + 1) *
                             (adjustedMaxPos.z - adjustedMinPos.z + 1);
            var progressPercent = (scanProgress.TotalBlocksProcessed * 100) / totalBlocks;

            WriteTo(coreScanLcds, $"  ⏳ Scanning: {scanProgress.TotalBlocksProcessed}/{totalBlocks} blocks ({progressPercent}%)");

            // Update persistent data
            persistentData.AddOrUpdate(scanProgressKey, scanProgress, (k, v) => scanProgress);
        }

        return coreBlocksFound;
    }

    // Helper method to track entities that have cores
    private static void TrackEntityWithCores(ConcurrentDictionary<string, object> persistentData, int entityId)
    {
        var entitiesWithCores = GetEntitiesWithCores(persistentData);
        entitiesWithCores.Add(entityId);
        persistentData.AddOrUpdate("EntitiesWithCores", entitiesWithCores, (k, v) => entitiesWithCores);
    }

    // Helper method to remove entity from cores list if no cores found
    private static void RemoveEntityFromCoresList(ConcurrentDictionary<string, object> persistentData, int entityId)
    {
        var entitiesWithCores = GetEntitiesWithCores(persistentData);
        if (entitiesWithCores.Remove(entityId))
        {
            persistentData.AddOrUpdate("EntitiesWithCores", entitiesWithCores, (k, v) => entitiesWithCores);
        }
    }


    // Also enhance the CleanupStaleEntityData method to provide feedback:
    private static void CleanupStaleEntityData(ICsScriptFunctions CsRoot)
    {
        try
        {
            var persistentData = CsRoot.Root.GetPersistendData();
            var entitiesWithCores = GetEntitiesWithCores(persistentData);
            var allEntities = CsRoot.Root.GetEntities();

            if (allEntities == null) return;

            var currentEntityIds = new HashSet<int>(allEntities.Select(e => e.Id));
            var toRemove = entitiesWithCores.Where(id => !currentEntityIds.Contains(id)).ToList();

            var progressToRemove = new List<string>();

            // Clean up both core tracking and progress data
            foreach (var entityId in toRemove)
            {
                entitiesWithCores.Remove(entityId);

                // Also clean up any progress data for removed entities
                var progressKey = $"CoreScan_Progress_{entityId}";
                if (persistentData.TryRemove(progressKey, out _))
                {
                    progressToRemove.Add(progressKey);
                }
            }

            // Update the entities with cores list if we removed any
            if (toRemove.Count > 0)
            {
                persistentData.AddOrUpdate("EntitiesWithCores", entitiesWithCores, (k, v) => entitiesWithCores);
            }

            // Log cleanup activity for debugging (optional)
            if (toRemove.Count > 0 || progressToRemove.Count > 0)
            {
                // You could add logging here if needed:
                // Console.WriteLine($"Cleaned up {toRemove.Count} stale entities and {progressToRemove.Count} progress entries");
            }
        }
        catch (Exception ex)
        {
            // Log cleanup errors if needed:
            // Console.WriteLine($"Error during cleanup: {ex.Message}");
        }
    }

    // Helper method to get or create scan progress
    private static EntityScanProgress GetOrCreateScanProgress(ConcurrentDictionary<string, object> persistentData, string key, VectorInt3 minPos, VectorInt3 maxPos)
    {
        if (persistentData.TryGetValue(key, out var existingProgress) && existingProgress is EntityScanProgress progress)
        {
            // Check if the entity bounds have changed (structure modified)
            if (progress.MinPos.x != minPos.x || progress.MinPos.y != minPos.y || progress.MinPos.z != minPos.z ||
                progress.MaxPos.x != maxPos.x || progress.MaxPos.y != maxPos.y || progress.MaxPos.z != maxPos.z)
            {
                // Bounds changed, start over
                return CreateNewScanProgress(minPos, maxPos);
            }

            return progress;
        }

        // Create new progress
        return CreateNewScanProgress(minPos, maxPos);
    }

    private static EntityScanProgress CreateNewScanProgress(VectorInt3 minPos, VectorInt3 maxPos)
    {
        return new EntityScanProgress
        {
            CurrentX = minPos.x,
            CurrentY = minPos.y,
            CurrentZ = minPos.z,
            TotalBlocksProcessed = 0,
            IsComplete = false,
            StartedAt = DateTime.Now,
            MinPos = minPos,
            MaxPos = maxPos
        };
    }

    // Helper class to track scanning progress
    public class EntityScanProgress
    {
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public int CurrentZ { get; set; }
        public int TotalBlocksProcessed { get; set; }
        public bool IsComplete { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public VectorInt3 MinPos { get; set; }
        public VectorInt3 MaxPos { get; set; }
    }


    // Replace the UpdateExistingProjectors method with this optimized version:
    private static void UpdateExistingProjectors(ICsScriptFunctions CsRoot, ILcd[] coreScanLcds)
    {
        try
        {
            var persistentData = CsRoot.Root.GetPersistendData();
            var entitiesWithCores = GetEntitiesWithCores(persistentData);

            if (entitiesWithCores.Count == 0)
            {
                WriteTo(coreScanLcds, "No entities with cores found for projector updates");
                return;
            }

            var allEntities = CsRoot.Root.GetEntities();
            if (allEntities == null) return;

            var maxDistance = 2000f;
            var nearbyEntities = allEntities
                .Where(entity => UnityEngine.Vector3.Distance(CsRoot.Root.E.Pos, entity.Position) <= maxDistance)
                .Where(entity => entitiesWithCores.Contains(entity.Id)); // Only entities with cores

            var updatedProjectors = 0;

            foreach (var entity in nearbyEntities)
            {
                try
                {
                    updatedProjectors += UpdateProjectorsOnEntity(entity);
                }
                catch
                {
                    continue;
                }
            }

            if (updatedProjectors > 0)
            {
                WriteTo(coreScanLcds, $"Updated {updatedProjectors} projectors on {nearbyEntities.Count()} entities with cores");
            }
        }
        catch (Exception ex)
        {
            WriteTo(coreScanLcds, $"Error updating projectors: {ex.Message}");
        }
    }


    // Helper method to get list of entities that have cores
    private static HashSet<int> GetEntitiesWithCores(ConcurrentDictionary<string, object> persistentData)
    {
        var entitiesWithCores = new HashSet<int>();

        // Check if we have cached information about entities with cores
        if (persistentData.TryGetValue("EntitiesWithCores", out var cacheObj) && cacheObj is HashSet<int> cachedEntities)
        {
            return cachedEntities;
        }

        return entitiesWithCores;
    }

    // Helper method to update projectors on a specific entity
    private static int UpdateProjectorsOnEntity(IEntity entity)
    {
        var updatedProjectors = 0;
        var structure = entity.Structure;
        var minPos = structure.MinPos;
        var maxPos = structure.MaxPos;

        // Apply Y-offset
        var adjustedMinPos = new VectorInt3(minPos.x, 128 + minPos.y, minPos.z);
        var adjustedMaxPos = new VectorInt3(maxPos.x, 128 + maxPos.y, maxPos.z);

        // Efficient scan for existing projectors only
        for (int x = adjustedMinPos.x; x <= adjustedMaxPos.x; x++)
        {
            for (int y = adjustedMinPos.y; y <= adjustedMaxPos.y; y++)
            {
                for (int z = adjustedMinPos.z; z <= adjustedMaxPos.z; z++)
                {
                    var blockPos = new VectorInt3(x, y, z);
                    try
                    {
                        var block = structure.GetBlock(blockPos);
                        if (block != null)
                        {
                            block.Get(out int blockType, out _, out _, out _);
                            if (blockType == 1400) // LCD projector
                            {
                                UpdateExistingProjector(structure, blockPos);
                                updatedProjectors++;
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        return updatedProjectors;
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
        1401,
        1402,
        2768,
        2769,
        2770,
        2771,
        //3148, // salvage core
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

    // Replace the PlaceLcdProjector method with this updated version:
    private static string PlaceLcdProjector(ICsScriptFunctions CsRoot, IEntity entity, IStructure structure, VectorInt3 corePos, ILcd[] debugLcds)
    {
        var results = new List<string>();

        try
        {
            // Find and place up to 5 projectors above the core
            var abovePositions = FindEmptySpaces(structure, corePos, 1, 100, 5); // Search 100, place max 5
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
            var belowPositions = FindEmptySpaces(structure, corePos, -1, 100, 5); // Search 100, place max 5
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