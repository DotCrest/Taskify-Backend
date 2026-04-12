using Domain.Models;

namespace Application.Specifications.SpaceSpecifications
{
    public class SpaceByIdSpecification : BaseSpecification<Space>
    {
        public SpaceByIdSpecification(int spaceId) : base(x => x.Id == spaceId)
        {
            AddInclude(x => x.Quests);
        }
    }

}
