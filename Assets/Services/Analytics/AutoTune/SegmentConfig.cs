using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace Unity.AutoTune {

	// keep this as an immutable object
	public class SegmentConfig 
	{
		public string segment_id;
		public int group_id;
		public Dictionary<string, object> settings;
        public string config_hash;

        // makes a shallow copy of the dictionary - this is required because we will
        // cast the settings to the right type. The supported types may be different
        // the types in JSON e.g. int vs long
		public SegmentConfig(string segment_id, int group_id, Dictionary<string, object> settings, string config_hash)
		{
			this.segment_id = segment_id;
			this.group_id = group_id;
			this.copySettings(settings);
			this.config_hash = config_hash;
		}

		// miniJSON only supports dictionary
		public string ToJsonDictionary() {
			var dict = new Dictionary<string, object>();
	    	dict.Add("segment_id", this.segment_id);
	    	dict.Add("group_id", this.group_id);
	    	dict.Add("settings", this.settings);
	    	dict.Add("config_hash", this.config_hash);
    		return Json.Serialize(dict);
		}

		public static SegmentConfig fromJsonDictionary(string json) {
			var dict = Json.Deserialize(json) as Dictionary<string,object>;
			return new SegmentConfig(
				(string) dict["segment_id"],
				(int)(long) dict["group_id"],
				dict["settings"] as Dictionary<string,object>,
				(string) dict["config_hash"]
				);
		}

        private void copySettings(Dictionary<string, object> settings)
        {
            this.settings = new Dictionary<string, object>();
            if (settings != null)
            {
                foreach (var entry in settings)
                {
                    var settingValue = entry.Value;
                    if (settingValue is long)
                    {
                        this.settings[entry.Key] = (int)(long)settingValue;
                    }
                    else if (settingValue is double)
                    {
                        this.settings[entry.Key] = (float)(double)settingValue;
                    }
                    else
                    {
                        this.settings[entry.Key] = settingValue;
                    }
                }
            }

		}
	}
}
