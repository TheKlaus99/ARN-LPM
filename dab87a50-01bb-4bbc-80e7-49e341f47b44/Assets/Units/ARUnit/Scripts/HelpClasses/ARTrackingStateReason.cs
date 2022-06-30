﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARUnit
{
	public enum ARTrackingStateReason
	{
		/** Tracking State Reason unsupport. */
		ARTrackingStateReasonUnSupported,

		/** Tracking is not limited. */
		ARTrackingStateReasonNone,

		/** Tracking is limited due to initialization in progress. */
		ARTrackingStateReasonInitializing,

		/** Tracking is limited due to a excessive motion of the camera. */
		ARTrackingStateReasonExcessiveMotion,

		/** Tracking is limited due to a lack of features visible to the camera. */
		ARTrackingStateReasonInsufficientFeatures,

		/** Tracking is limited due to a relocalization in progress. */
		ARTrackingStateReasonRelocalizing
	}


}
