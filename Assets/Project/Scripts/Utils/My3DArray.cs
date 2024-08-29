using System;
using UnityEngine;

public class My3DArray<T> {

	private readonly T[] arr;
	private readonly int width;
	private readonly int height;
	private readonly int length;

	public My3DArray(int width, int height, int length) {
		arr = new T [width * height * length];
		this.width = width;
		this.height = height;
		this.length = length;
	}

	public T this[int x, int y, int z] {
		get {
			if (outOfRange(x, y, z)) throw new IndexOutOfRangeException();
			return arr[z * width * height + y * width + x];

		}
		set {
			if (outOfRange(x, y, z)) throw new IndexOutOfRangeException();
			arr[z * width * height + y * width + x] = value;
		}
	}

	public T this[Vector3Int pos] {
		get => this[pos.x, pos.y, pos.z];
		set => this[pos.x, pos.y, pos.z] = value;
	}

	private bool outOfRange(int x, int y, int z) {
		return 0 > x || x >= width ||
		       0 > y || y >= height ||
		       0 > z || z >= length;
	}

}
