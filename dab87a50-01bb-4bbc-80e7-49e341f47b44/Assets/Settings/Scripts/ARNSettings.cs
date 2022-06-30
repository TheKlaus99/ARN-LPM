using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARNSettings : MonoBehaviour
{
	public static SettingsScriptable settings;
	public SettingsScriptable currentSettings;

	private void Awake()
	{
		settings = currentSettings;
	}
}
