namespace Camply.Shared.Dtos.Vote
{
    public class VoteOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Index { get; set; }
        public bool IsEdited  { get; set; }
        public float VotePercentage  { get; set; }
        public bool IsVoted  { get; set; }
    }
}