using Assets.SDK.JoyyouInput;
namespace Assets.SDK
{
	public partial class JoyYouSDK : IDeviceInfomation
	{
		private static Joystick joystick = null;

		string IDeviceInfomation.GetMACAddress()
		{
			return JoyYouNativeInterface.GetMACAddress();
		}

		Joystick IDeviceInfomation.GetJoystick()
		{
			if (joystick == null)
				joystick = new Joystick();
			return joystick;
		}

		void IDeviceInfomation.ProcessJoystick(string message)
		{
			((IDeviceInfomation)this).GetJoystick().ParserPhysicMessage(message);
		}
	}
}
