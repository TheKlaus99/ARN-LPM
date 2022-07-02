//	сreated by immeasurability immeasurability@gmail.com
//	inspired by Neodrop neodrop@unity3d.ru
using UnityEngine;
//	using System.Collections;

using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;


//	при сохранение и загрузки больших объемов данных рекоминдуется использовать бинарный файл!!!
//		от Neodrop http://www.unity3d.ru/distribution/viewtopic.php?f=13&t=1039
//	ВНИМАНИЕ!!! не забудте организовать защиту подмены данных в файле!!!
static public class exml<___type>{

	static public void save(object __object,string __file){

		try{

			StreamWriter __stream=new StreamWriter(__file);

			XmlSerializer __xml=new XmlSerializer(typeof(___type));

			__xml.Serialize(__stream,__object);

			__stream.Close();

		}catch(SerializationException e){

			Debug.Log("error serialize: "+e.Message);

			throw;
			
		}

	}

	public static ___type load(string __file){

		if(!File.Exists(__file)){
			
			return
				default(___type);
		}

		___type __return;

		try{
			
			StreamReader __stream=new StreamReader(__file);
			
			XmlSerializer __xml=new XmlSerializer(typeof(___type));
			
			__return=(___type)__xml.Deserialize(__stream);
			
			__stream.Close();
			
		}catch(SerializationException e){
			
			Debug.Log("error deserialize: "+e.Message);
			
			throw;
		}
		
		return
			__return;
	}

	public static ___type load(___type __return,string __file){

		___type __load=exml<___type>.load(__file);

		if(__load==null){

			return
				__return;
		}

		return
			__load;
	}
}
