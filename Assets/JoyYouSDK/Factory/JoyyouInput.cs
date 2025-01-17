namespace Assets.SDK.JoyyouInput
{
	public class Joystick
	{
		public enum KeyState
		{
			UP,
 			DOWN,
		}

		public struct JoyAngle
		{
			public float x;
			public float y;
		}

		public static KeyState KeyA { get; private set; }
		public static KeyState KeyB { get; private set; }
		public static KeyState KeyX { get; private set; }
		public static KeyState KeyY { get; private set; }
		public static KeyState KeyBack { get; private set; }
		public static KeyState KeyStart { get; private set; }
		public static KeyState KeyL1 { get; private set; }
		public static float KeyL2 { get; private set; }
		public static KeyState KeyR1 { get; private set; }
		public static float KeyR2 { get; private set; }
		public static KeyState KeyHome { get; private set; }
		public static KeyState KeyHelp { get; private set; }
		public static KeyState KeyDirectionLeft { get; private set; }
		public static KeyState KeyDirectionRight { get; private set; }
		public static KeyState KeyDirectionUp { get; private set; }
		public static KeyState KeyDirectionDown { get; private set; }
        public static KeyState KeyOK { get; private set; }
		public static KeyState KeyJL { get; private set; }
		public static KeyState KeyJR { get; private set; }

		public static KeyState KeyMenu { get; private set; }

		private static JoyAngle left_joy = new JoyAngle();
		private static JoyAngle right_joy = new JoyAngle();

		public static JoyAngle JoyLeft
		{
			get	{ return left_joy; }
		}

		public static JoyAngle JoyRight
		{
			get { return right_joy; }
		}

		static Joystick()
		{
			Rest();
		}

		public Joystick()
		{
			// if needed
		}

		public static void Rest()
		{
			KeyA = KeyState.UP;
			KeyB = KeyState.UP;
			KeyX = KeyState.UP;
			KeyY = KeyState.UP;
			KeyL1 = KeyState.UP;
			KeyL2 = 0.0f;
			KeyR1 = KeyState.UP;
			KeyR2 = 0.0f;
			KeyStart = KeyState.UP;
			KeyBack = KeyState.UP;
			KeyHome = KeyState.UP;
			KeyHelp = KeyState.UP;
			KeyDirectionLeft = KeyState.UP;
			KeyDirectionRight = KeyState.UP;
			KeyDirectionUp = KeyState.UP;
			KeyDirectionDown = KeyState.UP;
			KeyMenu = KeyState.UP;
            KeyOK = KeyState.UP;
			left_joy.x = left_joy.y = 0.0f;
			right_joy.x = right_joy.y = 0.0f;
		}

		public void ParserPhysicMessage(string msg)
		{
			bool ret = true;
			string[] events = msg.Split(';');
			foreach (string e in events)
			{
				ret = ResetEvent(e);
				if (!ret)
				{
					break;
				}
			}
		}

		private bool ResetEvent(string ent)
		{
			string[] des = ent.Split(':');
			bool retValue = true;
			if (des.Length == 2)
			{
				string eventName = des[0];
				if (eventName.Equals("A"))
				{
					KeyA = (KeyState) int.Parse(des[1]);
				}
				else if (eventName.Equals("B"))
				{
					KeyB = (KeyState) int.Parse(des[1]);
				}
				else if (eventName.Equals("X"))
				{
					KeyX = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("Y"))
				{
					KeyY = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("L1"))
				{
					KeyL1 = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("L2"))
				{
					KeyL2 = float.Parse(des[1]);
				}
				else if (eventName.Equals("R1"))
				{
					KeyR1 = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("R2"))
				{
					KeyR2 = float.Parse(des[1]);
				}
				else if (eventName.Equals("HOME"))
				{
					KeyHome = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("BACK"))
				{
					KeyBack = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("START"))
				{
					KeyStart = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("HELP"))
				{
					KeyHelp = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("UP"))
				{
					KeyDirectionUp = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("DOWN"))
				{
					KeyDirectionDown = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("LEFT"))
				{
					KeyDirectionLeft = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("RIGHT"))
				{
					KeyDirectionRight = (KeyState)int.Parse(des[1]);
				}
                else if (eventName.Equals("OK"))
                {
                    KeyOK = (KeyState)int.Parse(des[1]);
                }
				else if (eventName.Equals("JL"))
				{
					KeyJL = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("JR"))
				{
					KeyJR = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("MENU"))
				{
					KeyMenu = (KeyState)int.Parse(des[1]);
				}
				else if (eventName.Equals("JLXY"))
				{
					string [] posXY = des[1].Split(',');
					left_joy.x = float.Parse(posXY[0]);
					left_joy.y = float.Parse(posXY[1]);
				}
				else if (eventName.Equals("JRXY"))
				{
					string[] posXY = des[1].Split(',');
					right_joy.x = float.Parse(posXY[0]);
					right_joy.y = float.Parse(posXY[1]);
				}
			}
			else
			{
				retValue = false;
			}

			return retValue;
		}
	}
}
