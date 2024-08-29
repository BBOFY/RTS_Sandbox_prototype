using System;

public class My2DArray<T> {

	private readonly T[] arr;
	private readonly int width;
	private readonly int height;

	public My2DArray(int width, int height) {
		arr = new T [width * height];
		this.width = width;
		this.height = height;
	}

	public T this[int x, int y] {
		get {
			if (outOfRange(x, y)) throw new IndexOutOfRangeException();
			return arr[y * width + x];
		}
		set {
			if (outOfRange(x, y)) throw new IndexOutOfRangeException();
			arr[y * width + x] = value;
		}
	}

	private bool outOfRange(int x, int y) {
		return 0 > x || x >= width ||
		       0 > y || y >= height;
	}
}
