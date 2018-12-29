using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GlobalstatsIO_StatisticValues
{
	public string key = null;
	public string value = "0";
	public string sorting = null;
	public string rank = "0";
	public string value_change = "0";
	public string rank_change = "0";
}

[System.Serializable]
public class GlobalstatsIO_LinkData
{
	public string url = null;
	public string pin = null;
}

[System.Serializable]
public class GlobalstatsIO_LeaderboardValue
{
	public string name = null;
	public string user_profile = null;
	public string user_icon = null;
	public string rank = "0";
	public string value = "0";
}

[System.Serializable]
public class GlobalstatsIO_Leaderboard {

	public GlobalstatsIO_LeaderboardValue[] data;
}

public class GlobalstatsIO
{
	[System.Serializable]
	private class GlobalstatsIO_AccessToken
	{
		public string access_token = null;
		public string token_type = null;
		public string expires_in = null;
		public Int32 created_at = (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;

		public bool isValid()
		{
			//Check if still valid, allow a 2 minute grace period
			return (created_at + int.Parse(expires_in) - 120) > (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
		}
	}

	[System.Serializable]
	private class GlobalstatsIO_StatisticResponse
	{
		public string name = null;
		public string _id = null;
		[SerializeField]
		public List<GlobalstatsIO_StatisticValues> values = null;
	}

	public static string api_id = "";
	public static string api_secret = "";

	private static GlobalstatsIO_AccessToken api_access_token = null;

	private static List<GlobalstatsIO_StatisticValues> statistic_values = new List<GlobalstatsIO_StatisticValues> ();
	public static string statistic_id = "";
	public static string user_name = "";
	public static GlobalstatsIO_LinkData link_data = null;

	private bool getAccessToken ()
	{
		string url = "https://api.globalstats.io/oauth/access_token";

		WWWForm form = new WWWForm ();
		form.AddField ("grant_type", "client_credentials");
		form.AddField ("scope", "endpoint_client");
		form.AddField ("client_id", api_id);
		form.AddField ("client_secret", api_secret);

		WWW www = new WWW (url, form);

		while (!www.isDone) {
			System.Threading.Thread.Sleep (100);
		}

		// check for errors
		if (www.error == null) {
			UnityEngine.Debug.Log ("WWW getAccessToken Ok!: ");
		} else {
			UnityEngine.Debug.Log ("WWW getAccessToken Error: " + www.error);
			UnityEngine.Debug.Log ("WWW Content: " + www.text);
			return false;
		}

		api_access_token = JsonUtility.FromJson<GlobalstatsIO_AccessToken> (www.text);

		return true;
	}

	public bool share (string id = "", string name = "", Dictionary<string, string> values = null)
	{
		if (api_access_token == null || !api_access_token.isValid()) {
			if (!getAccessToken ()) {
				return false;
			}
		}

		// If no id is supplied but we have one stored, reuse it.
		if (id == "" && statistic_id != "")
			id = statistic_id;

		string url = "https://api.globalstats.io/v1/statistics";
		if (id != "") {
			url = "https://api.globalstats.io/v1/statistics/" + id;
		} else {
			if (name == "")
				name = "anonymous";
		}

		string json_payload = "{\"name\":\"" + name + "\",\"values\":{";

		bool semicolon = false;
		foreach (KeyValuePair<string,string> value in values) {
			if (semicolon)
				json_payload += ",";
			json_payload += "\"" + value.Key + "\":\"" + value.Value + "\"";
			semicolon = true;
		}
		json_payload += "}}";

		byte[] pData = Encoding.ASCII.GetBytes (json_payload.ToCharArray ());

		GlobalstatsIO_StatisticResponse statistic = null;

		if (id == "") {
			Dictionary<string,string> headers = new Dictionary<string,string> ();
			headers.Add ("Authorization", "Bearer " + api_access_token.access_token);
			headers.Add ("Content-Type", "application/json");
			headers.Add ("Content-Length", json_payload.Length.ToString ());

			WWW www = new WWW (url, pData, headers);

			while (!www.isDone) {
				System.Threading.Thread.Sleep (100);
			}

			// check for errors
			if (www.error == null) {
				//UnityEngine.Debug.Log ("WWW POST Ok!");
			} else {
				UnityEngine.Debug.Log ("WWW POST Error: " + www.error);
				UnityEngine.Debug.Log ("WWW POST Content: " + www.text);
				return false;
			}

			statistic = JsonUtility.FromJson<GlobalstatsIO_StatisticResponse> (www.text);
		} else {
			UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Put(url, pData);
			www.SetRequestHeader("Authorization", "Bearer " + api_access_token.access_token);
			www.SetRequestHeader("Content-Type", "application/json");

			UnityEngine.Networking.UploadHandler uploader = new UnityEngine.Networking.UploadHandlerRaw (pData);
			www.uploadHandler = uploader;

			www.Send ();

			while (!www.isDone) {
				System.Threading.Thread.Sleep (100);
			}

			// check for errors
			if (www.error == null) {
				//UnityEngine.Debug.Log ("WWW PUT Ok!");
			} else {
				UnityEngine.Debug.Log ("WWW PUT Error: " + www.error);
				UnityEngine.Debug.Log ("WWW PUT Content: " + Encoding.ASCII.GetString(www.downloadHandler.data));
				return false;
			}

			statistic = JsonUtility.FromJson<GlobalstatsIO_StatisticResponse> (Encoding.ASCII.GetString(www.downloadHandler.data));
		}

		// ID is available only on create, not on update, so do not overwrite it
		if(statistic._id != null && statistic._id != "")
			statistic_id = statistic._id;

		user_name = statistic.name;

		//Store the returned data statically
		foreach (GlobalstatsIO_StatisticValues value in statistic.values) {
			bool updated_existing = false;
			for (int i = 0; i < statistic_values.Count; i++) {
				if (statistic_values[i].key == value.key) {
					statistic_values[i] = value;
					updated_existing = true;
					break;
				}
			}
			if (!updated_existing) {
				statistic_values.Add (value);
			}
		}

		return true;
	}

	public GlobalstatsIO_StatisticValues getStatistic(string key)
	{
		for (int i = 0; i < statistic_values.Count; i++) {
			if (statistic_values[i].key == key) {
				return statistic_values [i];
			}
		}
		return null;
	}

	public bool linkStatistic(string id = "")
	{
		if (api_access_token == null || !api_access_token.isValid()) {
			if (!getAccessToken ()) {
				return false;
			}
		}

		// If no id is supplied but we have one stored, reuse it.
		if (id == "" && statistic_id != "")
			id = statistic_id;

		string url = "https://api.globalstats.io/v1/statisticlinks/"+id+"/request";

		string json_payload = "{}";
		byte[] pData = Encoding.ASCII.GetBytes (json_payload.ToCharArray ());

		Dictionary<string,string> headers = new Dictionary<string,string> ();
		headers.Add ("Authorization", "Bearer " + api_access_token.access_token);
		headers.Add ("Content-Type", "application/json");
		headers.Add ("Content-Length", json_payload.Length.ToString ());

		WWW www = new WWW (url, pData, headers);

		while (!www.isDone) {
			System.Threading.Thread.Sleep (100);
		}

		// check for errors
		if (www.error == null) {
			//UnityEngine.Debug.Log ("WWW POST Ok!");
		} else {
			UnityEngine.Debug.Log ("WWW POST Error: " + www.error);
			UnityEngine.Debug.Log ("WWW POST Content: " + www.text);
			return false;
		}

		link_data = JsonUtility.FromJson<GlobalstatsIO_LinkData> (www.text);
		return true;
	}
	
	// numberOfPlayer can be 100 at max.
	public GlobalstatsIO_Leaderboard getLeaderboard(string gtd, int numberOfPlayers) {


		if (numberOfPlayers < 0) {
			return new GlobalstatsIO_Leaderboard();
		} else if(numberOfPlayers > 100) { // Number has to be between 0 and 100
			numberOfPlayers = 100;
		}

		if (api_access_token == null || !api_access_token.isValid())
		{
			if (!getAccessToken())
			{
				return new GlobalstatsIO_Leaderboard();
			}
		}


		string url = "https://api.globalstats.io/v1/gtdleaderboard/" + gtd;

		string json_payload = "{\"limit\":" + numberOfPlayers + "\n}";

		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers.Add("Authorization", "Bearer " + api_access_token.access_token);
		headers.Add("Content-Type", "application/json");
		headers.Add("Cache-Control", "no-cache");
		headers.Add("Content-Length", json_payload.Length.ToString());

		byte[] pData = Encoding.ASCII.GetBytes(json_payload.ToCharArray());


		WWW www = new WWW(url, pData, headers);


		while (!www.isDone)
		{
			System.Threading.Thread.Sleep(100);
		}

		// check for errors
		if (www.error == null)
		{
			//UnityEngine.Debug.Log ("WWW POST Ok!");
		}
		else
		{
			UnityEngine.Debug.Log("WWW POST Error: " + www.error);
			UnityEngine.Debug.Log("WWW POST Content: " + www.text);
			return new GlobalstatsIO_Leaderboard();
		}

		GlobalstatsIO_Leaderboard leaderboard = JsonUtility.FromJson<GlobalstatsIO_Leaderboard>(www.text);

		return leaderboard;
	}
	
}
