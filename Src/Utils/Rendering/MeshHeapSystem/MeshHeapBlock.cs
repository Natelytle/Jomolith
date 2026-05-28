using Godot;
using System;
using System.Collections.Generic;

namespace Jomolith.Utils.Rendering.MeshHeapSystem;

// ReSharper disable once Godot.MissingParameterlessConstructor
public partial class MeshHeapBlock<TUser, TDiscriminator> : MultiMeshInstance3D
where TUser : MeshHeapUser<TUser, TDiscriminator>
where TDiscriminator : struct, IHeapDiscriminator<TDiscriminator> {
	public readonly int Capacity;
	public readonly int MeshIndex;
	public readonly int MaterialIndex;
	public readonly TDiscriminator Discriminator;
	public Vector3I GeometricOrigin { get; init; } = Vector3I.Zero;

	public int Count { get; protected set; } = 0;
	public float Usage => (float)Count / (float)Capacity;
	public bool PotentiallyFragmented => Usage <= Heap.DefragThreshold;
	public bool IsFull => Count == Capacity;
	public bool IsEmpty => Count == 0;
	public bool Dead => dead;

	protected MeshHeap<TUser, TDiscriminator> Heap;
	protected Mesh Mesh;
	protected readonly TUser[] Users;
	protected readonly Stack<int> UpdateRequests = new Stack<int>();
	protected int PreviousCount = 0;
	private bool dead = false;

	public MeshHeap<TUser, TDiscriminator> ParentHeap => Heap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		// Update all instances that were queued to be updated
		while (UpdateRequests.TryPop(out int i)) {
			TUser user = Users[i];
			MeshHeapUpdateType updateType = user.HeapUpdate; // Which fields should be updated?

			// Update transform
			if ((updateType & MeshHeapUpdateType.Transform) != 0)
				Multimesh.SetInstanceTransform(i, user.HeapTransform);

			// Update color
			if ((updateType & MeshHeapUpdateType.Color) != 0 && Heap.UsesColor)
				Multimesh.SetInstanceColor(i, user.HeapColor);

			// Update custom data
			if ((updateType & MeshHeapUpdateType.CustomData) != 0 && Heap.UsesCustomData)
				Multimesh.SetInstanceCustomData(i, user.HeapCustomData);
		}

		// The count decreased this frame
		if (Count < PreviousCount) {
			// If this block is empty, it should kill itself, NOW!
			if (IsEmpty) {
				dead = true;
				Heap.RegisterDeadBlock(this);
				return;
			}

			// If the usage expressed as a percentage falls below the threshold,
			// mark this block as potentially fragmented
			if (PotentiallyFragmented) {
				Heap.RegisterFragmentedBlock(this);
			}
		}

		PreviousCount = Count;
	}

	// Should be called from within this class, or from a TUser
	// Enqueues an update to the instance buffer
	public void QueueUpdate(TUser user, MeshHeapUpdateType type) {
		if (user.HeapBlockIndex < 0) return;
		if (user.HeapUpdate != 0) {
			user.HeapUpdate |= type;
			return;
		}
		user.HeapUpdate |= type;
		UpdateRequests.Push(user.HeapBlockIndex);
	}
	public void QueueUpdateIndex(int i) {
		if (i < 0) return;
		UpdateRequests.Push(i);
	}

	// Util function for swapping 2 items in the block
	protected void Swap(int a, int b) {
		TUser userA = Users[a];
		TUser userB = Users[b];
		userA.HeapBlockIndex = b;
		userB.HeapBlockIndex = a;
		(Users[a], Users[b]) = (userB, userA);
		QueueUpdate(userA, MeshHeapUpdateType.All);
		QueueUpdate(userB, MeshHeapUpdateType.All);
	}

	// Appends a user to the block
	internal void AppendUser(TUser user) {
		user.HeapBlock = this;
		user.HeapBlockIndex = Count;
		QueueUpdate(user, MeshHeapUpdateType.All);
		Users[Count++] = user;
		Multimesh.VisibleInstanceCount = Count;
	}

	// Removes an instance from the block
	public void Remove(TUser user) => RemoveAt(user.HeapBlockIndex);
	internal TUser RemoveAt(int i) {

		TUser user = Users[i];
		user.HeapBlockIndex = -1;

		// If we aren't removing the very last instance in the block, we need to move it down to this now-empty slot
		if (i != Count - 1) {
			TUser latest = Users[Count - 1];
			latest.HeapBlockIndex = i;
			Users[i] = latest;
			QueueUpdate(latest, MeshHeapUpdateType.All);
		}

		Count--;
		Multimesh.VisibleInstanceCount = Count;

		return user;
	}

	// This method helps determine if an instance of certain mesh index and discriminator can be inserted into this block
	internal bool IsVacant(TDiscriminator otherDiscriminator, int otherMeshIndex, int otherMaterialIndex) {
		if (IsFull) return false;
		if (otherMeshIndex != MeshIndex) return false;
		if (otherMaterialIndex != MaterialIndex) return false;
		return otherDiscriminator.Matches(Discriminator);
	}

	public MeshHeapBlock(
		MeshHeap<TUser, TDiscriminator> heap,
		int capacity,
		int meshIndex,
		int materialIndex,
		TDiscriminator discriminator
	) {
		Heap = heap;
		Capacity = capacity;
		Users = new TUser[capacity];

		MeshIndex = meshIndex;
		MaterialIndex = materialIndex;
		Mesh = heap.Meshes[meshIndex];
		MaterialOverride = heap.Materials[materialIndex];

		// Setup the multimesh
		MultiMesh mm = new() {
			Mesh = Mesh,
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			InstanceCount = 0,
			VisibleInstanceCount = 0,

			UseColors = heap.UsesColor,
			UseCustomData = heap.UsesCustomData
		};
		mm.InstanceCount = capacity;
		Multimesh = mm;

		Discriminator = discriminator;
	}
}

