using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Search
{

	[CreateAssetMenu(fileName = "SearchDataScriptable", menuName = "SearchData", order = 0)]
	public class DataScriptable : ScriptableObject
	{
		public List<SearchSer> data;
	}
}
