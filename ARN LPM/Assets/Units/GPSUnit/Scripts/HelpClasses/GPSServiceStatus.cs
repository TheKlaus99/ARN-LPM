namespace GPSUnit
{
	[System.Serializable]
	public enum GPSServiceStatus
	{

		NonInitializing = 0,
		//
		// Summary:
		//     Location service is stopped.
		Stopped = 1,
		//
		// Summary:
		//     Location service is initializing, some time later it will switch to.
		Initializing = 2,
		//
		// Summary:
		//     Location service is running and locations could be queried.
		Running = 3,
		//
		// Summary:
		//     Location service failed (user denied access to location service).
		Failed = 4,
		//
		// Summary:
		// 		Uuser denied access to location service.
		Disable = 5
	}
}
