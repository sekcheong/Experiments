using System;
using System.Collections.Generic;
using System.Diagnostics;

static class Program
{
	public class DynamicArray<T> : IEnumerable<T>
	{

		private T[] _items;
		private int _capacity;
		private double _loadFactor;
		private const double DEFAULT_LOADFACTOR = 0.5;
		private const int DEFAULT_CAPACITY = 16;
		private int _count;		

		public DynamicArray()
		{
			this.Init(DEFAULT_CAPACITY, DEFAULT_LOADFACTOR, null);
		}

		public DynamicArray(int capacity)
		{
			this.Init(capacity, DEFAULT_LOADFACTOR, null);
		}

		public DynamicArray(int capacity, double loadFactor)
		{
			this.Init(capacity, loadFactor, null);
		}

		public DynamicArray(List<T> items)
		{
			this.Init(DEFAULT_CAPACITY, DEFAULT_LOADFACTOR, items.ToArray());
		}

		public DynamicArray(T[] items)
		{
			this.Init(DEFAULT_CAPACITY, DEFAULT_LOADFACTOR, items);
		}

		public DynamicArray(int capacity, double loadFactor, List<T> items)
		{
			this.Init(capacity, loadFactor, items.ToArray());
		}

		public DynamicArray(int capacity, double loadFactor, T[] items)
		{
			this.Init(capacity, loadFactor, items);
		}

		private void Init(int capacity, double loadFactor, T[] items)
		{
			_capacity = capacity;
			_loadFactor = loadFactor;

			if (items != null) _capacity = items.Length;
			
			if (_capacity < 1) 	_capacity = DEFAULT_CAPACITY;
			if (!(_loadFactor > 0.2 && _loadFactor < 0.8)) _loadFactor = DEFAULT_LOADFACTOR;
						
			_items = new T[_capacity];
			if (items != null) {
				for (int i = 0; i < items.Length; i++) {
					_items[i] = items[i];
				}
				_count = items.Length;
			}
			else {
				_count = 0;
			}

		}


		public int Count { get { return _count; } }

		
		public int Capacity { get {return _items.Length;}}


		public T this[int index]
		{
			get { return _items[index]; }
		}


		public void Add(T item) {
			if (_count == _items.Length) Expand();						
			_items[_count] = item;
			_count++;
		}


		private void Expand()
		{
			int size = (int)((double)_items.Length / _loadFactor);
			T[] s = new T[size];
			Array.Copy(_items, s, _count);
			_items = s;
		}

		
		public T[] Compact()
		{
			if (_items.Length == _count) return _items;
			T[] s = new T[_count];
			Array.Copy(_items, s, _count);
			s = _items;
			return _items;
		}


		public IEnumerator<T> GetEnumerator()
		{			
			return new DynamicArrayEnumerator<T>(_items, _count);
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{		
			return _items.GetEnumerator();
		}

	}


	public class DynamicArrayEnumerator<T> : IEnumerator<T>
	{
		private T[] _items;
		private int _count;
		private int _pos;

		public DynamicArrayEnumerator(T[] items, int count)
		{
			this.Reset();
			_items = items;
			_count = count;
		}

		public T Current
		{
			get { return _items[_pos]; }
		}

		public void Dispose()
		{
			
		}

		object System.Collections.IEnumerator.Current
		{
			get { return  (object) _items[_pos]; }
		}

		public bool MoveNext()
		{
			_pos++;
			return (_pos < _count);
		}

		public void Reset()
		{
			_pos = -1;
		}
	}
	
	static void Main()
	{
		const int REPEATS = 50;
		const int SIZE = 1000000;
		List<int> list = new List<int>(SIZE);
		Random rand = new Random(12345);
		for (int i = 0; i < SIZE; i++) {
			list.Add(rand.Next(5000));
		}
		int[] arr = list.ToArray();
		DynamicArray<int> darr = new DynamicArray<int>();

		foreach (int v in list) {
			darr.Add(v);
		}

		long hits = 0;
		long chk = 0;
		Stopwatch watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			int len = list.Count;
			for (int i = 0; i < len; i++) {
				chk += list[i];
				hits++;
			}
		}
		watch.Stop();
		Console.WriteLine("List/for      : {0}ms ({1})  ", watch.ElapsedMilliseconds, chk);

		chk = 0;
		hits = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			for (int i = 0; i < arr.Length; i++) {
				chk += arr[i];
				hits++;
			}			
		}
		watch.Stop();
		Console.WriteLine("Array/for     : {0}ms ({1})", watch.ElapsedMilliseconds, chk);


		chk = 0;
		hits = 0;
		watch = Stopwatch.StartNew();
		int[] arr2 = darr.Compact();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			for (int i = 0; i < arr2.Length; i++) {
				chk += arr2[i];
				hits++;
			}
		}
		watch.Stop();
		Console.WriteLine("DArray/for    : {0}ms ({1})", watch.ElapsedMilliseconds, chk);



		Console.WriteLine();
		chk = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			foreach (int i in list) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("List/foreach  : {0}ms ({1})", watch.ElapsedMilliseconds, chk);

		chk = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			foreach (int i in arr) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("Array/foreach : {0}ms ({1})", watch.ElapsedMilliseconds, chk);


		chk = 0;		
		watch = Stopwatch.StartNew();
		int[] c = darr.Compact();
		for (int rpt = 0; rpt < REPEATS; rpt++) {
			foreach (int i in c) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("DArray/foreach: {0}ms ({1})", watch.ElapsedMilliseconds, chk);
		Console.WriteLine();
		Console.WriteLine("Total number of hits: {0:N0}", hits);
		Console.ReadKey();
	}
}


//long StopBytes = 0;
//foo myFoo;

//long StartBytes = System.GC.GetTotalMemory(true);
//myFoo = new foo();
//StopBytes = System.GC.GetTotalMemory(true);
//GC.KeepAlive(myFoo); // This ensure a reference to object keeps object in memory