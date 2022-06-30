using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARUnit
{
	public enum ARTrackingState
	{
		ARTrackingStateUnSupported,

		/** Tracking is not available. */
		ARTrackingStateNotAvailable,

		/** Tracking is limited. See tracking reason for details. */
		ARTrackingStateLimited,

		/** Tracking is Normal. */
		ARTrackingStateNormal
	}
}
