using UnityEngine;

namespace DevTest.Utils
{
	/// <summary>
	/// By extending this class, a singleton behavior is inherited. The class can be accesed as static
	/// using its instance (called Instance)
	/// </summary>
	public abstract class Sc_Singleton<T> : MonoBehaviour where T : Component
	{

		private static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<T>();

					if (instance == null)
					{
						GameObject g = new GameObject("Controller");
						instance = g.AddComponent<T>();

					}
				}
				return instance;
			}
		}

		void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
			}
			else
			{
				if (instance != this)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
