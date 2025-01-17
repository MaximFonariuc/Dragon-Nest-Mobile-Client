// The MIT License (MIT)

// Copyright 2015 Siney/Pangweiwei siney@yeah.net
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace com.tencent.pandora
{
	using System;
	using System.Collections.Generic;
	
	public class LuaTimer : LuaObject
	{
		class Timer
		{
			internal int sn;
			internal int cycle;
			internal int deadline;
			internal Func<int, bool> handler;
			internal bool delete;
			internal LinkedList<Timer> container;
		}
		class Wheel
		{
			internal static int dial_scale = 256;
			internal int head;
			internal LinkedList<Timer>[] vecDial;
			internal int dialSize;
			internal int timeRange;
			internal Wheel nextWheel;
			internal Wheel(int dialSize)
			{
				this.dialSize = dialSize;
				this.timeRange = dialSize * dial_scale;
				this.head = 0;
				this.vecDial = new LinkedList<Timer>[dial_scale];
				for (int i = 0; i < dial_scale; ++i)
				{
					this.vecDial[i] = new LinkedList<Timer>();
				}
			}
			internal LinkedList<Timer> nextDial()
			{
				return vecDial[head++];
			}
			internal void add(int delay, Timer tm)
			{
				var index = (head + (delay - (dialSize - jiffies_msec)) / dialSize) % dial_scale;
				var container = vecDial[index];
				container.AddLast(GetTimerNodeFormPool(tm));
				tm.container = container;
			}
		}
		static int nextSn = 0;
		static int jiffies_msec = 20;
		static float jiffies_sec = jiffies_msec * .001f;
		static Wheel[] wheels;
		static float pileSecs;
		static float nowTime;
		static Dictionary<int, Timer> mapSnTimer;
		static LinkedList<Timer> executeTimers;
		
		static int intpow(int n, int m)
		{
			int ret = 1;
			for (int i = 0; i < m; ++i)
				ret *= n;
			return ret;
		}
		
		static void innerAdd(int deadline, Timer tm)
		{
			tm.deadline = deadline;
			int delay = Math.Max(0, deadline - now());
			Wheel suitableWheel = wheels[wheels.Length - 1];
			for (int i = 0; i < wheels.Length; ++i)
			{
				var wheel = wheels[i];
				if (delay < wheel.timeRange)
				{
					suitableWheel = wheel;
					break;
				}
			}
			suitableWheel.add(delay, tm);
		}
		
		static void innerDel(Timer tm)
		{
			innerDel(tm, true);
		}
		
		static void innerDel(Timer tm,bool removeFromMap)
		{
			tm.delete = true;
			if (tm.container != null)
			{
				LinkedListNode<Timer> node = tm.container.Find(tm);
				if(node != null)
				{
					tm.container.Remove(node);
					RecycleTimerNodeToPool(node);
				}
				tm.container = null;
			}
			if (removeFromMap) mapSnTimer.Remove(tm.sn);
		}
		
		static int now()
		{
			return (int)(nowTime * 1000);
		}
		
		internal static void tick(float deltaTime)
		{
			nowTime += deltaTime;
			pileSecs += deltaTime;
			int cycle = 0;
			while (pileSecs >= jiffies_sec)
			{
				pileSecs -= jiffies_sec;
				cycle++;
			}
			for (int i = 0; i < cycle; ++i)
			{
				var timers = wheels[0].nextDial();
				LinkedListNode<Timer> node = timers.First;
				for (int j = 0; j < timers.Count; ++j)
				{
					var tm = node.Value;
					executeTimers.AddLast(GetTimerNodeFormPool(tm));
					node = node.Next;
				}
				while(timers.Count > 0)
				{
					LinkedListNode<Timer> recycleNode = timers.First;
					timers.RemoveFirst();
					RecycleTimerNodeToPool(recycleNode);
				}

				for (int j = 0; j < wheels.Length; ++j)
				{
					var wheel = wheels[j];
					if (wheel.head == Wheel.dial_scale)
					{
						wheel.head = 0;
						if (wheel.nextWheel != null)
						{
							var tms = wheel.nextWheel.nextDial();
							LinkedListNode<Timer> tmsNode = tms.First;
							for (int k = 0; k < tms.Count; ++k)
							{
								var tm = tmsNode.Value;
								if (tm.delete)
								{
									mapSnTimer.Remove(tm.sn);
								}
								else
								{
									innerAdd(tm.deadline, tm);
								}
								tmsNode = tmsNode.Next;
							}
							while (tms.Count > 0)
							{
								LinkedListNode<Timer> recycleNode = tms.First;
								tms.RemoveFirst();
								RecycleTimerNodeToPool(recycleNode);
							}
						}
					}
					else
					{
						break;
					}
				}
			}
			
			while (executeTimers.Count > 0)
			{
				var firstNode = executeTimers.First;
				var tm = firstNode.Value;
				RecycleTimerNodeToPool(firstNode);
				executeTimers.RemoveFirst();
				if (!tm.delete && tm.handler(tm.sn) && tm.cycle > 0)
				{
					innerAdd(now() + tm.cycle, tm);
				}
				else
				{
					mapSnTimer.Remove(tm.sn);
				}
			}
		}
		
		static void init()
		{
			int count = 2;
			wheels = new Wheel[count];
			for (int i = 0; i < count; ++i)
			{
				wheels[i] = new Wheel(jiffies_msec * intpow(Wheel.dial_scale, i));
				if (i > 0)
				{
					wheels[i - 1].nextWheel = wheels[i];
				}
			}
			mapSnTimer = new Dictionary<int, Timer>();
			executeTimers = new LinkedList<Timer>();
		}
		
		static int fetchSn()
		{
			return ++nextSn;
		}
		
		internal static int add(int delay, Action<int> handler)
		{
			return add(delay, 0, (int sn) =>
			           {
				handler(sn);
				return false;
			});
		}
		
		internal static int add(int delay, int cycle, Func<int, bool> handler)
		{
			Timer tm = new Timer();
			tm.sn = fetchSn();
			tm.cycle = cycle;
			tm.handler = handler;
			tm.delete = false;
			mapSnTimer[tm.sn] = tm;
			innerAdd(now() + delay, tm);
			return tm.sn;
		}
		
		internal static void del(int sn)
		{
			Timer tm;
			if (mapSnTimer.TryGetValue(sn, out tm))
			{
				innerDel(tm);
			}
		}

        internal static void delAll()
        {
            if(mapSnTimer != null)
            {
                foreach (var t in mapSnTimer)
                {
                    innerDel(t.Value, false);
                }
                mapSnTimer.Clear();
            }
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int Delete(IntPtr l)
		{
			try{
				int id;
				checkType(l, 1, out id);
				del(id);
				return ok(l);
			}catch(Exception e)
			{
				return LuaObject.error(l, e);
			}
		}
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int Add(IntPtr l)
		{
			try{
				int top = LuaDLL.pua_gettop(l);
				if (top == 2)
				{
					int delay;
					checkType(l, 1, out delay);
					LuaDelegate ld;
					checkType(l, 2, out ld);
					Action<int> ua;
					if (ld.d != null)
						ua = (Action<int>)ld.d;
					else
					{
						IntPtr ml = LuaState.get(l).L;
						ua = (int id) =>
						{
							int error = pushTry(ml);
							pushValue(ml, id);
							ld.pcall(1, error);
							LuaDLL.pua_settop(ml, error - 1);
						};
					}
					ld.d = ua;
					pushValue(l, true);
					pushValue(l, add(delay, ua));
					return 2;
				}
				else if (top == 3)
				{
					int delay, cycle;
					checkType(l, 1, out delay);
					checkType(l, 2, out cycle);
					LuaDelegate ld;
					checkType(l, 3, out ld);
					Func<int, bool> ua;
					
					if (ld.d != null)
						ua = (Func<int, bool>)ld.d;
					else
					{
						IntPtr ml = LuaState.get(l).L;
						ua = (int id) =>
						{
							int error = pushTry(ml);
							pushValue(ml, id);
							ld.pcall(1, error);
							bool ret = LuaDLL.pua_toboolean(ml, -1);
							LuaDLL.pua_settop(ml, error - 1);
							return ret;
						};
					}
					ld.d = ua;
					pushValue(l, true);
					pushValue(l, add(delay, cycle, ua));
					return 2;
				}
				return LuaObject.error(l,"Argument error");
			}catch(Exception e)
			{
				return LuaObject.error(l, e);
			}
		}
		
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int DeleteAll(IntPtr l)
		{
			if (mapSnTimer == null) return 0;
			try
			{
                delAll();
				
				pushValue(l, true);
				return 1;
			}
			catch (Exception e)
			{
				return LuaObject.error(l, e);
			}
		}
		
		
		static public void reg(IntPtr l)
		{
			init();
			getTypeTable(l, "LuaTimer");
			addMember(l, Add, false);
			addMember(l, Delete, false);
			addMember(l, DeleteAll, false);
			createTypeMetatable(l, typeof(LuaTimer));
		}

		#region Timer LinkListNode对象池
        private static Queue<LinkedListNode<Timer>> TIMER_NODE_POOL = new Queue<LinkedListNode<Timer>>(64);
		private static int createdCount = 0;
		private static int recycledCount = 0;

		private static LinkedListNode<Timer> GetTimerNodeFormPool(Timer timer)
		{
			LinkedListNode<Timer> node;
			if(TIMER_NODE_POOL.Count == 0)
			{
				node = new LinkedListNode<Timer>(timer);
			}
			else
			{
				node = TIMER_NODE_POOL.Dequeue();
				node.Value = timer;
			}
			return node;
		}

		private static void RecycleTimerNodeToPool(LinkedListNode<Timer> node)
		{
			node.Value = null;
			TIMER_NODE_POOL.Enqueue(node);
		}

		#endregion

	}

}