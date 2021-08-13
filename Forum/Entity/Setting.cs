using System.ComponentModel.DataAnnotations;

namespace Forum.Entity
{
	public class Setting
	{
		[Key, Required]
		public int Id { get; set; }

		[Required]
		public string SettingKey { get; set; }

		[Required]
		public string Value { get; set; }

		[Required]
		public int UserId { get; set; }
	}
}
