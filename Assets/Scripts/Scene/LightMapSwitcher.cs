using UnityEngine;

using System.Linq;

public class LightMapSwitcher : MonoBehaviour
{
	//public Texture2D[] DayNear;
	public Texture2D[] DayFar;
	//public Texture2D[] NightNear;
	public Texture2D[] NightFar;
	
	private LightmapData[] dayLightMaps;
	private LightmapData[] nightLightMaps;
	
	void Start ()
	{

		
		// Sort the Day and Night arrays in numerical order, so you can just blindly drag and drop them into the inspector
		//DayNear = DayNear.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
		DayFar = DayFar.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
		//NightNear = NightNear.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
		NightFar = NightFar.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
		
		// Put them in a LightMapData structure
		dayLightMaps = new LightmapData[DayFar.Length];
		for (int i=0; i<DayFar.Length; i++)
		{
			dayLightMaps[i] = new LightmapData();
			dayLightMaps[i].lightmapDir = DayFar[i];
			dayLightMaps[i].lightmapColor = DayFar[i];
		}
		
		nightLightMaps = new LightmapData[NightFar.Length];
		for (int i=0; i<NightFar.Length; i++)
		{
			nightLightMaps[i] = new LightmapData();
			nightLightMaps[i].lightmapDir = NightFar[i];
			nightLightMaps[i].lightmapColor = NightFar[i];
		}
	}
	
	#region Publics
	public void SetToDay()
	{
		LightmapSettings.lightmaps = dayLightMaps;
	}
	
	public void SetToNight()
	{
		LightmapSettings.lightmaps = nightLightMaps;
	}
	#endregion
	
	#region Debug
	[ContextMenu ("Set to Night")]
	void Debug00()
	{
		SetToNight();
	}
	
	[ContextMenu ("Set to Day")]
	void Debug01()
	{
		SetToDay();
	}
	#endregion
}
