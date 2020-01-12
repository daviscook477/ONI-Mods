using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSerialization;
using UnityEngine;

namespace SideScreen {
	public class Test : KMonoBehaviour, ISaveLoadable {
		[SerializeField]
		[Serialize]
		private List<Tag> acceptedTags = new List<Tag>();

		public List<Tag> AcceptedTags {
			get {
				return acceptedTags;
			}
		}
	}
}
