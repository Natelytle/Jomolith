using Godot;
using System;
using System.Collections.Generic;

namespace Jomolith.Utils.Rendering.MeshHeapSystem;

public partial class MeshHeap<TUser, TDiscriminator> : Node3D
where TUser : MeshHeapUser<TUser, TDiscriminator>
where TDiscriminator : struct, IHeapDiscriminator<TDiscriminator> {

	public required Mesh[] Meshes { get; init; }
	public required Material[] Materials { get; init; }
	public bool UsesColor { get; init; } = true;
	public bool UsesCustomData { get; init; } = true;

	public int BlockCapacity = 4096;
	public float BlockGeometricSize = 2048.0f;
	public bool BlockGeometricSorting = false;
	public float DefragThreshold = 0.4f;
	public int MaxSwapsPerFrame = 1024;

	private readonly List<MeshHeapBlock<TUser,TDiscriminator>> blocks = new List<MeshHeapBlock<TUser, TDiscriminator>>();
	private bool defragCheck = false;
	private List<BlockGroup> defragGroups = new List<BlockGroup>();
	private List<MeshHeapBlock<TUser,TDiscriminator>> deadBlocks = new List<MeshHeapBlock<TUser, TDiscriminator>>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	// Should only be called from a block
	internal void RegisterDeadBlock(MeshHeapBlock<TUser,TDiscriminator> block) {
		deadBlocks.Add(block);
	}

	#region Defragging Process

	// Record used for attaching unique properties to a list of blocks
	private record BlockGroup(
		List<MeshHeapBlock<TUser, TDiscriminator>> Blocks,
		TDiscriminator Discriminator,
		int MeshIndex,
		int MaterialIndex
	);

	/*
	private List<BlockGroup> FindFragmentedBlocks() {
		List<BlockGroup> groups = new();

		foreach (var block in blocks) {
			float usage = (float)block.Count / (float)block.Capacity;
			if (usage > DefragThreshold) continue;

			// If we made it past the guard clause, congratulations, this block is potentially fragmented


		}
	}
	*/

	// Should only be called by the blocks themselves
	// Blocks automatically register to the heap as potentially fragmented if:
	// - Their count is less than it was previous frame
	// - Their count is below the heap's defrag threshold (DefragThreshold)
	internal void RegisterFragmentedBlock(MeshHeapBlock<TUser,TDiscriminator> block) {
		defragCheck = true;

		// Find a matching group and add it to the record's list
		// If we can't find a matching group, simply create one
		foreach (var group in defragGroups) {
			if (!group.Discriminator.Matches(block.Discriminator)) continue;
			if (group.MeshIndex != block.MeshIndex) continue;
			if (group.MaterialIndex != block.MaterialIndex) continue;

			// We got past all the guard clauses, that means this is the matching group!
			group.Blocks.Add(block);
			return;
		}

		// If we got here, we found no matching blocks :(, we must create a new one
		BlockGroup newGroup = new(
			new List<MeshHeapBlock<TUser, TDiscriminator>>(),
			block.Discriminator,
			block.MeshIndex,
			block.MaterialIndex
		);
		newGroup.Blocks.Add(block);
		defragGroups.Add(newGroup);
	}

	// Defrag blocks in groups of matching unique properties (discriminator, mesh index, material index)
	// Will not make more than maxSwaps swaps
	// If it doesn't use up all swaps, it will turn defragCheck off to prevent automatically defragging next frame redundantly
	private void Defrag(List<BlockGroup> groups, int maxSwaps) {

		// Keep track of how many more swaps we may make this frame
		// We subtract 1 so that we can use a value of -1 to detect if we've run out of swaps
		int swapsLeft = maxSwaps - 1;
		GD.Print($"Defragging.. ({swapsLeft})");

		foreach (var group in groups) {
			swapsLeft = DefragGroup(group, swapsLeft + 1) - 1;

			// Terminate if we ran out of swaps
			if (swapsLeft < 0) return;
		}

		// We didn't run out of swaps! yay!
		defragCheck = false;
	}

	// Defrags a single group of potentially fragmented blocks
	// Returns how many swaps are left after being allotted maxSwaps swaps
	// Also assumes that maxSwaps is 1 less than it should be, so that -1 can be used to say "I ran out of swaps :("
	private int DefragGroup(BlockGroup group, int maxSwaps) {
		// We must keep track of blocks that we deemed to not actually be fragmented
		// This can happen if a block registered itself as fragmented but was later appended to
		// ... or if it just can't be paired up with another block for now
		// ... or if we successfully defragged it lol
		Stack<int> notFragmented = new();
		int swapsLeft = maxSwaps;

		// For every pair of potentially fragmented blocks:
		// - Double check to make sure both are actually fragmented (and not dead)
		// - Transfer as many instances from block B into block A as possible
		for (int i = 0; i+1 < group.Blocks.Count; i += 2) {
			MeshHeapBlock<TUser,TDiscriminator> blockA = group.Blocks[i];
			MeshHeapBlock<TUser,TDiscriminator> blockB = group.Blocks[i+1];

			// Double check to see if this block is still fragmented
			if (blockA.IsEmpty || !blockA.PotentiallyFragmented) {
				notFragmented.Push(i);
				continue;
			}
			if (blockB.IsEmpty || !blockB.PotentiallyFragmented) {
				notFragmented.Push(i+1);
				continue;
			}

			// Calculate how many swaps we can safely make in bulk
			int swapCount = Math.Min(blockA.Capacity - blockA.Count, blockB.Count);
			swapCount = Math.Min(swapCount, swapsLeft);
			swapsLeft -= swapCount;

			// Finally, we can swap instances from block B to A
			GD.Print($"Defragging {swapCount} instances");
			for (int j = 0; j < swapCount; j++) {
				// Remove the very last item in block B to avoid making it perform an extra swap and IBO update
				TUser refugee = blockB.RemoveAt(blockB.Count - 1);
				blockA.AppendUser(refugee);
			}
		}

		// Remove blocks that aren't actually fragmented from the apparatus
		while (notFragmented.TryPop(out int i)) {
			group.Blocks.RemoveAt(i);
		}

		// If there's a single block, it can't be defragged lol
		if (group.Blocks.Count == 1) group.Blocks.Clear();

		return swapsLeft;
	}
	#endregion

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		// Check to see if we gotta try defragging
		if (defragCheck) {
			// Find fragmented groups of blocks
			//List<BlockGroup> fragmentedBlocks = FindFragmentedBlocks();

			// Defrag 'em
			Defrag(defragGroups, MaxSwapsPerFrame);
		}

		// Remove dead blocks
		if (deadBlocks.Count > 0) {
			GD.Print($"Removing {deadBlocks.Count} dead blocks..");
			foreach (var block in deadBlocks) {
				// Double-check just in case :)
				if (block.Count > 0) continue;

				blocks.Remove(block);
				RemoveChild(block);
				block.QueueFree();
			}

			deadBlocks.Clear();
		}

		foreach (var block in blocks) {
			if (block.Dead) GD.Print("DEAD BLOCK IS STILL ACTIVE");
		}
	}

	#region Block Management
	public MeshHeapBlock<TUser,TDiscriminator> GetVacantBlock(TUser user, int meshIndex, int materialIndex) {
		// If geometric sorting is enabled, we must figure out which section of the world the block must be in
		Vector3I blockPos = Vector3I.Zero;
		if (BlockGeometricSorting) {
			Vector3 pos = user.HeapTransform.Origin / BlockGeometricSize;
			blockPos = (Vector3I)pos;
		}

		// Search for a vacant block
		foreach (var block in blocks) {
			// If geomtric sorting is enabled, this block must be in the appropriate section
			if (BlockGeometricSorting) {
				if (blockPos != block.GeometricOrigin) continue;
			}

			// If this block is vacant (which implies a matching discriminator), return this block
			if (block.IsVacant(user.HeapDiscriminator, meshIndex, materialIndex)) return block;
		}

		// At this point, no vacant block was found, we must create a new one instead
		MeshHeapBlock<TUser,TDiscriminator> newBlock = new(this, BlockCapacity, meshIndex, materialIndex, user.HeapDiscriminator) {
			GeometricOrigin = blockPos
		};
		blocks.Add(newBlock);
		AddChild(newBlock);

		return newBlock;
	}

	public void AppendInstance(TUser user, int meshIndex, int materialIndex) {
		// Get a vacant block
		MeshHeapBlock<TUser,TDiscriminator> block = GetVacantBlock(user, meshIndex, materialIndex);
		block.AppendUser(user);
	}
	#endregion
}
