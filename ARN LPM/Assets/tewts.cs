using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tewts : MonoBehaviour
{

	IEnumerator Start()
	{
		yield return new WaitForSeconds(2);
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
