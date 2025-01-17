namespace Assets.SDK
{
	public partial class JoyYouSDK : IBaiduStat
	{
		enum eventType
		{
			nothing = 0,
			eventStat = 1,
			eventStart = 2,
			eventEnd = 3,
			pageStart = 4,
			pageEnd = 5,
			eventWithDuration = 6
		};
		
		void IBaiduStat.singleEventLog(string eventId, string eventLabel)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.eventStat, eventId, eventLabel, 0);
		}
		
		void IBaiduStat.eventStart(string eventId, string eventLabel)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.eventStart, eventId, eventLabel, 0);
		}
		void IBaiduStat.eventEnd(string eventId, string eventLabel)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.eventEnd, eventId, eventLabel, 0);
		}
		
		void IBaiduStat.eventWithDurationTime(string eventId, string eventLabel, int duration)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.eventWithDuration, eventId, eventLabel, duration);
		}
		
		void IBaiduStat.pageStart(string pageName)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.pageStart, pageName, "", 0);
		}
		
		void IBaiduStat.pageEnd(string pageName)
		{
			JoyYouNativeInterface.BaiduStat((int)eventType.pageEnd, pageName, "", 0);
		}
	}
}
