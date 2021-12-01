using System;
using System.Text.Json.Serialization;

namespace ContentHubFacade.Models.MClientData
{
	public class ContentHubChangeMessage
	{
		#region Public Properties

		[JsonPropertyName("saveEntityMessage")]
		public SaveEntityMessage SaveEntityMessage { get; set; }

		/* Currently unknown type */
		public object Context { get; set; }

		#endregion
	}

	public class SaveEntityMessage
	{
		#region Public Properties

		public string EventType { get; set; }

		public DateTime TimeStamp { get; set; }

		public bool IsNew { get; set; }

		public string TargetDefinition { get; set; }

		public long TargetId { get; set; }

		public string TargetIdentifier { get; set; }

		public DateTime CreatedOn { get; set; }

		public long UserId { get; set; }

		public long Version { get; set; }

		#endregion
	}
}
