﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

static class Program
{
	public class DynamicArray<T>:IEnumerable<T> {

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
			for (int i = 0; i < _count; i++) {
				s[i] = _items[i];
			}
			_items = s;
		}

		
		public T[] Shrink()
		{
			if (_items.Length == _count) return _items;
			T[] s = new T[_count];
			for (int i = 0; i < _count; i++) {
				s[i] = _items[i];
			}
			s = _items;
			return _items;
		}


		public IEnumerator<T> GetEnumerator()
		{
			Shrink();
			foreach (T v in _items) {
				yield return v;
			}
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			Shrink();
			return _items.GetEnumerator();
		}
	}


	
	static void Main()
	{		
		List<int> list = new List<int>(6000000);
		Random rand = new Random(12345);
		for (int i = 0; i < 6000000; i++) {
			list.Add(rand.Next(5000));
		}
		int[] arr = list.ToArray();
		DynamicArray<int> darr = new DynamicArray<int>();

		foreach (int v in list) {
			darr.Add(v);
		}
		int hits = 0;
		int chk = 0;
		Stopwatch watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < 100; rpt++) {
			int len = list.Count;
			for (int i = 0; i < len; i++) {
				chk += list[i];
				hits++;
			}
		}
		watch.Stop();
		Console.WriteLine("List/for: {0}ms ({1}) hits:{2}", watch.ElapsedMilliseconds, chk, hits);

		chk = 0;
		hits = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < 100; rpt++) {
			for (int i = 0; i < arr.Length; i++) {
				chk += arr[i];
				hits++;
			}
		}
		watch.Stop();
		Console.WriteLine("Array/for: {0}ms ({1})", watch.ElapsedMilliseconds, chk);


		chk = 0;
		hits = 0;
		watch = Stopwatch.StartNew();
		int[] arr2 = darr.Shrink();		
		for (int rpt = 0; rpt < 100; rpt++) {
			for (int i = 0; i < arr2.Length; i++) {
				chk += arr2[i];
				hits++;
			}
		}
		watch.Stop();
		Console.WriteLine("Dynamic Array/for: {0}ms ({1})", watch.ElapsedMilliseconds, chk);


		chk = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < 100; rpt++) {
			foreach (int i in list) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("List/foreach: {0}ms ({1})", watch.ElapsedMilliseconds, chk);

		chk = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < 100; rpt++) {
			foreach (int i in arr) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("Array/foreach: {0}ms ({1})", watch.ElapsedMilliseconds, chk);


		chk = 0;
		watch = Stopwatch.StartNew();
		for (int rpt = 0; rpt < 100; rpt++) {
			foreach (int i in darr) {
				chk += i;
			}
		}
		watch.Stop();
		Console.WriteLine("Dynamic Array/foreach: {0}ms ({1})", watch.ElapsedMilliseconds, chk);

		Console.ReadLine();
	}
}


//long StopBytes = 0;
//foo myFoo;

//long StartBytes = System.GC.GetTotalMemory(true);
//myFoo = new foo();
//StopBytes = System.GC.GetTotalMemory(true);
//GC.KeepAlive(myFoo); // This ensure a reference to object keeps object in memory