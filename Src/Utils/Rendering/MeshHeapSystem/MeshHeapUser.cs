using System;
using Godot;

namespace Jomolith.Utils.Rendering.MeshHeapSystem;

[Flags]
public enum MeshHeapUpdateType : byte {
	Transform   = 0b0001,
	Color	   	= 0b0010,
	CustomData  = 0b0100,

	All			= 0b0111,
	None		= 0b0000
}

public abstract class MeshHeapUser<TSelf, TDiscriminator>
where TSelf : MeshHeapUser<TSelf, TDiscriminator>
where TDiscriminator : struct, IHeapDiscriminator<TDiscriminator> {

	public MeshHeapBlock<TSelf,TDiscriminator> HeapBlock { get; internal set; } = null!;
	internal MeshHeapUpdateType HeapUpdate;
	internal int HeapBlockIndex;

	public void Remove() {
		HeapBlock.RemoveAt(HeapBlockIndex);
	}

	public void QueueUpdate(MeshHeapUpdateType type) {
		if (HeapUpdate != MeshHeapUpdateType.None) {
			HeapUpdate |= type;
			return;
		}
		HeapUpdate |= type;
		HeapBlock.QueueUpdateIndex(HeapBlockIndex);
	}

	public MeshHeap<TSelf, TDiscriminator> Heap => HeapBlock.ParentHeap;

	public abstract TDiscriminator HeapDiscriminator { get; }
	public abstract Transform3D HeapTransform { get; }
	public abstract Color HeapColor { get; }
	public abstract Color HeapCustomData { get; }
}
