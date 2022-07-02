using UnityEngine;
using System.Collections;


static public class emd5{

	static public string hash(string __string){

		System.Text.UTF8Encoding __utf=new System.Text.UTF8Encoding();
		byte[] __bytes=__utf.GetBytes(__string);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider __md5=new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] __hash_bytes=__md5.ComputeHash(__bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string __hash="";
		
		for(int __i=0;__i<__hash_bytes.Length;__i++){

			__hash+=System.Convert.ToString(__hash_bytes[__i],16).PadLeft(2,'0');
		}
		
		return
			__hash.PadLeft(32,'0');
	}

}
