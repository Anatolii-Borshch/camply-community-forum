namespace Camply.Shared.Dtos.Vote
{
    public class VoteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsEdited { get; set; }
        public Guid ForumId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorUsername { get; set; } = null!;
        public List<VoteOptionDto> VoteOptions { get; set; } = null!;
    }
}