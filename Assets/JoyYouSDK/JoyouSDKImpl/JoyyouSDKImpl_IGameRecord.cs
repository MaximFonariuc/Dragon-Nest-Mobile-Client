namespace Assets.SDK
{
	public delegate void IGameRecord_DelayInit();

	public partial class JoyYouSDK : IGameRecord
	{
		static bool isSupported_IGameRecord = false;

		void IGameRecord.PauseRecording()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_PauseRecording();
			}
		}

		void IGameRecord.ResumeRecording()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_ResumeRecording();
			}
		}

		void IGameRecord.StartRecording()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_StartRecording();
			}
		}

		void IGameRecord.StopRecording()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_StopRecording();
			}
		}

		void IGameRecord.ShowControlBar(bool visible)
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_ShowCtrlBar(visible);
			}
		}

		void IGameRecord.ShowWelfareCenter()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_ShowWelfareCenter();
			}
		}

		void IGameRecord.ShowVideoStore()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_ShowVideoStore();
			}
		}

		void IGameRecord.ShowPlayerClub()
		{
			if (isSupported_IGameRecord)
			{
				doIGameRecord_DelayInit();
				JoyYouNativeInterface.GameRecordItf_ShowPlayerClub();
			}
		}
		
		public static event IGameRecord_DelayInit doDelayInit;

		bool isDelayInitDone = false;
		void doIGameRecord_DelayInit()
		{
			if (!isDelayInitDone && doDelayInit != null)
			{
				try
				{
					doDelayInit();
					isDelayInitDone = true;
				}
				catch (System.Exception ex)
				{
					System.Console.WriteLine(ex.StackTrace);	
				}
			}
		}

		public static void AddEventObs( IGameRecord_DelayInit ds )
		{
			doDelayInit += ds;
		}
	}
}
