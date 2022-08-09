using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Post.Query.Domain.Entities
{
    [Table("Comments")]
    public class CommentEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public bool Edited { get; set; }

        public Guid PostId { get; set; }

        [JsonIgnore]
        public PostEntity Post { get; set; }
    }
}